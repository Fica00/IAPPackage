using System;
using System.Collections;
using Unity.Services.Core;
using UnityEngine;

namespace IAPProducts
{
    public static class InitializeGamingServices
    {
        private static Action<bool,string> callBack;
        private static MonoBehaviour monoBehaviour;
        private static string initializationMessage;
        private static int initializationResult;

        public static void Init(MonoBehaviour _monoBehaviour,Action<bool,string> _callBack)
        {
            initializationMessage = string.Empty;
            initializationResult = 0;
            monoBehaviour = _monoBehaviour;
            callBack = _callBack;
            switch (UnityServices.State)
            {
                case ServicesInitializationState.Initialized:
                    callBack?.Invoke(true,string.Empty);
                    return;
                case ServicesInitializationState.Initializing:
                    break;
                case ServicesInitializationState.Uninitialized:
                    Initialize();
                    break;
            }
        }
        
        private static void Initialize()
        {
            monoBehaviour.StartCoroutine(CheckForInitializationResult());
            try
            {
                var _options = new InitializationOptions();
                UnityServices.InitializeAsync(_options).ContinueWith(_task => initializationResult=1);
            }
            catch (Exception _exception)
            {
                initializationResult = -1;
                initializationMessage = _exception.Message;
            }
        }

        private static IEnumerator CheckForInitializationResult()
        {
            while (true)
            {
                if (initializationResult!=0)
                {
                    callBack?.Invoke(initializationResult==1,initializationMessage);
                    yield break;
                }
                yield return null;
            }
        }
    }
}