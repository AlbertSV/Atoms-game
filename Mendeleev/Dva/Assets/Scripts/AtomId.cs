namespace Dva
{
    // An atom's composition (neutrons/electrons/protons) is packed into a single int ID:
    // Base + neutrons * NeutronScale + electrons * ElectronScale + protons.
    public static class AtomId
    {
        public const int Base = 1_000_000_000;
        public const int NeutronScale = 1_000_000;
        public const int ElectronScale = 1_000;

        public static int Encode(int neutrons, int electrons, int protons)
        {
            return Base + neutrons * NeutronScale + electrons * ElectronScale + protons;
        }

        public static void Decode(int atomId, out int neutrons, out int electrons, out int protons)
        {
            int packed = atomId - Base;
            neutrons = packed / NeutronScale;
            electrons = (packed % NeutronScale) / ElectronScale;
            protons = (packed % NeutronScale) % ElectronScale;
        }
    }
}
