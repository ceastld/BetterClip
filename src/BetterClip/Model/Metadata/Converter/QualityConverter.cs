// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using BetterClip.Control;
using BetterClip.Extension;
using BetterClip.Model.Intrinsic;

namespace BetterClip.Model.Metadata.Converter;

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
