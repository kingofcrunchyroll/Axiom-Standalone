using UnityEngine;
using static Axiom.Menu.Main;
using static Axiom.Settings;

namespace Axiom.Classes
{
	public class ButtonCollider : MonoBehaviour
	{
		public string relatedText;

		public bool incremental;
		public bool positive;

		public static float buttonCooldown = 0f;
		
		public void OnTriggerEnter(Collider collider)
		{
			if (Time.time > buttonCooldown && collider == buttonCollider && menu != null)
			{
                buttonCooldown = Time.time + 0.2f;
                GorillaTagger.Instance.StartVibration(rightHanded, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
                VRRig.LocalRig.PlayHandTapLocal(8, rightHanded, 0.4f);
                if (incremental)
                    ToggleIncremental(relatedText, positive);
                else
                    Toggle(relatedText);
            }
		}
	}
}
