#include "stdafx.h"

#include <memory>
#include <fbxsdk.h>
#include <assert.h>

#include "NodeConverter.h"

#include "SceneLoader.h"

using namespace NetGL::FbxConverter;
using namespace FbxConverterTypes;

Node^ SceneLoader::Load(System::String ^filename) {
	char *const fname = StringUtil::toCharPtr(filename);

	FbxManager *const manager(FbxManager::Create());
	assert(manager);
	FbxIOSettings *const ioSettings(FbxIOSettings::Create(manager, IOSROOT));
	assert(ioSettings);
	manager->SetIOSettings(ioSettings);
	FbxImporter *const importer(FbxImporter::Create(manager, ""));
	assert(importer);

	if (importer->Initialize(fname, -1, ioSettings) == false) {
		throw std::runtime_error("Invalid FBX file");
	}

	FbxScene *const fbxscene(FbxScene::Create(manager, "demo"));
	if (importer->Import(fbxscene) == false) {
		throw std::runtime_error("Failed to import FBX file");
	}
	FbxNode *const node = fbxscene->GetRootNode();

	assert(fbxscene);
	NodeConverter nodeConverter{};
	auto result = nodeConverter.convert(node);

	fbxscene->Destroy();
	importer->Destroy();
	ioSettings->Destroy();
	manager->Destroy();

	return result;
}