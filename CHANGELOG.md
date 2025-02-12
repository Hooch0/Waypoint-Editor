# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

## [1.1.2] - 2025-02-12
### Fixed
- An issue where a scene is open but not currently saved on the Computer (can happen when a new scene is made) and attemping to make a asset file will cause an error.

### Changed
- Changed how disabling selection of scene objects when in waypoint edit mode is handled.

## [1.1.1] - 2023-12-24

### Fixed
- Issue with some events not being disposed when the scene changes. This fix adds a function that lets you manually un-register any references that the event may still have
    when its time to remove it.

## [1.1.0] - 2023-12-13

### Added

- WaypointSceneAsset is now used to contain all waypoint data moving forward.
- Editor based runtime generation.
- Auto-Generation options.

### Fixed

- Issues with toolbar selection.
- Issues with serializtion and keeping references when entering/leaving playmode.

### Changed

- Switched data storage from WaypointSceneController to WaypointSceneAsset.
- Waypoint editor now has a reference to a WaypointSceneAsset instead of WaypointSceneController.
- Backend storage is now based on arrays instead of dictionaries.
