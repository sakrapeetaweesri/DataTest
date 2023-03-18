﻿using UnityEngine;
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
                glowOuter = material.GetFloat(ID_GlowOuter) * scaleRat�t�<z����'u�:zy������H�.3T��>Km��a�ϋ�Z���÷�Q��Q�2?��7̰D��q-��''��k�KS�{B.1ZS��HǢ�vq{�]���f���U}��Q�����xG.���\�zg5�ܮ<k�r��G-��pt/{��%]d��y'@%��ղ���h�2/��8�4��+�J�!Q�ґd���]���LH�A~mp��e
��YxzӶ�~��
~�F��f��#��oG�T<��Y���RwZ�>��1����?㷁BS���Fۍw~�(�⿜D�=$2_<	%|���,���[��d����|����
�����ƒ���w�Ӄ����N��b��r�pn��6�J��|�g�,#�C�����t�x�~�a�}���ַ���h��T���y�����^���
G��A׹3�%���|�)��U�=tn����N}gƆxOåm�[A|��-
�hd�Nn��Ű��e}�����V��d���A�K�W�.���պ=7�^��+]!T4g�Nt���@u���m�]�*(5���'k�D=Z�ALZ�Rl����Зׇ)(��3�p�Gb��b��7�֎��R�	�F"��5Q������a1�bX���b%Z�`\�
�l#_�2�ƀ�jh��&�]t7`�b>ñqhC]8���Բ��o1_�}y�C�Y��wdG�>��\Ƅj�c��µW�-Zś�T$����I��
6��(�$�ͩ�u^�e�9����RE'r��;�ۜ�>�`Y���j�>
0C����rUX7(1���']����Ȝk���X씴a�N�/�X����9����1~(�ţtlh�n;\,���׶E��V$�A��ɒ��˰z`!�/��`0�^�������i[-��l�k&|�3���M�F���+�{+!xA\��3����>�2�5A�ίoZy�-Ew.ӥ�;�g>���Vg�fd���v6���'��6��EZ�Tۀ�R(L�kr�T����f<{�sz0���G�                      eETUUU  �
��    � `� ��������x�	���                                                                                ��      �       ���
	   v�
���w�������w��
����v�������f�x�����f�	���{w����������  
�                        0����7X�s��B�ӊ#Mh��2�p)rr������˾�ɰ2j�(��п�*����z�N8.nD_��o���_'O/���v�'.��iO�Y�_u����0��~�܍�<�Cs]#�C���Q$gA�G� �+����ÿ 	H-��[��Nf�I֋
䪏<�@?�'h�Nk�����TB�u�kʨ�5�c O�D�+�)�`�.�OC�/�����w?�6��8����ظXkPp �x	���$~)��"�0tB^9����P.�!Ͳ�';g� ��%�r��'�����M��V���L}l�����NW�|�P
�K`H�n�<�N��\��-�P�V�Vuc����ګY����#�V.- ޠ�ae3�l���vf��I�5�>!�/Sؠ��8�Z@ƳTa��V���
0"#�V}T ��Ya���'�Z��9�0��]�����[Y�C�e�.Qĳ�A�oW������E�0�}���Z�0�(�������ϗ�x���,M��V��N���[��2e< E�%h��NDM���e��5�ڧ!@�� oX>X�T�['��gF ��J[��i}��#x�t�"I��#w�5�޻s�^��F�m���υh�P�An���B9~�,�<7�V�����%R�%�8�U���ݛ'+�����2m<R����R����a�p���U�Cl��p6���iq_�iCK�����N7�4�\h���Bsd�L-홒^:g'�Rh���t�!����r�Pgv-�Oϲqbn��N#���h��Mυs�z�]B��$c�/�� �+;�����
�WW�>Oۂpq��4:�[KAC�F�n
觋u��-*n>��K��ID�g;4���N*t\re}w[ ѝ2�EW�.F�&(:I��໌u
v[�[�>��졙|'�I^
�'0:�g}�}�$�Y�J��;hO���>)	4�,Y{3S)��A(���B��?��d"QS�h��j5��Zח����iu�ֲ��XW�����g��h6#��8>D;D��w�5��l�ܱ�&���u�m�6�[ǓW�5Z��I��1 ?ӟ}8ڶ%ڂ�,�o�.�;�z�Ntg��%U�W���*,|6k{�$���Y�%[�~���g��Uzu������s��>��f�J��?��:�t�W_��8�-{-8� ��V7§ ��/u��)0�>�Y�X�����w'���O�%d��^s6T�����J��vrI�3�TB�_'[&]��^�'��<���u�S|ZO4�bd�J?G;��;�ŋ����bP�+=�H������#��RA��z�6���uw���mitlT�OI��Uyb�w�.�,�_�x/�f�Y��n��q;'�d�9x�F����y�t�l�Y���q�	M���"��h��~޼����a�
CN3�oI�w��u����>M���}Un��1^�B�mf�{�,h��y[��Fł�/GI���uk��;������	�h>�_o�����,�l�z���$T��c٠�Cxr�����b� O�a0�|$@3@���)�<L�
����pMM-�?T#sC9ё.�7�n�sA2���#|��?܁�&��MR��NV�U}�����b��A�)::��l;ۜ)��J� Ѭ֚�)����䡌
�t�E�� ���>�)hʼ������x�O>zR�cs�~�J.�I
N��.�!/����O����9�"��fx� �q��f}�w �Ҕ�V����^�kǬ�x���B2$���]B�'Rd���R�^B�H�N$�dO�,��b��JJf��h�3E��"������ O4�"�o\=9����cǸ���!�ΐ�<B�-@���"�G)�ߦ�	;Z�0k��@![P/�3
���C��J�8Sn��A���& �قDD������~Fy�'Gg�����
�x��>=��W�j�A�#�lƳ��[U�X17�x�X\&�L	�Bd�ǯ����+����4�x�����zuh��7�����!M�'f`u���Tq����kfzڂ0��	D�h75���`֝���sBa\2Ďo8�C֨IS�)������[�@��3"�_ƌu��Q\f{�P%J�[G��֢�����7S��40XK�����6?������nB�����b f�
��\>|a�<�t����괌�s
s1Vղ@�/                      eEUUUf  �
��    
�

 `p p���x��xw�	�p�                                                                 
                      �
	     v��
�   �	� 
  �v �� � pv�	
�� yv��	�	 iv��� ��jx��

 �zv��
�� ���
� 
	p                        �)=
2%�:y�Q��
tB�sI`<1`��z�A�+jF���xl�X�K9�	�53,�
0V�s��ae�Z׶�`�lnE�@�NKY�$���Ѡ�fR�-�Rr�#��ӑhIA%Ӷ�Y笑��+)�?�p	���z��
b�˙;�7vT�/��>;�pOdwQ-� 9-���$�he���M�6*��Wl��w��+�xo�}��?�ft;��S'ڑ�B�`7���j
ju [�����L�R0ٗ>�Q�� 8���B���TÒ��ʕ�Ͱ���yn�|�R�f W i�	+��!�l�m����\X3�E� �9ծs�CH��+�L�7q����G��3Nj���G��M-(��M�ч3)��=�5z2��v&�~��OO�ղ�k��S�Qf��kȝ#�RCG9\�;6GW�͔�Η����Βk�I1��vvvF%�qb|9[*Xk���N��L,,'��rla�N�&�Tj���#{R�y�um�R���۸R���,e\�lz��}��S�w�T�v�aw��[�Vmב>���a ng�BccV��#ݚ� ��>ڼ�^aJ��9�r��&���q=���c1�v���J4�:�m 8��4��pYV�mO��?^(\�0�x(�%i.�t��k����_ǚ�s�Ս t�	F� u����5��B�:�:>ycV+�˞�aH]B�Q����C���A;��-<���-
��5�U��3M:!m�|�qy�e
|�1"ȑ���|^c��f"�ָ5��A�ŀN�X2�/�ZR\ӛ8�h����?��tS�}���
�����m!k��*vX�+
Z�~}6��qk�]�}{��� ���"�S�h���x����4a|�^f��FI���=�}G��^�K{�ЗU�F�Y����.)���W�Z�p8�|�Ao�p�shݓA�����[7�:����Xd�����l�|)�A)�)8+3G΀aAJ�J�bm-��2�:�9�����-�:br��
���+�/�{�����0hEa'Z�7��¼��m`�Q�{Ȟ�:�9��A������h�,� X�Bz�ptd�_T�ES�fa���1 ��-�N�^���s�'��L� ��İE���~��R���X����oA8lE�؅3��9�5"|C(��Pw�k��ļx�?���_s8o�4�'�)��� ��q�3�|�2ă߲g��u���"o� Q���g,d���`�W�VCC�?��b6����-+hG��Wc)��&�}                      eDTUTU  ���   � p� ����������	���                                                                                �  �    �     �����  ������v�	

 �v����������  ��v��	� �v���   pv��� ������ �                         ���B*@�A�Q.��YTV�ᯫ	��ǯ��= ~�;%��Q�0�Vd��!X1�d���i����`s �vd��Gm=/w'WZUi�X�UȔj��E�ję� �����*=Y ������@<3�s#�y*����E���	��g���w50/�ſd�����&*��E��b��;�Cq�[��Ľd�g����L���#���u��l?��N΢E�/Z����Q�.*v���C����������ș�Zyu�s���	ؙ��+g��t����Hn����C*��*���bcnF-'(W{��+�H�FB�[��g�[Ő�*C��Tbc_�k�q������u�r33�z�8'=}�C�h(Nt^~�23/�WW���Ձ?���l�ƼW��g�v'X�)�<��5�  ��G|�����Y9]�����Z�nw�nMgո^��J�����_/eu ]�3��4S��[��b���������+&e�y�]y݉�4�L�rR��i��\Q�#"��,��5�/���P��\����w�ϳ���Q��l��)��\�o�ä��ڳ5*��i��V2��~M��2N��"3nI�s35_2)3!�S13�ٙCI�8P���3��W���C��n2���_�c�� QU�Q���:��- #�5��F���7�5� �?
�iu�O�M_�[0�"����I����o_t����%B��Y�,>FScc�����;�2^]�D)d5�MU��" j��cj�n�Ќ�������!f���ɱ���ـ���5�&5GTB�,��JmH]b�߭�B�v��\+b܁����t�@T�J!Rd�n0�3c�9)nۊ����L�W�n3`�G�ۡ��φ�c�]1���50����Sê�6	�ݙm��D��%�O�>���JM@�om	b@h�4G� [^XG܂J�!e>$z�[�H�. ��	�D8��n4"I"�$�����d�{��S���o��9hb�s$A��i@��E��m��՘>��b)�֠
�Aڎ��m��4����M8����zh��Eܒym��:=Cۉ`6�^#{-�ܤ�+�z�a���Ex?Dݛa&�Z�Ea4�| &�A�n��e#_�}�ۺb_��$}�Ult�؏@��׸�\T�#l3��])4X���Y����&x�>�� ��¾�-i�+�RN k(&�ǼiĿ�GM��](�Z�>�v�U� 3��0�%9�8�������TzA����/߃u��~eW*y�^ 1+�g�?��#���x��2�/�=�� �X�7pR���l���K��Y��D�:x*����D�B@S� ��8%j�:"���`C-I�y�y��%��/
�r*�q\���f#��@:(��.��Pc��v�D4������ԤqK<F��*U�[��IU�ԟ�<� �5���� �&�YY��'���9�p����~��Pp�ľ4�3D�5�T�Gk�ζ���,��4�o��g������U���~����:�����K�u�r��yy��G��O_��f������[Tz�`��8LѐxE�Cf�)�TZ��Jp`��C��"3{}��cN,:z�RBO]��&�B�d����-�	�`m�yL��҃0I%f@��׶o�r� S�^�m.��QPXjpUS����6*��sM߀�D���`᠅�� �)w�9�&��cn=�`P9�\��)
P�B�gt*ĩ����p��ͼ�b�9��D�������H_N��s�O.*�O8T@]�x�W���)�3��\�o��A�?g�m���D�.9܊�0/�����7��w� 	�bQ�Y>b�mG� |H��S��p�<��m`O�	����w��!��0c@�`I� �����\�=$(8pAj2 =^N� ��,����Z�>gr)�UR�o�K��
����8	~f���w���{)��5o�u���o��̑��4ږ�q�QG�n��8w�iX��5�>}9Q[�' l�                      TEUUUU  ���  � pp�����������	���                                                                                � 
	    ��     w�	
�
 ��� �  w���	��y�������z���
�� �v�����	�v���
 �v����
�z�	�
 
�                        	5�dQfͯ 1�nM�"@p��i
��QQ��Dw��aS��5p�!h�?�:���ɈUk/*=���D�۬Q Sl7L��Г��l����~:�?|���E� �l {�o:N<��O��������`�P}����v��Bg�'2,C��Վn���F�����*��pX��G�p�H��B tI���h��PZ��^{���?]ǵ�T���/m�h;i\+�~�z3TT����˩%u�;J�$}�����4�-CQY�:���~P�	�AC�4{k�*�z!�GQT%(�)��4��K�;=诜�
C���1=++<�����1t	�)�3_T�.�J��b*�`�'�+�r�IZ{PP�Em\%�އ��o��Jg�e�ˏ�e�v�^��t��<,��Հ�C9�p�!g��8��e|��͙34f��6��������r�͵X�f��t����҃���݇+4���%""B��M� KK$���J���8^��8���^n�\I��f��yE\�//k,�Yv��.ҵ�EZf-&}�@�L���3�P
��<��
�
 ��Sij��