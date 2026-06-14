using UnityEngine;

public static class LinkValidator
{
    public static bool IsLinkValid(int rayA, int indexA, int rayB, int indexB)
    {
        if (rayA == -1 || rayB == -1) return false;

        // Chebyshev(8yönlü) mesafe hesabýnda birleþtiriyoruz
        int distance = Mathf.Max(Mathf.Abs(rayA - rayB), Mathf.Abs(indexA - indexB));

        // Eðer mesafe tam olarak 1 ise; bu noktalar yatay, dikey veya çapraz komþudur.
        // Eðer mesafe 0 ise (ayný nokta) veya 1'den büyükse (uzak) false döner.
        return distance == 1;
    }
}