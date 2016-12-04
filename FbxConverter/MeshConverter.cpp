#include <stdlib.h>
#include <assert.h>

#include "stdafx.h"
#include "MeshConverter.h"

using namespace NetGL::FbxConverter;
using namespace FbxConverterTypes;

namespace {
	FbxVector4 loadNormal(FbxMesh *const fbxmesh, int vertexIndex, int controlPointIndex) {
		FbxGeometryElementNormal* normal = fbxmesh->GetElementNormal(0);
		FbxGeometryElement::EMappingMode mappingMode = normal->GetMappingMode();
		int index = 0;

		if (mappingMode == FbxGeometryElement::eByControlPoint) {
			switch (normal->GetReferenceMode()) {
			case FbxGeometryElement::eDirect: {
				index = controlPointIndex;
				break;
			}

			case FbxGeometryElement::eIndexToDirect: {
				index = normal->GetIndexArray().GetAt(controlPointIndex);
				break;
			}

			default:
				break;
			}
		} else if (mappingMode == FbxGeometryElement::eByPolygonVertex) {
			index = vertexIndex;
		} else {
		}

		return normal->GetDirectArray().GetAt(index);
	}
	FbxVector4 loadTangent(FbxMesh *const fbxmesh, int vertexIndex, int controlPointIndex) {
		FbxGeometryElementTangent* tangent = fbxmesh->GetElementTangent(0);
		FbxGeometryElement::EMappingMode mappingMode = tangent->GetMappingMode();
		int index = 0;

		if (mappingMode == FbxGeometryElement::eByControlPoint) {
			switch (tangent->GetReferenceMode()) {
			case FbxGeometryElement::eDirect: {
				index = controlPointIndex;
				break;
			}

			case FbxGeometryElement::eIndexToDirect: {
				index = tangent->GetIndexArray().GetAt(controlPointIndex);
				break;
			}

			default:
				break;
			}
		} else if (mappingMode == FbxGeometryElement::eByPolygonVertex) {
			index = vertexIndex;
		} else {
		}

		return tangent->GetDirectArray().GetAt(index);
	}
	FbxVector2 loadUV(FbxMesh *const fbxmesh, int vertexIndex, int controlPointIndex) {
		FbxGeometryElementUV* uv = fbxmesh->GetElementUV(0);
		FbxGeometryElement::EMappingMode mappingMode = uv->GetMappingMode();
		int index = 0;

		if (mappingMode == FbxGeometryElement::eByControlPoint) {
			switch (uv->GetReferenceMode()) {
			case FbxGeometryElement::eDirect: {
				index = controlPointIndex;
				break;
			}

			case FbxGeometryElement::eIndexToDirect: {
				index = uv->GetIndexArray().GetAt(controlPointIndex);
				break;
			}

			default:
				break;
			}
		} else if (mappingMode == FbxGeometryElement::eByPolygonVertex) {
			index = vertexIndex;
		} else {
		}

		return uv->GetDirectArray().GetAt(index);
	}

	namespace {
		Vec3 toVec3(fbxsdk::FbxVector4 d) {
			Vec3 result((float)d[0], (float)d[1], (float)d[2]);
			return result;
		}
		Vec2 toVec2(fbxsdk::FbxVector2 d) {
			Vec2 result((float)d[0], (float)d[1]);
			return result;
		}
	}
}

Mesh^ MeshConverter::convert(FbxMesh *const fbxmesh) {
	auto mesh = gcnew Mesh();

	int polygonCount = fbxmesh->GetPolygonCount();
	int controlPointsCount = fbxmesh->GetControlPointsCount();

	for (int i = 0; i < polygonCount; i++) {
		if (fbxmesh->GetPolygonSize(i) != 3) {
			throw std::exception("Exception: Mesh is not triangulated. Can't proceed.");
		}
	}

	FbxGeometryElementNormal *const normalEl = fbxmesh->GetElementNormal(0);
	FbxGeometryElementTangent *const tangentEl = fbxmesh->GetElementTangent(0);
	FbxGeometryElementUV *const texEl = fbxmesh->GetElementUV(0);

	for (int polygon = 0; polygon < polygonCount; polygon++) {
		for (int v = 0; v < 3; v++) {
			int index = fbxmesh->GetPolygonVertex(polygon, v);
			if (index == -1) {
				throw std::exception("Exception: can't retrive vertex for polygon");
			}

			FbxVector4 fv = fbxmesh->GetControlPointAt(index);
			mesh->vertices->Add(toVec3(fv));

			//normal
			if (normalEl) {
				FbxVector4 fn = loadNormal(fbxmesh, polygon * 3 + v, index);
				mesh->normals->Add(toVec3(fn));
			}

			if (tangentEl) {
				FbxVector4 fn = loadTangent(fbxmesh, polygon * 3 + v, index);
				mesh->tangents->Add(toVec3(fn));
			}

			//mapping
			if (texEl) {
				FbxVector2 uv = loadUV(fbxmesh, polygon * 3 + v, index);
				mesh->uvs->Add(toVec2(uv));
			}
		}
	}

	return mesh;
}
