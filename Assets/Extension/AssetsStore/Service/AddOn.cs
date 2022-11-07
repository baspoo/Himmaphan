using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AddOn : MonoBehaviour {
	
	public class IEnume : MonoBehaviour {
		//Setting;
		bool autoDestory;


		public Coroutine _StartCorotine( IEnumerator coro ){
			return StartCoroutine (coro);
		}
		public void _Destoey(){
			Destroy (this);
		}
		// WWW download
		Coroutine coroutine = null;
		public void StopAndDeleteIEnumerator(){
			if (coroutine != null)
				StopCoroutine (coroutine);
			if(autoDestory)
				_Destoey ();
		}
		public void internetWWW(string url ,Service.IEnume.IEnumeratorWWWCallback callback){
			coroutine = StartCoroutine(IEInternetWWW(url , callback));
		}
		IEnumerator IEInternetWWW( string url ,Service.IEnume.IEnumeratorWWWCallback callback ){
			WWW www = new WWW (url);
			yield return www;
			callback (www);
			if(autoDestory)
				_Destoey ();
		}
		// WWW Waitting
		public void Waitting(float waiting ,Service.IEnume.IEnumeratorCallback callback){
			coroutine = StartCoroutine(IEWaitting(waiting , callback));
		}
		IEnumerator IEWaitting( float waiting ,Service.IEnume.IEnumeratorCallback callback ){
			if (waiting <= 0.0f)
				yield return new WaitForEndOfFrame ();
			else
				yield return new WaitForSeconds (waiting);
			callback ();
			if(autoDestory)
				_Destoey ();
		}
		// WWW Waitting Loop
		public void WaittingLoop(float waiting  ,int Count ,Service.IEnume.IEnumeratorCallbackLoop callback , float plustime){
			coroutine =  StartCoroutine(IEWaittingLoop(waiting  ,Count ,callback,plustime));
		}
		IEnumerator IEWaittingLoop( float waiting  ,int Count ,Service.IEnume.IEnumeratorCallbackLoop callback , float plustime ){
			for (int index = 0; index < Count; index++) {
				waiting += plustime;
				if (waiting <= 0.0f)
					yield return new WaitForEndOfFrame ();
				else
					yield return new WaitForSeconds (waiting);
				callback (index);
			}
			if(autoDestory)
				_Destoey ();
		}
	}







	public class Timmer : MonoBehaviour {
		public string ID;

		void Update(){
			TimmerUpdate ();
		}
		public float timmerRunnig;
		public float timerMaximum;
		public bool isTimmerActive = false;
		public bool isLoop = false;
		public int roundRunning = 0;
		public int roundMaximum = 0;
        public bool isIgnoreTimeScale;

		Service.Timmer.TimmerCallback m_callback;
		Service.Timmer.TimmerCallbackLoop m_callbackloop;
		Service.Timmer.TimmerCallbackInfinity m_callbackinfinity;


		public delegate void realtime_update (float runtime);
		public realtime_update RealtimeUpdate = null;
		void realtime(){
			if (RealtimeUpdate != null) {
				RealtimeUpdate (timmerRunnig);
			}
		}

		void TimmerUpdate(){
            float delta = isIgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

            if (isTimmerActive) {
				realtime ();
				if (timmerRunnig < timerMaximum) {
					timmerRunnig += delta;
				} else {
					roundRunning++;
					if (roundMaximum != 0) {
						if (roundRunning >= roundMaximum)
							isLoop = false;
					}
					if (!isLoop) {
						isTimmerActive = false;
						StopAndDelete ();
					}
					timmerRunnig = 0.0f;

					if (m_callback != null) 
						m_callback ();
					
					if (m_callbackloop != null)
						m_callbackloop (roundRunning-1);

					if (m_callbackinfinity != null) 
						m_callbackinfinity (this);
				}
			}
		}
		public void ResetAndPlayAgian(){
			timmerRunnig = 0.0f;
			roundRunning = 0;
			isTimmerActive = true;
		}
		public void StopAndDelete(){
			isTimmerActive = false;
			Destroy (this);
		}
		public void TimmerRoundStart(float waiting  , int countloop, Service.Timmer.TimmerCallbackLoop callback){
			timerMaximum = waiting;
			m_callbackloop = callback;
			isLoop = true;
			roundMaximum = countloop;
			isTimmerActive = true;

			if (isLoop && roundMaximum == 0) 
			{
				Debug.LogError("!!! TimmerRoundStart roundMaximum is 0 == inityLoop!!!");
			}

		}
		public void TimmerInfinityStart(float waiting  , Service.Timmer.TimmerCallbackInfinity callback){
			timerMaximum = waiting;
			m_callbackinfinity = callback;
			isLoop = true;
			roundMaximum = 0;
			isTimmerActive = true;
		}
		public void TimmerStart(float waiting  , Service.Timmer.TimmerCallback callback){
			timerMaximum = waiting;
			m_callback = callback;
			isLoop = false;
			roundMaximum = 0;
			isTimmerActive = true;
		}



		public void TimmerStep(){
			timestepdatas = new List<timestepdata> ();
		}
		public class timestepdata{
			public float waiting;
			public Service.Timmer.TimmerCallback callback;
		}
		int indextimestep = 0;
		List<timestepdata> timestepdatas;
		public timestepdata AddTimmerStep(float waiting  , Service.Timmer.TimmerCallback callback){
			timestepdata timestep = new timestepdata ();
			timestep.waiting = waiting;
			timestep.callback = ()=>{
				callback();
				indextimestep++;
				if(indextimestep<timestepdatas.Count)
					StartTimmerStep();
				else
				{
					isTimmerActive = false;
					StopAndDelete ();
				}
			};
			timestepdatas.Add (timestep);
			return timestep;
		}
		public void StartTimmerStep( ){

			if (timestepdatas.Count == 0)
				return;

			timerMaximum = timestepdatas[indextimestep].waiting;
			m_callback = timestepdatas[indextimestep].callback;;
			isLoop = true;
			roundMaximum = timestepdatas.Count;
			isTimmerActive = true;
		}




		public void ForceFinish(){
			if (m_callback != null) 
				m_callback ();
			
			if (m_callbackloop != null)
				m_callbackloop (roundRunning);
			
			if (m_callbackinfinity != null) 
				m_callbackinfinity (this);
			StopAndDelete ();
		}
	}
}









