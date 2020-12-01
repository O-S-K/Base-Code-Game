using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace OSK
{
    public class UIController : MonoBehaviour
    {
        public delegate void Callback0();
        public static IEnumerator WaitForRealSeconds(float time)
        {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + time)
            {
                yield return null;
            }
        }

        public static IEnumerator MoveLocal(GameObject obj, Vector3 to, float duration, Callback0 fn)
        {
            float elapsed = 0;
            Vector3 from = obj.transform.localPosition;
            while (elapsed <= duration && obj.activeSelf)
            {
                obj.transform.localPosition = Vector3.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            obj.transform.localPosition = to;
            if (fn != null) fn();
        }

        public static IEnumerator MoveLocalWaiter(GameObject obj, Vector3 to, float duration, float time)
        {
            yield return new WaitForSeconds(time);
            float elapsed = 0;
            Vector3 from = obj.transform.localPosition;
            while (elapsed <= duration)
            {
                Vector3 current = Vector3.Lerp(from, to, elapsed / duration);
                obj.transform.localPosition = new Vector3(current.x, obj.transform.localPosition.y, current.z);
                elapsed += Time.deltaTime;
                yield return null;
            }
            obj.transform.localPosition = new Vector3(to.x, obj.transform.localPosition.y, to.z);
        }

        public static IEnumerator Move(GameObject obj, Vector3 to, float duration, Callback0 fn)
        {
            float elapsed = 0;
            Vector3 from = obj.transform.position;
            while (elapsed <= duration)
            {
                obj.transform.position = Vector3.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            obj.transform.position = to;
            if (fn != null) fn();
        }

        public static IEnumerator MoveAnchor(RectTransform rec, Vector2 to, float duration, Callback0 fn)
        {
            float elapsed = 0;
            Vector3 from = rec.anchoredPosition;
            while (elapsed <= duration)
            {
                elapsed += Time.deltaTime;
                rec.anchoredPosition = Vector2.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            if (fn != null) fn();
        }

        public static IEnumerator Scale(Transform trans, Vector3 to, float duration, Callback0 fn = null)
        {
            float elapsed = 0;
            Vector3 from = trans.localScale;
            while (elapsed <= duration)
            {
                elapsed += Time.deltaTime;
                trans.localScale = Vector3.Lerp(from, to, quartEaseInOut(elapsed / duration));
                yield return null;
            }
            if (fn != null) fn();
        }

        public static IEnumerator FadeOut(Image image, float duration, Callback0 fn)
        {

            float elapsed = 0;
            while (elapsed <= duration)
            {
                elapsed += Time.deltaTime;
                Color currenColor = image.color;
                image.color = new Color(currenColor.r, currenColor.g, currenColor.b, Mathf.Lerp(1, 0, elapsed / duration));
                yield return null;
            }
            if (fn != null) fn();
        }

        public static IEnumerator DeActive(GameObject obj, float duration, Callback0 fn)
        {

            yield return new WaitForSeconds(duration);
            obj.SetActive(false);
            if (fn != null) fn();
        }

        public static IEnumerator StartShakeGO(GameObject obj, float shakeRange, float duration)
        {
            float shakeTimeCounter = 0;
            var originalPos = obj.transform.position;
            while (shakeTimeCounter < duration)
            {
                shakeTimeCounter += Time.unscaledDeltaTime;
                float randX = RandomShakeRange(shakeRange);
                float randY = RandomShakeRange(shakeRange);
                float randZ = RandomShakeRange(shakeRange);
                var range = new Vector3(randX, randY, randZ);
                obj.transform.position = originalPos + range;
                yield return null;
            }
            obj.transform.position = originalPos;
        }

        static float RandomShakeRange(float range)
        {
            return Random.Range(-Mathf.Abs(range), Mathf.Abs(range));
        }

        public static float quartEaseInOut(float time)
        {
            time = time * 2;
            if (time < 1) return 0.5f * time * time * time * time;

            time -= 2;
            return -0.5f * (time * time * time * time - 2);
        }

        public static IEnumerator MoveLocalSpecial(GameObject obj, Vector3 to, float duration)
        {
            float elapsed = 0;
            Vector3 from = obj.transform.localPosition;
            while (elapsed <= duration)
            {
                obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, Mathf.Lerp(from.y, to.y, elapsed / duration), obj.transform.localPosition.z);
                elapsed += Time.deltaTime;
                yield return null;
            }
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, to.y, obj.transform.localPosition.z);
        }

        public static IEnumerator RotateLocalZ(GameObject obj, Vector3 to, float duration)
        {
            float elapsed = 0;
            Vector3 from = obj.transform.localEulerAngles;
            while (elapsed <= duration)
            {
                elapsed += Time.deltaTime;
                obj.transform.localEulerAngles = Vector3.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            obj.transform.localEulerAngles = to;
        }

        public static IEnumerator RotateLoop(GameObject obj, Vector3 axis, float angle)
        {
            while (true && obj.activeSelf)
            {
                obj.transform.Rotate(axis, angle);
                yield return new WaitForSeconds(0.01f);
            }
        }

        public static IEnumerator FadeOutCanvas(GameObject obj, float duration)
        {
            float elapsed = 0;
            Graphic graphic = obj.GetComponent<Graphic>();
            Color color = graphic.color;
            graphic.color = new Color(color.r, color.g, color.b, 1);

            while (elapsed <= duration)
            {
                elapsed += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(1, 0, elapsed / duration);
                graphic.color = color;
                yield return null;
            }
            graphic.color = new Color(color.r, color.g, color.b, 0);
        }

        public static void FadeOutCanvasNow(GameObject obj)
        {
            Graphic graphic = obj.GetComponent<Graphic>();
            Color color = graphic.color;
            graphic.color = new Color(color.r, color.g, color.b, 0);
        }

        public static IEnumerator FadeInCanvas(GameObject obj, float duration)
        {
            float elapsed = 0;
            Graphic graphic = obj.GetComponent<Graphic>();
            Color color = graphic.color;
            graphic.color = new Color(color.r, color.g, color.b, 0);

            while (elapsed <= duration)
            {
                elapsed += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(0, 1, elapsed / duration);
                graphic.color = color;
                yield return null;
            }
            graphic.color = new Color(color.r, color.g, color.b, 1);
        }

        public static IEnumerator FadeInCanvasTo(GameObject obj, float duration, float a)
        {
            float elapsed = 0;
            Graphic graphic = obj.GetComponent<Graphic>();
            Color color = graphic.color;
            graphic.color = new Color(color.r, color.g, color.b, 0);
            while (elapsed <= duration)
            {
                elapsed += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(0, a, elapsed / duration);
                graphic.color = color;
                yield return null;
            }
            graphic.color = new Color(color.r, color.g, color.b, a);
        }

        public static void FadeInCanvasNow(GameObject obj)
        {
            Graphic graphic = obj.GetComponent<Graphic>();
            Color color = graphic.color;
            graphic.color = new Color(color.r, color.g, color.b, 1);
        }

        public static IEnumerator MoveAnchor(GameObject obj, Vector2 to, float duration)
        {
            float elapsed = 0;
            RectTransform rec = obj.GetComponent<RectTransform>();
            Vector3 from = rec.anchoredPosition;

            while (elapsed <= duration)
            {
                elapsed += Time.deltaTime;
                rec.anchoredPosition = Vector2.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            rec.anchoredPosition = to;
        }

        public static void MoveAnchorNow(GameObject obj, Vector2 to)
        {
            RectTransform rec = obj.GetComponent<RectTransform>();
            rec.anchoredPosition = to;
        }

        public static IEnumerator ScaleInCanvas(GameObject obj, float duration)
        {
            float elapsed = 0;
            obj.transform.localScale = new Vector3(0, 1, 1);
            while (elapsed <= duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float s = Mathf.Lerp(0, 1, elapsed / duration);
                obj.transform.localScale = new Vector3(s, 1, 1);
                yield return null;
            }
            obj.transform.localScale = new Vector3(1, 1, 1);
        }

        public static IEnumerator ScaleOutCanvas(GameObject obj, float duration)
        {
            float elapsed = 0;
            obj.transform.localScale = new Vector3(1, 1, 1);
            while (elapsed <= duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float s = Mathf.Lerp(1, 0, elapsed / duration);
                obj.transform.localScale = new Vector3(s, 1, 1);
                yield return null;
            }
            obj.transform.localScale = new Vector3(0, 1, 1);
        }

        public static void ScaleCanvasNow(GameObject obj, float toScale)
        {
            obj.transform.localScale = new Vector3(toScale, 1, 1);
        }
    }
}