using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Enemies {
	public class Constant : Enemy {
		protected void Awake() {
			TryGetComponent(out ThreadComponent);
		}

		protected void Start() {
			if (ThreadComponent) ThreadComponent.AddThread<Items.PointerOrbital>();
		}
	}
}
