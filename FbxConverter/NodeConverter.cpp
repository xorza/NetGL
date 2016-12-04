#include <assert.h>
#include <stdint.h>
#include <memory>

#include "stdafx.h"
#include "NodeConverter.h"
#include "MeshConverter.h"

using namespace NetGL::FbxConverter;
using namespace FbxConverterTypes;

namespace {
	Vec3 toVec3(const fbxsdk::FbxDouble3 &d) {
		Vec3 result((float)d[0], (float)d[1], (float)d[2]);
		return result;
	}
}

NodeConverter::NodeConverter() {}

Node^ NodeConverter::convert(FbxNode *const fbxnode) {
	Node ^node = gcnew Node();

	node->name = StringUtil::toString(fbxnode->GetName());
	node->position = toVec3(fbxnode->LclTranslation.Get());
	node->rotation = toVec3(fbxnode->LclRotation.Get());
	node->scale = toVec3(fbxnode->LclScaling.Get());

	FbxMesh *const fbxmesh = fbxnode->GetMesh();
	if (fbxmesh != nullptr) {
		MeshConverter meshConverter{};
		node->mesh = meshConverter.convert(fbxmesh);
	}

	for (int i = 0; i < fbxnode->GetChildCount(false); i++) {
		FbxNode *const fbxchild = fbxnode->GetChild(i);
		node->nodes->Add(convert(fbxchild));
	}

	return node;
}
