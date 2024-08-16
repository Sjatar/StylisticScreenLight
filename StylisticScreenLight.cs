using System;
using System.Linq;
using UnityEngine;
using Klak.Spout;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using VNyanInterface;

namespace Sja_StylisticScreenLight
{
    public class StylisticScreenLight : MonoBehaviour 
    {
        [Header("Dimensions of source:")]
        [SerializeField] SpoutResources _resources = null;
        [SerializeField] public String spoutSourceName = "screen";
        [SerializeField] public int widthBase = 1920;
        [SerializeField] public int heightBase = 1080;
        [SerializeField] public int vNyanXOffset = 0;
        [SerializeField] public int vNyanYOffset = 0;
        [SerializeField] public int spoutXOffset = 0;
        [SerializeField] public int spoutYOffset = 0;

        [Header("Compose Mask")] 
        [SerializeField] public float baseValue = (float)0.2;
        [SerializeField] public float inputValue = (float)2;
        [SerializeField] public float highLightValue = (float)5;
        [SerializeField] public float highLightOffsetX = (float)10;
        [SerializeField] public float highLightOffsetY = (float)10;
        [SerializeField] public float brightnessThreshold = (float)0.5;
        
        [Header("Gaussian shader")]
        [SerializeField] public int quality = 64;
        [SerializeField] public int iterationsGaussPub = 3;
        private static int iterationsGauss;

        [Header("Kawase blur:")] [SerializeField]
        public int iterationsKawPub = 3;
        private static int iterationsKaw;
        
        [Header("Shaders to use:")] 
        [SerializeField] public Shader gaussShader;
        [SerializeField] public Shader dualKawase;
        [SerializeField] public Shader comShader;
        
        private static Material gaussMat;
        private static Material kawaseMat;
        private static Material comMat;

        private static RenderTexture screenTexture;
        
        private GameObject spoutRec;
        
        private static IParameterInterface para = VNyanInterface.VNyanInterface.VNyanParameter;
        
        public class RenderEffectClass : IRenderEffect
        {
            void IRenderEffect.onRenderEffect(RenderTexture source, RenderTexture destination)
            {
                int width = source.width;
                int height = source.height;
                int widthScreen = 1920;
                int heightScreen = 1080;
                RenderTexture bufferTex1;
                RenderTexture bufferTex2;
                gaussMat.SetTexture("_Alpha", source);
                
                bufferTex1 = RenderTexture.GetTemporary(widthScreen, heightScreen);
                Graphics.Blit(screenTexture, bufferTex1);
                
                for (int i = 0; i < iterationsKaw; i += 1)
                {
                    widthScreen /= 2;
                    heightScreen /= 2;
                    bufferTex2 = RenderTexture.GetTemporary((int)Math.Floor((float)widthScreen), (int)Math.Floor((float)widthScreen));
                    Graphics.Blit(bufferTex1, bufferTex2, kawaseMat, 0);
                    RenderTexture.ReleaseTemporary(bufferTex1);
                
                    widthScreen /= 2;
                    heightScreen /= 2;
                    bufferTex1 = RenderTexture.GetTemporary((int)Math.Floor((float)widthScreen), (int)Math.Floor((float)heightScreen));
                    Graphics.Blit(bufferTex2, bufferTex1, kawaseMat, 0);
                    RenderTexture.ReleaseTemporary(bufferTex2);
                }

                for (int i = 0; i < iterationsKaw; i += 1)
                {
                    widthScreen *= 2;
                    heightScreen *= 2;
                    bufferTex2 = RenderTexture.GetTemporary((int)Math.Floor((float)widthScreen), (int)Math.Floor((float)heightScreen));
                    Graphics.Blit(bufferTex1, bufferTex2, kawaseMat, 1);
                    RenderTexture.ReleaseTemporary(bufferTex1);
                
                    widthScreen *= 2;
                    heightScreen *= 2;
                    bufferTex1 = RenderTexture.GetTemporary((int)Math.Floor((float)widthScreen), (int)Math.Floor((float)heightScreen));
                    Graphics.Blit(bufferTex2, bufferTex1, kawaseMat, 1);
                    RenderTexture.ReleaseTemporary(bufferTex2);
                }
                
                Graphics.Blit(bufferTex1, screenTexture);
                RenderTexture.ReleaseTemporary(bufferTex1);

                bufferTex1 = RenderTexture.GetTemporary(width, height);
                bufferTex2 = RenderTexture.GetTemporary(width, height);
                Graphics.Blit(source, bufferTex1);
                
                for (int i = 0; i < iterationsGauss; i += 1)
                {
                    Graphics.Blit(bufferTex1, bufferTex2, gaussMat, 0);
                    Graphics.Blit(bufferTex2, bufferTex1, gaussMat, 1);
                }
                RenderTexture.ReleaseTemporary(bufferTex2);
                comMat.SetTexture("_HighLightTex", bufferTex1);
                
                Graphics.Blit(source, destination, comMat);
                
                    // RenderTexture.ReleaseTemporary(bufferTex1);
                    //
                    // bufferTex1 = RenderTexture.GetTemporary(width, height);
                    //
                    // Graphics.Blit(source, bufferTex1, gaussMat, 0);
                    // Graphics.Blit(bufferTex1, destination,gaussMat,1);
                RenderTexture.ReleaseTemporary(bufferTex1);
            }

            bool IRenderEffect.isEffectActive()
            {
                return VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("SSLBool") == 1.0;
            }

        }

        class gaussArraysStruc
        {
            public float[] gaussWeights { get; set; }
            public float[] gaussOffsets { get; set; }
            public int gaussArrayLength { get; set; }
        }
        private gaussArraysStruc calcGaussArrays(int funQuality)
        {
            int length = (int)Math.Floor((float)funQuality / 2) + 1;
            float[] weightsBuf = new float[length];
            float[] weights = new float[length];
            weightsBuf[0] = 1;
            weightsBuf.CopyTo(weights, 0);
                    
            for (int i = 0; i < funQuality; i +=1)
            {
                if (i % 2 == 0)
                {
                    for (int j = 0; j < weightsBuf.Length - 1; j += 1)
                    {
                        weights[j] = weightsBuf[j] + weightsBuf[j + 1];
                    }
                }
                else
                {
                    weights[0] = weightsBuf[0] * 2;
                    for (int j = 1; j < weightsBuf.Length; j += 1)
                    {
                        weights[j] = weightsBuf[j - 1] + weightsBuf[j];
                    }
                }
                weights.CopyTo(weightsBuf, 0);
            }

            length = 0;
            
            for (int i = 0; i < weights.Length-1; i += 1)
            {
                if (weights[i] / weights[0] >= 0.05)
                {
                    length += 1;
                }
            }
            
            float sum = 0;

            for (int i = 0; i < length; i += 1)
            {
                sum += weights[i];
            }
            
            for (int i = 0; i < length; i += 1)
            {
                weights[i] /= 2*sum;
            }
            
            int oldLength = length;
            length = (int)Math.Floor((float)length / 2) + 1;
            float[] weightsRed = new float[length];
            float[] offset = new float[length];
            
            if (oldLength % 2 != 0)
            {
                weightsRed[weightsRed.Length-1] = weights[oldLength-1];
                
                if (funQuality % 2 == 0)
                {
                    offset[offset.Length - 1] = oldLength-1;
                }
                else
                {
                    offset[offset.Length - 1] = oldLength - (float)0.5;
                }
            }
            else
            {
                weightsRed = new float[length-1];
                offset = new float[length-1];
            }
            
            for (int i = 0; i < length-1; i += 1)
            {
                weightsRed[i] = weights[(i*2)] + weights[(i*2) + 1];
                if (funQuality % 2 == 0)
                {
                    offset[i] = (weights[i*2] * (i*2) + weights[i*2 + 1] * (i*2 + 1)) / weightsRed[i];
                }
                else
                {
                    offset[i] = (weights[i*2] * ((i*2) + (float)0.5) + weights[(i*2) + 1] * ((i * 2 + 1) + (float)0.5)) / weightsRed[i];
                }
                
            }
            
            float[] weightsRedOut = {0,0,0,0,0,0,0};
            weightsRed.CopyTo(weightsRedOut, 0);

            float[] offsetOut = {0,0,0,0,0,0,0};
            offset.CopyTo(offsetOut, 0);
            
            return new gaussArraysStruc { gaussWeights = weightsRedOut, gaussOffsets = offsetOut, gaussArrayLength = weightsRed.Length};
        }
        
        // Start is called before the first frame update
        void Start()
        {
            if (VNyanInterface.VNyanInterface.VNyanParameter != null)
            {
                para.setVNyanParameterFloat("SSLBool", 1);
                para.setVNyanParameterFloat("SSLQuality", quality);
                para.setVNyanParameterFloat("SSLIterationsKaw",iterationsKawPub);
                para.setVNyanParameterFloat("SSLIterationsGauss", iterationsGaussPub);
                para.setVNyanParameterFloat("SSLBaseValue", baseValue);
                para.setVNyanParameterFloat("SSLInputValue", inputValue);
                para.setVNyanParameterFloat("SSLHighLightValue", highLightValue);
                para.setVNyanParameterFloat("SSLHighLightOffsetX", highLightOffsetX);
                para.setVNyanParameterFloat("SSLHighLightOffsetY", highLightOffsetY);
                para.setVNyanParameterFloat("SSLBrightnessThreshold", brightnessThreshold);
               para.setVNyanParameterString("SSLSpoutSourceName", spoutSourceName);
               
                para.setVNyanParameterFloat("SSLSpoutWidth", widthBase);
                para.setVNyanParameterFloat("SSLSpoutHeight", heightBase);
                para.setVNyanParameterFloat("SSLVNyanXOffset", vNyanXOffset);
                para.setVNyanParameterFloat("SSLVNyanYOffset", vNyanYOffset);
                para.setVNyanParameterFloat("SSLSpoutXOffset", spoutXOffset);
                para.setVNyanParameterFloat("SSLSpoutYOffset", spoutYOffset);
            }
            
            screenTexture = new RenderTexture(widthBase, heightBase, 0);
                screenTexture.Create();
            
            gaussMat = new Material(gaussShader);
                gaussArraysStruc gaussArrays = calcGaussArrays(quality);
                gaussMat.SetFloatArray("_GaussWeights", gaussArrays.gaussWeights);
                gaussMat.SetFloatArray("_GaussOffsets", gaussArrays.gaussOffsets);
                gaussMat.SetInt("_ArrayLength", gaussArrays.gaussArrayLength);
                iterationsGauss = iterationsGaussPub;  
                //Debug.Log($"[{string.Join(", ", gaussArrays.gaussWeights)}]");
                //Debug.Log($"[{string.Join(", ", gaussArrays.gaussOffsets)}]");

            kawaseMat = new Material(dualKawase);
                iterationsKaw = iterationsKawPub;    
            
            comMat = new Material(comShader);
                comMat.SetTexture("_ScreenTex", screenTexture);
                comMat.SetFloat("_BaseValue", baseValue);
                comMat.SetFloat("_InputValue", inputValue);
                comMat.SetFloat("_HighLightValue", highLightValue);
                comMat.SetFloat("_HighLightOffsetX", highLightOffsetX);
                comMat.SetFloat("_HighLightOffsetY", highLightOffsetY);
                comMat.SetFloat("_BrightnessThreshold", brightnessThreshold);
                comMat.SetFloat("_MainXOffset", vNyanXOffset);
                comMat.SetFloat("_MainYOffset", vNyanYOffset);
                comMat.SetFloat("_ScreenXOffset", spoutXOffset);
                comMat.SetFloat("_ScreenYOffset", spoutYOffset);
                
            
            spoutRec = new GameObject("Spout Receiver");
            spoutRec.AddComponent<SpoutReceiver>();
            
            var receiver = spoutRec.GetComponent<SpoutReceiver>();
            receiver.SetResources(_resources);
            receiver.sourceName = spoutSourceName;
            receiver.targetTexture = screenTexture;

            IRenderEffect renderEffect = new RenderEffectClass();
            
            if ( VNyanInterface.VNyanInterface.VNyanRender != null )
            {
                VNyanInterface.VNyanInterface.VNyanRender.registerRenderEffect(renderEffect);
            }
        }

        private void Update()
        {
            if ( VNyanInterface.VNyanInterface.VNyanParameter != null )
            {
                int qualityNew =                 (int)para.getVNyanParameterFloat("SSLQuality");
                float baseValueNew =           (float)para.getVNyanParameterFloat("SSLBaseValue");
                float inputValueNew =          (float)para.getVNyanParameterFloat("SSLInputValue");
                float highLightValueNew =      (float)para.getVNyanParameterFloat("SSLHighLightValue");
                float highLightOffsetXNew =    (float)para.getVNyanParameterFloat("SSLHighLightOffsetX");
                float highLightOffsetYNew =    (float)para.getVNyanParameterFloat("SSLHighLightOffsetY");
                float brightnessThresholdNew = (float)para.getVNyanParameterFloat("SSLBrightnessThreshold");
                
                string spoutSourceNameNew =  (string)para.getVNyanParameterString("SSLSpoutSourceName");
                int widthBaseNew =              (int)para.getVNyanParameterFloat("SSLSpoutWidth");
                int heightBaseNew =             (int)para.getVNyanParameterFloat("SSLSpoutHeight");
                int vNyanXOffsetNew =           (int)para.getVNyanParameterFloat("SSLVNyanXOffset");
                int vNyanYOffsetNew =           (int)para.getVNyanParameterFloat("SSLVNyanYOffset");
                int spoutXOffsetNew =           (int)para.getVNyanParameterFloat("SSLSpoutXOffset");
                int spoutYOffsetNew =           (int)para.getVNyanParameterFloat("SSLSpoutYOffset");
                
                iterationsGauss =                (int)para.getVNyanParameterFloat("SSLIterationsGauss");
                iterationsKaw =                  (int)para.getVNyanParameterFloat("SSLIterationsKaw");

                if (spoutSourceNameNew != spoutSourceName)
                {
                    var receiver = spoutRec.GetComponent<SpoutReceiver>();
                    receiver.sourceName = spoutSourceNameNew;
                    spoutSourceName = spoutSourceNameNew;
                }

                if (vNyanXOffsetNew != vNyanXOffset)
                {
                    comMat.SetFloat("_MainXOffset", vNyanXOffsetNew);
                    vNyanXOffset = vNyanXOffsetNew;
                }

                if (vNyanYOffsetNew != vNyanYOffset)
                {
                    comMat.SetFloat("_MainYOffset", vNyanYOffsetNew);
                    vNyanYOffset = vNyanYOffsetNew;
                }

                if (spoutXOffsetNew != spoutXOffset)
                {
                    comMat.SetFloat("_ScreenXOffset", spoutXOffsetNew);
                    spoutXOffset = spoutXOffsetNew;
                }

                if (spoutYOffsetNew != spoutYOffset)
                {
                    comMat.SetFloat("_ScreenYOffset", spoutYOffsetNew);
                    spoutYOffset = spoutYOffsetNew;
                }
                
                if (widthBaseNew != widthBase)
                {
                    screenTexture.Release();
                    screenTexture.width = widthBaseNew;
                    screenTexture.Create();
                    comMat.SetTexture("_ScreenTex", screenTexture);
                    widthBase = widthBaseNew;
                }
                
                if (heightBaseNew != heightBase)
                {
                    screenTexture.Release();
                    screenTexture.height = heightBaseNew;
                    screenTexture.Create();
                    comMat.SetTexture("_ScreenTex", screenTexture);
                    heightBase = heightBaseNew;
                }
                
                if (qualityNew != quality)
                {
                    gaussArraysStruc gaussArrays = calcGaussArrays(qualityNew);
                    
                    gaussMat.SetFloatArray("_GaussWeights", gaussArrays.gaussWeights);
                    gaussMat.SetFloatArray("_GaussOffsets", gaussArrays.gaussOffsets);
                    gaussMat.SetInt("_ArrayLength", gaussArrays.gaussArrayLength);
                    
                    quality = qualityNew;
                }

                if (brightnessThresholdNew != brightnessThreshold)
                {
                    comMat.SetFloat("_BrightnessThreshold", brightnessThresholdNew);
                    brightnessThreshold = brightnessThresholdNew;
                }
                
                if (baseValueNew != baseValue)
                {
                    comMat.SetFloat("_BaseValue", baseValueNew);
                    baseValue = baseValueNew;
                }

                if (inputValueNew != inputValue)
                {
                    comMat.SetFloat("_InputValue", inputValueNew);
                    inputValue = inputValueNew;
                }
                
                if (highLightValueNew != highLightValue)
                {
                    comMat.SetFloat("_HighLightValue", highLightValueNew);
                    highLightValue = highLightValueNew;
                }

                if (highLightOffsetXNew != highLightOffsetX)
                {
                    comMat.SetFloat("_HighLightOffsetX", highLightOffsetXNew);
                    highLightOffsetX = highLightOffsetXNew;
                }
                
                if (highLightOffsetYNew != highLightOffsetY)
                {
                    comMat.SetFloat("_HighLightOffsetY", highLightOffsetYNew);
                    highLightOffsetY = highLightOffsetYNew;
                }
            }
        }
    }
}