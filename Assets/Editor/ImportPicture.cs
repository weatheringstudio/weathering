

//namespace Weathering
//{
//    using System.IO;
//    using System.Reflection;
//    using UnityEditor;
//    using UnityEngine;

//    public class ImportPicture : AssetPostprocessor
//    {
//        void OnPreprocessTexture() {
//            TextureImporter importer = assetImporter as TextureImporter;
//            if (importer != null) {
//                if (IsFirstImport(importer)) {
//                    //importer.textureType = TextureImporterType.Sprite;
//                    //TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("iPhone");
//                    //bool isPowerOfTwo = IsPowerOfTwo(importer);
//                    //TextureImporterFormat defaultAlpha = isPowerOfTwo ? TextureImporterFormat.PVRTC_RGBA4 : TextureImporterFormat.ASTC_RGBA_4x4;
//                    //TextureImporterFormat defaultNotAlpha = isPowerOfTwo ? TextureImporterFormat.PVRTC_RGB4 : TextureImporterFormat.ASTC_RGB_6x6;
//                    //settings.overridden = true;
//                    //settings.format = importer.DoesSourceTextureHaveAlpha() ? defaultAlpha : defaultNotAlpha;
//                    //importer.SetPlatformTextureSettings(settings);

//                    //settings = importer.GetPlatformTextureSettings("Android");
//                    //settings.overridden = true;
//                    //settings.allowsAlphaSplitting = false;
//                    //bool divisible4 = IsDivisibleOf4(importer);
//                    //defaultAlpha = divisible4 ? TextureImporterFormat.ETC2_RGBA8Crunched : TextureImporterFormat.ASTC_RGBA_4x4;
//                    //defaultNotAlpha = divisible4 ? TextureImporterFormat.ETC_RGB4Crunched : TextureImporterFormat.ASTC_RGB_6x6;
//                    //settings.format = importer.DoesSourceTextureHaveAlpha() ? defaultAlpha : defaultNotAlpha;
//                    //importer.SetPlatformTextureSettings(settings);

//                    importer.spriteImportMode = SpriteImportMode.Single;
//                    importer.spritePixelsPerUnit = 16;
//                    importer.filterMode = FilterMode.Point;
//                    importer.textureCompression = TextureImporterCompression.Uncompressed;
//                }
//            }
//        }
//        ////被4整除
//        //bool IsDivisibleOf4(TextureImporter importer) {
//        //    (int width, int height) = GetTextureImporterSize(importer);
//        //    return (width % 4 == 0 && height % 4 == 0);
//        //}

//        ////2的整数次幂
//        //bool IsPowerOfTwo(TextureImporter importer) {
//        //    (int width, int height) = GetTextureImporterSize(importer);
//        //    return (width == height) && (width > 0) && ((width & (width - 1)) == 0);
//        //}

//        //贴图不存在、meta文件不存在、图片尺寸发生修改需要重新导入
//        bool IsFirstImport(TextureImporter importer) {
//            (int width, int height) = GetTextureImporterSize(importer);
//            Texture tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
//            bool hasMeta = File.Exists(AssetDatabase.GetAssetPathFromTextMetaFilePath(assetPath));
//            return tex == null || !hasMeta || (tex.width != width && tex.height != height);
//        }

//        //获取导入图片的宽高
//        (int, int) GetTextureImporterSize(TextureImporter importer) {
//            if (importer != null) {
//                object[] args = new object[2];
//                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
//                mi.Invoke(importer, args);
//                return ((int)args[0], (int)args[1]);
//            }
//            return (0, 0);
//        }
//    }
//}

