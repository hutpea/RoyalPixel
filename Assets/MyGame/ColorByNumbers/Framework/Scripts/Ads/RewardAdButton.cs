using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
	[RequireComponent(typeof(Button))]
	public class RewardAdButton : MonoBehaviour
	{
		[System.Serializable]
		public class OnRewardAdEvent : UnityEngine.Events.UnityEvent {}

		#region Inspector Variables
		
		[SerializeField] private OnRewardAdEvent onRewardAdShowing	= null;
		[SerializeField] private OnRewardAdEvent onRewardAdClosed	= null;
		[SerializeField] private OnRewardAdEvent onRewardGranted	= null;

		#endregion

		#region Unity Methods

		private void Start()
		{
			gameObject.SetActive(false);

			bool areRewardAdsEnabled = MobileAdsManager.Instance.AreRewardAdsEnabled;

			if (areRewardAdsEnabled)
			{
				UpdateUI();

				MobileAdsManager.Instance.OnRewardAdLoaded	+= UpdateUI;
				MobileAdsManager.Instance.OnAdsRemoved		+= OnAdsRemoved;

				gameObject.GetComponent<Button>().onClick.AddListener(OnClicked);
			}
		}

		#endregion

		#region Private Methods

		private void UpdateUI()
		{
			gameObject.SetActive(MobileAdsManager.Instance.RewardAdState == AdNetworkHandler.AdState.Loaded);
		}

		private void OnAdsRemoved()
		{
			MobileAdsManager.Instance.OnRewardAdLoaded	-= UpdateUI;
			MobileAdsManager.Instance.OnAdsRemoved		-= OnAdsRemoved;

			gameObject.SetActive(false);
		}

		private void OnClicked()
		{
			gameObject.SetActive(false);

			onRewardAdShowing?.Invoke();

			MobileAdsManager.Instance.ShowRewardAd(OnRewardAdClosed, OnRewardAdGranted);
		}

		private void OnRewardAdGranted(string id, double amount)
		{
			onRewardGranted?.Invoke();
		}

		private void OnRewardAdClosed()
		{
			onRewardAdClosed?.Invoke();
		}

		#endregion
	}
}
