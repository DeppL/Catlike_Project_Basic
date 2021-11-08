using Unity.Mathematics;

using static Unity.Mathematics.math;

public static partial class Noise {
	public struct Voronoi1D<L, D, F> : INoise
		where L : struct, ILattice
		where D : struct, IVoronoiDistance
		where F : struct, IVoronoiFunction {

		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
			var l = default(L);
			var d = default(D);
			LatticeSpan4 x = default(L).GetLatticeSpan4(positions.c0, frequency);

			float4x2 minima = 2f;
			for (var u = -1; u <= 1; u++) {
				SmallXXHash4 h = hash.Eat(l.ValidateSingleStep(x.p0 + u, frequency));
				minima = UpdateVoronoiMinima(minima, d.GetDistance(h.Floats01A + u - x.g0));
			}
			return default(F).Evaluate(d.Finalize1D(minima));
		}
	}

	public struct Voronoi2D<L, D, F> : INoise
		where L : struct, ILattice
		where D : struct, IVoronoiDistance
		where F : struct, IVoronoiFunction {
		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
			var l = default(L);
			var d = default(D);
			LatticeSpan4
				x = l.GetLatticeSpan4(positions.c0, frequency),
				z = l.GetLatticeSpan4(positions.c2, frequency);
			float4x2 minima = 2f;
			for (var u = -1; u <= 1; u++) {
				SmallXXHash4 hx = hash.Eat(l.ValidateSingleStep(x.p0 + u, frequency));
				float4 xoffset = u - x.g0;
				for (var v = -1; v <= 1; v++) {
					SmallXXHash4 h = hx.Eat(l.ValidateSingleStep(z.p0 + v, frequency));
					float4 zoffset = v - z.g0;
					minima = UpdateVoronoiMinima(minima, d.GetDistance(
						h.Floats01A + xoffset, h.Floats01B + zoffset
					));
					minima = UpdateVoronoiMinima(minima, d.GetDistance(
						h.Floats01C + xoffset, h.Floats01D + zoffset
					));
				}
			}
			// minima.c0 = min(minima.c0, 1f);
			// minima.c1 = min(minima.c1, 1f);
			return default(F).Evaluate(d.Finalize2D(minima));
		}
	}

	public struct Voronoi3D<L, D, F> : INoise
		where L : struct, ILattice
		where D : struct, IVoronoiDistance
		where F : struct, IVoronoiFunction {
		public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
			var l = default(L);
			var d = default(D);
			LatticeSpan4
				x = l.GetLatticeSpan4(positions.c0, frequency),
				y = l.GetLatticeSpan4(positions.c1, frequency),
				z = l.GetLatticeSpan4(positions.c2, frequency);
			float4x2 minima = 2f;
			for (var u = -1; u <= 1; u++) {
				SmallXXHash4 hx = hash.Eat(l.ValidateSingleStep(x.p0 + u, frequency));
				float4 xoffset = u - x.g0;
				for (var v = -1; v <= 1; v++) {
					SmallXXHash4 hy = hx.Eat(l.ValidateSingleStep(y.p0 + v, frequency));
					float4 yoffset = v - y.g0;
					for (var w = -1; w <= 1; w++) {
						SmallXXHash4 h = hy.Eat(l.ValidateSingleStep(z.p0 + w, frequency));
						float4 zoffset = w - z.g0;
						minima = UpdateVoronoiMinima(minima, d.GetDistance(
							h.GetBitsAsFloats01(5, 0) + xoffset,
							h.GetBitsAsFloats01(5, 5) + yoffset,
							h.GetBitsAsFloats01(5, 10) + zoffset
						));
						minima = UpdateVoronoiMinima(minima, d.GetDistance(
							h.GetBitsAsFloats01(5, 15) + xoffset,
							h.GetBitsAsFloats01(5, 20) + yoffset,
							h.GetBitsAsFloats01(5, 25) + zoffset
						));
					}
				}
			}
			return default(F).Evaluate(d.Finalize3D(minima));
		}
	}

	static float4x2 UpdateVoronoiMinima (float4x2 minima, float4 distances) {
		// return select(minima, distances, distances < minima);
		bool4 newMinimum = distances < minima.c0;
		minima.c1 = select(
			select(minima.c1, distances, distances < minima.c1),
			minima.c0,
			newMinimum
		);
		minima.c0 = select(minima.c0, distances, newMinimum);
		return minima;
	}
	// static float4 GetDistance(float4 x, float4 y) => sqrt(x * x + y * y);
	// static float4 GetDistance(float4 x, float4 y, float4 z) =>
	// 	sqrt(x * x + y * y + z * z);
}