using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CakeRandom  {
	public static int ChooseRandomIndex( List<double> percents){
	


		//** Sum %.
		double sum = 0;
		foreach (double percent in percents) {
			sum += percent;
		}

		//**  calculate all new percent.
		double devide = sum / 100.0;
		List<double> new_percents = new List<double> ();
		foreach (double percent in percents) {
			double per = percent / devide;
			new_percents.Add (per);
		}

		//** Random Number
		double randomNumber = Random.Range(0,100);

		//** calculate Range.
		double range = 0;
		//int range_index = 0;
		List<double> rangeList = new List<double> ();
		foreach (double percent in new_percents) {
			//range_index++;
			range += percent;
			rangeList.Add (range);
			//Debug.Log ("["+range_index.ToString() + "] " + range);
		}


		//** choose index
		int index = 0;
		foreach (double select_range in rangeList) {
			if (select_range >= randomNumber)
				return index;
			else
				index++;
		}

		return 0;
	}
}
