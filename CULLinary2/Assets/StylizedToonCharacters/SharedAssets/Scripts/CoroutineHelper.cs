using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Nicoplv.Characters
{
	public class CoroutineHelper : MonoBehaviour
	{
        #region Variables

        private static CoroutineHelper instance = null;

        #endregion

        #region Methods

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            GameObject b_gameObject = new GameObject(typeof(CoroutineHelper).ToString());
            instance = b_gameObject.AddComponent<CoroutineHelper>();
            DontDestroyOnLoad(b_gameObject);
        }

        public static Coroutine Start(IEnumerator _routine)
        {
            return instance.StartCoroutine(_routine);
        }

        #endregion
    }
}