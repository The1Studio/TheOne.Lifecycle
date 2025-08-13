# TheOne.Lifecycle

Lifecycle Manager for Unity

## Installation

### Option 1: Unity Scoped Registry (Recommended)

Add the following scoped registry to your project's `Packages/manifest.json`:

```json
{
  "scopedRegistries": [
    {
      "name": "TheOne Studio",
      "url": "https://upm.the1studio.org/",
      "scopes": [
        "com.theone"
      ]
    }
  ],
  "dependencies": {
    "com.theone.lifecycle": "1.1.0"
  }
}
```

### Option 2: Git URL

Add to Unity Package Manager:
```
https://github.com/The1Studio/TheOne.Lifecycle.git
```

## Features

- Centralized lifecycle management for Unity applications
- Multi-phase loading system (Early, Main, Late)
- Update loop management (Update, LateUpdate, FixedUpdate)
- Application state listeners (Focus, Pause/Resume)
- Both synchronous and asynchronous loading support
- Integration with VContainer, Zenject, and custom DI frameworks
- Progress reporting for loading operations
- Automatic service discovery and registration

## Dependencies

- TheOne.Extensions

## Usage

### Basic Service Implementation

```csharp
using TheOne.Lifecycle;

// Simple loadable service
public class GameManager : ILoadable, IUpdatable
{
    public void Load()
    {
        Debug.Log("GameManager loaded");
        // Initialize game state
    }
    
    public void Update()
    {
        // Update game logic every frame
    }
}

// Async service with progress reporting
public class AssetLoader : IAsyncLoadable
{
    #if THEONE_UNITASK
    public async UniTask LoadAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default)
    {
        progress?.Report(0f);
        
        // Load critical assets
        await LoadCriticalAssets();
        progress?.Report(0.5f);
        
        // Load additional content
        await LoadAdditionalContent();
        progress?.Report(1f);
    }
    #else
    public IEnumerator LoadAsync(Action? callback = null, IProgress<float>? progress = null)
    {
        progress?.Report(0f);
        
        // Load critical assets
        yield return LoadCriticalAssets();
        progress?.Report(0.5f);
        
        // Load additional content
        yield return LoadAdditionalContent();
        progress?.Report(1f);
        
        callback?.Invoke();
    }
    #endif
}
```

### Service Registration & Lifecycle Usage

```csharp
// Manual initialization
var lifecycleManager = new MyLifecycleManager(
    earlyLoadableServices,      // Services that load first
    asyncEarlyLoadableServices,
    loadableServices,           // Main loading phase
    asyncLoadableServices,
    lateLoadableServices,       // Services that load last
    asyncLateLoadableServices,
    updatableServices,          // Services that need Update()
    lateUpdatableServices,      // Services that need LateUpdate()
    fixedUpdatableServices,     // Services that need FixedUpdate()
    focusGainListeners,         // Application focus listeners
    focusLostListeners,
    pauseListeners,             // Application pause listeners
    resumeListeners
);

// Start the lifecycle
#if THEONE_UNITASK
await lifecycleManager.LoadAsync(
    progress: new Progress<float>(p => Debug.Log($"Loading: {p:P}")),
    cancellationToken: cancellationToken
);
#else
yield return lifecycleManager.LoadAsync(
    callback: () => Debug.Log("Lifecycle loading completed"),
    progress: new Progress<float>(p => Debug.Log($"Loading: {p:P}"))
);
#endif
```

### Application State Listeners

```csharp
public class NetworkManager : IFocusLostListener, IFocusGainListener, IPauseListener, IResumeListener
{
    public void OnFocusLost()
    {
        // Pause network operations when app loses focus
        DisconnectFromServer();
    }
    
    public void OnFocusGain()
    {
        // Resume network operations when app gains focus
        ReconnectToServer();
    }
    
    public void OnPause()
    {
        // Save game state when app is paused
        SaveGameState();
    }
    
    public void OnResume()
    {
        // Restore game state when app resumes
        RestoreGameState();
    }
}
```

### Advanced Multi-Phase Loading

```csharp
public class ConfigService : IEarlyLoadable
{
    public void Load()
    {
        // Load configuration first - other services depend on this
        LoadConfiguration();
    }
}

public class DatabaseService : ILoadable
{
    public void Load()
    {
        // Load after configuration is ready
        InitializeDatabase();
    }
}

public class UIService : ILateLoadable
{
    public void Load()
    {
        // Load UI last when everything else is ready
        InitializeUI();
    }
}
```

## Architecture

### Folder Structure

```
TheOne.Lifecycle/
├── Scripts/
│   ├── ILifecycleManager.cs          # Main lifecycle manager interface
│   ├── LifecycleManager.cs           # Abstract base lifecycle manager
│   ├── DILifecycleManager.cs         # Generic DI integration
│   ├── VContainerLifecycleManager.cs # VContainer implementation
│   ├── ZenjectLifecycleManager.cs    # Zenject implementation
│   ├── ILoadable.cs                  # Loading interfaces
│   ├── IUpdatable.cs                 # Update loop interfaces
│   ├── IListener.cs                  # Application state listeners
│   └── DI/                           # DI framework extensions
│       ├── LifecycleManagerVContainer.cs
│       ├── LifecycleManagerZenject.cs
│       └── LifecycleManagerDI.cs
```

### Core Interfaces

#### Loading Interfaces

**Synchronous Loading:**
- `IEarlyLoadable.Load()` - Services that load first
- `ILoadable.Load()` - Main loading phase
- `ILateLoadable.Load()` - Services that load last

**Asynchronous Loading:**
```csharp
#if THEONE_UNITASK
// UniTask-based async interfaces
IAsyncEarlyLoadable.LoadAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default)
IAsyncLoadable.LoadAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default)  
IAsyncLateLoadable.LoadAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default)
#else
// Coroutine-based async interfaces
IAsyncEarlyLoadable.LoadAsync(Action? callback = null, IProgress<float>? progress = null)
IAsyncLoadable.LoadAsync(Action? callback = null, IProgress<float>? progress = null)
IAsyncLateLoadable.LoadAsync(Action? callback = null, IProgress<float>? progress = null)
#endif
```

#### Update Interfaces
- `IUpdatable` - Services that need Update() calls
- `ILateUpdatable` - Services that need LateUpdate() calls
- `IFixedUpdatable` - Services that need FixedUpdate() calls

#### Application State Listeners
- `IFocusGainListener` - Notified when application gains focus
- `IFocusLostListener` - Notified when application loses focus
- `IPauseListener` - Notified when application is paused
- `IResumeListener` - Notified when application resumes

### Core Classes

#### `ILifecycleManager`
Main interface for lifecycle operations:

```csharp
#if THEONE_UNITASK
UniTask LoadAsync(IProgress<float>? progress = null, CancellationToken cancellationToken = default);
#else
IEnumerator LoadAsync(Action? callback = null, IProgress<float>? progress = null);
#endif
```

- `LoadAsync()` - Initiates the complete loading sequence
- Supports both UniTask and Coroutine implementations  
- Built-in progress reporting and cancellation support
- Returns UniTask when THEONE_UNITASK is defined, otherwise returns IEnumerator

#### `LifecycleManager`
Abstract base implementation that:
- Manages service collections for each lifecycle phase
- Orchestrates multi-phase loading sequence
- Creates and manages Unity event listeners
- Provides automatic cleanup via IDisposable
- Handles both sync and async operations seamlessly

#### DI Framework Implementations
- `VContainerLifecycleManager` - VContainer integration
- `ZenjectLifecycleManager` - Zenject integration
- `DILifecycleManager` - Generic DI container support

### Loading Sequence

The lifecycle manager executes services in this order:

1. **Early Loading Phase**
   - `IEarlyLoadable.Load()` - Synchronous early services
   - `IAsyncEarlyLoadable.LoadAsync()` - Async early services (parallel)

2. **Main Loading Phase**
   - `ILoadable.Load()` - Synchronous main services
   - `IAsyncLoadable.LoadAsync()` - Async main services (parallel)

3. **Late Loading Phase**
   - `ILateLoadable.Load()` - Synchronous late services
   - `IAsyncLateLoadable.LoadAsync()` - Async late services (parallel)

4. **Event Listener Setup**
   - Creates persistent GameObject for Unity events
   - Registers all update and application state listeners

### Design Patterns

- **Inversion of Control**: Services registered externally, managed centrally
- **Observer Pattern**: Application state change notifications
- **Template Method**: Abstract loading sequence with concrete implementations
- **Dependency Injection**: Framework-agnostic service resolution
- **Multi-Phase Initialization**: Controlled dependency loading order

### Code Style & Conventions

- **Namespace**: All code under `TheOne.Lifecycle` namespace
- **Null Safety**: Uses `#nullable enable` directive
- **Interfaces**: Prefixed with `I` (e.g., `ILifecycleManager`)
- **Async Support**: Conditional compilation for UniTask vs Coroutines
- **Service Naming**: Clear, purpose-driven interface names
- **Framework Integration**: Conditional compilation for different DI frameworks

### Integration with DI Frameworks

#### VContainer
```csharp
using TheOne.Lifecycle.DI;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // Register your services
        builder.RegisterEntryPoint<GameManager>();
        builder.Register<AssetLoader>(Lifetime.Singleton).AsImplementedInterfaces();
        
        // Register lifecycle manager
        builder.RegisterLifecycleManager();
    }
}
```

#### Zenject
```csharp
using TheOne.Lifecycle.DI;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Register your services
        Container.BindInterfacesTo<GameManager>().AsSingle();
        Container.BindInterfacesTo<AssetLoader>().AsSingle();
        
        // Register lifecycle manager
        Container.BindLifecycleManager();
    }
}
```

#### Custom DI
```csharp
using TheOne.Lifecycle.DI;

// Register services with your DI container
container.Register<IGameManager, GameManager>();
container.Register<IAssetLoader, AssetLoader>();

// Register lifecycle manager
container.RegisterLifecycleManager();
```

## Performance Considerations

- Services are collected once during construction to avoid runtime allocations
- Update loops use direct event subscription for minimal overhead
- Async operations run in parallel within each phase for faster loading
- EventListener GameObject persists with DontDestroyOnLoad for efficiency
- Proper cleanup prevents memory leaks when lifecycle manager is disposed

## Best Practices

1. **Loading Order**: Use Early/Main/Late phases to manage service dependencies
2. **Async Operations**: Prefer async interfaces for I/O bound operations
3. **Progress Reporting**: Always report progress for long-running async operations
4. **State Management**: Use application listeners to handle focus/pause appropriately
5. **Service Granularity**: Keep services focused on single responsibilities
6. **Error Handling**: Implement proper error handling in async load methods
7. **Cancellation**: Support cancellation tokens in async operations
8. **Testing**: Mock ILifecycleManager for unit tests
9. **Cleanup**: Always dispose lifecycle manager when application shuts down