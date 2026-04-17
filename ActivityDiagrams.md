# Enemy Activity Diagrams
**2D Shooter — Assignment 1 Deliverable**
Author: Dillon Madson

---

## 1. Shooter Enemy

The Shooter Enemy keeps a preferred distance from the player and fires slow, accurate bullets (1 shot per 3 seconds).

```mermaid
flowchart TD
    Start([Start each frame]) --> CheckPlayer{Player exists?}
    CheckPlayer -- No --> EndA([End])
    CheckPlayer -- Yes --> CalcDist[Calculate distance to player]
    CalcDist --> RotatePlayer[Rotate to face player]
    RotatePlayer --> DistDecision{Distance vs preferredDistance?}

    DistDecision -- "Too close (< pref - tol)" --> BackAway[Move AWAY from player]
    DistDecision -- "Too far (> pref + tol)" --> CloseIn[Move TOWARD player]
    DistDecision -- "In sweet spot" --> StopMove[Stop moving]

    BackAway --> Attack
    CloseIn --> Attack
    StopMove --> Attack

    Attack[Attack: try to fire weapon] --> FireRateCheck{Fire rate cooldown ready?}
    FireRateCheck -- No --> EndA
    FireRateCheck -- Yes --> Spawn[Spawn bullet at firePoint]
    Spawn --> EndA
```

---

## 2. Exploding Enemy

The Exploding Enemy charges at the player and detonates when in close range, dealing heavy area damage.

```mermaid
flowchart TD
    Start([Start each frame]) --> CheckExplode{Is already exploding?}
    CheckExplode -- Yes --> EndB([End - coroutine running])
    CheckExplode -- No --> CheckPlayer{Player exists?}
    CheckPlayer -- No --> EndB
    CheckPlayer -- Yes --> CalcDist[Calculate distance to player]
    CalcDist --> RotatePlayer[Rotate to face player]
    RotatePlayer --> Charge[Move TOWARD player at full speed]
    Charge --> DistCheck{Distance <= explosionTriggerDistance?}

    DistCheck -- No --> EndB
    DistCheck -- Yes --> StartFuse[Start ExplodeRoutine coroutine]

    subgraph ExplodeRoutine [Coroutine: ExplodeRoutine]
        SetFlag[Set isExploding = true] --> StopAll[Stop moving]
        StopAll --> FlashLoop{Elapsed < fuseTime?}
        FlashLoop -- Yes --> ToggleColor[Flip color: red ⇄ white]
        ToggleColor --> Wait[Wait flashInterval seconds]
        Wait --> FlashLoop
        FlashLoop -- No --> Detonate[OverlapCircleAll within explosionRadius]
        Detonate --> Damage[Deal explosionDamage to every Character hit]
        Damage --> Destroy[Destroy self]
    end

    StartFuse --> SetFlag
    Destroy --> EndB
```

---

## 3. Machine Gun Enemy

The Machine Gun Enemy chases the player and sprays bullets at high rate with random spread (low accuracy).

```mermaid
flowchart TD
    Start([Start each frame]) --> CheckPlayer{Player exists?}
    CheckPlayer -- No --> EndC([End])
    CheckPlayer -- Yes --> CalcDist[Calculate distance to player]
    CalcDist --> RotatePlayer[Rotate to face player]
    RotatePlayer --> Chase[Move TOWARD player]
    Chase --> RangeCheck{Distance <= shootingRange?}

    RangeCheck -- No --> EndC
    RangeCheck -- Yes --> Attack[Attack: prepare to fire]

    Attack --> RandSpread[Pick random angle in spreadAngle range]
    RandSpread --> BuildRot[Build rotation = firePoint.rotation × spread]
    BuildRot --> FireRateCheck{Fire rate cooldown ready?}

    FireRateCheck -- No --> EndC
    FireRateCheck -- Yes --> Spawn[Spawn bullet with spread rotation]
    Spawn --> EndC
```

---

## Inheritance Hierarchy

All three enemy types inherit from a shared `Enemy` base class, which itself inherits from `Character`. This is the OOP pattern Assignment 1 demonstrates:

```mermaid
classDiagram
    Character <|-- Enemy
    Character <|-- Player
    Enemy <|-- ShooterEnemy
    Enemy <|-- ExplodingEnemy
    Enemy <|-- MachineGunEnemy

    class Character {
        +moveSpeed
        +healthModule
        +Move()
        +Rotate()
        +virtual Attack()
    }
    class Enemy {
        #playerTargetTransform
        #distanceToPlayer
        +virtual Update()
    }
    class ShooterEnemy {
        -preferredDistance
        -fireRate (slow)
        +override Attack()
    }
    class ExplodingEnemy {
        -explosionRadius
        -fuseTime
        +override Attack()
    }
    class MachineGunEnemy {
        -spreadAngle
        -fireRate (fast)
        +override Attack()
    }
```
