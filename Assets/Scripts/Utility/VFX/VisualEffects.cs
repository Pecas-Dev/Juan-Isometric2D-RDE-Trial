using UnityEngine;
using System.Collections;


namespace JuanIsometric2D.VFX
{
    public static class VisualEffects
    {
        public static IEnumerator FlickerSprite(SpriteRenderer sprite, float duration = 0.5f, float interval = 0.05f)
        {
            float elapsedTime = 0f;
            bool isVisible = true;

            while (elapsedTime < duration)
            {
                sprite.enabled = isVisible;
                isVisible = !isVisible;
                elapsedTime += interval;

                yield return new WaitForSeconds(interval);
            }

            sprite.enabled = true;
        }

        public static IEnumerator HealingEffect(SpriteRenderer sprite, float duration = 0.5f)
        {
            Color originalColor = sprite.color;
            Color healColor = Color.green;

            float elapsedTime = 0f;
            float fadeTime = 0.2f;

            while (elapsedTime < fadeTime)
            {
                sprite.color = Color.Lerp(originalColor, healColor, elapsedTime / fadeTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(duration - (fadeTime * 2));

            elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                sprite.color = Color.Lerp(healColor, originalColor, elapsedTime / fadeTime);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            sprite.color = originalColor;
        }
    }
}
