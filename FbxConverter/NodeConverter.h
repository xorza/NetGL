#pragma once

#include <fbxsdk.h>


namespace NetGL {
	namespace FbxConverter {
		class NodeConverter {
		public:
			NodeConverter();
			FbxConverterTypes::Node^ convert(FbxNode *const fbxnode);
		};
	}
}