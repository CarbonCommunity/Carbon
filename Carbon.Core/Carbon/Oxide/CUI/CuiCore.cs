using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using UnityEngine.UI;
using UnityEngine;

namespace Oxide.Game.Rust.Cui
{
    public class ComponentConverter : JsonConverter
    {
        public override void WriteJson ( JsonWriter writer, object value, JsonSerializer serializer )
        {
            throw new NotImplementedException ();
        }

        public override object ReadJson ( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
        {
            var jobject = JObject.Load ( reader );
            var text = jobject [ "type" ].ToString ();
            var typeFromHandle = ( Type )null;

            switch ( text )
            {
                case "UnityEngine.UI.Image":
                    typeFromHandle = typeof ( CuiImageComponent );
                    break;

            }

            // uint num = < PrivateImplementationDetails >.ComputeStringHash ( text );
            // if ( num <= 1466421966U )
            // {
            //     if ( num <= 976416075U )
            //     {
            //         if ( num != 938738728U )
            //         {
            //             if ( num == 976416075U )
            //             {
            //                 if ( text == "UnityEngine.UI.Image" )
            //                 {
            //                     typeFromHandle = typeof ( CuiImageComponent );
            //                     goto IL_197;
            //                 }
            //             }
            //         }
            //         else if ( text == "UnityEngine.UI.Outline" )
            //         {
            //             typeFromHandle = typeof ( CuiOutlineComponent );
            //             goto IL_197;
            //         }
            //     }
            //     else if ( num != 1120441549U )
            //     {
            //         if ( num == 1466421966U )
            //         {
            //             if ( text == "UnityEngine.UI.InputField" )
            //             {
            //                 typeFromHandle = typeof ( CuiInputFieldComponent );
            //             }
            //         }
            //     }
            //     else if ( text == "UnityEngine.UI.RawImage" )
            //     {
            //         typeFromHandle = typeof ( CuiRawImageComponent );
            //     }
            // }
            // else if ( num <= 3307054824U )
            // {
            //     if ( num != 2471485801U )
            //     {
            //         if ( num == 3307054824U )
            //         {
            //             if ( text == "NeedsCursor" )
            //             {
            //                 typeFromHandle = typeof ( CuiNeedsCursorComponent );
            //             }
            //         }
            //     }
            //     else if ( text == "RectTransform" )
            //     {
            //         typeFromHandle = typeof ( CuiRectTransformComponent );
            //     }
            // }
            // else if ( num != 4090570613U )
            // {
            //     if ( num == 4278175142U )
            //     {
            //         if ( text == "UnityEngine.UI.Button" )
            //         {
            //             typeFromHandle = typeof ( CuiButtonComponent );
            //             goto IL_197;
            //         }
            //     }
            // }
            // else if ( text == "UnityEngine.UI.Text" )
            // {
            //     typeFromHandle = typeof ( CuiTextComponent );
            //     goto IL_197;
            // }

            var instance = Activator.CreateInstance ( typeFromHandle );
            serializer.Populate ( jobject.CreateReader (), instance );
            return instance;
        }

        public override bool CanConvert ( Type objectType )
        {
            return objectType == typeof ( ICuiComponent );
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
    }

    #region Components

    public class CuiButtonComponent : ICuiComponent, ICuiColor
    {
        public string Type
        {
            get
            {
                return "UnityEngine.UI.Button";
            }
        }

        [JsonProperty ( "command" )]
        public string Command { get; set; }

        [JsonProperty ( "close" )]
        public string Close { get; set; }

        [DefaultValue ( "Assets/Content/UI/UI.Background.Tile.psd" )]
        [JsonProperty ( "sprite" )]
        public string Sprite { get; set; } = "Assets/Content/UI/UI.Background.Tile.psd";

        [DefaultValue ( "Assets/Icons/IconMaterial.mat" )]
        [JsonProperty ( "material" )]
        public string Material { get; set; } = "Assets/Icons/IconMaterial.mat";

        public string Color { get; set; } = "1.0 1.0 1.0 1.0";

        [DefaultValue ( Image.Type.Simple )]
        [JsonConverter ( typeof ( StringEnumConverter ) )]
        [JsonProperty ( "imagetype" )]
        public Image.Type ImageType { get; set; }

        [JsonProperty ( "fadeIn" )]
        public float FadeIn { get; set; }
    }
    public class CuiElementContainer : List<CuiElement>
    {
        public string Add ( CuiButton button, string parent = "Hud", string name = null )
        {
            if ( string.IsNullOrEmpty ( name ) )
            {
                name = CuiHelper.GetGuid ();
            }
            base.Add ( new CuiElement
            {
                Name = name,
                Parent = parent,
                FadeOut = button.FadeOut,
                Components =
                {
                    button.Button,
                    button.RectTransform
                }
            } );
            if ( !string.IsNullOrEmpty ( button.Text.Text ) )
            {
                base.Add ( new CuiElement
                {
                    Parent = name,
                    FadeOut = button.FadeOut,
                    Components =
                    {
                        button.Text,
                        new CuiRectTransformComponent()
                    }
                } );
            }
            return name;
        }

        public string Add ( CuiLabel label, string parent = "Hud", string name = null )
        {
            if ( string.IsNullOrEmpty ( name ) )
            {
                name = CuiHelper.GetGuid ();
            }
            base.Add ( new CuiElement
            {
                Name = name,
                Parent = parent,
                FadeOut = label.FadeOut,
                Components =
                {
                    label.Text,
                    label.RectTransform
                }
            } );
            return name;
        }

        public string Add ( CuiPanel panel, string parent = "Hud", string name = null )
        {
            if ( string.IsNullOrEmpty ( name ) )
            {
                name = CuiHelper.GetGuid ();
            }
            CuiElement cuiElement = new CuiElement
            {
                Name = name,
                Parent = parent,
                FadeOut = panel.FadeOut
            };
            if ( panel.Image != null )
            {
                cuiElement.Components.Add ( panel.Image );
            }
            if ( panel.RawImage != null )
            {
                cuiElement.Components.Add ( panel.RawImage );
            }
            cuiElement.Components.Add ( panel.RectTransform );
            if ( panel.CursorEnabled )
            {
                cuiElement.Components.Add ( new CuiNeedsCursorComponent () );
            }
            base.Add ( cuiElement );
            return name;
        }

        public string ToJson ()
        {
            return this.ToString ();
        }

        public override string ToString ()
        {
            return CuiHelper.ToJson ( this, false );
        }
    }
    public class CuiImageComponent : ICuiComponent, ICuiColor
    {
        public string Type
        {
            get
            {
                return "UnityEngine.UI.Image";
            }
        }

        [DefaultValue ( "Assets/Content/UI/UI.Background.Tile.psd" )]
        [JsonProperty ( "sprite" )]
        public string Sprite { get; set; } = "Assets/Content/UI/UI.Background.Tile.psd";

        [DefaultValue ( "Assets/Icons/IconMaterial.mat" )]
        [JsonProperty ( "material" )]
        public string Material { get; set; } = "Assets/Icons/IconMaterial.mat";

        public string Color { get; set; } = "1.0 1.0 1.0 1.0";

        [DefaultValue ( Image.Type.Simple )]
        [JsonConverter ( typeof ( StringEnumConverter ) )]
        [JsonProperty ( "imagetype" )]
        public Image.Type ImageType { get; set; }

        [JsonProperty ( "png" )]
        public string Png { get; set; }

        [JsonProperty ( "fadeIn" )]
        public float FadeIn { get; set; }
    }
    public class CuiInputFieldComponent : ICuiComponent, ICuiColor
    {
        public string Type
        {
            get
            {
                return "UnityEngine.UI.InputField";
            }
        }

        [DefaultValue ( "Text" )]
        [JsonProperty ( "text" )]
        public string Text { get; set; } = "Text";

        [DefaultValue ( 14 )]
        [JsonProperty ( "fontSize" )]
        public int FontSize { get; set; } = 14;

        [DefaultValue ( "RobotoCondensed-Bold.ttf" )]
        [JsonProperty ( "font" )]
        public string Font { get; set; } = "RobotoCondensed-Bold.ttf";

        [DefaultValue ( TextAnchor.UpperLeft )]
        [JsonConverter ( typeof ( StringEnumConverter ) )]
        [JsonProperty ( "align" )]
        public TextAnchor Align { get; set; }

        public string Color { get; set; } = "1.0 1.0 1.0 1.0";

        [DefaultValue ( 100 )]
        [JsonProperty ( "characterLimit" )]
        public int CharsLimit { get; set; } = 100;

        [JsonProperty ( "command" )]
        public string Command { get; set; }

        [DefaultValue ( false )]
        [JsonProperty ( "password" )]
        public bool IsPassword { get; set; }
    }
    public class CuiNeedsCursorComponent : ICuiComponent
    {
        public string Type
        {
            get
            {
                return "NeedsCursor";
            }
        }
    }
    public class CuiOutlineComponent : ICuiComponent, ICuiColor
    {
        public string Type
        {
            get
            {
                return "UnityEngine.UI.Outline";
            }
        }

        public string Color { get; set; } = "1.0 1.0 1.0 1.0";

        [DefaultValue ( "1.0 -1.0" )]
        [JsonProperty ( "distance" )]
        public string Distance { get; set; } = "1.0 -1.0";

        [DefaultValue ( false )]
        [JsonProperty ( "useGraphicAlpha" )]
        public bool UseGraphicAlpha { get; set; }
    }
    public class CuiRawImageComponent : ICuiComponent, ICuiColor
    {
        public string Type
        {
            get
            {
                return "UnityEngine.UI.RawImage";
            }
        }

        [DefaultValue ( "Assets/Icons/rust.png" )]
        [JsonProperty ( "sprite" )]
        public string Sprite { get; set; } = "Assets/Icons/rust.png";

        public string Color { get; set; } = "1.0 1.0 1.0 1.0";

        [JsonProperty ( "material" )]
        public string Material { get; set; }

        [JsonProperty ( "url" )]
        public string Url { get; set; }

        [JsonProperty ( "png" )]
        public string Png { get; set; }

        public float FadeIn { get; set; }
    }
    public class CuiRectTransformComponent : ICuiComponent
    {
        public string Type
        {
            get
            {
                return "RectTransform";
            }
        }

        [DefaultValue ( "0.0 0.0" )]
        [JsonProperty ( "anchormin" )]
        public string AnchorMin { get; set; } = "0.0 0.0";

        [DefaultValue ( "1.0 1.0" )]
        [JsonProperty ( "anchormax" )]
        public string AnchorMax { get; set; } = "1.0 1.0";

        [DefaultValue ( "0.0 0.0" )]
        [JsonProperty ( "offsetmin" )]
        public string OffsetMin { get; set; } = "0.0 0.0";

        [DefaultValue ( "0.0 0.0" )]
        [JsonProperty ( "offsetmax" )]
        public string OffsetMax { get; set; } = "0.0 0.0";
    }
    public class CuiTextComponent : ICuiComponent, ICuiColor
    {
        public string Type
        {
            get
            {
                return "UnityEngine.UI.Text";
            }
        }

        [DefaultValue ( "Text" )]
        [JsonProperty ( "text" )]
        public string Text { get; set; } = "Text";

        [DefaultValue ( 14 )]
        [JsonProperty ( "fontSize" )]
        public int FontSize { get; set; } = 14;

        [DefaultValue ( "RobotoCondensed-Bold.ttf" )]
        [JsonProperty ( "font" )]
        public string Font { get; set; } = "RobotoCondensed-Bold.ttf";

        [DefaultValue ( TextAnchor.UpperLeft )]
        [JsonConverter ( typeof ( StringEnumConverter ) )]
        [JsonProperty ( "align" )]
        public TextAnchor Align { get; set; }

        public string Color { get; set; } = "1.0 1.0 1.0 1.0";

        [JsonProperty ( "fadeIn" )]
        public float FadeIn { get; set; }
    }

    #endregion

    public class CuiButton
    {
        public CuiButtonComponent Button { get; } = new CuiButtonComponent ();

        public CuiRectTransformComponent RectTransform { get; } = new CuiRectTransformComponent ();

        public CuiTextComponent Text { get; } = new CuiTextComponent ();

        public float FadeOut { get; set; }
    }
    public class CuiElement
    {
        [DefaultValue ( "AddUI CreatedPanel" )]
        [JsonProperty ( "name" )]
        public string Name { get; set; } = "AddUI CreatedPanel";

        [JsonProperty ( "parent" )]
        public string Parent { get; set; } = "Hud";

        [JsonProperty ( "components" )]
        public List<ICuiComponent> Components { get; } = new List<ICuiComponent> ();

        [JsonProperty ( "fadeOut" )]
        public float FadeOut { get; set; }
    }
    public class CuiLabel
    {
        public CuiTextComponent Text { get; } = new CuiTextComponent ();

        public CuiRectTransformComponent RectTransform { get; } = new CuiRectTransformComponent ();

        public float FadeOut { get; set; }
    }
    public class CuiPanel
    {
        public CuiImageComponent Image { get; set; } = new CuiImageComponent ();

        public CuiRawImageComponent RawImage { get; set; }

        public CuiRectTransformComponent RectTransform { get; } = new CuiRectTransformComponent ();

        public bool CursorEnabled { get; set; }

        public float FadeOut { get; set; }
    }
}
