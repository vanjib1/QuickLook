﻿// Copyright © 2018 Paddy Xu
// 
// This file is part of QuickLook program.
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using QuickLook.Common.ExtensionMethods;

namespace QuickLook.Plugin.ImageViewer.AnimatedImage
{
    internal class GifAnimationProvider : AnimationProvider
    {
        private Bitmap _frame;
        private BitmapSource _frameSource;
        private bool _isPlaying;

        public GifAnimationProvider(string path, Dispatcher uiDispatcher) : base(path, uiDispatcher)
        {
            _frame = (Bitmap) Image.FromFile(path);
            _frameSource = _frame.ToBitmapSource();

            Animator = new Int32AnimationUsingKeyFrames {RepeatBehavior = RepeatBehavior.Forever};

            Animator.KeyFrames.Add(new DiscreteInt32KeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
            Animator.KeyFrames.Add(new DiscreteInt32KeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(50))));
            Animator.KeyFrames.Add(new DiscreteInt32KeyFrame(2, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100))));
        }

        public override void Dispose()
        {
            if (_frame == null)
                return;

            ImageAnimator.StopAnimate(_frame, OnFrameChanged);
            _frame.Dispose();

            _frame = null;
            _frameSource = null;
        }

        public override Task<BitmapSource> GetRenderedFrame(int index)
        {
            if (!_isPlaying)
            {
                _isPlaying = true;
                ImageAnimator.Animate(_frame, OnFrameChanged);
            }

            return new Task<BitmapSource>(() => _frameSource);
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            ImageAnimator.UpdateFrames();
            _frameSource = _frame.ToBitmapSource();
        }
    }
}
