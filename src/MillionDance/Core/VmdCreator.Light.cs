﻿using System.Collections.Generic;
using System.Linq;
using Imas.Data.Serialized;
using JetBrains.Annotations;
using OpenMLTD.MillionDance.Entities.Vmd;
using OpenTK;

namespace OpenMLTD.MillionDance.Core {
    partial class VmdCreator {

        [NotNull]
        public VmdMotion CreateLight([CanBeNull] ScenarioObject lights) {
            IReadOnlyList<VmdLightFrame> frames;

            if (ProcessLightFrames && lights != null) {
                frames = CreateLightFrames(lights);
            } else {
                frames = null;
            }

            return new VmdMotion(ModelName, null, null, null, frames, null);
        }

        [NotNull, ItemNotNull]
        private IReadOnlyList<VmdLightFrame> CreateLightFrames([NotNull] ScenarioObject scenarioObject) {
            var lightFrameList = new List<VmdLightFrame>();
            var lightControls = scenarioObject.Scenario.Where(s => s.Type == ScenarioDataType.LightColor).ToArray();

            foreach (var lightControl in lightControls) {
                var n = (int)((float)lightControl.AbsoluteTime * 60.0f);
                int frameIndex;

                if (_conversionConfig.Transform60FpsTo30Fps) {
                    frameIndex = n / 2;
                } else {
                    frameIndex = n;
                }

                var frame = new VmdLightFrame(frameIndex);

                frame.Position = new Vector3(0.5f, -1f, -0.5f);

                var c = lightControl.Color;
                frame.Color = new Vector3(c.R, c.G, c.B);

                lightFrameList.Add(frame);
            }

            return lightFrameList;
        }

    }
}
