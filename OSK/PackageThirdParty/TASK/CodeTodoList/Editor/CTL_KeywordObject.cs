using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeTodoList
{
	[Serializable]
	public class CTL_KeywordObject
	{
		public const char SEPARATOR = '¤'; //CTL...¤ can't be used in the key as it act as separator

		[SerializeField]
		string mKey;
		[SerializeField]
		Color mColor;
		[SerializeField]
		bool mVisible;
		[SerializeField]
		bool mValid;

		public bool mNeedFocus = false;

		public bool mIsDragged = false;

		public CTL_KeywordObject()
		{
			mKey = "";
			mColor = new Color(.25f, 1f, .5f, 1f);
			mVisible = true;
		}

		public string KeySearch
		{
			//get { return CTL_Settings.CaseSensitive?"//" + Key: "//" + Key.ToLower(); }
			get { return CTL_Settings.CaseSensitive ? Key : Key.ToLower(); }
		}

		public bool SimilarKey(CTL_KeywordObject other)
		{
			return (CTL_Settings.CaseSensitive && Key == other.Key) || (!CTL_Settings.CaseSensitive && Key.ToLower() == other.Key.ToLower());
		}

		public string Key
		{
			get { return mKey; }
			set
			{
				string newKey = value.Replace(SEPARATOR.ToString(), "").Replace(CTL_KeywordList.SEPARATOR.ToString(), "");
				if(newKey != mKey)
				{
					mKey = newKey;
					CTL_Settings.KeywordsList.KeywordModified(this, true);
				}
			}
		}

		public Color Color
		{
			get { return mColor; }
			set
			{
				if (value != mColor)
				{
					mColor = value;
					CTL_Settings.KeywordsList.KeywordModified(this, false);
				}
			}
		}

		public bool Visible
		{
			get { return mVisible; }
			set
			{
				if (value != mVisible)
				{
					CTL_TodoObject.sNeedRefreshDisplayedElementsList = true;
					mVisible = value;
					CTL_Settings.KeywordsList.KeywordModified(this, false);
				}
			}
		}

		public bool Valid
		{
			get { return mValid; }
			set
			{
				if (value != mValid)
				{
					mValid = value;
					CTL_Settings.KeywordsList.KeywordModified(this, false);
				}
			}
		}

		public string Export()
		{
			return	mKey + 
					SEPARATOR + "#" + ColorUtility.ToHtmlStringRGBA(mColor) + 
					SEPARATOR + mVisible.ToString() + 
					SEPARATOR + mValid.ToString();
		}

		public static CTL_KeywordObject LoadFromString(string str)
		{
			CTL_KeywordObject obj = new CTL_KeywordObject();
			string[] splitted = str.Split(SEPARATOR);
			try
			{
				obj.Key = splitted[0];
				ColorUtility.TryParseHtmlString(splitted[1], out obj.mColor);
				bool.TryParse(splitted[2], out obj.mVisible);
				bool.TryParse(splitted[3], out obj.mValid);

				if(obj.mColor.a == 0)
				{
					obj.mColor.a = 1;
				}
			}
			catch (Exception)
			{
				obj.mValid = false;
			}

			return obj;
		}
	}
}

