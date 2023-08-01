using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class IAPPriceUpdate : MonoBehaviour
{
    public UnityEvent<string> onPriceUpdated;

    [SerializeField]
    private string defaultPriceString;

    [SerializeField]
    private string productId;
    [SerializeField]
    private PlaceHolderText placeHolder;

    // Start is called before the first frame update
    private void Start()
    {
        onPriceUpdated?.Invoke(defaultPriceString);
        if (CodelessIAPStoreListener.initializationComplete)
        {
            FetchPrice();
        }
        else {
            StartCoroutine(IEWaitingForIAPInitialization());
        }
    }

    private IEnumerator IEWaitingForIAPInitialization() {
        while (!CodelessIAPStoreListener.initializationComplete)
        {
            yield return null;
        }
        FetchPrice();
    }

    private void FetchPrice() {
        Product product = CodelessIAPStoreListener.Instance.GetProduct(productId);
        if (product != null) {
            string localizedPriceString = product.metadata.localizedPriceString;
            if (localizedPriceString != null)
            {
                onPriceUpdated?.Invoke(localizedPriceString);

            }
        }
    }
}
