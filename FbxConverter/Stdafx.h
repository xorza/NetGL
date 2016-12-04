// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once

class StringUtil {
public:
	static char* toCharPtr(System::String^ str) {
		return reinterpret_cast<char*>(System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(str).ToPointer());
	}

	static System::String^ toString(const char *const ptr) {
		System::String^ clistr = gcnew System::String(ptr);
		return clistr;
	}

private:
	StringUtil() = delete;
};