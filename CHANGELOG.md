# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

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
