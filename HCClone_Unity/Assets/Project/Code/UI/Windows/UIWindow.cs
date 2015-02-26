using UnityEngine;
using System;
using System.Collections;

public class UIWindow : MonoBehaviour {
	[SerializeField]
	protected EUIWindowKey _windowKey = EUIWindowKey.None;

	protected Action _onPreShow = null;
	protected Action _onPostShow = null;
	protected Action _onPreHide = null;
	protected Action _onPostHide = null;

	public void Show() {
		if (_onPreShow != null) {
			_onPreShow();
		}

		gameObject.SetActive(true);

		if (_onPostShow != null) {
			_onPostShow();
		}
	}

	public void Hide() {
		if (_onPreHide != null) {
			_onPreHide();
		}

		gameObject.SetActive(false);

		if (_onPostHide != null) {
			_onPostHide();
		}
	}
}
