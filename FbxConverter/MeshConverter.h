#pragma once

#include <fbxsdk.h>

namespace NetGL {
	namespace FbxConverter {
		class MeshConverter {
		public:
			MeshConverter() {}

			FbxConverterTypes::Mesh^ convert(FbxMesh *const fbxmesh);
		};
	}
}