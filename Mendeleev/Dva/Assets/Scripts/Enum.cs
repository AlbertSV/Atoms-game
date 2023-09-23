using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dva
{
    public enum GeneralParticleType
    {
        Electron,
        Proton,
        Neutron
    }

    public enum ParticleState
    {
        InAtom,
        Free
    }

    public enum AtomState
    {
        Neutral,
        Minus,
        Plus
    }

    public enum SpecialParticleType
    {
        BlackHole,
        TimeFast,
        TimeSlow,
        FastNeutron,
        FieldRise,
        FiledShrink
    }

    public enum Materials
    {
        Actinides = 1,
        AlkaliMetal = 2,
        AlkalineEarthMetal = 3,
        Lantanides = 4,
        Metalloids = 5,
        NobleGas = 6,
        PostTransitionMetals = 7,
        ReactiveNonmetals = 8,
        TransitionMetals = 9,
        UnknownProperties = 10
    }
}