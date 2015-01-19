cmake_minimum_required(VERSION 2.8.4)
project(modernc__)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")

set(SOURCE_FILES c++/main.cpp)
add_executable(modernc__ ${SOURCE_FILES})