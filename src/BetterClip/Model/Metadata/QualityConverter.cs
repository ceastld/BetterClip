// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using BetterClip.Extension;
using BetterClip.Model.Intrinsic;
using BetterClip.View.Controls;

namespace BetterClip.View.Converters;

internal sealed class QualityConverter : ValueConverter<QualityType, Uri>
{
    /// <inheritdoc/>
    public override Uri Convert(QualityType from)
    {
        string name = Enum.GetName(from) ?? from.ToString();
        if (name == nameof(QualityType.QUALITY_ORANGE_SP))
        {
            name = "QUALITY_RED";
        }

        return Web.HutaoEndpoints.StaticRaw("Bg", $"UI_{name}.png").ToUri();
    }
}
