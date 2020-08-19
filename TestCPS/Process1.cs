using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestCPS
{
	class Process1
	{
		static ReaderWriterLockSlim rlLock = new ReaderWriterLockSlim();
		static string synchro_obj = "dummy string";
		static Semaphore synchro_sema;
		static SemaphoreSlim synchro_sema_slim;

		//public static async Task TestProcess()
		public static void TestProcess()
		{
			Console.WriteLine("Process:START");
			//System.Threading.Thread.Sleep(1000);

			//await TestClient.Test1();
			//Task t = Task.Run(() =>TestClient.Test1());
			Task t = Task.Run(TestClient.Test1);
			//TestClient.Test1();

			Console.WriteLine("Process:After Test");
			//t.Wait();
			Console.WriteLine("■");
			//for(int i = 0;i < 3;i++) {
			//	Console.WriteLine("■");
			//	System.Threading.Thread.Sleep(1000);
			//	//System.Threading.Tasks.Task.Delay(1000);
			//	//t.Wait();
			//	//while(!t.IsCompleted){
			//	//    t.Wait(100);
			//	//Console.WriteLine("■");
			//}
			//Console.WriteLine("");

			Console.WriteLine("Process:END");

			
		}

		public static void TestProcess2()
		{
			for(int i = 0;i < 50;i++){
				Console.WriteLine("In for : {0}",i);
				Task t = new Task((num) =>{
							//int num = i;
							//Console.WriteLine("In run : {0}",i);
							//rlLock.AcquireWriterLock(-1);
							rlLock.EnterWriteLock();
							Process1.DummyProc(num.ToString());
							//rlLock.ReleaseWriterLock();
							rlLock.ExitWriteLock();
							},i);
				t.Start();
				//while(t.Status != TaskStatus.Running){
				//while(t.Status != TaskStatus.RanToCompletion){
				//	System.Threading.Thread.Sleep(100);
				//	Console.WriteLine("Status : {0}",t.Status);
				//}
			}
		}
		public static void TestProcess3()
		{
			for(int i = 0;i < 10;i++){
				Console.WriteLine("In for : {0}",i);
				Monitor.Enter(synchro_obj);
				Task t = new Task((num) =>{
								Monitor.Enter(synchro_obj);
								Process1.DummyProc3(num.ToString());
								try{
									Monitor.Exit(synchro_obj);
								}catch(Exception ex){
									Console.WriteLine(ex);
								}
							},i);
				t.Start();
				Monitor.Exit(synchro_obj);
				Console.WriteLine("Out for : {0}",i);
				System.Threading.Thread.Sleep(1000);
			}
		}
		public static void TestProcess4()
		{
			synchro_sema = new Semaphore(1,1);

			for(int i = 0;i < 10;i++){
				Console.WriteLine("In for : {0}",i);
				synchro_sema.WaitOne();
				Monitor.Enter(synchro_obj);
				Task t = new Task((num) =>{
								Process1.DummyProc3(num.ToString());
								synchro_sema.Release();
							},i);
				t.Start();
				Console.WriteLine("Out for : {0}",i);
				//System.Threading.Thread.Sleep(1000);
			}
		}
		public static void TestProcess5()
		{
			synchro_sema_slim = new SemaphoreSlim(1,1);

			for(int i = 0;i < 10;i++){
				Console.WriteLine("In for : {0}",i);
				synchro_sema_slim.Wait();
				Monitor.Enter(synchro_obj);
				Task t = new Task((num) =>{
								Process1.DummyProc3(num.ToString());
								synchro_sema_slim.Release();
							},i);
				t.Start();
				Console.WriteLine("Out for : {0}",i);
				System.Threading.Thread.Sleep(2000);
			}
		}
		public static void DummyProc(string num)
		{
			
			Console.WriteLine("DummyProc({0}):{1}",Thread.CurrentThread.ManagedThreadId,num);
			for(int i = 0;i < 3;i++){
				System.Threading.Thread.Sleep(1000);
				Console.WriteLine("★");
			}
		}
		public static void DummyProc3(string num)
		{
			//Monitor.Enter(synchro_obj);
			Console.WriteLine("DummyProc3({0}):{1}",Thread.CurrentThread.ManagedThreadId,num);


			for(int i = 0;i < 3;i++){
				System.Threading.Thread.Sleep(1000);
				Console.WriteLine("★");
			}

			//Monitor.Exit(synchro_obj);
		}
	}
}
