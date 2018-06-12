﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

using Autodesk.Max;
using Autodesk.Max.IColorManager;
using BatchLabs.Max2019.Plugin.Common;
using BatchLabs.Plugin.Common.Code;
using BatchLabs.Plugin.Common.Contract;
using BatchLabs.Plugin.Common.Resources;

using MediaColor = System.Windows.Media.Color;

namespace BatchLabs.Max2019.Plugin.Max
{
    public class MaxRequestInterceptor : IMaxRequestHandler
    {
        public string CurrentSceneFilePath => MaxGlobalInterface.Instance.COREInterface16.CurFilePath;

        public string CurrentSceneFileName => MaxGlobalInterface.Instance.COREInterface16.CurFileName;

        public int RenderWidth => MaxGlobalInterface.Instance.COREInterface16.RendWidth;

        public int RenderHeight => MaxGlobalInterface.Instance.COREInterface16.RendHeight;

        public int FrameStart => MaxGlobalInterface.Instance.COREInterface16.RendStart;

        public Brush GetUiColorBrush(BrushColorEnum brushColor)
        {
            var colorManager = MaxGlobalInterface.Instance.ColorManager;
            switch (brushColor)
            {
                case BrushColorEnum.Text:
                    return GetUiColorBrush(colorManager, GuiColors.Text);
                case BrushColorEnum.InputBox:
                    return GetUiColorBrush(colorManager, GuiColors.Window);
                case BrushColorEnum.Spinner:
                    return GetUiColorBrush(colorManager, GuiColors.TrackViewScaleOriginLine);
                case BrushColorEnum.Warning:
                    return GetUiColorBrush(colorManager, GuiColors.TrackViewAutoTangentHandle);
                case BrushColorEnum.Window:
                    return GetUiColorBrush(colorManager, GuiColors.Background);
                default:
                    throw new NotImplementedException();
            }
        }

        public IEnumerable<Tuple<string, Brush>> GetAllColorBrushes()
        {
            var colors = Enum.GetValues(typeof(GuiColors)).Cast<GuiColors>().OrderBy(color => color.ToString());
            var colorManager = MaxGlobalInterface.Instance.ColorManager;
            foreach (var guiColor in colors)
            {
                yield return new Tuple<string, Brush>(guiColor.ToString(), GetUiColorBrush(colorManager, guiColor));
            }
        }

        public async Task<List<IAssetFile>> GetFoundSceneAssets()
        {
            try
            {
                var assetCallback = new AssetWranglerCallback();
                await Task.Run(() =>
                {
                    MaxGlobalInterface.Instance.COREInterface16.EnumAuxFiles(assetCallback, AssetSearchFlags.FileEnumAll);
                });

                return assetCallback.AssetList;
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"{Strings.SubmitJob_Assets_Error}: {ex.Message}. {ex}");
            }

            return new List<IAssetFile>();
        }

        public async Task<List<IAssetFile>> GetMissingAssets()
        {
            try
            {
                var assetCallback = new AssetWranglerCallback();
                await Task.Run(() =>
                {
                    MaxGlobalInterface.Instance.COREInterface16.EnumAuxFiles(assetCallback, AssetSearchFlags.FileEnumMissingActive);
                });

                return assetCallback.AssetList;
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"{Strings.SubmitJob_MissingAssets_Error}: {ex.Message}. {ex}");
            }


            return new List<IAssetFile>();
        }

        /// <summary>
        /// Get the brush color associated with the desired GuiColor and match our
        /// dialog style colors to it.
        /// </summary>
        /// <param name="colorManager"></param>
        /// <param name="guiColor"></param>
        /// <returns></returns>
        private static Brush GetUiColorBrush(IIColorManager colorManager, GuiColors guiColor)
        {
            var color = colorManager.GetColor(guiColor, State.Normal);
            var mcolorText = MediaColor.FromRgb(color.R, color.G, color.B);

            return new SolidColorBrush(mcolorText);
        }
    }
}
