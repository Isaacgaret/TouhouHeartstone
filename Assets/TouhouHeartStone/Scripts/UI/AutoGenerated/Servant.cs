//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using BJSYGameCore.UI;
    
    public partial class Servant : UIObject
    {
        protected override void Awake()
        {
            base.Awake();
            this.autoBind();
            this.onAwake();
        }
        public void autoBind()
        {
            this.m_as_Image = this.GetComponent<Image>();
            this._Mask = this.transform.Find("Mask").GetComponent<Mask>();
            this._Image = this.transform.Find("Mask").Find("Image").GetComponent<Image>();
        }
        [SerializeField()]
        private Image m_as_Image;
        public Image asImage
        {
            get
            {
                if ((this.m_as_Image == null))
                {
                    this.m_as_Image = this.GetComponent<Image>();
                }
                return this.m_as_Image;
            }
        }
        [SerializeField()]
        private Mask _Mask;
        public Mask Mask
        {
            get
            {
                if ((this._Mask == null))
                {
                    this._Mask = this.transform.Find("Mask").GetComponent<Mask>();
                }
                return this._Mask;
            }
        }
        [SerializeField()]
        private Image _Image;
        public Image Image
        {
            get
            {
                if ((this._Image == null))
                {
                    this._Image = this.transform.Find("Mask").Find("Image").GetComponent<Image>();
                }
                return this._Image;
            }
        }
        partial void onAwake();
    }
}