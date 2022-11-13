using System.Collections;
using UnityEngine;

namespace UltraMan.Extensions
{
    public static class MonobehaviorExtension
    {
        /// <summary> 
        /// Starts an interuptable coroutine.
        /// A reference to a persistant instance recorder must be passed.
        /// The coroutine will start and will record the coroutine instance inside 'instance'.
        /// If another coroutine is called with the same instance recorder, the previous couroutine will stop automatically.
        /// </summary>
        /// <param name="o"> This monobehavior. </param>
        /// <param name="coroutine"> The coroutine to perform. </param>
        /// <param name="instance"> The recorder to record this courtine instance upon. </param>
        /// <returns> Coroutine of new routine. Can be yield returned. </returns>
        public static Coroutine StartInterruptableCoroutine(this MonoBehaviour o, IEnumerator coroutine, ref IEnumerator instance)
        {
            /* Stop An Instance of This Routine */
            if (instance != null)
                o.StopCoroutine(instance);

            /* Records The Instance */
            instance = coroutine;

            /* Performs The Routine */
            return o.StartCoroutine(instance);

        }
    }
}
