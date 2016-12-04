#pragma once

#include <string>


namespace NetGL {
	namespace FbxConverter {
		public ref class SceneLoader
			:public FbxConverterTypes::ISceneLoader {
		public:
			SceneLoader() {};

			virtual FbxConverterTypes::Node^ Load(System::String ^filename);
		};
	}
}
