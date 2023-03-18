using UnityEngine;
using System.Linq;
using System.Collections;


namespace TMPro
{
    public static class ShaderUtilities
    {
        // Shader Property IDs
        public static int ID_MainTex;

        public static int ID_FaceTex;
        public static int ID_FaceColor;
        public static int ID_FaceDilate;
        public static int ID_Shininess;

        public static int ID_UnderlayColor;
        public static int ID_UnderlayOffsetX;
        public static int ID_UnderlayOffsetY;
        public static int ID_UnderlayDilate;
        public static int ID_UnderlaySoftness;

        /// <summary>
        /// Property ID for the _UnderlayOffset shader property used by URP and HDRP shaders
        /// </summary>
        public static int ID_UnderlayOffset;

        /// <summary>
        /// Property ID for the _UnderlayIsoPerimeter shader property used by URP and HDRP shaders
        /// </summary>
        public static int ID_UnderlayIsoPerimeter;

        public static int ID_WeightNormal;
        public static int ID_WeightBold;

        public static int ID_OutlineTex;
        public static int ID_OutlineWidth;
        public static int ID_OutlineSoftness;
        public static int ID_OutlineColor;

        public static int ID_Outline2Color;
        public static int ID_Outline2Width;

        public static int ID_Padding;
        public static int ID_GradientScale;
        public static int ID_ScaleX;
        public static int ID_ScaleY;
        public static int ID_PerspectiveFilter;
        public static int ID_Sharpness;

        public static int ID_TextureWidth;
        public static int ID_TextureHeight;

        public static int ID_BevelAmount;

        public static int ID_GlowColor;
        public static int ID_GlowOffset;
        public static int ID_GlowPower;
        public static int ID_GlowOuter;
        public static int ID_GlowInner;

        public static int ID_LightAngle;

        public static int ID_EnvMap;
        public static int ID_EnvMatrix;
        public static int ID_EnvMatrixRotation;

        //public static int ID_MaskID;
        public static int ID_MaskCoord;
        public static int ID_ClipRect;
        public static int ID_MaskSoftnessX;
        public static int ID_MaskSoftnessY;
        public static int ID_VertexOffsetX;
        public static int ID_VertexOffsetY;
        public static int ID_UseClipRect;

        public static int ID_StencilID;
        public static int ID_StencilOp;
        public static int ID_StencilComp;
        public static int ID_StencilReadMask;
        public static int ID_StencilWriteMask;

        public static int ID_ShaderFlags;
        public static int ID_ScaleRatio_A;
        public static int ID_ScaleRatio_B;
        public static int ID_ScaleRatio_C;

        public static string Keyword_Bevel = "BEVEL_ON";
        public static string Keyword_Glow = "GLOW_ON";
        public static string Keyword_Underlay = "UNDERLAY_ON";
        public static string Keyword_Ratios = "RATIOS_OFF";
        //public static string Keyword_MASK_OFF = "MASK_OFF";
        public static string Keyword_MASK_SOFT = "MASK_SOFT";
        public static string Keyword_MASK_HARD = "MASK_HARD";
        public static string Keyword_MASK_TEX = "MASK_TEX";
        public static string Keyword_Outline = "OUTLINE_ON";

        public static string ShaderTag_ZTestMode = "unity_GUIZTestMode";
        public static string ShaderTag_CullMode = "_CullMode";

        private static float m_clamp = 1.0f;
        public static bool isInitialized = false;


        /// <summary>
        /// Returns a reference to the mobile distance field shader.
        /// </summary>
        internal static Shader ShaderRef_MobileSDF
        {
            get
            {
                if (k_ShaderRef_MobileSDF == null)
                    k_ShaderRef_MobileSDF = Shader.Find("TextMeshPro/Mobile/Distance Field");

                return k_ShaderRef_MobileSDF;
            }
        }
        static Shader k_ShaderRef_MobileSDF;

        /// <summary>
        /// Returns a reference to the mobile bitmap shader.
        /// </summary>
        internal static Shader ShaderRef_MobileBitmap
        {
            get
            {
                if (k_ShaderRef_MobileBitmap == null)
                    k_ShaderRef_MobileBitmap = Shader.Find("TextMeshPro/Mobile/Bitmap");

                return k_ShaderRef_MobileBitmap;
            }
        }
        static Shader k_ShaderRef_MobileBitmap;


        /// <summary>
        ///
        /// </summary>
        static ShaderUtilities()
        {
            GetShaderPropertyIDs();
        }

        /// <summary>
        ///
        /// </summary>
        public static void GetShaderPropertyIDs()
        {
            if (isInitialized == false)
            {
                //Debug.Log("Getting Shader property IDs");
                isInitialized = true;

                ID_MainTex = Shader.PropertyToID("_MainTex");

                ID_FaceTex = Shader.PropertyToID("_FaceTex");
                ID_FaceColor = Shader.PropertyToID("_FaceColor");
                ID_FaceDilate = Shader.PropertyToID("_FaceDilate");
                ID_Shininess = Shader.PropertyToID("_FaceShininess");

                ID_UnderlayColor = Shader.PropertyToID("_UnderlayColor");
                ID_UnderlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");
                ID_UnderlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");
                ID_UnderlayDilate = Shader.PropertyToID("_UnderlayDilate");
                ID_UnderlaySoftness = Shader.PropertyToID("_UnderlaySoftness");

                ID_UnderlayOffset = Shader.PropertyToID("_UnderlayOffset");
                ID_UnderlayIsoPerimeter = Shader.PropertyToID("_UnderlayIsoPerimeter");

                ID_WeightNormal = Shader.PropertyToID("_WeightNormal");
                ID_WeightBold = Shader.PropertyToID("_WeightBold");

                ID_OutlineTex = Shader.PropertyToID("_OutlineTex");
                ID_OutlineWidth = Shader.PropertyToID("_OutlineWidth");
                ID_OutlineSoftness = Shader.PropertyToID("_OutlineSoftness");
                ID_OutlineColor = Shader.PropertyToID("_OutlineColor");

                ID_Outline2Color = Shader.PropertyToID("_Outline2Color");
                ID_Outline2Width = Shader.PropertyToID("_Outline2Width");

                ID_Padding = Shader.PropertyToID("_Padding");
                ID_GradientScale = Shader.PropertyToID("_GradientScale");
                ID_ScaleX = Shader.PropertyToID("_ScaleX");
                ID_ScaleY = Shader.PropertyToID("_ScaleY");
                ID_PerspectiveFilter = Shader.PropertyToID("_PerspectiveFilter");
                ID_Sharpness = Shader.PropertyToID("_Sharpness");

                ID_TextureWidth = Shader.PropertyToID("_TextureWidth");
                ID_TextureHeight = Shader.PropertyToID("_TextureHeight");

                ID_BevelAmount = Shader.PropertyToID("_Bevel");

                ID_LightAngle = Shader.PropertyToID("_LightAngle");

                ID_EnvMap = Shader.PropertyToID("_Cube");
                ID_EnvMatrix = Shader.PropertyToID("_EnvMatrix");
                ID_EnvMatrixRotation = Shader.PropertyToID("_EnvMatrixRotation");


                ID_GlowColor = Shader.PropertyToID("_GlowColor");
                ID_GlowOffset = Shader.PropertyToID("_GlowOffset");
                ID_GlowPower = Shader.PropertyToID("_GlowPower");
                ID_GlowOuter = Shader.PropertyToID("_GlowOuter");
                ID_GlowInner = Shader.PropertyToID("_GlowInner");

                //ID_MaskID = Shader.PropertyToID("_MaskID");
                ID_MaskCoord = Shader.PropertyToID("_MaskCoord");
                ID_ClipRect = Shader.PropertyToID("_ClipRect");
                ID_UseClipRect = Shader.PropertyToID("_UseClipRect");
                ID_MaskSoftnessX = Shader.PropertyToID("_MaskSoftnessX");
                ID_MaskSoftnessY = Shader.PropertyToID("_MaskSoftnessY");
                ID_VertexOffsetX = Shader.PropertyToID("_VertexOffsetX");
                ID_VertexOffsetY = Shader.PropertyToID("_VertexOffsetY");

                ID_StencilID = Shader.PropertyToID("_Stencil");
                ID_StencilOp = Shader.PropertyToID("_StencilOp");
                ID_StencilComp = Shader.PropertyToID("_StencilComp");
                ID_StencilReadMask = Shader.PropertyToID("_StencilReadMask");
                ID_StencilWriteMask = Shader.PropertyToID("_StencilWriteMask");

                ID_ShaderFlags = Shader.PropertyToID("_ShaderFlags");
                ID_ScaleRatio_A = Shader.PropertyToID("_ScaleRatioA");
                ID_ScaleRatio_B = Shader.PropertyToID("_ScaleRatioB");
                ID_ScaleRatio_C = Shader.PropertyToID("_ScaleRatioC");

                // Set internal shader references
                if (k_ShaderRef_MobileSDF == null)
                    k_ShaderRef_MobileSDF = Shader.Find("TextMeshPro/Mobile/Distance Field");

                if (k_ShaderRef_MobileBitmap == null)
                    k_ShaderRef_MobileBitmap = Shader.Find("TextMeshPro/Mobile/Bitmap");
            }
        }



        // Scale Ratios to ensure property ranges are optimum in Material Editor
        public static void UpdateShaderRatios(Material mat)
        {
            //Debug.Log("UpdateShaderRatios() called.");

            float ratio_A = 1;
            float ratio_B = 1;
            float ratio_C = 1;

            bool isRatioEnabled = !mat.shaderKeywords.Contains(Keyword_Ratios);

            if (!mat.HasProperty(ID_GradientScale) || !mat.HasProperty(ID_FaceDilate))
                return;

            // Compute Ratio A
            float scale = mat.GetFloat(ID_GradientScale);
            float faceDilate = mat.GetFloat(ID_FaceDilate);
            float outlineThickness = mat.GetFloat(ID_OutlineWidth);
            float outlineSoftness = mat.GetFloat(ID_OutlineSoftness);

            float weight = Mathf.Max(mat.GetFloat(ID_WeightNormal), mat.GetFloat(ID_WeightBold)) / 4.0f;

            float t = Mathf.Max(1, weight + faceDilate + outlineThickness + outlineSoftness);

            ratio_A = isRatioEnabled ? (scale - m_clamp) / (scale * t) : 1;

            //float ratio_A_old = mat.GetFloat(ID_ScaleRatio_A);

            // Only set the ratio if it has changed.
            //if (ratio_A != ratio_A_old)
                mat.SetFloat(ID_ScaleRatio_A, ratio_A);

            // Compute Ratio B
            if (mat.HasProperty(ID_GlowOffset))
            {
                float glowOffset = mat.GetFloat(ID_GlowOffset);
                float glowOuter = mat.GetFloat(ID_GlowOuter);

                float range = (weight + faceDilate) * (scale - m_clamp);

                t = Mathf.Max(1, glowOffset + glowOuter);

                ratio_B = isRatioEnabled ? Mathf.Max(0, scale - m_clamp - range) / (scale * t) : 1;
                //float ratio_B_old = mat.GetFloat(ID_ScaleRatio_B);

                // Only set the ratio if it has changed.
                //if (ratio_B != ratio_B_old)
                    mat.SetFloat(ID_ScaleRatio_B, ratio_B);
            }

            // Compute Ratio C
            if (mat.HasProperty(ID_UnderlayOffsetX))
            {
                float underlayOffsetX = mat.GetFloat(ID_UnderlayOffsetX);
                float underlayOffsetY = mat.GetFloat(ID_UnderlayOffsetY);
                float underlayDilate = mat.GetFloat(ID_UnderlayDilate);
                float underlaySoftness = mat.GetFloat(ID_UnderlaySoftness);

                float range = (weight + faceDilate) * (scale - m_clamp);

                t = Mathf.Max(1, Mathf.Max(Mathf.Abs(underlayOffsetX), Mathf.Abs(underlayOffsetY)) + underlayDilate + underlaySoftness);

                ratio_C = isRatioEnabled ? Mathf.Max(0, scale - m_clamp - range) / (scale * t) : 1;
                //float ratio_C_old = mat.GetFloat(ID_ScaleRatio_C);

                // Only set the ratio if it has changed.
                //if (ratio_C != ratio_C_old)
                    mat.SetFloat(ID_ScaleRatio_C, ratio_C);
            }
        }



        // Function to calculate padding required for Outline Width & Dilation for proper text alignment
        public static Vector4 GetFontExtent(Material material)
        {
            // Revised implementation where style no longer affects alignment
            return Vector4.zero;

            /*
            if (material == null || !material.HasProperty(ShaderUtilities.ID_GradientScale))
                return Vector4.zero;   // We are using an non SDF Shader.

            float scaleRatioA = material.GetFloat(ID_ScaleRatio_A);
            float faceDilate = material.GetFloat(ID_FaceDilate) * scaleRatioA;
            float outlineThickness = material.GetFloat(ID_OutlineWidth) * scaleRatioA;

            float extent = Mathf.Min(1, faceDilate + outlineThickness);
            extent *= material.GetFloat(ID_GradientScale);

            return new Vector4(extent, extent, extent, extent);
            */
        }


        // Function to check if Masking is enabled
        public static bool IsMaskingEnabled(Material material)
        {
            if (material == null || !material.HasProperty(ShaderUtilities.ID_ClipRect))
                return false;

            if (material.shaderKeywords.Contains(ShaderUtilities.Keyword_MASK_SOFT) || material.shaderKeywords.Contains(ShaderUtilities.Keyword_MASK_HARD) || material.shaderKeywords.Contains(ShaderUtilities.Keyword_MASK_TEX))
                return true;

            return false;
        }


        // Function to determine how much extra padding is required as a result of material properties like dilate, outline thickness, softness, glow, etc...
        public static float GetPadding(Material material, bool enableExtraPadding, bool isBold)
        {
            //Debug.Log("GetPadding() called.");

            if (isInitialized == false)
                GetShaderPropertyIDs();

            // Return if Material is null
            if (material == null) return 0;

            int extraPadding = enableExtraPadding ? 4 : 0;

            // Check if we are using a non Distance Field Shader
            if (material.HasProperty(ID_GradientScale) == false)
            {
                if (material.HasProperty(ID_Padding))
                    extraPadding += (int)material.GetFloat(ID_Padding);

                return extraPadding + 1.0f;
            }

            Vector4 padding = Vector4.zero;
            Vector4 maxPadding = Vector4.zero;

            //float weight = 0;
            float faceDilate = 0;
            float faceSoftness = 0;
            float outlineThickness = 0;
            float scaleRatio_A = 0;
            float scaleRatio_B = 0;
            float scaleRatio_C = 0;

            float glowOffset = 0;
            float glowOuter = 0;

            float uniformPadding = 0;
            // Iterate through each of the assigned materials to find the max values to set the padding.

            // Update Shader Ratios prior to computing padding
            UpdateShaderRatios(material);

            string[] shaderKeywords = material.shaderKeywords;

            if (material.HasProperty(ID_ScaleRatio_A))
                scaleRatio_A = material.GetFloat(ID_ScaleRatio_A);

            //weight = 0; // Mathf.Max(material.GetFloat(ID_WeightNormal), material.GetFloat(ID_WeightBold)) / 2.0f * scaleRatio_A;

            if (material.HasProperty(ID_FaceDilate))
                faceDilate = material.GetFloat(ID_FaceDilate) * scaleRatio_A;

            if (material.HasProperty(ID_OutlineSoftness))
                faceSoftness = material.GetFloat(ID_OutlineSoftness) * scaleRatio_A;

            if (material.HasProperty(ID_OutlineWidth))
                outlineThickness = material.GetFloat(ID_OutlineWidth) * scaleRatio_A;

            uniformPadding = outlineThickness + faceSoftness + faceDilate;

            // Glow padding contribution
            if (material.HasProperty(ID_GlowOffset) && shaderKeywords.Contains(Keyword_Glow)) // Generates GC
            {
                if (material.HasProperty(ID_ScaleRatio_B))
                    scaleRatio_B = material.GetFloat(ID_ScaleRatio_B);

                glowOffset = material.GetFloat(ID_GlowOffset) * scaleRatio_B;
                glowOuter = material.GetFloat(ID_GlowOuter) * scaleRattÑ<z‚®¨ó'uë©:zyù¸¸¹—¿Hñ.3Tœ‡>KmšêaÅÏ‹ìZ¬òëŒÃ·¼Q°·Q2?Õÿ7Ì°DÂóq-éô''’ºk¡KSª{B.1ZS¬óHÇ¢ívq{É]¸ëĞf°ÉõU}ÉíQûŒ‹¤ŒxG.ØĞÆ\‹zg5ƒÜ®<kÔr­ÁG-ÛÙpt/{‹û%]d…¡y'@%”Õ²•íÜhŞ2/Óë8Ô4»Ä+ûJØ!Q¨Ò‘d³ñ]µ¢½LHÛA~mp‡ªe
ì±YxzÓ¶ø~ÎŞ
~ûF‹—fÙõ#÷ÙoG¹T<ùYå¨öİRwZé>õ‡1ªÄáş?ã·BSÍğŸÏFÛw~™(Àâ¿œDë=$2_<	%|µ¤‘,°Åà[ö´d“÷şê|çÊğ‰
»™æéÕÆ’¯„šwËÓƒ—°‰µNı÷b¢ír°pnîŸÛ6ôJÁŠ|Èg,#ÑCİÀ¹¹ÛtÃxË~äaÁ}ßı…Ö·ê—ëhâÿT°”¿y§ëõŒş^­è
G¬ğA×¹3¶%Ñğœô|ç)¸±U·=tn•ìîN}gÆ†xOÃ¥mŠ[A|ßÆ-
áhdÅNnğºÜÅ°Åúe}ÜÀğü‹V”æd¾ÚAÎK¾WÚ.“¯…Õº=7ó³^è+]!T4g†Nt¶´½@u¡ÕÔm¦]æ*(5£•'kõD=ZALZÍRlª£Ñ‘Ğ—×‡)(ÖØ3pŠGbÎÆb÷£7‡ÖıÿRà»	ÀF"›·5QÀ¤Éæî°a1bXŠ¼¡b%Zİ`\¡
ğ¬¶l#_ï¡2‚Æ€äjhºœ&Ï]t7`ób>Ã±qhC]8…ÅôÔ²ÇÛo1_¥}y‹CñY–wdG¸>¼ë\Æ„jŒcÂÚÂµWµ-ZÅ›ºT$™áöéI”ˆ
6‚Ò(Ø$±Í©Ïu^æŠeá9…òìûRE'rœ‹;ÛÛœÔ>´`Yëèõj>
0CÒå¬ØrUX7(1…‚']ßô­ëÈœkÀ¼ÅXì”´aNğ¼¬/X„ëúê9ÃØÆÂ1~(¨Å£tlhµn;\, ’½×¶Eç‚ÑV$¤A®»É’Ò÷Ë°z`!Ÿ/»›`0™^À”ù›ëõ•i[-Êlèk&|Ş3ÏèMÓFğ¥ç+ø{+!xA\Øÿ3—·ÉÆ>Œ25AÏÎ¯oZy¢-Ew.Ó¥£;øg>Êú³Vgœfd‚Ëv6š¸ª'ÄÚ6¾ªEZòTÛ€ÓR(LÛkrçT…èÏñf<{Çsz0çÇêG                      eETUUU  °
»š    ° `€ €›‰™ˆ™ˆx§	™€€                                                                                ¸»      §       ‡©«
	   vº
º°«w‰™º™š›Šwˆ˜
ª™ ‹v§™ª›ª™‰f˜x™›«»Šf€	»«°{w˜ª»¹°€™™º  
Š                        0‡†Ø…7Xòsü¼B›ÓŠ#Mhò÷2ñp)rr¦øÃÍÇüË¾ôÉ°2j’(Ÿ‘Ğ¿Í*­ğƒÇzãN8.nD_ûÎo˜ûƒ_'O/ÄÕÆvà'.–óiO‘Y¸_u™‹¡Ø0ù¸~™Ü¥<ŸCs]#îC©ÂQ$gAˆGä +–ƒ©Ã¿ 	H-àÀ[à‡Nf¡IÖ‹
äª<×@?¶'hNk”´Õøğ¦TB´uœkÊ¨…5’c O¯D¨+)Ç`Á.óOCĞ/ô³×Íúw?6ßÙ8ÚØû™Ø¸XkPp ‘x	»âÀ$~)×Ä"0tB^9¶§¦P.Ì!Í²Ê';gã ›ü%˜r—«'®ÏóøñMœÛV¼ì¬L}l—ı‰ï­ßNW¾|µP
ĞK`HÈnª<±Nª\¾-ìP›VçVucò—§Ÿ•Ú«Y¶Á–ì#ÙV.- Ş ûae3Íl¢¥¿vf¶ĞIó5¥>!œ/SØ ÃÄ8úZ@Æ³TaÔÎV¬“Ç
0"#îV}T •¸Ya¹€–'ÕZÒú9Ã0¥½]±”ÇË[Y“Cöeª.QÄ³—A×oWµ‡‰ ¤Eó0È}¥«œZİ0ˆ(ôòËú¬‚Ï—x„‘€,Mã€ÈVÚÈNš©”[ëÒ2e< Eœ%h×ñNDM–°×e—²5öÚ§!@ƒ™ oX>X€TÎ['°ógF ½ĞJ[€i}åÑ#xŒtÿ"IãÉ#wŸ5‚Ş»sÒ^ºÿFÅmˆ­Ï…hP–An¡¡ÚB9~”,ù<7í„V™œ¡¥á%R‰%®8«U„¥œİ›'+ŸŒ‰²2m<R©çÓßRú¨¦ê¹a±pó÷U“ClíÎp6ÖçÍiq_ÍiCKƒ…­ñìN7è4Ò\h±×BsdêL-í™’^:g'’Rh©æŒt–!Íû†¥r¦Pgv-·OÏ²qbnøµN#¤úÕh¾¸MÏ…sÑz‹]B´û$cÖ/æÎ è+;»£˜•õ
ÔWW…>OÛ‚pq“¨4:à[KAC¸F¾n
è§‹u¼ -*n>’îK§—IDg;4éëØN*t\re}w[ Ñ2™EWØ.FÓ&(:I‰Æà»Œu
v[[¥>çÒì¡™|'¢I^
'0:¬g}ï¾}ë$·YñJ¡®;hOÄÃ÷>)	4¸,Y{3S)ÒA(‰óïBüÈ?Èá·d"QSêhöµj5‚Z×—îÀæiuÏÖ²²¯XWêõÜíg€™h6#òŠó8>D;DËÌw´5µ¶lâÜ±ü&”™umê6[Ç“Wƒ5Zæ®Iµœ1 ?ÓŸ}8Ú¶%Ú‚Ë,¢oƒ.À;Áz½Ntg–¿%U¼WİÑÙ*,|6k{³$ı´ç¿YÎ%[ˆ~×ÊûgÄÂUzu¾¸¾öë÷séçŸ>ù±f³Jı¾?ûµ:€t›W_ÃÑ8Ô-{-8— ¤½V7Â§ µÃ/uù)0¡>øYçX™ÀÄÌw'Š˜ôO÷%dšïŒ“ş^s6T’ü²÷ÓJ·ãvrIƒ3ÕTBú_'[&]Ü^’'Îõ<¶ªïu¨S|ZO4îbdñJ?G;Àí;şÅ‹ûŠÚîbP©+=„HîûÚİöİ#ŒöRA¹¦z½6öÂuw”¤¶mitlTäOIõùUybôwŞ.Ë,æ_å»x/üf–YËÓnşËq;'òd©9xçF¶ÿÀ‡yà«t†læYÙèÎqä	Mì¼ú"¥Éh–¡~Ş¼¡¸¢aŸ
CN3¬oIòw¼°uéàğ¯½ö>MêËì}Unş 1^òBämfñ{¦,h¡İy[›¹FÅ‚”/GI…¿áukÁ‹;º˜‘îÜ 	Üh>Õ_oùš§ƒÁ,îlÔz¸õ$TïÚcÙ ÄCxrÀ¢¾ÂbÏ Oa0ö|$@3@‡ÖŠ)ˆ<Lœ
œƒª¡pMM-è?T#sC9Ñ‘.—7ñnğsA2¸²”#|ø†?Ü&šåMR¾á NVòU}ÂØÔîÌb—èA‡)::³«l;Ûœ)ø€Jì› Ñ¬Öšñ)«œµùä¡Œ
ót©EÅÉ ¶Òê°>Á)hÊ¼‹“Òú‚xç°O>zRÑcs¦~‰J.°I
N¼ó.ö!/û‹O‘ÏŞÇ9ô"òfxÅ q¸†f}Ùw ĞÒ”šVØì‰Ğ÷^ûkÇ¬ºxùç†ïB2$ü]Bò'Rd‡‹‘R„^B¼HçN$ıdOª,ùÜb¿¯JJfıâh¦3E’µ"±—ÿ¿ö O4"Ño\=9çÓÿôcÇ¸«·—!óÎ§<BÚ-@‡Íé"²G)Ûß¦—	;Zñ0k@![P/®3
‹ÎCÍõJÜ8Sn—©AË÷°& àÙ‚DDºÁ©ª¯ò~Fyú'Ggæêİÿù
ì·xØÒ>=¹çWæjáªAÁ#™lÆ³ˆ´[UˆX17ˆxĞX\&®L	¥Bd‚Ç¯¨˜ˆ¿+’·¥Ô4Òx¾µ®ÄzuhÎà7Òø”Šâ!M'f`uÑîâ Tq’ñü¶kfzÚ‚0öÿ	DÆh75äÈ×`ÖÀËÇsBa\2Äo8ØCÖ¨IS)ãº‘ÂæÚ[õ@úÆ3"ï_ÆŒuƒí‚Q\f{çP%J‘[GíçÖ¢Ç‹®—7Së‹ş40XKì‚Áè›”6?ı¹Œ™¿“nBÜìÆÜüb f´
ÏÃ\>|aß<¿tÑÖæÕê´Œ¼s
s1VÕ²@ü/                      eEUUUf   
ªš    
 

 `p pŠ‰¨xˆxw—	šp€                                                                 
                      ˆ
	     v˜¨
    ˆ	© 
  €v ¨ª ª pv¨	
ª yv†ˆ	©	 iv‰¨™ ªjx™ˆ

 ªzv©¨
   €‡
© 
	p                        )=
2%ó:yÚQ‚±
tBósI`<1`ùÊz™A˜+jF›±éxl€XšK9å	Î53,ê
0VÒsÓaeÁZ×¶Ä`•lnE‹@ƒNKY$©ÖÊÑ ÑfRÑ-ÑRrµ#îÉÓ‘hIA%Ó¶‡Yç¬‘ÑÆ+)¨?¤p	«¶zÊ
bÔË™;Ÿ7vTØ/ƒß>;ápOdwQ-˜ 9-öáå$çhe‰Œ‰Mã6*æÅWl…¬w”+µxo€}§…?ºft;´ÚS'Ú‘B‰`7íìîj
ju [¹·“¶¥L¡R0Ù—>ÕQ‚ä 8¦¾ÌBù˜ƒTÃ’’ÑÊ•åÍ°Ãø¥yn‚|ÁRšf W iã	+ğà!şl»mà”\X3ÍEø ‚9Õ®sŞCHƒŞ+ËLâ7q§™ìÌG¬Ë3Nj—¥¼GÈM-(œ”M§Ñ‡3)ˆó=é5z2³…v&~½ôOOÈÕ²ök²©Sì‘QfÊĞkÈ#ŸRCG9\òœ—;6GW®Í”©Î—‰ûš´Î’k‰I1‰ĞvvvF%±qb|9[*Xk¸ÙNùL,,'ËírlaÿNó½&TjÔı¢#{RÄyõumÛRëÍÍÛ¸R†,e\˜lzãÕ}÷¢SŒw¡Tövøaw½…[üVm×‘>òÿ¥a ngîBccV÷Í#İš« ö>Ú¼è^aJÍõ9érÎÔ&ª§Øq=ÆŸ”c1™vùµĞJ4ï:Õm 8€§4¡¥pYVá‹mOàŸ?^(\Ù0ıx(­%i.ßtå¢ò«k¹–Ğ_ÇšøsòÕ t­	F× u‰ªÿÂ5¶ªB„:“:>ycV+äË§aH]BÿQ ›–İC—­ûA;¯—-<Úâç-
Ÿñ”5ÃUäê3M:!mÛ|qy¶e
|Â1"È‘ÏÁ¬|^cîúf"­Ö¸5ÛÇA˜Å€N«X2¿/­ZR\Ó›8êhê”õ¢°?ª­tS®}ºóó
§­ş­m!këå*vX”+
Z›~}6¹×qkÚ]¼}{ÌÆË ı…Ç"…Sâh¾ ñ›x•æ÷„4a|ƒ^f‰FIŸ«=Ô}Gª®^ÚK{‚Ğ—UºFµî•¼Y­™«ç.)ˆÜßWÂ˜ÁZ¼p8û|úAo¸p´shİ“A³ùâÃÌ[7·:íÿŒ‚Xd‚ÓÌèlÀ|)ÂA)ö)8+3GÎ€aAJ°J bm-‡2Æ:µ9ßŞşóÅ-:br³å
†‚ñ+å/{¾™ÿáö0hEa'Zœ7ûÒÂ¼ÊÚm`­Qà{Èã:è9ïÀA’ÎøÀù·h›,½ XBz·ptdï_T†ESfa©ø»1 Šô-ÁN®^„”sú'ËĞL‰ ŒøÄ°E„¾š~áR¥ûŞX¥šºoA8lEòØ…3û¥9Š5"|C(ÒÒPw÷k¦óÄ¼xŸ?äØ_s8oŒ4ƒ'À)èğÆ ØÆqû3ñ|•2Äƒß²gŒŸu¤‹›"oÚ Q¹ùg,dÆòğ`¼WÎVCCù?Ñæ¼b6·ÁÒı-+hG šWc)&à}                      eDTUTU  °»›   ° p€ €›‰™ˆ ™ˆˆ¨	»€€                                                                                ¹  °    ˆ     ‡‡º»»  °†™¸°vŠ	

 €v‡˜»ºº°€†§¹  »‹v˜¹	° €v‡˜©   pv‡˜° °€€™º°                          ®µˆB*@®AçˆQ.ÉÉYTVñá¯«	½ÈÇ¯âÀ= ~¶;%€¼Q‰0™Vd§!X1€d©Øíi »õ×`s Œvd¶â¯Gm=/w'WZUi‘XÅUÈ”j³›EÈjÄ™äº ñÒâşš*=Y ûö‡ğëÈ@<3æŸs#›y*şÄü…E•Á¿	Á¹g·Àïw50/™Å¿dãĞëëÏ&*àŸE™ıb—;üCqë[ÅÅÄ½dÅg¾ßşÀLœ¾#çÈÁuÊßl?³ªNÎ¢Eî/Z•©™§Q.*væù¶Cßıç—¯Ó×ÑÏúÈ™¹Zyuäs´İı	Ø™·Š+g¥·t—íÛîHnÏĞÃÔC*‹Â*½¨ÊbcnF-'(W{¢¯+ŞH°FBª[±Ég‡[Åõ*CÛ£Tbc_¹k·qÍÒöíø´u´r33ózÚ8'=}ğ¡Cßh(Nt^~‡23/øWWŠşÕ?éÛä»lØÆ¼Wôßgèv'X·)½<Ñá5Ò  ãıG|ğİÚúY9]Úıœä³Z²nw“nMgÕ¸^×ïJòÓ×¬_/eu ]û3ÒÚ4S©¬[ÙÁbÇı€¹²ÀÚä+&eÊyú]yİ‰¾4ŸL‘rR‡úiñéœ\QÂ#"·ä,¯Ç5‘/‘º·P¡\ëåÃïwâÏ³¡”ÄQ‘öl±¼)ÓÂ\šo€Ã¤èİÚ³5*´îišV2·À~M¨ù2N÷ò"3nIŸs35_2)3!ÏS13ñÙ™CIó8PüËæ3èÖW«¼C‚ùn2ßÿ÷_îc‡ QUìQúşæ:ñÊ- #—5¥•ÂF‚¦ª7º5Ø ó?
·iu¸O¡M_ë[0í"••úIœßíâo_t³Åùñ%B¶YÊ,>FScc”ÒôœÚ;ã2^]şD)d5ĞMU¶" j¢òcjµnÄĞŒ£ûªƒá®°!f­³ÚÉ±üà—Ù€ÆÓê5¯&5GTBå,¡•JmH]báŒß­ÚB²vµˆ\+bÜ¢®’„tµ@TºJ!Rdµn0è3c±9)nÛŠˆ‘øÏLÜWÅn3`§GµÛ¡ùöÏ†½cæ]1ÖèÏ50º¶íòSÃªä6	¡İ™mø²D€ %…O×>“ãî‰JM@om	b@hî4Gé [^XGÜ‚J„!e>$zí[£H¡. ¢Ï	¶D8ˆ¤n4"I"ƒ$¹ŒèËëd†{ÁÁSğØÁoÌÖ9hbœs$AÌÖi@£ìEçÈmÅÚÕ˜>ú­b)¸Ö 
ïAÚÚÎmÚñ4€€¼ M8í©ç«Å©zhÒÅEÜ’ym•À:=CÛ‰`6í^#{-ÅÜ¤»+ĞzĞağ‡îEx?Dİ›a&‘Z¼Ea4ã| &îAë‚n¥£e#_ì} Ûºb_ŠÉ$}ïUltñØ@ÿç×¸÷\Tô#l3Í€])4Xë¡ããY»·±ï³&x>†ö ¨ŞÂ¾¿-iê’+³RN k(&¨Ç¼iÄ¿¢GM¢É](ÿZ¶>„vªUİ 3›¡0Ó%9Æ8€®¦áßçÛTzAªüˆÄ/ßƒu©³~eW*yô^ 1+Œg¾?¨#˜•ıx•™2‚/–=·û ŸXŒ7pRúÆÈl—K¼‚YƒáDĞ:x*œšÇDÕB@SÁ ÀÈ8%jƒ:"ßÂà`C-I®yÈy‹ı%Ì¢/
Œr*ôq\‹áŠf#Á¼@:(ƒµ.—ìPcÃåvD4»³øÿ“Ô¤qK<FÿÕ*U˜[ûÅIU¿ÔŸú<ø ”5ÂÕìÈ ´&ÃYY½'¬¨9ñ²p‹˜–å~Ûè¼PpÁÄ¾4õ3Dã5€T¯Gk¿Î¶¶ªê¿,¦Ô4©oÎÓg›À¼õ—şUœ¦¨~æï˜êø:¶™í§ï¿ÿKá´u»r¹ÚyyƒÑGëíO_‡‘få¶öÁ®½„[Tzí`‰Õ8LÑxE“Cfş)TZàJp`ËÁC¾£"3{}¶”cN,:z·RBO]Àÿ&¨BÇdœñµØë-»	”`m¼yL–Òƒ0I%f@€¦×¶oÔrÅ Sß^™m.¡×QPXjpUSş¢©÷6*×”sMß€ÉDû‚æ`á …÷æ Ñ)w°9í‚&„ıcn=´`P9Á\‹¹)
PäBØgt*Ä©¥À’¹pƒ¦Í¼Éb„9öÅD¬¬Í˜€£¡H_NºÿsêO.*éO8T@]ÇxõWµÀŒ)ì3ˆÖ\Şo„¥Aö?g·mÂß·Dù.9ÜŠñ0/°­ş˜á7æw´ 	¨bQ¾Y>b—mGå |H—×S€ pï<ám`OÂ	®—ğÂ›w¶Œ!„€0c@„`I¶ ¢¤Åô„\‹=$(8pAj2 =^N‹ Ãø,¦“»ÿZå>gr)ÄURošKÃû
©‡½å8	~f£ıwäá{)º­5oâuƒêšoüõÌ‘²ã4Ú–Ôq´QG½nÁ 8wÔiX†÷5‚>}9Q[Ÿ' l                      TEUUUU   »›    pp°€‹‰™ˆˆˆ‡§	©€€                                                                                ¸ 
	    ©€     wˆ	
º
  ‡¸ ª  w˜¨˜	 y†¸«©°°›z‡ˆ˜
 ‹ ‰v‡§¸«»	€v˜š
 Šv˜ˆ» 
°z	š
 
€                        	5ædQfÍ¯ 1‘nM¦"@p¨‰i
ËíQQéŸÜDw„¥aSåÿ5pª!h»?Î:µÓÕÉˆUk/*=•®—D¦Û¬Q Sl7LŸçĞ“ŸÇlŠ¸ç~:©?|‘²øEã ™l {Õo:N<‹O—şüáŠôûà`ÇP}ŸŠ£ÂvšçBgÿ'2,C¦Õn–ÁÃF£ÆõœÎ*œªpXñÜG¦pªHš¢B tIÚàõh¸£PZ –^{ ¢ï?]Çµ¢Tšİ/mØh;i\+Â~‰z3TT§µÃÅË©%u‰;Jâ$}¡€¥Œ4•-CQYÛ:Åé~P‚	×AC±4{kĞ*êz!İGQT%(’)ŠÑ4ÀÊKı;=è¯œÍ
C¬øø1=++<ŒüãÚî1t	â)£3_T˜.–J·‚b*²`±'ë+‚rIZ{PPóEm\%”Ş‡äĞo„ìJg¡ešË­e¿v^»ûtŒü<,›èÕ€ƒC9Óp¾!gœÁ8¸äˆe|¼»Í™34fƒÎ6ŸóŞÏáÕÙÙrÆÍµXŸfàòtà¢µ“Òƒ½µèİ‡+4½€°%""Bş§MÍ KK$È¤¶JœûÏ8^µ•8ö„÷^nŒ\Iôâf“¹yE\Ğ//k,šYvá.Òµ³EZf-&}İ@ˆLÂÈ 3¢P
ûÏ<ª¤
Ğ
 ¡‚SijïÁ