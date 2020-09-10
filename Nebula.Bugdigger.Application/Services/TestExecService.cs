using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace Nebula.Bugdigger
{
    public class TestExecService : ITransientDependency
    {
        private Dictionary<string, byte[]> DataRecvBuf = new Dictionary<string, byte[]>();
        private ConnectionMultiplexer _connectionMultiplexer;
        private ISubscriber _iSubscriber;
        private IDatabase _iDatabase;
        private IObjectMapper _objectMapper;
        public TestExecService(ConnectionMultiplexer connectionMultiplexer, IObjectMapper objectMapper)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _iSubscriber = connectionMultiplexer.GetSubscriber();
            _iDatabase = connectionMultiplexer.GetDatabase();
            _objectMapper = objectMapper;
        }
        public async Task<TestResultDto> RunTestScript(UseCase testScript)
        {
            var testResultViewModel = new TestResultDto();
            testResultViewModel.TestCaseId = testScript.id;
            testResultViewModel.TestTime = DateTime.Now.ToShortDateString().ToString();

            BeginRecv();

            for (int index = 0; index < testScript.TestStep.Count; index++)
            {
                var testStep = testScript.TestStep[index];
                var stepResult = new TestStepResultDto();
                stepResult.Expect = testStep.Param.Data;
                stepResult.Actual = testStep.Param.Data;
                stepResult.Param = _objectMapper.Map<Param, TestDataDto>(testStep.Param);

                if (testStep.Operate == Operate.Send)
                {
                    stepResult.DataDirection = "Send";
                }
                else if (testStep.Operate == Operate.Recv)
                {
                    stepResult.DataDirection = "Recv";
                }


                if (testStep.Delay > 0)
                {
                    await Task.Delay(testStep.Delay);
                }
                if (testStep.Operate == Operate.Send)
                {
                    SendData(testStep.GetSendData());
                    stepResult.IsPass = true;


                    var content = testStep.BinStringDataToByteList(testStep.GetData()).ToArray();
                    StringBuilder data = new StringBuilder(content.Length * 8);
                    foreach (byte b in content)
                    {
                        data.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
                    }

                    foreach (var field in testStep.ICD)
                    {
                        var icdResult = new ICDResultDto();
                        var offset = field.Param.StartByte * 8 + field.Param.StartBit;
                        icdResult.Expect = data.ToString().Substring(offset, field.Param.Length);
                        icdResult.Actual = icdResult.Expect;
                        icdResult.Remark = field.Remark;
                        icdResult.IsPass = true;
                        stepResult.ICD.Add(icdResult);
                    }
                    for (int icdindex = 0; icdindex < stepResult.ICD.Count; icdindex++)
                    {
                        var icd = stepResult.ICD[icdindex];
                        stepResult.Remark += "[TestSetp " + (index + 1) + ":ICD Field " + (icdindex + 1) + "]" + icd.Remark + " : 通过\n";
                    }
                    stepResult.Remark += "[TestSetp " + (index + 1) + "]" + testStep.Remark;
                }
                else if (testStep.Operate == Operate.Recv)
                {
                    string header = Convert.ToString((byte)testStep.Param.BusType, 16).PadLeft(2, '0');
                    header += Convert.ToString(testStep.Param.NetNo, 16).PadLeft(2, '0');
                    header += Convert.ToString(testStep.Param.DesChannel1, 16).PadLeft(8, '0');
                    header += Convert.ToString(testStep.Param.DesChannel2, 16).PadLeft(8, '0');
                    Console.WriteLine("Compare：" + header);
                    if (DataRecvBuf.ContainsKey(header))
                    {
                        stepResult.Actual = ByteToHexString(DataRecvBuf[header]);

                        if (testStep.ICD.Count == 0)
                        {
                            if (stepResult.Actual == testStep.GetData()) stepResult.IsPass = true;
                            else stepResult.IsPass = false;
                        }
                        else
                        {
                            StringBuilder result = new StringBuilder(DataRecvBuf[header].Length * 8);
                            foreach (byte b in DataRecvBuf[header])
                            {
                                result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
                            }
                            var content = testStep.BinStringDataToByteList(testStep.GetData()).ToArray();
                            StringBuilder data = new StringBuilder(content.Length * 8);
                            foreach (byte b in content)
                            {
                                data.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
                            }
                            foreach (var field in testStep.ICD)
                            {
                                var icdResult = new ICDResultDto();
                                icdResult.Remark = field.Remark;
                                var offset = field.Param.StartByte * 8 + field.Param.StartBit;
                                icdResult.Expect = data.ToString().Substring(offset, field.Param.Length);
                                icdResult.Actual = result.ToString().Substring(offset, field.Param.Length);
                                if (icdResult.Expect.Equals(icdResult.Actual))
                                {
                                    icdResult.IsPass = true;
                                }
                                else icdResult.IsPass = false;
                                stepResult.ICD.Add(icdResult);
                            }
                            stepResult.IsPass = true;
                            for (int icdindex = 0; icdindex < stepResult.ICD.Count; icdindex++)
                            {
                                var icd = stepResult.ICD[icdindex];
                                if (!icd.IsPass)
                                {
                                    stepResult.IsPass = false;
                                    stepResult.Remark += "[TestSetp " + (index + 1) + ":ICD Field " + (icdindex + 1) + "]" + icd.Remark + " : 不通过\n";
                                }
                                else
                                {
                                    stepResult.Remark += "[TestSetp " + (index + 1) + ":ICD Field " + (icdindex + 1) + "]" + icd.Remark + " : 通过\n";
                                }
                            }
                            stepResult.Remark += "[TestSetp " + (index + 1) + "]" + testStep.Remark;
                        }

                    }
                    else
                    {
                        stepResult.Actual = "未接收到数据";
                        stepResult.IsPass = false;
                        stepResult.Remark += "[TestSetp " + (index + 1) + "]" + testStep.Remark + "  未接收到数据";
                    }
                    DataRecvBuf.Remove(header);
                }


                testResultViewModel.TestStepResults.Add(stepResult);
            }

            testResultViewModel.IsPass = "通过";
            foreach (var stepResult in testResultViewModel.TestStepResults)
            {

                if (!stepResult.IsPass)
                {
                    testResultViewModel.IsPass = "不通过";
                    testResultViewModel.Result += stepResult.Remark + " : 不通过\n";
                }
                else
                {
                    testResultViewModel.Result += stepResult.Remark + " : 通过\n";
                }
            }
            return testResultViewModel;
        }

        private void SendData(byte[] data)
        {
            var sendData = Convert.ToBase64String(data);
            var key = data[4].ToString() + "_" + data[5].ToString() + "_";
            key += getInt(data, 6).ToString() + "_";
            key += getInt(data, 10).ToString() + "_";
            key += getInt(data, 14).ToString() + "_";
            key += getInt(data, 18).ToString();
            _iSubscriber.Publish("BusSim", sendData);
            var value = Convert.ToBase64String(data.Skip(40).ToArray());
            _iDatabase.StringSet(key, value);
        }

        private int getInt(byte[] data, int index)
        {
            string value = "";
            for (int i = index; i < index + 4; i++)
            {
                value += Convert.ToString(data[i], 16).PadLeft(2, '0');
            }
            return Convert.ToInt32(value, 16);
        }

        private void DataRecvCallBack(byte[] data)
        {
            SetLastData(data);
        }

        private void BeginRecv()
        {
            _iSubscriber.Subscribe("BusSim", (channel, value) => {
                var data = Convert.FromBase64String(value);
                DataRecvCallBack(data);
            });
        }

        private void EndRecv()
        {
            _iSubscriber.UnsubscribeAll();
        }


        private void SetLastData(byte[] recvData)
        {
            string header = Convert.ToString(recvData[4], 16).PadLeft(2, '0');
            header += Convert.ToString(recvData[5], 16).PadLeft(2, '0');
            for (int i = 14; i < 22; i++) header += Convert.ToString(recvData[i], 16).PadLeft(2, '0');
            Console.WriteLine("Recived：" + header);
            DataRecvBuf[header] = recvData.Skip(40).ToArray();
        }

        private string ByteToHexString(byte[] body)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in body)
            {
                string s = Convert.ToString(b, 16).PadLeft(2, '0');
                stringBuilder.Append(s);
            }
            return stringBuilder.ToString();

        }
    }
}
