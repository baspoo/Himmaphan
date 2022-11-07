using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TaskService
{


    [System.Serializable]
    public class JobService
    {
        /// <summary>
        /// limit = 0 : run all task together on start (unlimit).
        /// limit = 1 : run one by one task.
        /// limit = 2,3,4,5,..N : run together by count task & wait finish.
        /// </summary>
        public int limit = 3;
        //public long counter = 0;
        [SerializeField]
        int runCount = 0;
        public List<JobData> tasks = new List<JobData>();
        [System.Serializable]
        public class JobData
        {
            public string jobName;
            //public long counter;
            [SerializeField]
            public bool run;
            public System.Action action;
            public void OnRun()
            {
                run = true;
                action?.Invoke();
            }
        }




        public void AddJob(string job, System.Action action)
        {
            tasks.Add(new JobData()
            {
                jobName = job,
                action = action
                //counter = counter
            }); ;
            //counter++;
            Next();
        }
        public void Done()
        {
            runCount--;
            if (limit != 0)
                Next();
        }


        void Next()
        {
            if (tasks.Count == 0)
                return;

            var job = tasks[0];
            if (job.run)
            {
                Next();
                return;
            }
            if (runCount < limit || limit == 0)
            {
                runCount++;
                job.OnRun();
                tasks.Remove(job);
                Next();
            }
            else
            {
                return;
            }
        }





    }


















    class FunctionExample
    {
        Function onEventTest = new Function();
        void Test()
        {

            //--> Add Event
            onEventTest.add("test_1", () =>
            {
                Debug.Log("start & done");
            });
            onEventTest.add("test_2", () =>
            {
                Debug.Log("start & done");
            });
            //--> Call Event
            onEventTest.call("test_1");
            onEventTest.callall();





            //--> Add Event Await
            //--> Add normal
            onEventTest.add("test_1", () =>
            {
                Debug.Log("start");
                3.Wait(() =>
                {
                    Debug.Log("done");
                });
                // <-- await = false (auto Done)
            });
            //--> Add await
            onEventTest.add("test_2", () =>
            {
                Debug.Log("start");
                3.Wait(() =>
                {
                    Debug.Log("done");
                    onEventTest.jobDone(); // step1 <-- jobDone()
                });
            }).await = true; // step2 <-- await = true
                             //--> Call Event
            onEventTest.callall();





            //--> Add Event Return
            onEventTest.add("test_1", () =>
            {
                return 3 + 7;
            });
            //--> Call Event Return
            var value_10 = (int)onEventTest.call("test_1");






            //--> Add Event Parameter
            onEventTest.add("test_1", (parameter) =>
            {
                var reault = 3 + (int)parameter;
                Debug.Log($"{reault} == 13");
            });
            //--> Call Event Send Parameter
            onEventTest.call("test_1", 10);





            //--> Add Event Parameter & Return
            onEventTest.add("test_1", (parameter) =>
            {
                return 3 + (int)parameter;
            });
            //--> Call Event Send Parameter & Return
            var value_13 = (int)onEventTest.call("test_1", 10);



            //--> Add Event Arguments[ ]
            onEventTest.add("test_1", (parameter) =>
            {
                var arguments = (object[])parameter;

                Debug.Log($"arguments[0]:{(string)arguments[0]}");  // <== "xxx"
                Debug.Log($"arguments[1]:{(int)arguments[1]}");     // <== 200
                Debug.Log($"arguments[2]:{(string)arguments[2]}");  // <== "baspoo"
                Debug.Log($"arguments[3]:{((Service.Formula)arguments[3])["name"]}");   // <== "ritichai"
                Debug.Log($"arguments[4]:{(bool)arguments[4]}");    // <== false

            });
            //--> Call Event Send Arguments[ ]
            onEventTest.call("test_1", new object[5] { "xxx", 200, "baspoo", new Service.Formula("name", "ritichai"), false });








            //--> Save & Load Version
            onEventTest.add("test_1", () => { });
            onEventTest.add("test_2", () => { });
            onEventTest.add("test_3", () => { });
            onEventTest.saveVersion("save_1");
            onEventTest.clear();


            onEventTest.add("test_4", () => { });
            onEventTest.add("test_5", () => { });
            onEventTest.add("test_6", () => { });
            onEventTest.saveVersion("save_2");
            onEventTest.clear();


            onEventTest.loadVersion("save_1"); // --> {test_1,test_2,test_3}
            onEventTest.loadVersion("save_2"); // --> {test_4,test_5,test_6}



            //--> Save & Load Version (Dynamic)
            onEventTest.add("test_1", () => { });
            onEventTest.add("test_2", () => { });
            onEventTest.add("test_3", () => { });
            onEventTest.saveVersion("save_1");

            onEventTest.add("test_4", () => { });
            onEventTest.add("test_5", () => { });
            onEventTest.saveVersion("save_2");


            onEventTest.loadVersion("save_1"); // --> {test_1,test_2,test_3}
            onEventTest.loadVersion("save_2"); // --> {test_1,test_2,test_3,test_4,test_5}

        }
    }

    [System.Serializable]
    public class Function
    {


        bool onceTimeAlway = false;
        bool repleteFunctionName = false;

        public Function(bool onceTimeAlway = false, bool repleteFunctionName = false)
        {
            this.onceTimeAlway = onceTimeAlway;
            this.repleteFunctionName = repleteFunctionName;
        }
        [System.Serializable]
        public class FunctionData
        {
            public string function;
            public bool onceTime;
            public System.Func<object, object> exe;
            public bool await;


            public bool isRemove => onceTime || !isavalible;

            public bool isavalible => instancenotnull ? instanceObj != null : true;
            private bool instancenotnull;
            private GameObject instanceObj;
            public FunctionData SetInstance(GameObject instance)
            {
                this.instancenotnull = true;
                this.instanceObj = instance;
                return this;
            }
        }
        public int Count => functionData.Count;
        [SerializeField] List<FunctionData> functionData = new List<FunctionData>();
        Dictionary<string, List<FunctionData>> m_version;
        public void saveVersion(string version)
        {
            if (m_version == null) m_version = new Dictionary<string, List<FunctionData>>();
            var clone = new List<FunctionData>();
            functionData.ForEach(x => clone.Add(x));
            if (m_version.ContainsKey(version))
                m_version[version] = clone;
            else
                m_version.Add(version, clone);
        }
        public void loadVersion(string version)
        {
            if (m_version == null)
                return;
            if (m_version.ContainsKey(version))
                functionData = m_version[version];
        }

        JobService job;
        public void jobDone()
        {
            if (job != null)
                job.Done();
        }

        FunctionData get(string function)
        {
            return functionData.Find(x => x.function == function);
        }
        public void des(string function)
        {
            functionData.RemoveAll(x => x.function == function);
        }

        FunctionData put(string function, System.Func<object, object> exe, bool onec)
        {
            FunctionData f = new FunctionData()
            {
                function = function,
                exe = exe,
                onceTime = onec || onceTimeAlway
            };
            if (!repleteFunctionName)
                des(function);
            functionData.Add(f);
            return f;
        }
        public FunctionData add(string function, System.Func<object, object> fuc, bool once = false)
        {
            return put(function, fuc, once);
        }
        public FunctionData add(string function, System.Action fuc, bool once = false)
        {
            return put(function, (data) =>
            {
                fuc.Invoke();
                return null;
            }, once);
        }
        public FunctionData add(string function, System.Action<object> fuc, bool once = false)
        {
            return put(function, (data) =>
            {
                fuc.Invoke(data);
                return null;
            }, once);
        }
        public FunctionData add(string function, System.Func<object> fuc, bool once = false)
        {
            return put(function, (data) =>
            {
                return fuc.Invoke();
            }, once);
        }
        public object call(string function, object data = null)
        {
            var fuct = get(function);
            if (fuct == null)
                return null;
            if (fuct.isRemove)
                functionData.Remove(fuct);
            return execute(fuct, data);
        }
        public JobService callall(object data = null)
        {
            if (functionData.Count == 0)
                return job;

            bool sequentialTask = functionData.Find(x => x.await) != null;

            if (sequentialTask)
            {
                job = new JobService() { limit = 1 };
                foreach (var funt in functionData)
                {
                    job.AddJob(funt.function, () =>
                    {
                        execute(funt, data);
                        if (!funt.await)
                            Service.IEnume.Wait(0, () => { job.Done(); });
                    });
                }
                functionData.RemoveAll(x => x.isRemove);
            }
            else
            {
                foreach (var funt in functionData)
                {
                    execute(funt, data);
                }
                functionData.RemoveAll(x => x.isRemove);
            }

            return job;
        }
        object execute(FunctionData fuct, object data = null)
        {
            if (fuct != null && fuct.isavalible)
            {
                return fuct.exe.Invoke(data);
            }
            else
            {
                return null;
            }
        }
        public void clear()
        {
            functionData.Clear();
        }
    }





}




