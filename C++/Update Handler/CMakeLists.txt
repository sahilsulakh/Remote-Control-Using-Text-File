cmake_minimum_required(VERSION 3.16)
project(AutoUpdater)

# Set C++ standard
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Add executable
add_executable(AutoUpdater
    main.cpp
    MainForm.cpp
    UpdateHandler.cpp
    Version.cpp
)

# Link libraries
target_link_libraries(AutoUpdater
    wininet
    comctl32
    shell32
)

# Set Windows subsystem
set_target_properties(AutoUpdater PROPERTIES
    WIN32_EXECUTABLE TRUE
)

# For Visual Studio, set startup project
if(MSVC)
    set_property(DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR} PROPERTY VS_STARTUP_PROJECT AutoUpdater)
endif()