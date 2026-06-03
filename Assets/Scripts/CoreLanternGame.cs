using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class CoreLanternGame : MonoBehaviour
{
    const float ArenaRadius = 12.5f;
    const int MaxWave = 10;
    const int MaxModuleLevel = 3;
    // ── Performance caps: hard limits to prevent runaway accumulation ──
    // When a cap is reached, the OLDEST entry is destroyed to make room for the new one.
    // This keeps frame rate stable in heavy combat and prevents Editor freezes.
    const int MaxBullets = 180;
    const int MaxEnemies = 80;
    const int MaxPickups = 60;
    const int MaxSpriteGhosts = 40;
    const int MaxFloatingTexts = 36;
    const int BaseRerollCost = 30;
    // ── Embercore Studio リリース惁E�� ──
    const string GameVersion = "0.9.0 (EA)";       // 表示用バージョン (Steam EA 開始時に 1.0.0 へ)
    const string StudioName = "Embercore Studio";  // Shared studio credit label.
    const string AutoStartRunKey = "CoreLantern_AutoStartRun";
    const string LastRunPartnerStyleKey = "CoreLantern_LastRunPartnerStyle";   // 同設定再戦用
    const string QuickRetryKey = "CoreLantern_QuickRetry";                      // Skip partner select on quick retry.
    const string OptionPrefPrefix = "CoreLantern_Option_";
    // Keep large UI surfaces procedural until proper high-res/9-slice assets exist.
    // The generated bitmap frames were noisy and looked cheap when stretched.
    // Generated image rollout flags.
    static readonly bool UseGeneratedBackgrounds = true;
    static readonly bool UseGeneratedOverlayImages = false;
    static readonly bool UseGeneratedHudBars = false;
    static readonly bool UseHud9SliceCandidates = true;     // Codex takeover: test quiet HUD9 art on wave progress only.
    static readonly bool UseWaveProgressHudV2 = true;      // Isolated v2 pilot: wave bar only.
    static readonly bool UseHudBarV2 = true;               // Isolated v2 pilot: HP/EXP bars only.
    static readonly bool UseGeneratedHudPanels = false;     // Stage 3: HUD panel frames.
    static readonly bool UseGeneratedCutscenes = true;      // Stage 4: evolution/cross-evolution backgrounds.
    static readonly bool UseGeneratedCards = false;         // Stage 5: カードフレーム
    static readonly bool UseButtonV2 = true;               // Stage 6: ボタン背景 v2 素材
    static readonly bool UseTitleUiV2 = true;              // Stage 7: title screen decorative v2 assets.
    static readonly bool UseResultUiV2 = true;             // Stage 8: result screen exact-size v2 assets.
    static readonly bool UseCardPartsV2 = true;            // Stage 9: exact-size upgrade card parts.

    readonly List<Enemy> enemies = new();
    readonly List<Bullet> bullets = new();
    readonly List<Pickup> pickups = new();
    readonly List<Spark> sparks = new();
    readonly List<BinaryDigit> binaryDigits = new();
    readonly List<SpriteGhost> spriteGhosts = new();
    readonly List<FloatingText> floatingTexts = new();
    readonly List<AmbientNeon> ambientNeons = new();
    readonly List<SpriteRenderer> dropShadowRenderers = new();
    readonly List<SpriteRenderer> enhancedVisualRenderers = new();
    readonly List<Upgrade> upgradePool = new();
    readonly List<Upgrade> fusionUpgradePool = new();
    readonly Dictionary<string, int> moduleLevels = new();
    readonly Dictionary<string, Sprite> skinSpriteCache = new();

    Transform player;
    Transform lantern;
    Sprite circleSprite;
    Sprite squareSprite;
    Sprite diamondSprite;
    Sprite playerSprite;
    Sprite lanternSprite;
    Sprite runnerSprite;
    Sprite bruteSprite;
    Sprite shooterSprite;
    Sprite bossSprite;
    Sprite dasherSprite;
    Sprite bomberSprite;
    Sprite phantomSprite;
    Sprite lavaCrawlerSprite;
    // Stage-specific enemy sprites; fall back to existing enemy sprites when missing.
    Sprite magmaTitanSprite;
    Sprite corruptionDroneSprite;
    Sprite frostKnightSprite;
    Sprite voltDasherSprite;
    // Boss-specific sprites for mid boss / final boss.
    Sprite pulswyrmSprite;
    Sprite nullwyrmSprite;
    Sprite binaryZeroSprite;
    Sprite binaryOneSprite;
    Sprite novaLinkSprite;
    Sprite bulwarkLinkSprite;
    Sprite siphonLinkSprite;
    Sprite phaseLinkSprite;
    Sprite[] playerFormSprites;
    Sprite floorTileASprite;
    Sprite floorTileBSprite;
    Sprite floorTileCSprite;
    Sprite floorDarkBaseSprite;
    Sprite floorGridOverlaySprite;
    Sprite floorCoreMarkSprite;
    Sprite floorHazardLavaSprite;
    Sprite stage2LavaPoolSprite;            // Codex 2026-05-22 batch2: dedicated lava pool marker.
    Sprite stage2HeatVentSprite;            // Codex 2026-05-22 batch2: dedicated heat vent marker.
    Sprite stage3CorruptionSprite;
    Sprite stage3RelayDeviceSprite;
    Sprite stageThumbArenaSprite;
    Sprite stageThumbLavaSprite;
    Sprite stageThumbBrokenCoreSprite;
    Sprite stageThumbFrostSprite;
    Sprite stageThumbStormSprite;
    Sprite stage4FrostCrystalSprite;
    Sprite stage4FrostPatchSprite;
    Sprite stage5LightningMarkerSprite;
    Sprite stage5LightningStrikeSprite;
    Sprite titleCoreEggSprite;
    Sprite resultCoreEggSprite;
    Sprite iconCoreEggSprite;
    Sprite pickupData64Sprite;
    Sprite corePlatformSprite;
    Sprite coreRingOuterSprite;
    Sprite coreRingInnerSprite;
    Sprite coreDamageCrack1Sprite;
    Sprite coreDamageCrack2Sprite;
    Sprite coreDamageCrack3Sprite;
    Sprite propNeonPylonSprite;
    Sprite propDataTerminalSprite;
    Sprite propCrateSprite;
    Sprite spawnRingNormalSprite;
    Sprite spawnRingBossSprite;
    Sprite[] floorCrackSprites;
    Sprite[] floorCableSprites;
    Sprite[] boundaryStoneSprites;
    Sprite[] serverDebrisSprites;
    Sprite cardCyanSprite;
    Sprite cardGoldSprite;
    Sprite cardGreenSprite;
    Sprite cardMagentaSprite;
    Sprite cardRedSprite;
    Sprite cardHeaderBasicV2Sprite;
    Sprite cardHeaderRareV2Sprite;
    Sprite cardHeaderEpicV2Sprite;
    Sprite cardTitlePlateV2Sprite;
    Sprite cardLevelPlateV2Sprite;
    Sprite cardRarityPlateBasicV2Sprite;
    Sprite cardRarityPlateRareV2Sprite;
    Sprite cardRarityPlateEpicV2Sprite;
    Sprite cardBottomRailCyanV2Sprite;
    Sprite cardBottomRailGoldV2Sprite;
    Sprite cardSpecialCornerEpicV2Sprite;
    Sprite cardSpecialCornerCrossV2Sprite;
    Sprite bulletSpeedSprite;
    Sprite bulletPowerSprite;
    Sprite bulletGuardSprite;
    Sprite bulletFusionSprite;
    Sprite dataChipSprite;
    Sprite evolutionRingSprite;
    Sprite panelFrameCyanSprite;
    Sprite panelFrameGoldSprite;
    // Button V2 sprites
    Sprite btnPrimaryStartSprite;
    Sprite btnRunStartSprite;
    Sprite btnRunBackSprite;
    Sprite btnRerollSprite;
    Sprite btnSkipRewardSprite;
    Sprite btnPauseResumeSprite;
    Sprite btnWide320x42Sprite;
    Sprite btnResultRetrySprite;
    Sprite btnResultMenuSprite;
    Sprite btnMenuSecondarySprite;
    Sprite btnCloseSmallSprite;
    Sprite btnCloseWideSprite;
    Sprite btnDangerChipSprite;
    Sprite btnRunStageChipSprite;
    Sprite btnStepperSprite;
    // Title screen game logo (compact preferred, full fallback, text fallback)
    Sprite gameLogoSprite;
    Sprite gameLogoTitleCompactSprite;
    // Title UI V2 sprites
    Sprite titleCoreEmblemSprite;
    Sprite titleSubtitlePlateSprite;
    Sprite titleStatsRibbonSprite;
    Sprite titleLogoUnderlineSprite;
    Sprite titleNavRailSprite;
    Sprite titleCornerAccentSprite;
    // Result V2 sprites
    Sprite resultDeckFrameSprite;
    Sprite resultPortraitFrameSprite;
    Sprite resultStatTileSprite;
    Sprite resultBadgeRouteSprite;
    Sprite resultBadgeFusionSprite;
    Sprite resultMvpRowSprite;
    Sprite resultSummaryPlateSprite;
    Sprite resultFooterGuideSprite;
    Sprite resultRankMedalSSprite;
    Sprite resultRankMedalASprite;
    Sprite resultRankMedalBSprite;
    Sprite resultRankMedalCSprite;
    Sprite resultRankMedalDSprite;
    Sprite hudPanelWaveSprite;
    Sprite hudPanelHpSprite;
    Sprite hudPanelLevelSprite;
    Sprite hudPanelChipMiniSprite;
    Sprite hudPanelStatusSprite;
    Sprite hudPanelLoadoutSprite;
    Sprite hudPanelLinksSprite;
    Sprite hudBarBackSprite;
    Sprite hudBarPlayerHpFillSprite;
    Sprite hudBarCoreHpFillSprite;
    Sprite hudBarHpLagSprite;
    Sprite hudBarExpFillSprite;
    Sprite hudBarBackV2Sprite;
    Sprite hudBarPlayerHpFillV2Sprite;
    Sprite hudBarCoreHpFillV2Sprite;
    Sprite hudBarHpLagV2Sprite;
    Sprite hudBarExpFillV2Sprite;
    Sprite hud9BarBackSprite;
    Sprite hud9BarPlayerHpFillSprite;
    Sprite hud9BarCoreHpFillSprite;
    Sprite hud9BarExpFillSprite;
    Sprite hud9PanelWaveSprite;
    Sprite hud9PanelHpSprite;
    Sprite hud9PanelLevelSprite;
    Sprite hud9PanelChipMiniSprite;
    Sprite hudWaveProgressFrameSprite;
    Sprite hudWaveProgressNormalSprite;
    Sprite hudWaveProgressBossSprite;
    Sprite hudWaveProgressFrameV2Sprite;
    Sprite hudWaveProgressNormalV2Sprite;
    Sprite hudWaveProgressBossV2Sprite;
    Sprite hud9WaveProgressFrameSprite;
    Sprite hud9WaveProgressNormalSprite;
    Sprite hud9WaveProgressBossSprite;
    Sprite hudBossBarFrameSprite;
    Sprite hudBossBarFillSprite;
    Sprite hudRelicSlotFrameSprite;
    Sprite hudIconPlayerHpSprite;
    Sprite hudIconCoreHpSprite;
    Sprite hudIconExpSprite;
    Sprite hudIconDataChipSprite;
    Sprite hudBossWarningBannerSprite;
    Sprite hudDangerPlayerSprite;
    Sprite hudDangerCoreSprite;
    Sprite hudChoiceDimSprite;
    Sprite mainMenuBackgroundSprite;
    Sprite resultClearBackgroundSprite;
    Sprite resultGameOverBackgroundSprite;
    Sprite bossPulswyrmBackgroundSprite;
    Sprite bossNullwyrmBackgroundSprite;
    Sprite cutinEvolveSpeedSprite;
    Sprite cutinEvolvePowerSprite;
    Sprite cutinEvolveGuardSprite;
    Sprite cutinFusionSprite;
    Sprite relicMagnetSprite;
    Sprite relicDeathlessSprite;
    Sprite relicCorePulseSprite;
    Sprite relicExplosionSprite;
    Sprite relicStormSprite;
    Sprite relicMirrorSprite;
    Sprite relicTitanSprite;
    Sprite relicOverdriveSprite;
    Sprite relicDataSurgeSprite;
    Sprite relicApexSprite;
    readonly Dictionary<string, Sprite> moduleIconSprites = new();
    SpriteRenderer playerRenderer;
    SpriteRenderer playerGlowRenderer;
    SpriteRenderer lanternGlowRenderer;
    SpriteRenderer coreDamageCrack1Renderer;
    SpriteRenderer coreDamageCrack2Renderer;
    SpriteRenderer coreDamageCrack3Renderer;
    Transform playerVisualBody;
    Vector3 lastPlayerVisualPosition;
    Vector2 playerVisualVelocity;
    float playerMotionPhase;
    float playerAfterimageTimer;
    float playerFootstepTimer;
    float playerAttackPulse;
    Transform playerWorldHpRoot;
    Transform playerWorldHpFill;
    Transform eggWorldHpRoot;
    Transform eggWorldHpFill;
    SpriteRenderer playerWorldHpFillRenderer;
    SpriteRenderer eggWorldHpFillRenderer;
    Transform fusionAura;
    Transform shieldAura;
    Transform playerStageHalo;
    Transform playerLeftAccent;
    Transform playerRightAccent;
    Transform playerCrestAccent;
    readonly Transform[] linkCompanions = new Transform[4];
    readonly Transform[] linkCompanionGlows = new Transform[4];
    readonly SpriteRenderer[] linkCompanionRenderers = new SpriteRenderer[4];
    readonly SpriteRenderer[] linkCompanionGlowRenderers = new SpriteRenderer[4];
    SpriteRenderer fusionAuraRenderer;
    SpriteRenderer shieldAuraRenderer;
    SpriteRenderer playerStageHaloRenderer;
    SpriteRenderer playerLeftAccentRenderer;
    SpriteRenderer playerRightAccentRenderer;
    SpriteRenderer playerCrestAccentRenderer;
    Camera mainCamera;

    Canvas canvas;
    Text hudText;
    Text nameText;
    Text waveText;
    Text controlText;
    Text resourceText;
    Text chipHudText;
    Text levelText;
    Text expValueText;
    Text statsText;
    Text loadoutText;
    Text statusText;
    Text relicDisplayText;
    Text eventLogText;
    Text centerText;
    Text panelTitleText;
    Text panelSubtitleText;
    Text synergyText;
    Text playerHpValueText;
    Text eggHpValueText;
    Text waveProgressText;
    Text fusionProgressText;
    Text evolutionCutsceneTitleText;
    Text evolutionCutsceneNameText;
    Text evolutionCutsceneDescText;
    Text menuBestText;
    Text menuMissionText;
    Text missionRecordText;
    Text codexText;
    Text codexRecordText;
    ScrollRect codexScrollRect;
    RectTransform codexScrollContentRect;
    Text treeSelectedPartnerText;
    readonly List<GameObject> codexCards = new();
    readonly List<Image> codexCardIcons = new();
    readonly List<Image> codexCardSilhouettes = new();
    readonly List<Text> codexCardTitles = new();
    readonly List<Text> codexCardStatusTexts = new();
    readonly List<Text> codexCardDescTexts = new();
    readonly List<string> codexCardKinds = new();
    readonly List<string> codexCardKeys = new();
    readonly List<string> codexCardDescriptions = new();
    readonly List<Color> codexCardAccents = new();
    readonly List<GameObject> missionCards = new();
    readonly List<Text> missionCardTitles = new();
    readonly List<Text> missionCardStatusTexts = new();
    readonly List<Text> missionCardDescTexts = new();
    readonly List<Text> missionCardRewardTexts = new();
    readonly List<string> missionCardIds = new();
    Text resultTitleText;
    Text resultBodyText;
    Image resultPortraitImage;
    Image resultBackgroundImage;
    Image resultPortraitGlow;
    Text resultRouteBadgeText;
    Text resultFusionBadgeText;
    Text resultRunSummaryText;
    Image resultRouteBadgeImage;
    Image resultFusionBadgeImage;
    readonly Text[] resultStatLabels = new Text[6];
    readonly Text[] resultStatValues = new Text[6];
    readonly GameObject[] resultStatRoots = new GameObject[6];
    Image resultRankMedalImage;
    readonly Text[] resultMvpTitleTexts = new Text[3];
    readonly Text[] resultMvpDetailTexts = new Text[3];
    readonly Image[] resultMvpIcons = new Image[3];
    readonly GameObject[] resultMvpCardRoots = new GameObject[3];
    GameObject resultRouteBadgePanel;
    GameObject resultFusionBadgePanel;
    Image playerHpFill;
    Image eggHpFill;
    Image playerHpLagFill;
    Image eggHpLagFill;
    Image dataFill;
    Image waveProgressFill;
    Image fusionProgressFill;
    Image evolutionCutsceneBack;
    Image evolutionCutsceneBeam;
    Image evolutionCutscenePortrait;
    Image evolutionCutsceneRing;
    Image dangerImage;
    readonly List<Image> evolutionDots = new();
    readonly List<Image> linkSlotImages = new();
    readonly List<Image> linkSlotIconImages = new();
    readonly List<Text> linkSlotTexts = new();
    readonly List<Image> relicHudSlotFrames = new();
    readonly List<Image> relicHudSlotIcons = new();
    readonly List<Image> treeRoutePreviewIcons = new();
    readonly List<Image> treeCrossPreviewIcons = new();
    readonly List<Image> treePartnerCardImages = new();
    readonly List<Outline> treePartnerCardOutlines = new();
    readonly List<Text> treePartnerBadgeTexts = new();
    readonly List<OptionButton> optionButtons = new();
    readonly List<string> eventLog = new();
    GameObject waveProgressRoot;
    GameObject hpPanelRoot;     // For user-adjustable HP bar scale
    GameObject levelPanelRoot;  // For user-adjustable EXP bar scale
    GameObject wavePanelRoot;   // Wave + Partner Name panel (top-left HUD)
    // Bar back-rects for direct width scaling (preferred over localScale)
    RectTransform playerHpBarBackRect;
    RectTransform eggHpBarBackRect;
    RectTransform expBarBackRect;
    RectTransform waveProgressBackRect;
    Vector2 playerHpBarBaseSize;
    Vector2 eggHpBarBaseSize;
    Vector2 expBarBaseSize;
    Vector2 waveProgressBaseSize;
    GameObject statsPanelRoot;
    GameObject eventLogPanelRoot;
    GameObject dataPanelRoot;
    GameObject chipHudRoot;
    GameObject loadoutPanelRoot;
    GameObject statusPanelRoot;
    GameObject mainMenuPanel;
    GameObject codexPanel;
    GameObject missionBoardPanel;
    GameObject evolutionTreePanel;
    GameObject resultPanel;
    GameObject pausePanel;
    GameObject optionsPanel;
    GameObject choiceFocusOverlay;
    GameObject evolutionCutscenePanel;
    GameObject upgradePanel;
    GameObject moduleStatusSidePanel;  // Side panel shown next to upgrade panel: current modules + link evo conditions
    Text moduleStatusText;             // Fallback "(none)" text when no modules picked
    Text linkEvolutionConditionText;   // Doubles as tooltip area on module hover
    Text[] moduleSlotTexts;            // 12 hoverable module slots
    Image[] moduleSlotBgs;             // Slot backgrounds (raycast targets for hover)
    readonly List<string> currentModuleSlotTitles = new();
    const int ModuleSlotCount = 12;
    bool moduleSlotHovering;           // True while a slot is being hovered
    Button rerollButton;
    Button skipRewardButton;
    Button startMenuButton;
    Button menuOptionsButton;
    Button menuCodexButton;
    Button menuMissionButton;
    Button menuTreeButton;
    Button codexCloseButton;
    Button missionCloseButton;
    Button treeCloseButton;
    Button resultRetryButton;
    Button resultMenuButton;
    Button partnerBackButton;
    Button resumeButton;
    Button restartButton;
    Button pauseOptionsButton;
    GameObject partnerPanel;
    RectTransform partnerScrollContentRect;
    ScrollRect partnerScrollRect;
    GameObject bossBarRoot;
    Image bossBarFill;
    Image flashImage;
    readonly List<Button> upgradeButtons = new();
    readonly List<Button> partnerButtons = new();
    readonly List<string> runUnlockedMissions = new();
    Font uiFont;
    AudioSource sfxSource;
    AudioSource bgmSource;
    readonly Dictionary<string, AudioClip> audioClips = new();
    readonly Dictionary<string, float> sfxLastPlayedAt = new();
    float lastBossHitFxAt;
    float lastBossHitFreezeAt;

    System.Random rng;
    Vector2 aimDirection = Vector2.up;
    Vector2 mouseMoveTarget;
    bool hasMouseMoveTarget;
    float shootTimer;
    float waveTimer;
    float elapsedTime;
    float messageTimer;
    float flashTimer;
    float flashDuration;
    float shakeTimer;
    float shakeDuration;
    float shakePower;
    float evolutionCutsceneTimer;
    // 進化カチE��インの長ぁE1.45 →2.10s に延長 (大きな瞬間として印象付けめE
    float evolutionCutsceneDuration = 2.10f;
    // ── Boss cutscene state ──
    GameObject bossCutscenePanel;
    Image bossCutsceneBackgroundImage;
    Image bossCutsceneVignette;
    Image bossCutsceneSlashTop;
    Image bossCutsceneSlashBottom;
    Image bossCutscenePortrait;
    Text bossCutsceneWarning;
    Text bossCutsceneNameText;
    Text bossCutsceneSubText;
    float bossCutsceneTimer;
    float bossCutsceneDuration = 1.8f;
    bool bossCutsceneIsVictory;
    bool bossCutscenePendingSpawn;
    bool bossCutscenePendingMid;
    bool bossCutsceneVictoryMidBoss;
    float playerHitPulse;
    float eggHitPulse;
    float playerHpVisual = 1f;
    float eggHpVisual = 1f;
    int wave = 1;
    int enemiesToSpawn;
    int enemiesTotalThisWave;
    int enemiesKilledThisWave;
    // ── プロ改喁E 連続撃破ストリーク (同ジャンル定番の達�E感ルーチE ──
    // 0.8秒以内の連続キルを記録する。
    int killStreak;
    float lastKillTime;
    int runMaxKillStreak;     // ラン中の最高ストリーク (リザルト表示用)
    const float KillStreakWindow = 0.85f;
    // ── プロ改喁E 死因記録 (同ジャンル「何で死んだか�EからなぁE��対筁E ──
    string lastDamageSource = "";        // 直前�Eダメージ溁E(吁EDamagePlayer 呼び出し�Eで設宁E
    string runDeathCause = "";           // 実際に致死した時�E死因 (リザルトに表示)
    // ── プロ改喁E Banish/Lock シスチE�� (Brotato 流�Eカード除夁E+ ロック) ──
    HashSet<string> runBannedTitles = new HashSet<string>();
    bool[] lockedSlots = new bool[3];
    List<Upgrade> currentUpgradeChoices = new List<Upgrade>();   // 現在表示中のカーチE(リロール時に lock 維持E
    float spawnTimer;
    bool titleScreen = true;
    bool choosingUpgrade;
    bool paused;
    bool gameOver;
    bool victory;
    bool runtimeReloading;
    bool showAdvancedHud;
    bool showEventLogPanel;
    bool showControlHelp;
    bool enhancedVisuals = true;
    bool screenShakeEnabled = true;
    bool screenFlashEnabled = true;
    bool hitFreezeEnabled = true;

    // ── Sustained Laser (Pulse Hydra) ───────────────────────────
    // Always-on beam that locks onto nearest enemy and ticks damage every LaserTickInterval sec.
    bool laserActive;
    float laserMoveBonus = 1f;             // コアハ�Eモニクス: レーザー中の移動倍率 (1.0 = なぁE
    bool eggResonanceActive;               // エッグレゾナンス: コアHP満タン晁Edmg ボ�Eナス
    // ── Solar Anchor (コア共鳴垁E  Eシンプル化版 ──
    // ① Core Aura: コア周りに常時ゴールドダメージリング (4 DPS / 半征E4m)
    // ② Core Bond: コア半征E3m 以冁E��ら�E身の攻撃+30%
    // 旧 Resonance Wave は常時オーラに統合。
    bool solarAnchorActive;
    const float CoreAuraRadius = 4.0f;
    const float CoreAuraDps = 4.0f;
    const float CoreBondNearRadius = 3.0f;      // こ�E距離以冁E��攻撃�Eーナス
    const float CoreBondNearBonus = 1.30f;      // 近接時�E攻撃倍率 (+30%)
    float coreAuraDpsBonus;                     // コアシンク強匁E(加箁E
    float coreAuraRadiusBonus;                  // コアシンク強匁E(加箁E
    bool coreBondNearActive;
    Transform coreAuraVisual;
    float coreAuraTickAcc;
    Transform laserBeamTransform;
    SpriteRenderer laserBeamRenderer;
    Color laserBeamColor = new Color(1f, 0.45f, 0.85f);   // default pink/magenta
    float laserMaxLength = 8f;
    float laserDamageMultiplier = 5f;                      // bulletDamage ×this ×tickInterval per tick
    float laserTickTimer;
    const float LaserTickInterval = 0.15f;

    // ── Funnel bits (Halo Caster) ─────────────────────────────
    // Orbital drones that auto-attack nearest enemy independently.
    // Also feature: every N sec, one bit detaches and charges into an enemy for big damage.
    enum FunnelBitState { Orbit, ChargeOut, ChargeReturn }
    const int FunnelMaxBits = 6;
    int funnelBitCount;                            // 0 = disabled (default)
    float funnelOrbitRadius = 1.45f;       // モジュール変更可: センチネルピ�EチE��で拡張
    float funnelBitFireRate = 0.50f;                // sec between shots per bit
    readonly Transform[] funnelBitTransforms = new Transform[FunnelMaxBits];
    readonly float[] funnelBitTimers = new float[FunnelMaxBits];
    readonly FunnelBitState[] funnelBitStates = new FunnelBitState[FunnelMaxBits];
    readonly Enemy[] funnelBitTargets = new Enemy[FunnelMaxBits];
    float funnelBitAngleBase;
    // Detach attack
    float funnelDetachTimer = 3f;                                 // initial delay (was 4f)
    float funnelDetachInterval = 4.5f;                            // sec between detach attacks (module-modifiable)
    const float FunnelDetachSpeed = 20f;                          // m/s when charging
    const float FunnelDetachReturnSpeed = 14f;                    // m/s when returning to orbit
    const float FunnelDetachHitRadius = 0.6f;                     // contact distance for impact
    float funnelDetachDamageMultiplier = 4.5f;                    // ×normal bit bullet dmg (module-modifiable)
    // ArenaRadius = 12.5, enemies spawn at ~12.9 from origin.
    const float FunnelDetachMaxRange = 16f;
    // Laser line-damage chained multiplier (module-modifiable)
    float laserChainDamageMul = 0.60f;
    // Lifesteal heal amount per damaging hit (module-modifiable)
    float lifestealAmount = 0.06f;
    // Lifesteal heal-on-kill bonus (module-modifiable, only triggers if bulletLifesteal ON)
    float lifestealKillBonus = 0f;

    // Camera zoom multiplier (1.0 = default, lower = zoom out, higher = zoom in)
    float cameraZoom = 1.0f;
    const float CameraBaseSize = 6.8f;
    // Hit-freeze timer  Ewhen > 0, Time.timeScale is forced to 0 for impact (unscaled time still ticks)
    float hitFreezeTimer;
    bool bgmEnabled = true;
    bool sfxEnabled = true;
    // ダメージ数孁E(敵/プレイヤー両方) を表示するか。クリーン UI 好み層向け
    bool showDamageNumbers = true;
    float bgmVolume = 0.85f;
    float sfxVolume = 0.85f;
    // HUD bar scales (HP/EXP/Wave panels)
    float hudHpScale = 1f;
    float hudExpScale = 1f;
    float hudWaveScale = 1f;
    static readonly float[] HudScalePresets = { 0.70f, 0.85f, 1.00f, 1.20f, 1.45f, 1.75f };
    Enemy bossEnemy;
    string waveTrait = "Normal";
    float waveTouchDamageMult = 1f;

    // Normal difficulty baseline: keep non-meta partners viable before their build comes online.
    float playerHp = 6f;
    float playerMaxHp = 6f;
    float lanternHp = 24f;
    float lanternMaxHp = 24f;
    float xp;
    float xpToLevel = 7f;
    float dataChips;
    int level = 1;

    float moveSpeed = 4.9f;
    float fireRate = 2.8f;
    int bulletCount = 1;
    int bulletPierce;
    float bulletDamage = 1f;
    float bulletSpeed = 10f;
    float bulletSize = 0.16f;
    float bulletLifeMultiplier = 1f;
    float lightRadius = 5.2f;
    float pickupRange = 1.25f;
    float explodeChance;
    bool orbitShield;
    float orbitDamage = 1.7f;
    // ── Knockback (GUARD route) ──
    // Pushes enemies away on contact / shield hit. GUARD form & specific modules raise this.
    float playerKnockback;          // strength applied on contact (also = radial burst meters)
    float orbitKnockback;           // strength applied when shield ring damages enemy
    float playerKnockbackCooldown;  // cooldown timer for radial knockback burst (sec)
    const float PlayerKnockbackInterval = 0.18f; // how often the AoE pulse can re-fire
    float dataMultiplier = 1f;
    float pickupHealChance;
    float bossDamageMultiplier = 1f;
    float contactBurst;
    int speedChainBonus;
    bool speedEchoActive;
    bool guardPulseUpgrade;
    bool reflectShield;
    float speedEchoTimer;
    float guardPulseTimer;
    bool scatterSynergy;
    bool lanceSynergy;
    bool recoverySynergy;
    bool aegisSynergy;
    // ── Evolution combos (Vampire Survivors 風: 特定モジュールの絁E��合わせで上位融吁E ──
    bool evoStellarHalo;
    bool evoPhotonLance;
    bool evoHyperSwarm;
    bool evoCascadeStorm;
    bool evoCrimsonHunter;
    bool evoCriticalCascade;
    bool evoBurstStorm;
    bool evoDataHarvester;
    float stellarHaloPulseTimer;
    const float StellarHaloPulseInterval = 2.0f;
    float cascadeStormSplashRadius;          // Cascade Storm: 突撃命中時�E追加爆発半征E(0=無効)
    // ── Danger Level (Brotato 風: クリアで上位難度解放) ──
    const int MaxDangerLevel = 5;
    const string DangerSelectedKey = "Danger_Selected";
    const string DangerMaxClearedKey = "Danger_MaxCleared";
    int dangerLevel = 0;
    int dangerMaxCleared = 0;
    Text dangerSelectorLabel;
    readonly List<Button> dangerChipButtons = new();
    // ── Stage system (STAGE_DESIGN_SPEC.md 準拠) ──
    // Stage 0 = Lantern Field (default, 既存挙勁E
    // Stage 1 = Lava Cache (溶岩 + 熱噴出口)
    // Stage 2 = Broken Core Network (汚染パッチ+ リレー裁E��)
    // Stage 3 = Frost Vault (氷クリスタル破壊報酬 + 結霜パッチ移動低丁E
    // Stage 4 = Storm Spire (ランダム雷撁Eハイリスク&ハイリワーチE
    const int MaxStageId = 4;
    const string StageSelectedKey = "Stage_Selected";
    const string StageMaxUnlockedKey = "Stage_MaxUnlocked";
    int currentStageId = 0;
    int stageMaxUnlocked = 0;
    string stageName = "Lantern Field";
    // stageSubtitle / stageVisionMultiplier: 未使用 (Roslyn 警告 0 化のため削除)
    readonly List<StageHazardZone> stageHazards = new();
    readonly List<GameObject> stageObjects = new();
    System.Random stageRandom = new System.Random();
    float playerLavaContactTimer;
    float playerLavaTickTimer;
    bool corruptionSlowActive;             // Stage 3 汚染パッチ接触中の移動低下フラグ
    Text stageSelectorLabel;
    readonly List<Button> stageChipButtons = new();
    GameObject runConfigPanel;
    Text mainMenuSelectionBriefText;
    GameObject evolutionComboCodexPanel;
    Text evolutionComboCodexProgressText;
    Button menuComboButton;
    readonly List<GameObject> evolutionComboCodexCards = new();
    readonly List<Text> evolutionComboCodexTitles = new();
    readonly List<Text> evolutionComboCodexReqsTexts = new();
    readonly List<Text> evolutionComboCodexDescTexts = new();
    readonly List<Outline> evolutionComboCodexOutlines = new();
    // Stage 2 (Lava Cache) constants
    // ── 2026-05-22 Play 想定�E保守チューニング ──
    // spec の「罰よりも探索」方釁E+ Codex の溶岩縁データ報酬と絁E��合わぁE
    // - レーン幁E1.3 →1.7 (安�Eな通路を確俁E
    // - プ�Eル数 5 →4 (寁E��緩咁E
    // - プ�Eル最大サイズ 1.35 →1.20 (大きすぎる罠を回避)
    // - grace 0.55 →0.75s (掠めただけならノーダメ)
    // - DPS 0.24 →0.18 (1秒接触ダメ 0.24 →0.18)
    // - Vent チE��グラチE0.85 →1.10s (反応時間確俁E
    // - コア/スポ�Eン安�E距離 +0.4 ずつ拡大
    const float LavaPoolMinRingRadius = 3.6f;
    const float LavaPoolMaxRingRadius = 9.6f;
    const float LavaPoolSizeMin = 0.80f;
    const float LavaPoolSizeMax = 1.20f;
    const int LavaPoolCount = 4;
    const int HeatVentCount = 2;
    const float HeatVentMinRingRadius = 8.0f;
    const float HeatVentMaxRingRadius = 11.4f;
    const float LavaDamagePerSecond = 0.18f;
    const float LavaGraceSeconds = 0.75f;
    const float HeatVentPulseInterval = 14f;
    const float HeatVentPulseDuration = 3f;
    const float HeatVentTelegraphSeconds = 1.10f;
    const float HeatVentVisionMultiplier = 0.76f;
    const float DefaultStage2VisionMultiplier = 0.9f;
    const float MinCombinedVisionMultiplier = 0.68f;
    const float HazardSafetyFromCore = 2.4f;
    const float HazardSafetyFromPlayerSpawn = 2.0f;
    const float HazardSafeLaneWidth = 1.7f;
    // Stage 2 Thermal Surge (Phase 2 未実裁E��の Nullwyrm 追加プレチE��ャー)
    const float BossThermalSurgeCooldown = 8f;
    float bossThermalSurgeTimer;
    // ── Stage 3 (Broken Core Network) constants ──
    // 「報酬重視、罰は弱め」方釁E 汚染パッチ�E Stage 2 lava より優しい
    const int CorruptionPatchCount = 4;
    const float CorruptionPatchMinRingRadius = 3.6f;
    const float CorruptionPatchMaxRingRadius = 10.0f;
    const float CorruptionPatchSizeMin = 0.85f;
    const float CorruptionPatchSizeMax = 1.30f;
    const float CorruptionDamagePerSecond = 0.10f;
    const float CorruptionGraceSeconds = 0.85f;
    const float CorruptionSlowMultiplier = 0.75f;
    const int RelayDeviceCount = 3;
    const float RelayDeviceMinRingRadius = 4.5f;
    const float RelayDeviceMaxRingRadius = 9.5f;
    const float RelayDeviceRadius = 0.85f;
    const float RelayActivationSeconds = 2.0f;             // 立ち続け時間
    const float RelayDataReward = 25f;                     // 起動�E功で +25 データ
    const float RelayHealReward = 2f;                      // + 自分P+2
    const float RelayCoreHealReward = 2f;                  // + コアHP+2
    // ── Stage 4 (Frost Vault) constants ──
    const int FrostCrystalCount = 4;
    const float FrostCrystalMinRingRadius = 4.0f;
    const float FrostCrystalMaxRingRadius = 10.0f;
    const float FrostCrystalRadius = 0.70f;
    const float FrostCrystalHp = 12f;
    const float FrostCrystalFreezeRadius = 1.5f;
    const float FrostCrystalFreezeSeconds = 2.5f;
    const float FrostCrystalDataReward = 15f;
    const int FrostPatchCount = 3;
    const float FrostPatchMinRingRadius = 3.5f;
    const float FrostPatchMaxRingRadius = 9.0f;
    const float FrostPatchSizeMin = 0.90f;
    const float FrostPatchSizeMax = 1.40f;
    const float FrostPatchSlowMultiplier = 0.75f;
    bool frostPatchSlowActive;
    // ── Stage 5 (Storm Spire) constants ──
    const float LightningStrikeInterval = 12f;
    const float LightningTelegraphSeconds = 1.5f;
    const float LightningStrikeRadius = 2.5f;
    const float LightningEnemyDamage = 12f;
    const float LightningStunSeconds = 1.5f;
    const float LightningPlayerDamage = 3f;
    float lightningNextStrikeTimer;
    // ── アクセシビリチE��補正 (難度とは独立、Option で 0.5x、E.5x 調整) ──
    const string AccessibilityHpKey = "Accessibility_EnemyHpMul";
    const string AccessibilityDmgKey = "Accessibility_EnemyDmgMul";
    const string AccessibilitySpdKey = "Accessibility_EnemySpdMul";
    float accessibilityEnemyHpMul = 1f;
    float accessibilityEnemyDmgMul = 1f;
    float accessibilityEnemySpdMul = 1f;
    static readonly float[] AccessibilityPresets = { 0.5f, 0.75f, 1.0f, 1.25f, 1.5f };
    // New module fields
    float critChance;
    bool shockwaveOnHit;
    float playerAuraDamage;
    float playerAuraRadius = 0.75f;     // melee characters extend this far beyond default
    bool waveKillActive;
    int waveKillCounter;
    float bonusDataOnKill;
    bool bulletLifesteal;
    // allyName 削除: 未使用 (Roslyn 警告 0 化のため)。リンクの存在チェックは allyNova/Bulwark/Siphon/Phase で十分
    bool allyNova;
    bool allyBulwark;
    bool allySiphon;
    bool allyPhase;
    float phaseDodgeTimer;
    float trailTimer;
    bool bossModuleReady = false;
    int runMaxKillWave;
    int runMaxWaveKills;
    float vacuumSurgeTimer;
    readonly System.Collections.Generic.HashSet<string> relicSet = new();
    bool choosingRelic;
    bool deathShieldActive;
    // Last Stand: ラン中 1回限り、�E死ダメージめE1HP まで救渁E(Brotato / Hades 風)
    // deathShield relic とは独立。
    bool lastStandUsed;
    float lastStandInvulnTimer;
    const float LastStandReviveHp = 1f;
    const float LastStandInvulnSeconds = 1.4f;
    bool corePulseRelicActive;
    float corePulseTimer;
    bool fusionActive;
    string fusionName = "未融合";
    // ── クロス進化選択キュー (auto-trigger →ユーザー選択化) ──
    string pendingFusionName;
    string pendingFusionMessage;
    System.Action pendingFusionApply;
    bool queuedCutscene;
    string queuedCutsceneTitle;
    string queuedCutsceneName;
    string queuedCutsceneDesc;
    Color queuedCutsceneColor;
    float allyShotTimer;
    int evolutionStage;
    int formStyle;
    int evolutionVariantStyle;
    int fusionStyle;
    int partnerStyle;
    int evolutionTreeSelectedSpecies = 1;
    string formName = "プチフォーム";
    string partnerName = "Cobalt Pup";
    string partnerTrait = "標準型";
    int bestWave;
    string collectionSummary = "";

    float runDamageDealt;
    int runEnemiesKilled;
    int runBossKilled;
    readonly List<string> runPickedModules = new();
    readonly Dictionary<string, int> runModulePicks = new();
    float cardAppearStartTime;
    // 誤クリチE��防止: パネルが開ぁE��直征E0.45s はカード選択を無効匁E    // (前�Ewave クリチE��余韻で意図しなぁE��ジュール/購入が選ばれる事故を防ぁE
    const float ChoiceClickGuardDuration = 0.45f;
    float choiceClickGuardUntil;
    readonly Dictionary<Button, Vector2> cardBasePositions = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void StartGame()
    {
        if (FindAnyObjectByType<CoreLanternGame>() != null)
            return;

        var go = new GameObject("Core Lantern Game");
        go.AddComponent<CoreLanternGame>();
    }

    void Awake()
    {
        rng = new System.Random();
        Time.timeScale = 1f;
        LoadProgress();
        CreateAssets();
        CreateWorld();
        CreateUi();
        BuildUpgrades();
        ShowTitle();
        if (PlayerPrefs.GetInt(AutoStartRunKey, 0) == 1)
        {
            PlayerPrefs.SetInt(AutoStartRunKey, 0);
            PlayerPrefs.Save();
            StartRun();
        }
    }

    void Update()
    {
        // Mirror module status side panel visibility to upgrade panel (they used to share parent)
        if (moduleStatusSidePanel != null && upgradePanel != null
            && moduleStatusSidePanel.activeSelf != upgradePanel.activeSelf)
            moduleStatusSidePanel.SetActive(upgradePanel.activeSelf);
        RepairActiveChoiceButtonInteractivity();

        // Binary rain runs on unscaledDeltaTime so it keeps falling during pause/upgrade.
        // Skip when game over / victory  Eresult panel covers screen, and skipping
        // prevents touching potentially stale transforms.
        if (!gameOver && !victory)
            UpdateBinaryRain();

        // ── Hit freeze: brief Time.timeScale=0 for impact on big hits ──
        if (hitFreezeTimer > 0f)
        {
            hitFreezeTimer -= Time.unscaledDeltaTime;
            if (hitFreezeTimer <= 0f && !paused && !choosingUpgrade && !choosingRelic && !gameOver && !victory)
                Time.timeScale = 1f;
        }

        // ── Boss cutscene update (runs on unscaledDeltaTime, pauses game while active) ──
        if (bossCutscenePanel != null && bossCutscenePanel.activeSelf)
        {
            // Pause gameplay while cutscene plays (but only if not already paused for menu)
            if (!paused && !choosingUpgrade && !choosingRelic) Time.timeScale = 0f;
            UpdateBossCutscene();
            if (!bossCutscenePanel.activeSelf && !paused && !choosingUpgrade && !choosingRelic && !gameOver && !victory)
                Time.timeScale = 1f;
            return;
        }

        if (titleScreen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (codexPanel != null && codexPanel.activeSelf) ToggleCodexPanel();
                else if (evolutionTreePanel != null && evolutionTreePanel.activeSelf) ToggleEvolutionTreePanel();
                else if (evolutionComboCodexPanel != null && evolutionComboCodexPanel.activeSelf) ToggleEvolutionComboCodexPanel();
                else if (missionBoardPanel != null && missionBoardPanel.activeSelf) ToggleMissionBoardPanel();
                else if (optionsPanel != null && optionsPanel.activeSelf) ToggleOptionsPanel();
                else if (runConfigPanel != null && runConfigPanel.activeSelf) CloseRunConfig();
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                // RunConfig 表示中なら確宁E/ それ以外なめERunConfig を開ぁE(ESC で戻れる)
                if (runConfigPanel != null && runConfigPanel.activeSelf)
                    ConfirmRunConfig();
                else if (!IsTitleOverlayOpen())
                    OpenRunConfig();
            }
            return;
        }

        if (partnerPanel != null && partnerPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnFromPartnerSelectToMenu();
            return;
        }

        if (gameOver || victory)
        {
            if (Input.GetKeyDown(KeyCode.R))
                RetryRun();
            if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Escape))
                ReturnToMainMenu();
            return;
        }

        if (evolutionCutsceneTimer > 0f)
        {
            UpdateEvolutionCutscene();
            return;
        }

        if (Input.GetKeyDown(KeyCode.O) && !choosingUpgrade && !choosingRelic && !gameOver && !victory)
            ToggleOptionsPanel();

        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) && !choosingUpgrade && !choosingRelic)
            SetPaused(!paused);
        if (Input.GetKeyDown(KeyCode.Escape) && (choosingUpgrade || choosingRelic))
        {
            // Selection required  Eflash the title to signal it
            if (panelTitleText != null)
                panelTitleText.color = new Color(1f, 0.35f, 0.28f);
            PlaySfx("Hit", 280f, 0.06f, 0.12f);
        }

        // ── プロ改喁E モジュール選択中の Lock (Q/W/E) / Banish (Z/X/C) ──
        if (choosingUpgrade && !choosingRelic && upgradePanel != null && upgradePanel.activeSelf && currentUpgradeChoices.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Q)) TryToggleLockSlot(0);
            if (Input.GetKeyDown(KeyCode.W)) TryToggleLockSlot(1);
            if (Input.GetKeyDown(KeyCode.E)) TryToggleLockSlot(2);
            if (Input.GetKeyDown(KeyCode.Z)) TryBanishSlot(0);
            if (Input.GetKeyDown(KeyCode.X)) TryBanishSlot(1);
            if (Input.GetKeyDown(KeyCode.C)) TryBanishSlot(2);
        }

        if (paused)
        {
            UpdateChoiceCardEffects();
            return;
        }

        if (choosingUpgrade)
        {
            UpdateChoiceCardEffects();
            return;
        }

        if (choosingRelic)
        {
            UpdateChoiceCardEffects();
            return;
        }

        HandlePlayer();
        HandleShooting();
        HandleAlly();
        UpdateRouteAbilities();
        HandleSpawning();
        elapsedTime += Time.deltaTime;
        if (lastStandInvulnTimer > 0f)
            lastStandInvulnTimer -= Time.deltaTime;
        UpdateEvolutionComboBanner();
        UpdateSolarAnchor();
        UpdateEnemies();
        UpdateStageHazards(Time.deltaTime);
        UpdateBullets();
        UpdatePickups();
        UpdateSparks();
        UpdateSpriteGhosts();
        UpdateFloatingTexts();
        UpdateWave();
        UpdatePlayerVisualEffects();
        UpdateCamera();
        UpdateScreenEffects();
        UpdateUi();
    }

    void CreateAssets()
    {
        circleSprite = MakeSprite(64, (x, y, size) =>
        {
            var dx = x - size * 0.5f + 0.5f;
            var dy = y - size * 0.5f + 0.5f;
            return dx * dx + dy * dy <= size * size * 0.25f;
        });

        squareSprite = MakeSprite(16, (x, y, size) => true);

        diamondSprite = MakeSprite(32, (x, y, size) =>
        {
            var dx = Mathf.Abs(x - size * 0.5f + 0.5f);
            var dy = Mathf.Abs(y - size * 0.5f + 0.5f);
            return dx + dy <= size * 0.48f;
        });

        var generatedForms = new[]
        {
            MakePartnerSprite(0),
            MakePartnerSprite(1),
            MakePartnerSprite(2),
            MakePartnerSprite(3)
        };
        playerSprite = LoadOptionalSprite("Skins/Player", generatedForms[0]);
        playerFormSprites = new[]
        {
            LoadOptionalSprite("Skins/Player_Form1", playerSprite),
            LoadOptionalSprite("Skins/Player_Form2", generatedForms[1]),
            LoadOptionalSprite("Skins/Player_Form3", generatedForms[2]),
            LoadOptionalSprite("Skins/Player_Form4", generatedForms[3])
        };
        lanternSprite = LoadOptionalSprite("Skins/Lantern", MakeDataEggSprite());
        runnerSprite = LoadOptionalSprite("Skins/Runner", MakeRunnerSprite());
        bruteSprite = LoadOptionalSprite("Skins/Brute", MakeBruteSprite());
        shooterSprite = LoadOptionalSprite("Skins/Shooter", MakeShooterSprite());
        bossSprite = LoadOptionalSprite("Skins/Boss", MakeBossSprite());
        dasherSprite = LoadOptionalSprite("Skins/Dasher", MakeDasherSprite());
        bomberSprite = LoadOptionalSprite("Skins/Bomber", MakeBomberSprite());
        phantomSprite = LoadOptionalSprite("Skins/Phantom", MakePhantomSprite());
        lavaCrawlerSprite = LoadOptionalSprite("Skins/LavaCrawler", MakeLavaCrawlerSprite());
        // Stage-specific enemy sprites; use existing sprites as fallbacks until generated art is present.
        magmaTitanSprite = LoadOptionalSprite("Skins/Enemy_MagmaTitan", bruteSprite);
        corruptionDroneSprite = LoadOptionalSprite("Skins/Enemy_CorruptionDrone", phantomSprite);
        frostKnightSprite = LoadOptionalSprite("Skins/Enemy_FrostKnight", bruteSprite);
        voltDasherSprite = LoadOptionalSprite("Skins/Enemy_VoltDasher", dasherSprite);
        // Boss-specific sprites; shared bossSprite remains the fallback.
        pulswyrmSprite = LoadOptionalSprite("Skins/Boss_Pulswyrm", bossSprite);
        nullwyrmSprite = LoadOptionalSprite("Skins/Boss_Nullwyrm", bossSprite);
        binaryZeroSprite = MakeBinaryZeroSprite();
        binaryOneSprite = MakeBinaryOneSprite();
        novaLinkSprite = LoadOptionalSprite("Skins/Link_Nova", MakeLinkSprite(0));
        bulwarkLinkSprite = LoadOptionalSprite("Skins/Link_Bulwark", MakeLinkSprite(1));
        siphonLinkSprite = LoadOptionalSprite("Skins/Link_Siphon", MakeLinkSprite(2));
        phaseLinkSprite = LoadOptionalSprite("Skins/Link_Phase", MakeLinkSprite(3));
        floorTileASprite = LoadOptionalSprite("Skins/Floor_TileA", null);
        floorTileBSprite = LoadOptionalSprite("Skins/Floor_TileB", null);
        floorTileCSprite = LoadOptionalSprite("Skins/Floor_TileC", null);
        floorDarkBaseSprite = LoadOptionalSprite("Skins/Floor_DarkBase_A", null);
        floorGridOverlaySprite = LoadOptionalSprite("Skins/Floor_GridOverlay_A", null);
        floorCoreMarkSprite = LoadOptionalSprite("Skins/Floor_CoreMark_A", null);
        floorHazardLavaSprite = LoadOptionalSprite("Skins/Floor_Hazard_Lava", null);
        stage2LavaPoolSprite = LoadOptionalSprite("Skins/Stage2_LavaPool_A", null);
        stage2HeatVentSprite = LoadOptionalSprite("Skins/Stage2_HeatVent_A", null);
        stage3CorruptionSprite = LoadOptionalSprite("Skins/Stage3_CorruptionPatch_A", null);
        stage3RelayDeviceSprite = LoadOptionalSprite("Skins/Stage3_RelayDevice_A", null);
        stageThumbArenaSprite = LoadOptionalSprite("Skins/StageThumb_Arena", null);
        stageThumbLavaSprite = LoadOptionalSprite("Skins/StageThumb_Lava", null);
        stageThumbBrokenCoreSprite = LoadOptionalSprite("Skins/StageThumb_BrokenCore", null);
        stageThumbFrostSprite = LoadOptionalSprite("Skins/StageThumb_Frost", null);
        stageThumbStormSprite = LoadOptionalSprite("Skins/StageThumb_Storm", null);
        stage4FrostCrystalSprite = LoadOptionalSprite("Skins/Stage4_FrostCrystal_A", null);
        stage4FrostPatchSprite = LoadOptionalSprite("Skins/Stage4_FrostPatch_A", null);
        stage5LightningMarkerSprite = LoadOptionalSprite("Skins/Stage5_LightningMarker_A", null);
        stage5LightningStrikeSprite = LoadOptionalSprite("Skins/Stage5_LightningStrike_A", null);
        titleCoreEggSprite = LoadOptionalSprite("Skins/Title_CoreEgg_ASelected_v1", null);
        resultCoreEggSprite = LoadOptionalSprite("Skins/Result_CoreEgg_ASelected_v1", null);
        iconCoreEggSprite = LoadOptionalSprite("Skins/Icon_CoreEgg_ASelected_v1", null);
        pickupData64Sprite = LoadOptionalSprite("Skins/Pickup_Data_64", null);
        corePlatformSprite = LoadOptionalSprite("Skins/Core_Platform", null);
        coreRingOuterSprite = LoadOptionalSprite("Skins/Core_RingOuter", null);
        coreRingInnerSprite = LoadOptionalSprite("Skins/Core_RingInner", null);
        coreDamageCrack1Sprite = LoadOptionalSprite("Skins/Core_DamageCrack_1", null);
        coreDamageCrack2Sprite = LoadOptionalSprite("Skins/Core_DamageCrack_2", null);
        coreDamageCrack3Sprite = LoadOptionalSprite("Skins/Core_DamageCrack_3", null);
        propNeonPylonSprite = LoadOptionalSprite("Skins/Prop_NeonPylon_A", null);
        propDataTerminalSprite = LoadOptionalSprite("Skins/Prop_DataTerminal_A", null);
        propCrateSprite = LoadOptionalSprite("Skins/Prop_Crate_A", null);
        spawnRingNormalSprite = LoadOptionalSprite("Skins/Spawn_Ring_Normal", null);
        spawnRingBossSprite = LoadOptionalSprite("Skins/Spawn_Ring_Boss", null);
        floorCrackSprites = new[]
        {
            LoadOptionalSprite("Skins/Floor_Crack_A", null),
            LoadOptionalSprite("Skins/Floor_Crack_B", null)
        };
        floorCableSprites = new[]
        {
            LoadOptionalSprite("Skins/Floor_Cable_A", null),
            LoadOptionalSprite("Skins/Floor_Cable_B", null)
        };
        boundaryStoneSprites = new[]
        {
            LoadOptionalSprite("Skins/Boundary_Stone_A", null),
            LoadOptionalSprite("Skins/Boundary_Stone_B", null)
        };
        serverDebrisSprites = new[]
        {
            LoadOptionalSprite("Skins/Prop_ServerDebris_A", null),
            LoadOptionalSprite("Skins/Prop_ServerDebris_B", null)
        };
        cardCyanSprite = LoadOptionalSprite("Skins/Card_Cyan", null);
        cardGoldSprite = LoadOptionalSprite("Skins/Card_Gold", null);
        cardGreenSprite = LoadOptionalSprite("Skins/Card_Green", null);
        cardMagentaSprite = LoadOptionalSprite("Skins/Card_Magenta", null);
        cardRedSprite = LoadOptionalSprite("Skins/Card_Red", null);
        cardHeaderBasicV2Sprite = LoadOptionalSprite("Skins/Card_Header_Basic_v2", null);
        cardHeaderRareV2Sprite = LoadOptionalSprite("Skins/Card_Header_Rare_v2", null);
        cardHeaderEpicV2Sprite = LoadOptionalSprite("Skins/Card_Header_Epic_v2", null);
        cardTitlePlateV2Sprite = LoadOptionalSprite("Skins/Card_TitlePlate_v2", null);
        cardLevelPlateV2Sprite = LoadOptionalSprite("Skins/Card_LevelPlate_v2", null);
        cardRarityPlateBasicV2Sprite = LoadOptionalSprite("Skins/Card_RarityPlate_Basic_v2", null);
        cardRarityPlateRareV2Sprite = LoadOptionalSprite("Skins/Card_RarityPlate_Rare_v2", null);
        cardRarityPlateEpicV2Sprite = LoadOptionalSprite("Skins/Card_RarityPlate_Epic_v2", null);
        cardBottomRailCyanV2Sprite = LoadOptionalSprite("Skins/Card_BottomRail_Cyan_v2", null);
        cardBottomRailGoldV2Sprite = LoadOptionalSprite("Skins/Card_BottomRail_Gold_v2", null);
        cardSpecialCornerEpicV2Sprite = LoadOptionalSprite("Skins/Card_SpecialCorner_Epic_v2", null);
        cardSpecialCornerCrossV2Sprite = LoadOptionalSprite("Skins/Card_SpecialCorner_Cross_v2", null);
        bulletSpeedSprite = LoadOptionalSprite("Skins/Bullet_Speed", null);
        bulletPowerSprite = LoadOptionalSprite("Skins/Bullet_Power", null);
        bulletGuardSprite = LoadOptionalSprite("Skins/Bullet_Guard", null);
        bulletFusionSprite = LoadOptionalSprite("Skins/Bullet_Fusion", null);
        dataChipSprite = LoadOptionalSprite("Skins/Pickup_Data", null);
        evolutionRingSprite = LoadOptionalSprite("Skins/Evolution_Ring", null);
        panelFrameCyanSprite = LoadOptionalSprite("Skins/Panel_FrameCyan", null);
        panelFrameGoldSprite = LoadOptionalSprite("Skins/Panel_FrameGold", null);
        hudPanelWaveSprite = LoadOptionalSprite("Skins/HUD_Panel_Wave", null);
        hudPanelHpSprite = LoadOptionalSprite("Skins/HUD_Panel_HP", null);
        hudPanelLevelSprite = LoadOptionalSprite("Skins/HUD_Panel_Level", null);
        hudPanelChipMiniSprite = LoadOptionalSprite("Skins/HUD_Panel_ChipMini", null);
        hudPanelStatusSprite = LoadOptionalSprite("Skins/HUD_Panel_Status", null);
        hudPanelLoadoutSprite = LoadOptionalSprite("Skins/HUD_Panel_Loadout", null);
        hudPanelLinksSprite = LoadOptionalSprite("Skins/HUD_Panel_Links", null);
        hudBarBackSprite = LoadOptionalSprite("Skins/HUD_Bar_Back", null);
        hudBarPlayerHpFillSprite = LoadOptionalSprite("Skins/HUD_Bar_HP_PlayerFill", null);
        hudBarCoreHpFillSprite = LoadOptionalSprite("Skins/HUD_Bar_HP_CoreFill", null);
        hudBarHpLagSprite = LoadOptionalSprite("Skins/HUD_Bar_HP_Lag", null);
        hudBarExpFillSprite = LoadOptionalSprite("Skins/HUD_Bar_EXP_Fill", null);
        hudBarBackV2Sprite = LoadOptionalSprite("Skins/HUD_Bar_Back_v2", null);
        hudBarPlayerHpFillV2Sprite = LoadOptionalSprite("Skins/HUD_Bar_HP_PlayerFill_v2", null);
        hudBarCoreHpFillV2Sprite = LoadOptionalSprite("Skins/HUD_Bar_HP_CoreFill_v2", null);
        hudBarHpLagV2Sprite = LoadOptionalSprite("Skins/HUD_Bar_HP_Lag_v2", null);
        hudBarExpFillV2Sprite = LoadOptionalSprite("Skins/HUD_Bar_EXP_Fill_v2", null);
        hud9BarBackSprite = LoadOptionalSprite("Skins/HUD9_Bar_Back", null);
        hud9BarPlayerHpFillSprite = LoadOptionalSprite("Skins/HUD9_Bar_HP_PlayerFill", null);
        hud9BarCoreHpFillSprite = LoadOptionalSprite("Skins/HUD9_Bar_HP_CoreFill", null);
        hud9BarExpFillSprite = LoadOptionalSprite("Skins/HUD9_Bar_EXP_Fill", null);
        hud9PanelWaveSprite = LoadOptionalSprite("Skins/HUD9_Panel_Wave", null);
        hud9PanelHpSprite = LoadOptionalSprite("Skins/HUD9_Panel_HP", null);
        hud9PanelLevelSprite = LoadOptionalSprite("Skins/HUD9_Panel_Level", null);
        hud9PanelChipMiniSprite = LoadOptionalSprite("Skins/HUD9_Panel_ChipMini", null);
        hudWaveProgressFrameSprite = LoadOptionalSprite("Skins/HUD_WaveProgress_Frame", null);
        hudWaveProgressNormalSprite = LoadOptionalSprite("Skins/HUD_WaveProgress_Fill_Normal", null);
        hudWaveProgressBossSprite = LoadOptionalSprite("Skins/HUD_WaveProgress_Fill_Boss", null);
        hudWaveProgressFrameV2Sprite = LoadOptionalSprite("Skins/HUD_WaveProgress_Frame_v2", null);
        hudWaveProgressNormalV2Sprite = LoadOptionalSprite("Skins/HUD_WaveProgress_Fill_Normal_v2", null);
        hudWaveProgressBossV2Sprite = LoadOptionalSprite("Skins/HUD_WaveProgress_Fill_Boss_v2", null);
        hud9WaveProgressFrameSprite = LoadOptionalSprite("Skins/HUD9_WaveProgress_Frame", null);
        hud9WaveProgressNormalSprite = LoadOptionalSprite("Skins/HUD9_WaveProgress_Fill_Normal", null);
        hud9WaveProgressBossSprite = LoadOptionalSprite("Skins/HUD9_WaveProgress_Fill_Boss", null);
        hudBossBarFrameSprite = LoadOptionalSprite("Skins/HUD_BossBar_Frame", null);
        hudBossBarFillSprite = LoadOptionalSprite("Skins/HUD_BossBar_Fill", null);
        hudRelicSlotFrameSprite = LoadOptionalSprite("Skins/HUD_RelicSlot_Frame", null);
        hudIconPlayerHpSprite = LoadOptionalSprite("Skins/HUD_Icon_PlayerHP", null);
        hudIconCoreHpSprite = LoadOptionalSprite("Skins/HUD_Icon_CoreHP", null);
        hudIconExpSprite = LoadOptionalSprite("Skins/HUD_Icon_EXP", null);
        // 優先度: 専用 HUD アイコン > 新 64x64 pickup > 既孁Epickup
        hudIconDataChipSprite = LoadOptionalSprite("Skins/HUD_Icon_DataChip", null);
        if (hudIconDataChipSprite == null)
            hudIconDataChipSprite = pickupData64Sprite;
        hudBossWarningBannerSprite = LoadOptionalSprite("Skins/HUD_BossWarning_Banner", null);
        hudDangerPlayerSprite = LoadOptionalSprite("Skins/HUD_DangerVignette_Player", null);
        hudDangerCoreSprite = LoadOptionalSprite("Skins/HUD_DangerVignette_Core", null);
        hudChoiceDimSprite = LoadOptionalSprite("Skins/HUD_ChoiceDim", null);
        // Button V2 sprites
        btnPrimaryStartSprite    = LoadOptionalSprite("Skins/Button_Primary_Start_v2", null);
        btnRunStartSprite        = LoadOptionalSprite("Skins/Button_RunStart_v2", null);
        btnRunBackSprite         = LoadOptionalSprite("Skins/Button_RunBack_v2", null);
        btnRerollSprite          = LoadOptionalSprite("Skins/Button_Reroll_v2", null);
        btnSkipRewardSprite      = LoadOptionalSprite("Skins/Button_SkipReward_v2", null);
        btnPauseResumeSprite     = LoadOptionalSprite("Skins/Button_PauseResume_v2", null);
        btnWide320x42Sprite      = LoadOptionalSprite("Skins/Button_Wide_320x42_v2", null);
        btnResultRetrySprite     = LoadOptionalSprite("Skins/Button_ResultRetry_v2", null);
        btnResultMenuSprite      = LoadOptionalSprite("Skins/Button_ResultMenu_v2", null);
        btnMenuSecondarySprite   = LoadOptionalSprite("Skins/Button_MenuSecondary_v2", null);
        btnCloseSmallSprite      = LoadOptionalSprite("Skins/Button_CloseSmall_v2", null);
        btnCloseWideSprite       = LoadOptionalSprite("Skins/Button_CloseWide_v2", null);
        btnDangerChipSprite      = LoadOptionalSprite("Skins/Button_DangerChip_v2", null);
        btnRunStageChipSprite    = LoadOptionalSprite("Skins/Button_RunStageChip_v2", null);
        btnStepperSprite         = LoadOptionalSprite("Skins/Button_Stepper_v2", null);
        // Title UI V2 sprites
        gameLogoSprite             = LoadOptionalSprite("Skins/GameLogo", null);
        gameLogoTitleCompactSprite = LoadOptionalSprite("Skins/GameLogo_TitleCompact", gameLogoSprite);
        titleCoreEmblemSprite    = LoadOptionalSprite("Skins/Title_CoreEmblem_v2", null);
        titleSubtitlePlateSprite = LoadOptionalSprite("Skins/Title_SubtitlePlate_v2", null);
        titleStatsRibbonSprite   = LoadOptionalSprite("Skins/Title_StatsRibbon_v2", null);
        titleLogoUnderlineSprite = LoadOptionalSprite("Skins/Title_LogoUnderline_v2", null);
        titleNavRailSprite       = LoadOptionalSprite("Skins/Title_NavRail_v2", null);
        titleCornerAccentSprite  = LoadOptionalSprite("Skins/Title_CornerAccent_v2", null);
        // Result V2 sprites
        resultDeckFrameSprite    = LoadOptionalSprite("Skins/Result_DeckFrame_v2", null);
        resultPortraitFrameSprite = LoadOptionalSprite("Skins/Result_PortraitFrame_v2", null);
        resultStatTileSprite     = LoadOptionalSprite("Skins/Result_StatTile_v2", null);
        resultBadgeRouteSprite   = LoadOptionalSprite("Skins/Result_BadgeStrip_Route_v2", null);
        resultBadgeFusionSprite  = LoadOptionalSprite("Skins/Result_BadgeStrip_Fusion_v2", null);
        resultMvpRowSprite       = LoadOptionalSprite("Skins/Result_MvpRow_v2", null);
        resultSummaryPlateSprite = LoadOptionalSprite("Skins/Result_SummaryPlate_v2", null);
        resultFooterGuideSprite  = LoadOptionalSprite("Skins/Result_FooterGuide_v2", null);
        resultRankMedalSSprite   = LoadOptionalSprite("Skins/Result_RankMedal_S_v2", null);
        resultRankMedalASprite   = LoadOptionalSprite("Skins/Result_RankMedal_A_v2", null);
        resultRankMedalBSprite   = LoadOptionalSprite("Skins/Result_RankMedal_B_v2", null);
        resultRankMedalCSprite   = LoadOptionalSprite("Skins/Result_RankMedal_C_v2", null);
        resultRankMedalDSprite   = LoadOptionalSprite("Skins/Result_RankMedal_D_v2", null);
        mainMenuBackgroundSprite = LoadOptionalSprite("Skins/Background_MainMenu", LoadOptionalSprite("Skins/MainMenu_Background", null));
        resultClearBackgroundSprite = LoadOptionalSprite("Skins/Background_Victory", LoadOptionalSprite("Skins/Result_Clear_Background", null));
        resultGameOverBackgroundSprite = LoadOptionalSprite("Skins/Background_Defeat", LoadOptionalSprite("Skins/Result_GameOver_Background", null));
        bossPulswyrmBackgroundSprite = LoadOptionalSprite("Skins/Background_BossPulswyrm", null);
        bossNullwyrmBackgroundSprite = LoadOptionalSprite("Skins/Background_BossNullwyrm", null);
        cutinEvolveSpeedSprite = LoadOptionalSprite("Skins/Background_Evolution", LoadOptionalSprite("Skins/Cutin_Evolve_Speed", null));
        cutinEvolvePowerSprite = LoadOptionalSprite("Skins/Cutin_Evolve_Power", null);
        cutinEvolveGuardSprite = LoadOptionalSprite("Skins/Cutin_Evolve_Guard", null);
        cutinFusionSprite = LoadOptionalSprite("Skins/Background_CrossEvolution", LoadOptionalSprite("Skins/Cutin_Fusion", null));
        relicMagnetSprite = LoadOptionalSprite("Skins/Relic_Magnet", null);
        relicDeathlessSprite = LoadOptionalSprite("Skins/Relic_Deathless", null);
        relicCorePulseSprite = LoadOptionalSprite("Skins/Relic_CorePulse", null);
        relicExplosionSprite = LoadOptionalSprite("Skins/Relic_Explosion", null);
        relicStormSprite = LoadOptionalSprite("Skins/Relic_Storm", null);
        relicMirrorSprite = LoadOptionalSprite("Skins/Relic_Mirror", null);
        relicTitanSprite = LoadOptionalSprite("Skins/Relic_Titan", null);
        relicOverdriveSprite = LoadOptionalSprite("Skins/Relic_Overdrive", null);
        relicDataSurgeSprite = LoadOptionalSprite("Skins/Relic_DataSurge", null);
        relicApexSprite = LoadOptionalSprite("Skins/Relic_Apex", null);
        LoadModuleIconSprites();
    }

    void LoadModuleIconSprites()
    {
        moduleIconSprites.Clear();
        LoadModuleIcon("bullet", "ModuleIcon_Bullet");
        LoadModuleIcon("fire_rate", "ModuleIcon_FireRate");
        LoadModuleIcon("core_light", "ModuleIcon_CoreLight");
        LoadModuleIcon("damage", "ModuleIcon_Damage");
        LoadModuleIcon("speed", "ModuleIcon_Speed");
        LoadModuleIcon("repair", "ModuleIcon_Repair");
        LoadModuleIcon("magnet", "ModuleIcon_Magnet");
        LoadModuleIcon("explosion", "ModuleIcon_Explosion");
        LoadModuleIcon("guard_ring", "ModuleIcon_GuardRing");
        LoadModuleIcon("hp", "ModuleIcon_HP");
        LoadModuleIcon("core_repair", "ModuleIcon_CoreRepair");
        LoadModuleIcon("pierce", "ModuleIcon_Pierce");
        LoadModuleIcon("boss", "ModuleIcon_Boss");
        LoadModuleIcon("chain", "ModuleIcon_Chain");
        LoadModuleIcon("reflect", "ModuleIcon_Reflect");
        LoadModuleIcon("aura", "ModuleIcon_Aura");
        LoadModuleIcon("wave", "ModuleIcon_Wave");
        LoadModuleIcon("lifesteal", "ModuleIcon_Lifesteal");
        LoadModuleIcon("data", "ModuleIcon_DataForge");
        LoadModuleIcon("skip", "ModuleIcon_Skip");
    }

    void LoadModuleIcon(string key, string skinName)
    {
        var sprite = LoadOptionalSprite("Skins/" + skinName, null);
        if (sprite != null)
            moduleIconSprites[key] = sprite;
    }

    Sprite LoadOptionalSprite(string resourcePath, Sprite fallback)
    {
        var sprite = Resources.Load<Sprite>(resourcePath);
        if (sprite != null)
            return sprite;

        var texture = Resources.Load<Texture2D>(resourcePath);
        if (texture == null)
            return fallback;

        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        var pixelsPerUnit = Mathf.Max(1f, Mathf.Max(texture.width, texture.height));
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }

    Sprite LoadSkinSprite(string resourcePath)
    {
        if (skinSpriteCache.TryGetValue(resourcePath, out var cached))
            return cached;

        var sprite = LoadOptionalSprite(resourcePath, null);
        skinSpriteCache[resourcePath] = sprite;
        return sprite;
    }

    void ApplySimpleSprite(Image image, Sprite sprite, Color color)
    {
        if (image == null || sprite == null)
            return;

        image.sprite = sprite;
        image.color = color;
        image.type = Image.Type.Simple;
        image.preserveAspect = false;
    }

    void ApplySlicedSprite(Image image, Sprite sprite, Color color)
    {
        if (image == null || sprite == null)
            return;

        image.sprite = sprite;
        image.color = color;
        image.type = Image.Type.Sliced;
        image.preserveAspect = false;
    }

    void ApplyScreenBackgroundSprite(Image image, Sprite sprite, Color color)
    {
        if (image == null || sprite == null)
            return;

        image.sprite = sprite;
        image.color = color;
        image.type = Image.Type.Simple;
        image.preserveAspect = true;
        image.raycastTarget = false;
    }

    void ApplyFilledSprite(Image image, Sprite sprite, Color color)
    {
        if (image == null)
            return;

        if (sprite != null)
            image.sprite = sprite;
        image.color = color;
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
        image.fillOrigin = (int)Image.OriginHorizontal.Left;
        image.preserveAspect = false;
    }

    Sprite GetHudPanelSprite(string name)
    {
        if (name == "Wave Panel" || name == "Wave Progress Panel")
            return hudPanelWaveSprite;
        if (name == "HP Panel")
            return hudPanelHpSprite;
        if (name == "Level Panel")
            return hudPanelLevelSprite;
        if (name == "Chip Mini Panel" || name == "Data Panel")
            return hudPanelChipMiniSprite;
        if (name == "Loadout Panel")
            return hudPanelLoadoutSprite;
        if (name == "Partner Status Panel")
            return hudPanelLinksSprite;
        if (name == "Stats Panel" || name == "Event Log Panel")
            return hudPanelStatusSprite;
        if (name.Contains("Result"))
            return panelFrameGoldSprite != null ? panelFrameGoldSprite : hudPanelStatusSprite;
        if (name.Contains("Pause") || name.Contains("Options") || name.Contains("Mission") || name.Contains("Codex") || name.Contains("Evolution") || name.Contains("Tree") || name.Contains("Main"))
            return panelFrameCyanSprite != null ? panelFrameCyanSprite : hudPanelStatusSprite;
        return null;
    }

    void ApplyGeneratedPanelSkin(GameObject panel, string name)
    {
        if (panel == null)
            return;

        var image = panel.GetComponent<Image>();
        if (image == null)
            return;

        if (UseHud9SliceCandidates)
        {
            var hud9 = GetHud9PanelSprite(name);
            if (hud9 != null)
            {
                ApplySlicedSprite(image, hud9, Color.white);
                image.raycastTarget = true;
                return;
            }
        }

        if (!UseGeneratedHudPanels)
            return;

        var sprite = GetHudPanelSprite(name);
        if (sprite != null)
        {
            ApplySimpleSprite(image, sprite, Color.white);
            image.raycastTarget = true;
        }
    }

    Sprite GetHud9PanelSprite(string name)
    {
        if (name == "Wave Panel")
            return hud9PanelWaveSprite;
        if (name == "HP Panel")
            return hud9PanelHpSprite;
        if (name == "Level Panel")
            return hud9PanelLevelSprite;
        if (name == "Chip Mini Panel")
            return hud9PanelChipMiniSprite;
        return null;
    }

    Sprite GetHudBarFillSprite(string label)
    {
        if (label == "EXP")
            return hudBarExpFillSprite;
        if (label.Contains("HP"))
            return playerHpFill == null ? hudBarPlayerHpFillSprite : hudBarCoreHpFillSprite;
        return null;
    }

    Sprite GetHudBarFillV2Sprite(string label)
    {
        if (label == "EXP")
            return hudBarExpFillV2Sprite;
        if (label.Contains("HP"))
            return playerHpFill == null ? hudBarPlayerHpFillV2Sprite : hudBarCoreHpFillV2Sprite;
        return null;
    }

    Sprite GetHud9BarFillSprite(string label)
    {
        if (label == "EXP")
            return hud9BarExpFillSprite;
        if (label.Contains("HP"))
            return playerHpFill == null ? hud9BarPlayerHpFillSprite : hud9BarCoreHpFillSprite;
        return null;
    }

    Sprite GetHudBarIconSprite(string label)
    {
        if (label == "EXP")
            return hudIconExpSprite;
        if (label.Contains("HP"))
            return playerHpFill == null ? hudIconPlayerHpSprite : hudIconCoreHpSprite;
        return null;
    }

    Image CreateHudIcon(Transform parent, string name, Sprite sprite, Vector2 position, Vector2 size)
    {
        if (sprite == null)
            return null;

        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.color = Color.white;
        image.preserveAspect = true;
        image.raycastTarget = false;
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return image;
    }

    Sprite GetRelicSprite(string id)
    {
        switch (id)
        {
            case "magnet": return relicMagnetSprite;
            case "deathless": return relicDeathlessSprite;
            case "core_pulse": return relicCorePulseSprite;
            case "explosion": return relicExplosionSprite;
            case "storm": return relicStormSprite;
            case "mirror": return relicMirrorSprite;
            case "titan": return relicTitanSprite != null ? relicTitanSprite : diamondSprite;
            case "overdrive": return relicOverdriveSprite != null ? relicOverdriveSprite : diamondSprite;
            case "data_surge": return relicDataSurgeSprite != null ? relicDataSurgeSprite : diamondSprite;
            case "apex": return relicApexSprite != null ? relicApexSprite : diamondSprite;
            default: return diamondSprite;
        }
    }

    Sprite GetModuleIconSprite(string key)
    {
        return moduleIconSprites.TryGetValue(key, out var sprite) ? sprite : null;
    }

    Sprite GetGeneratedModuleIcon(string title)
    {
        if (string.IsNullOrEmpty(title))
            return null;

        if (title.Contains("スキップ") || title.Contains("緊急補給"))
            return GetModuleIconSprite("skip");
        if (title.Contains("ヴァンパイア") || title.Contains("吸血") || title.Contains("Lifesteal"))
            return GetModuleIconSprite("lifesteal");
        if (title.Contains("反射") || title.Contains("リフレクト") || title.Contains("鏡") || title.Contains("Mirror"))
            return GetModuleIconSprite("reflect");
        if (title.Contains("チェイン") || title.Contains("エコー") || title.Contains("Phantom") || title.Contains("Mirage"))
            return GetModuleIconSprite("chain");
        if (title.Contains("波動") || title.Contains("波紋") || title.Contains("パルス") || title.Contains("Pulse") || title.Contains("サージ"))
            return GetModuleIconSprite("wave");
        if (title.Contains("オーラ") || title.Contains("Wraith") || title.Contains("フィールド"))
            return GetModuleIconSprite("aura");
        if (title.Contains("ボス") || title.Contains("Break") || title.Contains("ブレイカー"))
            return GetModuleIconSprite("boss");
        if (title.Contains("ピアス") || title.Contains("貫通") || title.Contains("ロング") || title.Contains("ホライゾン") || title.Contains("スプリット") || title.Contains("鎧貫通"))
            return GetModuleIconSprite("pierce");
        if (title.Contains("炎") || title.Contains("バースト") || title.Contains("インフェルノ") || title.Contains("Explosion"))
            return GetModuleIconSprite("explosion");
        if (title.Contains("マグネット") || title.Contains("回収") || title.Contains("Siphon") || title.Contains("データ圧縮") || title.Contains("データフォージ") || title.Contains("Data"))
            return GetModuleIconSprite("magnet") ?? GetModuleIconSprite("data");
        if (title.Contains("修復") || title.Contains("回復") || title.Contains("リカバリー") || title.Contains("Recovery"))
            return GetModuleIconSprite("repair");
        if (title.Contains("HP") || title.Contains("不屈"))
            return GetModuleIconSprite("hp");
        if (title.Contains("コア") || title.Contains("Core") || title.Contains("照射"))
            return GetModuleIconSprite("core_repair") ?? GetModuleIconSprite("core_light");
        if (title.Contains("ガード") || title.Contains("リング") || title.Contains("Bulwark") || title.Contains("Bastion") || title.Contains("盾"))
            return GetModuleIconSprite("guard_ring");
        if (title.Contains("パワー") || title.Contains("ヘビー") || title.Contains("巨弾") || title.Contains("ストライク") || title.Contains("POWER"))
            return GetModuleIconSprite("damage");
        if (title.Contains("クロック") || title.Contains("連射") || title.Contains("Fire") || title.Contains("サイクル"))
            return GetModuleIconSprite("fire_rate");
        if (title.Contains("スピード") || title.Contains("クイック") || title.Contains("移動") || title.Contains("Phase") || title.Contains("SPEED") || title.Contains("加速"))
            return GetModuleIconSprite("speed");
        if (title.Contains("デュアル") || title.Contains("ワイド") || title.Contains("弾") || title.Contains("Twin") || title.Contains("Volley") || title.Contains("Shot"))
            return GetModuleIconSprite("bullet");

        return null;
    }

    void ApplyModuleCardIcon(Button button, string title, Color accentColor)
    {
        var sprite = GetGeneratedModuleIcon(title);
        if (button == null || sprite == null)
            return;

        var icon = button.transform.Find("Card Icon");
        if (icon != null)
        {
            var iconImage = icon.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = sprite;
                iconImage.color = Color.white;
                iconImage.preserveAspect = true;
            }
            var iconRect = icon.GetComponent<RectTransform>();
            if (iconRect != null)
                iconRect.sizeDelta = new Vector2(88, 88);
        }

        var iconCore = button.transform.Find("Card Icon Core");
        if (iconCore != null)
        {
            var iconCoreImage = iconCore.GetComponent<Image>();
            if (iconCoreImage != null)
                iconCoreImage.color = Color.clear;
        }

        var ring = button.transform.Find("Card Energy Ring");
        if (ring != null)
        {
            var ringImage = ring.GetComponent<Image>();
            if (ringImage != null)
                ringImage.color = WithAlpha(accentColor, 0.09f);
        }
    }

    void ApplyRelicCardIcon(Button button, string relicId)
    {
        if (button == null)
            return;

        var relicSprite = GetRelicSprite(relicId);
        var icon = button.transform.Find("Card Icon");
        if (icon != null)
        {
            var iconImage = icon.GetComponent<Image>();
            if (iconImage != null && relicSprite != null)
            {
                iconImage.sprite = relicSprite;
                iconImage.color = Color.white;
                iconImage.preserveAspect = true;
            }
            var iconRect = icon.GetComponent<RectTransform>();
            if (iconRect != null)
                iconRect.sizeDelta = new Vector2(92, 92);
        }

        var iconCore = button.transform.Find("Card Icon Core");
        if (iconCore != null)
        {
            var iconCoreImage = iconCore.GetComponent<Image>();
            if (iconCoreImage != null)
                iconCoreImage.color = Color.clear;
        }
    }

    Sprite GetCurrentCutinSprite()
    {
        if (!UseGeneratedCutscenes)
            return null;

        if (!string.IsNullOrEmpty(queuedCutsceneTitle) && queuedCutsceneTitle.Contains("CROSS"))
            return cutinFusionSprite;
        if (formStyle == 2)
            return cutinEvolvePowerSprite;
        if (formStyle == 3)
            return cutinEvolveGuardSprite;
        return cutinEvolveSpeedSprite;
    }

    Sprite GetPartnerVariantSprite(int stage, int routeStyle, int fusedStyle, int species, int variantStyle = 0)
    {
        stage = Mathf.Clamp(stage, 0, 3);
        routeStyle = Mathf.Clamp(routeStyle, 0, 3);
        fusedStyle = Mathf.Clamp(fusedStyle, 0, 6);
        species = Mathf.Clamp(species, 1, 11);
        variantStyle = Mathf.Clamp(variantStyle, 0, 4);

        if (stage > 0 && (routeStyle > 0 || fusedStyle > 0))
        {
            var overridePaths = new List<string>();
            if (fusedStyle > 0)
            {
                if (variantStyle > 0)
                    overridePaths.Add("Skins/Partner_S" + species + "_F" + fusedStyle + "_V" + variantStyle + "_L" + stage);
                overridePaths.Add("Skins/Partner_S" + species + "_F" + fusedStyle + "_L" + stage);
                overridePaths.Add("Skins/Fusion_" + fusedStyle);
            }
            if (routeStyle > 0)
            {
                if (variantStyle > 0)
                {
                    overridePaths.Add("Skins/Route_" + routeStyle + "_V" + variantStyle + "_Stage" + stage);
                    overridePaths.Add("Skins/Partner_S" + species + "_R" + routeStyle + "_V" + variantStyle + "_L" + stage);
                }
                overridePaths.Add("Skins/Partner_S" + species + "_R" + routeStyle + "_L" + stage);
                overridePaths.Add("Skins/Route_" + routeStyle + "_Stage" + stage);
            }

            for (var i = 0; i < overridePaths.Count; i++)
            {
                var sprite = LoadSkinSprite(overridePaths[i]);
                if (sprite != null)
                    return sprite;
            }

            return MakePartnerVariantSprite(stage, routeStyle, fusedStyle, species, variantStyle);
        }

        var paths = new List<string>();
        if (fusedStyle > 0)
        {
            if (variantStyle > 0)
                paths.Add("Skins/Partner_S" + species + "_F" + fusedStyle + "_V" + variantStyle + "_L" + stage);
            paths.Add("Skins/Partner_S" + species + "_F" + fusedStyle + "_L" + stage);
        }
        if (routeStyle > 0 && variantStyle > 0)
            paths.Add("Skins/Partner_S" + species + "_R" + routeStyle + "_V" + variantStyle + "_L" + stage);
        if (stage > 0)
            paths.Add("Skins/Partner_S" + species + "_L" + stage);
        paths.Add("Skins/Partner_S" + species + "_L0");

        for (var i = 0; i < paths.Count; i++)
        {
            var sprite = LoadSkinSprite(paths[i]);
            if (sprite != null)
                return sprite;
        }

        if (stage == 0)
        {
            var baseSprite = LoadSkinSprite("Skins/Partner_S" + species + "_L0");
            if (baseSprite != null)
                return baseSprite;
        }

        return MakePartnerVariantSprite(stage, routeStyle, fusedStyle, species, variantStyle);
    }

    Sprite MakeSprite(int size, Func<int, int, int, bool> mask)
    {
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
                texture.SetPixel(x, y, mask(x, y, size) ? Color.white : Color.clear);
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    Sprite MakeColorSprite(int size, Func<float, float, Color> paint)
    {
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var nx = (x + 0.5f) / size * 2f - 1f;
                var ny = (y + 0.5f) / size * 2f - 1f;
                texture.SetPixel(x, y, paint(nx, ny));
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    Sprite MakePartnerSprite(int stage)
    {
        return MakeColorSprite(96, (x, y) =>
        {
            var outline = new Color(0.02f, 0.04f, 0.12f, 1f);
            var baseColor = Color.Lerp(new Color(0.25f, 0.86f, 1f), new Color(0.12f, 0.38f, 1f), stage / 3f);
            var bright = new Color(0.65f, 1f, 1f, 1f);
            var chest = new Color(0.92f, 1f, 1f, 1f);
            var horn = new Color(1f, 0.92f, 0.22f, 1f);
            var body = Ellipse(x, y + 0.08f, 0.42f + stage * 0.05f, 0.48f + stage * 0.04f);
            var head = Ellipse(x, y - 0.42f, 0.38f + stage * 0.03f, 0.28f + stage * 0.03f);
            var leftEar = Ellipse(x + 0.34f, y - 0.58f, 0.13f, 0.28f);
            var rightEar = Ellipse(x - 0.34f, y - 0.58f, 0.13f, 0.28f);
            var tail = Ellipse(x + 0.43f, y + 0.16f, 0.18f + stage * 0.03f, 0.11f);
            var claws = stage >= 1 && (Ellipse(x + 0.28f, y + 0.39f, 0.1f, 0.08f) || Ellipse(x - 0.28f, y + 0.39f, 0.1f, 0.08f));
            var wing = stage >= 2 && (Ellipse(x + 0.56f, y + 0.04f, 0.18f, 0.32f) || Ellipse(x - 0.56f, y + 0.04f, 0.18f, 0.32f));
            var crest = stage >= 3 && Triangle(x, y - 0.76f, 0f, -0.36f, 0.22f);
            var inner = body || head || leftEar || rightEar || tail || claws || wing || crest;
            var outer = Ellipse(x, y + 0.08f, 0.48f + stage * 0.05f, 0.54f + stage * 0.04f)
                || Ellipse(x, y - 0.42f, 0.44f, 0.34f)
                || Ellipse(x + 0.34f, y - 0.58f, 0.17f, 0.32f)
                || Ellipse(x - 0.34f, y - 0.58f, 0.17f, 0.32f)
                || Ellipse(x + 0.43f, y + 0.16f, 0.23f + stage * 0.03f, 0.15f)
                || (stage >= 1 && (Ellipse(x + 0.28f, y + 0.39f, 0.13f, 0.11f) || Ellipse(x - 0.28f, y + 0.39f, 0.13f, 0.11f)))
                || (stage >= 2 && (Ellipse(x + 0.56f, y + 0.04f, 0.22f, 0.36f) || Ellipse(x - 0.56f, y + 0.04f, 0.22f, 0.36f)))
                || (stage >= 3 && Triangle(x, y - 0.76f, 0f, -0.39f, 0.27f));

            if (outer && !inner)
                return outline;
            if (wing)
                return new Color(0.2f, 0.94f, 1f, 1f);
            if (tail || claws || crest)
                return horn;
            if (body || head || leftEar || rightEar)
            {
                var leftEye = Ellipse(x + 0.13f, y - 0.49f, 0.065f, 0.078f);
                var rightEye = Ellipse(x - 0.13f, y - 0.49f, 0.065f, 0.078f);
                var eyeSpark = Ellipse(x + 0.105f, y - 0.525f, 0.018f, 0.022f) || Ellipse(x - 0.155f, y - 0.525f, 0.018f, 0.022f);
                var cheek = Ellipse(x + 0.25f, y - 0.39f, 0.055f, 0.035f) || Ellipse(x - 0.25f, y - 0.39f, 0.055f, 0.035f);
                if (eyeSpark)
                    return Color.white;
                if (leftEye || rightEye)
                    return outline;
                if (cheek)
                    return new Color(0.72f, 1f, 1f, 1f);
                if (Ellipse(x, y + 0.11f, 0.22f, 0.26f))
                    return chest;
                if (Mathf.Abs(x) < 0.045f && y < -0.28f && y > -0.38f)
                    return bright;
                return baseColor;
            }
            return Color.clear;
        });
    }

    Sprite MakeStyledPartnerSprite(int stage, int style, int fusedStyle)
    {
        if (style == 0 && fusedStyle == 0)
            return MakePartnerSprite(stage);

        return MakeColorSprite(112, (x, y) =>
        {
            var outline = new Color(0.015f, 0.025f, 0.055f, 1f);
            var coreBlue = Color.Lerp(new Color(0.25f, 0.88f, 1f), new Color(0.1f, 0.34f, 1f), Mathf.Clamp01(stage / 3f));
            var baseColor = coreBlue;
            var accent = new Color(0.92f, 1f, 1f, 1f);
            var armor = new Color(0.18f, 0.28f, 0.36f, 1f);

            if (style == 1)
            {
                baseColor = new Color(0.24f, 0.96f, 1f, 1f);
                accent = new Color(0.95f, 1f, 0.35f, 1f);
                armor = new Color(0.1f, 0.34f, 0.48f, 1f);
            }
            else if (style == 2)
            {
                baseColor = new Color(1f, 0.58f, 0.16f, 1f);
                accent = new Color(1f, 0.92f, 0.22f, 1f);
                armor = new Color(0.48f, 0.12f, 0.08f, 1f);
            }
            else if (style == 3)
            {
                baseColor = new Color(0.34f, 0.96f, 0.58f, 1f);
                accent = new Color(0.88f, 1f, 0.72f, 1f);
                armor = new Color(0.14f, 0.34f, 0.22f, 1f);
            }

            if (fusedStyle == 1)
            {
                baseColor = new Color(0.38f, 0.88f, 1f, 1f);
                accent = new Color(1f, 0.36f, 0.95f, 1f);
                armor = new Color(0.16f, 0.08f, 0.28f, 1f);
            }
            else if (fusedStyle == 2)
            {
                baseColor = new Color(0.28f, 1f, 0.76f, 1f);
                accent = new Color(0.9f, 1f, 0.28f, 1f);
                armor = new Color(0.08f, 0.28f, 0.36f, 1f);
            }
            else if (fusedStyle == 3)
            {
                baseColor = new Color(1f, 0.8f, 0.28f, 1f);
                accent = new Color(0.45f, 1f, 0.58f, 1f);
                armor = new Color(0.25f, 0.22f, 0.08f, 1f);
            }

            var bulk = style == 2 || fusedStyle == 3 ? 0.08f : 0f;
            var slim = style == 1 || fusedStyle == 2 ? 0.05f : 0f;
            var body = Ellipse(x, y + 0.06f, 0.42f + stage * 0.045f + bulk - slim, 0.48f + stage * 0.035f + bulk);
            var head = Ellipse(x, y - 0.42f, 0.36f + stage * 0.025f + bulk * 0.35f, 0.27f + stage * 0.02f);
            var earOffset = style == 1 || fusedStyle == 2 ? 0.42f : 0.34f;
            var leftEar = Ellipse(x + earOffset, y - 0.58f, style == 1 ? 0.1f : 0.13f, style == 1 ? 0.36f : 0.28f);
            var rightEar = Ellipse(x - earOffset, y - 0.58f, style == 1 ? 0.1f : 0.13f, style == 1 ? 0.36f : 0.28f);
            var tail = Ellipse(x + 0.44f + stage * 0.02f, y + 0.16f, 0.17f + stage * 0.035f, style == 1 ? 0.08f : 0.11f);
            var feet = Ellipse(x + 0.22f, y + 0.44f, 0.11f, 0.08f) || Ellipse(x - 0.22f, y + 0.44f, 0.11f, 0.08f);
            var claws = stage >= 1 && (Ellipse(x + 0.31f, y + 0.35f, 0.09f, 0.08f) || Ellipse(x - 0.31f, y + 0.35f, 0.09f, 0.08f));
            var wing = (style == 1 && stage >= 1 || stage >= 2 || fusedStyle == 1)
                && (Ellipse(x + 0.57f, y + 0.02f, 0.16f, 0.35f) || Ellipse(x - 0.57f, y + 0.02f, 0.16f, 0.35f));
            var horn = (style == 2 || stage >= 2 || fusedStyle == 1)
                && (Triangle(x + 0.17f, y - 0.73f, 0f, -0.22f, 0.15f) || Triangle(x - 0.17f, y - 0.73f, 0f, -0.22f, 0.15f));
            var shell = (style == 3 || fusedStyle == 3)
                && (Ellipse(x, y + 0.03f, 0.25f + stage * 0.035f, 0.35f + stage * 0.025f) && !Ellipse(x, y + 0.03f, 0.18f, 0.26f));
            var coreMark = Ellipse(x, y + 0.08f, 0.12f, 0.16f);
            var fusionHalo = fusedStyle > 0 && Ellipse(x, y - 0.02f, 0.74f, 0.7f) && !Ellipse(x, y - 0.02f, 0.68f, 0.64f);
            var fusionOrb = fusedStyle == 2 && (Ellipse(x + 0.58f, y - 0.2f, 0.07f, 0.07f) || Ellipse(x - 0.58f, y - 0.2f, 0.07f, 0.07f));
            var fusionShield = fusedStyle == 3 && (Ellipse(x + 0.48f, y + 0.08f, 0.14f, 0.38f) || Ellipse(x - 0.48f, y + 0.08f, 0.14f, 0.38f));
            var fusionCrown = fusedStyle == 1 && Triangle(x, y - 0.84f, 0f, -0.34f, 0.25f);

            var inner = body || head || leftEar || rightEar || tail || feet || claws || wing || horn || shell || coreMark || fusionHalo || fusionOrb || fusionShield || fusionCrown;
            var outer = Ellipse(x, y + 0.06f, 0.48f + stage * 0.045f + bulk - slim, 0.54f + stage * 0.035f + bulk)
                || Ellipse(x, y - 0.42f, 0.42f + bulk * 0.3f, 0.33f)
                || Ellipse(x + earOffset, y - 0.58f, (style == 1 ? 0.14f : 0.17f), (style == 1 ? 0.4f : 0.32f))
                || Ellipse(x - earOffset, y - 0.58f, (style == 1 ? 0.14f : 0.17f), (style == 1 ? 0.4f : 0.32f))
                || Ellipse(x + 0.44f + stage * 0.02f, y + 0.16f, 0.23f + stage * 0.03f, 0.16f)
                || Ellipse(x + 0.22f, y + 0.44f, 0.14f, 0.11f)
                || Ellipse(x - 0.22f, y + 0.44f, 0.14f, 0.11f)
                || (stage >= 1 && (Ellipse(x + 0.31f, y + 0.35f, 0.13f, 0.11f) || Ellipse(x - 0.31f, y + 0.35f, 0.13f, 0.11f)))
                || ((style == 1 && stage >= 1 || stage >= 2 || fusedStyle == 1) && (Ellipse(x + 0.57f, y + 0.02f, 0.21f, 0.4f) || Ellipse(x - 0.57f, y + 0.02f, 0.21f, 0.4f)))
                || ((style == 2 || stage >= 2 || fusedStyle == 1) && (Triangle(x + 0.17f, y - 0.73f, 0f, -0.26f, 0.2f) || Triangle(x - 0.17f, y - 0.73f, 0f, -0.26f, 0.2f)))
                || ((style == 3 || fusedStyle == 3) && Ellipse(x, y + 0.03f, 0.3f + stage * 0.035f, 0.4f + stage * 0.025f))
                || (fusedStyle > 0 && Ellipse(x, y - 0.02f, 0.78f, 0.74f) && !Ellipse(x, y - 0.02f, 0.64f, 0.6f))
                || (fusedStyle == 2 && (Ellipse(x + 0.58f, y - 0.2f, 0.1f, 0.1f) || Ellipse(x - 0.58f, y - 0.2f, 0.1f, 0.1f)))
                || (fusedStyle == 3 && (Ellipse(x + 0.48f, y + 0.08f, 0.18f, 0.43f) || Ellipse(x - 0.48f, y + 0.08f, 0.18f, 0.43f)))
                || (fusedStyle == 1 && Triangle(x, y - 0.84f, 0f, -0.38f, 0.3f));

            if (outer && !inner)
                return outline;
            if (fusionHalo || fusionOrb || fusionCrown)
                return accent;
            if (fusionShield || shell)
                return armor;
            if (wing)
                return fusedStyle == 1 ? new Color(1f, 0.34f, 0.98f, 1f) : new Color(0.28f, 0.96f, 1f, 1f);
            if (horn || tail || claws || feet)
                return accent;
            if (body || head || leftEar || rightEar)
            {
                var leftEye = Ellipse(x + 0.13f, y - 0.48f, 0.062f, 0.074f);
                var rightEye = Ellipse(x - 0.13f, y - 0.48f, 0.062f, 0.074f);
                var eyeSpark = Ellipse(x + 0.105f, y - 0.515f, 0.018f, 0.02f) || Ellipse(x - 0.155f, y - 0.515f, 0.018f, 0.02f);
                var cheek = Ellipse(x + 0.25f, y - 0.38f, 0.052f, 0.033f) || Ellipse(x - 0.25f, y - 0.38f, 0.052f, 0.033f);
                if (eyeSpark)
                    return Color.white;
                if (leftEye || rightEye)
                    return outline;
                if (cheek)
                    return Color.Lerp(baseColor, Color.white, 0.45f);
                if (coreMark)
                    return new Color(0.96f, 1f, 1f, 1f);
                if (Mathf.Abs(x) < 0.045f && y < -0.28f && y > -0.38f)
                    return accent;
                return baseColor;
            }
            return Color.clear;
        });
    }

    Sprite MakeEvolvedPartnerSprite(int stage, int style, int fusedStyle)
    {
        if (style == 0 && fusedStyle == 0)
            return MakePartnerSprite(stage);

        return MakeColorSprite(144, (x, y) =>
        {
            var stage01 = Mathf.Clamp01(stage / 3f);
            var outline = new Color(0.008f, 0.015f, 0.04f, 1f);
            var baseColor = Color.Lerp(new Color(0.26f, 0.9f, 1f), new Color(0.12f, 0.34f, 1f), stage01);
            var accent = new Color(0.9f, 1f, 1f, 1f);
            var armor = new Color(0.14f, 0.25f, 0.34f, 1f);
            var soft = new Color(0.92f, 1f, 1f, 1f);

            if (style == 1)
            {
                baseColor = Color.Lerp(new Color(0.26f, 0.95f, 1f), new Color(0.1f, 0.58f, 1f), stage01);
                accent = new Color(0.86f, 1f, 0.26f, 1f);
                armor = new Color(0.08f, 0.3f, 0.5f, 1f);
            }
            else if (style == 2)
            {
                baseColor = Color.Lerp(new Color(1f, 0.58f, 0.18f), new Color(0.95f, 0.22f, 0.08f), stage01);
                accent = new Color(1f, 0.9f, 0.2f, 1f);
                armor = new Color(0.42f, 0.08f, 0.06f, 1f);
            }
            else if (style == 3)
            {
                baseColor = Color.Lerp(new Color(0.36f, 1f, 0.6f), new Color(0.12f, 0.78f, 0.36f), stage01);
                accent = new Color(0.88f, 1f, 0.7f, 1f);
                armor = new Color(0.08f, 0.28f, 0.18f, 1f);
            }

            if (fusedStyle == 1)
            {
                baseColor = new Color(0.32f, 0.86f, 1f, 1f);
                accent = new Color(1f, 0.34f, 0.96f, 1f);
                armor = new Color(0.16f, 0.06f, 0.28f, 1f);
            }
            else if (fusedStyle == 2)
            {
                baseColor = new Color(0.24f, 1f, 0.75f, 1f);
                accent = new Color(0.94f, 1f, 0.25f, 1f);
                armor = new Color(0.06f, 0.28f, 0.34f, 1f);
            }
            else if (fusedStyle == 3)
            {
                baseColor = new Color(1f, 0.78f, 0.24f, 1f);
                accent = new Color(0.44f, 1f, 0.58f, 1f);
                armor = new Color(0.27f, 0.2f, 0.05f, 1f);
            }

            var powerBulk = style == 2 || fusedStyle == 3 ? 0.09f : 0f;
            var speedSlim = style == 1 || fusedStyle == 2 ? 0.035f : 0f;
            var guardBulk = style == 3 ? 0.045f : 0f;
            var bodyRx = 0.34f + stage * 0.055f + powerBulk + guardBulk - speedSlim;
            var bodyRy = 0.43f + stage * 0.052f + powerBulk * 0.8f;
            var headRx = 0.31f + stage * 0.025f + powerBulk * 0.25f;
            var headRy = 0.23f + stage * 0.025f;

            var body = Ellipse(x, y + 0.08f, bodyRx, bodyRy);
            var head = Ellipse(x, y - 0.42f, headRx, headRy);
            var earReach = 0.32f + stage * 0.035f + (style == 1 ? 0.08f : 0f);
            var earTall = 0.26f + stage * 0.04f + (style == 1 ? 0.08f : 0f);
            var leftEar = Ellipse(x + earReach, y - 0.58f, 0.1f + stage * 0.012f, earTall);
            var rightEar = Ellipse(x - earReach, y - 0.58f, 0.1f + stage * 0.012f, earTall);
            var tail = Ellipse(x + 0.4f + stage * 0.055f, y + 0.17f, 0.15f + stage * 0.045f, 0.09f + stage * 0.012f);
            var tailTip = stage >= 1 && Ellipse(x + 0.52f + stage * 0.06f, y + 0.16f, 0.075f + stage * 0.018f, 0.07f);
            var paws = Ellipse(x + 0.22f, y + 0.43f, 0.09f + stage * 0.012f, 0.07f) || Ellipse(x - 0.22f, y + 0.43f, 0.09f + stage * 0.012f, 0.07f);
            var claws = stage >= 1 && (Ellipse(x + 0.31f, y + 0.34f, 0.075f + stage * 0.012f, 0.065f) || Ellipse(x - 0.31f, y + 0.34f, 0.075f + stage * 0.012f, 0.065f));
            var wing = (stage >= 2 || style == 1 || fusedStyle == 1) && (Ellipse(x + 0.55f + stage * 0.02f, y + 0.02f, 0.13f + stage * 0.025f, 0.29f + stage * 0.045f) || Ellipse(x - 0.55f - stage * 0.02f, y + 0.02f, 0.13f + stage * 0.025f, 0.29f + stage * 0.045f));
            var largeWing = stage >= 3 && (Ellipse(x + 0.7f, y + 0.0f, 0.11f, 0.38f) || Ellipse(x - 0.7f, y + 0.0f, 0.11f, 0.38f));
            var horn = (stage >= 2 || style == 2 || fusedStyle == 1) && (Triangle(x + 0.16f, y - 0.74f, 0f, -0.23f, 0.17f + stage * 0.025f) || Triangle(x - 0.16f, y - 0.74f, 0f, -0.23f, 0.17f + stage * 0.025f));
            var crest = stage >= 3 && Triangle(x, y - 0.84f, 0f, -0.28f, 0.28f);
            var shoulderArmor = stage >= 2 && (Ellipse(x + 0.34f, y - 0.02f, 0.11f + powerBulk, 0.18f + guardBulk) || Ellipse(x - 0.34f, y - 0.02f, 0.11f + powerBulk, 0.18f + guardBulk));
            var chestGem = stage >= 1 && Mathf.Abs(x) + Mathf.Abs(y - 0.06f) < 0.12f + stage * 0.012f;
            var armorLine = stage >= 2 && (Mathf.Abs(x) < 0.035f && y > -0.16f && y < 0.32f || Mathf.Abs(y + 0.04f) < 0.026f && Mathf.Abs(x) < 0.28f + stage * 0.02f);
            var cheek = Ellipse(x + 0.23f, y - 0.39f, 0.045f, 0.03f) || Ellipse(x - 0.23f, y - 0.39f, 0.045f, 0.03f);
            var muzzle = Ellipse(x, y - 0.34f, 0.15f + stage * 0.01f, 0.07f);
            var halo = fusedStyle > 0 && (Ellipse(x, y - 0.02f, 0.72f, 0.66f) && !Ellipse(x, y - 0.02f, 0.66f, 0.6f));
            var stageRing = stage >= 3 && fusedStyle == 0 && (Ellipse(x, y + 0.02f, 0.67f, 0.62f) && !Ellipse(x, y + 0.02f, 0.62f, 0.57f));
            var fusionOrb = fusedStyle == 2 && (Ellipse(x + 0.62f, y - 0.18f, 0.065f, 0.065f) || Ellipse(x - 0.62f, y - 0.18f, 0.065f, 0.065f));
            var fusionShield = fusedStyle == 3 && (Ellipse(x + 0.5f, y + 0.08f, 0.13f, 0.36f) || Ellipse(x - 0.5f, y + 0.08f, 0.13f, 0.36f));

            var inner = body || head || leftEar || rightEar || tail || tailTip || paws || claws || wing || largeWing || horn || crest || shoulderArmor || chestGem || armorLine || cheek || muzzle || halo || stageRing || fusionOrb || fusionShield;
            var outer = Ellipse(x, y + 0.08f, bodyRx + 0.055f, bodyRy + 0.06f)
                || Ellipse(x, y - 0.42f, headRx + 0.06f, headRy + 0.055f)
                || Ellipse(x + earReach, y - 0.58f, 0.14f + stage * 0.012f, earTall + 0.045f)
                || Ellipse(x - earReach, y - 0.58f, 0.14f + stage * 0.012f, earTall + 0.045f)
                || Ellipse(x + 0.4f + stage * 0.055f, y + 0.17f, 0.2f + stage * 0.045f, 0.13f + stage * 0.012f)
                || (stage >= 1 && Ellipse(x + 0.52f + stage * 0.06f, y + 0.16f, 0.105f + stage * 0.018f, 0.1f))
                || Ellipse(x + 0.22f, y + 0.43f, 0.125f + stage * 0.012f, 0.1f)
                || Ellipse(x - 0.22f, y + 0.43f, 0.125f + stage * 0.012f, 0.1f)
                || (stage >= 1 && (Ellipse(x + 0.31f, y + 0.34f, 0.11f + stage * 0.012f, 0.1f) || Ellipse(x - 0.31f, y + 0.34f, 0.11f + stage * 0.012f, 0.1f)))
                || ((stage >= 2 || style == 1 || fusedStyle == 1) && (Ellipse(x + 0.55f + stage * 0.02f, y + 0.02f, 0.18f + stage * 0.025f, 0.35f + stage * 0.045f) || Ellipse(x - 0.55f - stage * 0.02f, y + 0.02f, 0.18f + stage * 0.025f, 0.35f + stage * 0.045f)))
                || (stage >= 3 && (Ellipse(x + 0.7f, y + 0.0f, 0.155f, 0.43f) || Ellipse(x - 0.7f, y + 0.0f, 0.155f, 0.43f)))
                || ((stage >= 2 || style == 2 || fusedStyle == 1) && (Triangle(x + 0.16f, y - 0.74f, 0f, -0.27f, 0.22f + stage * 0.025f) || Triangle(x - 0.16f, y - 0.74f, 0f, -0.27f, 0.22f + stage * 0.025f)))
                || (stage >= 3 && Triangle(x, y - 0.84f, 0f, -0.32f, 0.33f))
                || (stage >= 2 && (Ellipse(x + 0.34f, y - 0.02f, 0.15f + powerBulk, 0.22f + guardBulk) || Ellipse(x - 0.34f, y - 0.02f, 0.15f + powerBulk, 0.22f + guardBulk)))
                || (fusedStyle > 0 && (Ellipse(x, y - 0.02f, 0.77f, 0.71f) && !Ellipse(x, y - 0.02f, 0.62f, 0.56f)))
                || (stage >= 3 && fusedStyle == 0 && (Ellipse(x, y + 0.02f, 0.71f, 0.66f) && !Ellipse(x, y + 0.02f, 0.58f, 0.53f)))
                || (fusedStyle == 2 && (Ellipse(x + 0.62f, y - 0.18f, 0.095f, 0.095f) || Ellipse(x - 0.62f, y - 0.18f, 0.095f, 0.095f)))
                || (fusedStyle == 3 && (Ellipse(x + 0.5f, y + 0.08f, 0.17f, 0.41f) || Ellipse(x - 0.5f, y + 0.08f, 0.17f, 0.41f)));

            if (outer && !inner)
                return outline;
            if (halo || stageRing || fusionOrb || crest)
                return accent;
            if (fusionShield || shoulderArmor || armorLine)
                return armor;
            if (wing || largeWing)
                return Color.Lerp(baseColor, accent, style == 1 || fusedStyle == 1 ? 0.64f : 0.42f);
            if (horn || tailTip || claws || paws)
                return accent;
            if (tail)
                return Color.Lerp(baseColor, accent, 0.38f);
            if (body || head || leftEar || rightEar || muzzle || cheek || chestGem)
            {
                var eyeSize = stage >= 3 ? 0.052f : 0.062f;
                var leftEye = Ellipse(x + 0.12f, y - 0.49f, eyeSize, 0.07f);
                var rightEye = Ellipse(x - 0.12f, y - 0.49f, eyeSize, 0.07f);
                var eyeSpark = Ellipse(x + 0.098f, y - 0.522f, 0.017f, 0.019f) || Ellipse(x - 0.145f, y - 0.522f, 0.017f, 0.019f);
                if (eyeSpark)
                    return Color.white;
                if (leftEye || rightEye)
                    return outline;
                if (chestGem)
                    return fusedStyle > 0 ? accent : soft;
                if (cheek)
                    return Color.Lerp(baseColor, Color.white, 0.48f);
                if (muzzle)
                    return Color.Lerp(baseColor, Color.white, 0.34f);
                if (Mathf.Abs(x) < 0.035f && y < -0.28f && y > -0.37f)
                    return accent;
                if (Ellipse(x, y + 0.12f, 0.16f + stage * 0.018f, 0.21f + stage * 0.02f))
                    return Color.Lerp(baseColor, soft, 0.52f);
                return baseColor;
            }
            return Color.clear;
        });
    }

    Sprite MakePartnerVariantSprite(int stage, int routeStyle, int fusedStyle, int species, int variantStyle = 0)
    {
        stage = Mathf.Clamp(stage, 0, 3);
        species = Mathf.Clamp(species, 1, 11);
        variantStyle = Mathf.Clamp(variantStyle, 0, 4);

        return MakeColorSprite(144, (x, y) =>
        {
            var stage01 = Mathf.Clamp01(stage / 3f);
            var outline = new Color(0.006f, 0.012f, 0.032f, 1f);
            var baseColor = Color.Lerp(GetPartnerBaseColor(species), GetPartnerDeepColor(species), stage01 * 0.55f);
            var accent = GetPartnerAccentColor(species);
            var armor = GetPartnerArmorColor(species);

            if (routeStyle == 1)
            {
                baseColor = Color.Lerp(baseColor, new Color(0.18f, 0.82f, 1f), 0.34f);
                accent = Color.Lerp(accent, new Color(0.58f, 1f, 1f), 0.42f);
            }
            else if (routeStyle == 2)
            {
                baseColor = Color.Lerp(baseColor, new Color(1f, 0.42f, 0.12f), 0.3f);
                accent = Color.Lerp(accent, new Color(1f, 0.86f, 0.18f), 0.48f);
            }
            else if (routeStyle == 3)
            {
                baseColor = Color.Lerp(baseColor, new Color(0.22f, 0.92f, 0.48f), 0.3f);
                accent = Color.Lerp(accent, new Color(0.72f, 1f, 0.54f), 0.44f);
                armor = Color.Lerp(armor, new Color(0.08f, 0.28f, 0.16f), 0.38f);
            }

            if (fusedStyle > 0)
                accent = GetFusionAccentColor(fusedStyle);

            var guard = species == 3;
            var hexCat = species == 4;
            var drift = species == 5;
            var ironBear = species == 6;
            var spark = species == 2;
            var cobalt = species == 1;
            var owl = hexCat;
            var echo = hexCat;

            var routeBulk = routeStyle == 2 ? 0.075f : routeStyle == 3 ? 0.045f : 0f;
            var routeSlim = routeStyle == 1 ? 0.045f : 0f;
            var speciesBulk = ironBear ? 0.18f : guard ? 0.14f : owl ? -0.005f : spark ? 0f : drift ? -0.025f : 0.02f;
            var bodyRx = 0.37f + stage * 0.055f + routeBulk + speciesBulk - routeSlim;
            var bodyRy = 0.45f + stage * 0.05f + (guard ? -0.035f : ironBear ? 0.035f : 0f) + routeBulk * 0.55f;
            var headRx = (ironBear ? 0.39f : owl ? 0.34f : guard ? 0.32f : 0.335f) + stage * 0.022f + routeBulk * 0.25f;
            var headRy = (ironBear ? 0.29f : owl ? 0.255f : 0.255f) + stage * 0.019f;

            var body = Ellipse(x, y + 0.08f, bodyRx, bodyRy);
            var head = Ellipse(x, y - 0.42f, headRx, headRy);
            var earY = y - 0.6f;
            var dogEar = cobalt && (Ellipse(x + 0.36f, earY, 0.13f, 0.32f) || Ellipse(x - 0.36f, earY, 0.13f, 0.32f));
            var kitEar = (spark || drift) && (Triangle(x + 0.28f, y - 0.69f, -0.03f, -0.39f, 0.24f) || Triangle(x - 0.28f, y - 0.69f, 0.03f, -0.39f, 0.24f));
            var catEar = hexCat && (Triangle(x + 0.25f, y - 0.67f, -0.04f, -0.33f, 0.22f) || Triangle(x - 0.25f, y - 0.67f, 0.04f, -0.33f, 0.22f));
            var bearEar = ironBear && (Ellipse(x + 0.25f, y - 0.63f, 0.12f, 0.13f) || Ellipse(x - 0.25f, y - 0.63f, 0.12f, 0.13f));
            var owlHorn = false;
            var moleSnout = guard && Ellipse(x, y - 0.51f, 0.2f, 0.11f);

            var tail = !guard && !ironBear && Ellipse(x + 0.48f + stage * 0.045f, y + 0.16f, drift ? 0.18f + stage * 0.065f : 0.19f + stage * 0.04f, drift ? 0.23f : 0.13f);
            var twinTail = (spark || echo || drift && stage >= 1) && (Ellipse(x + 0.56f, y + 0.31f, 0.15f + stage * 0.035f, 0.1f) || Ellipse(x + 0.53f, y + 0.0f, 0.15f + stage * 0.035f, 0.1f));
            var owlWing = hexCat && (Ellipse(x + 0.48f, y + 0.02f, 0.12f + stage * 0.018f, 0.32f + stage * 0.035f) || Ellipse(x - 0.48f, y + 0.02f, 0.12f + stage * 0.018f, 0.32f + stage * 0.035f));
            var moleClaw = guard && (Ellipse(x + 0.4f, y + 0.35f, 0.2f + stage * 0.026f, 0.105f) || Ellipse(x - 0.4f, y + 0.35f, 0.2f + stage * 0.026f, 0.105f));
            var bearClaw = ironBear && (Ellipse(x + 0.43f, y + 0.28f, 0.19f + stage * 0.03f, 0.12f) || Ellipse(x - 0.43f, y + 0.28f, 0.19f + stage * 0.03f, 0.12f));
            var ironPlate = ironBear && (Hex(x, y + 0.04f, 0.26f + stage * 0.025f) || Mathf.Abs(y + 0.3f) < 0.045f && Mathf.Abs(x) < 0.28f);
            var paws = Ellipse(x + 0.22f, y + 0.43f, 0.09f + stage * 0.012f, 0.07f) || Ellipse(x - 0.22f, y + 0.43f, 0.09f + stage * 0.012f, 0.07f);

            var routeWing = (routeStyle == 1 || fusedStyle == 1 || fusedStyle == 4) && (Ellipse(x + 0.6f, y + 0.03f, 0.13f + stage * 0.025f, 0.34f + stage * 0.055f) || Ellipse(x - 0.6f, y + 0.03f, 0.13f + stage * 0.025f, 0.34f + stage * 0.055f));
            var powerHorn = (routeStyle == 2 || fusedStyle == 1) && (Triangle(x + 0.16f, y - 0.77f, 0f, -0.28f, 0.19f + stage * 0.025f) || Triangle(x - 0.16f, y - 0.77f, 0f, -0.28f, 0.19f + stage * 0.025f));
            var guardShell = (routeStyle == 3 || fusedStyle == 3 || fusedStyle == 5) && (Ellipse(x, y + 0.03f, 0.28f + stage * 0.035f, 0.36f + stage * 0.025f) && !Ellipse(x, y + 0.03f, 0.2f, 0.27f));
            var stageCrest = stage >= 2 && Triangle(x, y - 0.82f, 0f, -0.24f - stage * 0.03f, 0.22f + stage * 0.03f);
            var chestGem = Mathf.Abs(x) + Mathf.Abs(y - 0.06f) < 0.105f + stage * 0.014f;
            var visor = hexCat && Mathf.Abs(y + 0.45f) < 0.045f && Mathf.Abs(x) < 0.22f;
            var echoWave = echo && (Ellipse(x, y + 0.02f, 0.7f, 0.58f) && !Ellipse(x, y + 0.02f, 0.63f, 0.51f));
            var sparkBolt = spark && (Mathf.Abs(x + 0.04f) + Mathf.Abs(y + 0.02f) < 0.12f || Triangle(x, y - 0.04f, 0.08f, -0.05f, 0.18f));
            var driftSlash = drift && (Mathf.Abs(x + y * 0.55f) < 0.035f && y > -0.12f && y < 0.42f || Mathf.Abs(x + y * 0.55f - 0.18f) < 0.026f && y > -0.02f && y < 0.38f);
            var guardRivet = guard && (Ellipse(x + 0.18f, y - 0.02f, 0.035f, 0.035f) || Ellipse(x - 0.18f, y - 0.02f, 0.035f, 0.035f));
            var speedRapidWings = routeStyle == 1 && variantStyle == 1 && (Ellipse(x + 0.72f, y + 0.0f, 0.09f + stage * 0.018f, 0.46f + stage * 0.05f) || Ellipse(x - 0.72f, y + 0.0f, 0.09f + stage * 0.018f, 0.46f + stage * 0.05f));
            var speedLaserFin = routeStyle == 1 && variantStyle == 2 && (Mathf.Abs(x) < 0.045f && y < -0.64f && y > -0.93f || Mathf.Abs(x + 0.38f) < 0.035f && y < -0.18f && y > -0.58f || Mathf.Abs(x - 0.38f) < 0.035f && y < -0.18f && y > -0.58f);
            var speedMirageAfterimage = routeStyle == 1 && variantStyle == 3 && (Ellipse(x + 0.42f, y + 0.08f, 0.22f, 0.42f) && !Ellipse(x + 0.42f, y + 0.08f, 0.16f, 0.34f) || Ellipse(x - 0.42f, y + 0.08f, 0.22f, 0.42f) && !Ellipse(x - 0.42f, y + 0.08f, 0.16f, 0.34f));
            var speedHomingPods = routeStyle == 1 && variantStyle == 4 && (Ellipse(x + 0.66f, y - 0.34f, 0.11f, 0.11f) || Ellipse(x - 0.66f, y - 0.34f, 0.11f, 0.11f) || Ellipse(x + 0.58f, y + 0.34f, 0.1f, 0.1f) || Ellipse(x - 0.58f, y + 0.34f, 0.1f, 0.1f));
            var powerExplosiveCore = routeStyle == 2 && variantStyle == 1 && (Ellipse(x, y + 0.1f, 0.21f + stage * 0.018f, 0.21f + stage * 0.018f) || Mathf.Abs(x) + Mathf.Abs(y - 0.1f) < 0.24f + stage * 0.018f);
            var powerGiantArms = routeStyle == 2 && variantStyle == 2 && (Ellipse(x + 0.56f, y + 0.13f, 0.2f + stage * 0.04f, 0.32f + stage * 0.035f) || Ellipse(x - 0.56f, y + 0.13f, 0.2f + stage * 0.04f, 0.32f + stage * 0.035f));
            var powerBreakerHorns = routeStyle == 2 && variantStyle == 3 && (Triangle(x + 0.26f, y - 0.84f, 0f, -0.34f, 0.3f + stage * 0.03f) || Triangle(x - 0.26f, y - 0.84f, 0f, -0.34f, 0.3f + stage * 0.03f));
            var powerSiegePlates = routeStyle == 2 && variantStyle == 4 && (Hex(x + 0.44f, y + 0.0f, 0.21f + stage * 0.025f) || Hex(x - 0.44f, y + 0.0f, 0.21f + stage * 0.025f) || Mathf.Abs(y + 0.66f) < 0.04f && Mathf.Abs(x) < 0.32f);
            var guardCoreShield = routeStyle == 3 && variantStyle == 1 && (Ellipse(x, y + 0.1f, 0.52f + stage * 0.035f, 0.48f + stage * 0.03f) && !Ellipse(x, y + 0.1f, 0.42f + stage * 0.02f, 0.38f + stage * 0.02f));
            var guardCounterSpikes = routeStyle == 3 && variantStyle == 2 && (Triangle(x + 0.58f, y - 0.2f, -0.02f, 0.03f, 0.2f + stage * 0.025f) || Triangle(x - 0.58f, y - 0.2f, 0.02f, 0.03f, 0.2f + stage * 0.025f) || Triangle(x + 0.4f, y + 0.38f, -0.02f, 0.16f, 0.18f) || Triangle(x - 0.4f, y + 0.38f, 0.02f, 0.16f, 0.18f));
            var guardRecoveryHalo = routeStyle == 3 && variantStyle == 3 && (Ellipse(x, y - 0.72f, 0.34f + stage * 0.035f, 0.08f) && !Ellipse(x, y - 0.72f, 0.24f + stage * 0.02f, 0.045f) || Mathf.Abs(x) < 0.04f && y > -0.83f && y < -0.61f || Mathf.Abs(y + 0.72f) < 0.04f && Mathf.Abs(x) < 0.13f);
            var guardMirrorShield = routeStyle == 3 && variantStyle == 4 && (Hex(x + 0.58f, y + 0.06f, 0.25f + stage * 0.025f) || Hex(x - 0.58f, y + 0.06f, 0.25f + stage * 0.025f));

            var novaGear = fusedStyle == 1 || fusedStyle == 4;
            var bulwarkGear = fusedStyle == 1 || fusedStyle == 3 || fusedStyle == 5;
            var siphonGear = fusedStyle == 2 || fusedStyle == 3 || fusedStyle == 6;
            var phaseGear = fusedStyle == 4 || fusedStyle == 5 || fusedStyle == 6;
            var novaCannon = novaGear && (Ellipse(x + 0.72f, y - 0.06f, 0.105f, 0.3f) || Ellipse(x - 0.72f, y - 0.06f, 0.105f, 0.3f));
            var bulwarkPlate = bulwarkGear && (Hex(x + 0.54f, y + 0.1f, 0.24f) || Hex(x - 0.54f, y + 0.1f, 0.24f));
            var siphonOrb = siphonGear && (Ellipse(x + 0.58f, y + 0.36f, 0.105f, 0.105f) || Ellipse(x - 0.58f, y + 0.36f, 0.105f, 0.105f) || Ellipse(x, y + 0.56f, 0.095f, 0.095f));
            var phaseRing = phaseGear && (Ellipse(x, y + 0.02f, 0.84f, 0.74f) && !Ellipse(x, y + 0.02f, 0.72f, 0.62f));
            var fusionCrest = fusedStyle > 0 && Triangle(x, y - 0.9f, 0f, -0.33f, 0.28f + stage * 0.025f);
            var fusionCore = fusedStyle > 0 && Mathf.Abs(x) + Mathf.Abs(y - 0.08f) < 0.16f + stage * 0.012f;

            var eye = Ellipse(x + 0.12f, y - 0.48f, owl ? 0.075f : 0.055f, owl ? 0.08f : 0.065f) || Ellipse(x - 0.12f, y - 0.48f, owl ? 0.075f : 0.055f, owl ? 0.08f : 0.065f);
            var eyeSpark = Ellipse(x + 0.095f, y - 0.515f, 0.017f, 0.019f) || Ellipse(x - 0.145f, y - 0.515f, 0.017f, 0.019f);
            var cheek = Ellipse(x + 0.23f, y - 0.38f, 0.045f, 0.03f) || Ellipse(x - 0.23f, y - 0.38f, 0.045f, 0.03f);
            var muzzle = !owl && Ellipse(x, y - 0.34f, guard ? 0.16f : ironBear ? 0.17f : 0.14f, ironBear ? 0.085f : 0.065f);

            var inner = body || head || dogEar || kitEar || catEar || bearEar || owlHorn || moleSnout || tail || twinTail || owlWing || moleClaw || bearClaw || ironPlate || paws
                || routeWing || powerHorn || guardShell || stageCrest || chestGem || visor || echoWave || sparkBolt || driftSlash || guardRivet
                || speedRapidWings || speedLaserFin || speedMirageAfterimage || speedHomingPods
                || powerExplosiveCore || powerGiantArms || powerBreakerHorns || powerSiegePlates
                || guardCoreShield || guardCounterSpikes || guardRecoveryHalo || guardMirrorShield
                || novaCannon || bulwarkPlate || siphonOrb || phaseRing || fusionCrest || fusionCore || eye || eyeSpark || cheek || muzzle;

            var outer = Ellipse(x, y + 0.08f, bodyRx + 0.06f, bodyRy + 0.065f)
                || Ellipse(x, y - 0.42f, headRx + 0.06f, headRy + 0.06f)
                || (cobalt && (Ellipse(x + 0.34f, earY, 0.14f, 0.32f) || Ellipse(x - 0.34f, earY, 0.14f, 0.32f)))
                || ((spark || drift) && (Triangle(x + 0.26f, y - 0.66f, -0.03f, -0.39f, 0.23f) || Triangle(x - 0.26f, y - 0.66f, 0.03f, -0.39f, 0.23f)))
                || (hexCat && (Triangle(x + 0.24f, y - 0.64f, -0.04f, -0.33f, 0.22f) || Triangle(x - 0.24f, y - 0.64f, 0.04f, -0.33f, 0.22f)))
                || (ironBear && (Ellipse(x + 0.25f, y - 0.63f, 0.16f, 0.17f) || Ellipse(x - 0.25f, y - 0.63f, 0.16f, 0.17f)))
                || (guard && Ellipse(x, y - 0.5f, 0.22f, 0.12f))
                || (!guard && !ironBear && Ellipse(x + 0.42f + stage * 0.04f, y + 0.16f, (drift ? 0.17f : 0.21f) + stage * 0.035f, drift ? 0.21f : 0.14f))
                || ((spark || echo || drift && stage >= 1) && (Ellipse(x + 0.5f, y + 0.3f, 0.16f + stage * 0.03f, 0.12f) || Ellipse(x + 0.47f, y + 0.02f, 0.16f + stage * 0.03f, 0.12f)))
                || (hexCat && (Ellipse(x + 0.48f, y + 0.04f, 0.17f + stage * 0.02f, 0.36f + stage * 0.04f) || Ellipse(x - 0.48f, y + 0.04f, 0.17f + stage * 0.02f, 0.36f + stage * 0.04f)))
                || (guard && (Ellipse(x + 0.34f, y + 0.34f, 0.19f + stage * 0.02f, 0.12f) || Ellipse(x - 0.34f, y + 0.34f, 0.19f + stage * 0.02f, 0.12f)))
                || (ironBear && (Ellipse(x + 0.39f, y + 0.28f, 0.24f + stage * 0.03f, 0.16f) || Ellipse(x - 0.39f, y + 0.28f, 0.24f + stage * 0.03f, 0.16f) || Hex(x, y + 0.04f, 0.32f + stage * 0.025f)))
                || Ellipse(x + 0.22f, y + 0.43f, 0.13f + stage * 0.012f, 0.1f)
                || Ellipse(x - 0.22f, y + 0.43f, 0.13f + stage * 0.012f, 0.1f)
                || ((routeStyle == 1 || fusedStyle == 1 || fusedStyle == 4) && (Ellipse(x + 0.6f, y + 0.03f, 0.18f + stage * 0.025f, 0.4f + stage * 0.055f) || Ellipse(x - 0.6f, y + 0.03f, 0.18f + stage * 0.025f, 0.4f + stage * 0.055f)))
                || ((routeStyle == 2 || fusedStyle == 1) && (Triangle(x + 0.16f, y - 0.77f, 0f, -0.34f, 0.25f + stage * 0.025f) || Triangle(x - 0.16f, y - 0.77f, 0f, -0.34f, 0.25f + stage * 0.025f)))
                || ((routeStyle == 3 || fusedStyle == 3 || fusedStyle == 5) && Ellipse(x, y + 0.03f, 0.34f + stage * 0.035f, 0.42f + stage * 0.025f))
                || (stage >= 2 && Triangle(x, y - 0.82f, 0f, -0.31f - stage * 0.03f, 0.29f + stage * 0.03f))
                || (echo && (Ellipse(x, y + 0.02f, 0.76f, 0.64f) && !Ellipse(x, y + 0.02f, 0.55f, 0.44f)))
                || (spark && (Mathf.Abs(x + 0.04f) + Mathf.Abs(y + 0.02f) < 0.16f || Triangle(x, y - 0.04f, 0.08f, -0.08f, 0.24f)))
                || (drift && (Mathf.Abs(x + y * 0.55f) < 0.06f && y > -0.16f && y < 0.46f || Mathf.Abs(x + y * 0.55f - 0.18f) < 0.05f && y > -0.06f && y < 0.42f))
                || (guard && (Ellipse(x + 0.18f, y - 0.02f, 0.06f, 0.06f) || Ellipse(x - 0.18f, y - 0.02f, 0.06f, 0.06f)))
                || (routeStyle == 1 && variantStyle == 1 && (Ellipse(x + 0.72f, y + 0.0f, 0.14f + stage * 0.018f, 0.52f + stage * 0.05f) || Ellipse(x - 0.72f, y + 0.0f, 0.14f + stage * 0.018f, 0.52f + stage * 0.05f)))
                || (routeStyle == 1 && variantStyle == 2 && (Mathf.Abs(x) < 0.075f && y < -0.61f && y > -0.96f || Mathf.Abs(x + 0.38f) < 0.065f && y < -0.15f && y > -0.61f || Mathf.Abs(x - 0.38f) < 0.065f && y < -0.15f && y > -0.61f))
                || (routeStyle == 1 && variantStyle == 3 && (Ellipse(x + 0.42f, y + 0.08f, 0.27f, 0.48f) && !Ellipse(x + 0.42f, y + 0.08f, 0.12f, 0.28f) || Ellipse(x - 0.42f, y + 0.08f, 0.27f, 0.48f) && !Ellipse(x - 0.42f, y + 0.08f, 0.12f, 0.28f)))
                || (routeStyle == 1 && variantStyle == 4 && (Ellipse(x + 0.66f, y - 0.34f, 0.15f, 0.15f) || Ellipse(x - 0.66f, y - 0.34f, 0.15f, 0.15f) || Ellipse(x + 0.58f, y + 0.34f, 0.14f, 0.14f) || Ellipse(x - 0.58f, y + 0.34f, 0.14f, 0.14f)))
                || (routeStyle == 2 && variantStyle == 1 && (Ellipse(x, y + 0.1f, 0.27f + stage * 0.018f, 0.27f + stage * 0.018f) || Mathf.Abs(x) + Mathf.Abs(y - 0.1f) < 0.31f + stage * 0.018f))
                || (routeStyle == 2 && variantStyle == 2 && (Ellipse(x + 0.56f, y + 0.13f, 0.26f + stage * 0.04f, 0.38f + stage * 0.035f) || Ellipse(x - 0.56f, y + 0.13f, 0.26f + stage * 0.04f, 0.38f + stage * 0.035f)))
                || (routeStyle == 2 && variantStyle == 3 && (Triangle(x + 0.26f, y - 0.84f, 0f, -0.39f, 0.37f + stage * 0.03f) || Triangle(x - 0.26f, y - 0.84f, 0f, -0.39f, 0.37f + stage * 0.03f)))
                || (routeStyle == 2 && variantStyle == 4 && (Hex(x + 0.44f, y + 0.0f, 0.28f + stage * 0.025f) || Hex(x - 0.44f, y + 0.0f, 0.28f + stage * 0.025f) || Mathf.Abs(y + 0.66f) < 0.07f && Mathf.Abs(x) < 0.38f))
                || (routeStyle == 3 && variantStyle == 1 && Ellipse(x, y + 0.1f, 0.58f + stage * 0.035f, 0.54f + stage * 0.03f))
                || (routeStyle == 3 && variantStyle == 2 && (Triangle(x + 0.58f, y - 0.2f, -0.02f, -0.01f, 0.27f + stage * 0.025f) || Triangle(x - 0.58f, y - 0.2f, 0.02f, -0.01f, 0.27f + stage * 0.025f) || Triangle(x + 0.4f, y + 0.38f, -0.02f, 0.12f, 0.24f) || Triangle(x - 0.4f, y + 0.38f, 0.02f, 0.12f, 0.24f)))
                || (routeStyle == 3 && variantStyle == 3 && (Ellipse(x, y - 0.72f, 0.4f + stage * 0.035f, 0.11f) || Mathf.Abs(x) < 0.07f && y > -0.86f && y < -0.58f || Mathf.Abs(y + 0.72f) < 0.07f && Mathf.Abs(x) < 0.17f))
                || (routeStyle == 3 && variantStyle == 4 && (Hex(x + 0.58f, y + 0.06f, 0.31f + stage * 0.025f) || Hex(x - 0.58f, y + 0.06f, 0.31f + stage * 0.025f)))
                || (novaGear && (Ellipse(x + 0.72f, y - 0.06f, 0.15f, 0.36f) || Ellipse(x - 0.72f, y - 0.06f, 0.15f, 0.36f)))
                || (bulwarkGear && (Hex(x + 0.54f, y + 0.1f, 0.3f) || Hex(x - 0.54f, y + 0.1f, 0.3f)))
                || (siphonGear && (Ellipse(x + 0.58f, y + 0.36f, 0.14f, 0.14f) || Ellipse(x - 0.58f, y + 0.36f, 0.14f, 0.14f) || Ellipse(x, y + 0.56f, 0.13f, 0.13f)))
                || (phaseGear && (Ellipse(x, y + 0.02f, 0.9f, 0.8f) && !Ellipse(x, y + 0.02f, 0.66f, 0.56f)))
                || (fusedStyle > 0 && Triangle(x, y - 0.9f, 0f, -0.38f, 0.34f + stage * 0.025f))
                || (fusedStyle > 0 && Mathf.Abs(x) + Mathf.Abs(y - 0.08f) < 0.21f + stage * 0.012f);

            if (outer && !inner)
                return outline;
            if (eyeSpark)
                return Color.white;
            if (eye)
                return owl ? accent : outline;
            if (phaseRing || echoWave || driftSlash || speedMirageAfterimage)
                return WithAlpha(accent, 0.72f);
            if (novaCannon || siphonOrb || stageCrest || fusionCrest || fusionCore || sparkBolt || guardRivet || bearClaw || speedLaserFin || speedHomingPods || powerExplosiveCore || powerBreakerHorns || guardCounterSpikes || guardRecoveryHalo)
                return accent;
            if (bulwarkPlate || guardShell || visor || ironPlate || powerGiantArms || powerSiegePlates || guardCoreShield || guardMirrorShield)
                return armor;
            if (routeWing || owlWing || speedRapidWings)
                return Color.Lerp(baseColor, accent, routeStyle == 1 || fusedStyle == 1 || fusedStyle == 4 ? 0.65f : 0.4f);
            if (powerHorn || kitEar || catEar || owlHorn || moleClaw || paws)
                return accent;
            if (tail || twinTail)
                return Color.Lerp(baseColor, accent, spark || echo || drift ? 0.46f : 0.32f);
            if (chestGem)
                return fusedStyle > 0 ? accent : new Color(0.93f, 1f, 1f, 1f);
            if (cheek || muzzle || moleSnout)
                return Color.Lerp(baseColor, Color.white, 0.36f);
            if (body || head || dogEar || bearEar)
                return baseColor;
            return Color.clear;
        });
    }

    Color GetPartnerBaseColor(int species)
    {
        switch (species)
        {
            case 2:
                return new Color(1f, 0.62f, 0.18f, 1f);
            case 3:
                return new Color(0.28f, 0.78f, 0.46f, 1f);
            case 4:
                return new Color(0.62f, 0.34f, 1f, 1f);
            case 5:
                return new Color(0.95f, 0.42f, 0.72f, 1f);
            case 6:
                return new Color(0.24f, 0.28f, 0.34f, 1f);
            case 7:  // Wraith Lynx  Emelee (sharp white/silver)
                return new Color(0.78f, 0.85f, 0.95f, 1f);
            case 8:  // Genesis Core  Ehidden all-link (golden cyan blend)
                return new Color(0.40f, 0.85f, 0.95f, 1f);
            case 9:  // Halo Caster  Efunnel (deep navy w/ cyan halo)
                return new Color(0.22f, 0.40f, 0.72f, 1f);
            case 10: // Pulse Hydra  Elaser (deep crimson/magenta)
                return new Color(0.62f, 0.18f, 0.46f, 1f);
            case 11: // Solar Anchor  Ecore resonance (gold-cyan)
                return new Color(0.42f, 0.34f, 0.12f, 1f);
            default:
                return new Color(0.3f, 0.9f, 1f, 1f);
        }
    }

    Color GetPartnerDeepColor(int species)
    {
        switch (species)
        {
            case 2:
                return new Color(0.9f, 0.2f, 0.06f, 1f);
            case 3:
                return new Color(0.08f, 0.34f, 0.18f, 1f);
            case 4:
                return new Color(0.22f, 0.08f, 0.46f, 1f);
            case 5:
                return new Color(0.46f, 0.08f, 0.32f, 1f);
            case 6:
                return new Color(0.05f, 0.06f, 0.09f, 1f);
            case 7:
                return new Color(0.12f, 0.14f, 0.20f, 1f);
            case 8:
                return new Color(0.10f, 0.18f, 0.32f, 1f);
            case 9:
                return new Color(0.06f, 0.14f, 0.32f, 1f);
            case 10:
                return new Color(0.30f, 0.04f, 0.18f, 1f);
            case 11: // Solar Anchor  Ewarm gold deep
                return new Color(0.30f, 0.20f, 0.04f, 1f);
            default:
                return new Color(0.08f, 0.32f, 0.9f, 1f);
        }
    }

    Color GetPartnerAccentColor(int species)
    {
        switch (species)
        {
            case 2:
                return new Color(1f, 0.82f, 0.22f, 1f);
            case 3:
                return new Color(0.62f, 1f, 0.48f, 1f);
            case 4:
                return new Color(0.9f, 0.48f, 1f, 1f);
            case 5:
                return new Color(1f, 0.52f, 0.82f, 1f);
            case 6:
                return new Color(1f, 0.82f, 0.24f, 1f);
            case 7:  // bright silver-cyan
                return new Color(0.78f, 0.95f, 1f, 1f);
            case 8:  // gold-cyan prismatic
                return new Color(1f, 0.95f, 0.32f, 1f);
            case 9:  // halo cyan (Halo Caster)
                return new Color(0.55f, 0.92f, 1f, 1f);
            case 10: // beam pink/magenta (Pulse Hydra)
                return new Color(1f, 0.45f, 0.85f, 1f);
            case 11: // bright gold (Solar Anchor)
                return new Color(1f, 0.86f, 0.28f, 1f);
            default:
                return new Color(0.45f, 1f, 1f, 1f);
        }
    }

    Color GetPartnerArmorColor(int species)
    {
        switch (species)
        {
            case 2:
                return new Color(0.42f, 0.12f, 0.05f, 1f);
            case 3:
                return new Color(0.08f, 0.24f, 0.16f, 1f);
            case 4:
                return new Color(0.16f, 0.08f, 0.28f, 1f);
            case 5:
                return new Color(0.28f, 0.06f, 0.2f, 1f);
            case 6:
                return new Color(0.08f, 0.09f, 0.12f, 1f);
            case 7:
                return new Color(0.06f, 0.08f, 0.12f, 1f);
            case 8:
                return new Color(0.05f, 0.12f, 0.18f, 1f);
            case 9:
                return new Color(0.04f, 0.08f, 0.18f, 1f);
            case 10:
                return new Color(0.16f, 0.03f, 0.10f, 1f);
            case 11: // Solar Anchor  Eburnt amber armor
                return new Color(0.18f, 0.12f, 0.04f, 1f);
            default:
                return new Color(0.08f, 0.22f, 0.32f, 1f);
        }
    }

    Color GetFusionAccentColor(int style)
    {
        switch (style)
        {
            case 1:
                return new Color(1f, 0.36f, 0.95f, 1f);
            case 2:
                return new Color(0.7f, 1f, 0.26f, 1f);
            case 3:
                return new Color(1f, 0.82f, 0.28f, 1f);
            case 4:
                return new Color(0.45f, 0.78f, 1f, 1f);
            case 5:
                return new Color(0.82f, 0.55f, 1f, 1f);
            case 6:
                return new Color(0.55f, 1f, 0.92f, 1f);
            default:
                return new Color(0.45f, 1f, 1f, 1f);
        }
    }

    Sprite MakeDataEggSprite()
    {
        return MakeColorSprite(96, (x, y) =>
        {
            var body = Ellipse(x, y, 0.44f, 0.58f);
            var border = Ellipse(x, y, 0.5f, 0.64f) && !body;
            if (border)
                return new Color(1f, 0.78f, 0.18f, 1f);
            if (!body)
                return Color.clear;
            var gem = Mathf.Abs(x) + Mathf.Abs(y) < 0.12f;
            var midRing = Ellipse(x, y, 0.36f, 0.47f) && !Ellipse(x, y, 0.27f, 0.36f);
            var cross = Mathf.Abs(x) < 0.065f || Mathf.Abs(y) < 0.055f;
            if (gem)
                return new Color(0.88f, 1f, 1f, 1f);
            if (cross)
                return new Color(1f, 0.92f, 0.38f, 1f);
            if (midRing)
                return new Color(0.52f, 1f, 1f, 0.92f);
            var d = Mathf.Clamp01(1f - x * x * 2.5f - y * y * 1.5f);
            return Color.Lerp(new Color(0.06f, 0.78f, 0.95f, 1f), new Color(0.24f, 1f, 1f, 1f), d * 0.65f);
        });
    }

    Sprite MakeLinkSprite(int type)
    {
        return MakeColorSprite(112, (x, y) =>
        {
            var outline = new Color(0.015f, 0.025f, 0.05f, 1f);
            if (type == 0)
            {
                var body = Ellipse(x, y + 0.1f, 0.26f, 0.32f);
                var head = Ellipse(x, y - 0.3f, 0.24f, 0.18f);
                var wing = Ellipse(x + 0.43f, y + 0.02f, 0.16f, 0.34f) || Ellipse(x - 0.43f, y + 0.02f, 0.16f, 0.34f);
                var ear = Triangle(x + 0.19f, y - 0.47f, 0f, -0.18f, 0.18f) || Triangle(x - 0.19f, y - 0.47f, 0f, -0.18f, 0.18f);
                var tail = Triangle(x, y + 0.53f, 0f, -0.2f, 0.26f);
                var chest = Mathf.Abs(x) + Mathf.Abs(y + 0.01f) < 0.13f;
                var eye = Ellipse(x + 0.08f, y - 0.34f, 0.032f, 0.038f) || Ellipse(x - 0.08f, y - 0.34f, 0.032f, 0.038f);
                var spark = Ellipse(x + 0.064f, y - 0.36f, 0.012f, 0.014f) || Ellipse(x - 0.096f, y - 0.36f, 0.012f, 0.014f);
                var inner = body || head || wing || ear || tail || chest || eye || spark;
                var outer = Ellipse(x, y + 0.1f, 0.32f, 0.38f)
                    || Ellipse(x, y - 0.3f, 0.3f, 0.24f)
                    || Ellipse(x + 0.43f, y + 0.02f, 0.21f, 0.4f)
                    || Ellipse(x - 0.43f, y + 0.02f, 0.21f, 0.4f)
                    || Triangle(x + 0.19f, y - 0.47f, 0f, -0.22f, 0.23f)
                    || Triangle(x - 0.19f, y - 0.47f, 0f, -0.22f, 0.23f)
                    || Triangle(x, y + 0.53f, 0f, -0.25f, 0.32f);
                if (outer && !inner)
                    return outline;
                if (spark)
                    return Color.white;
                if (eye)
                    return outline;
                if (chest || tail)
                    return new Color(0.9f, 1f, 0.25f, 1f);
                if (wing || ear)
                    return new Color(0.32f, 0.95f, 1f, 1f);
                if (body || head)
                    return new Color(0.12f, 0.45f, 1f, 1f);
            }
            else if (type == 1)
            {
                var body = Ellipse(x, y + 0.08f, 0.33f, 0.32f);
                var shell = Hex(x, y - 0.02f, 0.43f);
                var head = Ellipse(x, y - 0.42f, 0.22f, 0.16f);
                var arm = Ellipse(x + 0.43f, y + 0.05f, 0.11f, 0.19f) || Ellipse(x - 0.43f, y + 0.05f, 0.11f, 0.19f);
                var horn = Triangle(x, y - 0.58f, 0f, -0.2f, 0.16f);
                var plate = Mathf.Abs(x) < 0.055f && y > -0.2f && y < 0.24f || Mathf.Abs(y + 0.01f) < 0.045f && Mathf.Abs(x) < 0.28f;
                var eye = Ellipse(x + 0.075f, y - 0.44f, 0.026f, 0.032f) || Ellipse(x - 0.075f, y - 0.44f, 0.026f, 0.032f);
                var inner = body || shell || head || arm || horn || plate || eye;
                var outer = Ellipse(x, y + 0.08f, 0.39f, 0.38f)
                    || Hex(x, y - 0.02f, 0.51f)
                    || Ellipse(x, y - 0.42f, 0.28f, 0.22f)
                    || Ellipse(x + 0.43f, y + 0.05f, 0.15f, 0.24f)
                    || Ellipse(x - 0.43f, y + 0.05f, 0.15f, 0.24f)
                    || Triangle(x, y - 0.58f, 0f, -0.24f, 0.21f);
                if (outer && !inner)
                    return outline;
                if (eye)
                    return outline;
                if (plate || horn)
                    return new Color(0.9f, 1f, 0.7f, 1f);
                if (shell || arm)
                    return new Color(0.18f, 0.88f, 0.48f, 1f);
                if (body || head)
                    return new Color(0.08f, 0.28f, 0.18f, 1f);
            }
            else if (type == 2)
            {
                var body = Ellipse(x, y + 0.07f, 0.25f, 0.36f);
                var head = Ellipse(x, y - 0.35f, 0.22f, 0.17f);
                var ear = Ellipse(x + 0.3f, y - 0.37f, 0.1f, 0.23f) || Ellipse(x - 0.3f, y - 0.37f, 0.1f, 0.23f);
                var antenna = Triangle(x + 0.13f, y - 0.52f, 0f, -0.16f, 0.12f) || Triangle(x - 0.13f, y - 0.52f, 0f, -0.16f, 0.12f);
                var tail = Ellipse(x + 0.37f, y + 0.2f, 0.14f, 0.09f);
                var gem = Mathf.Abs(x) + Mathf.Abs(y + 0.01f) < 0.14f;
                var foot = Ellipse(x + 0.16f, y + 0.42f, 0.08f, 0.055f) || Ellipse(x - 0.16f, y + 0.42f, 0.08f, 0.055f);
                var eye = Ellipse(x + 0.075f, y - 0.38f, 0.025f, 0.03f) || Ellipse(x - 0.075f, y - 0.38f, 0.025f, 0.03f);
                var inner = body || head || ear || antenna || tail || gem || foot || eye;
                var outer = Ellipse(x, y + 0.07f, 0.31f, 0.42f)
                    || Ellipse(x, y - 0.35f, 0.28f, 0.23f)
                    || Ellipse(x + 0.3f, y - 0.37f, 0.14f, 0.28f)
                    || Ellipse(x - 0.3f, y - 0.37f, 0.14f, 0.28f)
                    || Triangle(x + 0.13f, y - 0.52f, 0f, -0.2f, 0.16f)
                    || Triangle(x - 0.13f, y - 0.52f, 0f, -0.2f, 0.16f)
                    || Ellipse(x + 0.37f, y + 0.2f, 0.18f, 0.13f)
                    || Ellipse(x + 0.16f, y + 0.42f, 0.11f, 0.08f)
                    || Ellipse(x - 0.16f, y + 0.42f, 0.11f, 0.08f);
                if (outer && !inner)
                    return outline;
                if (eye)
                    return outline;
                if (gem)
                    return new Color(0.92f, 1f, 0.35f, 1f);
                if (antenna || tail || foot)
                    return new Color(0.45f, 1f, 0.42f, 1f);
                if (ear)
                    return new Color(1f, 0.92f, 0.28f, 1f);
                if (body || head)
                    return new Color(0.1f, 0.72f, 0.52f, 1f);
            }
            else
            {
                var body = Ellipse(x, y + 0.04f, 0.26f, 0.34f);
                var head = Ellipse(x, y - 0.36f, 0.2f, 0.16f);
                var ear = Triangle(x + 0.18f, y - 0.5f, 0f, -0.22f, 0.14f) || Triangle(x - 0.18f, y - 0.5f, 0f, -0.22f, 0.14f);
                var wisp1 = Ellipse(x + 0.32f, y + 0.18f, 0.1f, 0.22f);
                var wisp2 = Ellipse(x - 0.32f, y + 0.22f, 0.1f, 0.18f);
                var wisp3 = Ellipse(x + 0.04f, y + 0.42f, 0.18f, 0.1f);
                var diamond = Mathf.Abs(x) + Mathf.Abs(y + 0.02f) < 0.13f;
                var echo = Ellipse(x + 0.42f, y - 0.1f, 0.08f, 0.18f) || Ellipse(x - 0.42f, y - 0.1f, 0.08f, 0.18f);
                var eye = Ellipse(x + 0.07f, y - 0.38f, 0.024f, 0.03f) || Ellipse(x - 0.07f, y - 0.38f, 0.024f, 0.03f);
                var inner = body || head || ear || wisp1 || wisp2 || wisp3 || diamond || echo || eye;
                var outer = Ellipse(x, y + 0.04f, 0.32f, 0.4f)
                    || Ellipse(x, y - 0.36f, 0.26f, 0.22f)
                    || Triangle(x + 0.18f, y - 0.5f, 0f, -0.26f, 0.18f)
                    || Triangle(x - 0.18f, y - 0.5f, 0f, -0.26f, 0.18f)
                    || Ellipse(x + 0.32f, y + 0.18f, 0.14f, 0.26f)
                    || Ellipse(x - 0.32f, y + 0.22f, 0.14f, 0.22f)
                    || Ellipse(x + 0.04f, y + 0.42f, 0.22f, 0.14f)
                    || Ellipse(x + 0.42f, y - 0.1f, 0.12f, 0.22f)
                    || Ellipse(x - 0.42f, y - 0.1f, 0.12f, 0.22f);
                if (outer && !inner)
                    return outline;
                if (eye)
                    return outline;
                if (diamond)
                    return new Color(1f, 0.92f, 1f, 1f);
                if (echo)
                    return new Color(0.58f, 0.36f, 1f, 0.55f);
                if (wisp1 || wisp2 || wisp3)
                    return new Color(0.78f, 0.5f, 1f, 0.78f);
                if (ear)
                    return new Color(1f, 0.42f, 0.95f, 1f);
                if (body || head)
                    return new Color(0.32f, 0.18f, 0.62f, 1f);
            }

            return Color.clear;
        });
    }

    Sprite MakeRunnerSprite()
    {
        return MakeColorSprite(72, (x, y) =>
        {
            var body = Ellipse(x, y, 0.38f, 0.28f);
            var leg = Mathf.Abs(y) < 0.16f && (Mathf.Abs(x - 0.48f) < 0.08f || Mathf.Abs(x + 0.48f) < 0.08f);
            var streak = Ellipse(x + 0.56f, y + 0.04f, 0.19f, 0.085f) || Ellipse(x - 0.56f, y + 0.04f, 0.19f, 0.085f);
            var outer = Ellipse(x, y, 0.45f, 0.35f) || Mathf.Abs(y) < 0.2f && (Mathf.Abs(x - 0.48f) < 0.11f || Mathf.Abs(x + 0.48f) < 0.11f);
            if (streak && !body && !outer)
                return new Color(1f, 0.34f, 0.42f, 0.72f);
            if (outer && !(body || leg))
                return new Color(0.06f, 0.02f, 0.04f, 1f);
            if (body)
            {
                var d = Mathf.Clamp01(1f - x * x * 2.4f - y * y * 5f);
                return Color.Lerp(new Color(1f, 0.16f, 0.22f, 1f), new Color(1f, 0.62f, 0.52f, 1f), d * 0.52f);
            }
            if (leg)
                return new Color(0.95f, 0.08f, 0.16f, 1f);
            if (Mathf.Abs(x) < 0.08f && Mathf.Abs(y) < 0.08f)
                return new Color(1f, 0.92f, 0.24f, 1f);
            return Color.clear;
        });
    }

    Sprite MakeBruteSprite()
    {
        return MakeColorSprite(88, (x, y) =>
        {
            var shell = Ellipse(x, y, 0.52f, 0.48f);
            var ring = shell && Ellipse(x, y, 0.38f, 0.35f) && !Ellipse(x, y, 0.28f, 0.25f);
            var horn = Triangle(x, y - 0.58f, 0f, -0.24f, 0.26f);
            var hornOuter = Triangle(x, y - 0.58f, 0f, -0.28f, 0.31f);
            if ((Ellipse(x, y, 0.59f, 0.55f) || hornOuter) && !(shell || horn))
                return new Color(0.05f, 0.02f, 0.09f, 1f);
            if (horn)
                return new Color(0.16f, 1f, 0.94f, 1f);
            if (ring)
                return new Color(1f, 0.14f, 0.9f, 1f);
            if (shell)
            {
                var diagLine = Mathf.Abs(x + y) < 0.042f || Mathf.Abs(x - y) < 0.042f;
                if (diagLine)
                    return new Color(0.78f, 0.26f, 1f, 0.9f);
                var d = Mathf.Clamp01(1f - x * x * 1.8f - y * y * 1.8f);
                return Color.Lerp(new Color(0.52f, 0.16f, 0.92f, 1f), new Color(0.7f, 0.32f, 1f, 1f), d * 0.48f);
            }
            return Color.clear;
        });
    }

    Sprite MakeShooterSprite()
    {
        return MakeColorSprite(80, (x, y) =>
        {
            var core = Hex(x, y, 0.48f);
            var cannon = (x > 0.44f && x < 0.68f || x < -0.44f && x > -0.68f) && Mathf.Abs(y) < 0.09f;
            var cannonOuter = (x > 0.42f && x < 0.72f || x < -0.42f && x > -0.72f) && Mathf.Abs(y) < 0.13f;
            if ((Hex(x, y, 0.55f) || cannonOuter) && !(core || cannon))
                return new Color(0.05f, 0.04f, 0.01f, 1f);
            if (cannon)
                return new Color(1f, 0.78f, 0.12f, 0.92f);
            if (core)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) < 0.18f)
                    return new Color(0.2f, 1f, 0.92f, 1f);
                var d = Mathf.Clamp01(1f - (x * x + y * y) / 0.24f);
                return Color.Lerp(new Color(1f, 0.86f, 0.12f, 1f), new Color(1f, 1f, 0.7f, 1f), d * 0.52f);
            }
            return Color.clear;
        });
    }

    // ── New enemy sprites ──────────────────────────────────────────────
    Sprite MakeDasherSprite()
    {
        // Sleek arrowhead  Ecyan, wide base at bottom (y=-0.42), tip at top (y=+0.30)
        // Triangle(x,y, cx,cy, size): base at y=cy, tip at y=cy+size
        return MakeColorSprite(72, (x, y) =>
        {
            var body  = Triangle(x, y, 0f, -0.42f, 0.72f);   // upward-pointing arrow
            var tail  = Ellipse(x, y + 0.30f, 0.26f, 0.14f); // speed-trail oval at y=-0.30
            var slash = Mathf.Abs(x - y * 0.35f) < 0.052f && body;
            var outer = Triangle(x, y, 0f, -0.48f, 0.80f) || Ellipse(x, y + 0.30f, 0.32f, 0.20f);
            if (outer && !(body || tail))
                return new Color(0.02f, 0.06f, 0.09f, 1f);
            if (slash)
                return new Color(0.70f, 1f, 1f, 0.88f);
            if (body)
            {
                var d = Mathf.Clamp01(1f - x * x * 4.5f - (y - 0.05f) * (y - 0.05f) * 1.8f);
                return Color.Lerp(new Color(0.08f, 0.78f, 1f, 1f), new Color(0.52f, 1f, 1f, 1f), d * 0.55f);
            }
            if (tail)
                return new Color(0.06f, 0.52f, 0.82f, 0.72f);
            if (Mathf.Abs(x) < 0.05f && Mathf.Abs(y - 0.14f) < 0.05f)
                return new Color(1f, 1f, 0.9f, 1f);
            return Color.clear;
        });
    }

    Sprite MakeBomberSprite()
    {
        // Round bomb shape  Eorange/yellow, bulging centre, warning stripes
        return MakeColorSprite(80, (x, y) =>
        {
            var core = Ellipse(x, y, 0.46f, 0.44f);
            var stripe = core && (Mathf.Abs(x * 0.7f - y * 0.7f) < 0.048f || Mathf.Abs(x * 0.7f + y * 0.7f) < 0.048f);
            var fuse = x > 0.32f && x < 0.52f && Mathf.Abs(y - 0.34f) < 0.06f;
            var outer = Ellipse(x, y, 0.53f, 0.51f) || (x > 0.30f && x < 0.54f && Mathf.Abs(y - 0.34f) < 0.10f);
            if (outer && !(core || fuse))
                return new Color(0.06f, 0.04f, 0.01f, 1f);
            if (fuse)
                return new Color(0.22f, 0.82f, 0.22f, 1f);
            if (stripe)
                return new Color(0.12f, 0.06f, 0.01f, 0.72f);
            if (core)
            {
                var d = Mathf.Clamp01(1f - (x * x + y * y) * 2.1f);
                return Color.Lerp(new Color(1f, 0.44f, 0.06f, 1f), new Color(1f, 0.82f, 0.28f, 1f), d * 0.55f);
            }
            return Color.clear;
        });
    }

    Sprite MakePhantomSprite()
    {
        // Wisp / ghost shape  Edark violet, asymmetric flowing form
        return MakeColorSprite(76, (x, y) =>
        {
            var body = Ellipse(x, y, 0.34f, 0.44f);
            var tendril = (Ellipse(x + 0.22f, y + 0.42f, 0.12f, 0.22f) || Ellipse(x - 0.18f, y + 0.38f, 0.10f, 0.18f));
            var eyeL = Ellipse(x + 0.12f, y - 0.14f, 0.07f, 0.055f);
            var eyeR = Ellipse(x - 0.12f, y - 0.14f, 0.07f, 0.055f);
            var outerBody = Ellipse(x, y, 0.41f, 0.51f) || Ellipse(x + 0.22f, y + 0.42f, 0.17f, 0.27f) || Ellipse(x - 0.18f, y + 0.38f, 0.15f, 0.23f);
            if (outerBody && !(body || tendril))
                return new Color(0.04f, 0.01f, 0.08f, 1f);
            if (eyeL || eyeR)
                return new Color(0.82f, 0.56f, 1f, 1f);
            if (body || tendril)
            {
                var d = Mathf.Clamp01(1f - (x * x + y * y) * 2.5f);
                var a = (body ? 0.82f : 0.58f);
                return new Color(
                    Color.Lerp(new Color(0.38f, 0.08f, 0.72f), new Color(0.62f, 0.28f, 1f), d * 0.5f).r,
                    Color.Lerp(new Color(0.38f, 0.08f, 0.72f), new Color(0.62f, 0.28f, 1f), d * 0.5f).g,
                    Color.Lerp(new Color(0.38f, 0.08f, 0.72f), new Color(0.62f, 0.28f, 1f), d * 0.5f).b,
                    a);
            }
            return Color.clear;
        });
    }

    Sprite MakeLavaCrawlerSprite()
    {
        // Stage 2 exclusive: squat heat-armored crawler with a glowing furnace core.
        return MakeColorSprite(76, (x, y) =>
        {
            var shell = Ellipse(x, y + 0.02f, 0.46f, 0.34f);
            var head = Ellipse(x, y - 0.36f, 0.28f, 0.18f);
            var legL1 = Ellipse(x + 0.43f, y + 0.1f, 0.14f, 0.08f);
            var legL2 = Ellipse(x + 0.42f, y - 0.12f, 0.13f, 0.07f);
            var legR1 = Ellipse(x - 0.43f, y + 0.1f, 0.14f, 0.08f);
            var legR2 = Ellipse(x - 0.42f, y - 0.12f, 0.13f, 0.07f);
            var core = Ellipse(x, y + 0.02f, 0.16f, 0.12f);
            var crack = shell && (Mathf.Abs(x + y * 0.35f) < 0.035f || Mathf.Abs(x - y * 0.45f) < 0.028f);
            var outer = Ellipse(x, y + 0.02f, 0.53f, 0.40f) || Ellipse(x, y - 0.36f, 0.34f, 0.23f)
                || legL1 || legL2 || legR1 || legR2;
            if (outer && !(shell || head || legL1 || legL2 || legR1 || legR2))
                return new Color(0.08f, 0.025f, 0.012f, 1f);
            if (core)
                return new Color(1f, 0.62f, 0.12f, 1f);
            if (crack)
                return new Color(1f, 0.32f, 0.05f, 0.95f);
            if (shell || head || legL1 || legL2 || legR1 || legR2)
            {
                var d = Mathf.Clamp01(1f - (x * x + y * y) * 1.7f);
                return Color.Lerp(new Color(0.28f, 0.08f, 0.03f, 1f), new Color(0.68f, 0.18f, 0.04f, 1f), d * 0.65f);
            }
            return Color.clear;
        });
    }

    // ── Binary digit sprites for falling-rain background ─────────────
    Sprite MakeBinaryOneSprite()
    {
        // Stylised "1": vertical bar, top serif, bottom base  Elike a digital readout
        return MakeColorSprite(32, (x, y) =>
        {
            var body       = Mathf.Abs(x) < 0.11f && Mathf.Abs(y) < 0.62f;
            var topSerif   = (y > 0.34f && y < 0.50f) && (x > -0.36f && x < 0.05f);
            var bottomBase = (y > -0.66f && y < -0.50f) && Mathf.Abs(x) < 0.34f;
            if (body || topSerif || bottomBase)
                return Color.white;
            return Color.clear;
        });
    }

    Sprite MakeBinaryZeroSprite()
    {
        // Stylised "0": tall hollow oval
        return MakeColorSprite(32, (x, y) =>
        {
            var outer = Ellipse(x, y, 0.30f, 0.58f);
            var inner = Ellipse(x, y, 0.16f, 0.42f);
            if (outer && !inner)
                return Color.white;
            return Color.clear;
        });
    }

    Sprite MakeBossSprite()
    {
        return MakeColorSprite(128, (x, y) =>
        {
            var body = Ellipse(x, y + 0.02f, 0.45f, 0.56f);
            var head = Ellipse(x, y - 0.54f, 0.34f, 0.22f);
            var leftWing = Ellipse(x + 0.55f, y + 0.02f, 0.24f, 0.5f);
            var rightWing = Ellipse(x - 0.55f, y + 0.02f, 0.24f, 0.5f);
            var horns = Triangle(x + 0.18f, y - 0.75f, 0f, -0.2f, 0.16f) || Triangle(x - 0.18f, y - 0.75f, 0f, -0.2f, 0.16f);
            var sigil = body && Mathf.Abs(x) + Mathf.Abs(y + 0.02f) < 0.19f;
            var wingVein = (leftWing && Mathf.Abs((x + 0.55f) - (y + 0.02f) * 0.46f) < 0.032f)
                        || (rightWing && Mathf.Abs((x - 0.55f) + (y + 0.02f) * 0.46f) < 0.032f);
            var inner = body || head || leftWing || rightWing || horns;
            var outer = Ellipse(x, y + 0.02f, 0.52f, 0.63f)
                || Ellipse(x, y - 0.54f, 0.41f, 0.29f)
                || Ellipse(x + 0.55f, y + 0.02f, 0.29f, 0.56f)
                || Ellipse(x - 0.55f, y + 0.02f, 0.29f, 0.56f)
                || Triangle(x + 0.18f, y - 0.75f, 0f, -0.24f, 0.21f)
                || Triangle(x - 0.18f, y - 0.75f, 0f, -0.24f, 0.21f);
            if (outer && !inner)
                return new Color(0.02f, 0.01f, 0.04f, 1f);
            if (sigil)
                return new Color(1f, 0.18f, 0.78f, 1f);
            if (body || head || leftWing || rightWing)
            {
                if (Mathf.Abs(x) < 0.05f || Mathf.Abs(y + 0.02f) < 0.04f)
                    return new Color(0.15f, 1f, 0.95f, 1f);
                if (wingVein)
                    return new Color(0.15f, 1f, 0.95f, 0.62f);
                if (Ellipse(x + 0.13f, y - 0.57f, 0.045f, 0.045f) || Ellipse(x - 0.13f, y - 0.57f, 0.045f, 0.045f))
                    return new Color(1f, 0.18f, 0.78f, 1f);
                var d = Mathf.Clamp01(1f - (x * x + (y + 0.02f) * (y + 0.02f)) * 1.15f);
                return Color.Lerp(new Color(0.28f, 0.1f, 0.54f, 1f), new Color(0.44f, 0.18f, 0.74f, 1f), d * 0.6f);
            }
            return Color.clear;
        });
    }

    bool Ellipse(float x, float y, float rx, float ry)
    {
        return x * x / (rx * rx) + y * y / (ry * ry) <= 1f;
    }

    bool Triangle(float x, float y, float cx, float cy, float size)
    {
        var px = Mathf.Abs(x - cx);
        var py = y - cy;
        return py >= 0f && py <= size && px <= (size - py) * 0.75f;
    }

    bool Hex(float x, float y, float size)
    {
        x = Mathf.Abs(x);
        y = Mathf.Abs(y);
        return x <= size * 0.86f && y <= size && x * 0.58f + y <= size;
    }

    void CreateWorld()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            var cameraGo = new GameObject("Main Camera");
            mainCamera = cameraGo.AddComponent<Camera>();
            cameraGo.tag = "MainCamera";
        }

        mainCamera.orthographic = true;
        mainCamera.orthographicSize = CameraBaseSize / Mathf.Max(0.1f, cameraZoom);
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = new Color(0.005f, 0.012f, 0.02f);
        mainCamera.transform.position = new Vector3(0, 0, -10);
        if (mainCamera.GetComponent<AudioListener>() == null)
            mainCamera.gameObject.AddComponent<AudioListener>();

        CreateBackground();
        lantern = CreateSpriteObject("Lantern Core", lanternSprite, new Vector3(0, 0, 0), Color.white, 1.25f).transform;
        lanternGlowRenderer = CreateGlow("Lantern Glow", lantern, new Color(1f, 0.68f, 0.2f, 0.17f), lightRadius * 1.75f);
        CreateDropShadow(lantern, 1.42f, 0.42f, 0.42f);
        CreateWorldHpBar("Core HP Bar", lantern, new Vector3(0, 1.05f, -0.4f), 1.55f, new Color(1f, 0.78f, 0.18f), out eggWorldHpRoot, out eggWorldHpFill, out eggWorldHpFillRenderer);

        player = CreateSpriteObject("Player", playerFormSprites[0], new Vector3(0, -2.1f, 0), Color.white, 0.62f).transform;
        playerRenderer = CreateBodyRenderer(player, player.GetComponent<SpriteRenderer>(), "Player Body", 0);
        playerVisualBody = playerRenderer.transform;
        lastPlayerVisualPosition = player.position;
        playerGlowRenderer = CreateGlow("Player Glow", player, new Color(0.25f, 0.74f, 1f, 0.13f), 2.1f);
        CreateDropShadow(player, 1.18f, 0.32f, 0.36f);
        CreateWorldHpBar("Player HP Bar", player, new Vector3(0, 0.86f, -0.4f), 1.08f, new Color(0.28f, 0.9f, 1f), out playerWorldHpRoot, out playerWorldHpFill, out playerWorldHpFillRenderer);
        CreatePlayerAuras();

        SetupAudio();
    }

    void CreateBackground()
    {
        // Force plain dark square instead of Floor_DarkBase_A.png (it had diagonal stripe pattern)
        var floorSprite = squareSprite;
        var floorColor = new Color(0.012f, 0.025f, 0.037f);
        var floor = CreateSpriteObject("Ash Floor", floorSprite, Vector3.zero, floorColor, ArenaRadius * 3.6f);
        floor.transform.position = new Vector3(0, 0, 1.2f);
        floor.GetComponent<SpriteRenderer>().sortingOrder = -16;

        // ── Floor grid overlay PNG: DISABLED (caused screen-wide diagonal stripes)
        // The procedural Data Grid lines below provide cleaner structure.
        // Re-enable only if a non-diagonal pattern PNG is provided.

        // ── PNG floor tiles: very faint base texture (was alpha 0.46 →0.08) ──
        if (floorTileASprite != null || floorTileBSprite != null || floorTileCSprite != null)
        {
            const float tileStep = 1.48f;
            for (var ix = -8; ix <= 8; ix++)
            {
                for (var iy = -8; iy <= 8; iy++)
                {
                    var jitter = new Vector2(Mathf.Lerp(-0.06f, 0.06f, (float)rng.NextDouble()), Mathf.Lerp(-0.06f, 0.06f, (float)rng.NextDouble()));
                    var pos2 = new Vector2(ix * tileStep, iy * tileStep) + jitter;
                    if (pos2.magnitude > ArenaRadius - 0.35f)
                        continue;
                    // Only ~50% of grid cells get a tile (more breathing room)
                    if (rng.NextDouble() > 0.5) continue;

                    var skin = PickFloorTileSprite(ix, iy);
                    if (skin == null)
                        continue;

                    var tile = CreateSpriteObject("Generated Floor Tile", skin, new Vector3(pos2.x, pos2.y, 1.1f), new Color(0.55f, 0.75f, 0.9f, 0.08f), 1.42f);
                    tile.transform.rotation = Quaternion.Euler(0f, 0f, rng.Next(0, 4) * 90f);
                    var renderer = tile.GetComponent<SpriteRenderer>();
                    renderer.sortingOrder = -11;
                    TrackAmbientNeon(renderer);
                }
            }
        }

        // ── Cracks: 8→, very subtle ──
        if (HasSprite(floorCrackSprites))
        {
            for (var i = 0; i < 4; i++)
            {
                var sprite = PickSprite(floorCrackSprites);
                var pos = RandomArenaPoint(ArenaRadius - 1.5f, 1.0f, 1.0f);
                var crack = CreateSpriteObject("Floor Crack Decal", sprite, pos, new Color(1f, 1f, 1f, Mathf.Lerp(0.12f, 0.20f, (float)rng.NextDouble())), Mathf.Lerp(0.42f, 0.78f, (float)rng.NextDouble()));
                crack.transform.rotation = Quaternion.Euler(0f, 0f, rng.Next(0, 8) * 45f);
                crack.GetComponent<SpriteRenderer>().sortingOrder = -8;
            }
        }

        // ── Cables: 6→, very subtle (these were the bright criss-cross noise) ──
        if (HasSprite(floorCableSprites))
        {
            for (var i = 0; i < 3; i++)
            {
                var sprite = PickSprite(floorCableSprites);
                var pos = RandomArenaPoint(ArenaRadius - 1.8f, 2.6f, 0.98f);
                var cable = CreateSpriteObject("Floor Cable Decal", sprite, pos, new Color(1f, 1f, 1f, Mathf.Lerp(0.10f, 0.18f, (float)rng.NextDouble())), Mathf.Lerp(0.48f, 0.85f, (float)rng.NextDouble()));
                cable.transform.rotation = Quaternion.Euler(0f, 0f, rng.Next(0, 8) * 45f);
                var cableRenderer = cable.GetComponent<SpriteRenderer>();
                cableRenderer.sortingOrder = -7;
                TrackAmbientNeon(cableRenderer);
            }
        }

        // ── Random data tiles: 55→5, much darker (background only) ──
        for (var i = 0; i < 25; i++)
        {
            var angle = (float)rng.NextDouble() * Mathf.PI * 2f;
            var radius = Mathf.Sqrt((float)rng.NextDouble()) * (ArenaRadius - 1.2f);
            var pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 1.05f);
            var tileColor = i % 5 == 0
                ? new Color(0.08f, 0.78f, 1f, 0.07f)
                : i % 7 == 0
                    ? new Color(0.42f, 1f, 0.36f, 0.05f)
                    : new Color(0.04f, 0.10f, 0.14f, 0.18f);
            var tileRoll = rng.NextDouble();
            Sprite tileSprite;
            Vector3 tileScale;
            if (tileRoll < 0.22f)
            {
                tileSprite = diamondSprite;
                tileScale = Vector3.one * Mathf.Lerp(0.10f, 0.22f, (float)rng.NextDouble());
            }
            else if (tileRoll < 0.38f)
            {
                tileSprite = circleSprite;
                tileScale = Vector3.one * Mathf.Lerp(0.08f, 0.18f, (float)rng.NextDouble());
            }
            else
            {
                tileSprite = squareSprite;
                tileScale = new Vector3(Mathf.Lerp(0.14f, 0.45f, (float)rng.NextDouble()), Mathf.Lerp(0.05f, 0.18f, (float)rng.NextDouble()), 1f);
            }
            var tile = CreateSpriteObject("Floor Data Tile", tileSprite, pos, tileColor, 1f);
            tile.transform.localScale = tileScale;
            tile.transform.rotation = Quaternion.Euler(0, 0, rng.Next(0, 4) * 90f);
            tile.GetComponent<SpriteRenderer>().sortingOrder = -12;
        }

        // ── Server debris: 18→, smaller and dimmer ──
        for (var i = 0; i < 8; i++)
        {
            var angle = (float)rng.NextDouble() * Mathf.PI * 2f;
            var distance = Mathf.Lerp(4.4f, ArenaRadius - 0.7f, (float)rng.NextDouble());
            var pos = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance, 1.08f);
            if (HasSprite(serverDebrisSprites))
            {
                var debris = CreateSpriteObject("Server Debris Prop", PickSprite(serverDebrisSprites), pos, new Color(1f, 1f, 1f, Mathf.Lerp(0.32f, 0.52f, (float)rng.NextDouble())), Mathf.Lerp(0.30f, 0.55f, (float)rng.NextDouble()));
                debris.transform.rotation = Quaternion.Euler(0, 0, rng.Next(0, 16) * 22.5f);
                var debrisRenderer = debris.GetComponent<SpriteRenderer>();
                debrisRenderer.sortingOrder = -6;
                TrackAmbientNeon(debrisRenderer);
            }
            else
            {
                var slab = CreateSpriteObject("Broken Server Plate", squareSprite, pos, new Color(0.025f, 0.055f, 0.07f, 0.5f), 1f);
                slab.transform.localScale = new Vector3(Mathf.Lerp(0.45f, 0.95f, (float)rng.NextDouble()), Mathf.Lerp(0.16f, 0.32f, (float)rng.NextDouble()), 1f);
                slab.transform.rotation = Quaternion.Euler(0, 0, rng.Next(0, 8) * 22.5f);
                slab.GetComponent<SpriteRenderer>().sortingOrder = -13;

                var glyph = CreateSpriteObject("Plate Glyph", squareSprite, pos + Vector3.back * 0.02f, i % 3 == 0 ? new Color(0.16f, 0.9f, 1f, 0.12f) : new Color(0.42f, 1f, 0.36f, 0.09f), 1f);
                glyph.transform.localScale = new Vector3(0.16f, 0.03f, 1f);
                glyph.transform.rotation = slab.transform.rotation;
                var glyphRenderer = glyph.GetComponent<SpriteRenderer>();
                glyphRenderer.sortingOrder = -5;
                TrackAmbientNeon(glyphRenderer);
            }
        }

        // Pylons 10→, alpha 0.88→.42 (dimmer so they don't draw the eye)
        if (propNeonPylonSprite != null)
        {
            for (var i = 0; i < 4; i++)
            {
                var pos = RandomArenaPoint(ArenaRadius - 1.4f, 5.2f, 0.72f);
                var pylon = CreateSpriteObject("Neon Pylon Prop", propNeonPylonSprite, pos, new Color(1f, 1f, 1f, 0.42f), Mathf.Lerp(0.42f, 0.62f, (float)rng.NextDouble()));
                pylon.transform.rotation = Quaternion.Euler(0f, 0f, rng.Next(0, 8) * 45f);
                var pylonRenderer = pylon.GetComponent<SpriteRenderer>();
                pylonRenderer.sortingOrder = -2;
                TrackAmbientNeon(pylonRenderer);
            }
        }

        // Terminals 7→, dimmer
        if (propDataTerminalSprite != null)
        {
            for (var i = 0; i < 3; i++)
            {
                var pos = RandomArenaPoint(ArenaRadius - 2.0f, 3.6f, 0.74f);
                var terminal = CreateSpriteObject("Data Terminal Prop", propDataTerminalSprite, pos, new Color(1f, 1f, 1f, 0.45f), Mathf.Lerp(0.40f, 0.58f, (float)rng.NextDouble()));
                terminal.transform.rotation = Quaternion.Euler(0f, 0f, rng.Next(0, 8) * 45f);
                var terminalRenderer = terminal.GetComponent<SpriteRenderer>();
                terminalRenderer.sortingOrder = -2;
                TrackAmbientNeon(terminalRenderer);
            }
        }

        // Crates 16→, dimmer
        if (propCrateSprite != null)
        {
            for (var i = 0; i < 6; i++)
            {
                var pos = RandomArenaPoint(ArenaRadius - 1.2f, 4.4f, 0.76f);
                var crate = CreateSpriteObject("Data Crate Prop", propCrateSprite, pos, new Color(1f, 1f, 1f, 0.38f), Mathf.Lerp(0.38f, 0.58f, (float)rng.NextDouble()));
                crate.transform.rotation = Quaternion.Euler(0f, 0f, rng.Next(0, 8) * 45f);
                crate.GetComponent<SpriteRenderer>().sortingOrder = -3;
            }
        }

        // Clean tech grid  Estrong lines kept but secondary lines barely visible
        for (var i = -12; i <= 12; i++)
        {
            var strong = i % 4 == 0;
            var gridColor = strong ? new Color(0.12f, 0.72f, 0.92f, 0.18f) : new Color(0.08f, 0.36f, 0.48f, 0.06f);
            var vertical = CreateSpriteObject("Data Grid V", squareSprite, new Vector3(i, 0, 0.95f), gridColor, 1f);
            vertical.transform.localScale = new Vector3(strong ? 0.035f : 0.016f, ArenaRadius * 2f, 1f);
            var horizontal = CreateSpriteObject("Data Grid H", squareSprite, new Vector3(0, i, 0.95f), gridColor, 1f);
            horizontal.transform.localScale = new Vector3(ArenaRadius * 2f, strong ? 0.035f : 0.016f, 1f);
        }

        for (var i = 0; i < 32; i++)
        {
            var angle = Mathf.PI * 2f * i / 32f;
            var pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * ArenaRadius;
            var boundarySprite = PickSprite(boundaryStoneSprites);
            var marker = CreateSpriteObject("Boundary Stone", boundarySprite != null ? boundarySprite : diamondSprite, pos, boundarySprite != null ? new Color(1f, 1f, 1f, 0.92f) : i % 4 == 0 ? new Color(0.16f, 0.7f, 0.86f, 0.72f) : new Color(0.13f, 0.16f, 0.21f, 0.92f), boundarySprite != null ? 0.58f + (i % 3) * 0.08f : 0.25f + (i % 3) * 0.05f);
            marker.transform.rotation = Quaternion.Euler(0, 0, i * 11.25f);
            var markerRenderer = marker.GetComponent<SpriteRenderer>();
            markerRenderer.sortingOrder = -1;
            if (boundarySprite != null)
                TrackAmbientNeon(markerRenderer);
        }

        if (floorCoreMarkSprite != null)
        {
            var mark = CreateSpriteObject("Core Floor Mark", floorCoreMarkSprite, new Vector3(0, 0, 0.86f), new Color(1f, 1f, 1f, 0.74f), 4.2f);
            var markRenderer = mark.GetComponent<SpriteRenderer>();
            markRenderer.sortingOrder = -7;
            TrackAmbientNeon(markRenderer);
        }

        if (corePlatformSprite != null)
        {
            var platform = CreateSpriteObject("Core Platform", corePlatformSprite, new Vector3(0, 0, 0.78f), new Color(1f, 1f, 1f, 0.9f), 2.85f);
            var platformRenderer = platform.GetComponent<SpriteRenderer>();
            platformRenderer.sortingOrder = -6;
            TrackAmbientNeon(platformRenderer);
        }

        if (coreRingOuterSprite != null)
        {
            var outer = CreateSpriteObject("Core Outer Ring Art", coreRingOuterSprite, new Vector3(0, 0, 0.7f), new Color(1f, 1f, 1f, 0.86f), 3.35f);
            var outerRenderer = outer.GetComponent<SpriteRenderer>();
            outerRenderer.sortingOrder = -5;
            TrackAmbientNeon(outerRenderer);
        }

        if (coreRingInnerSprite != null)
        {
            var inner = CreateSpriteObject("Core Inner Ring Art", coreRingInnerSprite, new Vector3(0, 0, 0.66f), new Color(1f, 1f, 1f, 0.92f), 2.1f);
            var innerRenderer = inner.GetComponent<SpriteRenderer>();
            innerRenderer.sortingOrder = -4;
            TrackAmbientNeon(innerRenderer);
        }

        coreDamageCrack1Renderer = CreateCoreDamageCrack("Core Damage Crack Light", coreDamageCrack1Sprite, 3.15f, -3);
        coreDamageCrack2Renderer = CreateCoreDamageCrack("Core Damage Crack Mid", coreDamageCrack2Sprite, 3.25f, -2);
        coreDamageCrack3Renderer = CreateCoreDamageCrack("Core Damage Crack Heavy", coreDamageCrack3Sprite, 3.35f, -1);

        for (var i = 0; i < 24; i++)
        {
            var angle = Mathf.PI * 2f * i / 24f;
            var pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.88f) * 2.15f;
            var ring = CreateSpriteObject("Core Circuit Ring", diamondSprite, pos, new Color(1f, 0.82f, 0.2f, 0.28f), 0.16f);
            ring.transform.rotation = Quaternion.Euler(0, 0, i * 15f);
            var ringRenderer = ring.GetComponent<SpriteRenderer>();
            ringRenderer.sortingOrder = -8;
            TrackAmbientNeon(ringRenderer);
        }

        for (var i = 0; i < 56; i++)
        {
            var angle = Mathf.PI * 2f * i / 56f;
            var radius = i % 2 == 0 ? 3.05f : 3.45f;
            var pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.9f) * radius;
            var node = CreateSpriteObject("Core Neon Node", circleSprite, pos, i % 3 == 0 ? new Color(1f, 0.78f, 0.18f, 0.36f) : new Color(0.1f, 0.85f, 1f, 0.25f), i % 3 == 0 ? 0.08f : 0.045f);
            var nodeRenderer = node.GetComponent<SpriteRenderer>();
            nodeRenderer.sortingOrder = -7;
            TrackAmbientNeon(nodeRenderer);
        }

        // Circuit Traces: 34→4, dimmer (outer ring criss-cross was noisy)
        for (var i = 0; i < 14; i++)
        {
            var angle = (float)rng.NextDouble() * Mathf.PI * 2f;
            var distance = Mathf.Lerp(4.6f, ArenaRadius - 1.0f, (float)rng.NextDouble());
            var pos = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance, 0.93f);
            var trace = CreateSpriteObject("Circuit Trace", squareSprite, pos, i % 2 == 0 ? new Color(0.08f, 0.78f, 1f, 0.10f) : new Color(0.5f, 1f, 0.38f, 0.08f), 1f);
            trace.transform.localScale = new Vector3(Mathf.Lerp(0.45f, 1.4f, (float)rng.NextDouble()), 0.028f, 1f);
            trace.transform.rotation = Quaternion.Euler(0, 0, rng.Next(0, 8) * 45f);
            var traceRenderer = trace.GetComponent<SpriteRenderer>();
            traceRenderer.sortingOrder = -6;
            TrackAmbientNeon(traceRenderer);
        }

        // ── Binary rain (Matrix-style 0/1 cascade) ───────────────────────
        CreateBinaryRain();
    }

    // ── Binary rain background (Digimon "digital world" feel) ────────────
    const float BinaryTopY = 11f;
    const float BinaryBottomY = -10f;
    const float BinaryLeftX = -16f;
    const float BinaryRightX = 16f;

    void CreateBinaryRain()
    {
        var parent = new GameObject("Binary Rain Container").transform;
        for (var i = 0; i < 55; i++)
        {
            var isOne = rng.NextDouble() < 0.5;
            var sprite = isOne ? binaryOneSprite : binaryZeroSprite;
            var x = Mathf.Lerp(BinaryLeftX, BinaryRightX, (float)rng.NextDouble());
            var y = Mathf.Lerp(BinaryBottomY, BinaryTopY, (float)rng.NextDouble());
            var depth = (float)rng.NextDouble(); // 0 = far back, 1 = near front
            var baseAlpha = Mathf.Lerp(0.14f, 0.30f, depth);
            var scale = Mathf.Lerp(0.22f, 0.36f, depth);
            var colorRoll = rng.NextDouble();
            var color = colorRoll < 0.6 ? new Color(0.20f, 0.85f, 1f, baseAlpha)        // cyan
                      : colorRoll < 0.9 ? new Color(0.22f, 1f, 0.62f, baseAlpha)        // green-cyan
                                       : new Color(0.95f, 0.82f, 0.22f, baseAlpha);    // amber
            var go = CreateSpriteObject("Binary " + (isOne ? "1" : "0"), sprite, new Vector3(x, y, 1.6f), color, scale);
            var renderer = go.GetComponent<SpriteRenderer>();
            renderer.sortingOrder = -16; // way behind everything (including floor decals at -13~-5)
            go.transform.SetParent(parent);
            binaryDigits.Add(new BinaryDigit
            {
                transform = go.transform,
                renderer = renderer,
                fallSpeed = Mathf.Lerp(0.45f, 1.6f, depth),
                flickerTimer = (float)rng.NextDouble() * 2f,
                baseAlpha = baseAlpha
            });
        }
    }

    void UpdateBinaryRain()
    {
        if (binaryDigits.Count == 0) return;
        var dt = Time.unscaledDeltaTime; // keep falling even when paused/upgrading
        for (var i = 0; i < binaryDigits.Count; i++)
        {
            var d = binaryDigits[i];
            if (d.transform == null) continue;
            var pos = d.transform.position;
            pos.y -= d.fallSpeed * dt;
            if (pos.y < BinaryBottomY)
            {
                pos.y = BinaryTopY + (float)rng.NextDouble() * 2f;
                pos.x = Mathf.Lerp(BinaryLeftX, BinaryRightX, (float)rng.NextDouble());
                // Occasionally swap glyph for variety
                if (rng.NextDouble() < 0.25 && d.renderer != null)
                {
                    var isOne = rng.NextDouble() < 0.5;
                    d.renderer.sprite = isOne ? binaryOneSprite : binaryZeroSprite;
                }
            }
            d.transform.position = pos;
            // Subtle flicker every 0.3-1.7s
            d.flickerTimer -= dt;
            if (d.flickerTimer <= 0f && d.renderer != null)
            {
                d.flickerTimer = 0.3f + (float)rng.NextDouble() * 1.4f;
                var c = d.renderer.color;
                c.a = d.baseAlpha * Mathf.Lerp(0.55f, 1.25f, (float)rng.NextDouble());
                d.renderer.color = c;
            }
        }
    }

    Sprite PickFloorTileSprite(int x, int y)
    {
        var index = Mathf.Abs(x * 17 + y * 31) % 3;
        if (index == 0 && floorTileASprite != null)
            return floorTileASprite;
        if (index == 1 && floorTileBSprite != null)
            return floorTileBSprite;
        if (floorTileCSprite != null)
            return floorTileCSprite;
        if (floorTileASprite != null)
            return floorTileASprite;
        return floorTileBSprite;
    }

    bool HasSprite(Sprite[] sprites)
    {
        if (sprites == null)
            return false;

        for (var i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null)
                return true;
        }

        return false;
    }

    Sprite PickSprite(Sprite[] sprites)
    {
        if (!HasSprite(sprites))
            return null;

        var start = rng.Next(0, sprites.Length);
        for (var i = 0; i < sprites.Length; i++)
        {
            var sprite = sprites[(start + i) % sprites.Length];
            if (sprite != null)
                return sprite;
        }

        return null;
    }

    Vector3 RandomArenaPoint(float maxRadius, float minRadius, float z)
    {
        maxRadius = Mathf.Max(0.1f, maxRadius);
        minRadius = Mathf.Clamp(minRadius, 0f, maxRadius);
        var angle = (float)rng.NextDouble() * Mathf.PI * 2f;
        var radius01 = Mathf.Sqrt((float)rng.NextDouble());
        var radius = Mathf.Lerp(minRadius, maxRadius, radius01);
        return new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, z);
    }

    SpriteRenderer CreateCoreDamageCrack(string name, Sprite sprite, float scale, int sortingOrder)
    {
        if (sprite == null)
            return null;

        var crack = CreateSpriteObject(name, sprite, new Vector3(0, 0, 0.62f), Color.clear, scale);
        var renderer = crack.GetComponent<SpriteRenderer>();
        renderer.sortingOrder = sortingOrder;
        return renderer;
    }

    void TrackAmbientNeon(SpriteRenderer renderer)
    {
        ambientNeons.Add(new AmbientNeon
        {
            renderer = renderer,
            baseColor = renderer.color,
            phase = (float)rng.NextDouble() * Mathf.PI * 2f
        });
    }

    GameObject CreateSpriteObject(string name, Sprite sprite, Vector3 position, Color color, float scale)
    {
        var go = new GameObject(name);
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = Mathf.RoundToInt(-position.z * 10);
        go.transform.position = position;
        go.transform.localScale = Vector3.one * scale;
        return go;
    }

    SpriteRenderer CreateGlow(string name, Transform parent, Color color, float scale)
    {
        var glow = CreateSpriteObject(name, circleSprite, parent.position + Vector3.forward * 0.1f, color, scale);
        glow.transform.SetParent(parent);
        glow.transform.localPosition = Vector3.forward * 0.1f;
        var renderer = glow.GetComponent<SpriteRenderer>();
        renderer.sortingOrder = -4;
        return renderer;
    }

    SpriteRenderer CreateBodyRenderer(Transform parent, SpriteRenderer source, string name, int sortingOffset)
    {
        var body = new GameObject(name);
        body.transform.SetParent(parent, false);
        body.transform.localPosition = Vector3.zero;
        body.transform.localRotation = Quaternion.identity;
        body.transform.localScale = Vector3.one;

        var renderer = body.AddComponent<SpriteRenderer>();
        if (source != null)
        {
            renderer.sprite = source.sprite;
            renderer.color = source.color;
            renderer.sortingOrder = source.sortingOrder + sortingOffset;
            source.enabled = false;
        }

        return renderer;
    }

    void CreateDropShadow(Transform parent, float width, float height, float alpha)
    {
        var shadow = CreateSpriteObject(parent.name + " Shadow", circleSprite, parent.position + Vector3.forward * 0.16f, new Color(0f, 0f, 0f, alpha), 1f);
        shadow.transform.SetParent(parent, false);
        shadow.transform.localPosition = new Vector3(0f, 0.16f, 0.16f);
        shadow.transform.localScale = new Vector3(width, height, 1f);
        var renderer = shadow.GetComponent<SpriteRenderer>();
        renderer.sortingOrder = -5;
        dropShadowRenderers.Add(renderer);
        shadow.SetActive(enhancedVisuals);
    }

    SpriteRenderer CreateVisualTrim(Transform parent, string name, Sprite sprite, Vector3 localPosition, Vector3 localScale, Color color, int sortingOrder)
    {
        var trim = CreateSpriteObject(name, sprite, parent.position + localPosition, color, 1f);
        trim.transform.SetParent(parent, false);
        trim.transform.localPosition = localPosition;
        trim.transform.localScale = localScale;
        var renderer = trim.GetComponent<SpriteRenderer>();
        renderer.sortingOrder = sortingOrder;
        enhancedVisualRenderers.Add(renderer);
        trim.SetActive(enhancedVisuals);
        return renderer;
    }

    void CreateWorldHpBar(string name, Transform parent, Vector3 offset, float width, Color fillColor, out Transform root, out Transform fill, out SpriteRenderer fillRenderer)
    {
        var rootGo = new GameObject(name);
        rootGo.transform.SetParent(parent, false);
        rootGo.transform.localPosition = offset;
        root = rootGo.transform;

        var back = CreateSpriteObject(name + " Back", squareSprite, parent.position, new Color(0.02f, 0.025f, 0.035f, 0.88f), 1f);
        back.transform.SetParent(root, false);
        back.transform.localPosition = Vector3.zero;
        back.transform.localScale = new Vector3(width, 0.075f, 1f);
        back.GetComponent<SpriteRenderer>().sortingOrder = 70;

        var fillGo = CreateSpriteObject(name + " Fill", squareSprite, parent.position, fillColor, 1f);
        fillGo.transform.SetParent(root, false);
        fillGo.transform.localPosition = Vector3.zero;
        fillGo.transform.localScale = new Vector3(width, 0.048f, 1f);
        fillRenderer = fillGo.GetComponent<SpriteRenderer>();
        fillRenderer.sortingOrder = 71;
        fill = fillGo.transform;
    }

    void CreateEnemyHpBar(Enemy enemy, float width, float yOffset, Color fillColor)
    {
        CreateWorldHpBar(enemy.type + " HP Bar", enemy.transform, new Vector3(0f, yOffset, -0.35f), width, new Color(fillColor.r, fillColor.g, fillColor.b, 0.92f), out enemy.hpBarRoot, out enemy.hpBarFill, out enemy.hpBarFillRenderer);
        enemy.hpBarWidth = width;
        if (enemy.hpBarRoot != null)
            enemy.hpBarRoot.gameObject.SetActive(false);
    }

    void UpdateEnemyHpBar(Enemy enemy)
    {
        if (enemy.hpBarRoot == null || enemy.hpBarFill == null)
            return;

        var ratio = Mathf.Clamp01(enemy.hp / Mathf.Max(0.01f, enemy.maxHp));
        enemy.hpBarRoot.gameObject.SetActive(ratio < 0.995f && enemy.hp > 0f);
        enemy.hpBarFill.localScale = new Vector3(Mathf.Max(0.02f, enemy.hpBarWidth * ratio), 0.048f, 1f);
        enemy.hpBarFill.localPosition = new Vector3(-enemy.hpBarWidth * (1f - ratio) * 0.5f, 0f, 0f);
        if (enemy.hpBarFillRenderer != null)
        {
            var critical = ratio <= 0.35f;
            enemy.hpBarFillRenderer.color = critical
                ? Color.Lerp(new Color(1f, 0.16f, 0.12f, 0.95f), new Color(1f, 0.9f, 0.22f, 0.95f), Mathf.Sin(Time.time * 10f) * 0.5f + 0.5f)
                : new Color(0.34f, 1f, 0.72f, 0.92f);
        }
    }

    void CreatePlayerAuras()
    {
        var shieldGo = CreateSpriteObject("Guard Aura", circleSprite, player.position + Vector3.forward * 0.08f, new Color(0.22f, 1f, 0.72f, 0.12f), 1.65f);
        shieldGo.transform.SetParent(player, false);
        shieldGo.transform.localPosition = Vector3.forward * 0.08f;
        shieldAura = shieldGo.transform;
        shieldAuraRenderer = shieldGo.GetComponent<SpriteRenderer>();
        shieldAuraRenderer.sortingOrder = -3;
        shieldGo.SetActive(false);

        var fusionGo = CreateSpriteObject("Fusion Aura", diamondSprite, player.position + Vector3.forward * 0.07f, Color.clear, 1.75f);
        fusionGo.transform.SetParent(player, false);
        fusionGo.transform.localPosition = Vector3.forward * 0.07f;
        fusionAura = fusionGo.transform;
        fusionAuraRenderer = fusionGo.GetComponent<SpriteRenderer>();
        fusionAuraRenderer.sortingOrder = -2;
        fusionGo.SetActive(false);

        playerStageHaloRenderer = CreateVisualTrim(player, "Evolution Stage Halo", diamondSprite, new Vector3(0f, 0.05f, -0.18f), new Vector3(0.9f, 0.9f, 1f), new Color(0.35f, 1f, 1f, 0.12f), 36);
        playerStageHalo = playerStageHaloRenderer.transform;
        playerLeftAccentRenderer = CreateVisualTrim(player, "Evolution Left Accent", diamondSprite, new Vector3(-0.48f, 0.02f, -0.16f), new Vector3(0.22f, 0.46f, 1f), new Color(0.35f, 1f, 1f, 0.78f), 43);
        playerLeftAccent = playerLeftAccentRenderer.transform;
        playerRightAccentRenderer = CreateVisualTrim(player, "Evolution Right Accent", diamondSprite, new Vector3(0.48f, 0.02f, -0.16f), new Vector3(0.22f, 0.46f, 1f), new Color(0.35f, 1f, 1f, 0.78f), 43);
        playerRightAccent = playerRightAccentRenderer.transform;
        playerCrestAccentRenderer = CreateVisualTrim(player, "Evolution Crest Accent", diamondSprite, new Vector3(0f, -0.56f, -0.17f), new Vector3(0.18f, 0.28f, 1f), new Color(1f, 0.86f, 0.28f, 0.9f), 44);
        playerCrestAccent = playerCrestAccentRenderer.transform;

        CreateLinkCompanion(0, "Nova Link", novaLinkSprite, new Color(0.35f, 0.95f, 1f));
        CreateLinkCompanion(1, "Bulwark Link", bulwarkLinkSprite, new Color(0.42f, 1f, 0.56f));
        CreateLinkCompanion(2, "Siphon Link", siphonLinkSprite, new Color(0.95f, 1f, 0.35f));
        CreateLinkCompanion(3, "Phase Link", phaseLinkSprite, new Color(0.78f, 0.48f, 1f));
    }

    void CreateLinkCompanion(int index, string name, Sprite sprite, Color glowColor)
    {
        var glow = CreateSpriteObject(name + " Glow", circleSprite, player.position + Vector3.forward * 0.04f, new Color(glowColor.r, glowColor.g, glowColor.b, 0.14f), 0.72f);
        linkCompanionGlows[index] = glow.transform;
        linkCompanionGlowRenderers[index] = glow.GetComponent<SpriteRenderer>();
        linkCompanionGlowRenderers[index].sortingOrder = 38;

        var companion = CreateSpriteObject(name, sprite, player.position + Vector3.back * 0.08f, Color.white, 0.48f);
        linkCompanions[index] = companion.transform;
        linkCompanionRenderers[index] = companion.GetComponent<SpriteRenderer>();
        linkCompanionRenderers[index].sortingOrder = 42;
        var sigilSprite = index == 1 ? squareSprite : index == 2 ? diamondSprite : circleSprite;
        var sigilColor = index == 0 ? new Color(0.45f, 1f, 1f, 0.9f) : index == 1 ? new Color(0.5f, 1f, 0.55f, 0.9f) : new Color(1f, 1f, 0.35f, 0.9f);
        CreateVisualTrim(companion.transform, name + " Sigil", sigilSprite, new Vector3(0f, 0.02f, -0.12f), new Vector3(0.16f, 0.16f, 1f), sigilColor, 45);

        glow.SetActive(false);
        companion.SetActive(false);
    }

    void SetupAudio()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxEnabled ? 0.35f * Mathf.Clamp01(sfxVolume) : 0f;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.volume = bgmEnabled ? 0.18f * Mathf.Clamp01(bgmVolume) : 0f;

        LoadAudioClip("BGM");
        LoadAudioClip("BGM_Stage2");
        LoadAudioClip("BGM_Stage3");
        LoadAudioClip("BGM_Stage4");
        LoadAudioClip("BGM_Stage5");
        LoadAudioClip("BGM_Boss_Pulswyrm");
        LoadAudioClip("BGM_Boss_Nullwyrm");
        LoadAudioClip("BGM_BossPulswyrm");
        LoadAudioClip("BGM_BossNullwyrm");
        LoadAudioClip("Shoot");
        LoadAudioClip("Hit");
        LoadAudioClip("Kill");
        LoadAudioClip("Pickup");
        LoadAudioClip("LevelUp");
        LoadAudioClip("Evolve");
        LoadAudioClip("Fusion");
        LoadAudioClip("Boss");
        LoadAudioClip("GameOver");
        GenerateProceduralAudio();
    }

    void LoadAudioClip(string name)
    {
        var clip = Resources.Load<AudioClip>("Audio/" + name);
        if (clip != null)
            audioClips[name] = clip;
    }

    void CreateUi()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        var canvasGo = new GameObject("Game UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

        CreateHud();

        centerText = CreateText("Center Message", canvas.transform, Vector2.zero, TextAnchor.MiddleCenter, 38, new Color(1f, 0.85f, 0.45f));
        centerText.rectTransform.sizeDelta = new Vector2(900, 180);
        centerText.fontStyle = FontStyle.Bold;

        CreateChoiceFocusOverlay();

        upgradePanel = new GameObject("Upgrade Panel", typeof(Image));
        upgradePanel.transform.SetParent(canvas.transform, false);
        var image = upgradePanel.GetComponent<Image>();
        image.color = new Color(0.006f, 0.018f, 0.03f, 0.95f);
        if (UseGeneratedHudPanels && panelFrameCyanSprite != null)
            ApplySimpleSprite(image, panelFrameCyanSprite, Color.white);
        var upgradeOutline = upgradePanel.AddComponent<Outline>();
        upgradeOutline.effectColor = new Color(0.35f, 1f, 1f, 0.36f);
        upgradeOutline.effectDistance = new Vector2(1.4f, -1.4f);
        var rect = upgradePanel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        // Shift LEFT by 100px so the wider panel does not overlap the right-side
        // module status panel (220 wide, anchored to canvas right edge).
        rect.anchoredPosition = new Vector2(-100f, 0f);
        // Panel: 840×80 →1000×40 (1.18×wide, 1.42×tall)  Efits within 1280×20 canvas,
        // leaves room for the right-side module status panel (canvas-anchored).
        rect.sizeDelta = new Vector2(1000, 540);
        AddPanelAccent(upgradePanel.transform, rect.sizeDelta);

        panelTitleText = CreateText("Upgrade Title", upgradePanel.transform, new Vector2(0, 232), TextAnchor.MiddleCenter, 34, new Color(1f, 0.85f, 0.46f));
        panelTitleText.text = "進化モジュールを選ぶ";
        panelTitleText.fontStyle = FontStyle.Bold;
        panelTitleText.rectTransform.sizeDelta = new Vector2(940, 46);

        panelSubtitleText = CreateText("Upgrade Subtitle", upgradePanel.transform, new Vector2(0, 196), TextAnchor.MiddleCenter, 18, new Color(0.78f, 1f, 0.96f));
        panelSubtitleText.text = "";
        panelSubtitleText.fontStyle = FontStyle.Bold;
        panelSubtitleText.rectTransform.sizeDelta = new Vector2(920, 28);
        panelSubtitleText.resizeTextForBestFit = true;
        panelSubtitleText.resizeTextMinSize = 14;
        panelSubtitleText.resizeTextMaxSize = 18;

        for (var i = 0; i < 3; i++)
        {
            // Cards spaced 310px apart (was 260)  Ecenter card at 0, sides at ±310
            var button = CreateButton("Upgrade " + (i + 1), upgradePanel.transform, new Vector2(-310 + i * 310, -8));
            upgradeButtons.Add(button);
        }

        rerollButton = CreateWideButton("Reroll Button", upgradePanel.transform, new Vector2(-130, -228), new Vector2(440, 52));
        SetButtonSpriteV2(rerollButton, btnRerollSprite);
        // Bigger reroll text  Ewas using default 18
        var rerollText = rerollButton.GetComponentInChildren<Text>();
        if (rerollText != null) { rerollText.fontSize = 20; rerollText.color = new Color(1f, 0.94f, 0.62f); }

        skipRewardButton = CreateWideButton("Skip Reward Button", upgradePanel.transform, new Vector2(300, -228), new Vector2(300, 52));
        SetButtonSpriteV2(skipRewardButton, btnSkipRewardSprite);
        var skipText = skipRewardButton.GetComponentInChildren<Text>();
        if (skipText != null) { skipText.fontSize = 18; skipText.color = new Color(0.72f, 1f, 0.96f); }

        // ── Module status side panel (right of upgrade panel) ──
        CreateModuleStatusSidePanel();

        upgradePanel.SetActive(false);
        if (moduleStatusSidePanel != null) moduleStatusSidePanel.SetActive(false);
        CreateMainMenuPanel();
        CreatePausePanel();
        CreateOptionsPanel();
        CreatePartnerSelectPanel();
        CreateResultPanel();
        CreateBossBar();
        CreateEvolutionCutscene();
        CreateBossCutscene();
        ApplyOptions();
    }

    void CreateChoiceFocusOverlay()
    {
        choiceFocusOverlay = new GameObject("Choice Focus Overlay", typeof(Image));
        choiceFocusOverlay.transform.SetParent(canvas.transform, false);
        var image = choiceFocusOverlay.GetComponent<Image>();
        if (UseGeneratedOverlayImages && hudChoiceDimSprite != null)
            ApplySimpleSprite(image, hudChoiceDimSprite, new Color(1f, 1f, 1f, 0.86f));
        else
            image.color = new Color(0f, 0.01f, 0.02f, 0.78f);
        image.raycastTarget = false;
        var rect = choiceFocusOverlay.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        choiceFocusOverlay.SetActive(false);
    }

    void SetChoiceFocus(bool active)
    {
        if (choiceFocusOverlay != null)
            choiceFocusOverlay.SetActive(active);
        if (active && centerText != null)
        {
            centerText.text = "";
            ClearScreenFlash();
            ClearDangerOverlay();
        }
        ApplyOptions();
    }

    bool IsChoiceFocusActive()
    {
        return choosingUpgrade || choiceFocusOverlay != null && choiceFocusOverlay.activeSelf;
    }

    // Ignore clicks briefly after a choice panel opens.
    bool IsChoiceClickGuarded()
    {
        return Time.unscaledTime < choiceClickGuardUntil;
    }

    void RepairActiveChoiceButtonInteractivity()
    {
        if (upgradePanel == null || !upgradePanel.activeSelf || !choosingUpgrade || choosingRelic)
            return;

        for (var i = 0; i < upgradeButtons.Count; i++)
        {
            var button = upgradeButtons[i];
            if (button != null && button.gameObject.activeSelf && !button.interactable)
                button.interactable = true;
        }

        if (skipRewardButton != null && skipRewardButton.gameObject.activeSelf && !skipRewardButton.interactable)
            skipRewardButton.interactable = true;
    }

    void CreateMainMenuPanel()
    {
        mainMenuPanel = new GameObject("Main Menu Screen", typeof(Image));
        mainMenuPanel.transform.SetParent(canvas.transform, false);
        var back = mainMenuPanel.GetComponent<Image>();
        back.color = new Color(0.002f, 0.012f, 0.02f, 0.99f);
        if (UseGeneratedBackgrounds && mainMenuBackgroundSprite != null)
            ApplyScreenBackgroundSprite(back, mainMenuBackgroundSprite, Color.white);
        StretchToParent(mainMenuPanel);

        // ── 背景を静かに: ゲーム画面の透けを濃いスクリムで抑え、雑然さを消す ──
        // 売れ筋タイトルの定石「複雑なアートで埋めない/静かな背景」に寄せる。
        CreateStretchImage("Menu Deep Scrim", mainMenuPanel.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0.004f, 0.012f, 0.018f, 0.82f));
        // 中央へ向かう淡いビネット (上下を少し沈ませて中央の主役を際立たせる)
        CreateStretchImage("Menu Top Vignette", mainMenuPanel.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -180), Vector2.zero, new Color(0f, 0.006f, 0.01f, 0.55f));
        CreateStretchImage("Menu Bottom Vignette", mainMenuPanel.transform, Vector2.zero, new Vector2(1, 0), Vector2.zero, new Vector2(0, 160), new Color(0f, 0.006f, 0.01f, 0.55f));
        // 上の青帯・下の黄帯は撤去 (ミニマル配色化)。左右レールは暗色で控えめに統一。
        CreateStretchImage("Menu Left Rail", mainMenuPanel.transform, Vector2.zero, new Vector2(0, 1), Vector2.zero, new Vector2(96, 0), new Color(0.01f, 0.03f, 0.045f, 0.55f));
        CreateStretchImage("Menu Right Rail", mainMenuPanel.transform, new Vector2(1, 0), Vector2.one, new Vector2(-96, 0), Vector2.zero, new Color(0.01f, 0.03f, 0.045f, 0.55f));

        // ── 四隅装飾は控えめに (クラッター削減)。alpha を大きく下げ、さりげない縁取り程度に。 ──
        if (UseTitleUiV2 && titleCornerAccentSprite != null)
        {
            CreateMenuSpriteImage("Menu Corner TL V2", mainMenuPanel.transform, titleCornerAccentSprite, new Vector2(-560, 258), new Vector2(150, 150), new Color(1f, 1f, 1f, 0.30f), true);
            var tr = CreateMenuSpriteImage("Menu Corner TR V2", mainMenuPanel.transform, titleCornerAccentSprite, new Vector2(560, 258), new Vector2(150, 150), new Color(1f, 1f, 1f, 0.30f), true);
            var bl = CreateMenuSpriteImage("Menu Corner BL V2", mainMenuPanel.transform, titleCornerAccentSprite, new Vector2(-560, -274), new Vector2(150, 150), new Color(1f, 1f, 1f, 0.22f), true);
            var br = CreateMenuSpriteImage("Menu Corner BR V2", mainMenuPanel.transform, titleCornerAccentSprite, new Vector2(560, -274), new Vector2(150, 150), new Color(1f, 1f, 1f, 0.22f), true);
            if (tr != null) tr.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
            if (bl != null) bl.rectTransform.localScale = new Vector3(1f, -1f, 1f);
            if (br != null) br.rectTransform.localScale = new Vector3(-1f, -1f, 1f);
        }

        // ── Title logo: 主役として大きく上部に (売れ筋の定石「ロゴが画面を占有」)。 ──
        var titleLogo = gameLogoTitleCompactSprite != null ? gameLogoTitleCompactSprite : gameLogoSprite;
        if (titleLogo != null)
            CreateMenuSpriteImage("Main Menu Game Logo", mainMenuPanel.transform, titleLogo, new Vector2(0, 224), new Vector2(720, 230), new Color(1f, 1f, 1f, 1f), true);

        var title = CreateText("Main Menu Title", mainMenuPanel.transform, new Vector2(0, 224), TextAnchor.MiddleCenter, 54, new Color(0.68f, 1f, 1f));
        title.text = "EGGCORE PROTOCOL";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(940, 68);
        title.resizeTextForBestFit = true;
        title.resizeTextMinSize = 40;
        title.resizeTextMaxSize = 54;
        var titleOutline = title.gameObject.AddComponent<Outline>();
        titleOutline.effectColor = new Color(0.03f, 0.82f, 1f, 0.18f);
        titleOutline.effectDistance = new Vector2(2.4f, -2.4f);
        title.gameObject.SetActive(titleLogo == null);  // logo present → hide text title

        // サブコピーは廃止 (ユーザー要望: 中央の文字を消してスッキリさせる)。

        // ── Progress summary (BEST/CLEARS/Stage/Danger) はタイトルから退避。 ──
        // Refresh メソッドが参照するので生成のみ行い、タイトルでは非表示にする。
        menuBestText = CreateText("Main Menu Archive Brief", mainMenuPanel.transform, new Vector2(0, 171), TextAnchor.MiddleCenter, 14, new Color(1f, 0.94f, 0.62f));
        menuBestText.fontStyle = FontStyle.Bold;
        menuBestText.rectTransform.sizeDelta = new Vector2(880, 22);
        menuBestText.gameObject.SetActive(false);

        mainMenuSelectionBriefText = CreateText("Main Menu Selection Brief", mainMenuPanel.transform, new Vector2(0, 134), TextAnchor.MiddleCenter, 13, new Color(0.84f, 1f, 0.94f));
        mainMenuSelectionBriefText.rectTransform.sizeDelta = new Vector2(820, 20);
        mainMenuSelectionBriefText.gameObject.SetActive(false);

        // ── 主役ビジュアル: キャラ + データエッグ + 光輪でフォーカルポイントと奥行きを作る。 ──
        // 売れ筋タイトルの定石「単一のフォーカルポイント / キャラアート / レイヤーで奥行き」に寄せる。
        // 旧ヒーローデッキ (CreateMainMenuHeroVisual) は情報過多なので使わず、主役だけに特化。
        CreateMainMenuHeroArt(mainMenuPanel.transform);

        startMenuButton = CreateWideButton("Start Run Button", mainMenuPanel.transform, new Vector2(0, -10), new Vector2(440, 64));
        SetButtonSpriteV2(startMenuButton, btnPrimaryStartSprite);
        var startImage = startMenuButton.GetComponent<Image>();
        if (startImage != null)
            startImage.color = new Color(0.005f, 0.022f, 0.032f, 0.98f);
        var startOutline = startMenuButton.GetComponent<Outline>();
        if (startOutline != null)
        {
            startOutline.effectColor = new Color(0.62f, 1f, 0.92f, 0.42f);
            startOutline.effectDistance = new Vector2(1.8f, -1.8f);
        }
        CreateStretchImage("Start Button Inner Rail", startMenuButton.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(24, -12), new Vector2(-24, -8), new Color(0.22f, 1f, 0.96f, 0.36f));
        var startText = startMenuButton.GetComponentInChildren<Text>();
        if (startText != null)
        {
            startText.text = "▶ START RUN";
            startText.fontSize = 26;
            startText.fontStyle = FontStyle.Bold;
            startText.color = new Color(0.62f, 1f, 0.95f);
        }
        startMenuButton.onClick.RemoveAllListeners();
        startMenuButton.onClick.AddListener(OpenRunConfig);

        // legacy: menuMissionText is kept as a field for compatibility.
        menuMissionText = null;
        // Old Stage/Danger chips were moved into RunConfigPanel; clear legacy lists.
        stageChipButtons.Clear();
        dangerChipButtons.Clear();

        // ── Secondary buttons: START の下に適度な間隔で並べる。Nav rail はタイトルでは使わない。 ──
        const float secY = -118f;
        const float secW = 138f;
        const float secH = 40f;
        const float secGap = 12f;
        const float secFirstCenter = -300f;

        menuMissionButton = CreateWideButton("Menu Mission Button", mainMenuPanel.transform, new Vector2(secFirstCenter, secY), new Vector2(secW, secH));
        SetButtonSpriteV2(menuMissionButton, btnMenuSecondarySprite);
        menuMissionButton.GetComponentInChildren<Text>().text = "MISSION";
        menuMissionButton.GetComponentInChildren<Text>().fontSize = 16;
        menuMissionButton.onClick.RemoveAllListeners();
        menuMissionButton.onClick.AddListener(ToggleMissionBoardPanel);

        menuTreeButton = CreateWideButton("Menu Tree Button", mainMenuPanel.transform, new Vector2(secFirstCenter + (secW + secGap), secY), new Vector2(secW, secH));
        SetButtonSpriteV2(menuTreeButton, btnMenuSecondarySprite);
        menuTreeButton.GetComponentInChildren<Text>().text = "進化ツリー";
        menuTreeButton.GetComponentInChildren<Text>().fontSize = 16;
        menuTreeButton.onClick.RemoveAllListeners();
        menuTreeButton.onClick.AddListener(ToggleEvolutionTreePanel);

        menuComboButton = CreateWideButton("Menu Combo Button", mainMenuPanel.transform, new Vector2(secFirstCenter + 2 * (secW + secGap), secY), new Vector2(secW, secH));
        SetButtonSpriteV2(menuComboButton, btnMenuSecondarySprite);
        menuComboButton.GetComponentInChildren<Text>().text = "進化コンボ";
        menuComboButton.GetComponentInChildren<Text>().fontSize = 16;
        menuComboButton.GetComponentInChildren<Text>().color = new Color(1f, 0.92f, 0.42f);
        menuComboButton.onClick.RemoveAllListeners();
        menuComboButton.onClick.AddListener(ToggleEvolutionComboCodexPanel);

        menuCodexButton = CreateWideButton("Menu Codex Button", mainMenuPanel.transform, new Vector2(secFirstCenter + 3 * (secW + secGap), secY), new Vector2(secW, secH));
        SetButtonSpriteV2(menuCodexButton, btnMenuSecondarySprite);
        menuCodexButton.GetComponentInChildren<Text>().text = "図鑑";
        menuCodexButton.GetComponentInChildren<Text>().fontSize = 16;
        menuCodexButton.onClick.RemoveAllListeners();
        menuCodexButton.onClick.AddListener(ToggleCodexPanel);

        menuOptionsButton = CreateWideButton("Menu Options Button", mainMenuPanel.transform, new Vector2(secFirstCenter + 4 * (secW + secGap), secY), new Vector2(secW, secH));
        SetButtonSpriteV2(menuOptionsButton, btnMenuSecondarySprite);
        menuOptionsButton.GetComponentInChildren<Text>().text = "OPTIONS";
        menuOptionsButton.GetComponentInChildren<Text>().fontSize = 16;
        menuOptionsButton.onClick.RemoveAllListeners();
        menuOptionsButton.onClick.AddListener(ToggleOptionsPanel);

        // ガイド文はタイトルから削除 (操作説明は必要時にオプション/ヘルプ側で)。

        // ── 極小バージョン表記のみ。スタジオ名 (Embercore) は名称未確定のため非表示。 ──
        var credit = CreateText("Main Menu Credit", mainMenuPanel.transform, new Vector2(0, -332), TextAnchor.MiddleCenter, 11, new Color(0.42f, 0.58f, 0.64f, 0.72f));
        credit.text = "Eggcore Protocol  v" + GameVersion;
        credit.rectTransform.sizeDelta = new Vector2(640, 16);

        CreateCodexPanel();
        CreateMissionBoardPanel();
        CreateEvolutionTreePanel();
        CreateEvolutionComboCodexPanel();
        CreateRunConfigPanel();
        mainMenuPanel.SetActive(false);
    }

    // タイトルの主役ビジュアル。初期相棒6体 (Partner_S1..S6_L0) をピクセルアートで横一列・均等配置し、
    // 「選べる相棒の豊富さ」をアピールする。各6体は接地点Y・占有高さが統一済み (足並みが揃う)。
    // 奥行きは行背後の広い淡グロウ + 各キャラのアクセント色グロウで作る (情報過多のバッジ/枠は持たない)。
    // 縦位置: ロゴ(y=224)とSTART(y=-10)の間。ロゴ下端~144 と START上端~22 の帯に収める。
    void CreateMainMenuHeroArt(Transform parent)
    {
        const int count = 6;          // 初期相棒6体 (species 1..6)
        const float cy = 86f;         // 行の中心 Y
        const float spacing = 150f;   // キャラ中心間の間隔
        const float box = 150f;       // 各キャラ表示サイズ
        const float left = -(count - 1) * 0.5f * spacing;  // -375 (中央そろえ)

        // ── 最奥: データエッグのエンブレム (相棒が守る対象の象徴・淡く背後に) ──
        if (titleCoreEggSprite != null)
            CreateMenuSpriteImage("Hero Data Egg Emblem", parent, titleCoreEggSprite, new Vector2(0, cy + 2), new Vector2(330, 330), new Color(1f, 1f, 1f, 0.16f), true);

        // ── 奥行き: 行全体の背後に広く淡い光のバンド (フラットさを避ける) ──
        CreateMenuSpriteImage("Hero Row Glow Far", parent, circleSprite, new Vector2(0, cy + 6), new Vector2(900, 300), new Color(0.10f, 0.55f, 0.95f, 0.06f), false);
        CreateMenuSpriteImage("Hero Row Glow Mid", parent, circleSprite, new Vector2(0, cy + 2), new Vector2(560, 230), new Color(0.16f, 0.85f, 1f, 0.06f), false);

        // ── 6体を横一列・均等配置。各キャラはアクセント色のグロウ + 足元グロウを伴う。 ──
        for (var i = 0; i < count; i++)
        {
            var species = i + 1;                  // 1..6 = Partner_S{species}_L0
            var x = left + i * spacing;
            var accent = GetPartnerAccentColor(species);

            // 背後グロウ (キャラの色を一目で区別 / 暗色キャラも映える)
            CreateMenuSpriteImage("Hero Halo " + species, parent, circleSprite, new Vector2(x, cy + 4), new Vector2(150, 150), new Color(accent.r, accent.g, accent.b, 0.08f), false);
            // 足元グロウ (横長楕円で接地感・行のリズム)
            CreateMenuSpriteImage("Hero Puck " + species, parent, circleSprite, new Vector2(x, cy - 56), new Vector2(112, 36), new Color(accent.r, accent.g, accent.b, 0.12f), false);
            // キャラ本体
            CreateMenuSpriteImage("Hero Partner " + species, parent, GetPartnerVariantSprite(0, 0, 0, species), new Vector2(x, cy), new Vector2(box, box), Color.white, true);
        }
    }

    void CreateMainMenuHeroVisual(Transform parent)
    {
        var hero = CreatePanel("Main Menu Hero Deck", parent, new Vector2(0, 8), new Vector2(820, 196), new Vector2(0.5f, 0.5f));
        var heroImage = hero.GetComponent<Image>();
        if (heroImage != null)
            heroImage.color = new Color(0.002f, 0.018f, 0.026f, 0.72f);
        var heroOutline = hero.GetComponent<Outline>();
        if (heroOutline != null)
        {
            heroOutline.effectColor = new Color(0.24f, 1f, 1f, 0.28f);
            heroOutline.effectDistance = new Vector2(1.4f, -1.4f);
        }

        CreateStretchImage("Hero Top Cyan Rail", hero.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(34, -18), new Vector2(-34, -14), new Color(0.2f, 0.94f, 1f, 0.32f));
        CreateStretchImage("Hero Bottom Gold Rail", hero.transform, Vector2.zero, new Vector2(1, 0), new Vector2(90, 18), new Vector2(-90, 22), new Color(1f, 0.76f, 0.18f, 0.22f));
        CreateStretchImage("Hero Center Wash", hero.transform, Vector2.zero, Vector2.one, new Vector2(180, 20), new Vector2(-180, -20), new Color(0.06f, 0.28f, 0.32f, 0.11f));

        if (UseTitleUiV2 && titleCoreEmblemSprite != null)
            CreateMenuSpriteImage("Hero Core Emblem V2", hero.transform, titleCoreEmblemSprite, new Vector2(-12, 0), new Vector2(210, 210), new Color(1f, 1f, 1f, 0.86f), true);

        CreateMenuSpriteImage("Hero Core Halo Outer", hero.transform, circleSprite, new Vector2(-12, 0), new Vector2(184, 184), new Color(0.18f, 0.95f, 1f, 0.10f), false);
        CreateMenuSpriteImage("Hero Core Halo Gold", hero.transform, circleSprite, new Vector2(-12, 0), new Vector2(140, 140), new Color(1f, 0.78f, 0.18f, 0.15f), false);
        CreateMenuSpriteImage("Hero Core Halo Inner", hero.transform, circleSprite, new Vector2(-12, 0), new Vector2(96, 96), new Color(0.28f, 1f, 0.94f, 0.12f), false);

        var cobalt = CreateMenuSpriteImage("Hero Cobalt Pup", hero.transform, GetPartnerVariantSprite(3, 1, 0, 1), new Vector2(-262, -2), new Vector2(190, 190), Color.white, true);
        if (cobalt != null)
            cobalt.rectTransform.rotation = Quaternion.Euler(0f, 0f, -3f);

        CreateMenuSpriteImage("Hero Data Egg", hero.transform, lanternSprite, new Vector2(-12, 3), new Vector2(112, 112), Color.white, true);

        CreateMainMenuRouteBadge(hero.transform, new Vector2(276, 52), "SPEED", GetPartnerVariantSprite(3, 1, 0, 1), new Color(0.24f, 0.9f, 1f));
        CreateMainMenuRouteBadge(hero.transform, new Vector2(276, -4), "POWER", GetPartnerVariantSprite(3, 2, 0, 1), new Color(1f, 0.68f, 0.22f));
        CreateMainMenuRouteBadge(hero.transform, new Vector2(276, -60), "GUARD", GetPartnerVariantSprite(3, 3, 0, 1), new Color(0.42f, 1f, 0.66f));
    }

    Image CreateMenuSpriteImage(string name, Transform parent, Sprite sprite, Vector2 position, Vector2 size, Color color, bool preserveAspect)
    {
        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.preserveAspect = preserveAspect;
        image.raycastTarget = false;
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return image;
    }

    void CreateMainMenuRouteBadge(Transform parent, Vector2 position, string label, Sprite sprite, Color accent)
    {
        var badge = CreatePanel("Menu Route " + label, parent, position, new Vector2(148, 44), new Vector2(0.5f, 0.5f));
        var badgeImage = badge.GetComponent<Image>();
        if (badgeImage != null)
            badgeImage.color = new Color(0.004f, 0.022f, 0.03f, 0.84f);
        var outline = badge.GetComponent<Outline>();
        if (outline != null)
            outline.effectColor = WithAlpha(accent, 0.42f);
        CreateStretchImage(label + " Rail", badge.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(12, -8), new Vector2(-48, -5), WithAlpha(accent, 0.44f));
        CreateMenuSpriteImage(label + " Icon", badge.transform, sprite, new Vector2(-52, -1), new Vector2(40, 40), Color.white, true);
        var text = CreateText(label + " Label", badge.transform, new Vector2(16, 4), TextAnchor.MiddleLeft, 14, WithAlpha(accent, 1f));
        text.text = label;
        text.fontStyle = FontStyle.Bold;
        text.rectTransform.sizeDelta = new Vector2(86, 18);
        var sub = CreateText(label + " Sub", badge.transform, new Vector2(16, -13), TextAnchor.MiddleLeft, 9, new Color(0.70f, 0.92f, 0.96f));
        sub.text = "L3 FORM";
        sub.rectTransform.sizeDelta = new Vector2(86, 12);
    }

    void CreateRunConfigPanel()
    {
        runConfigPanel = new GameObject("Run Config Screen", typeof(Image));
        runConfigPanel.transform.SetParent(canvas.transform, false);
        var back = runConfigPanel.GetComponent<Image>();
        back.color = new Color(0.002f, 0.012f, 0.02f, 0.99f);
        if (UseGeneratedBackgrounds && mainMenuBackgroundSprite != null)
            ApplyScreenBackgroundSprite(back, mainMenuBackgroundSprite, new Color(1f, 1f, 1f, 0.50f));
        StretchToParent(runConfigPanel);

        CreateStretchImage("RC Top Glow", runConfigPanel.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -118), Vector2.zero, new Color(0.08f, 0.76f, 1f, 0.13f));
        CreateStretchImage("RC Bottom Glow", runConfigPanel.transform, Vector2.zero, new Vector2(1, 0), Vector2.zero, new Vector2(0, 118), new Color(1f, 0.72f, 0.16f, 0.08f));

        var title = CreateText("RC Title", runConfigPanel.transform, new Vector2(0, 280), TextAnchor.MiddleCenter, 42, new Color(0.68f, 1f, 1f));
        title.text = "RUN SETUP";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(900, 54);

        var subtitle = CreateText("RC Subtitle", runConfigPanel.transform, new Vector2(0, 232), TextAnchor.MiddleCenter, 17, new Color(0.86f, 1f, 0.96f));
        subtitle.text = "ステージと難度を選んでランを開始";
        subtitle.rectTransform.sizeDelta = new Vector2(760, 26);

        // ── STAGE セクション ────────────────────────────────────────
        var stageHeader = CreateText("RC Stage Header", runConfigPanel.transform, new Vector2(0, 170), TextAnchor.MiddleCenter, 20, new Color(0.55f, 0.95f, 1f));
        stageHeader.text = "── STAGE ──";
        stageHeader.fontStyle = FontStyle.Bold;
        stageHeader.rectTransform.sizeDelta = new Vector2(400, 28);

        // 大垁ESTAGE チッチE(5個対応、各 180×08 + サムネ表示)
        const float bigChipW = 180f;
        const float bigChipH = 108f;
        const float bigChipGap = 14f;
        // 5チッチE 全幁E= 5*180 + 4*14 = 956, first center = -478 + 90 = -388
        const float bigStageFirstCenter = -388f;
        stageChipButtons.Clear();
        for (var s = 0; s <= MaxStageId; s++)
        {
            var sIndex = s;
            var chip = CreateWideButton("Stage Chip " + s, runConfigPanel.transform,
                new Vector2(bigStageFirstCenter + s * (bigChipW + bigChipGap), 65), new Vector2(bigChipW, bigChipH));
            // Stage 3/4 thumbnails may be missing; keep fallback colors distinct.
            Sprite thumb = null;
            Color fallbackTint = new Color(0.05f, 0.14f, 0.18f, 0.95f);
            if (s == 0) thumb = stageThumbArenaSprite;
            else if (s == 1) thumb = stageThumbLavaSprite;
            else if (s == 2) thumb = stageThumbBrokenCoreSprite;
            else if (s == 3) { thumb = stageThumbFrostSprite; fallbackTint = new Color(0.10f, 0.28f, 0.42f, 0.95f); }
            else if (s == 4) { thumb = stageThumbStormSprite; fallbackTint = new Color(0.22f, 0.12f, 0.34f, 0.95f); }
            if (thumb != null)
            {
                var bgImg = chip.GetComponent<Image>();
                bgImg.sprite = thumb;
                bgImg.preserveAspect = false;
                bgImg.type = Image.Type.Simple;
                bgImg.color = new Color(0.92f, 1f, 1f, 0.65f);
            }
            else
            {
                var bgImg = chip.GetComponent<Image>();
                if (bgImg != null) bgImg.color = fallbackTint;
            }
            var txt = chip.GetComponentInChildren<Text>();
            txt.text = s == 0 ? "S1\nLantern Field"
                    : s == 1 ? "S2\nLava Cache"
                    : s == 2 ? "S3\nBroken Core"
                    : s == 3 ? "S4\nFrost Vault"
                    : "S5\nStorm Spire";
            txt.fontSize = 16;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            var outline = txt.gameObject.GetComponent<Outline>();
            if (outline == null) outline = txt.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.92f);
            outline.effectDistance = new Vector2(1.6f, -1.6f);
            chip.onClick.RemoveAllListeners();
            chip.onClick.AddListener(() => SelectStage(sIndex));
            stageChipButtons.Add(chip);
        }

        // ── DANGER セクション ──────────────────────────────────────
        var dangerHeader = CreateText("RC Danger Header", runConfigPanel.transform, new Vector2(0, -40), TextAnchor.MiddleCenter, 20, new Color(1f, 0.78f, 0.28f));
        dangerHeader.text = "── DANGER LEVEL ──";
        dangerHeader.fontStyle = FontStyle.Bold;
        dangerHeader.rectTransform.sizeDelta = new Vector2(400, 28);

        // 大垁EDANGER チッチE6倁E(80×6)
        const float dChipW = 80f;
        const float dChipGap = 12f;
        // total = 6*80 + 5*12 = 540, first center = -540/2 + 80/2 = -230
        const float dFirstCenter = -230f;
        dangerChipButtons.Clear();
        for (var d = 0; d <= MaxDangerLevel; d++)
        {
            var dIndex = d;
            var chip = CreateWideButton("Danger Chip " + d, runConfigPanel.transform,
                new Vector2(dFirstCenter + d * (dChipW + dChipGap), -100), new Vector2(dChipW, 56));
            var txt = chip.GetComponentInChildren<Text>();
            txt.text = "D" + d;
            txt.fontSize = 22;
            txt.fontStyle = FontStyle.Bold;
            SetButtonSpriteV2(chip, btnDangerChipSprite);
            chip.onClick.RemoveAllListeners();
            chip.onClick.AddListener(() => SelectDangerLevel(dIndex));
            dangerChipButtons.Add(chip);
        }

        // 現在選択�E詳細
        stageSelectorLabel = CreateText("RC Selection Info", runConfigPanel.transform, new Vector2(0, -170), TextAnchor.MiddleCenter, 16, new Color(0.84f, 1f, 0.94f));
        stageSelectorLabel.rectTransform.sizeDelta = new Vector2(900, 26);
        stageSelectorLabel.horizontalOverflow = HorizontalWrapMode.Overflow;
        dangerSelectorLabel = stageSelectorLabel;

        // ── BACK / START ボタン ──────────────────────────────────
        var backBtn = CreateWideButton("RC Back Button", runConfigPanel.transform, new Vector2(-220, -260), new Vector2(220, 60));
        SetButtonSpriteV2(backBtn, btnRunBackSprite);
        var backText = backBtn.GetComponentInChildren<Text>();
        backText.text = "→BACK";
        backText.fontSize = 22;
        backText.fontStyle = FontStyle.Bold;
        backBtn.onClick.RemoveAllListeners();
        backBtn.onClick.AddListener(CloseRunConfig);

        var goBtn = CreateWideButton("RC Start Button", runConfigPanel.transform, new Vector2(220, -260), new Vector2(280, 60));
        SetButtonSpriteV2(goBtn, btnRunStartSprite);
        var goText = goBtn.GetComponentInChildren<Text>();
        goText.text = "▶ START RUN";
        goText.fontSize = 24;
        goText.fontStyle = FontStyle.Bold;
        goText.color = new Color(0.62f, 1f, 0.95f);
        goBtn.onClick.RemoveAllListeners();
        goBtn.onClick.AddListener(ConfirmRunConfig);

        var rcGuide = CreateText("RC Guide", runConfigPanel.transform, new Vector2(0, -322), TextAnchor.MiddleCenter, 13, new Color(0.72f, 0.92f, 1f));
        rcGuide.text = "Enter / Space で START    ESC で BACK";
        rcGuide.rectTransform.sizeDelta = new Vector2(760, 22);

        runConfigPanel.SetActive(false);
    }

    void OpenRunConfig()
    {
        if (runConfigPanel == null) return;
        if (IsTitleOverlayOpen()) return;
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        runConfigPanel.SetActive(true);
        runConfigPanel.transform.SetAsLastSibling();
        RefreshStageChips();
        RefreshDangerChips();
        PlaySfx("Pickup", 660f, 0.05f, 0.10f);
    }

    void CloseRunConfig()
    {
        if (runConfigPanel != null) runConfigPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        RefreshMainMenuSummary();
        RefreshSelectionInfoLine();
        PlaySfx("Pickup", 480f, 0.04f, 0.08f);
    }

    void ConfirmRunConfig()
    {
        if (runConfigPanel != null) runConfigPanel.SetActive(false);
        StartRun();
    }

    void RefreshMainMenuSummary()
    {
        if (menuBestText != null)
        {
            var clears = PlayerPrefs.GetInt("Clears", 0);
            menuBestText.text = "BEST " + bestWave + "/" + MaxWave
                + "   |   CLEARS " + clears
                + "   |   MISSIONS " + CountCompletedMissions() + "/" + BuildMissionDefinitions().Length
                + "   |   DANGER D" + dangerMaxCleared
                + "   |   STAGE S" + Mathf.Min(MaxStageId + 1, stageMaxUnlocked + 1) + "/" + (MaxStageId + 1)
                + "   |   COMBO " + CountDiscoveredCombos() + "/" + BuildEvolutionComboRecipes().Length;
        }
    }

    Image CreateStretchImage(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color color)
    {
        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        return image;
    }

    void StretchToParent(GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void CreateCodexPanel()
    {
        codexPanel = new GameObject("Codex Screen", typeof(Image));
        codexPanel.transform.SetParent(mainMenuPanel.transform, false);
        var image = codexPanel.GetComponent<Image>();
        image.color = new Color(0f, 0.006f, 0.012f, 0.92f);
        StretchToParent(codexPanel);

        CreateStretchImage("Codex Top Glow", codexPanel.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -110), Vector2.zero, new Color(0.1f, 0.85f, 1f, 0.13f));
        CreateStretchImage("Codex Bottom Glow", codexPanel.transform, Vector2.zero, new Vector2(1, 0), Vector2.zero, new Vector2(0, 110), new Color(0.74f, 1f, 0.28f, 0.08f));

        codexCards.Clear();
        codexCardIcons.Clear();
        codexCardSilhouettes.Clear();
        codexCardTitles.Clear();
        codexCardStatusTexts.Clear();
        codexCardDescTexts.Clear();
        codexCardKinds.Clear();
        codexCardKeys.Clear();
        codexCardDescriptions.Clear();
        codexCardAccents.Clear();

        var deck = CreatePanel("Codex Deck", codexPanel.transform, Vector2.zero, new Vector2(1040, 680), new Vector2(0.5f, 0.5f));
        SetPanelAccentColor(deck.transform, new Color(0.45f, 1f, 1f), new Color(0.74f, 1f, 0.28f));

        var title = CreateText("Codex Title", deck.transform, new Vector2(0, 316), TextAnchor.MiddleCenter, 32, new Color(0.72f, 1f, 1f));
        title.text = "相棒図鑑";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(900, 44);
        if (iconCoreEggSprite != null)
            CreateMenuSpriteImage("Codex Title Egg Icon", deck.transform, iconCoreEggSprite, new Vector2(-150, 316), new Vector2(40, 40), Color.white, true);

        var subtitle = CreateText("Codex Subtitle", deck.transform, new Vector2(0, 280), TextAnchor.MiddleCenter, 13, new Color(0.62f, 0.95f, 1f));
        subtitle.text = "発見した相棒・進化ルート・クロス進化を確認";
        subtitle.rectTransform.sizeDelta = new Vector2(900, 22);

        codexRecordText = CreateText("Codex Record", deck.transform, new Vector2(0, 254), TextAnchor.MiddleCenter, 14, new Color(1f, 0.88f, 0.38f));
        codexRecordText.fontStyle = FontStyle.Bold;
        codexRecordText.rectTransform.sizeDelta = new Vector2(760, 22);

        codexText = CreateText("Codex Text Legacy", deck.transform, Vector2.zero, TextAnchor.UpperLeft, 1, Color.clear);
        codexText.gameObject.SetActive(false);

        var scrollGo = new GameObject("Codex Scroll View", typeof(RectTransform), typeof(ScrollRect));
        scrollGo.transform.SetParent(deck.transform, false);
        var scrollViewRect = scrollGo.GetComponent<RectTransform>();
        scrollViewRect.anchorMin = scrollViewRect.anchorMax = new Vector2(0.5f, 0.5f);
        scrollViewRect.pivot = new Vector2(0.5f, 0.5f);
        scrollViewRect.anchoredPosition = new Vector2(0, -34);
        scrollViewRect.sizeDelta = new Vector2(930, 500);

        var viewportGo = new GameObject("Codex Viewport", typeof(Image), typeof(Mask));
        viewportGo.transform.SetParent(scrollGo.transform, false);
        var viewportImage = viewportGo.GetComponent<Image>();
        viewportImage.color = new Color(0f, 0f, 0f, 0.01f);
        viewportImage.raycastTarget = true;
        var viewportMask = viewportGo.GetComponent<Mask>();
        viewportMask.showMaskGraphic = false;
        var viewportRect = viewportGo.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;

        var contentGo = new GameObject("Codex Content", typeof(RectTransform));
        contentGo.transform.SetParent(viewportGo.transform, false);
        codexScrollContentRect = contentGo.GetComponent<RectTransform>();
        codexScrollContentRect.anchorMin = new Vector2(0, 1);
        codexScrollContentRect.anchorMax = new Vector2(1, 1);
        codexScrollContentRect.pivot = new Vector2(0.5f, 1f);
        codexScrollContentRect.anchoredPosition = Vector2.zero;
        codexScrollContentRect.sizeDelta = new Vector2(0, 830);

        codexScrollRect = scrollGo.GetComponent<ScrollRect>();
        codexScrollRect.viewport = viewportRect;
        codexScrollRect.content = codexScrollContentRect;
        codexScrollRect.horizontal = false;
        codexScrollRect.vertical = true;
        codexScrollRect.movementType = ScrollRect.MovementType.Clamped;
        codexScrollRect.scrollSensitivity = 38f;
        codexScrollRect.inertia = true;

        CreateCodexSection(codexScrollContentRect, "PARTNER", 320, new Color(0.32f, 0.95f, 1f),
            new[]
            {
                new CodexEntry("partner", "Cobalt Pup", "Partner_Cobalt Pup", "標準型。青狼の素体。攻撃と移動のバランスがよい。", new Color(0.32f, 0.86f, 1f), 1),
                new CodexEntry("partner", "Ember Drake", "Partner_Ember Drake", "弾幕型。橙竜の素体。手数で押す連射ビルド。", new Color(1f, 0.82f, 0.34f), 2),
                new CodexEntry("partner", "Sage Hare", "Partner_Sage Hare", "防衛型。緑の賢兎素体。コアを守る粘り強さがある。", new Color(0.48f, 1f, 0.6f), 3),
                new CodexEntry("partner", "Hex Cat", "Partner_Hex Cat", "連鎖型。紫魔猫の素体。チェインと残像追撃を軸にする。", new Color(0.86f, 0.5f, 1f), 4),
                new CodexEntry("partner", "Drift Fox", "Partner_Drift Fox", "回避型。桃色の狐素体。Phase回避を持ち高速で立ち回る。", new Color(1f, 0.46f, 0.76f), 5),
                new CodexEntry("partner", "Iron Bear", "Partner_Iron Bear", "装甲型。黒熊の素体。HPと接触反撃に優れた重装甲。", new Color(0.85f, 0.85f, 0.95f), 6),
                new CodexEntry("partner", "Wraith Lynx", "Partner_Wraith Lynx", "近接型。高速白狼。オーラと吸血で近距離を制圧。", new Color(0.78f, 0.95f, 1f), 7),
                new CodexEntry("partner", "Genesis Core", "Partner_Genesis Core", "全種解放型。4種リンクを内蔵した隠し総合素体。", new Color(1f, 0.95f, 0.32f), 8),
                new CodexEntry("partner", "Halo Caster", "Partner_Halo Caster", "ファンネル型。自律ビットが周回攻撃と離脱突撃を行う。", new Color(0.55f, 0.92f, 1f), 9),
                new CodexEntry("partner", "Pulse Hydra", "Partner_Pulse Hydra", "レーザー型。直線上の敵を全員貫通ダメージで焼く高火力型。", new Color(1f, 0.45f, 0.85f), 10),
                new CodexEntry("partner", "Solar Anchor", "Partner_Solar Anchor", "コア共鳴型。コア周りに常時ダメリング、近接で攻撃30%のTD特化。", new Color(1f, 0.86f, 0.28f), 11)
            });

        CreateCodexSection(codexScrollContentRect, "EVOLUTION ROUTE", 38, new Color(1f, 0.88f, 0.38f),
            new[]
            {
                new CodexEntry("route", "SPEED", "Route_SPEED", "連射・移動・貫通を伸ばす高速ルート。", new Color(0.32f, 1f, 1f), 1),
                new CodexEntry("route", "POWER", "Route_POWER", "一撃の力とバーストを伸ばす攻撃ルート。", new Color(1f, 0.58f, 0.16f), 2),
                new CodexEntry("route", "GUARD", "Route_GUARD", "耐久・防衛・回復を伸ばす守備ルート。", new Color(0.42f, 1f, 0.62f), 3)
            });

        CreateCodexSection(codexScrollContentRect, "CROSS EVOLVE", -116, new Color(1f, 0.42f, 0.96f),
            new[]
            {
                new CodexEntry("fusion", "Nova Aegis", "Fusion_Nova Aegis", "Nova + Bulwark。追撃と防衛を両立。", new Color(1f, 0.42f, 0.96f), 1),
                new CodexEntry("fusion", "Photon Siphon", "Fusion_Photon Siphon", "Nova + Siphon。回収と連射を連鎖。", new Color(0.74f, 1f, 0.32f), 2),
                new CodexEntry("fusion", "Core Bastion", "Fusion_Core Bastion", "Bulwark + Siphon。コア防衛に特化。", new Color(1f, 0.86f, 0.34f), 3),
                new CodexEntry("fusion", "Nova Phantom", "Fusion_Nova Phantom", "Nova + Phase。残像と高速連鎖で崩す。", new Color(0.38f, 0.9f, 1f), 4),
                new CodexEntry("fusion", "Aegis Drift", "Fusion_Aegis Drift", "Bulwark + Phase。反射機動防衛を両立。", new Color(0.78f, 0.56f, 1f), 5),
                new CodexEntry("fusion", "Photon Wraith", "Fusion_Photon Wraith", "Siphon + Phase。回収と回復で粘る。", new Color(0.42f, 1f, 0.78f), 6)
            });

        codexCloseButton = CreateWideButton("Codex Close Button", deck.transform, new Vector2(0, -322), new Vector2(240, 32));
        SetButtonSpriteV2(codexCloseButton, btnCloseSmallSprite);
        codexCloseButton.GetComponentInChildren<Text>().text = "CLOSE";
        codexCloseButton.onClick.RemoveAllListeners();
        codexCloseButton.onClick.AddListener(ToggleCodexPanel);
        codexPanel.SetActive(false);
    }

    sealed class MissionDefinition
    {
        public readonly string id;
        public readonly string title;
        public readonly string description;
        public readonly string reward;
        public readonly Color accent;
        public readonly Func<bool, bool> isComplete;

        public MissionDefinition(string id, string title, string description, string reward, Color accent, Func<bool, bool> isComplete)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.reward = reward;
            this.accent = accent;
            this.isComplete = isComplete;
        }
    }

    MissionDefinition[] BuildMissionDefinitions()
    {
        return new[]
        {
            new MissionDefinition("first_clear", "初回防衛成功", "Wave10を突破してプロトコルを完了する。", "報酬: ラン開始時 データチップ+10", new Color(0.55f, 1f, 0.78f), cleared => cleared),
            new MissionDefinition("speed_clear", "SPEED適性試験", "SPEEDルートでWave10を突破する。", "報酬: ラン開始時 移動3% / 連射3%", new Color(0.34f, 1f, 1f), cleared => cleared && formStyle == 1),
            new MissionDefinition("power_clear", "POWER火力試験", "POWERルートでWave10を突破する。", "報酬: ラン開始時 攻撃力+0.08", new Color(1f, 0.68f, 0.22f), cleared => cleared && formStyle == 2),
            new MissionDefinition("guard_clear", "GUARD防衛試験", "GUARDルートでWave10を突破する。", "報酬: ラン開始時 コアHP+2", new Color(0.48f, 1f, 0.55f), cleared => cleared && formStyle == 3),
            new MissionDefinition("first_fusion", "初クロス進化", "任意のクロス進化を1回成立させる。", "報酬: ラン開始時 回収範囲+5%", new Color(1f, 0.46f, 0.95f), cleared => fusionActive),
            new MissionDefinition("data_120", "データ収集班", "1ランでデータチップを120以上集める。", "報酬: リロール費用 30→25", new Color(0.76f, 1f, 0.34f), cleared => dataChips >= 120f),
            new MissionDefinition("core_keeper", "コアキーパー", "Wave10突破時にコアHPを60%以上残す。", "報酬: ラン開始時 自分HP+1", new Color(1f, 0.86f, 0.3f), cleared => cleared && lanternHp / Mathf.Max(1f, lanternMaxHp) >= 0.6f),
            new MissionDefinition("hunter_180", "ハンターログ", "1ランで敵を180体以上倒す。", "報酬: ラン開始時 バースト率+2%", new Color(0.95f, 0.56f, 1f), cleared => runEnemiesKilled >= 180)
        };
    }

    void CreateMissionBoardPanel()
    {
        missionBoardPanel = new GameObject("Mission Board Screen", typeof(Image));
        missionBoardPanel.transform.SetParent(mainMenuPanel.transform, false);
        var image = missionBoardPanel.GetComponent<Image>();
        image.color = new Color(0f, 0.006f, 0.012f, 0.93f);
        StretchToParent(missionBoardPanel);

        CreateStretchImage("Mission Top Glow", missionBoardPanel.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -110), Vector2.zero, new Color(0.74f, 1f, 0.32f, 0.1f));
        CreateStretchImage("Mission Bottom Glow", missionBoardPanel.transform, Vector2.zero, new Vector2(1, 0), Vector2.zero, new Vector2(0, 120), new Color(0.12f, 0.82f, 1f, 0.1f));

        var deck = CreatePanel("Mission Board Deck", missionBoardPanel.transform, Vector2.zero, new Vector2(920, 660), new Vector2(0.5f, 0.5f));
        SetPanelAccentColor(deck.transform, new Color(0.58f, 1f, 0.84f), new Color(1f, 0.86f, 0.3f));

        var title = CreateText("Mission Board Title", deck.transform, new Vector2(0, 304), TextAnchor.MiddleCenter, 32, new Color(0.72f, 1f, 0.92f));
        title.text = "MISSION BOARD";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(820, 44);

        missionRecordText = CreateText("Mission Record", deck.transform, new Vector2(0, 268), TextAnchor.MiddleCenter, 14, new Color(1f, 0.9f, 0.42f));
        missionRecordText.fontStyle = FontStyle.Bold;
        missionRecordText.rectTransform.sizeDelta = new Vector2(820, 24);

        var defs = BuildMissionDefinitions();
        for (var i = 0; i < defs.Length; i++)
        {
            var col = i % 2;
            var row = i / 2;
            var x = col == 0 ? -230 : 230;
            var y = 198 - row * 112;
            CreateMissionCard(deck.transform, defs[i], new Vector2(x, y), new Vector2(410, 92));
        }

        missionCloseButton = CreateWideButton("Mission Close Button", deck.transform, new Vector2(0, -304), new Vector2(260, 38));
        SetButtonSpriteV2(missionCloseButton, btnCloseSmallSprite);
        missionCloseButton.GetComponentInChildren<Text>().text = "CLOSE";
        missionCloseButton.onClick.RemoveAllListeners();
        missionCloseButton.onClick.AddListener(ToggleMissionBoardPanel);
        missionBoardPanel.SetActive(false);
    }

    void CreateMissionCard(Transform parent, MissionDefinition def, Vector2 position, Vector2 size)
    {
        var go = new GameObject("Mission Card " + def.id, typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = new Color(0.018f, 0.044f, 0.064f, 0.96f);
        var outline = go.AddComponent<Outline>();
        outline.effectColor = WithAlpha(def.accent, 0.38f);
        outline.effectDistance = new Vector2(1.2f, -1.2f);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var strip = new GameObject("Mission Strip", typeof(Image));
        strip.transform.SetParent(go.transform, false);
        var stripImage = strip.GetComponent<Image>();
        stripImage.color = WithAlpha(def.accent, 0.62f);
        stripImage.raycastTarget = false;
        var stripRect = strip.GetComponent<RectTransform>();
        stripRect.anchorMin = stripRect.anchorMax = new Vector2(0, 1);
        stripRect.pivot = new Vector2(0, 1);
        stripRect.anchoredPosition = new Vector2(10, -8);
        stripRect.sizeDelta = new Vector2(112, 3);

        var iconGo = new GameObject("Mission Icon", typeof(Image));
        iconGo.transform.SetParent(go.transform, false);
        var icon = iconGo.GetComponent<Image>();
        icon.sprite = diamondSprite;
        icon.color = def.accent;
        icon.raycastTarget = false;
        var iconRect = iconGo.GetComponent<RectTransform>();
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.pivot = new Vector2(0, 0.5f);
        iconRect.anchoredPosition = new Vector2(18, 0);
        iconRect.sizeDelta = new Vector2(34, 34);

        var title = CreateText("Mission Title", go.transform, new Vector2(64, -8), TextAnchor.UpperLeft, 16, new Color(0.92f, 1f, 1f));
        title.text = def.title;
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(250, 22);

        var status = CreateText("Mission Status", go.transform, new Vector2(306, -8), TextAnchor.UpperLeft, 12, def.accent);
        status.fontStyle = FontStyle.Bold;
        status.rectTransform.sizeDelta = new Vector2(86, 20);

        var desc = CreateText("Mission Desc", go.transform, new Vector2(64, -34), TextAnchor.UpperLeft, 11, new Color(0.82f, 0.96f, 1f));
        desc.text = def.description;
        desc.rectTransform.sizeDelta = new Vector2(318, 24);

        var reward = CreateText("Mission Reward", go.transform, new Vector2(64, -60), TextAnchor.UpperLeft, 11, new Color(1f, 0.9f, 0.42f));
        reward.text = def.reward;
        reward.rectTransform.sizeDelta = new Vector2(318, 22);

        missionCards.Add(go);
        missionCardTitles.Add(title);
        missionCardStatusTexts.Add(status);
        missionCardDescTexts.Add(desc);
        missionCardRewardTexts.Add(reward);
        missionCardIds.Add(def.id);
    }

    void RefreshMissionBoard()
    {
        var defs = BuildMissionDefinitions();
        var completedCount = CountCompletedMissions();
        if (missionRecordText != null)
            missionRecordText.text = "MISSION " + completedCount + " / " + defs.Length + "    " + GetMissionRankName(completedCount);

        for (var i = 0; i < missionCards.Count && i < defs.Length; i++)
        {
            var def = defs[i];
            var completed = IsMissionCompleted(def.id);
            var image = missionCards[i].GetComponent<Image>();
            if (image != null)
                image.color = completed ? new Color(0.025f, 0.07f, 0.085f, 0.98f) : new Color(0.012f, 0.022f, 0.03f, 0.96f);
            var outline = missionCards[i].GetComponent<Outline>();
            if (outline != null)
                outline.effectColor = completed ? WithAlpha(def.accent, 0.72f) : new Color(0.18f, 0.28f, 0.34f, 0.46f);
            if (missionCardTitles[i] != null)
                missionCardTitles[i].color = completed ? Color.Lerp(Color.white, def.accent, 0.22f) : new Color(0.68f, 0.8f, 0.84f);
            if (missionCardStatusTexts[i] != null)
            {
                missionCardStatusTexts[i].text = completed ? "CLEAR" : "OPEN";
                missionCardStatusTexts[i].color = completed ? new Color(0.55f, 1f, 0.78f) : new Color(0.55f, 0.68f, 0.72f);
            }
            if (missionCardDescTexts[i] != null)
                missionCardDescTexts[i].color = completed ? new Color(0.86f, 0.98f, 1f) : new Color(0.56f, 0.68f, 0.72f);
            if (missionCardRewardTexts[i] != null)
            {
                missionCardRewardTexts[i].text = completed ? def.reward + "  [取得済" : def.reward;
                missionCardRewardTexts[i].color = completed ? def.accent : new Color(0.82f, 0.72f, 0.42f);
            }
        }
    }

    int CountCompletedMissions()
    {
        var count = 0;
        var defs = BuildMissionDefinitions();
        for (var i = 0; i < defs.Length; i++)
        {
            if (IsMissionCompleted(defs[i].id))
                count++;
        }
        return count;
    }

    bool IsMissionCompleted(string id)
    {
        return PlayerPrefs.GetInt(GetMissionPrefKey(id), 0) == 1;
    }

    string GetMissionPrefKey(string id)
    {
        return "Mission_" + id;
    }

    string GetMissionRankName(int completed)
    {
        if (completed >= 8)
            return "解析ランク S";
        if (completed >= 6)
            return "解析ランク A";
        if (completed >= 4)
            return "解析ランク B";
        if (completed >= 2)
            return "解析ランク C";
        return "解析ランク D";
    }

    string BuildMissionSummaryText()
    {
        var defs = BuildMissionDefinitions();
        var completed = CountCompletedMissions();
        var nextTitle = "全ミッション完了";
        var nextDesc = "次は高難度ステージ追加の準備";
        for (var i = 0; i < defs.Length; i++)
        {
            if (IsMissionCompleted(defs[i].id))
                continue;
            nextTitle = defs[i].title;
            nextDesc = defs[i].description;
            break;
        }

        return "進捗 " + completed + " / " + defs.Length + "    " + GetMissionRankName(completed) + "\n" +
               "次の目標: " + nextTitle + "\n" + nextDesc + "\n\n" +
               BuildMetaRewardSummary();
    }

    string BuildMetaRewardSummary()
    {
        var parts = new List<string>();
        if (IsMissionCompleted("first_clear")) parts.Add("初期データ+10");
        if (IsMissionCompleted("speed_clear")) parts.Add("機動+連射");
        if (IsMissionCompleted("power_clear")) parts.Add("攻撃");
        if (IsMissionCompleted("guard_clear")) parts.Add("コアHP+");
        if (IsMissionCompleted("first_fusion")) parts.Add("回収+");
        if (IsMissionCompleted("data_120")) parts.Add("リロール25");
        if (IsMissionCompleted("core_keeper")) parts.Add("自分P+");
        if (IsMissionCompleted("hunter_180")) parts.Add("バースト");
        return parts.Count == 0 ? "Mission Boardで報酬を解放。" : "解放済み報酬: " + string.Join(" / ", parts);
    }

    int GetRerollCost()
    {
        return IsMissionCompleted("data_120") ? 25 : BaseRerollCost;
    }

    void ApplyMetaProgressionRewards()
    {
        var rewards = new List<string>();
        if (IsMissionCompleted("first_clear"))
        {
            dataChips += 10f;
            rewards.Add("DATA+10");
        }
        if (IsMissionCompleted("speed_clear"))
        {
            moveSpeed *= 1.03f;
            fireRate *= 1.03f;
            rewards.Add("SPEED+");
        }
        if (IsMissionCompleted("power_clear"))
        {
            bulletDamage += 0.08f;
            rewards.Add("ATK+");
        }
        if (IsMissionCompleted("guard_clear"))
        {
            lanternMaxHp += 2f;
            lanternHp += 2f;
            rewards.Add("CORE+");
        }
        if (IsMissionCompleted("first_fusion"))
        {
            pickupRange *= 1.05f;
            rewards.Add("PICK+");
        }
        if (IsMissionCompleted("core_keeper"))
        {
            playerMaxHp += 1f;
            playerHp += 1f;
            rewards.Add("HP+");
        }
        if (IsMissionCompleted("hunter_180"))
        {
            explodeChance += 0.02f;
            rewards.Add("BURST+");
        }

        if (rewards.Count > 0)
            AddEventLog("ARCHIVE REWARD: " + string.Join(" / ", rewards));
    }

    void CreateEvolutionTreePanel()
    {
        treeRoutePreviewIcons.Clear();
        treeCrossPreviewIcons.Clear();
        treePartnerCardImages.Clear();
        treePartnerCardOutlines.Clear();
        treePartnerBadgeTexts.Clear();
        evolutionTreeSelectedSpecies = Mathf.Clamp(partnerStyle > 0 ? partnerStyle : 1, 1, 11);

        evolutionTreePanel = new GameObject("Evolution Tree Screen", typeof(Image));
        evolutionTreePanel.transform.SetParent(mainMenuPanel.transform, false);
        var image = evolutionTreePanel.GetComponent<Image>();
        image.color = new Color(0f, 0.006f, 0.012f, 0.93f);
        StretchToParent(evolutionTreePanel);

        CreateStretchImage("Tree Top Glow", evolutionTreePanel.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -120), Vector2.zero, new Color(0.16f, 0.82f, 1f, 0.12f));
        CreateStretchImage("Tree Bottom Glow", evolutionTreePanel.transform, Vector2.zero, new Vector2(1, 0), Vector2.zero, new Vector2(0, 120), new Color(1f, 0.42f, 0.96f, 0.08f));

        var deck = CreatePanel("Evolution Tree Deck", evolutionTreePanel.transform, Vector2.zero, new Vector2(1040, 680), new Vector2(0.5f, 0.5f));
        SetPanelAccentColor(deck.transform, new Color(0.45f, 1f, 1f), new Color(1f, 0.45f, 0.92f));

        var title = CreateText("Evolution Tree Title", deck.transform, new Vector2(0, 316), TextAnchor.MiddleCenter, 32, new Color(0.7f, 1f, 1f));
        title.text = "進化ツリー";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(920, 44);

        var subtitle = CreateText("Evolution Tree Subtitle", deck.transform, new Vector2(0, 280), TextAnchor.MiddleCenter, 13, new Color(0.72f, 0.95f, 1f));
        subtitle.text = "BEAST LINKを素体に、SPEED / POWER / GUARDで成長し、リンク吸収でクロス進化する";
        subtitle.rectTransform.sizeDelta = new Vector2(940, 22);

        CreatePartnerPreviewStrip(deck.transform);
        treeSelectedPartnerText = CreateText("Tree Selected Partner", deck.transform, new Vector2(0, 156), TextAnchor.MiddleCenter, 12, new Color(0.82f, 1f, 0.96f));
        treeSelectedPartnerText.fontStyle = FontStyle.Bold;
        treeSelectedPartnerText.rectTransform.sizeDelta = new Vector2(760, 20);

        CreateRoutePreviewPanel(deck.transform, "SPEED", 1, new Vector2(-338, 0), new Color(0.32f, 1f, 1f), "選択中の素体が高速・連射・貫通型へ伸びる。");
        CreateRoutePreviewPanel(deck.transform, "POWER", 2, new Vector2(0, 0), new Color(1f, 0.66f, 0.22f), "選択中の素体が火力・爆発・重撃型へ伸びる。");
        CreateRoutePreviewPanel(deck.transform, "GUARD", 3, new Vector2(338, 0), new Color(0.52f, 1f, 0.56f), "選択中の素体が耐久・防衛・回復型へ伸びる。");
        CreateCrossPreviewPanel(deck.transform);
        RefreshEvolutionTreeRoutePreview(false);

        treeCloseButton = CreateWideButton("Evolution Tree Close Button", deck.transform, new Vector2(0, -322), new Vector2(240, 32));
        SetButtonSpriteV2(treeCloseButton, btnCloseSmallSprite);
        treeCloseButton.GetComponentInChildren<Text>().text = "CLOSE";
        treeCloseButton.onClick.RemoveAllListeners();
        treeCloseButton.onClick.AddListener(ToggleEvolutionTreePanel);
        evolutionTreePanel.SetActive(false);
    }

    void CreatePartnerPreviewStrip(Transform parent)
    {
        var header = CreateText("Tree Partner Header", parent, new Vector2(-480, 252), TextAnchor.MiddleLeft, 12, new Color(0.32f, 1f, 1f));
        header.text = "BEAST LINK 素体";
        header.fontStyle = FontStyle.Bold;
        header.rectTransform.pivot = new Vector2(0f, 0.5f);
        header.rectTransform.anchoredPosition = new Vector2(-480, 252);
        header.rectTransform.sizeDelta = new Vector2(180, 18);

        var names = new[] { "Cobalt", "Ember", "Sage", "Hex", "Drift", "Iron", "Wraith", "Genesis", "Halo", "Pulse", "Solar" };
        for (var i = 0; i < names.Length; i++)
        {
            var x = -440 + i * 88;
            var species = i + 1;
            var card = CreateTreeIconCard(parent, names[i], GetPartnerVariantSprite(0, 0, 0, species), new Vector2(x, 202), new Vector2(80, 56), GetPartnerAccentColor(species), true);
            var cardImage = card.GetComponent<Image>();
            var outline = card.GetComponent<Outline>();
            var button = card.AddComponent<Button>();
            SetButtonTint(button, cardImage.color, Color.Lerp(cardImage.color, GetPartnerAccentColor(species), 0.24f));
            var pickedSpecies = species;
            button.onClick.AddListener(() => SelectEvolutionTreeSpecies(pickedSpecies));

            var badge = CreateText("Tree Partner Route Badge", card.transform, new Vector2(0, 21), TextAnchor.MiddleCenter, 8, new Color(0.76f, 1f, 0.96f));
            badge.text = "S/P/G";
            badge.fontStyle = FontStyle.Bold;
            badge.resizeTextForBestFit = true;
            badge.resizeTextMinSize = 6;
            badge.resizeTextMaxSize = 8;
            badge.rectTransform.sizeDelta = new Vector2(68, 12);
            treePartnerCardImages.Add(cardImage);
            treePartnerCardOutlines.Add(outline);
            treePartnerBadgeTexts.Add(badge);
        }
    }

    void CreateRoutePreviewPanel(Transform parent, string routeName, int routeStyle, Vector2 position, Color accent, string desc)
    {
        var panel = CreatePanel("Tree Route " + routeName, parent, position, new Vector2(308, 244), new Vector2(0.5f, 0.5f));
        SetPanelAccentColor(panel.transform, accent, new Color(1f, 0.86f, 0.32f));
        var title = CreateText("Tree Route Title " + routeName, panel.transform, new Vector2(0, 96), TextAnchor.MiddleCenter, 18, accent);
        title.text = routeName;
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(250, 28);

        for (var stage = 0; stage <= 3; stage++)
        {
            var x = -111 + stage * 74;
            var label = stage == 0 ? "BASE" : "Lv" + (stage * 3);
            var card = CreateTreeIconCard(panel.transform, label, GetEvolutionTreePreviewSprite(stage, routeStyle, evolutionTreeSelectedSpecies), new Vector2(x, 34), new Vector2(64, 78), accent, true);
            var icon = card.transform.Find("Tree Icon Sprite")?.GetComponent<Image>();
            if (icon != null)
                treeRoutePreviewIcons.Add(icon);
        }

        var arrow = CreateText("Tree Route Arrow " + routeName, panel.transform, new Vector2(0, -20), TextAnchor.MiddleCenter, 12, new Color(0.8f, 1f, 0.96f));
        arrow.text = "Lv3  → Lv6  → Lv9";
        arrow.rectTransform.sizeDelta = new Vector2(250, 20);

        var body = CreateText("Tree Route Desc " + routeName, panel.transform, new Vector2(0, -70), TextAnchor.MiddleCenter, 12, new Color(0.84f, 0.98f, 1f));
        body.text = desc;
        body.rectTransform.sizeDelta = new Vector2(246, 54);
        body.resizeTextForBestFit = true;
        body.resizeTextMinSize = 9;
        body.resizeTextMaxSize = 12;
    }

    void CreateCrossPreviewPanel(Transform parent)
    {
        var panel = CreatePanel("Tree Cross Panel", parent, new Vector2(0, -214), new Vector2(940, 136), new Vector2(0.5f, 0.5f));
        SetPanelAccentColor(panel.transform, new Color(1f, 0.42f, 0.96f), new Color(0.42f, 1f, 1f));
        var title = CreateText("Tree Cross Title", panel.transform, new Vector2(-340, 48), TextAnchor.MiddleLeft, 14, new Color(1f, 0.56f, 1f));
        title.text = "CROSS EVOLVE";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(220, 22);

        var names = new[] { "Nova Aegis", "Photon Siphon", "Core Bastion", "Nova Phantom", "Aegis Drift", "Photon Wraith" };
        for (var i = 0; i < names.Length; i++)
        {
            var x = -390 + i * 156;
            var fusion = i + 1;
            var route = fusion == 3 || fusion == 5 ? 3 : fusion == 2 || fusion == 6 ? 1 : 2;
            var card = CreateTreeIconCard(panel.transform, names[i], GetPartnerVariantSprite(3, route, fusion, evolutionTreeSelectedSpecies), new Vector2(x, -12), new Vector2(136, 76), GetFusionAccentColor(fusion), PlayerPrefs.GetInt("Fusion_" + names[i], 0) == 1);
            var icon = card.transform.Find("Tree Icon Sprite")?.GetComponent<Image>();
            if (icon != null)
                treeCrossPreviewIcons.Add(icon);
        }
    }

    Sprite GetEvolutionTreePreviewSprite(int stage, int routeStyle, int species)
    {
        var previewVariant = stage == 0 ? 0 : Mathf.Clamp(stage, 1, 4);
        return GetPartnerVariantSprite(stage, routeStyle, 0, species, previewVariant);
    }

    GameObject CreateTreeIconCard(Transform parent, string label, Sprite sprite, Vector2 position, Vector2 size, Color accent, bool unlocked)
    {
        var go = new GameObject("Tree Icon " + label, typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = unlocked ? new Color(0.02f, 0.07f, 0.09f, 0.94f) : new Color(0.01f, 0.018f, 0.024f, 0.94f);
        var outline = go.AddComponent<Outline>();
        outline.effectColor = unlocked ? WithAlpha(accent, 0.46f) : new Color(0.16f, 0.24f, 0.28f, 0.46f);
        outline.effectDistance = new Vector2(1f, -1f);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var iconGo = new GameObject("Tree Icon Sprite", typeof(Image));
        iconGo.transform.SetParent(go.transform, false);
        var icon = iconGo.GetComponent<Image>();
        icon.sprite = sprite;
        icon.color = unlocked ? Color.white : new Color(0.05f, 0.08f, 0.1f, 0.95f);
        icon.preserveAspect = true;
        icon.raycastTarget = false;
        var iconRect = iconGo.GetComponent<RectTransform>();
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = new Vector2(0, size.y > 65f ? 10 : 7);
        iconRect.sizeDelta = new Vector2(Mathf.Min(68f, size.x - 12f), Mathf.Min(62f, size.y - 22f));

        var text = CreateText("Tree Icon Label", go.transform, new Vector2(0, -size.y * 0.5f + 12), TextAnchor.MiddleCenter, size.x > 80f ? 10 : 9, unlocked ? new Color(0.86f, 1f, 0.96f) : new Color(0.45f, 0.56f, 0.6f));
        text.text = unlocked ? label : "???";
        text.fontStyle = FontStyle.Bold;
        text.rectTransform.sizeDelta = new Vector2(size.x - 8, 18);
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 7;
        text.resizeTextMaxSize = size.x > 80f ? 10 : 9;
        return go;
    }

    void SelectEvolutionTreeSpecies(int species)
    {
        evolutionTreeSelectedSpecies = Mathf.Clamp(species, 1, 11);
        RefreshEvolutionTreeRoutePreview(true);
    }

    void RefreshEvolutionTreeRoutePreview(bool playSfx)
    {
        evolutionTreeSelectedSpecies = Mathf.Clamp(evolutionTreeSelectedSpecies, 1, 11);
        var speciesName = GetPartnerShortName(evolutionTreeSelectedSpecies);
        if (treeSelectedPartnerText != null)
            treeSelectedPartnerText.text = "選択中: " + speciesName + "    SPEED / POWER / GUARD すべて進化可能";

        for (var i = 0; i < treePartnerCardImages.Count; i++)
        {
            var selected = i + 1 == evolutionTreeSelectedSpecies;
            var accent = GetPartnerAccentColor(i + 1);
            if (treePartnerCardImages[i] != null)
                treePartnerCardImages[i].color = selected ? Color.Lerp(new Color(0.02f, 0.07f, 0.09f, 0.96f), accent, 0.18f) : new Color(0.02f, 0.07f, 0.09f, 0.94f);
            if (treePartnerCardOutlines[i] != null)
                treePartnerCardOutlines[i].effectColor = selected ? WithAlpha(Color.white, 0.9f) : WithAlpha(accent, 0.46f);
            if (treePartnerBadgeTexts[i] != null)
            {
                treePartnerBadgeTexts[i].text = selected ? "ACTIVE" : "S/P/G";
                treePartnerBadgeTexts[i].color = selected ? Color.white : new Color(0.76f, 1f, 0.96f);
            }
        }

        for (var route = 1; route <= 3; route++)
        {
            for (var stage = 0; stage <= 3; stage++)
            {
                var index = (route - 1) * 4 + stage;
                if (index >= 0 && index < treeRoutePreviewIcons.Count && treeRoutePreviewIcons[index] != null)
                    treeRoutePreviewIcons[index].sprite = GetEvolutionTreePreviewSprite(stage, route, evolutionTreeSelectedSpecies);
            }
        }

        for (var i = 0; i < treeCrossPreviewIcons.Count; i++)
        {
            var fusion = i + 1;
            var route = fusion == 3 || fusion == 5 ? 3 : fusion == 2 || fusion == 6 ? 1 : 2;
            if (treeCrossPreviewIcons[i] != null)
                treeCrossPreviewIcons[i].sprite = GetPartnerVariantSprite(3, route, fusion, evolutionTreeSelectedSpecies);
        }

        if (playSfx)
            PlaySfx("Pickup", 680f + evolutionTreeSelectedSpecies * 26f, 0.035f, 0.07f);
    }

    string GetPartnerShortName(int species)
    {
        switch (Mathf.Clamp(species, 1, 11))
        {
            case 2:
                return "Ember";
            case 3:
                return "Sage";
            case 4:
                return "Hex";
            case 5:
                return "Drift";
            case 6:
                return "Iron";
            case 7:
                return "Wraith";
            case 8:
                return "Genesis";
            case 9:
                return "Halo";
            case 10:
                return "Pulse";
            case 11:
                return "Solar";
            default:
                return "Cobalt";
        }
    }

    sealed class CodexEntry
    {
        public readonly string kind;
        public readonly string name;
        public readonly string prefKey;
        public readonly string description;
        public readonly Color accent;
        public readonly int variant;

        public CodexEntry(string kind, string name, string prefKey, string description, Color accent, int variant)
        {
            this.kind = kind;
            this.name = name;
            this.prefKey = prefKey;
            this.description = description;
            this.accent = accent;
            this.variant = variant;
        }
    }

    void CreateCodexSection(Transform parent, string header, float centerY, Color accent, CodexEntry[] entries)
    {
        var compact = entries.Length > 3;
        var wideGrid = entries.Length > 6;
        var columns = wideGrid ? 4 : 3;
        var headerOffset = compact ? 49f : 48f;
        var cardSize = wideGrid ? new Vector2(208, 72) : compact ? new Vector2(286, 76) : new Vector2(286, 82);
        var rowStep = wideGrid ? 78f : compact ? 84f : 0f;
        var startX = wideGrid ? -339f : -310f;
        var columnStep = wideGrid ? 226f : 310f;
        var headerText = CreateText("Codex Header " + header, parent, new Vector2(-440, centerY + headerOffset), TextAnchor.MiddleLeft, 14, accent);
        headerText.text = header;
        headerText.fontStyle = FontStyle.Bold;
        headerText.rectTransform.sizeDelta = new Vector2(360, 22);

        var headerLine = new GameObject("Codex Header Line " + header, typeof(Image));
        headerLine.transform.SetParent(parent, false);
        var headerLineImage = headerLine.GetComponent<Image>();
        headerLineImage.color = WithAlpha(accent, 0.36f);
        headerLineImage.raycastTarget = false;
        var headerLineRect = headerLine.GetComponent<RectTransform>();
        headerLineRect.anchorMin = headerLineRect.anchorMax = new Vector2(0.5f, 0.5f);
        headerLineRect.pivot = new Vector2(0, 0.5f);
        headerLineRect.anchoredPosition = new Vector2(-150, centerY + headerOffset);
        headerLineRect.sizeDelta = new Vector2(590, 2);

        for (var i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            var column = i % columns;
            var row = i / columns;
            var x = startX + column * columnStep;
            var y = centerY - row * rowStep;
            CreateCodexCard(parent, entry, new Vector2(x, y), cardSize);
        }
    }

    void CreateCodexCard(Transform parent, CodexEntry entry, Vector2 position, Vector2 size)
    {
        var go = new GameObject("Codex Card " + entry.name, typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = new Color(0.022f, 0.05f, 0.075f, 0.96f);
        var outline = go.AddComponent<Outline>();
        outline.effectColor = WithAlpha(entry.accent, 0.4f);
        outline.effectDistance = new Vector2(1.2f, -1.2f);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        var wideCard = size.x < 230f;
        var compact = size.y < 100f;
        var iconBackSize = wideCard ? 48f : compact ? 54f : 66f;
        var iconSize = wideCard ? 40f : compact ? 46f : 56f;
        var textX = wideCard ? 66f : compact ? 74f : 86f;

        var strip = new GameObject("Codex Card Strip", typeof(Image));
        strip.transform.SetParent(go.transform, false);
        var stripImage = strip.GetComponent<Image>();
        stripImage.color = WithAlpha(entry.accent, 0.55f);
        stripImage.raycastTarget = false;
        var stripRect = strip.GetComponent<RectTransform>();
        stripRect.anchorMin = stripRect.anchorMax = new Vector2(0, 1);
        stripRect.pivot = new Vector2(0, 1);
        stripRect.anchoredPosition = new Vector2(8, -8);
        stripRect.sizeDelta = new Vector2(84, 3);

        var iconBack = new GameObject("Codex Card Icon Back", typeof(Image));
        iconBack.transform.SetParent(go.transform, false);
        var iconBackImage = iconBack.GetComponent<Image>();
        iconBackImage.sprite = circleSprite;
        iconBackImage.color = new Color(0.05f, 0.12f, 0.16f, 0.95f);
        iconBackImage.raycastTarget = false;
        var iconBackRect = iconBack.GetComponent<RectTransform>();
        iconBackRect.anchorMin = iconBackRect.anchorMax = new Vector2(0, 0.5f);
        iconBackRect.pivot = new Vector2(0, 0.5f);
        iconBackRect.anchoredPosition = new Vector2(12, compact ? 0 : 12);
        iconBackRect.sizeDelta = new Vector2(iconBackSize, iconBackSize);

        var silhouetteGo = new GameObject("Codex Card Silhouette", typeof(Image));
        silhouetteGo.transform.SetParent(iconBack.transform, false);
        var silhouette = silhouetteGo.GetComponent<Image>();
        silhouette.sprite = GetCodexIconSprite(entry);
        silhouette.color = new Color(0.05f, 0.08f, 0.12f, 0.95f);
        silhouette.preserveAspect = true;
        silhouette.raycastTarget = false;
        var silhouetteRect = silhouetteGo.GetComponent<RectTransform>();
        silhouetteRect.anchorMin = silhouetteRect.anchorMax = new Vector2(0.5f, 0.5f);
        silhouetteRect.pivot = new Vector2(0.5f, 0.5f);
        silhouetteRect.anchoredPosition = Vector2.zero;
        silhouetteRect.sizeDelta = new Vector2(iconSize, iconSize);

        var iconGo = new GameObject("Codex Card Icon", typeof(Image));
        iconGo.transform.SetParent(iconBack.transform, false);
        var icon = iconGo.GetComponent<Image>();
        icon.sprite = silhouette.sprite;
        icon.color = Color.white;
        icon.preserveAspect = true;
        icon.raycastTarget = false;
        var iconRect = iconGo.GetComponent<RectTransform>();
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = Vector2.zero;
        iconRect.sizeDelta = new Vector2(iconSize, iconSize);

        var titleMaxSize = wideCard ? 12 : compact ? 13 : 15;
        var titleText = CreateText("Codex Card Title", go.transform, new Vector2(textX, compact ? -10 : -12), TextAnchor.UpperLeft, titleMaxSize, new Color(0.92f, 1f, 1f));
        titleText.text = entry.name;
        titleText.fontStyle = FontStyle.Bold;
        titleText.rectTransform.sizeDelta = new Vector2(size.x - textX - 10, compact ? 20 : 24);
        titleText.resizeTextForBestFit = true;
        titleText.resizeTextMinSize = wideCard ? 8 : compact ? 10 : 11;
        titleText.resizeTextMaxSize = titleMaxSize;

        var statusText = CreateText("Codex Card Status", go.transform, new Vector2(textX, compact ? -30 : -36), TextAnchor.UpperLeft, wideCard ? 9 : compact ? 10 : 12, entry.accent);
        statusText.fontStyle = FontStyle.Bold;
        statusText.rectTransform.sizeDelta = new Vector2(size.x - textX - 10, 18);

        var descText = CreateText("Codex Card Desc", go.transform, new Vector2(textX, compact ? -49 : -58), TextAnchor.UpperLeft, wideCard ? 8 : compact ? 9 : 11, new Color(0.84f, 0.96f, 1f));
        descText.rectTransform.sizeDelta = new Vector2(size.x - textX - 10, compact ? 24 : 38);
        descText.alignment = TextAnchor.UpperLeft;
        descText.resizeTextForBestFit = true;
        descText.resizeTextMinSize = wideCard ? 6 : compact ? 7 : 9;
        descText.resizeTextMaxSize = wideCard ? 8 : compact ? 9 : 11;

        codexCards.Add(go);
        codexCardIcons.Add(icon);
        codexCardSilhouettes.Add(silhouette);
        codexCardTitles.Add(titleText);
        codexCardStatusTexts.Add(statusText);
        codexCardDescTexts.Add(descText);
        codexCardKinds.Add(entry.kind);
        codexCardKeys.Add(entry.prefKey);
        codexCardDescriptions.Add(entry.description);
        codexCardAccents.Add(entry.accent);
    }

    Sprite GetCodexIconSprite(CodexEntry entry)
    {
        if (entry.kind == "partner")
            return GetPartnerVariantSprite(0, 0, 0, entry.variant);
        if (entry.kind == "route")
            return GetPartnerVariantSprite(3, entry.variant, 0, partnerStyle > 0 ? partnerStyle : 1, 1);
        if (entry.kind == "fusion")
            return GetPartnerVariantSprite(3, formStyle > 0 ? formStyle : 1, entry.variant, partnerStyle > 0 ? partnerStyle : 1, evolutionVariantStyle);
        return GetPartnerVariantSprite(0, 0, 0, 1);
    }

    void RefreshCodexCards()
    {
        if (codexRecordText != null)
        {
            var clears = PlayerPrefs.GetInt("Clears", 0);
            codexRecordText.text = "BEST WAVE " + bestWave + " / " + MaxWave + "    CLEARS " + clears;
        }

        for (var i = 0; i < codexCards.Count; i++)
        {
            var key = codexCardKeys[i];
            var unlocked = PlayerPrefs.GetInt(key, 0) == 1;
            var accent = i < codexCardAccents.Count ? codexCardAccents[i] : new Color(0.55f, 1f, 0.9f);
            var image = codexCards[i].GetComponent<Image>();
            if (image != null)
                image.color = unlocked ? new Color(0.025f, 0.07f, 0.1f, 0.97f) : new Color(0.012f, 0.022f, 0.03f, 0.96f);
            var outline = codexCards[i].GetComponent<Outline>();
            if (outline != null)
                outline.effectColor = unlocked
                    ? WithAlpha(accent, 0.68f)
                    : new Color(0.18f, 0.28f, 0.34f, 0.46f);
            if (codexCardTitles[i] != null)
                codexCardTitles[i].color = unlocked ? Color.Lerp(Color.white, accent, 0.18f) : new Color(0.55f, 0.66f, 0.7f);
            if (codexCardIcons[i] != null)
            {
                codexCardIcons[i].gameObject.SetActive(unlocked);
                codexCardIcons[i].color = Color.white;
            }
            if (codexCardSilhouettes[i] != null)
                codexCardSilhouettes[i].color = unlocked ? new Color(0f, 0f, 0f, 0f) : new Color(0.05f, 0.08f, 0.12f, 0.95f);
            if (codexCardStatusTexts[i] != null)
            {
                codexCardStatusTexts[i].text = unlocked ? "[OPEN]" : "[LOCKED]";
                codexCardStatusTexts[i].color = unlocked ? Color.Lerp(accent, new Color(0.55f, 1f, 0.78f), 0.35f) : new Color(0.55f, 0.65f, 0.7f);
            }
            if (codexCardDescTexts[i] != null)
            {
                var description = i < codexCardDescriptions.Count ? codexCardDescriptions[i] : GetCodexDescriptionFor(i);
                codexCardDescTexts[i].text = unlocked ? description : "まだ未発見。プレイ中に選ぶと記録される。";
                codexCardDescTexts[i].color = unlocked ? new Color(0.86f, 0.98f, 1f) : new Color(0.55f, 0.68f, 0.75f);
            }
        }
    }

    string GetCodexDescriptionFor(int index)
    {
        var kind = codexCardKinds[index];
        var key = codexCardKeys[index];
        if (kind == "partner")
        {
            if (key.EndsWith("Cobalt Pup")) return "標準型。青狼の素体。攻撃と移動のバランスがよい。";
            if (key.EndsWith("Ember Drake")) return "弾幕型。橙竜の素体。手数で押す連射ビルド。";
            if (key.EndsWith("Sage Hare")) return "防衛型。緑の賢兎素体。コアを守る持久型。";
            if (key.EndsWith("Hex Cat")) return "連鎖型。紫魔猫の素体。チェインと残像追撃を軸にする。";
            if (key.EndsWith("Drift Fox")) return "回避型。桃色の狐素体。Phase回避を持ち高速で立ち回る。";
            if (key.EndsWith("Iron Bear")) return "装甲型。黒熊の素体。HPと接触反撃に優れた重装甲。";
            if (key.EndsWith("Wraith Lynx")) return "近接型。高速白狼。オーラと吸血で近距離を制圧。";
            if (key.EndsWith("Genesis Core")) return "全種解放型。4種リンクを内蔵した隠し総合素体。";
        }
        else if (kind == "route")
        {
            if (key.EndsWith("SPEED")) return "連射・移動・貫通を伸ばす高速ルート。";
            if (key.EndsWith("POWER")) return "一撃の力とバーストを伸ばす攻撃ルート。";
            if (key.EndsWith("GUARD")) return "耐久・防衛・回復を伸ばす守備ルート。";
        }
        else if (kind == "fusion")
        {
            if (key.EndsWith("Nova Aegis")) return "Nova + Bulwark。追撃と防衛を両立。";
            if (key.EndsWith("Photon Siphon")) return "Nova + Siphon。回収と連射を連鎖。";
            if (key.EndsWith("Core Bastion")) return "Bulwark + Siphon。コア防衛に特化。";
            if (key.EndsWith("Nova Phantom")) return "Nova + Phase。残像と高速連鎖で崩す。";
            if (key.EndsWith("Aegis Drift")) return "Bulwark + Phase。反射機動防衛を両立。";
            if (key.EndsWith("Photon Wraith")) return "Siphon + Phase。回収と回復で粘る。";
        }
        return "";
    }

    void CreatePausePanel()
    {
        pausePanel = CreatePanel("Pause Panel", canvas.transform, new Vector2(0, -12), new Vector2(460, 350), new Vector2(0.5f, 0.5f));
        var panelImage = pausePanel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.color = new Color(0.006f, 0.018f, 0.032f, 0.97f);
        SetPanelAccentColor(pausePanel.transform, new Color(0.32f, 1f, 1f), new Color(1f, 0.82f, 0.26f));

        var title = CreateText("Pause Title", pausePanel.transform, new Vector2(0, 112), TextAnchor.MiddleCenter, 32, new Color(0.72f, 1f, 1f));
        title.text = "PAUSED";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(410, 48);

        resumeButton = CreateWideButton("Resume Button", pausePanel.transform, new Vector2(0, 42), new Vector2(320, 48));
        SetButtonSpriteV2(resumeButton, btnPauseResumeSprite);
        resumeButton.GetComponentInChildren<Text>().text = "RESUME";
        resumeButton.onClick.RemoveAllListeners();
        resumeButton.onClick.AddListener(() => SetPaused(false));

        pauseOptionsButton = CreateWideButton("Pause Options Button", pausePanel.transform, new Vector2(0, -18), new Vector2(320, 42));
        SetButtonSpriteV2(pauseOptionsButton, btnWide320x42Sprite);
        pauseOptionsButton.GetComponentInChildren<Text>().text = "OPTIONS";
        pauseOptionsButton.onClick.RemoveAllListeners();
        pauseOptionsButton.onClick.AddListener(ToggleOptionsPanel);

        restartButton = CreateWideButton("Restart Button", pausePanel.transform, new Vector2(0, -76), new Vector2(320, 42));
        SetButtonSpriteV2(restartButton, btnWide320x42Sprite);
        restartButton.GetComponentInChildren<Text>().text = "RESTART RUN";
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(() =>
        {
            ReloadRuntime(autoStartRun: true);
        });

        var guide = CreateText("Pause Guide", pausePanel.transform, new Vector2(0, -138), TextAnchor.MiddleCenter, 14, new Color(0.75f, 0.94f, 1f));
        guide.text = "Esc / P で再開    O でオプション";
        guide.rectTransform.sizeDelta = new Vector2(410, 28);
        pausePanel.SetActive(false);
    }

    void CreateOptionsPanel()
    {
        // Compact 2-column layout. Gameplay difficulty is intentionally kept out of pause options.
        optionsPanel = CreatePanel("Options Panel", canvas.transform, Vector2.zero, new Vector2(600, 600), new Vector2(0.5f, 0.5f));
        var panelImage = optionsPanel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.color = new Color(0.004f, 0.016f, 0.027f, 0.97f);
        SetPanelAccentColor(optionsPanel.transform, new Color(0.32f, 1f, 1f), new Color(0.7f, 1f, 0.28f));

        var title = CreateText("Options Title", optionsPanel.transform, new Vector2(0, 268), TextAnchor.MiddleCenter, 24, new Color(0.72f, 1f, 1f));
        title.text = "OPTIONS";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(500, 36);

        // ── Two-column toggle grid ─────────────────────
        const float toggleColX = 135f;
        const float toggleTopY = 220f;
        const float toggleRowGap = 36f;
        CreateOptionButton("HUD 詳細",       new Vector2(-toggleColX, toggleTopY - 0 * toggleRowGap), () => showAdvancedHud, value => showAdvancedHud = value);
        CreateOptionButton("イベントログ",   new Vector2( toggleColX, toggleTopY - 0 * toggleRowGap), () => showEventLogPanel, value => showEventLogPanel = value);
        CreateOptionButton("操作表示",       new Vector2(-toggleColX, toggleTopY - 1 * toggleRowGap), () => showControlHelp, value => showControlHelp = value);
        CreateOptionButton("強化ビジュアル", new Vector2( toggleColX, toggleTopY - 1 * toggleRowGap), () => enhancedVisuals, value => enhancedVisuals = value);
        CreateOptionButton("画面揺れ",       new Vector2(-toggleColX, toggleTopY - 2 * toggleRowGap), () => screenShakeEnabled, value => screenShakeEnabled = value);
        CreateOptionButton("画面フラッシュ", new Vector2( toggleColX, toggleTopY - 2 * toggleRowGap), () => screenFlashEnabled, value => screenFlashEnabled = value);
        CreateOptionButton("ヒットフリーズ", new Vector2(-toggleColX, toggleTopY - 3 * toggleRowGap), () => hitFreezeEnabled, value => hitFreezeEnabled = value);
        CreateOptionButton("BGM",            new Vector2( toggleColX, toggleTopY - 3 * toggleRowGap), () => bgmEnabled, value => bgmEnabled = value);
        CreateOptionButton("SE",             new Vector2(-toggleColX, toggleTopY - 4 * toggleRowGap), () => sfxEnabled, value => sfxEnabled = value);
        CreateOptionButton("ダメージ数値",    new Vector2( toggleColX, toggleTopY - 4 * toggleRowGap), () => showDamageNumbers, value => showDamageNumbers = value);

        var audioHeader = CreateText("Audio Header", optionsPanel.transform, new Vector2(0, 30), TextAnchor.MiddleCenter, 14, new Color(0.92f, 1f, 0.74f));
        audioHeader.text = "── AUDIO MIX ──";
        audioHeader.fontStyle = FontStyle.Bold;
        audioHeader.rectTransform.sizeDelta = new Vector2(520, 20);
        CreateVolumeSlider("BGM 音量", new Vector2(0, 4), () => bgmVolume, v => bgmVolume = v);
        CreateVolumeSlider("SE 音量",  new Vector2(0, -26), () => sfxVolume, v => sfxVolume = v);

        // ── Scale controls (single column, 4 items) ──
        const float scaleTopY = -72f;
        const float scaleRowGap = 38f;
        CreateScaleControl("HP バー サイズ",   new Vector2(0, scaleTopY - 0 * scaleRowGap), () => hudHpScale, v => hudHpScale = v);
        CreateScaleControl("EXP バー サイズ",  new Vector2(0, scaleTopY - 1 * scaleRowGap), () => hudExpScale, v => hudExpScale = v);
        CreateScaleControl("WAVE バー サイズ", new Vector2(0, scaleTopY - 2 * scaleRowGap), () => hudWaveScale, v => hudWaveScale = v);
        CreateScaleControl("カメラ ズーム",    new Vector2(0, scaleTopY - 3 * scaleRowGap), () => cameraZoom, v => { cameraZoom = v; ApplyCameraZoom(); });

        var closeButton = CreateWideButton("Options Close Button", optionsPanel.transform, new Vector2(0, -258), new Vector2(220, 36));
        SetButtonSpriteV2(closeButton, btnCloseSmallSprite);
        closeButton.GetComponentInChildren<Text>().text = "CLOSE";
        closeButton.GetComponentInChildren<Text>().fontSize = 16;
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(ToggleOptionsPanel);

        optionsPanel.SetActive(false);
        UpdateOptionButtons();
    }

    void CreateScaleControl(string label, Vector2 position, Func<float> getter, Action<float> setter)
    {
        // [Label : 1.00x]  [−]  [+]     Ecompact +/∁Econtrol row
        var labelText = CreateText(label + " Text", optionsPanel.transform, new Vector2(-46, position.y), TextAnchor.MiddleLeft, 13, new Color(0.78f, 1f, 1f));
        labelText.text = label + " : " + getter().ToString("0.00") + "x";
        labelText.fontStyle = FontStyle.Bold;
        labelText.rectTransform.sizeDelta = new Vector2(244, 30);

        var minus = CreateWideButton(label + " Minus", optionsPanel.transform, new Vector2(102, position.y), new Vector2(44, 30));
        SetButtonSpriteV2(minus, btnStepperSprite);
        minus.GetComponentInChildren<Text>().text = "-";
        minus.onClick.RemoveAllListeners();
        minus.onClick.AddListener(() =>
        {
            var v = getter(); CycleHudScale(ref v, -1); setter(v);
            labelText.text = label + " : " + v.ToString("0.00") + "x";
            ApplyHudScales(); SaveOptions();
            PlaySfx("Pickup", 520f, 0.04f, 0.08f);
        });

        var plus = CreateWideButton(label + " Plus", optionsPanel.transform, new Vector2(154, position.y), new Vector2(44, 30));
        SetButtonSpriteV2(plus, btnStepperSprite);
        plus.GetComponentInChildren<Text>().text = "+";
        plus.onClick.RemoveAllListeners();
        plus.onClick.AddListener(() =>
        {
            var v = getter(); CycleHudScale(ref v, 1); setter(v);
            labelText.text = label + " : " + v.ToString("0.00") + "x";
            ApplyHudScales(); SaveOptions();
            PlaySfx("Pickup", 720f, 0.04f, 0.08f);
        });
    }

    void CreateVolumeSlider(string label, Vector2 position, Func<float> getter, Action<float> setter)
    {
        var labelText = CreateText(label + " Text", optionsPanel.transform, new Vector2(-198, position.y), TextAnchor.MiddleLeft, 13, new Color(0.78f, 1f, 1f));
        labelText.fontStyle = FontStyle.Bold;
        labelText.rectTransform.sizeDelta = new Vector2(170, 24);
        labelText.text = label + " : " + Mathf.RoundToInt(getter() * 100f) + "%";

        var root = new GameObject(label + " Slider", typeof(RectTransform), typeof(Slider));
        root.transform.SetParent(optionsPanel.transform, false);
        var rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = new Vector2(92, position.y);
        rootRect.sizeDelta = new Vector2(300, 22);

        var background = new GameObject("Background", typeof(Image));
        background.transform.SetParent(root.transform, false);
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.color = new Color(0.015f, 0.045f, 0.055f, 0.96f);
        backgroundImage.raycastTarget = true;
        var backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0, 0.5f);
        backgroundRect.anchorMax = new Vector2(1, 0.5f);
        backgroundRect.pivot = new Vector2(0.5f, 0.5f);
        backgroundRect.anchoredPosition = Vector2.zero;
        backgroundRect.sizeDelta = new Vector2(0, 10);

        var fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(root.transform, false);
        var fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.5f);
        fillAreaRect.anchorMax = new Vector2(1, 0.5f);
        fillAreaRect.pivot = new Vector2(0.5f, 0.5f);
        fillAreaRect.anchoredPosition = Vector2.zero;
        fillAreaRect.sizeDelta = new Vector2(-22, 10);

        var fill = new GameObject("Fill", typeof(Image));
        fill.transform.SetParent(fillArea.transform, false);
        var fillImage = fill.GetComponent<Image>();
        fillImage.color = new Color(0.36f, 1f, 0.92f, 0.9f);
        fillImage.raycastTarget = false;
        var fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(root.transform, false);
        var handleAreaRect = handleArea.GetComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(10, 0);
        handleAreaRect.offsetMax = new Vector2(-10, 0);

        var handle = new GameObject("Handle", typeof(Image));
        handle.transform.SetParent(handleArea.transform, false);
        var handleImage = handle.GetComponent<Image>();
        handleImage.sprite = circleSprite;
        handleImage.color = new Color(0.78f, 1f, 0.96f, 1f);
        handleImage.raycastTarget = true;
        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(22, 22);

        var slider = root.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.direction = Slider.Direction.LeftToRight;
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.value = Mathf.Clamp01(getter());
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(value =>
        {
            var clamped = Mathf.Clamp01(value);
            setter(clamped);
            labelText.text = label + " : " + Mathf.RoundToInt(clamped * 100f) + "%";
            ApplyOptions();
            SaveOptions();
        });
    }

    // Accessibility 用: AccessibilityPresets を循環 (0.5/0.75/1.0/1.25/1.5)
    void CycleAccessibility(ref float current, int direction)
    {
        var idx = 2; // default 1.0
        for (var i = 0; i < AccessibilityPresets.Length; i++)
            if (Mathf.Abs(AccessibilityPresets[i] - current) < 0.01f) { idx = i; break; }
        idx = Mathf.Clamp(idx + direction, 0, AccessibilityPresets.Length - 1);
        current = AccessibilityPresets[idx];
    }

    Text CreateAccessibilityControl(string label, Vector2 position, Func<float> getter, Action<float> setter)
    {
        var labelText = CreateText(label + " Acc Text", optionsPanel.transform, new Vector2(-46, position.y), TextAnchor.MiddleLeft, 13, new Color(0.92f, 1f, 0.78f));
        labelText.text = label + " : " + FormatAccessibilityValue(getter());
        labelText.fontStyle = FontStyle.Bold;
        labelText.rectTransform.sizeDelta = new Vector2(244, 30);

        var minus = CreateWideButton(label + " Acc Minus", optionsPanel.transform, new Vector2(102, position.y), new Vector2(44, 30));
        minus.GetComponentInChildren<Text>().text = "-";
        minus.onClick.RemoveAllListeners();
        minus.onClick.AddListener(() =>
        {
            var v = getter(); CycleAccessibility(ref v, -1); setter(v);
            labelText.text = label + " : " + FormatAccessibilityValue(v);
            SaveOptions();
            PlaySfx("Pickup", 520f, 0.04f, 0.08f);
        });

        var plus = CreateWideButton(label + " Acc Plus", optionsPanel.transform, new Vector2(154, position.y), new Vector2(44, 30));
        plus.GetComponentInChildren<Text>().text = "+";
        plus.onClick.RemoveAllListeners();
        plus.onClick.AddListener(() =>
        {
            var v = getter(); CycleAccessibility(ref v, 1); setter(v);
            labelText.text = label + " : " + FormatAccessibilityValue(v);
            SaveOptions();
            PlaySfx("Pickup", 720f, 0.04f, 0.08f);
        });
        return labelText;
    }

    string FormatAccessibilityValue(float v)
    {
        var pct = Mathf.RoundToInt(v * 100f);
        var tag = v < 0.99f ? "  (緩和)" : v > 1.01f ? "  (挑戦)" : "  (標準)";
        return pct + "%" + tag;
    }

    Button CreateOptionButton(string label, Vector2 position, Func<bool> getter, Action<bool> setter)
    {
        // 2-column layout: each button is 250 wide so two fit at ±135 with ~10px gap
        var button = CreateWideButton("Option " + label, optionsPanel.transform, position, new Vector2(250, 36));
        var text = button.GetComponentInChildren<Text>();
        text.fontSize = 15;
        text.fontStyle = FontStyle.Bold;
        optionButtons.Add(new OptionButton
        {
            button = button,
            label = text,
            name = label,
            getter = getter,
            setter = setter
        });
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            setter(!getter());
            UpdateOptionButtons();
            SaveOptions();
            ApplyOptions();
            PlaySfx("Pickup", 560f, 0.04f, 0.08f);
        });
        return button;
    }

    void ToggleOptionsPanel()
    {
        if (optionsPanel == null)
            return;

        var next = !optionsPanel.activeSelf;
        if (next && choosingRelic)
            return;
        if (next && !titleScreen && !paused && !choosingUpgrade)
            SetPaused(true);
        if (next && codexPanel != null)
            codexPanel.SetActive(false);
        if (next && missionBoardPanel != null)
            missionBoardPanel.SetActive(false);
        if (next && evolutionTreePanel != null)
            evolutionTreePanel.SetActive(false);
        optionsPanel.SetActive(next);
        if (next)
            optionsPanel.transform.SetAsLastSibling();
        UpdateOptionButtons();
        ApplyOptions();
    }

    bool IsTitleOverlayOpen()
    {
        return optionsPanel != null && optionsPanel.activeSelf
            || codexPanel != null && codexPanel.activeSelf
            || missionBoardPanel != null && missionBoardPanel.activeSelf
            || evolutionTreePanel != null && evolutionTreePanel.activeSelf
            || evolutionComboCodexPanel != null && evolutionComboCodexPanel.activeSelf;
    }

    void ToggleCodexPanel()
    {
        if (codexPanel == null || !titleScreen)
            return;

        var next = !codexPanel.activeSelf;
        if (next && optionsPanel != null)
            optionsPanel.SetActive(false);
        if (next && missionBoardPanel != null)
            missionBoardPanel.SetActive(false);
        if (next && evolutionTreePanel != null)
            evolutionTreePanel.SetActive(false);
        if (next && evolutionComboCodexPanel != null)
            evolutionComboCodexPanel.SetActive(false);
        if (next)
            RefreshCodexCards();
        codexPanel.SetActive(next);
        if (next && codexScrollRect != null)
            codexScrollRect.verticalNormalizedPosition = 1f;
        PlaySfx("Pickup", next ? 640f : 420f, 0.04f, 0.08f);
    }

    void ToggleMissionBoardPanel()
    {
        if (missionBoardPanel == null || !titleScreen)
            return;

        var next = !missionBoardPanel.activeSelf;
        if (next && optionsPanel != null)
            optionsPanel.SetActive(false);
        if (next && codexPanel != null)
            codexPanel.SetActive(false);
        if (next && evolutionTreePanel != null)
            evolutionTreePanel.SetActive(false);
        if (next && evolutionComboCodexPanel != null)
            evolutionComboCodexPanel.SetActive(false);
        if (next)
            RefreshMissionBoard();
        missionBoardPanel.SetActive(next);
        PlaySfx("Pickup", next ? 700f : 420f, 0.04f, 0.08f);
    }

    void ToggleEvolutionTreePanel()
    {
        if (evolutionTreePanel == null || !titleScreen)
            return;

        var next = !evolutionTreePanel.activeSelf;
        if (next && optionsPanel != null)
            optionsPanel.SetActive(false);
        if (next && codexPanel != null)
            codexPanel.SetActive(false);
        if (next && missionBoardPanel != null)
            missionBoardPanel.SetActive(false);
        if (next && evolutionComboCodexPanel != null)
            evolutionComboCodexPanel.SetActive(false);
        evolutionTreePanel.SetActive(next);
        PlaySfx("Pickup", next ? 760f : 420f, 0.04f, 0.08f);
    }

    void ToggleEvolutionComboCodexPanel()
    {
        if (evolutionComboCodexPanel == null || !titleScreen)
            return;

        var next = !evolutionComboCodexPanel.activeSelf;
        if (next && optionsPanel != null)
            optionsPanel.SetActive(false);
        if (next && codexPanel != null)
            codexPanel.SetActive(false);
        if (next && missionBoardPanel != null)
            missionBoardPanel.SetActive(false);
        if (next && evolutionTreePanel != null)
            evolutionTreePanel.SetActive(false);
        if (next)
            RefreshEvolutionComboCodex();
        evolutionComboCodexPanel.SetActive(next);
        PlaySfx("Pickup", next ? 720f : 420f, 0.04f, 0.08f);
    }

    void CreateEvolutionComboCodexPanel()
    {
        evolutionComboCodexPanel = new GameObject("Evolution Combo Codex Screen", typeof(Image));
        evolutionComboCodexPanel.transform.SetParent(mainMenuPanel.transform, false);
        var image = evolutionComboCodexPanel.GetComponent<Image>();
        image.color = new Color(0f, 0.006f, 0.012f, 0.94f);
        StretchToParent(evolutionComboCodexPanel);

        CreateStretchImage("Combo Top Glow", evolutionComboCodexPanel.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -110), Vector2.zero, new Color(1f, 0.78f, 0.32f, 0.13f));
        CreateStretchImage("Combo Bottom Glow", evolutionComboCodexPanel.transform, Vector2.zero, new Vector2(1, 0), Vector2.zero, new Vector2(0, 110), new Color(1f, 0.42f, 0.92f, 0.08f));

        var deck = CreatePanel("Combo Codex Deck", evolutionComboCodexPanel.transform, Vector2.zero, new Vector2(1040, 680), new Vector2(0.5f, 0.5f));
        SetPanelAccentColor(deck.transform, new Color(1f, 0.92f, 0.42f), new Color(1f, 0.45f, 0.92f));

        var title = CreateText("Combo Codex Title", deck.transform, new Vector2(0, 304), TextAnchor.MiddleCenter, 32, new Color(1f, 0.92f, 0.42f));
        title.text = "進化コンボ図鑑";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(960, 44);

        var subtitle = CreateText("Combo Codex Subtitle", deck.transform, new Vector2(0, 268), TextAnchor.MiddleCenter, 13, new Color(0.86f, 1f, 0.96f));
        subtitle.text = "発見した組み合わせのみ詳細表示 / 自分で発見するまでネタバレなし";
        subtitle.rectTransform.sizeDelta = new Vector2(960, 22);

        evolutionComboCodexProgressText = CreateText("Combo Codex Progress", deck.transform, new Vector2(0, 240), TextAnchor.MiddleCenter, 14, new Color(1f, 0.86f, 0.36f));
        evolutionComboCodexProgressText.fontStyle = FontStyle.Bold;
        evolutionComboCodexProgressText.rectTransform.sizeDelta = new Vector2(760, 22);

        // 8コンボを 2衁E×4刁Eで配置 (cardW=232, cardH=180, gap=18)
        var recipes = BuildEvolutionComboRecipes();
        evolutionComboCodexCards.Clear();
        evolutionComboCodexTitles.Clear();
        evolutionComboCodexReqsTexts.Clear();
        evolutionComboCodexDescTexts.Clear();
        evolutionComboCodexOutlines.Clear();
        const float cardW = 230f;
        const float cardH = 188f;
        const float colStep = 246f;
        const float rowStep = 200f;
        const int cols = 4;
        var firstX = -((cols - 1) * colStep) * 0.5f;
        var firstY = 92f;
        for (var i = 0; i < recipes.Length; i++)
        {
            var col = i % cols;
            var row = i / cols;
            var x = firstX + col * colStep;
            var y = firstY - row * rowStep;
            CreateComboCodexCard(deck.transform, recipes[i], new Vector2(x, y), new Vector2(cardW, cardH));
        }

        var closeBtn = CreateWideButton("Combo Codex Close Button", deck.transform, new Vector2(0, -304), new Vector2(260, 38));
        closeBtn.GetComponentInChildren<Text>().text = "CLOSE";
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(ToggleEvolutionComboCodexPanel);

        evolutionComboCodexPanel.SetActive(false);
    }

    void CreateComboCodexCard(Transform parent, EvolutionComboRecipe recipe, Vector2 position, Vector2 size)
    {
        var go = new GameObject("Combo Codex Card " + recipe.key, typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = new Color(0.022f, 0.05f, 0.075f, 0.96f);
        var outline = go.AddComponent<Outline>();
        outline.effectColor = WithAlpha(recipe.accent, 0.45f);
        outline.effectDistance = new Vector2(1.4f, -1.4f);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        evolutionComboCodexCards.Add(go);
        evolutionComboCodexOutlines.Add(outline);

        // 上部アクセントストリチE�E
        var strip = new GameObject("Combo Strip", typeof(Image));
        strip.transform.SetParent(go.transform, false);
        var stripImage = strip.GetComponent<Image>();
        stripImage.color = WithAlpha(recipe.accent, 0.62f);
        stripImage.raycastTarget = false;
        var stripRect = strip.GetComponent<RectTransform>();
        stripRect.anchorMin = new Vector2(0, 1);
        stripRect.anchorMax = new Vector2(1, 1);
        stripRect.pivot = new Vector2(0.5f, 1);
        stripRect.offsetMin = new Vector2(10, -8);
        stripRect.offsetMax = new Vector2(-10, -5);

        // タイトル (発見済みなら名前、未発見なめE???)
        var titleText = CreateText("Combo Card Title", go.transform, new Vector2(0, 60), TextAnchor.MiddleCenter, 17, recipe.accent);
        titleText.fontStyle = FontStyle.Bold;
        titleText.rectTransform.sizeDelta = new Vector2(size.x - 20, 26);
        evolutionComboCodexTitles.Add(titleText);

        // 忁E��モジュール一覧
        var reqsText = CreateText("Combo Card Reqs", go.transform, new Vector2(0, 18), TextAnchor.MiddleCenter, 12, new Color(0.78f, 0.94f, 1f));
        reqsText.rectTransform.sizeDelta = new Vector2(size.x - 20, 58);
        reqsText.horizontalOverflow = HorizontalWrapMode.Wrap;
        reqsText.verticalOverflow = VerticalWrapMode.Truncate;
        evolutionComboCodexReqsTexts.Add(reqsText);

        var descText = CreateText("Combo Card Desc", go.transform, new Vector2(0, -52), TextAnchor.MiddleCenter, 11, new Color(0.86f, 1f, 0.96f));
        descText.rectTransform.sizeDelta = new Vector2(size.x - 20, 52);
        descText.horizontalOverflow = HorizontalWrapMode.Wrap;
        descText.verticalOverflow = VerticalWrapMode.Truncate;
        evolutionComboCodexDescTexts.Add(descText);
    }

    void RefreshEvolutionComboCodex()
    {
        var recipes = BuildEvolutionComboRecipes();
        var discovered = CountDiscoveredCombos();
        if (evolutionComboCodexProgressText != null)
            evolutionComboCodexProgressText.text = "DISCOVERED  " + discovered + " / " + recipes.Length;
        for (var i = 0; i < recipes.Length && i < evolutionComboCodexCards.Count; i++)
        {
            var recipe = recipes[i];
            var unlocked = IsComboDiscovered(recipe.key);
            var card = evolutionComboCodexCards[i];
            if (card != null)
            {
                var img = card.GetComponent<Image>();
                if (img != null)
                    img.color = unlocked ? new Color(0.030f, 0.075f, 0.10f, 0.98f) : new Color(0.010f, 0.020f, 0.028f, 0.96f);
            }
            if (i < evolutionComboCodexOutlines.Count && evolutionComboCodexOutlines[i] != null)
                evolutionComboCodexOutlines[i].effectColor = unlocked
                    ? WithAlpha(recipe.accent, 0.78f)
                    : new Color(0.18f, 0.28f, 0.34f, 0.45f);
            if (i < evolutionComboCodexTitles.Count && evolutionComboCodexTitles[i] != null)
            {
                evolutionComboCodexTitles[i].text = unlocked ? recipe.displayName : "??? ??? ???";
                evolutionComboCodexTitles[i].color = unlocked ? recipe.accent : new Color(0.45f, 0.55f, 0.60f);
            }
            if (i < evolutionComboCodexReqsTexts.Count && evolutionComboCodexReqsTexts[i] != null)
            {
                if (unlocked)
                    evolutionComboCodexReqsTexts[i].text = string.Join("\n+ ", recipe.requirements).Insert(0, "+ ");
                else
                {
                    var hidden = "";
                    for (var j = 0; j < recipe.requirements.Length; j++)
                        hidden += (j == 0 ? "+ " : "\n+ ") + "???";
                    evolutionComboCodexReqsTexts[i].text = hidden;
                }
                evolutionComboCodexReqsTexts[i].color = unlocked ? new Color(0.86f, 1f, 0.96f) : new Color(0.42f, 0.52f, 0.58f);
            }
            if (i < evolutionComboCodexDescTexts.Count && evolutionComboCodexDescTexts[i] != null)
            {
                evolutionComboCodexDescTexts[i].text = unlocked
                    ? recipe.description
                    : "プレイ中に対応モジュールを揃えると発見・記録される。";
                evolutionComboCodexDescTexts[i].color = unlocked ? new Color(0.92f, 1f, 0.96f) : new Color(0.50f, 0.58f, 0.62f);
                evolutionComboCodexDescTexts[i].fontStyle = unlocked ? FontStyle.Normal : FontStyle.Italic;
            }
        }
    }

    string BuildCodexText()
    {
        var clears = PlayerPrefs.GetInt("Clears", 0);
        var text = "RUN RECORD\n";
        text += "Best Wave : " + bestWave + " / " + MaxWave + "\n";
        text += "Clears    : " + clears + "\n\n";
        text += "PARTNER\n";
        text += CodexLine("Cobalt Pup", PlayerPrefs.GetInt("Partner_Cobalt Pup", 0) == 1, "標準型。青狼の素体。バランス型。");
        text += CodexLine("Ember Drake", PlayerPrefs.GetInt("Partner_Ember Drake", 0) == 1, "弾幕型。橙竜の素体。連射と弾数。");
        text += CodexLine("Sage Hare", PlayerPrefs.GetInt("Partner_Sage Hare", 0) == 1, "防衛型。緑の賢兎素体。コア防衛特化。");
        text += CodexLine("Hex Cat", PlayerPrefs.GetInt("Partner_Hex Cat", 0) == 1, "連鎖型。紫魔猫の素体。チェインと残像追撃。");
        text += CodexLine("Drift Fox", PlayerPrefs.GetInt("Partner_Drift Fox", 0) == 1, "回避型。桃色の狐素体。Phase回避高速。");
        text += CodexLine("Iron Bear", PlayerPrefs.GetInt("Partner_Iron Bear", 0) == 1, "装甲型。黒熊の素体。HPと反撃の重装甲。");
        text += CodexLine("Wraith Lynx", PlayerPrefs.GetInt("Partner_Wraith Lynx", 0) == 1, "近接型。高速白狼。オーラと吸血で近距離制圧。");
        text += CodexLine("Genesis Core", PlayerPrefs.GetInt("Partner_Genesis Core", 0) == 1, "全種解放型。4種リンクを内蔵した隠し総合素体。");
        text += "\nEVOLUTION ROUTE\n";
        text += CodexLine("SPEED", PlayerPrefs.GetInt("Route_SPEED", 0) == 1, "連射・移動・貫通を伸ばす高速ルート。");
        text += CodexLine("POWER", PlayerPrefs.GetInt("Route_POWER", 0) == 1, "一撃の力とバーストを伸ばす攻撃ルート。");
        text += CodexLine("GUARD", PlayerPrefs.GetInt("Route_GUARD", 0) == 1, "耐久・防衛・回復を伸ばす守備ルート。");
        text += "\nCROSS EVOLVE\n";
        text += CodexLine("Nova Aegis", PlayerPrefs.GetInt("Fusion_Nova Aegis", 0) == 1, "Nova + Bulwark。追撃と防衛を両立する融合。");
        text += CodexLine("Photon Siphon", PlayerPrefs.GetInt("Fusion_Photon Siphon", 0) == 1, "Nova + Siphon。回収と連射を連鎖させる融合。");
        text += CodexLine("Core Bastion", PlayerPrefs.GetInt("Fusion_Core Bastion", 0) == 1, "Bulwark + Siphon。コア防衛に特化する融合。");
        text += CodexLine("Nova Phantom", PlayerPrefs.GetInt("Fusion_Nova Phantom", 0) == 1, "Nova + Phase。残像と高速連鎖で崩す融合。");
        text += CodexLine("Aegis Drift", PlayerPrefs.GetInt("Fusion_Aegis Drift", 0) == 1, "Bulwark + Phase。反射機動防衛を両立する融合。");
        text += CodexLine("Photon Wraith", PlayerPrefs.GetInt("Fusion_Photon Wraith", 0) == 1, "Siphon + Phase。回収と回復で粘る融合。");
        return text;
    }

    string CodexLine(string name, bool unlocked, string description)
    {
        return (unlocked ? "[OPEN] " : "[LOCK] ") + name + "\n  " + (unlocked ? description : "まだ未発見。プレイ中に選ぶと登録される。") + "\n";
    }

    void UpdateOptionButtons()
    {
        foreach (var option in optionButtons)
        {
            if (option == null || option.label == null)
                continue;

            var on = option.getter();
            option.label.text = option.name + " : " + (on ? "ON" : "OFF");
            var image = option.button != null ? option.button.GetComponent<Image>() : null;
            if (image != null)
                image.color = on ? new Color(0.045f, 0.16f, 0.18f, 0.96f) : new Color(0.035f, 0.045f, 0.055f, 0.94f);
        }
    }

    bool GetOptionPref(string key, bool defaultValue)
    {
        return PlayerPrefs.GetInt(OptionPrefPrefix + key, defaultValue ? 1 : 0) == 1;
    }

    void SetOptionPref(string key, bool value)
    {
        PlayerPrefs.SetInt(OptionPrefPrefix + key, value ? 1 : 0);
    }

    void LoadOptions()
    {
        showAdvancedHud = GetOptionPref("AdvancedHud", showAdvancedHud);
        showEventLogPanel = GetOptionPref("EventLog", showEventLogPanel);
        showControlHelp = GetOptionPref("ControlHelp", showControlHelp);
        enhancedVisuals = GetOptionPref("EnhancedVisuals", enhancedVisuals);
        screenShakeEnabled = GetOptionPref("ScreenShake", screenShakeEnabled);
        screenFlashEnabled = GetOptionPref("ScreenFlash", screenFlashEnabled);
        bgmEnabled = GetOptionPref("Bgm", bgmEnabled);
        sfxEnabled = GetOptionPref("Sfx", sfxEnabled);
        hitFreezeEnabled = GetOptionPref("HitFreeze", hitFreezeEnabled);
        showDamageNumbers = GetOptionPref("DamageNumbers", showDamageNumbers);
        bgmVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(OptionPrefPrefix + "BgmVolume", bgmVolume));
        sfxVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(OptionPrefPrefix + "SfxVolume", sfxVolume));
        hudHpScale = Mathf.Clamp(PlayerPrefs.GetFloat("HudHpScale", 1f), 0.5f, 2.0f);
        hudExpScale = Mathf.Clamp(PlayerPrefs.GetFloat("HudExpScale", 1f), 0.5f, 2.0f);
        hudWaveScale = Mathf.Clamp(PlayerPrefs.GetFloat("HudWaveScale", 1f), 0.5f, 2.0f);
        cameraZoom = Mathf.Clamp(PlayerPrefs.GetFloat("CameraZoom", 1.0f), 0.7f, 1.75f);
        accessibilityEnemyHpMul = 1f;
        accessibilityEnemyDmgMul = 1f;
        accessibilityEnemySpdMul = 1f;
    }

    void SaveOptions()
    {
        SetOptionPref("AdvancedHud", showAdvancedHud);
        SetOptionPref("EventLog", showEventLogPanel);
        SetOptionPref("ControlHelp", showControlHelp);
        SetOptionPref("EnhancedVisuals", enhancedVisuals);
        SetOptionPref("ScreenShake", screenShakeEnabled);
        SetOptionPref("ScreenFlash", screenFlashEnabled);
        SetOptionPref("Bgm", bgmEnabled);
        SetOptionPref("Sfx", sfxEnabled);
        SetOptionPref("HitFreeze", hitFreezeEnabled);
        SetOptionPref("DamageNumbers", showDamageNumbers);
        PlayerPrefs.SetFloat(OptionPrefPrefix + "BgmVolume", bgmVolume);
        PlayerPrefs.SetFloat(OptionPrefPrefix + "SfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("HudHpScale", hudHpScale);
        PlayerPrefs.SetFloat("HudExpScale", hudExpScale);
        PlayerPrefs.SetFloat("HudWaveScale", hudWaveScale);
        PlayerPrefs.SetFloat("CameraZoom", cameraZoom);
        PlayerPrefs.SetFloat(AccessibilityHpKey, 1f);
        PlayerPrefs.SetFloat(AccessibilityDmgKey, 1f);
        PlayerPrefs.SetFloat(AccessibilitySpdKey, 1f);
        PlayerPrefs.Save();
    }

    void ApplyHudScales()
    {
        // Use localScale on the BAR BACK (which contains fill+lag as children).
        // This scales the bar visually as a group without affecting label/value text positions.
        if (playerHpBarBackRect != null)
            playerHpBarBackRect.localScale = new Vector3(hudHpScale, hudHpScale, 1f);
        if (eggHpBarBackRect != null)
            eggHpBarBackRect.localScale = new Vector3(hudHpScale, hudHpScale, 1f);
        if (expBarBackRect != null)
            expBarBackRect.localScale = new Vector3(hudExpScale, hudExpScale, 1f);
        if (waveProgressBackRect != null)
            waveProgressBackRect.localScale = new Vector3(hudWaveScale, hudWaveScale, 1f);
    }

    void CycleHudScale(ref float current, int direction)
    {
        var idx = 2; // default 1.0x index
        for (var i = 0; i < HudScalePresets.Length; i++)
            if (Mathf.Abs(HudScalePresets[i] - current) < 0.01f) { idx = i; break; }
        idx = Mathf.Clamp(idx + direction, 0, HudScalePresets.Length - 1);
        current = HudScalePresets[idx];
    }

    void ApplyOptions()
    {
        var focusChoice = choosingUpgrade || choiceFocusOverlay != null && choiceFocusOverlay.activeSelf;
        if (statsPanelRoot != null)
            statsPanelRoot.SetActive(showAdvancedHud && !titleScreen && !focusChoice);
        if (eventLogPanelRoot != null)
            eventLogPanelRoot.SetActive(showEventLogPanel && !titleScreen && !focusChoice);
        if (dataPanelRoot != null)
            dataPanelRoot.SetActive(false);
        if (chipHudRoot != null)
            chipHudRoot.SetActive(!titleScreen && !focusChoice);
        // ACTIVE BUILD はビルド理解の根幹なので常時表示 (showAdvancedHud ゲートを外しぁE
        // 詳細な COMBAT 統計と LINKS パネルは引き続き advanced HUD 専用
        if (loadoutPanelRoot != null)
            loadoutPanelRoot.SetActive(!titleScreen && !focusChoice);
        if (statusPanelRoot != null)
            statusPanelRoot.SetActive(showAdvancedHud && !titleScreen && !focusChoice);
        // ── タイトル中は core HUD (wave / HP / level / wave-progress) もすべて隠ぁE──
        if (wavePanelRoot != null)
            wavePanelRoot.SetActive(!titleScreen && !focusChoice);
        if (hpPanelRoot != null)
            hpPanelRoot.SetActive(!titleScreen && !focusChoice);
        if (levelPanelRoot != null)
            levelPanelRoot.SetActive(!titleScreen && !focusChoice);
        if (waveProgressRoot != null)
            waveProgressRoot.SetActive(!titleScreen && !focusChoice);
        if (controlText != null)
            controlText.gameObject.SetActive((showControlHelp || paused) && !focusChoice && !titleScreen);

        ApplyHudScales();
        ApplyCameraZoom();

        foreach (var renderer in dropShadowRenderers)
        {
            if (renderer != null)
                renderer.gameObject.SetActive(enhancedVisuals);
        }

        foreach (var renderer in enhancedVisualRenderers)
        {
            if (renderer != null)
                renderer.gameObject.SetActive(enhancedVisuals);
        }

        if (bgmSource != null)
        {
            bgmSource.volume = bgmEnabled ? 0.18f * Mathf.Clamp01(bgmVolume) : 0f;
            if (!bgmEnabled && bgmSource.isPlaying)
                bgmSource.Pause();
            else if (bgmEnabled && !titleScreen && !bgmSource.isPlaying)
                PlayBgm();
        }

        if (sfxSource != null)
            sfxSource.volume = sfxEnabled ? 0.35f * Mathf.Clamp01(sfxVolume) : 0f;
    }

    // ── Boss cutscene: dramatic intro before boss spawn + victory finish on kill ──
    void CreateBossCutscene()
    {
        bossCutscenePanel = new GameObject("Boss Cutscene", typeof(Image));
        bossCutscenePanel.transform.SetParent(canvas.transform, false);
        bossCutsceneVignette = bossCutscenePanel.GetComponent<Image>();
        bossCutsceneVignette.color = Color.clear;
        bossCutsceneVignette.raycastTarget = false;
        var rect = bossCutscenePanel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;

        var backgroundGo = new GameObject("Boss Cutscene Background", typeof(Image));
        backgroundGo.transform.SetParent(bossCutscenePanel.transform, false);
        bossCutsceneBackgroundImage = backgroundGo.GetComponent<Image>();
        bossCutsceneBackgroundImage.color = Color.clear;
        bossCutsceneBackgroundImage.raycastTarget = false;
        StretchToParent(backgroundGo);

        // Diagonal slashing bars (top + bottom)  Eanime-style impact bars
        bossCutsceneSlashTop = MakeBossCutsceneSlash("Boss Slash Top", new Vector2(0, 1), new Vector2(0, 0));
        bossCutsceneSlashBottom = MakeBossCutsceneSlash("Boss Slash Bot", new Vector2(0, 0), new Vector2(0, 0));

        // Portrait (boss sprite shown large in center)
        var portraitGo = new GameObject("Boss Portrait", typeof(Image));
        portraitGo.transform.SetParent(bossCutscenePanel.transform, false);
        bossCutscenePortrait = portraitGo.GetComponent<Image>();
        bossCutscenePortrait.color = Color.clear;
        bossCutscenePortrait.preserveAspect = true;
        bossCutscenePortrait.raycastTarget = false;
        var pRect = bossCutscenePortrait.rectTransform;
        pRect.anchorMin = pRect.anchorMax = new Vector2(0.5f, 0.5f);
        pRect.pivot = new Vector2(0.5f, 0.5f);
        pRect.anchoredPosition = new Vector2(0, 30);
        pRect.sizeDelta = new Vector2(280, 280);

        // WARNING ribbon
        bossCutsceneWarning = CreateText("Boss Warning", bossCutscenePanel.transform, Vector2.zero, TextAnchor.MiddleCenter, 56, new Color(1f, 0.18f, 0.18f, 0));
        bossCutsceneWarning.text = "⚠ WARNING ⚠";
        bossCutsceneWarning.fontStyle = FontStyle.Bold;
        var wRect = bossCutsceneWarning.rectTransform;
        wRect.anchorMin = wRect.anchorMax = new Vector2(0.5f, 0.5f);
        wRect.pivot = new Vector2(0.5f, 0.5f);
        wRect.anchoredPosition = new Vector2(0, 200);
        wRect.sizeDelta = new Vector2(900, 80);

        // Boss name big text
        bossCutsceneNameText = CreateText("Boss Name", bossCutscenePanel.transform, Vector2.zero, TextAnchor.MiddleCenter, 72, new Color(1f, 0.92f, 0.4f, 0));
        bossCutsceneNameText.text = "";
        bossCutsceneNameText.fontStyle = FontStyle.Bold;
        var nRect = bossCutsceneNameText.rectTransform;
        nRect.anchorMin = nRect.anchorMax = new Vector2(0.5f, 0.5f);
        nRect.pivot = new Vector2(0.5f, 0.5f);
        nRect.anchoredPosition = new Vector2(0, -160);
        nRect.sizeDelta = new Vector2(1000, 100);

        // Sub text (protocol / wave info)
        bossCutsceneSubText = CreateText("Boss Sub", bossCutscenePanel.transform, Vector2.zero, TextAnchor.MiddleCenter, 22, new Color(0.85f, 1f, 1f, 0));
        bossCutsceneSubText.text = "";
        bossCutsceneSubText.fontStyle = FontStyle.Bold;
        var sRect = bossCutsceneSubText.rectTransform;
        sRect.anchorMin = sRect.anchorMax = new Vector2(0.5f, 0.5f);
        sRect.pivot = new Vector2(0.5f, 0.5f);
        sRect.anchoredPosition = new Vector2(0, -230);
        sRect.sizeDelta = new Vector2(900, 40);

        bossCutscenePanel.SetActive(false);
    }

    Image MakeBossCutsceneSlash(string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(bossCutscenePanel.transform, false);
        var img = go.GetComponent<Image>();
        img.color = Color.clear;
        if (UseGeneratedOverlayImages && hudBossWarningBannerSprite != null)
            img.sprite = hudBossWarningBannerSprite;
        img.raycastTarget = false;
        var r = go.GetComponent<RectTransform>();
        r.anchorMin = anchorMin; r.anchorMax = new Vector2(1, anchorMin.y);
        r.pivot = new Vector2(0.5f, anchorMin.y);
        r.anchoredPosition = Vector2.zero;
        r.sizeDelta = new Vector2(0, 80);
        return img;
    }

    // Begin a boss cutscene; spawning is deferred until the cutscene finishes
    void BeginBossCutscene(bool isMidBoss)
    {
        if (bossCutscenePanel == null)
        {
            // Fallback if not initialised  Ejust spawn directly
            SpawnBoss(isMidBoss);
            return;
        }
        bossCutsceneIsVictory = false;
        bossCutscenePendingSpawn = true;
        bossCutscenePendingMid = isMidBoss;
        bossCutsceneVictoryMidBoss = false;
        bossCutsceneTimer = bossCutsceneDuration;
        bossCutscenePanel.SetActive(true);
        bossCutscenePanel.transform.SetAsLastSibling();

        var accent = GetBossEncounterAccent(isMidBoss);
        bossCutsceneNameText.text = GetBossEncounterName(isMidBoss);
        bossCutsceneNameText.color = WithAlpha(accent, 0f);
        bossCutsceneWarning.text = isMidBoss ? "⚠ ELITE ENEMY ⚠" : "⚠ BOSS APPROACHING ⚠";
        bossCutsceneWarning.color = GetBossEncounterWarningColor(isMidBoss);
        bossCutsceneSubText.text = GetBossEncounterIntroText(isMidBoss);
        bossCutsceneSubText.color = WithAlpha(accent, 0f);
        bossCutscenePortrait.sprite = GetBossEncounterSprite(isMidBoss);
        var portraitTint = GetBossEncounterTint(isMidBoss);
        bossCutscenePortrait.color = new Color(portraitTint.r, portraitTint.g, portraitTint.b, 0f);
        bossCutsceneSlashTop.color = WithAlpha(accent, 0f);
        bossCutsceneSlashBottom.color = WithAlpha(accent, 0f);
        var bossBackground = UseGeneratedBackgrounds ? GetBossEncounterBackground(isMidBoss) : null;
        if (bossBackground != null)
            ApplyScreenBackgroundSprite(bossCutsceneBackgroundImage, bossBackground, new Color(1f, 1f, 1f, 0f));
        else if (bossCutsceneBackgroundImage != null)
        {
            bossCutsceneBackgroundImage.sprite = null;
            bossCutsceneBackgroundImage.color = Color.clear;
        }
        bossCutsceneVignette.color = new Color(0, 0, 0, 0);

        Shake(0.5f, 0.16f);
        PlayBossEncounterBgm(isMidBoss);
        PlaySfx("Boss", isMidBoss ? 110f : 80f, 0.35f, 0.65f);
    }

    string GetBossEncounterName(bool isMidBoss)
    {
        return isMidBoss ? "Pulswyrm" : "Nullwyrm";
    }

    string GetBossEncounterIntroText(bool isMidBoss)
    {
        return isMidBoss
            ? "MID-WAVE " + wave + " ELITE ENEMY"
            : "FINAL PROTOCOL - WAVE " + wave + " BOSS";
    }

    Color GetBossEncounterAccent(bool isMidBoss)
    {
        return isMidBoss ? new Color(1f, 0.62f, 0.16f) : new Color(1f, 0.20f, 0.92f);
    }

    Color GetBossEncounterWarningColor(bool isMidBoss)
    {
        return isMidBoss ? new Color(1f, 0.55f, 0.18f, 0f) : new Color(1f, 0.18f, 0.18f, 0f);
    }

    Sprite GetBossEncounterSprite(bool isMidBoss)
    {
        if (currentStageId == 0 && bossSprite != null)
            return bossSprite;

        var sprite = isMidBoss ? pulswyrmSprite : nullwyrmSprite;
        return sprite != null ? sprite : bossSprite;
    }

    Color GetBossEncounterTint(bool isMidBoss)
    {
        return isMidBoss ? new Color(1f, 0.78f, 0.42f, 1f) : Color.white;
    }

    Sprite GetBossEncounterBackground(bool isMidBoss)
    {
        return isMidBoss ? bossPulswyrmBackgroundSprite : bossNullwyrmBackgroundSprite;
    }

    string GetBossEncounterBgmKey(bool isMidBoss)
    {
        return isMidBoss ? "BGM_Boss_Pulswyrm" : "BGM_Boss_Nullwyrm";
    }

    string GetBossEncounterLegacyBgmKey(bool isMidBoss)
    {
        return isMidBoss ? "BGM_BossPulswyrm" : "BGM_BossNullwyrm";
    }

    // Brief victory burst after boss kill (called from KillEnemy)
    void BeginBossVictoryCutscene(bool isMidBoss)
    {
        if (bossCutscenePanel == null) return;
        bossCutsceneIsVictory = true;
        bossCutscenePendingSpawn = false;
        bossCutsceneVictoryMidBoss = isMidBoss;
        bossCutsceneTimer = 1.2f;
        bossCutscenePanel.SetActive(true);
        bossCutscenePanel.transform.SetAsLastSibling();

        var accent = isMidBoss ? new Color(1f, 0.78f, 0.42f) : new Color(0.4f, 1f, 0.85f);
        bossCutsceneNameText.text = GetBossEncounterName(isMidBoss) + "  ELIMINATED";
        bossCutsceneNameText.color = WithAlpha(accent, 0f);
        bossCutsceneWarning.text = "BOSS DESTROYED";
        bossCutsceneWarning.color = WithAlpha(new Color(1f, 0.92f, 0.4f), 0f);
        bossCutsceneSubText.text = "";
        bossCutscenePortrait.sprite = GetBossEncounterSprite(isMidBoss);
        var portraitTint = GetBossEncounterTint(isMidBoss);
        bossCutscenePortrait.color = new Color(portraitTint.r, portraitTint.g, portraitTint.b, 0f);
        bossCutsceneSlashTop.color = WithAlpha(accent, 0f);
        bossCutsceneSlashBottom.color = WithAlpha(accent, 0f);
        if (bossCutsceneBackgroundImage != null)
        {
            bossCutsceneBackgroundImage.sprite = null;
            bossCutsceneBackgroundImage.color = Color.clear;
        }
        bossCutsceneVignette.color = new Color(0, 0, 0, 0);
    }

    void UpdateBossCutscene()
    {
        if (bossCutscenePanel == null || !bossCutscenePanel.activeSelf) return;
        var dt = Time.unscaledDeltaTime;
        bossCutsceneTimer -= dt;
        var dur = bossCutsceneIsVictory ? 1.2f : bossCutsceneDuration;
        var elapsed = dur - bossCutsceneTimer;
        var t = Mathf.Clamp01(elapsed / dur);
        // Eased intro -> hold -> outro
        var introT = Mathf.Clamp01(t / 0.25f);
        var outroT = Mathf.Clamp01((t - 0.75f) / 0.25f);
        var alpha = (1f - outroT) * (introT < 1f ? introT : 1f);

        bossCutsceneVignette.color = new Color(0, 0, 0, 0.72f * alpha);
        if (bossCutsceneBackgroundImage != null && bossCutsceneBackgroundImage.sprite != null)
            bossCutsceneBackgroundImage.color = new Color(1f, 1f, 1f, 0.56f * alpha);

        // Slashing bars sweep in from sides
        var sweepT = Mathf.SmoothStep(0f, 1f, introT);
        var canvasWidth = ((RectTransform)canvas.transform).rect.width;
        var slashWidth = Mathf.Lerp(0f, canvasWidth, sweepT);
        bossCutsceneSlashTop.rectTransform.sizeDelta = new Vector2(slashWidth, 80);
        bossCutsceneSlashBottom.rectTransform.sizeDelta = new Vector2(slashWidth, 80);
        var slashAlpha = alpha * 0.85f;
        var slashColor = bossCutsceneSlashTop.color;
        slashColor.a = slashAlpha;
        bossCutsceneSlashTop.color = slashColor;
        slashColor = bossCutsceneSlashBottom.color;
        slashColor.a = slashAlpha;
        bossCutsceneSlashBottom.color = slashColor;

        // Portrait & text fade in with slight scale-up
        var scale = Mathf.Lerp(0.78f, 1f, Mathf.SmoothStep(0f, 1f, introT));
        bossCutscenePortrait.rectTransform.localScale = Vector3.one * scale;
        var c = bossCutscenePortrait.color; c.a = alpha; bossCutscenePortrait.color = c;
        c = bossCutsceneWarning.color; c.a = alpha * (0.6f + Mathf.Sin(Time.unscaledTime * 12f) * 0.4f); bossCutsceneWarning.color = c;
        c = bossCutsceneNameText.color; c.a = alpha; bossCutsceneNameText.color = c;
        c = bossCutsceneSubText.color; c.a = alpha; bossCutsceneSubText.color = c;

        if (bossCutsceneTimer <= 0f)
        {
            var wasVictory = bossCutsceneIsVictory;
            var wasMidBossVictory = bossCutsceneVictoryMidBoss;
            bossCutscenePanel.SetActive(false);
            if (bossCutscenePendingSpawn)
            {
                bossCutscenePendingSpawn = false;
                SpawnBoss(bossCutscenePendingMid);
            }
            else if (wasVictory && wasMidBossVictory && !gameOver && !victory)
            {
                bossCutsceneIsVictory = false;
                bossCutsceneVictoryMidBoss = false;
                PlayBgm();
            }
        }
    }

    void CreateEvolutionCutscene()
    {
        evolutionCutscenePanel = new GameObject("Evolution Cutscene", typeof(Image));
        evolutionCutscenePanel.transform.SetParent(canvas.transform, false);
        evolutionCutsceneBack = evolutionCutscenePanel.GetComponent<Image>();
        evolutionCutsceneBack.color = Color.clear;
        evolutionCutsceneBack.raycastTarget = false;
        var rect = evolutionCutscenePanel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var ringGo = new GameObject("Evolution Ring", typeof(Image));
        ringGo.transform.SetParent(evolutionCutscenePanel.transform, false);
        evolutionCutsceneRing = ringGo.GetComponent<Image>();
        evolutionCutsceneRing.sprite = evolutionRingSprite != null ? evolutionRingSprite : diamondSprite;
        evolutionCutsceneRing.color = Color.clear;
        evolutionCutsceneRing.raycastTarget = false;
        var ringRect = ringGo.GetComponent<RectTransform>();
        ringRect.anchorMin = ringRect.anchorMax = new Vector2(0.5f, 0.5f);
        ringRect.pivot = new Vector2(0.5f, 0.5f);
        ringRect.anchoredPosition = new Vector2(0, 10);
        ringRect.sizeDelta = new Vector2(420, 420);

        var beamGo = new GameObject("Evolution Beam", typeof(Image));
        beamGo.transform.SetParent(evolutionCutscenePanel.transform, false);
        evolutionCutsceneBeam = beamGo.GetComponent<Image>();
        evolutionCutsceneBeam.color = Color.clear;
        evolutionCutsceneBeam.raycastTarget = false;
        var beamRect = beamGo.GetComponent<RectTransform>();
        beamRect.anchorMin = beamRect.anchorMax = new Vector2(0.5f, 0.5f);
        beamRect.pivot = new Vector2(0.5f, 0.5f);
        beamRect.anchoredPosition = new Vector2(0, 20);
        beamRect.sizeDelta = new Vector2(980, 86);
        beamRect.localRotation = Quaternion.Euler(0f, 0f, -6f);

        var portraitGo = new GameObject("Evolution Portrait", typeof(Image));
        portraitGo.transform.SetParent(evolutionCutscenePanel.transform, false);
        evolutionCutscenePortrait = portraitGo.GetComponent<Image>();
        evolutionCutscenePortrait.color = Color.clear;
        evolutionCutscenePortrait.raycastTarget = false;
        var portraitRect = portraitGo.GetComponent<RectTransform>();
        portraitRect.anchorMin = portraitRect.anchorMax = new Vector2(0.5f, 0.5f);
        portraitRect.pivot = new Vector2(0.5f, 0.5f);
        portraitRect.anchoredPosition = new Vector2(0, 28);
        portraitRect.sizeDelta = new Vector2(230, 230);

        evolutionCutsceneTitleText = CreateText("Evolution Cutscene Title", evolutionCutscenePanel.transform, new Vector2(0, 184), TextAnchor.MiddleCenter, 30, new Color(0.7f, 1f, 1f));
        evolutionCutsceneTitleText.fontStyle = FontStyle.Bold;
        evolutionCutsceneTitleText.rectTransform.sizeDelta = new Vector2(760, 44);

        evolutionCutsceneNameText = CreateText("Evolution Cutscene Name", evolutionCutscenePanel.transform, new Vector2(0, -146), TextAnchor.MiddleCenter, 38, new Color(1f, 0.86f, 0.42f));
        evolutionCutsceneNameText.fontStyle = FontStyle.Bold;
        evolutionCutsceneNameText.rectTransform.sizeDelta = new Vector2(860, 58);

        evolutionCutsceneDescText = CreateText("Evolution Cutscene Desc", evolutionCutscenePanel.transform, new Vector2(0, -194), TextAnchor.MiddleCenter, 18, new Color(0.86f, 1f, 0.96f));
        evolutionCutsceneDescText.rectTransform.sizeDelta = new Vector2(840, 56);

        evolutionCutscenePanel.SetActive(false);
    }

    void CreateBossBar()
    {
        bossBarRoot = new GameObject("Boss HP", typeof(Image));
        bossBarRoot.transform.SetParent(canvas.transform, false);
        var back = bossBarRoot.GetComponent<Image>();
        back.color = new Color(0.03f, 0.015f, 0.04f, 0.88f);
        if (UseGeneratedHudBars && hudBossBarFrameSprite != null)
            ApplySimpleSprite(back, hudBossBarFrameSprite, Color.white);
        var rect = bossBarRoot.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0, -82);
        rect.sizeDelta = new Vector2(560, 22);

        var fillGo = new GameObject("Fill", typeof(Image));
        fillGo.transform.SetParent(bossBarRoot.transform, false);
        bossBarFill = fillGo.GetComponent<Image>();
        ApplyFilledSprite(bossBarFill, UseGeneratedHudBars ? hudBossBarFillSprite : null, new Color(0.95f, 0.18f, 0.78f));
        var fillRect = fillGo.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(3, 3);
        fillRect.offsetMax = new Vector2(-3, -3);
        bossBarRoot.SetActive(false);

        var dangerGo = new GameObject("Danger Overlay", typeof(Image));
        dangerGo.transform.SetParent(canvas.transform, false);
        dangerImage = dangerGo.GetComponent<Image>();
        dangerImage.color = Color.clear;
        dangerImage.raycastTarget = false;
        var dangerRect = dangerGo.GetComponent<RectTransform>();
        dangerRect.anchorMin = Vector2.zero;
        dangerRect.anchorMax = Vector2.one;
        dangerRect.offsetMin = Vector2.zero;
        dangerRect.offsetMax = Vector2.zero;

        var flashGo = new GameObject("Screen Flash", typeof(Image));
        flashGo.transform.SetParent(canvas.transform, false);
        flashImage = flashGo.GetComponent<Image>();
        flashImage.color = Color.clear;
        flashImage.raycastTarget = false;
        var flashRect = flashGo.GetComponent<RectTransform>();
        flashRect.anchorMin = Vector2.zero;
        flashRect.anchorMax = Vector2.one;
        flashRect.offsetMin = Vector2.zero;
        flashRect.offsetMax = Vector2.zero;
    }

    void CreateHud()
    {
        var hudPanel = CreatePanel("Wave Panel", canvas.transform, new Vector2(16, -16), new Vector2(250, 74), new Vector2(0, 1));
        wavePanelRoot = hudPanel;
        waveText = CreateText("Wave Text", hudPanel.transform, new Vector2(14, -10), TextAnchor.UpperLeft, 20, new Color(0.68f, 1f, 1f));
        waveText.fontStyle = FontStyle.Bold;
        waveText.rectTransform.sizeDelta = new Vector2(220, 30);
        nameText = CreateText("Partner Name", hudPanel.transform, new Vector2(14, -40), TextAnchor.UpperLeft, 16, new Color(0.72f, 0.94f, 1f));
        nameText.fontStyle = FontStyle.Bold;
        nameText.rectTransform.sizeDelta = new Vector2(228, 30);

        var hpPanel = CreatePanel("HP Panel", canvas.transform, new Vector2(16, -104), new Vector2(270, 104), new Vector2(0, 1));
        hpPanelRoot = hpPanel;
        playerHpFill = CreateHudBar(hpPanel.transform, new Vector2(14, -22), "自分HP", new Color(0.22f, 0.83f, 1f));
        playerHpBarBackRect = hpPanel.transform.Find("自分HP Bar Back")?.GetComponent<RectTransform>();
        if (playerHpBarBackRect != null) playerHpBarBaseSize = playerHpBarBackRect.sizeDelta;
        eggHpFill = CreateHudBar(hpPanel.transform, new Vector2(14, -62), "コア HP", new Color(1f, 0.8f, 0.18f));
        eggHpBarBackRect = hpPanel.transform.Find("コア HP Bar Back")?.GetComponent<RectTransform>();
        if (eggHpBarBackRect != null) eggHpBarBaseSize = eggHpBarBackRect.sizeDelta;

        var chipHud = CreatePanel("Chip Mini Panel", canvas.transform, new Vector2(16, -306), new Vector2(190, 42), new Vector2(0, 1));
        chipHudRoot = chipHud;
        SetPanelAccentColor(chipHud.transform, new Color(0.45f, 1f, 0.78f), new Color(1f, 0.86f, 0.32f));
        CreateHudIcon(chipHud.transform, "Chip HUD Icon", hudIconDataChipSprite, new Vector2(12, -10), new Vector2(20, 20));
        chipHudText = CreateText("Chip HUD Text", chipHud.transform, new Vector2(38, -9), TextAnchor.UpperLeft, 15, new Color(0.82f, 1f, 0.9f));
        chipHudText.fontStyle = FontStyle.Bold;
        chipHudText.rectTransform.sizeDelta = new Vector2(136, 24);

        var dataPanel = CreatePanel("Data Panel", canvas.transform, new Vector2(16, -354), new Vector2(250, 54), new Vector2(0, 1));
        dataPanelRoot = dataPanel;
        CreateHudIcon(dataPanel.transform, "Data Panel Icon", hudIconDataChipSprite, new Vector2(14, -13), new Vector2(22, 22));
        resourceText = CreateText("Resource Text", dataPanel.transform, new Vector2(44, -11), TextAnchor.UpperLeft, 17, new Color(0.85f, 1f, 0.86f));
        resourceText.fontStyle = FontStyle.Bold;
        resourceText.rectTransform.sizeDelta = new Vector2(190, 28);

        var levelPanel = CreatePanel("Level Panel", canvas.transform, new Vector2(16, -220), new Vector2(270, 76), new Vector2(0, 1));
        levelPanelRoot = levelPanel;
        levelText = CreateText("Level Text", levelPanel.transform, new Vector2(14, -10), TextAnchor.UpperLeft, 17, new Color(0.88f, 1f, 1f));
        levelText.fontStyle = FontStyle.Bold;
        levelText.rectTransform.sizeDelta = new Vector2(220, 30);
        dataFill = CreateHudBar(levelPanel.transform, new Vector2(14, -48), "EXP", new Color(0.5f, 0.95f, 0.35f));
        expBarBackRect = levelPanel.transform.Find("EXP Bar Back")?.GetComponent<RectTransform>();
        if (expBarBackRect != null) expBarBaseSize = expBarBackRect.sizeDelta;

        CreateWaveProgressPanel();
        CreateSkillPanel();
        CreateStatsPanel();
        CreateStatusPanel();
        CreateEventLogPanel();

        controlText = CreateText("Controls", canvas.transform, new Vector2(18, 16), TextAnchor.UpperLeft, 15, new Color(0.75f, 0.86f, 0.92f));
        var controlRect = controlText.rectTransform;
        controlRect.anchorMin = controlRect.anchorMax = new Vector2(0, 0);
        controlRect.pivot = new Vector2(0, 0);
        controlRect.sizeDelta = new Vector2(620, 34);

        hudText = CreateText("HUD Legacy Text", canvas.transform, new Vector2(0, 0), TextAnchor.UpperLeft, 1, Color.clear);
        hudText.gameObject.SetActive(false);
    }

    GameObject CreatePanel(string name, Transform parent, Vector2 position, Vector2 size, Vector2 anchor)
    {
        var hudPanel = new GameObject(name, typeof(Image));
        hudPanel.transform.SetParent(parent, false);
        var panelImage = hudPanel.GetComponent<Image>();
        panelImage.color = new Color(0.006f, 0.02f, 0.032f, 0.88f);
        ApplyGeneratedPanelSkin(hudPanel, name);
        // HUD9 9-slice パネルは枠を内蔵しているので、二重枠になる Outline / ネオン装飾線は付けない（安っぽさ軽減）
        var isHud9Panel = UseHud9SliceCandidates && GetHud9PanelSprite(name) != null;
        if (!isHud9Panel)
        {
            var outline = hudPanel.AddComponent<Outline>();
            outline.effectColor = new Color(0.2f, 0.92f, 1f, 0.36f);
            outline.effectDistance = new Vector2(1.2f, -1.2f);
        }
        var panelRect = hudPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = panelRect.anchorMax = anchor;
        panelRect.pivot = new Vector2(anchor.x, anchor.y);
        panelRect.anchoredPosition = position;
        panelRect.sizeDelta = size;
        if (!isHud9Panel)
            AddPanelAccent(hudPanel.transform, size);
        return hudPanel;
    }

    void AddPanelAccent(Transform parent, Vector2 size)
    {
        var bottomLine = new GameObject("Panel Neon Bottom", typeof(Image));
        bottomLine.transform.SetParent(parent, false);
        var bottomImage = bottomLine.GetComponent<Image>();
        bottomImage.color = new Color(0.08f, 0.52f, 0.68f, 0.24f);
        bottomImage.raycastTarget = false;
        var bottomRect = bottomLine.GetComponent<RectTransform>();
        bottomRect.anchorMin = bottomRect.anchorMax = new Vector2(1, 0);
        bottomRect.pivot = new Vector2(1, 0);
        bottomRect.anchoredPosition = new Vector2(-8, 6);
        bottomRect.sizeDelta = new Vector2(Mathf.Max(42f, size.x * 0.34f), 2f);

        var topLine = new GameObject("Panel Neon Top", typeof(Image));
        topLine.transform.SetParent(parent, false);
        var topImage = topLine.GetComponent<Image>();
        topImage.color = new Color(0.25f, 1f, 1f, 0.42f);
        topImage.raycastTarget = false;
        var topRect = topLine.GetComponent<RectTransform>();
        topRect.anchorMin = topRect.anchorMax = new Vector2(0, 1);
        topRect.pivot = new Vector2(0, 1);
        topRect.anchoredPosition = new Vector2(8, -6);
        topRect.sizeDelta = new Vector2(Mathf.Max(32f, size.x * 0.46f), 2f);

        var sideLine = new GameObject("Panel Neon Side", typeof(Image));
        sideLine.transform.SetParent(parent, false);
        var sideImage = sideLine.GetComponent<Image>();
        sideImage.color = new Color(1f, 0.78f, 0.2f, 0.32f);
        sideImage.raycastTarget = false;
        var sideRect = sideLine.GetComponent<RectTransform>();
        sideRect.anchorMin = sideRect.anchorMax = new Vector2(0, 1);
        sideRect.pivot = new Vector2(0, 1);
        sideRect.anchoredPosition = new Vector2(8, -10);
        sideRect.sizeDelta = new Vector2(2f, Mathf.Max(24f, size.y * 0.38f));

        AddPanelCorner(parent, "Panel Corner TL", new Vector2(0, 1), new Vector2(12, -12), new Color(0.42f, 1f, 1f, 0.42f));
        AddPanelCorner(parent, "Panel Corner TR", new Vector2(1, 1), new Vector2(-12, -12), new Color(0.42f, 1f, 1f, 0.28f));
        AddPanelCorner(parent, "Panel Corner BL", new Vector2(0, 0), new Vector2(12, 12), new Color(1f, 0.8f, 0.22f, 0.26f));
        AddPanelCorner(parent, "Panel Corner BR", new Vector2(1, 0), new Vector2(-12, 12), new Color(0.42f, 1f, 1f, 0.24f));
    }

    void AddPanelCorner(Transform parent, string name, Vector2 anchor, Vector2 position, Color color)
    {
        var corner = new GameObject(name, typeof(Image));
        corner.transform.SetParent(parent, false);
        var image = corner.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        var rect = corner.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(24, 2);

        var vertical = new GameObject(name + " V", typeof(Image));
        vertical.transform.SetParent(parent, false);
        var verticalImage = vertical.GetComponent<Image>();
        verticalImage.color = color;
        verticalImage.raycastTarget = false;
        var verticalRect = vertical.GetComponent<RectTransform>();
        verticalRect.anchorMin = verticalRect.anchorMax = anchor;
        verticalRect.pivot = anchor;
        verticalRect.anchoredPosition = position;
        verticalRect.sizeDelta = new Vector2(2, 24);
    }

    Image CreateHudBar(Transform parent, Vector2 position, string label, Color fillColor)
    {
        // Labels bumped to font 15 (was 13) for HUD readability
        var iconSprite = GetHudBarIconSprite(label);
        var labelOffset = iconSprite != null ? 20f : 0f;
        CreateHudIcon(parent, label + " Icon", iconSprite, position + new Vector2(0, 0), new Vector2(18, 18));
        var labelText = CreateText(label + " Label", parent, position + new Vector2(0, 3), TextAnchor.UpperLeft, 15, new Color(0.88f, 0.96f, 1f));
        labelText.text = label;
        labelText.fontStyle = FontStyle.Bold;
        labelText.rectTransform.anchoredPosition += new Vector2(labelOffset, 0);
        labelText.rectTransform.sizeDelta = new Vector2(86 - labelOffset, 22);

        var backGo = new GameObject(label + " Bar Back", typeof(Image));
        backGo.transform.SetParent(parent, false);
        var backImage = backGo.GetComponent<Image>();
        backImage.color = new Color(0.05f, 0.07f, 0.1f, 0.95f);
        if (UseHud9SliceCandidates && hud9BarBackSprite != null)
            ApplySlicedSprite(backImage, hud9BarBackSprite, Color.white);
        else if (UseHudBarV2 && hudBarBackV2Sprite != null)
            ApplySimpleSprite(backImage, hudBarBackV2Sprite, Color.white);
        else if (UseGeneratedHudBars && hudBarBackSprite != null)
            ApplySimpleSprite(backImage, hudBarBackSprite, Color.white);
        var backRect = backGo.GetComponent<RectTransform>();
        backRect.anchorMin = backRect.anchorMax = new Vector2(0, 1);
        backRect.pivot = new Vector2(0, 1);
        backRect.anchoredPosition = position + new Vector2(92, 0);
        backRect.sizeDelta = new Vector2(132, 20);

        var lagGo = new GameObject(label + " Bar Lag", typeof(Image));
        lagGo.transform.SetParent(backGo.transform, false);
        var lagImage = lagGo.GetComponent<Image>();
        lagImage.color = new Color(1f, 0.2f, 0.16f, 0.78f);
        if (UseHudBarV2 && label.Contains("HP") && hudBarHpLagV2Sprite != null)
            lagImage.sprite = hudBarHpLagV2Sprite;
        else if (UseGeneratedHudBars && hudBarHpLagSprite != null)
            lagImage.sprite = hudBarHpLagSprite;
        if (!label.Contains("HP"))
            lagImage.color = Color.clear;
        lagImage.type = Image.Type.Filled;
        lagImage.fillMethod = Image.FillMethod.Horizontal;
        lagImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        var lagRect = lagGo.GetComponent<RectTransform>();
        lagRect.anchorMin = Vector2.zero;
        lagRect.anchorMax = Vector2.one;
        lagRect.offsetMin = new Vector2(2, 2);
        lagRect.offsetMax = new Vector2(-2, -2);

        var fillGo = new GameObject(label + " Bar Fill", typeof(Image));
        fillGo.transform.SetParent(backGo.transform, false);
        var fillImage = fillGo.GetComponent<Image>();
        Sprite fillSprite = null;
        if (UseHud9SliceCandidates)
            fillSprite = GetHud9BarFillSprite(label);
        if (fillSprite == null)
            fillSprite = UseHudBarV2 ? GetHudBarFillV2Sprite(label) : (UseGeneratedHudBars ? GetHudBarFillSprite(label) : null);
        ApplyFilledSprite(fillImage, fillSprite, fillColor);
        var fillRect = fillGo.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(2, 2);
        fillRect.offsetMax = new Vector2(-2, -2);

        if (label.Contains("HP") || label == "EXP")
        {
            var valueText = CreateText(label + " Value", parent, position + new Vector2(92, -1), TextAnchor.UpperLeft, 15, Color.white);
            valueText.alignment = TextAnchor.MiddleCenter;
            valueText.fontStyle = FontStyle.Bold;
            valueText.rectTransform.sizeDelta = new Vector2(132, 20);
            if (label == "EXP")
            {
                expValueText = valueText;
                valueText.color = new Color(0.92f, 1f, 0.86f);
            }
            else if (label.Contains("自分"))
            {
                playerHpValueText = valueText;
                playerHpLagFill = lagImage;
            }
            else
            {
                eggHpValueText = valueText;
                eggHpLagFill = lagImage;
            }
        }
        return fillImage;
    }

    void CreateWaveProgressPanel()
    {
        var useHud9WaveProgress = UseHud9SliceCandidates && hud9WaveProgressFrameSprite != null;
        var progressPanelSize = (useHud9WaveProgress || UseWaveProgressHudV2) ? new Vector2(576, 54) : new Vector2(520, 54);
        waveProgressRoot = CreatePanel("Wave Progress Panel", canvas.transform, new Vector2(0, -18), progressPanelSize, new Vector2(0.5f, 1));
        if (useHud9WaveProgress || (UseWaveProgressHudV2 && hudWaveProgressFrameV2Sprite != null))
        {
            var rootImage = waveProgressRoot.GetComponent<Image>();
            if (useHud9WaveProgress)
                ApplySlicedSprite(rootImage, hud9WaveProgressFrameSprite, Color.white);
            else
                ApplySimpleSprite(rootImage, hudWaveProgressFrameV2Sprite, Color.white);
            if (rootImage != null)
                rootImage.raycastTarget = false;
        }
        waveProgressText = CreateText("Wave Progress Text", waveProgressRoot.transform, new Vector2(16, -8), TextAnchor.UpperLeft, 18, new Color(0.82f, 1f, 1f));
        waveProgressText.fontStyle = FontStyle.Bold;
        waveProgressText.rectTransform.sizeDelta = (useHud9WaveProgress || UseWaveProgressHudV2) ? new Vector2(544, 26) : new Vector2(488, 26);

        var backGo = new GameObject("Wave Progress Back", typeof(Image));
        backGo.transform.SetParent(waveProgressRoot.transform, false);
        var backImage = backGo.GetComponent<Image>();
        backImage.color = (useHud9WaveProgress || UseWaveProgressHudV2)
            ? new Color(0.01f, 0.025f, 0.034f, 0.42f)
            : new Color(0.035f, 0.05f, 0.07f, 0.95f);
        if (!UseWaveProgressHudV2 && UseGeneratedHudBars && hudWaveProgressFrameSprite != null)
            ApplySimpleSprite(backImage, hudWaveProgressFrameSprite, Color.white);
        var backRect = backGo.GetComponent<RectTransform>();
        backRect.anchorMin = backRect.anchorMax = new Vector2(0, 1);
        backRect.pivot = new Vector2(0, 1);
        backRect.anchoredPosition = (useHud9WaveProgress || UseWaveProgressHudV2) ? new Vector2(32, -33) : new Vector2(16, -33);
        backRect.sizeDelta = (useHud9WaveProgress || UseWaveProgressHudV2) ? new Vector2(512, 12) : new Vector2(488, 12);
        waveProgressBackRect = backRect;
        waveProgressBaseSize = backRect.sizeDelta;

        var fillGo = new GameObject("Wave Progress Fill", typeof(Image));
        fillGo.transform.SetParent(backGo.transform, false);
        waveProgressFill = fillGo.GetComponent<Image>();
        var initialProgressSprite = useHud9WaveProgress ? hud9WaveProgressNormalSprite : (UseWaveProgressHudV2 ? hudWaveProgressNormalV2Sprite : (UseGeneratedHudBars ? hudWaveProgressNormalSprite : null));
        ApplyFilledSprite(waveProgressFill, initialProgressSprite, new Color(0.2f, 0.88f, 1f, 0.95f));
        var fillRect = fillGo.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(2, 2);
        fillRect.offsetMax = new Vector2(-2, -2);
    }

    void CreateSkillPanel()
    {
        // Loadout panel  Eslightly taller to fit bumped-up fonts
        var loadoutPanel = CreatePanel("Loadout Panel", canvas.transform, new Vector2(16, 86), new Vector2(296, 92), new Vector2(0, 0));
        loadoutPanelRoot = loadoutPanel;
        var title = CreateText("Loadout Title", loadoutPanel.transform, new Vector2(14, -9), TextAnchor.UpperLeft, 16, new Color(0.60f, 1f, 1f));
        title.text = "ACTIVE BUILD";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(260, 22);

        loadoutText = CreateText("Loadout Text", loadoutPanel.transform, new Vector2(14, -36), TextAnchor.UpperLeft, 15, new Color(0.92f, 1f, 0.97f));
        loadoutText.rectTransform.sizeDelta = new Vector2(268, 50);
        loadoutText.text = "AUTO BIT x1\nRING -   BURST -   CROSS -";
    }

    void CreateStatsPanel()
    {
        var statsPanel = CreatePanel("Stats Panel", canvas.transform, new Vector2(-18, 18), new Vector2(286, 156), new Vector2(1, 0));
        statsPanelRoot = statsPanel;
        var title = CreateText("Stats Title", statsPanel.transform, new Vector2(14, -10), TextAnchor.UpperLeft, 15, new Color(0.55f, 1f, 1f));
        title.text = "COMBAT";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(250, 22);
        statsText = CreateText("Stats Text", statsPanel.transform, new Vector2(14, -36), TextAnchor.UpperLeft, 15, new Color(0.92f, 1f, 1f));
        statsText.rectTransform.sizeDelta = new Vector2(258, 110);
    }

    void CreateStatusPanel()
    {
        var statusPanel = CreatePanel("Partner Status Panel", canvas.transform, new Vector2(-18, -178), new Vector2(360, 228), new Vector2(1, 1));
        statusPanelRoot = statusPanel;
        var title = CreateText("Status Title", statusPanel.transform, new Vector2(16, -10), TextAnchor.UpperLeft, 14, new Color(0.55f, 1f, 1f));
        title.text = "LINKS";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(320, 22);

        for (var i = 0; i < 4; i++)
        {
            var dotGo = new GameObject("Evolution Dot " + i, typeof(Image));
            dotGo.transform.SetParent(statusPanel.transform, false);
            var dot = dotGo.GetComponent<Image>();
            dot.sprite = circleSprite;
            dot.color = new Color(0.1f, 0.16f, 0.18f, 0.9f);
            var rect = dotGo.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(220 + i * 24, -12);
            rect.sizeDelta = new Vector2(15, 15);
            evolutionDots.Add(dot);
        }

        var linkTitle = CreateText("Link Slot Title", statusPanel.transform, new Vector2(16, -36), TextAnchor.UpperLeft, 12, new Color(0.74f, 0.95f, 1f));
        linkTitle.text = "ALLY LINKS";
        linkTitle.fontStyle = FontStyle.Bold;
        linkTitle.rectTransform.sizeDelta = new Vector2(320, 18);

        var names = new[] { "NOVA", "BULW", "SIPH", "PHASE" };
        for (var i = 0; i < 4; i++)
        {
            var slot = new GameObject("Link Slot " + names[i], typeof(Image));
            slot.transform.SetParent(statusPanel.transform, false);
            var slotImage = slot.GetComponent<Image>();
            slotImage.color = new Color(0.035f, 0.055f, 0.07f, 0.9f);
            var slotOutline = slot.AddComponent<Outline>();
            slotOutline.effectColor = new Color(0.24f, 0.42f, 0.5f, 0.7f);
            slotOutline.effectDistance = new Vector2(1f, -1f);
            var rect = slot.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(14 + i * 82, -56);
            rect.sizeDelta = new Vector2(78, 46);
            linkSlotImages.Add(slotImage);

            var icon = new GameObject("Link Slot Icon " + names[i], typeof(Image));
            icon.transform.SetParent(slot.transform, false);
            var iconImage = icon.GetComponent<Image>();
            iconImage.sprite = i == 0 ? novaLinkSprite : i == 1 ? bulwarkLinkSprite : i == 2 ? siphonLinkSprite : phaseLinkSprite;
            iconImage.color = new Color(0.28f, 0.36f, 0.38f, 0.55f);
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            var iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchorMin = iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.anchoredPosition = new Vector2(5, 0);
            iconRect.sizeDelta = new Vector2(24, 24);
            linkSlotIconImages.Add(iconImage);

            var text = CreateText("Link Slot Text " + names[i], slot.transform, new Vector2(29, 0), TextAnchor.MiddleLeft, 9, new Color(0.55f, 0.65f, 0.68f));
            text.text = names[i] + "\nOFF";
            text.fontStyle = FontStyle.Bold;
            text.rectTransform.sizeDelta = new Vector2(45, 38);
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            linkSlotTexts.Add(text);
        }

        var gaugeBack = new GameObject("Fusion Gauge Back", typeof(Image));
        gaugeBack.transform.SetParent(statusPanel.transform, false);
        var gaugeBackImage = gaugeBack.GetComponent<Image>();
        gaugeBackImage.color = new Color(0.04f, 0.055f, 0.075f, 0.95f);
        var gaugeBackRect = gaugeBack.GetComponent<RectTransform>();
        gaugeBackRect.anchorMin = gaugeBackRect.anchorMax = new Vector2(0, 1);
        gaugeBackRect.pivot = new Vector2(0, 1);
        gaugeBackRect.anchoredPosition = new Vector2(16, -112);
        gaugeBackRect.sizeDelta = new Vector2(320, 14);

        var gaugeFill = new GameObject("Fusion Gauge Fill", typeof(Image));
        gaugeFill.transform.SetParent(gaugeBack.transform, false);
        fusionProgressFill = gaugeFill.GetComponent<Image>();
        fusionProgressFill.color = new Color(1f, 0.38f, 0.96f, 0.95f);
        fusionProgressFill.type = Image.Type.Filled;
        fusionProgressFill.fillMethod = Image.FillMethod.Horizontal;
        fusionProgressFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        var gaugeFillRect = gaugeFill.GetComponent<RectTransform>();
        gaugeFillRect.anchorMin = Vector2.zero;
        gaugeFillRect.anchorMax = Vector2.one;
        gaugeFillRect.offsetMin = new Vector2(2, 2);
        gaugeFillRect.offsetMax = new Vector2(-2, -2);

        fusionProgressText = CreateText("Fusion Progress Text", statusPanel.transform, new Vector2(16, -132), TextAnchor.UpperLeft, 12, new Color(1f, 0.72f, 1f));
        fusionProgressText.fontStyle = FontStyle.Bold;
        fusionProgressText.rectTransform.sizeDelta = new Vector2(320, 18);

        statusText = CreateText("Status Text", statusPanel.transform, new Vector2(16, -154), TextAnchor.UpperLeft, 12, new Color(0.9f, 1f, 1f));
        statusText.rectTransform.sizeDelta = new Vector2(320, 18);

        synergyText = CreateText("Synergy Text", statusPanel.transform, new Vector2(16, -172), TextAnchor.UpperLeft, 11, new Color(0.62f, 1f, 0.9f));
        synergyText.rectTransform.sizeDelta = new Vector2(320, 24);

        var relicSep = new GameObject("Relic Separator", typeof(UnityEngine.UI.Image));
        relicSep.transform.SetParent(statusPanel.transform, false);
        relicSep.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 0.82f, 0.18f, 0.22f);
        relicSep.GetComponent<UnityEngine.UI.Image>().raycastTarget = false;
        var sepRect = relicSep.GetComponent<RectTransform>();
        sepRect.anchorMin = sepRect.anchorMax = new Vector2(0.5f, 1f);
        sepRect.pivot = new Vector2(0.5f, 1f);
        sepRect.anchoredPosition = new Vector2(0, -196);
        sepRect.sizeDelta = new Vector2(328, 1);

        for (var i = 0; i < 4; i++)
        {
            var slot = new GameObject("Relic HUD Slot " + i, typeof(Image));
            slot.transform.SetParent(statusPanel.transform, false);
            var slotImage = slot.GetComponent<Image>();
            slotImage.sprite = UseGeneratedHudPanels && hudRelicSlotFrameSprite != null ? hudRelicSlotFrameSprite : squareSprite;
            slotImage.color = new Color(0.15f, 0.2f, 0.16f, 0.58f);
            slotImage.raycastTarget = false;
            var slotRect = slot.GetComponent<RectTransform>();
            slotRect.anchorMin = slotRect.anchorMax = new Vector2(0, 1);
            slotRect.pivot = new Vector2(0, 1);
            slotRect.anchoredPosition = new Vector2(16 + i * 32, -198);
            slotRect.sizeDelta = new Vector2(28, 28);
            relicHudSlotFrames.Add(slotImage);

            var icon = new GameObject("Relic HUD Icon " + i, typeof(Image));
            icon.transform.SetParent(slot.transform, false);
            var iconImage = icon.GetComponent<Image>();
            iconImage.color = Color.clear;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            var iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.5f, 0.5f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = new Vector2(21, 21);
            relicHudSlotIcons.Add(iconImage);
        }

        relicDisplayText = CreateText("Relic Display", statusPanel.transform, new Vector2(154, -202), TextAnchor.UpperLeft, 10, new Color(1f, 0.88f, 0.42f));
        relicDisplayText.rectTransform.sizeDelta = new Vector2(178, 24);
        relicDisplayText.resizeTextForBestFit = true;
        relicDisplayText.resizeTextMinSize = 8;
        relicDisplayText.resizeTextMaxSize = 10;
        relicDisplayText.text = "";
    }

    void CreateEventLogPanel()
    {
        var logPanel = CreatePanel("Event Log Panel", canvas.transform, new Vector2(0, 20), new Vector2(520, 70), new Vector2(0.5f, 0));
        eventLogPanelRoot = logPanel;
        eventLogText = CreateText("Event Log Text", logPanel.transform, new Vector2(14, -10), TextAnchor.UpperLeft, 15, new Color(0.82f, 1f, 0.95f));
        eventLogText.rectTransform.sizeDelta = new Vector2(492, 48);
        eventLogText.text = "";
    }

    void CreatePartnerSelectPanel()
    {
        // ── Full-screen partner select panel ─────────────────────────
        partnerPanel = new GameObject("Partner Select Panel", typeof(Image));
        partnerPanel.transform.SetParent(canvas.transform, false);
        var image = partnerPanel.GetComponent<Image>();
        image.color = new Color(0.004f, 0.014f, 0.022f, 0.97f);
        var rect = partnerPanel.GetComponent<RectTransform>();
        // Stretch to fill canvas (full screen)
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var title = CreateText("Partner Title", partnerPanel.transform, new Vector2(0, -50), TextAnchor.MiddleCenter, 32, new Color(0.55f, 0.95f, 1f));
        title.text = "BEAST LINK";
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.anchorMin = title.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        title.rectTransform.pivot = new Vector2(0.5f, 1f);
        title.rectTransform.anchoredPosition = new Vector2(0, -18);
        title.rectTransform.sizeDelta = new Vector2(800, 42);

        var subtitle = CreateText("Partner Subtitle", partnerPanel.transform, Vector2.zero, TextAnchor.MiddleCenter, 14, new Color(0.78f, 1f, 0.96f));
        subtitle.text = "好きな素体を選んでスタート / ホイールでスクロール";
        subtitle.fontStyle = FontStyle.Bold;
        subtitle.rectTransform.anchorMin = subtitle.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        subtitle.rectTransform.pivot = new Vector2(0.5f, 1f);
        subtitle.rectTransform.anchoredPosition = new Vector2(0, -66);
        subtitle.rectTransform.sizeDelta = new Vector2(840, 22);

        partnerBackButton = CreateWideButton("Partner Back", partnerPanel.transform, new Vector2(94, -42), new Vector2(150, 40));
        var backRect = partnerBackButton.GetComponent<RectTransform>();
        backRect.anchorMin = backRect.anchorMax = new Vector2(0, 1);
        backRect.pivot = new Vector2(0, 1);
        backRect.anchoredPosition = new Vector2(24, -24);
        partnerBackButton.GetComponentInChildren<Text>().text = "戻る";
        partnerBackButton.onClick.AddListener(ReturnFromPartnerSelectToMenu);

        var scrollGo = new GameObject("Partner Scroll View", typeof(RectTransform), typeof(ScrollRect));
        scrollGo.transform.SetParent(partnerPanel.transform, false);
        var scrollRectTransform = scrollGo.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = scrollRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRectTransform.pivot = new Vector2(0.5f, 0.5f);
        scrollRectTransform.anchoredPosition = new Vector2(0, -58);
        scrollRectTransform.sizeDelta = new Vector2(1120, 560);

        var viewportGo = new GameObject("Partner Scroll Viewport", typeof(Image), typeof(Mask));
        viewportGo.transform.SetParent(scrollGo.transform, false);
        var viewportImage = viewportGo.GetComponent<Image>();
        viewportImage.color = new Color(1f, 1f, 1f, 0.012f);
        viewportImage.raycastTarget = true;
        var mask = viewportGo.GetComponent<Mask>();
        mask.showMaskGraphic = false;
        var viewportRect = viewportGo.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;

        var contentGo = new GameObject("Partner Scroll Content", typeof(RectTransform));
        contentGo.transform.SetParent(viewportGo.transform, false);
        partnerScrollContentRect = contentGo.GetComponent<RectTransform>();
        partnerScrollContentRect.anchorMin = new Vector2(0.5f, 1f);
        partnerScrollContentRect.anchorMax = new Vector2(0.5f, 1f);
        partnerScrollContentRect.pivot = new Vector2(0.5f, 1f);
        partnerScrollContentRect.anchoredPosition = Vector2.zero;
        partnerScrollContentRect.sizeDelta = new Vector2(1080, 560);

        partnerScrollRect = scrollGo.GetComponent<ScrollRect>();
        partnerScrollRect.viewport = viewportRect;
        partnerScrollRect.content = partnerScrollContentRect;
        partnerScrollRect.horizontal = false;
        partnerScrollRect.vertical = true;
        partnerScrollRect.movementType = ScrollRect.MovementType.Clamped;
        partnerScrollRect.scrollSensitivity = 34f;
        partnerScrollRect.inertia = true;

        EnsurePartnerButtonCount(12);
    }

    void EnsurePartnerButtonCount(int count)
    {
        var parent = partnerScrollContentRect != null ? partnerScrollContentRect : partnerPanel != null ? partnerPanel.transform : null;
        if (parent == null)
            return;

        while (partnerButtons.Count < count)
        {
            var index = partnerButtons.Count;
            var button = CreateButton("Partner " + (index + 1), parent, Vector2.zero);
            var btnRect = button.GetComponent<RectTransform>();
            btnRect.anchorMin = btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.sizeDelta = new Vector2(300f, 220f);
            partnerButtons.Add(button);
        }
    }

    void LayoutPartnerButtons(int visibleCount)
    {
        const int columns = 3;
        const float cardW = 300f;
        const float cardH = 220f;
        const float colSpacing = 350f;
        const float rowSpacing = 252f;
        const float topPadding = 54f;
        const float bottomPadding = 54f;

        var rows = Mathf.Max(1, Mathf.CeilToInt(visibleCount / (float)columns));
        var contentHeight = Mathf.Max(560f, topPadding + rows * rowSpacing + bottomPadding);
        if (partnerScrollContentRect != null)
        {
            partnerScrollContentRect.sizeDelta = new Vector2(1080f, contentHeight);
            partnerScrollContentRect.anchoredPosition = Vector2.zero;
        }
        if (partnerScrollRect != null)
            partnerScrollRect.verticalNormalizedPosition = 1f;

        for (var i = 0; i < partnerButtons.Count; i++)
        {
            var button = partnerButtons[i];
            if (button == null)
                continue;

            var rect = button.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(cardW, cardH);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            var col = i % columns;
            var row = i / columns;
            var x = (col - (columns - 1) * 0.5f) * colSpacing;
            var y = contentHeight * 0.5f - topPadding - cardH * 0.5f - row * rowSpacing;
            var position = new Vector2(x, y);
            rect.anchoredPosition = position;
            rect.localScale = Vector3.one;
            cardBasePositions[button] = position;
        }
    }

    void ReturnFromPartnerSelectToMenu()
    {
        if (runtimeReloading)
            return;

        PlaySfx("Pickup", 420f, 0.045f, 0.1f);
        ReloadRuntime(autoStartRun: false);
    }

    void CreateResultPanel()
    {
        resultPanel = new GameObject("Result Screen", typeof(Image));
        resultPanel.transform.SetParent(canvas.transform, false);
        var image = resultPanel.GetComponent<Image>();
        image.color = new Color(0f, 0.006f, 0.012f, 0.92f);
        resultBackgroundImage = image;
        if (UseGeneratedBackgrounds && resultClearBackgroundSprite != null)
            ApplyScreenBackgroundSprite(image, resultClearBackgroundSprite, Color.white);
        StretchToParent(resultPanel);

        CreateStretchImage("Result Deep Scrim", resultPanel.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0f, 0.01f, 0.014f, 0.86f));
        CreateStretchImage("Result Top Wash", resultPanel.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -150), Vector2.zero, new Color(0.1f, 0.85f, 1f, 0.13f));
        CreateStretchImage("Result Bottom Wash", resultPanel.transform, Vector2.zero, new Vector2(1, 0), Vector2.zero, new Vector2(0, 150), new Color(1f, 0.7f, 0.2f, 0.1f));
        CreateStretchImage("Result Left Rail", resultPanel.transform, Vector2.zero, new Vector2(0, 1), Vector2.zero, new Vector2(90, 0), new Color(0.02f, 0.18f, 0.25f, 0.4f));
        CreateStretchImage("Result Right Rail", resultPanel.transform, new Vector2(1, 0), Vector2.one, new Vector2(-90, 0), Vector2.zero, new Color(0.04f, 0.08f, 0.13f, 0.66f));

        var deck = CreatePanel("Result Deck", resultPanel.transform, Vector2.zero, new Vector2(960, 580), new Vector2(0.5f, 0.5f));
        SetPanelAccentColor(deck.transform, new Color(0.42f, 1f, 1f), new Color(1f, 0.78f, 0.2f));
        if (UseResultUiV2 && resultDeckFrameSprite != null)
        {
            var deckImage = deck.GetComponent<Image>();
            if (deckImage != null)
                ApplySimpleSprite(deckImage, resultDeckFrameSprite, Color.white);
        }

        // データエッグの勝利エンブレム (デッキ中央の淡いウォーターマーク・文字の背後)
        if (resultCoreEggSprite != null)
            CreateMenuSpriteImage("Result Core Egg Emblem", deck.transform, resultCoreEggSprite, new Vector2(0, 18), new Vector2(360, 360), new Color(1f, 1f, 1f, 0.10f), true);

        resultTitleText = CreateText("Result Title", deck.transform, new Vector2(0, 252), TextAnchor.MiddleCenter, 36, new Color(0.75f, 1f, 1f));
        resultTitleText.fontStyle = FontStyle.Bold;
        resultTitleText.rectTransform.sizeDelta = new Vector2(880, 56);

        var subTitle = CreateText("Result Subtitle", deck.transform, new Vector2(0, 212), TextAnchor.MiddleCenter, 14, new Color(0.7f, 1f, 0.96f));
        subTitle.text = "PROTOCOL REPORT";
        subTitle.rectTransform.sizeDelta = new Vector2(880, 22);

        var portraitFrame = CreatePanel("Result Portrait Frame", deck.transform, new Vector2(-340, 50), new Vector2(220, 220), new Vector2(0.5f, 0.5f));
        SetPanelAccentColor(portraitFrame.transform, new Color(0.5f, 1f, 1f), new Color(1f, 0.84f, 0.32f));
        var portraitFrameImage = portraitFrame.GetComponent<Image>();
        if (portraitFrameImage != null)
        {
            portraitFrameImage.color = new Color(0.012f, 0.04f, 0.06f, 0.96f);
            if (UseResultUiV2 && resultPortraitFrameSprite != null)
                ApplySimpleSprite(portraitFrameImage, resultPortraitFrameSprite, Color.white);
        }

        var glowGo = new GameObject("Result Portrait Glow", typeof(Image));
        glowGo.transform.SetParent(portraitFrame.transform, false);
        resultPortraitGlow = glowGo.GetComponent<Image>();
        resultPortraitGlow.sprite = circleSprite;
        resultPortraitGlow.color = new Color(0.35f, 1f, 1f, 0.22f);
        resultPortraitGlow.raycastTarget = false;
        var glowRect = glowGo.GetComponent<RectTransform>();
        glowRect.anchorMin = glowRect.anchorMax = new Vector2(0.5f, 0.5f);
        glowRect.pivot = new Vector2(0.5f, 0.5f);
        glowRect.anchoredPosition = Vector2.zero;
        glowRect.sizeDelta = new Vector2(200, 200);

        var portraitGo = new GameObject("Result Portrait", typeof(Image));
        portraitGo.transform.SetParent(portraitFrame.transform, false);
        resultPortraitImage = portraitGo.GetComponent<Image>();
        resultPortraitImage.color = Color.white;
        resultPortraitImage.preserveAspect = true;
        resultPortraitImage.raycastTarget = false;
        var portraitRect = portraitGo.GetComponent<RectTransform>();
        portraitRect.anchorMin = portraitRect.anchorMax = new Vector2(0.5f, 0.5f);
        portraitRect.pivot = new Vector2(0.5f, 0.5f);
        portraitRect.anchoredPosition = Vector2.zero;
        portraitRect.sizeDelta = new Vector2(176, 176);

        resultRouteBadgePanel = CreateBadgeStrip("Result Route Badge", deck.transform, new Vector2(-340, -86), new Vector2(220, 44), new Color(0.22f, 0.78f, 1f), out resultRouteBadgeImage, out resultRouteBadgeText);
        resultFusionBadgePanel = CreateBadgeStrip("Result Fusion Badge", deck.transform, new Vector2(-340, -138), new Vector2(220, 44), new Color(1f, 0.38f, 0.96f), out resultFusionBadgeImage, out resultFusionBadgeText);

        var statsHeader = CreateText("Result Stats Header", deck.transform, new Vector2(20, 180), TextAnchor.MiddleCenter, 14, new Color(0.7f, 1f, 1f));
        statsHeader.text = "RUN STATS";
        statsHeader.fontStyle = FontStyle.Bold;
        statsHeader.rectTransform.sizeDelta = new Vector2(420, 22);

        var statKeys = new[] { "WAVE", "TIME", "KILLS", "DATA", "DAMAGE", "BOSS" };
        for (var i = 0; i < statKeys.Length; i++)
        {
            var col = i % 3;
            var row = i / 3;
            var x = -154 + col * 120;
            var y = 118 - row * 86;
            resultStatRoots[i] = CreateStatTile("Result Stat " + statKeys[i], deck.transform, new Vector2(x, y), new Vector2(112, 76), statKeys[i], out resultStatLabels[i], out resultStatValues[i]);
        }

        if (UseResultUiV2 && resultStatRoots[5] != null)
        {
            var rankMedalGo = new GameObject("Result Rank Medal V2", typeof(Image));
            rankMedalGo.transform.SetParent(resultStatRoots[5].transform, false);
            rankMedalGo.transform.SetAsFirstSibling();
            resultRankMedalImage = rankMedalGo.GetComponent<Image>();
            resultRankMedalImage.preserveAspect = true;
            resultRankMedalImage.raycastTarget = false;
            resultRankMedalImage.color = new Color(1f, 1f, 1f, 0.42f);
            var rankMedalRect = rankMedalGo.GetComponent<RectTransform>();
            rankMedalRect.anchorMin = rankMedalRect.anchorMax = new Vector2(0.5f, 0.5f);
            rankMedalRect.pivot = new Vector2(0.5f, 0.5f);
            rankMedalRect.anchoredPosition = new Vector2(0, -8);
            rankMedalRect.sizeDelta = new Vector2(52, 52);
        }

        if (UseResultUiV2 && resultSummaryPlateSprite != null)
            CreateMenuSpriteImage("Result Summary Plate V2", deck.transform, resultSummaryPlateSprite, new Vector2(-27, -54), new Vector2(520, 44), new Color(1f, 1f, 1f, 0.82f), false);

        resultRunSummaryText = CreateText("Result Run Summary", deck.transform, new Vector2(-27, -54), TextAnchor.MiddleCenter, 12, new Color(0.72f, 0.96f, 1f));
        resultRunSummaryText.rectTransform.sizeDelta = new Vector2(430, 44);
        resultRunSummaryText.lineSpacing = 1.0f;

        var mvpHeader = CreateText("Result MVP Header", deck.transform, new Vector2(345, 180), TextAnchor.MiddleCenter, 14, new Color(1f, 0.9f, 0.42f));
        mvpHeader.text = "MVP BUILD";
        mvpHeader.fontStyle = FontStyle.Bold;
        mvpHeader.rectTransform.sizeDelta = new Vector2(220, 22);

        for (var i = 0; i < resultMvpCardRoots.Length; i++)
        {
            var y = 118 - i * 86;
            CreateMvpRow("Result MVP " + i, deck.transform, new Vector2(345, y), new Vector2(230, 68), i, out resultMvpCardRoots[i], out resultMvpIcons[i], out resultMvpTitleTexts[i], out resultMvpDetailTexts[i]);
        }

        resultBodyText = CreateText("Result Body", deck.transform, new Vector2(0, -188), TextAnchor.MiddleCenter, 14, new Color(0.84f, 1f, 0.96f));
        resultBodyText.rectTransform.sizeDelta = new Vector2(880, 32);

        // プロ改喁E 再戦ボタンを目立たせる (一次行動として大きく & 強調色、同設定で即再戦が伝わめE
        resultRetryButton = CreateWideButton("Result Retry Button", deck.transform, new Vector2(-180, -232), new Vector2(340, 64));
        SetButtonSpriteV2(resultRetryButton, btnResultRetrySprite);
        var retryText = resultRetryButton.GetComponentInChildren<Text>();
        retryText.text = "▶ 同設定で再戦  [R]";
        retryText.fontSize = 18;
        retryText.fontStyle = FontStyle.Bold;
        retryText.color = new Color(1f, 0.95f, 0.45f);
        resultRetryButton.onClick.RemoveAllListeners();
        resultRetryButton.onClick.AddListener(RetryRun);
        // 再戦ボタンの背景色を�E設宁E(目立つ橙寁E��)
        var retryImg = resultRetryButton.GetComponent<Image>();
        if (retryImg != null) retryImg.color = new Color(0.20f, 0.12f, 0.04f, 0.95f);

        resultMenuButton = CreateWideButton("Result Menu Button", deck.transform, new Vector2(180, -232), new Vector2(240, 50));
        SetButtonSpriteV2(resultMenuButton, btnResultMenuSprite);
        resultMenuButton.GetComponentInChildren<Text>().text = "メニュー [M]";
        resultMenuButton.onClick.RemoveAllListeners();
        resultMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (UseResultUiV2 && resultFooterGuideSprite != null)
            CreateMenuSpriteImage("Result Footer Guide V2", deck.transform, resultFooterGuideSprite, new Vector2(0, -272), new Vector2(520, 28), new Color(1f, 1f, 1f, 0.78f), false);

        var guide = CreateText("Result Guide", deck.transform, new Vector2(0, -272), TextAnchor.MiddleCenter, 12, new Color(0.7f, 0.9f, 1f));
        guide.text = "R : 同設定で再戦    M / Esc : メインメニュー";

        // ── Embercore Studio フッタークレジチE�� ──
        var footerCredit = CreateText("Result Footer Credit", deck.transform, new Vector2(0, -300), TextAnchor.MiddleCenter, 10, new Color(0.45f, 0.62f, 0.72f));
        footerCredit.text = "Eggcore Protocol  ©  " + StudioName + "   v" + GameVersion;
        footerCredit.rectTransform.sizeDelta = new Vector2(620, 14);
        guide.rectTransform.sizeDelta = new Vector2(620, 20);
        resultPanel.SetActive(false);
    }

    GameObject CreateBadgeStrip(string name, Transform parent, Vector2 position, Vector2 size, Color accent, out Image iconImage, out Text labelText)
    {
        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = new Color(0.025f, 0.06f, 0.09f, 0.95f);
        var stripSprite = name.Contains("Fusion") ? resultBadgeFusionSprite : resultBadgeRouteSprite;
        var usingV2Strip = UseResultUiV2 && stripSprite != null;
        if (usingV2Strip)
            ApplySimpleSprite(image, stripSprite, Color.white);
        var outline = go.AddComponent<Outline>();
        outline.effectColor = WithAlpha(accent, usingV2Strip ? 0.16f : 0.5f);
        outline.effectDistance = usingV2Strip ? new Vector2(0.35f, -0.35f) : new Vector2(1.1f, -1.1f);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var iconGo = new GameObject(name + " Icon", typeof(Image));
        iconGo.transform.SetParent(go.transform, false);
        iconImage = iconGo.GetComponent<Image>();
        iconImage.sprite = diamondSprite;
        iconImage.color = accent;
        iconImage.raycastTarget = false;
        var iconRect = iconGo.GetComponent<RectTransform>();
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.pivot = new Vector2(0, 0.5f);
        iconRect.anchoredPosition = new Vector2(12, 0);
        iconRect.sizeDelta = new Vector2(20, 20);

        labelText = CreateText(name + " Text", go.transform, new Vector2(40, 0), TextAnchor.MiddleLeft, 14, new Color(0.92f, 1f, 0.98f));
        labelText.fontStyle = FontStyle.Bold;
        labelText.rectTransform.sizeDelta = new Vector2(size.x - 50, size.y - 10);

        return go;
    }

    GameObject CreateStatTile(string name, Transform parent, Vector2 position, Vector2 size, string label, out Text labelText, out Text valueText)
    {
        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = new Color(0.018f, 0.045f, 0.07f, 0.96f);
        var usingV2Tile = UseResultUiV2 && resultStatTileSprite != null;
        if (usingV2Tile)
            ApplySimpleSprite(image, resultStatTileSprite, Color.white);
        var outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(0.22f, 0.82f, 1f, usingV2Tile ? 0.12f : 0.4f);
        outline.effectDistance = usingV2Tile ? new Vector2(0.3f, -0.3f) : new Vector2(1f, -1f);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        if (!usingV2Tile)
        {
            var topStrip = new GameObject(name + " Strip", typeof(Image));
            topStrip.transform.SetParent(go.transform, false);
            var topImage = topStrip.GetComponent<Image>();
            topImage.color = new Color(0.4f, 1f, 1f, 0.42f);
            topImage.raycastTarget = false;
            var topRect = topStrip.GetComponent<RectTransform>();
            topRect.anchorMin = topRect.anchorMax = new Vector2(0, 1);
            topRect.pivot = new Vector2(0, 1);
            topRect.anchoredPosition = new Vector2(8, -8);
            topRect.sizeDelta = new Vector2(48, 2);
        }

        labelText = CreateText(name + " Label", go.transform, new Vector2(0, 22), TextAnchor.MiddleCenter, 12, new Color(0.62f, 0.94f, 1f));
        labelText.text = label;
        labelText.fontStyle = FontStyle.Bold;
        labelText.rectTransform.sizeDelta = new Vector2(size.x - 14, 16);

        valueText = CreateText(name + " Value", go.transform, new Vector2(0, -12), TextAnchor.MiddleCenter, 22, new Color(1f, 0.95f, 0.55f));
        valueText.fontStyle = FontStyle.Bold;
        valueText.rectTransform.sizeDelta = new Vector2(size.x - 14, 36);
        return go;
    }

    void CreateMvpRow(string name, Transform parent, Vector2 position, Vector2 size, int index, out GameObject root, out Image icon, out Text title, out Text detail)
    {
        var go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        root = go;
        var image = go.GetComponent<Image>();
        image.color = new Color(0.022f, 0.05f, 0.075f, 0.96f);
        var usingV2Row = UseResultUiV2 && resultMvpRowSprite != null;
        if (usingV2Row)
            ApplySimpleSprite(image, resultMvpRowSprite, Color.white);
        var outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 0.86f, 0.32f, usingV2Row ? 0.14f : 0.48f);
        outline.effectDistance = usingV2Row ? new Vector2(0.35f, -0.35f) : new Vector2(1.1f, -1.1f);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var iconBack = new GameObject(name + " Icon Back", typeof(Image));
        iconBack.transform.SetParent(go.transform, false);
        var iconBackImage = iconBack.GetComponent<Image>();
        iconBackImage.sprite = circleSprite;
        iconBackImage.color = new Color(0.06f, 0.16f, 0.2f, 0.92f);
        iconBackImage.raycastTarget = false;
        var iconBackRect = iconBack.GetComponent<RectTransform>();
        iconBackRect.anchorMin = iconBackRect.anchorMax = new Vector2(0, 0.5f);
        iconBackRect.pivot = new Vector2(0, 0.5f);
        iconBackRect.anchoredPosition = new Vector2(10, 0);
        iconBackRect.sizeDelta = new Vector2(44, 44);

        var iconGo = new GameObject(name + " Icon", typeof(Image));
        iconGo.transform.SetParent(iconBack.transform, false);
        icon = iconGo.GetComponent<Image>();
        icon.sprite = diamondSprite;
        icon.color = new Color(0.42f, 1f, 1f, 0.95f);
        icon.raycastTarget = false;
        var iconRect = iconGo.GetComponent<RectTransform>();
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = Vector2.zero;
        iconRect.sizeDelta = new Vector2(28, 28);

        var rankText = CreateText(name + " Rank", go.transform, new Vector2(size.x * 0.5f - 18, 22), TextAnchor.MiddleCenter, 11, new Color(1f, 0.84f, 0.35f));
        rankText.text = "#" + (index + 1);
        rankText.fontStyle = FontStyle.Bold;
        rankText.rectTransform.sizeDelta = new Vector2(28, 18);

        title = CreateText(name + " Title", go.transform, new Vector2(28, 12), TextAnchor.MiddleLeft, 13, new Color(1f, 0.96f, 0.7f));
        title.fontStyle = FontStyle.Bold;
        title.rectTransform.sizeDelta = new Vector2(size.x - 100, 22);
        title.resizeTextForBestFit = true;
        title.resizeTextMinSize = 10;
        title.resizeTextMaxSize = 13;

        detail = CreateText(name + " Detail", go.transform, new Vector2(28, -12), TextAnchor.MiddleLeft, 11, new Color(0.78f, 0.94f, 1f));
        detail.rectTransform.sizeDelta = new Vector2(size.x - 100, 18);
    }

    Text CreateText(string name, Transform parent, Vector2 anchoredPosition, TextAnchor alignment, int size, Color color)
    {
        var go = new GameObject(name, typeof(Text));
        go.transform.SetParent(parent, false);
        var text = go.GetComponent<Text>();
        text.font = GetUiFont();
        text.fontSize = size;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        var shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0.015f, 0.03f, 0.72f);
        shadow.effectDistance = new Vector2(1f, -1f);
        var rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = alignment == TextAnchor.UpperLeft ? new Vector2(0, 1) : new Vector2(0.5f, 0.5f);
        rect.pivot = alignment == TextAnchor.UpperLeft ? new Vector2(0, 1) : new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        return text;
    }

    Button CreateButton(string name, Transform parent, Vector2 position)
    {
        // Card 210×20 →280×60 (1.33×wide, 1.64×tall)  Emodule decisions feel substantial.
        // Layout: top y=+180, bot y=-180. All internal positions rescaled.
        var go = new GameObject(name, typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(280, 360);
        rect.anchoredPosition = position;
        var image = go.GetComponent<Image>();
        image.color = new Color(0.03f, 0.09f, 0.13f, 0.98f);
        var outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(0.45f, 0.95f, 1f, 0.38f);
        outline.effectDistance = new Vector2(1.6f, -1.6f);

        // Header strip  Esits 12px below top
        var header = new GameObject("Card Header Plate", typeof(Image));
        header.transform.SetParent(go.transform, false);
        var headerImage = header.GetComponent<Image>();
        headerImage.color = new Color(0.005f, 0.022f, 0.028f, 0.85f);
        if (UseCardPartsV2 && cardHeaderBasicV2Sprite != null)
            ApplySimpleSprite(headerImage, cardHeaderBasicV2Sprite, Color.white);
        headerImage.raycastTarget = false;
        var headerRect = header.GetComponent<RectTransform>();
        headerRect.anchorMin = headerRect.anchorMax = new Vector2(0.5f, 1f);
        headerRect.pivot = new Vector2(0.5f, 1f);
        headerRect.anchoredPosition = new Vector2(0, -12);
        headerRect.sizeDelta = new Vector2(252, 34);

        var glow = new GameObject("Card Glow Fill", typeof(Image));
        glow.transform.SetParent(go.transform, false);
        var glowImage = glow.GetComponent<Image>();
        glowImage.sprite = circleSprite;
        glowImage.color = new Color(0.28f, 1f, 1f, 0.06f);
        glowImage.raycastTarget = false;
        var glowRect = glow.GetComponent<RectTransform>();
        glowRect.anchorMin = glowRect.anchorMax = new Vector2(0.5f, 0.5f);
        glowRect.pivot = new Vector2(0.5f, 0.5f);
        glowRect.anchoredPosition = new Vector2(0, 50);
        glowRect.sizeDelta = new Vector2(252, 252);

        var strip = new GameObject("Card Neon Strip", typeof(Image));
        strip.transform.SetParent(go.transform, false);
        var stripImage = strip.GetComponent<Image>();
        stripImage.color = new Color(0.28f, 1f, 1f, 0.42f);
        stripImage.raycastTarget = false;
        var stripRect = strip.GetComponent<RectTransform>();
        stripRect.anchorMin = stripRect.anchorMax = new Vector2(0, 1);
        stripRect.pivot = new Vector2(0, 1);
        stripRect.anchoredPosition = new Vector2(12, -12);
        stripRect.sizeDelta = new Vector2(108, 4);

        // Icon centered around y=+72 (upper third)
        var icon = new GameObject("Card Icon", typeof(Image));
        icon.transform.SetParent(go.transform, false);
        var iconImage = icon.GetComponent<Image>();
        iconImage.sprite = diamondSprite;
        iconImage.color = new Color(0.4f, 1f, 1f, 0.24f);
        iconImage.raycastTarget = false;
        var iconRect = icon.GetComponent<RectTransform>();
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = new Vector2(0, 78);
        iconRect.sizeDelta = new Vector2(76, 76);

        var iconCore = new GameObject("Card Icon Core", typeof(Image));
        iconCore.transform.SetParent(go.transform, false);
        var iconCoreImage = iconCore.GetComponent<Image>();
        iconCoreImage.sprite = circleSprite;
        iconCoreImage.color = new Color(0.9f, 1f, 1f, 0.46f);
        iconCoreImage.raycastTarget = false;
        var iconCoreRect = iconCore.GetComponent<RectTransform>();
        iconCoreRect.anchorMin = iconCoreRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconCoreRect.pivot = new Vector2(0.5f, 0.5f);
        iconCoreRect.anchoredPosition = new Vector2(0, 78);
        iconCoreRect.sizeDelta = new Vector2(24, 24);

        var energyRing = new GameObject("Card Energy Ring", typeof(Image));
        energyRing.transform.SetParent(go.transform, false);
        var energyRingImage = energyRing.GetComponent<Image>();
        energyRingImage.sprite = circleSprite;
        energyRingImage.color = new Color(0.42f, 1f, 1f, 0.10f);
        energyRingImage.raycastTarget = false;
        var energyRingRect = energyRing.GetComponent<RectTransform>();
        energyRingRect.anchorMin = energyRingRect.anchorMax = new Vector2(0.5f, 0.5f);
        energyRingRect.pivot = new Vector2(0.5f, 0.5f);
        energyRingRect.anchoredPosition = new Vector2(0, 78);
        energyRingRect.sizeDelta = new Vector2(100, 100);

        var raritySpike = new GameObject("Card Rarity Spike", typeof(Image));
        raritySpike.transform.SetParent(go.transform, false);
        var raritySpikeImage = raritySpike.GetComponent<Image>();
        raritySpikeImage.sprite = diamondSprite;
        raritySpikeImage.color = new Color(0.42f, 1f, 1f, 0.30f);
        raritySpikeImage.raycastTarget = false;
        var raritySpikeRect = raritySpike.GetComponent<RectTransform>();
        raritySpikeRect.anchorMin = raritySpikeRect.anchorMax = new Vector2(1f, 1f);
        raritySpikeRect.pivot = new Vector2(1f, 1f);
        raritySpikeRect.anchoredPosition = new Vector2(-22, -22);
        raritySpikeRect.sizeDelta = new Vector2(28, 28);

        var bottomRail = new GameObject("Card Bottom Rail", typeof(Image));
        bottomRail.transform.SetParent(go.transform, false);
        var bottomRailImage = bottomRail.GetComponent<Image>();
        bottomRailImage.color = new Color(0.4f, 1f, 1f, 0.30f);
        if (UseCardPartsV2 && cardBottomRailCyanV2Sprite != null)
            ApplySimpleSprite(bottomRailImage, cardBottomRailCyanV2Sprite, Color.white);
        bottomRailImage.raycastTarget = false;
        var bottomRailRect = bottomRail.GetComponent<RectTransform>();
        bottomRailRect.anchorMin = bottomRailRect.anchorMax = new Vector2(0.5f, 0f);
        bottomRailRect.pivot = new Vector2(0.5f, 0f);
        bottomRailRect.anchoredPosition = new Vector2(0, 14);
        bottomRailRect.sizeDelta = new Vector2(210, 4);

        // Rarity row near the bottom (y=-148)
        var rarityPlate = new GameObject("Card Rarity Plate", typeof(Image));
        rarityPlate.transform.SetParent(go.transform, false);
        var rarityPlateImage = rarityPlate.GetComponent<Image>();
        rarityPlateImage.color = new Color(0.004f, 0.018f, 0.024f, 0.86f);
        if (UseCardPartsV2 && cardRarityPlateBasicV2Sprite != null)
            ApplySimpleSprite(rarityPlateImage, cardRarityPlateBasicV2Sprite, Color.white);
        rarityPlateImage.raycastTarget = false;
        var rarityPlateRect = rarityPlate.GetComponent<RectTransform>();
        rarityPlateRect.anchorMin = rarityPlateRect.anchorMax = new Vector2(0.5f, 0.5f);
        rarityPlateRect.pivot = new Vector2(0.5f, 0.5f);
        rarityPlateRect.anchoredPosition = new Vector2(0, -148);
        rarityPlateRect.sizeDelta = new Vector2(178, 28);

        var rarityText = CreateText("Card Rarity Text", go.transform, new Vector2(0, -148), TextAnchor.MiddleCenter, 16, new Color(1f, 0.94f, 0.42f));
        rarityText.text = "";
        rarityText.fontStyle = FontStyle.Bold;
        rarityText.rectTransform.sizeDelta = new Vector2(168, 26);

        var button = go.GetComponent<Button>();
        var colors = button.colors;
        colors.highlightedColor = new Color(0.22f, 0.26f, 0.31f);
        colors.pressedColor = new Color(0.9f, 0.68f, 0.25f);
        button.colors = colors;

        // Description label  Erect MUST be entirely below level text rect.
        // Level text: y=-50, h=22, pivot 0.5 →rect y=-61..-39
        // Description: y=-100, h=60, pivot 0.5 →rect y=-130..-70 (9px gap below level)
        // Rarity plate: y=-148, h=28 →rect y=-162..-134 (4px gap below desc)
        var text = CreateText("Label", go.transform, new Vector2(0, -100), TextAnchor.UpperCenter, 14, Color.white);
        text.rectTransform.sizeDelta = new Vector2(248, 60);
        text.fontStyle = FontStyle.Normal;
        text.lineSpacing = 1.02f;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 11;
        text.resizeTextMaxSize = 14;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;

        // Title plate at y=-18, bigger
        var titlePlate = new GameObject("Card Title Plate", typeof(Image));
        titlePlate.transform.SetParent(go.transform, false);
        var titlePlateImage = titlePlate.GetComponent<Image>();
        titlePlateImage.color = new Color(0.002f, 0.012f, 0.018f, 0.92f);
        if (UseCardPartsV2 && cardTitlePlateV2Sprite != null)
            ApplySimpleSprite(titlePlateImage, cardTitlePlateV2Sprite, Color.white);
        titlePlateImage.raycastTarget = false;
        var titlePlateRect = titlePlate.GetComponent<RectTransform>();
        titlePlateRect.anchorMin = titlePlateRect.anchorMax = new Vector2(0.5f, 0.5f);
        titlePlateRect.pivot = new Vector2(0.5f, 0.5f);
        titlePlateRect.anchoredPosition = new Vector2(0, -18);
        titlePlateRect.sizeDelta = new Vector2(248, 36);

        var titleText = CreateText("Card Title Text", go.transform, new Vector2(0, -18), TextAnchor.MiddleCenter, 21, Color.white);
        titleText.text = "";
        titleText.fontStyle = FontStyle.Bold;
        titleText.rectTransform.sizeDelta = new Vector2(240, 34);
        titleText.resizeTextForBestFit = true;
        titleText.resizeTextMinSize = 16;
        titleText.resizeTextMaxSize = 21;

        var levelPlate = new GameObject("Card Level Plate", typeof(Image));
        levelPlate.transform.SetParent(go.transform, false);
        var levelPlateImage = levelPlate.GetComponent<Image>();
        levelPlateImage.color = new Color(0.004f, 0.02f, 0.026f, 0.64f);
        if (UseCardPartsV2 && cardLevelPlateV2Sprite != null)
            ApplySimpleSprite(levelPlateImage, cardLevelPlateV2Sprite, Color.white);
        levelPlateImage.raycastTarget = false;
        var levelPlateRect = levelPlate.GetComponent<RectTransform>();
        levelPlateRect.anchorMin = levelPlateRect.anchorMax = new Vector2(0.5f, 0.5f);
        levelPlateRect.pivot = new Vector2(0.5f, 0.5f);
        levelPlateRect.anchoredPosition = new Vector2(0, -50);
        levelPlateRect.sizeDelta = new Vector2(240, 22);

        // Level text (e.g., "Lv 0 →Lv 1") at y=-50
        var levelText = CreateText("Card Level Text", go.transform, new Vector2(0, -50), TextAnchor.MiddleCenter, 16, new Color(0.82f, 1f, 0.96f));
        levelText.text = "";
        levelText.fontStyle = FontStyle.Bold;
        levelText.rectTransform.sizeDelta = new Vector2(240, 22);
        levelText.resizeTextForBestFit = true;
        levelText.resizeTextMinSize = 12;
        levelText.resizeTextMaxSize = 16;

        // Badge (route label e.g., "SPEED Lv 1/3") at top
        var badge = CreateText("Card Badge", go.transform, new Vector2(0, 156), TextAnchor.MiddleCenter, 16, new Color(0.72f, 1f, 1f));
        badge.text = "";
        badge.rectTransform.sizeDelta = new Vector2(248, 28);
        badge.fontStyle = FontStyle.Bold;
        return button;
    }

    Button CreateWideButton(string name, Transform parent, Vector2 position, Vector2 size)
    {
        var go = new GameObject(name, typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        var image = go.GetComponent<Image>();
        image.color = new Color(0.05f, 0.14f, 0.18f, 0.95f);
        var outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(0.35f, 1f, 1f, 0.28f);
        outline.effectDistance = new Vector2(1.2f, -1.2f);
        var button = go.GetComponent<Button>();
        SetButtonTint(button, image.color, new Color(0.09f, 0.28f, 0.34f));
        var text = CreateText("Label", go.transform, Vector2.zero, TextAnchor.MiddleCenter, 18, new Color(0.75f, 1f, 1f));
        text.rectTransform.sizeDelta = size - new Vector2(16, 8);
        text.fontStyle = FontStyle.Bold;
        return button;
    }

    void SetButtonTint(Button button, Color normal, Color highlight)
    {
        var image = button.GetComponent<Image>();
        image.color = normal;
        var colors = button.colors;
        colors.normalColor = normal;
        colors.highlightedColor = highlight;
        colors.selectedColor = highlight;
        colors.pressedColor = new Color(0.9f, 0.78f, 0.35f);
        button.colors = colors;
    }

    // Apply a V2 sprite to a button's background Image.
    // Sets color to white so the sprite renders at full brightness.
    // Hover = subtle cyan tint, Pressed = warm yellow (consistent with game palette).
    void SetButtonSpriteV2(Button button, Sprite sprite)
    {
        if (!UseButtonV2 || sprite == null || button == null) return;
        var img = button.GetComponent<Image>();
        if (img == null) return;
        img.sprite = sprite;
        img.type = Image.Type.Simple;
        img.preserveAspect = false;
        img.color = Color.white;
        var colors = button.colors;
        colors.normalColor    = Color.white;
        colors.highlightedColor = new Color(0.82f, 1f, 1f, 1f);
        colors.selectedColor    = colors.highlightedColor;
        colors.pressedColor     = new Color(1f, 0.90f, 0.38f, 1f);
        colors.disabledColor    = new Color(0.55f, 0.55f, 0.55f, 0.72f);
        button.colors = colors;
    }

#if false
    void BuildUpgrades()
    {
        upgradePool.Add(new Upgrade("デュアルビット", "弾を1発追加する。", () => bulletCount++));
        upgradePool.Add(new Upgrade("クロック加速", "攻撃速度 +12%。", () => fireRate *= 1.12f));
        upgradePool.Add(new Upgrade("コア照射", "データエッグの光の範囲 +10%。", () => { lightRadius *= 1.1f; lantern.GetChild(0).localScale = Vector3.one * lightRadius * 1.75f; }));
        upgradePool.Add(new Upgrade("パワーコード", "攻撃力 +18%。", () => bulletDamage *= 1.18f));
        upgradePool.Add(new Upgrade("クイックブート", "移動速度 +10%。", () => moveSpeed *= 1.1f));
        upgradePool.Add(new Upgrade("リカバリーパッチ", "自分とデータエッグを少し回復する。", () => { playerHp = Mathf.Min(playerMaxHp, playerHp + 1.6f); lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 3.2f); }));
        upgradePool.Add(new Upgrade("マグネット回収", "データ回収範囲 +20%。", () => pickupRange *= 1.2f));
        upgradePool.Add(new Upgrade("クラッシュバースト", "敵を倒すと爆発することがある。", () => explodeChance = Mathf.Min(0.42f, explodeChance + 0.1f)));
        upgradePool.Add(new Upgrade("ガードリング", "近くの敵を焼く小さな盾を得る。", () => { orbitShield = true; orbitDamage *= 1.12f; }));
        upgradePool.Add(new Upgrade("HPメモリ", "最大HP +1。少し回復する。", () => { playerMaxHp += 1f; playerHp = Mathf.Min(playerMaxHp, playerHp + 1.2f); }));
        upgradePool.Add(new Upgrade("コア修復", "データエッグの最大HP +2。修復もする。", () => { lanternMaxHp += 2f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 3f); }));
        upgradePool.Add(new Upgrade("ロングショット", "弾速と射程を少し強化する。", () => { bulletSpeed *= 1.1f; bulletLifeMultiplier *= 1.06f; }));
        upgradePool.Add(new Upgrade("ピアスコード", "弾が敵を1体貫通する。", () => bulletPierce++));
        upgradePool.Add(new Upgrade("ヘビーパケチE��", "弾が少し大きくなり、攻撃力も上がる。", () => { bulletSize *= 1.12f; bulletDamage *= 1.08f; }));
        upgradePool.Add(new Upgrade("データ圧縮", "データ獲得量 +12%。", () => dataMultiplier *= 1.12f));
        upgradePool.Add(new Upgrade("自己修復AI", "データ回収時にたまにHP回復。", () => pickupHealChance = Mathf.Min(0.34f, pickupHealChance + 0.08f)));
        upgradePool.Add(new Upgrade("対ボス解析", "ボスへのダメージ +18%。", () => bossDamageMultiplier *= 1.18f));
        upgradePool.Add(new Upgrade("ショックボディ", "接触してきた敵へ反撃ダメージ。", () => contactBurst += 0.65f));
        upgradePool.Add(new Upgrade("リング増幅", "ガードリングの威力を上げる。", () => { orbitShield = true; orbitDamage *= 1.22f; }));
        upgradePool.Add(new Upgrade("バースト確率", "クラッシュバーストの確率をさらに上げる。", () => explodeChance = Mathf.Min(0.55f, explodeChance + 0.12f)));
        upgradePool.Add(new Upgrade("ワイドショット", "弾数+1、ただし弾速が少し下がる。", () => { bulletCount++; bulletSpeed *= 0.94f; }));
        upgradePool.Add(new Upgrade("SPEED専用: チェインスパーク", "命中後、近くの敵へ光が連鎖する。\n連鎖回数 +1。", "★★★", () => speedChainBonus++));
        upgradePool.Add(new Upgrade("SPEED専用: ミラージュビット", "相棒の残像が自動で追撃弾を放つ。", "★★★", () => { speedEchoActive = true; speedEchoTimer = 0f; }));
        upgradePool.Add(new Upgrade("GUARD専用: コアパルス", "データエッグから防衛波が定期発生。\n近くの敵を押し返す。", "★★★", () => { guardPulseUpgrade = true; guardPulseTimer = 0f; }));
        upgradePool.Add(new Upgrade("GUARD専用: リフレクトウォール", "近くの敵弾を味方弾に変換する。", "★★★", () => reflectShield = true));
        upgradePool.Add(new Upgrade("GUARD専用: タワーノックバック", "接触した敵を半径内で強く弾き返す。\n防衛ラインを死守する。", "★★★", () => { playerKnockback += 6.0f; orbitKnockback += 4.0f; orbitShield = true; }));
        upgradePool.Add(new Upgrade("衝撃ノックバック", "接触敵を押し返す力を得る。\nガードリングも押し返し効果付与。", "★★★", () => { playerKnockback += 3.6f; orbitKnockback += 2.6f; }));
        upgradePool.Add(new Upgrade("仲間リンク: Nova", "相棒に攻撃回路を接続。\n火力より手数を少し補うリンク。", "★★★", () => { allyNova = true; fireRate *= 1.03f; }));
        upgradePool.Add(new Upgrade("仲間リンク: Bulwark", "相棒に防衛回路を接続。\nコア耐久を少し補い、近距離を守る。", "★★★", () => { allyBulwark = true; lanternMaxHp += 2f; lanternHp += 2f; }));
        upgradePool.Add(new Upgrade("仲間リンク: Siphon", "相棒に回収回路を接続。\n近くのデータを拾いやすくする。", "★★★", () => { allySiphon = true; pickupRange *= 1.08f; dataMultiplier *= 1.04f; }));
        upgradePool.Add(new Upgrade("仲間リンク: Phase", "相棒に回避回路を接続。\n近くの敵弾を一定間隔で位相回避する。", "★★★", () => { allyPhase = true; moveSpeed *= 1.04f; phaseDodgeTimer = 0f; }));

        upgradePool.Add(new Upgrade("チェイン回路", "命中後に近くの敵へ光が連鎖する。\n連鎖回数 +1。", "★★★", () => speedChainBonus++));
        upgradePool.Add(new Upgrade("チェイン増幅", "連鎖弾の威力 +28%、射程も少し伸びる。", "★★★", () => { bulletLifeMultiplier *= 1.08f; bulletDamage *= 1.06f; speedChainBonus = Mathf.Max(speedChainBonus, 1); }));
        upgradePool.Add(new Upgrade("リフレクトパネル", "近くの敵弾を味方弾に変換する。\nどのルートでも採用可能。", "★★★", () => { reflectShield = true; }));
        upgradePool.Add(new Upgrade("カウンターブースト", "反射と接触反撃のダメージを伸ばす。", "★★★", () => { contactBurst += 0.8f; reflectShield = true; bulletDamage *= 1.04f; }));
        upgradePool.Add(new Upgrade("エコーレゾナンス", "残像追撃を起動し、連鎖と相乗する。", "★★★", () => { speedEchoActive = true; speedEchoTimer = 0f; speedChainBonus++; }));
        upgradePool.Add(new Upgrade("パルスチャージ", "5体倒すごとに小さな範囲ダメージを発生させる。", "★★★", () => { explodeChance = Mathf.Min(0.55f, explodeChance + 0.06f); guardPulseUpgrade = true; }));
        upgradePool.Add(new Upgrade("フェーズリーチ", "回収範囲を広げ、Phase回避クールを即時リセット。", "★★★", () => { pickupRange *= 1.12f; if (allyPhase) phaseDodgeTimer = 0f; }));
        upgradePool.Add(new Upgrade("ハイブリッドコア", "弾数+1と移動6%、軽量ハイブリッド。", "★★★", () => { bulletCount++; moveSpeed *= 1.06f; bulletDamage *= 0.96f; }));

        // ─────────────────────────────────────────────────────────
        // SPEED 系追加モジュール (POWER 一強解消、機動ビルド�E選択肢を拡張)
        // ─────────────────────────────────────────────────────────
        upgradePool.Add(new Upgrade("ターボブースチE,
            "移動と連射��同時に底上げ。\n移動+15% / 連射+10%。",
            "★★★,
            () => { moveSpeed *= 1.15f; fireRate *= 1.10f; }));
        upgradePool.Add(new Upgrade("クイチE��リローチE,
            "封E��サイクルを高速化。\n連射+18% / 弾送E+12%。",
            "★★★,
            () => { fireRate *= 1.18f; bulletSpeed *= 1.12f; }));
        upgradePool.Add(new Upgrade("マルチチェイン",
            "連鎖回路を多重化。\nチェイン +2 hop / 連鎖威力 +6%。",
            "★★★,
            () => { speedChainBonus += 2; bulletDamage *= 1.06f; }));
        upgradePool.Add(new Upgrade("SPEED専用: ベロシチE��チャージ",
            "機動 = 火力。\n移動+12% / 攻撃+12% / 弾送E+8%。",
            "★★★,
            () => { moveSpeed *= 1.12f; bulletDamage *= 1.12f; bulletSpeed *= 1.08f; }));
        upgradePool.Add(new Upgrade("SPEED専用: ミラージュバースチE,
            "残像強匁E+ 連射��速。\n残像追撃破勁E/ 連射+12% / 移動+6%。",
            "★★★,
            () => { speedEchoActive = true; speedEchoTimer = 0f; fireRate *= 1.12f; moveSpeed *= 1.06f; }));

        // ─────────────────────────────────────────────────────────
        // GUARD 系追加モジュール (タワーチE��フェンス基軸、コア防衛�E選択肢を拡張)
        // ─────────────────────────────────────────────────────────
        upgradePool.Add(new Upgrade("コアキャパシチE��",
            "両方のHP最大値を底上げ。\n最大HP +2 / コア最大HP +4 / 両方回復。",
            "★★★,
            () => {
                playerMaxHp += 2f; playerHp = Mathf.Min(playerMaxHp, playerHp + 2f);
                lanternMaxHp += 4f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 4f);
            }));
        upgradePool.Add(new Upgrade("リング拡張",
            "ガードリングを多重化。\n威力 +25% / 押し返し +1.8 / 範囲効果。",
            "★★★,
            () => { orbitShield = true; orbitDamage *= 1.25f; orbitKnockback += 1.8f; }));
        upgradePool.Add(new Upgrade("スパイクシェル",
            "接触対策を強化。\n接触反撃 +1.5 / 押し返し +3.0 / 接触dmg軽渁E+12%。",
            "★★★,
            () => { contactBurst += 1.5f; playerKnockback += 3.0f; playerMaxHp += 1f; playerHp = Mathf.Min(playerMaxHp, playerHp + 1f); }));
        upgradePool.Add(new Upgrade("GUARD専用: トライリング",
            "ガードリング3層化。\n威力 +45% / 押し返し +3.5 / 範囲が大きく広がる。",
            "★★★,
            () => { orbitShield = true; orbitDamage *= 1.45f; orbitKnockback += 3.5f; }));
        upgradePool.Add(new Upgrade("GUARD専用: ガーチE��アンウェーチE,
            "コアパルスを増幁E��En防衛波起動/ コアHP最大 +3 / 反射起動。",
            "★★★,
            () => {
                guardPulseUpgrade = true; guardPulseTimer = 0f;
                lanternMaxHp += 3f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 3f);
                reflectShield = true;
            }));

        // ── 汎用バランス枠 (どのビルドにも合ぁE��庸) ──
        upgradePool.Add(new Upgrade("スチE��ィハ�EチE,
            "耐乁E��満遍なく底上げ。\n最大HP +1 / コア最大HP +2 / 両方回復。",
            "★★★,
            () => {
                playerMaxHp += 1f; playerHp = Mathf.Min(playerMaxHp, playerHp + 1f);
                lanternMaxHp += 2f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 2f);
            }));
        upgradePool.Add(new Upgrade("オーラブ�EスチE,
            "周辺攻撃を強化。\n近接オーラ +1.2 / オーラ半征E+0.35m。",
            "★★★,
            () => { playerAuraDamage += 1.2f; playerAuraRadius += 0.35f; }));

        // ── 新モジュール ───────────────────────────────────────────
        upgradePool.Add(new Upgrade("クリティカルコーチE, "15%の確率��攻撃が2.5倍ダメージになる。", "★★★, () => critChance += 0.15f));
        upgradePool.Add(new Upgrade("波紋弾", "弾が命中するたびに小さな衝撃波が広がる。", "★★★, () => shockwaveOnHit = true));
        upgradePool.Add(new Upgrade("プラズマオーラ", "プレイヤー周囲の近接敵に継続ダメージ。", "★★★, () => playerAuraDamage += 0.8f));
        upgradePool.Add(new Upgrade("波動砲", "8体撃破ごとに8方向へ弾が広がる。", "★★★, () => waveKillActive = true));
        upgradePool.Add(new Upgrade("超加速弾", "弾送E60%。封E���E短くなる。", "★★★, () => { bulletSpeed *= 1.6f; bulletLifeMultiplier *= 0.65f; }));
        upgradePool.Add(new Upgrade("データフォージ", "敵撃破ごとにボ�Eナスデータを獲得する。", "★★★, () => bonusDataOnKill += 0.5f));
        upgradePool.Add(new Upgrade("鎧貫送E, "弾が追加で1体貫通し、E��裁E��への火力25%。", "★★★, () => { bulletPierce++; bossDamageMultiplier *= 1.25f; }));
        upgradePool.Add(new Upgrade("HP吸収弾", "弾命中のたびに自分��少し回復する。", "★★★, () => bulletLifesteal = true));

        // ── シグネチャ強化モジュール (該当機構を持つキャラのみ出現) ──
        // ファンネル系 (Halo Caster用)  E全機同時突撃ベ�Eスの設訁E
        upgradePool.Add(new Upgrade("スウォームプロトコル",
            "群体�Eロトコル展開。\n周回ビチE�� +1橁E(最大6橁E、発封E��イクル -8%。",
            "★★★,
            () => {
                if (funnelBitCount > 0 && funnelBitCount < FunnelMaxBits)
                    EnsureFunnelBits(funnelBitCount + 1, new Color(0.55f, 0.92f, 1f));
                funnelBitFireRate *= 0.92f;  // bit数 cap 後も無駁E��ならなぁE��ぁE��ぁEfire rate bump
            }));
        upgradePool.Add(new Upgrade("ハイパ�Eサイクル",
            "ビット制御を加速。\n発封E��隁E-38%、定常 DPS が大幁E��増加。",
            "★★★,
            () => funnelBitFireRate *= 0.62f));
        upgradePool.Add(new Upgrade("ストライクキャスケーチE,
            "突撃のクールを大幁E��縮。\n離脱間隔 -50%、�E機突撃が常用化。",
            "★★★,
            () => funnelDetachInterval *= 0.50f));
        upgradePool.Add(new Upgrade("キネティチE��スパイク",
            "突撃に質量加速を付与。\n離脱ダメージ +90% (全機�E威力が同時に上がめE。",
            "★★★,
            () => funnelDetachDamageMultiplier *= 1.90f));

        // レーザー系 (Pulse Hydra用)  E高火力ピーキー設訁E
        upgradePool.Add(new Upgrade("フォトンサージ",
            "光子流を増幁E��Enレーザー継続ダメージ +60%。",
            "★★★,
            () => laserDamageMultiplier *= 1.60f));
        upgradePool.Add(new Upgrade("ホライゾンレンズ",
            "焦点を遠方へ拡張。\n封E��E+3m、ビーム威力 +15%。",
            "★★★,
            () => { laserMaxLength += 3f; laserDamageMultiplier *= 1.15f; }));
        upgradePool.Add(new Upgrade("スプリチE��ビ�Eム",
            "ビ�Eムを�E散展開。\n直線上�E敵への副次ダメージ 60% →100% (主弾と同筁E。",
            "★★★,
            () => laserChainDamageMul = Mathf.Min(1.0f, laserChainDamageMul + 0.40f)));

        // オーラ系 (Wraith Lynx / プラズマオーラ取得時)
        upgradePool.Add(new Upgrade("フィールド�Eロジェクター",
            "近接フィールドを増幁E��封E��Enオーラ半征E+0.7m (面制圧力が体感で増大)。",
            "★★★,
            () => playerAuraRadius += 0.7f));
        upgradePool.Add(new Upgrade("インフェルノハロー",
            "オーラに熱量を加える。\n継続ダメージ +4.5/秒。",
            "★★★,
            () => playerAuraDamage += 4.5f));

        // 吸血系 (HP吸収弾/Wraith Lynx)  E削るほど回復するゲームプランへ
        upgradePool.Add(new Upgrade("ヴァンパイアコア",
            "吸血回路を倍化。\n吸血 2倁E+ 撃破晁E+0.5 HP 回復 (Brute× / Boss×)。",
            "★★★,
            () => { lifestealAmount *= 2.0f; lifestealKillBonus += 0.5f; }));

        // ─────────────────────────────────────────────────────────
        // パ�Eトナー固有シグネチャ強匁E(吁E��ャラのアイチE��チE��チE��を伸ばぁE
        // IsUpgradeUnlocked で partnerStyle ゲート、E��んだ相棒以外には出現しなぁE        // ─────────────────────────────────────────────────────────
        // ① Cobalt Pup (style 1, バランス垁E
        upgradePool.Add(new Upgrade("バランスチャージ",
            "全方位�E出力を底上げ。\n攻撃+8% / 連射+8% / 移動+8% / 最大HP +1。",
            "★★★,
            () => {
                bulletDamage *= 1.08f; fireRate *= 1.08f; moveSpeed *= 1.08f;
                playerMaxHp += 1f; playerHp = Mathf.Min(playerMaxHp, playerHp + 1f);
            }));
        // ② Ember Drake (style 2, 弾幕型)
        upgradePool.Add(new Upgrade("フレアバレチE��",
            "弾幕展開を強化。\n弾数 +1 / 連射+12% / 単発威力 -8% (合計火力�E)。",
            "★★★,
            () => { bulletCount++; fireRate *= 1.12f; bulletDamage *= 0.92f; }));
        // ③ Sage Hare (style 3, 防衛型)
        upgradePool.Add(new Upgrade("エッグレゾナンス",
            "コアとの共鳴を強化。\nコア最大HP +6 / 修復 +4 / コア満タン晁E与dmg +20%。",
            "★★★,
            () => {
                lanternMaxHp += 6f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 4f);
                eggResonanceActive = true;
            }));
        // ④ Hex Cat (style 4, 連鎖型)
        upgradePool.Add(new Upgrade("チェインプロトコル",
            "弾命中時に最寁E��敵に連鎖。\nチェイン +1 hop / 連鎖威力 +50%。",
            "★★★,
            () => { speedChainBonus++; bulletDamage *= 1.08f; }));
        // ⑤ Drift Fox (style 5, 回避垁E
        upgradePool.Add(new Upgrade("ファントムスチE��チE,
            "回避サイクルを加速。\n回避クール -30% / 残像追撃を強化。",
            "★★★,
            () => { phaseDodgeTimer = 0f; speedEchoActive = true; speedChainBonus++; }));
        // ⑥ Iron Bear (style 6, 裁E��垁E
        upgradePool.Add(new Upgrade("アーマ�EローチE,
            "重裁E�Eロトコル展開。\n最大HP +3 / 接触ダメージ +1.5 / 移動-5%。",
            "★★★,
            () => {
                playerMaxHp += 3f; playerHp = Mathf.Min(playerMaxHp, playerHp + 3f);
                contactBurst += 1.5f; moveSpeed *= 0.95f;
            }));
        // ⑦ Wraith Lynx (style 7)  E既存ヴァンパイアコアとは別軸の近接系
        upgradePool.Add(new Upgrade("スラチE��ュサイクル",
            "近接接触を高速化。\n接触dmg +1.8 / オーラ半征E+0.3m。",
            "★★★,
            () => { contactBurst += 1.8f; playerAuraRadius += 0.3f; }));
        // ⑧ Halo Caster (style 9)  E既孁E個に加えた軌道強匁E
        upgradePool.Add(new Upgrade("センチネルピ�EチE��",
            "ビット軌道を最適化。\n周回半征E+25% / 発封E��E��が広がる。",
            "★★★,
            () => { funnelOrbitRadius *= 1.25f; }));
        // ⑨ Pulse Hydra (style 10)  E既孁E個に加えた持続強匁E
        upgradePool.Add(new Upgrade("コアハ�Eモニクス",
            "レーザー維持中の機動性UP。\nレーザー使用中の移動速度 +20% / レーザーDPS +15%。",
            "★★★,
            () => { laserMoveBonus = Mathf.Max(laserMoveBonus, 1.20f); laserDamageMultiplier *= 1.15f; }));
        // ⑩ Solar Anchor (style 11)  Eコアオーラ強匁E
        upgradePool.Add(new Upgrade("コアシンク",
            "コアオーラを増幁E��Enリング DPS +4 / 半征E+1.5m。",
            "★★★,
            () => {
                coreAuraDpsBonus += 4f;
                coreAuraRadiusBonus += 1.5f;
            }));
        // ─────────────────────────────────────────────────────────

        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Twin Core", "Nova Aegis専用。\n相棒追撃が2方向に増える。", "★★★, 1, () => { bulletCount++; allyShotTimer = 0f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Aegis Volley", "Nova Aegis専用。\n弾が広がり、盾リングも強くなる。", "★★★, 1, () => { bulletCount++; orbitShield = true; orbitDamage *= 1.14f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Data Bloom", "Photon Siphon専用。\nデータ獲得と連鎖弾を強化する。", "★★★, 2, () => { allySiphon = true; dataMultiplier *= 1.16f; pickupRange *= 1.08f; speedChainBonus++; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Photon Echo", "Photon Siphon専用。\n残像追撃と回収性能を伸ばす。", "★★★, 2, () => { speedEchoActive = true; pickupRange *= 1.1f; fireRate *= 1.04f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Sync Guard", "Core Bastion専用。\nコアが継続回復し、防衛波も強化。", "★★★, 3, () => { allyBulwark = true; lanternMaxHp += 3f; lanternHp += 3f; guardPulseUpgrade = true; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Bastion Wall", "Core Bastion専用。\n敵弾反射リング火力を強化する。", "★★★, 3, () => { reflectShield = true; orbitShield = true; orbitDamage *= 1.14f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Phantom Strike", "Nova Phantom専用。\n残像と連鎖威力を伸ばす。", "★★★, 4, () => { speedEchoActive = true; speedChainBonus++; bulletSpeed *= 1.08f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Mirage Burst", "Nova Phantom専用。\n移動と連射��追撃を一気に強化。", "★★★, 4, () => { moveSpeed *= 1.08f; fireRate *= 1.1f; bulletCount++; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Drift Counter", "Aegis Drift専用。\n反撃と反射強化、移動も伸びる。", "★★★, 5, () => { contactBurst += 1.2f; reflectShield = true; moveSpeed *= 1.06f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Aegis Pulse", "Aegis Drift専用。\nコア回復と防衛波を強化。", "★★★, 5, () => { guardPulseUpgrade = true; lanternMaxHp += 4f; lanternHp += 4f; orbitShield = true; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Spectral Drain", "Photon Wraith専用。\nデータと回復、移動を強化。", "★★★, 6, () => { dataMultiplier *= 1.22f; pickupHealChance = Mathf.Min(0.7f, pickupHealChance + 0.18f); moveSpeed *= 1.04f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Ghost Net", "Photon Wraith専用。\n回収範囲と弾速、回避を伸ばす。", "★★★, 6, () => { pickupRange *= 1.2f; bulletSpeed *= 1.08f; phaseDodgeTimer = 0f; }));
    }

#endif

    void BuildUpgrades()
    {
        upgradePool.Add(new Upgrade("デュアルビット", "弾を1発追加する。", () => bulletCount++));
        upgradePool.Add(new Upgrade("クロック加速", "攻撃速度 +12%。", () => fireRate *= 1.12f));
        upgradePool.Add(new Upgrade("コア照射", "データエッグの光の範囲 +10%。", () => { lightRadius *= 1.1f; lantern.GetChild(0).localScale = Vector3.one * lightRadius * 1.75f; }));
        upgradePool.Add(new Upgrade("パワーコード", "攻撃力 +18%。", () => bulletDamage *= 1.18f));
        upgradePool.Add(new Upgrade("クイックブート", "移動速度 +10%。", () => moveSpeed *= 1.1f));
        upgradePool.Add(new Upgrade("リカバリーパッチ", "自分とデータエッグを少し回復する。", () => { playerHp = Mathf.Min(playerMaxHp, playerHp + 1.6f); lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 3.2f); }));
        upgradePool.Add(new Upgrade("マグネット回収", "データ回収範囲 +20%。", () => pickupRange *= 1.2f));
        upgradePool.Add(new Upgrade("クラッシュバースト", "敵を倒すと爆発することがある。", () => explodeChance = Mathf.Min(0.42f, explodeChance + 0.1f)));
        upgradePool.Add(new Upgrade("ガードリング", "近くの敵を焼く小さな盾を得る。", () => { orbitShield = true; orbitDamage *= 1.12f; }));
        upgradePool.Add(new Upgrade("HPメモリ", "最大HP +1。少し回復する。", () => { playerMaxHp += 1f; playerHp = Mathf.Min(playerMaxHp, playerHp + 1.2f); }));
        upgradePool.Add(new Upgrade("コア修復", "データエッグの最大HP +2。修復もする。", () => { lanternMaxHp += 2f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 3f); }));
        upgradePool.Add(new Upgrade("ロングショット", "弾速と射程を少し強化する。", () => { bulletSpeed *= 1.1f; bulletLifeMultiplier *= 1.06f; }));
        upgradePool.Add(new Upgrade("ピアスコード", "弾が敵を1体貫通する。", () => bulletPierce++));
        upgradePool.Add(new Upgrade("ヘビーパケット", "弾が少し大きくなり、攻撃力も上がる。", () => { bulletSize *= 1.12f; bulletDamage *= 1.08f; }));
        upgradePool.Add(new Upgrade("データ圧縮", "データ獲得量 +12%。", () => dataMultiplier *= 1.12f));
        upgradePool.Add(new Upgrade("自己修復AI", "データ回収時にたまにHP回復。", () => pickupHealChance = Mathf.Min(0.34f, pickupHealChance + 0.08f)));
        upgradePool.Add(new Upgrade("対ボス解析", "ボスへのダメージ +18%。", () => bossDamageMultiplier *= 1.18f));
        upgradePool.Add(new Upgrade("ショックボディ", "接触してきた敵へ反撃ダメージ。", () => contactBurst += 0.65f));
        upgradePool.Add(new Upgrade("リング増幅", "ガードリングの威力を上げる。", () => { orbitShield = true; orbitDamage *= 1.22f; }));
        upgradePool.Add(new Upgrade("バースト確率", "クラッシュバーストの確率をさらに上げる。", () => explodeChance = Mathf.Min(0.55f, explodeChance + 0.12f)));
        upgradePool.Add(new Upgrade("ワイドショット", "弾数+1、ただし弾速が少し下がる。", () => { bulletCount++; bulletSpeed *= 0.94f; }));

        upgradePool.Add(new Upgrade("SPEED専用: チェインスパーク", "命中後、近くの敵へ光が連鎖する。\n連鎖回数 +1。", "★★★", () => speedChainBonus++));
        upgradePool.Add(new Upgrade("SPEED専用: ミラージュビット", "相棒の残像が自動で追撃弾を放つ。", "★★★", () => { speedEchoActive = true; speedEchoTimer = 0f; }));
        upgradePool.Add(new Upgrade("SPEED専用: ベロシティチャージ", "機動 = 火力。\n移動+12% / 攻撃+12% / 弾速+8%。", "★★★", () => { moveSpeed *= 1.12f; bulletDamage *= 1.12f; bulletSpeed *= 1.08f; }));
        upgradePool.Add(new Upgrade("SPEED専用: ミラージュバースト", "残像強化と連射加速。\n残像追撃 / 連射+12% / 移動+6%。", "★★★", () => { speedEchoActive = true; speedEchoTimer = 0f; fireRate *= 1.12f; moveSpeed *= 1.06f; }));

        upgradePool.Add(new Upgrade("GUARD専用: コアパルス", "データエッグから防衛波が定期発生。\n近くの敵を押し返す。", "★★★", () => { guardPulseUpgrade = true; guardPulseTimer = 0f; }));
        upgradePool.Add(new Upgrade("GUARD専用: リフレクトウォール", "近くの敵弾を味方弾に変換する。", "★★★", () => reflectShield = true));
        upgradePool.Add(new Upgrade("GUARD専用: タワーノックバック", "接触した敵を強く弾き返す。\n防衛ラインを死守する。", "★★★", () => { playerKnockback += 6.0f; orbitKnockback += 4.0f; orbitShield = true; }));
        upgradePool.Add(new Upgrade("GUARD専用: トライリング", "ガードリング3層化。\n威力 +45% / 押し返し +3.5。", "★★★", () => { orbitShield = true; orbitDamage *= 1.45f; orbitKnockback += 3.5f; }));
        upgradePool.Add(new Upgrade("GUARD専用: ガーディアンウェーブ", "防衛波起動。\nコアHP最大 +3 / 反射起動。", "★★★", () => { guardPulseUpgrade = true; guardPulseTimer = 0f; lanternMaxHp += 3f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 3f); reflectShield = true; }));

        upgradePool.Add(new Upgrade("仲間リンク: Nova", "相棒に攻撃回路を接続。\n火力より手数を少し補うリンク。", "★★★", () => { allyNova = true; fireRate *= 1.03f; }));
        upgradePool.Add(new Upgrade("仲間リンク: Bulwark", "相棒に防衛回路を接続。\nコア耐久を少し補い、近距離を守る。", "★★★", () => { allyBulwark = true; lanternMaxHp += 2f; lanternHp += 2f; }));
        upgradePool.Add(new Upgrade("仲間リンク: Siphon", "相棒に回収回路を接続。\n近くのデータを拾いやすくする。", "★★★", () => { allySiphon = true; pickupRange *= 1.08f; dataMultiplier *= 1.04f; }));
        upgradePool.Add(new Upgrade("仲間リンク: Phase", "相棒に回避回路を接続。\n近くの敵弾を一定間隔で位相回避する。", "★★★", () => { allyPhase = true; moveSpeed *= 1.04f; phaseDodgeTimer = 0f; }));

        upgradePool.Add(new Upgrade("チェイン回路", "命中後に近くの敵へ光が連鎖する。\n連鎖回数 +1。", "★★★", () => speedChainBonus++));
        upgradePool.Add(new Upgrade("チェイン増幅", "連鎖弾の威力 +28%、射程も少し伸びる。", "★★★", () => { bulletLifeMultiplier *= 1.08f; bulletDamage *= 1.06f; speedChainBonus = Mathf.Max(speedChainBonus, 1); }));
        upgradePool.Add(new Upgrade("リフレクトパネル", "近くの敵弾を味方弾に変換する。\nどのルートでも採用可能。", "★★★", () => reflectShield = true));
        upgradePool.Add(new Upgrade("カウンターブースト", "反射と接触反撃のダメージを伸ばす。", "★★★", () => { contactBurst += 0.8f; reflectShield = true; bulletDamage *= 1.04f; }));
        upgradePool.Add(new Upgrade("エコーレゾナンス", "残像追撃を起動し、連鎖と相乗する。", "★★★", () => { speedEchoActive = true; speedEchoTimer = 0f; speedChainBonus++; }));
        upgradePool.Add(new Upgrade("パルスチャージ", "5体倒すごとに小さな範囲ダメージを発生させる。", "★★★", () => { explodeChance = Mathf.Min(0.55f, explodeChance + 0.06f); guardPulseUpgrade = true; }));
        upgradePool.Add(new Upgrade("フェーズリーチ", "回収範囲を広げ、Phase回避クールを即時リセット。", "★★★", () => { pickupRange *= 1.12f; if (allyPhase) phaseDodgeTimer = 0f; }));
        upgradePool.Add(new Upgrade("ハイブリッドコア", "弾数+1と移動6%、軽量ハイブリッド。", "★★★", () => { bulletCount++; moveSpeed *= 1.06f; bulletDamage *= 0.96f; }));
        upgradePool.Add(new Upgrade("ターボブースト", "移動と連射を同時に底上げ。\n移動+15% / 連射+10%。", "★★★", () => { moveSpeed *= 1.15f; fireRate *= 1.10f; }));
        upgradePool.Add(new Upgrade("クイックリロード", "射撃サイクルを高速化。\n連射+18% / 弾速+12%。", "★★★", () => { fireRate *= 1.18f; bulletSpeed *= 1.12f; }));
        upgradePool.Add(new Upgrade("マルチチェイン", "連鎖回路を多重化。\nチェイン +2 hop / 連鎖威力 +6%。", "★★★", () => { speedChainBonus += 2; bulletDamage *= 1.06f; }));
        upgradePool.Add(new Upgrade("コアキャパシティ", "両方のHP最大値を底上げ。\n最大HP +2 / コア最大HP +4 / 両方回復。", "★★★", () => { playerMaxHp += 2f; playerHp = Mathf.Min(playerMaxHp, playerHp + 2f); lanternMaxHp += 4f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 4f); }));
        upgradePool.Add(new Upgrade("リング拡張", "ガードリングを多重化。\n威力 +25% / 押し返し +1.8。", "★★★", () => { orbitShield = true; orbitDamage *= 1.25f; orbitKnockback += 1.8f; }));
        upgradePool.Add(new Upgrade("スパイクシェル", "接触対策を強化。\n接触反撃 +1.5 / 押し返し +3.0。", "★★★", () => { contactBurst += 1.5f; playerKnockback += 3.0f; playerMaxHp += 1f; playerHp = Mathf.Min(playerMaxHp, playerHp + 1f); }));
        upgradePool.Add(new Upgrade("ステディハート", "耐久を満遍なく底上げ。\n最大HP +1 / コア最大HP +2 / 両方回復。", "★★★", () => { playerMaxHp += 1f; playerHp = Mathf.Min(playerMaxHp, playerHp + 1f); lanternMaxHp += 2f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 2f); }));
        upgradePool.Add(new Upgrade("オーラブースト", "周辺攻撃を強化。\n近接オーラ +1.2 / オーラ半径+0.35m。", "★★★", () => { playerAuraDamage += 1.2f; playerAuraRadius += 0.35f; }));

        upgradePool.Add(new Upgrade("クリティカルコード", "15%の確率で攻撃が2.5倍ダメージになる。", "★★★", () => critChance += 0.15f));
        upgradePool.Add(new Upgrade("波紋弾", "弾が命中するたびに小さな衝撃波が広がる。", "★★★", () => shockwaveOnHit = true));
        upgradePool.Add(new Upgrade("プラズマオーラ", "プレイヤー周囲の近接敵に継続ダメージ。", "★★★", () => playerAuraDamage += 0.8f));
        upgradePool.Add(new Upgrade("波動砲", "8体撃破ごとに8方向へ弾が広がる。", "★★★", () => waveKillActive = true));
        upgradePool.Add(new Upgrade("超加速弾", "弾速+60%。射程は短くなる。", "★★★", () => { bulletSpeed *= 1.6f; bulletLifeMultiplier *= 0.65f; }));
        upgradePool.Add(new Upgrade("データフォージ", "敵撃破ごとにボーナスデータを獲得する。", "★★★", () => bonusDataOnKill += 0.5f));
        upgradePool.Add(new Upgrade("鎧貫通", "弾が追加で1体貫通し、装甲への火力+25%。", "★★★", () => { bulletPierce++; bossDamageMultiplier *= 1.25f; }));
        upgradePool.Add(new Upgrade("HP吸収弾", "弾命中のたびに自分を少し回復する。", "★★★", () => bulletLifesteal = true));

        upgradePool.Add(new Upgrade("スウォームプロトコル", "群体プロトコル展開。\n周回ビット +1枠、発射サイクル -8%。", "★★★", () => { if (funnelBitCount > 0 && funnelBitCount < FunnelMaxBits) EnsureFunnelBits(funnelBitCount + 1, new Color(0.55f, 0.92f, 1f)); funnelBitFireRate *= 0.92f; }));
        upgradePool.Add(new Upgrade("ハイパーサイクル", "ビット制御を加速。\n発射間隔 -38%。", "★★★", () => funnelBitFireRate *= 0.62f));
        upgradePool.Add(new Upgrade("ストライクキャスケード", "突撃のクールを大幅短縮。\n離脱間隔 -50%。", "★★★", () => funnelDetachInterval *= 0.50f));
        upgradePool.Add(new Upgrade("キネティックスパイク", "突撃に質量加速を付与。\n離脱ダメージ +90%。", "★★★", () => funnelDetachDamageMultiplier *= 1.90f));
        upgradePool.Add(new Upgrade("フォトンサージ", "光子流を増幅。\nレーザー継続ダメージ +60%。", "★★★", () => laserDamageMultiplier *= 1.60f));
        upgradePool.Add(new Upgrade("ホライゾンレンズ", "焦点を遠方へ拡張。\n射程+3m、ビーム威力 +15%。", "★★★", () => { laserMaxLength += 3f; laserDamageMultiplier *= 1.15f; }));
        upgradePool.Add(new Upgrade("スプリットビーム", "ビームを分散展開。\n副次ダメージを強化。", "★★★", () => laserChainDamageMul = Mathf.Min(1.0f, laserChainDamageMul + 0.40f)));
        upgradePool.Add(new Upgrade("フィールドプロジェクター", "近接フィールドを増幅。\nオーラ半径+0.7m。", "★★★", () => playerAuraRadius += 0.7f));
        upgradePool.Add(new Upgrade("インフェルノハロー", "オーラに熱量を加える。\n継続ダメージ +4.5/秒。", "★★★", () => playerAuraDamage += 4.5f));
        upgradePool.Add(new Upgrade("ヴァンパイアコア", "吸血回路を倍化。\n吸血2倍 + 撃破時HP回復。", "★★★", () => { lifestealAmount *= 2.0f; lifestealKillBonus += 0.5f; }));

        upgradePool.Add(new Upgrade("バランスチャージ", "全方位の出力を底上げ。\n攻撃+8% / 連射+8% / 移動+8% / 最大HP +1。", "★★★", () => { bulletDamage *= 1.08f; fireRate *= 1.08f; moveSpeed *= 1.08f; playerMaxHp += 1f; playerHp = Mathf.Min(playerMaxHp, playerHp + 1f); }));
        upgradePool.Add(new Upgrade("フレアバレット", "弾幕展開を強化。\n弾数 +1 / 連射+12% / 単発威力 -8%。", "★★★", () => { bulletCount++; fireRate *= 1.12f; bulletDamage *= 0.92f; }));
        upgradePool.Add(new Upgrade("エッグレゾナンス", "コアとの共鳴を強化。\nコア最大HP +6 / 修復 +4。", "★★★", () => { lanternMaxHp += 6f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 4f); eggResonanceActive = true; }));
        upgradePool.Add(new Upgrade("チェインプロトコル", "弾命中時に最寄りの敵へ連鎖。\nチェイン +1 hop / 連鎖威力 +50%。", "★★★", () => { speedChainBonus++; bulletDamage *= 1.08f; }));
        upgradePool.Add(new Upgrade("ファントムステップ", "回避サイクルを加速。\n回避クール -30% / 残像追撃を強化。", "★★★", () => { phaseDodgeTimer = 0f; speedEchoActive = true; speedChainBonus++; }));
        upgradePool.Add(new Upgrade("アーマーローチ", "重装甲プロトコル展開。\n最大HP +3 / 接触ダメージ +1.5 / 移動-5%。", "★★★", () => { playerMaxHp += 3f; playerHp = Mathf.Min(playerMaxHp, playerHp + 3f); contactBurst += 1.5f; moveSpeed *= 0.95f; }));
        upgradePool.Add(new Upgrade("スラッシュサイクル", "近接接触を高速化。\n接触ダメージ +1.8 / オーラ半径+0.3m。", "★★★", () => { contactBurst += 1.8f; playerAuraRadius += 0.3f; }));
        upgradePool.Add(new Upgrade("センチネルピッチ", "ビット軌道を最適化。\n周回半径+25%。", "★★★", () => funnelOrbitRadius *= 1.25f));
        upgradePool.Add(new Upgrade("コアハーモニクス", "レーザー維持中の機動性UP。\n移動速度 +20% / レーザーDPS +15%。", "★★★", () => { laserMoveBonus = Mathf.Max(laserMoveBonus, 1.20f); laserDamageMultiplier *= 1.15f; }));
        upgradePool.Add(new Upgrade("コアシンク", "コアオーラを増幅。\nリング DPS +4 / 半径+1.5m。", "★★★", () => { coreAuraDpsBonus += 4f; coreAuraRadiusBonus += 1.5f; }));

        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Twin Core", "Nova Aegis専用。\n相棒追撃が2方向に増える。", "★★★", 1, () => { bulletCount++; allyShotTimer = 0f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Aegis Volley", "Nova Aegis専用。\n弾が広がり、盾リングも強くなる。", "★★★", 1, () => { bulletCount++; orbitShield = true; orbitDamage *= 1.14f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Data Bloom", "Photon Siphon専用。\nデータ獲得と連鎖弾を強化する。", "★★★", 2, () => { allySiphon = true; dataMultiplier *= 1.16f; pickupRange *= 1.08f; speedChainBonus++; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Photon Echo", "Photon Siphon専用。\n残像追撃と回収性能を伸ばす。", "★★★", 2, () => { speedEchoActive = true; pickupRange *= 1.1f; fireRate *= 1.04f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Sync Guard", "Core Bastion専用。\nコアが継続回復し、防衛波も強化。", "★★★", 3, () => { allyBulwark = true; lanternMaxHp += 3f; lanternHp += 3f; guardPulseUpgrade = true; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Bastion Wall", "Core Bastion専用。\n敵弾反射リング火力を強化する。", "★★★", 3, () => { reflectShield = true; orbitShield = true; orbitDamage *= 1.14f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Phantom Strike", "Nova Phantom専用。\n残像と連鎖威力を伸ばす。", "★★★", 4, () => { speedEchoActive = true; speedChainBonus++; bulletSpeed *= 1.08f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Mirage Burst", "Nova Phantom専用。\n移動と連射追撃を一気に強化。", "★★★", 4, () => { moveSpeed *= 1.08f; fireRate *= 1.1f; bulletCount++; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Drift Counter", "Aegis Drift専用。\n反撃と反射強化、移動も伸びる。", "★★★", 5, () => { contactBurst += 1.2f; reflectShield = true; moveSpeed *= 1.06f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Aegis Pulse", "Aegis Drift専用。\nコア回復と防衛波を強化。", "★★★", 5, () => { guardPulseUpgrade = true; lanternMaxHp += 4f; lanternHp += 4f; orbitShield = true; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Spectral Drain", "Photon Wraith専用。\nデータと回復、移動を強化。", "★★★", 6, () => { dataMultiplier *= 1.22f; pickupHealChance = Mathf.Min(0.7f, pickupHealChance + 0.18f); moveSpeed *= 1.04f; }));
        fusionUpgradePool.Add(new Upgrade("クロス進化専用: Ghost Net", "Photon Wraith専用。\n回収範囲と弾速、回避を伸ばす。", "★★★", 6, () => { pickupRange *= 1.2f; bulletSpeed *= 1.08f; phaseDodgeTimer = 0f; }));
    }

    void ShowTitle()
    {
        titleScreen = true;
        centerText.text = "";
        centerText.color = new Color(0.55f, 0.95f, 1f);
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (codexPanel != null)
            codexPanel.SetActive(false);
        if (missionBoardPanel != null)
            missionBoardPanel.SetActive(false);
        if (evolutionTreePanel != null)
            evolutionTreePanel.SetActive(false);
        if (resultPanel != null)
            resultPanel.SetActive(false);
        RefreshMainMenuSummary();
        if (runConfigPanel != null) runConfigPanel.SetActive(false);
        if (evolutionComboCodexPanel != null) evolutionComboCodexPanel.SetActive(false);
        RefreshStageChips();
        RefreshDangerChips();
        if (menuMissionText != null)
            menuMissionText.text = BuildMissionSummaryText();
        RefreshMissionBoard();
        nameText.text = "EGGCORE PROTOCOL";
        waveText.text = "データエッグを守り、侵食を10回しのぐ";
        playerHpFill.fillAmount = 1f;
        eggHpFill.fillAmount = 1f;
        if (playerHpLagFill != null)
            playerHpLagFill.fillAmount = 1f;
        if (eggHpLagFill != null)
            eggHpLagFill.fillAmount = 1f;
        if (playerHpValueText != null)
            playerHpValueText.text = Mathf.CeilToInt(playerHp) + " / " + Mathf.CeilToInt(playerMaxHp);
        if (eggHpValueText != null)
            eggHpValueText.text = Mathf.CeilToInt(lanternHp) + " / " + Mathf.CeilToInt(lanternMaxHp);
        dataFill.fillAmount = 0f;
        if (waveProgressText != null)
            waveProgressText.text = "WAVE 0 / " + MaxWave + "   READY";
        if (waveProgressFill != null)
            waveProgressFill.fillAmount = 1f;  // start full, drains as wave progresses
        if (dangerImage != null)
            dangerImage.color = Color.clear;
        resourceText.text = "データチップ  0";
        if (chipHudText != null)
            chipHudText.text = "◀データチップ 0";
        levelText.text = "レベル 1\nEXP 0 / " + Mathf.FloorToInt(xpToLevel);
        if (expValueText != null)
            expValueText.text = "0 / " + Mathf.FloorToInt(xpToLevel);
        statsText.text = "ATK " + bulletDamage.ToString("0.0") + "   SHOT " + bulletCount + "\nRANGE " + lightRadius.ToString("0.0") + "   PICK " + pickupRange.ToString("0.0") + "\n" + collectionSummary;
        statusText.text = "FORM " + GetStageName(evolutionStage);
        synergyText.text = "相棒がリンクを取り込んで進化する";
        if (fusionProgressText != null)
            fusionProgressText.text = "CROSS EVOLVE LOCKED";
        if (loadoutText != null)
            UpdateLoadoutPanel();
        UpdateStatusPanel();
        controlText.text = "Lv3 / Lv6 / Lv9 で相棒が進化する";
        partnerPanel.SetActive(false);
        bossBarRoot.SetActive(false);
        ApplyOptions();
    }

    void StartRun()
    {
        if (!titleScreen)
            return;
        SetPaused(false);
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (codexPanel != null)
            codexPanel.SetActive(false);
        if (missionBoardPanel != null)
            missionBoardPanel.SetActive(false);
        if (evolutionTreePanel != null)
            evolutionTreePanel.SetActive(false);
        if (resultPanel != null)
            resultPanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        runUnlockedMissions.Clear();
        evolutionVariantStyle = 0;
        // プロ改喁E kill streak / 死因記録をラン開始時にリセチE��
        killStreak = 0;
        lastKillTime = 0f;
        runMaxKillStreak = 0;
        lastDamageSource = "";
        runDeathCause = "";
        // Banish/Lock もラン開始時にクリア
        runBannedTitles.Clear();
        for (var i = 0; i < lockedSlots.Length; i++) lockedSlots[i] = false;
        currentUpgradeChoices.Clear();
        // Last Stand リセチE�� (ラン1回限めE
        lastStandUsed = false;
        lastStandInvulnTimer = 0f;
        // Solar Anchor リセチE�� (剁Erun のオーラ視覚を残さなぁE
        if (coreAuraVisual != null) { Destroy(coreAuraVisual.gameObject); coreAuraVisual = null; }
        coreAuraDpsBonus = 0f;
        coreAuraRadiusBonus = 0f;
        solarAnchorActive = false;
        coreBondNearActive = false;
        ApplyMetaProgressionRewards();
        if (PlayerPrefs.GetInt(QuickRetryKey, 0) == 1)
        {
            PlayerPrefs.SetInt(QuickRetryKey, 0);
            PlayerPrefs.Save();
            var savedStyle = PlayerPrefs.GetInt(LastRunPartnerStyleKey, 0);
            if (savedStyle > 0 && TryAutoPickPartner(savedStyle))
            {
                PlayBgm();
                return;
            }
        }
        OpenPartnerSelect();
        PlayBgm();
    }

    // 同設定再戦用: 保存済み partnerStyle と一致するパ�Eトナーを�E動選択して即 BeginWave
    bool TryAutoPickPartner(int targetStyle)
    {
        var partners = BuildPartnerPool();
        for (var i = 0; i < partners.Count; i++)
        {
            var partner = partners[i];
            if (partner.style == targetStyle)
            {
                partnerStyle = partner.style;
                partner.apply();
                ApplyPlayerVisuals(true);
                SpawnSparks(player.position, partner.accent, 24, 1.05f);
                Flash(new Color(partner.accent.r, partner.accent.g, partner.accent.b, 0.22f), 0.32f);
                AddEventLog("PARTNER: " + partner.name);
                AddEventLog("STAGE: " + stageName);
                ApplyStageConfig(currentStageId);
                BuildStageEnvironment();
                BeginWave(1);
                ShowMessage(partnerName + " 起動(同設定再戦)");
                UpdateUi();
                return true;
            }
        }
        return false;  // No matching saved partner; fall back to normal partner select.
    }

    void SetPaused(bool value)
    {
        if (titleScreen || gameOver || victory || choosingUpgrade || evolutionCutsceneTimer > 0f)
            value = false;

        paused = value;
        if (pausePanel != null)
            pausePanel.SetActive(paused);
        if (paused && restartButton != null)
            restartButton.interactable = true;
        if (!paused && optionsPanel != null && !titleScreen)
            optionsPanel.SetActive(false);
        Time.timeScale = paused ? 0f : 1f;
        ApplyOptions();
        if (paused)
            PlaySfx("Pickup", 520f, 0.045f, 0.1f);
    }

    void OpenPartnerSelect()
    {
        titleScreen = false;
        choosingUpgrade = true;
        centerText.text = "";
        partnerPanel.SetActive(true);
        SetChoiceFocus(true);
        cardAppearStartTime = Time.unscaledTime;
        choiceClickGuardUntil = Time.unscaledTime + ChoiceClickGuardDuration;

        // Show ALL partners (no random selection)  Elet user pick freely
        var pool = BuildPartnerPool();
        EnsurePartnerButtonCount(pool.Count);
        LayoutPartnerButtons(pool.Count);

        for (var i = 0; i < partnerButtons.Count; i++)
        {
            partnerButtons[i].gameObject.SetActive(i < pool.Count);
            if (i >= pool.Count) continue;

            var partner = pool[i];
            var label = partnerButtons[i].transform.Find("Label").GetComponent<Text>();
            label.text = partner.name + "\n\n" + partner.description;
            // Split description into trait (1st line) + detail (rest)
            var lines = partner.description.Split('\n');
            var traitLine = lines.Length > 0 ? lines[0] : "";
            var detailLine = lines.Length > 1 ? string.Join("\n", lines, 1, lines.Length - 1) : "";

            ApplyCardVisual(partnerButtons[i], label, partner.normal, partner.highlight, Color.Lerp(Color.white, partner.accent, 0.45f), partner.accent, partner.badge, 1.1f);
            SetCardText(partnerButtons[i], label, partner.name, traitLine, detailLine, Color.Lerp(Color.white, partner.accent, 0.35f), new Color(0.78f, 0.94f, 1f), partner.accent, 1.1f);

            // ── Reposition card internals for the scrollable 300×20 layout ──
            RepositionPartnerCardInternals(partnerButtons[i], partner, detailLine);

            label.rectTransform.sizeDelta = new Vector2(264, 30);
            label.rectTransform.anchoredPosition = new Vector2(0, -38);
            label.fontSize = 11;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 9;
            label.resizeTextMaxSize = 11;
            label.alignment = TextAnchor.UpperCenter;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.lineSpacing = 0.9f;
            label.text = detailLine;
            SetPartnerCardPreview(partnerButtons[i], partner.style, partner.accent);
            BuildPartnerStatGrid(partnerButtons[i], partner);
            partnerButtons[i].onClick.RemoveAllListeners();
            partnerButtons[i].onClick.AddListener(() =>
            {
                if (IsChoiceClickGuarded()) return;
                partnerStyle = partner.style;
                partner.apply();
                ApplyPlayerVisuals(true);
                SpawnSparks(player.position, partner.accent, 24, 1.05f);
                Flash(new Color(partner.accent.r, partner.accent.g, partner.accent.b, 0.22f), 0.32f);
                AddEventLog("PARTNER: " + partner.name);
                AddEventLog("STAGE: " + stageName);
                // プロ改喁E 同設定再戦のためにパ�Eトナー選択を記�E
                PlayerPrefs.SetInt(LastRunPartnerStyleKey, partner.style);
                PlayerPrefs.Save();
                partnerPanel.SetActive(false);
                choosingUpgrade = false;
                SetChoiceFocus(false);
                // ステージ環墁E��構篁E(Stage 1 = 何もしなぁE��Stage 2 = lava + vent 配置)
                ApplyStageConfig(currentStageId);
                BuildStageEnvironment();
                BeginWave(1);
                ShowMessage(partnerName + " 起動");
                UpdateUi();
            });
        }
    }

    List<RelicData> BuildRelicPool()
    {
        var pool = new List<RelicData>();
        if (!relicSet.Contains("magnet"))
            pool.Add(new RelicData("magnet", "磁力核",
                "ピックアップの吸引範囲が\n安定して広がる。",
                () => pickupRange += 1.15f));
        if (!relicSet.Contains("deathless"))
            pool.Add(new RelicData("deathless", "不滅の証",
                "一度だけ死を回避する。\n最大HPも少し増える。",
                () => { deathShieldActive = true; playerMaxHp += 1f; playerHp = Mathf.Min(playerMaxHp, playerHp + 1f); }));
        if (!relicSet.Contains("core_pulse"))
            pool.Add(new RelicData("core_pulse", "コアの鼓動",
                "Eggcore が周期的に\n周囲の敵へ小さな衝撃を放つ。",
                () => { corePulseRelicActive = true; corePulseTimer = 6.5f; }));
        if (!relicSet.Contains("explosion"))
            pool.Add(new RelicData("explosion", "爆発の紋章",
                "敵撃破時の爆発確率を\n大きく伸ばす。",
                () => explodeChance = Mathf.Min(0.38f, Mathf.Max(explodeChance + 0.18f, 0.22f))));
        if (!relicSet.Contains("storm"))
            pool.Add(new RelicData("storm", "嵐の心臓",
                "攻撃速度と弾速を伸ばし、\n攻撃の回転を上げる。",
                () => { fireRate *= 1.18f; bulletSpeed *= 1.08f; bulletLifeMultiplier *= 1.04f; }));
        if (!relicSet.Contains("mirror"))
            pool.Add(new RelicData("mirror", "鏡の誓約",
                "近くの敵弾を反射、\n防衛火力も少し伸びる。",
                () => { reflectShield = true; bulletDamage *= 1.04f; }));
        if (!relicSet.Contains("titan"))
            pool.Add(new RelicData("titan", "鋼鉄の盟約",
                "最大HPと押し返しを強化。\n守りを安定させる。",
                () => { playerMaxHp += 2f; playerHp = Mathf.Min(playerMaxHp, playerHp + 2f); playerKnockback += 1.1f; orbitKnockback += 0.6f; }));
        if (!relicSet.Contains("overdrive"))
            pool.Add(new RelicData("overdrive", "オーバードライブ",
                "攻撃力、貫通、クリティカルを\n少しずつ伸ばす。",
                () => { bulletDamage *= 1.12f; bulletPierce++; critChance += 0.06f; }));
        if (!relicSet.Contains("data_surge"))
            pool.Add(new RelicData("data_surge", "データサージ",
                "データ獲得と回復抽選を\nほどよく伸ばす。",
                () => { dataMultiplier *= 1.20f; pickupHealChance = Mathf.Min(0.40f, pickupHealChance + 0.08f); }));
        // ── 追加レリチE�� (ビルド方向性の幁E��広がめE ──
        if (!relicSet.Contains("chain_bond"))
            pool.Add(new RelicData("chain_bond", "電流の輪",
                "連鎖回路を強化する。\nチェイン +2 hop / 攻撃+5%。",
                () => { speedChainBonus += 2; bulletDamage *= 1.05f; }));
        if (!relicSet.Contains("resonance_wing"))
            pool.Add(new RelicData("resonance_wing", "共鳴の翼",
                "コアオーラを増幅する。\nリング DPS +3 / 半径+1m。",
                () => { coreAuraDpsBonus += 3f; coreAuraRadiusBonus += 1f; }));
        if (!relicSet.Contains("awakening_seal"))
            pool.Add(new RelicData("awakening_seal", "覚醒の刻印",
                "経験値を即座にチャージ。\n現レベルの半分を獲得 / データ獲得+8%。",
                () => { xp += xpToLevel * 0.5f; dataMultiplier *= 1.08f; CheckLevelUp(); }));
        if (!relicSet.Contains("collector_blessing"))
            pool.Add(new RelicData("collector_blessing", "回収の祝福",
                "拾い物特化。\n回収範囲 +50% / 拾い回復確率+15%。",
                () => { pickupRange *= 1.5f; pickupHealChance = Mathf.Min(0.65f, pickupHealChance + 0.15f); }));
        if (!relicSet.Contains("overcharge_core"))
            pool.Add(new RelicData("overcharge_core", "過充電の核",
                "弾数と連射を底上げ。\n弾数 +1 / 連射+12% / 単発威力 -6%。",
                () => { bulletCount++; fireRate *= 1.12f; bulletDamage *= 0.94f; }));

        if (!relicSet.Contains("apex") && allyNova && allyBulwark && allySiphon && allyPhase)
            pool.Add(new RelicData("apex", "最強進化 / APEX CORE",
                "全リンク所有時のみ出現。\n攻撃・連射・移動・HP・データを\nすべて大幅強化。最強の証。",
                () => {
                    bulletDamage *= 1.20f;
                    fireRate *= 1.15f;
                    moveSpeed *= 1.08f;
                    bulletCount += 1;
                    bulletPierce += 1;
                    playerMaxHp += 2f; playerHp = Mathf.Min(playerMaxHp, playerHp + 2f);
                    lanternMaxHp += 3f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 3f);
                    critChance += 0.08f;
                    dataMultiplier *= 1.15f;
                    speedChainBonus += 1;
                    deathShieldActive = true;
                }));
        return pool;
    }

    void OpenRelicSelect()
    {
        choosingRelic = true;
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        UpdateModuleStatusSidePanel();
        SetChoiceFocus(true);
        panelTitleText.text = "レリックを選ぶ";
        panelTitleText.color = new Color(1f, 0.88f, 0.28f);
        if (panelSubtitleText != null)
        {
            panelSubtitleText.text = "ランを通じて効果が続く";
            panelSubtitleText.color = new Color(1f, 0.92f, 0.58f);
        }
        cardAppearStartTime = Time.unscaledTime;
        choiceClickGuardUntil = Time.unscaledTime + ChoiceClickGuardDuration;
        rerollButton.gameObject.SetActive(false);
        if (skipRewardButton != null)
            skipRewardButton.gameObject.SetActive(false);

        var pool = BuildRelicPool();
        for (var r = pool.Count - 1; r > 0; r--)
        {
            var j = rng.Next(r + 1);
            var tmp = pool[r]; pool[r] = pool[j]; pool[j] = tmp;
        }

        // Center the 2 relic buttons (instead of leaving them at default left/middle x=-260, 0)
        var relicCount = Mathf.Min(2, pool.Count);
        for (var p = 0; p < upgradeButtons.Count; p++)
        {
            var rt = upgradeButtons[p].GetComponent<RectTransform>();
            if (rt != null)
            {
                float x;
                if (relicCount == 1)      x = 0f;
                else if (relicCount == 2) x = -160f + p * 320f;
                else                      x = -310f + p * 310f;
                var newPos = new Vector2(x, -8f);
                rt.anchoredPosition = newPos;
                // Sync cardBasePositions so the bob animation doesn't snap buttons
                // back to their old cached (left-aligned) position every frame.
                cardBasePositions[upgradeButtons[p]] = newPos;
            }
        }

        for (var i = 0; i < upgradeButtons.Count; i++)
        {
            if (i >= pool.Count || i >= 2)
            {
                upgradeButtons[i].gameObject.SetActive(false);
                continue;
            }
            upgradeButtons[i].gameObject.SetActive(true);
            upgradeButtons[i].interactable = true;
            var relic = pool[i];
            var label = upgradeButtons[i].transform.Find("Label").GetComponent<Text>();
            label.text = relic.title + "\n\n" + relic.description;
            ApplyCardVisual(upgradeButtons[i], label,
                new Color(0.13f, 0.09f, 0.02f, 0.98f),
                new Color(0.28f, 0.20f, 0.04f, 1f),
                new Color(1f, 0.92f, 0.72f),
                new Color(1f, 0.82f, 0.18f, 1f),
                "RELIC", 1.9f);
            SetCardText(upgradeButtons[i], label, relic.title, "永続レリック", relic.description, new Color(1f, 0.92f, 0.56f), new Color(0.95f, 0.9f, 0.72f), new Color(1f, 0.82f, 0.18f, 1f), 1.9f);
            ApplyRelicCardIcon(upgradeButtons[i], relic.id);
            upgradeButtons[i].onClick.RemoveAllListeners();
            var captured = relic;
            upgradeButtons[i].onClick.AddListener(() =>
            {
                if (IsChoiceClickGuarded()) return;
                relicSet.Add(captured.id);
                captured.apply();
                AddEventLog("RELIC: " + captured.title);
                PlaySfx("Evolve", 330f, 0.22f, 0.32f);
                SpawnSparks(player.position, new Color(1f, 0.88f, 0.28f), 22, 1.4f);
                Flash(new Color(1f, 0.82f, 0.18f, 0.16f), 0.35f);
                panelTitleText.color = new Color(1f, 0.82f, 0.42f);
                if (panelSubtitleText != null)
                    panelSubtitleText.color = new Color(0.72f, 1f, 0.96f);
                upgradePanel.SetActive(false);
                choosingRelic = false;
                SetChoiceFocus(false);
                BeginWave(wave + 1);
                OpenUpgrade();
            });
        }
    }

    // Reposition the internals of a partner card for the scrollable 300×20 layout.
    // Keep each text band visually isolated: badge →icon →name →trait →description →stats.
    void RepositionPartnerCardInternals(Button button, PartnerOption partner, string detailLine)
    {
        var t = button.transform;

        void Move(string name, Vector2 pos, Vector2 size)
        {
            var n = t.Find(name);
            if (n == null) return;
            var rt = n.GetComponent<RectTransform>();
            if (rt == null) return;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
        }

        Move("Card Header Plate", new Vector2(0, 94), new Vector2(268, 22));
        Move("Card Badge", new Vector2(0, 94), new Vector2(258, 20));
        Move("Card Glow Fill", new Vector2(0, 42), new Vector2(168, 168));
        Move("Card Icon", new Vector2(0, 48), new Vector2(56, 56));
        Move("Card Icon Core", new Vector2(0, 66), new Vector2(10, 10));
        Move("Card Energy Ring", new Vector2(0, 48), new Vector2(56, 56));
        Move("Card Title Plate", new Vector2(0, 14), new Vector2(248, 24));
        Move("Card Title Text", new Vector2(0, 14), new Vector2(240, 22));
        Move("Card Level Text", new Vector2(0, -9), new Vector2(240, 16));
        Move("Card Bottom Rail", new Vector2(0, -105), new Vector2(150, 0));
        Move("Card Rarity Plate", new Vector2(0, -98), new Vector2(0, 0));
        Move("Card Rarity Text", new Vector2(0, -98), new Vector2(0, 0));
        Move("Card Rarity Spike", new Vector2(-126, 92), new Vector2(16, 16));

        // Font tweaks for smaller card
        var titleText = t.Find("Card Title Text")?.GetComponent<Text>();
        if (titleText != null)
        {
            titleText.fontSize = 18;
            titleText.resizeTextMaxSize = 18;
            titleText.resizeTextMinSize = 13;
        }
        var levelText = t.Find("Card Level Text")?.GetComponent<Text>();
        if (levelText != null)
        {
            levelText.fontSize = 12;
            levelText.resizeTextMaxSize = 12;
            levelText.resizeTextMinSize = 9;
        }
        var headerText = t.Find("Card Badge")?.GetComponent<Text>();
        if (headerText != null)
        {
            headerText.fontSize = 11;
            headerText.resizeTextMaxSize = 11;
            headerText.resizeTextMinSize = 8;
        }
    }

    // ── Visual stat grid for partner cards ──────────────────────────────
    // Each partner card gets 5 rows of 5-segment bars showing HP/ATK/SPD/FIRE/SPC
    void BuildPartnerStatGrid(Button button, PartnerOption partner)
    {
        if (button == null) return;

        // Container  Ereuse if already built, else create
        var containerName = "Partner Stat Grid";
        var existing = button.transform.Find(containerName);
        if (existing != null)
            Destroy(existing.gameObject);

        var container = new GameObject(containerName, typeof(RectTransform));
        container.transform.SetParent(button.transform, false);
        var contRect = container.GetComponent<RectTransform>();
        contRect.anchorMin = contRect.anchorMax = new Vector2(0.5f, 0.5f);
        contRect.pivot = new Vector2(0.5f, 0.5f);
        // Card 290×00 (half=100). Stat grid sized for compact card.
        // Description: y=-42, h=14, rect y=-49..-35 (1px gap from trait bot -29)
        // Stat grid:  y=-72, h=44, rect y=-94..-50 (1px gap below desc, 6px above card bot -100)
        contRect.anchoredPosition = new Vector2(0, -83);
        contRect.sizeDelta = new Vector2(244, 42);

        var labels = new[] { "HP", "ATK", "SPD", "FIRE", "SPC" };
        var values = new[] { partner.statHp, partner.statAtk, partner.statSpd, partner.statFire, partner.statSpecial };
        var accent = partner.accent;
        const int rows = 5;
        const float rowHeight = 8.4f;  // 5 rows ×8.4 = 42
        const float startY = (rows - 1) * rowHeight * 0.5f;

        for (var r = 0; r < rows; r++)
        {
            var rowGo = new GameObject("Stat Row " + r, typeof(RectTransform));
            rowGo.transform.SetParent(container.transform, false);
            var rowRect = rowGo.GetComponent<RectTransform>();
            rowRect.anchorMin = rowRect.anchorMax = new Vector2(0.5f, 0.5f);
            rowRect.pivot = new Vector2(0.5f, 0.5f);
            rowRect.anchoredPosition = new Vector2(0, startY - r * rowHeight);
            rowRect.sizeDelta = new Vector2(244, rowHeight);

            var labelText = CreateText(labels[r] + " Label", rowGo.transform, new Vector2(-94, 0), TextAnchor.MiddleLeft, 9, new Color(0.94f, 1f, 1f, 1f));
            labelText.text = labels[r];
            labelText.fontStyle = FontStyle.Bold;
            labelText.horizontalOverflow = HorizontalWrapMode.Overflow;
            labelText.verticalOverflow = VerticalWrapMode.Overflow;
            ForceCenterRect(labelText.rectTransform, new Vector2(-94, 0), new Vector2(42, rowHeight));
            labelText.alignment = TextAnchor.MiddleLeft;

            const int segCount = 5;
            const float segWidth = 16f;
            const float segGap = 3f;
            var segStartX = -38f;
            for (var s = 0; s < segCount; s++)
            {
                var segGo = new GameObject("Seg " + s, typeof(Image));
                segGo.transform.SetParent(rowGo.transform, false);
                var segImg = segGo.GetComponent<Image>();
                var filled = s < values[r];
                segImg.color = filled
                    ? new Color(accent.r, accent.g, accent.b, 0.96f)
                    : new Color(0.10f, 0.16f, 0.22f, 0.78f);
                segImg.raycastTarget = false;
                var segRect = segGo.GetComponent<RectTransform>();
                segRect.anchorMin = segRect.anchorMax = new Vector2(0.5f, 0.5f);
                segRect.pivot = new Vector2(0, 0.5f);
                segRect.anchoredPosition = new Vector2(segStartX + s * (segWidth + segGap), 0);
                // Slim bars  Evisible but compact
                segRect.sizeDelta = new Vector2(segWidth, Mathf.Max(4f, rowHeight - 3f));
            }
        }
    }

    List<PartnerOption> BuildPartnerPool()
    {
        // Stat values (1-5): HP, ATK, SPD, FIRE, SPECIAL
        var list = new List<PartnerOption>();
        list.Add(new PartnerOption(
            "Cobalt Pup", "標準型\n攻撃・移動・進化のバランスが良い。",
            new Color(0.07f, 0.17f, 0.22f), new Color(0.12f, 0.32f, 0.42f), new Color(0.32f, 0.95f, 1f), "BALANCE LINK",
            1, 3, 4, 4, 3, 2,
            () => { partnerName = "Cobalt Pup"; partnerTrait = "標準型"; bulletDamage *= 1.08f; moveSpeed *= 1.06f; }));

        list.Add(new PartnerOption(
            "Ember Drake", "弾幕型\n連射と弾数に優れる橙竜素体。少し打たれ弱い。",
            new Color(0.16f, 0.12f, 0.05f), new Color(0.34f, 0.24f, 0.08f), new Color(1f, 0.78f, 0.2f), "BURST LINK",
            2, 2, 3, 3, 5, 4,
            () => { partnerName = "Ember Drake"; partnerTrait = "弾幕型"; fireRate *= 1.22f; bulletCount++; playerMaxHp -= 1f; playerHp = Mathf.Min(playerHp, playerMaxHp); }));

        list.Add(new PartnerOption(
            "Sage Hare", "防衛型\nコアを守る緑の兎賢者。リングが強く粘り強い。",
            new Color(0.08f, 0.16f, 0.1f), new Color(0.14f, 0.32f, 0.18f), new Color(0.45f, 1f, 0.55f), "GUARD LINK",
            3, 5, 2, 2, 3, 5,
            () => { partnerName = "Sage Hare"; partnerTrait = "防衛型"; lanternMaxHp += 7f; lanternHp += 7f; orbitShield = true; orbitDamage *= 1.22f; moveSpeed *= 0.94f; }));

        list.Add(new PartnerOption(
            "Hex Cat", "連鎖型\nチェイン+1と残像追撃を最初から持つ紫の魔猫。",
            new Color(0.14f, 0.04f, 0.18f), new Color(0.28f, 0.1f, 0.36f), new Color(0.86f, 0.5f, 1f), "CHAIN LINK",
            4, 3, 3, 4, 4, 5,
            () => { partnerName = "Hex Cat"; partnerTrait = "連鎖型"; speedChainBonus++; speedEchoActive = true; speedEchoTimer = 0f; fireRate *= 1.06f; }));

        list.Add(new PartnerOption(
            "Drift Fox", "回避型\n移動が速く、Phase回避を最初から持つ桃色の狐。",
            new Color(0.18f, 0.05f, 0.12f), new Color(0.4f, 0.12f, 0.28f), new Color(1f, 0.46f, 0.76f), "PHASE LINK",
            5, 3, 3, 5, 3, 4,
            () => { partnerName = "Drift Fox"; partnerTrait = "回避型"; moveSpeed *= 1.15f; allyPhase = true; phaseDodgeTimer = 0f; }));

        list.Add(new PartnerOption(
            "Iron Bear", "装甲型\nHP+2と接触反撃を持つ重装甲の黒熊。",
            new Color(0.05f, 0.07f, 0.1f), new Color(0.12f, 0.16f, 0.22f), new Color(0.85f, 0.85f, 0.95f), "ARMOR LINK",
            6, 5, 4, 2, 3, 4,
            () => { partnerName = "Iron Bear"; partnerTrait = "装甲型"; playerMaxHp += 2f; playerHp = Mathf.Min(playerMaxHp, playerHp + 2f); contactBurst += 0.8f; moveSpeed *= 0.92f; }));

        // ── 7. Wraith Lynx  E近接垁E────────────────────────────────────
        // 封E��ゼロ前提で、近接性能を�E方位で最強クラスに。弾はほぼ撃破なぁE��わりに
        // オーラ + 接触バースチE+ ノックバック + 移動速度 + HP + 吸血が�E部高い、E
        list.Add(new PartnerOption(
            "Wraith Lynx", "近接型\nオーラ・接触・吸血・KBの近接最強白狼。",
            new Color(0.09f, 0.11f, 0.15f), new Color(0.20f, 0.24f, 0.32f), new Color(0.78f, 0.95f, 1f), "MELEE LINK",
            7, 5, 5, 5, 1, 5,
            () => {
                partnerName = "Wraith Lynx"; partnerTrait = "近接型";
                moveSpeed *= 1.45f;                 // 1.32 →1.45 (高速突撃)
                playerMaxHp += 5f; playerHp = Mathf.Min(playerMaxHp, playerHp + 5f);  // +3 →+5 (耐乁E��匁E
                playerAuraDamage += 9.0f;           // 5.0 →9.0 (オーラがメイン火力
                playerAuraRadius = 1.85f;           // 1.45 →1.85 (範囲も拡大)
                contactBurst += 4.0f;               // 2.4 →4.0 (接触一撃破雑魚即死)
                playerKnockback += 3.5f;            // 2.5 →3.5 (ノックバック圧)
                bulletLifesteal = true;             // 削りで自己回復
                fireRate *= 0.10f;                  // 0.55 →0.10 (ほぼ撃破なぁE
                bulletDamage *= 0.30f;              // 0.65 →0.30 (弾は演�Eのみ)
            }));

        // ── 8. Genesis Core  E全リンク所持型�E�隠し最強�E�E─────────────
        // 4種類�Eリンク仲間を最初から�E部裁E��済み、E        // 個、E�Eスデータスは標準だが、リンク全部 ON で総合力が高い、E
        list.Add(new PartnerOption(
            "Genesis Core", "全種解放【隠し】\n4種リンクを全て解放した最強の総合素体。",
            new Color(0.06f, 0.12f, 0.18f), new Color(0.14f, 0.28f, 0.40f), new Color(1f, 0.95f, 0.32f), "GENESIS LINK",
            8, 4, 4, 4, 4, 5,
            () => {
                partnerName = "Genesis Core"; partnerTrait = "全種解放";
                allyNova = true;                   // 火力リンク
                allyBulwark = true;                // 防衛リンク
                allySiphon = true;                 // 回収リンク
                allyPhase = true;                  // 回避リンク
                phaseDodgeTimer = 0f;
                playerMaxHp += 2f; playerHp = Mathf.Min(playerMaxHp, playerHp + 2f);
                lanternMaxHp += 3f; lanternHp += 3f;
                bulletDamage *= 1.06f;
                moveSpeed *= 1.04f;
            }));

        // ── 9. Halo Caster  Eファンネル垁E─────────────────────────────
        // 自律ビチE��3橁E+ 本人弾の合計火力で押すスタイル、E
        list.Add(new PartnerOption(
            "Halo Caster", "ファンネル型\n3機の自律ビットが敵を自動攻撃する。",
            new Color(0.05f, 0.10f, 0.20f), new Color(0.12f, 0.24f, 0.42f), new Color(0.55f, 0.92f, 1f), "DRONE LINK",
            9, 3, 4, 4, 5, 5,
            () => {
                partnerName = "Halo Caster"; partnerTrait = "ファンネル型";
                EnsureFunnelBits(3, new Color(0.55f, 0.92f, 1f));
                fireRate *= 0.95f;                  // 本体連射��ぼ通常 (0.78 →0.95)
                bulletDamage *= 1.05f;              // 本体火力少し強匁E(0.85 →1.05)
                playerMaxHp += 1f; playerHp = Mathf.Min(playerMaxHp, playerHp + 1f);
                pickupRange *= 1.08f;
            }));

        // ── 10. Pulse Hydra  E持続レーザー垁E─────────────────────────
        // 直線上�E全敵を貫通ダメージで焼く。単発ターゲチE��は満タン、ライン敵は60%、E
        list.Add(new PartnerOption(
            "Pulse Hydra", "レーザー型\n直線上の敵を全員貫通ダメージで焼く高火力型。",
            new Color(0.16f, 0.04f, 0.12f), new Color(0.36f, 0.08f, 0.24f), new Color(1f, 0.45f, 0.85f), "BEAM LINK",
            10, 3, 5, 3, 5, 5,
            () => {
                partnerName = "Pulse Hydra"; partnerTrait = "レーザー型";
                EnsureLaser(true, new Color(1f, 0.45f, 0.85f));
                laserDamageMultiplier = 8.0f;       // 5.0 →8.0 (DPS紁E0%墁E
                laserMaxLength = 9f;                // 8 →9 (画面端まで届く)
                fireRate *= 0.55f;                  // 0.25 →0.55 (護身用に撃破めE
                bulletDamage *= 0.85f;              // 0.55 →0.85 (補助弾もそこそぁE
                playerMaxHp += 2f; playerHp = Mathf.Min(playerMaxHp, playerHp + 2f);  // +1 →+2
                moveSpeed *= 0.92f;                 // めE��遁E(ユーザー持E��で復活)
            }));

        // ── 11. Solar Anchor  Eコア共鳴垁E─────────────────────────
        // 「コアから離れなぁE���Eレイ。TD の本質を補強する第3弾キャラ最後�E枠、E        // コア近接 (≤ 3m): 全スチE+25%
        // コア遠距離 (> 6m): HP 自動回復停止
        // 8秒毎にコアから周囲ダメージパルス「Resonance Wave」発勁E
        list.Add(new PartnerOption(
            "Solar Anchor", "コア共鳴型\nコアの周りに常時ダメリング。コア近接で攻撃30%。",
            new Color(0.16f, 0.10f, 0.04f), new Color(0.32f, 0.22f, 0.06f), new Color(1f, 0.86f, 0.28f), "CORE LINK",
            11, 3, 4, 4, 5, 4,
            () => {
                partnerName = "Solar Anchor"; partnerTrait = "コア共鳴型";
                solarAnchorActive = true;
                playerMaxHp += 2f; playerHp = Mathf.Min(playerMaxHp, playerHp + 2f);
                lanternMaxHp += 4f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 4f);
                bulletDamage *= 0.95f;              // ベ�Eス弾は控えめ (共鳴ボ�Eナス前提)
                coreAuraTickAcc = 0f;  // Core Aura tick リセチE��
            }));

        return list;
    }

    void SetPartnerCardPreview(Button button, int style, Color accent)
    {
        var icon = button.transform.Find("Card Icon");
        if (icon != null)
        {
            var iconImage = icon.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = GetPartnerVariantSprite(0, 0, 0, style);
                iconImage.color = Color.white;
                iconImage.preserveAspect = true;
            }
            var rect = icon.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(0, 48);
                rect.sizeDelta = new Vector2(56, 56);
            }
        }

        var iconCore = button.transform.Find("Card Icon Core");
        if (iconCore != null)
        {
            var coreImage = iconCore.GetComponent<Image>();
            if (coreImage != null)
                coreImage.color = WithAlpha(accent, 0.68f);
            var coreRect = iconCore.GetComponent<RectTransform>();
            if (coreRect != null)
            {
                coreRect.anchoredPosition = new Vector2(0, 66);
                coreRect.sizeDelta = new Vector2(10, 10);
            }
        }

        var energyRing = button.transform.Find("Card Energy Ring");
        if (energyRing != null)
        {
            var ringRect = energyRing.GetComponent<RectTransform>();
            if (ringRect != null)
            {
                ringRect.anchoredPosition = new Vector2(0, 48);
                ringRect.sizeDelta = new Vector2(56, 56);
            }
        }
    }

    sealed class PartnerOption
    {
        public readonly string name;
        public readonly string description;
        public readonly Color normal;
        public readonly Color highlight;
        public readonly Color accent;
        public readonly string badge;
        public readonly int style;
        public readonly Action apply;
        // Visual stat values (1-5 scale): HP, ATK, SPD, FIRE, SPECIAL
        public readonly int statHp, statAtk, statSpd, statFire, statSpecial;

        public PartnerOption(string name, string description, Color normal, Color highlight, Color accent, string badge, int style,
            int statHp, int statAtk, int statSpd, int statFire, int statSpecial, Action apply)
        {
            this.name = name;
            this.description = description;
            this.normal = normal;
            this.highlight = highlight;
            this.accent = accent;
            this.badge = badge;
            this.style = style;
            this.statHp = statHp;
            this.statAtk = statAtk;
            this.statSpd = statSpd;
            this.statFire = statFire;
            this.statSpecial = statSpecial;
            this.apply = apply;
        }
    }

    void HandlePlayer()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        if (Input.GetMouseButton(0) && mainCamera != null)
        {
            var world = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseMoveTarget = new Vector2(world.x, world.y);
            hasMouseMoveTarget = true;
        }

        if (input.sqrMagnitude > 0.05f)
            hasMouseMoveTarget = false;

        if (hasMouseMoveTarget)
        {
            var toTarget = mouseMoveTarget - (Vector2)player.position;
            if (toTarget.magnitude < 0.18f)
            {
                hasMouseMoveTarget = false;
                input = Vector2.zero;
            }
            else
            {
                input = toTarget.normalized;
            }
        }

        // コアハ�Eモニクス: レーザー使用中は移動速度にボ�Eナス (1.0 = ボ�EナスなぁE
        var effectiveMoveSpeed = laserActive ? moveSpeed * laserMoveBonus : moveSpeed;
        // Solar Anchor: コア近接時�E移動�Eーナスは削除 (シンプル匁E、E        // 攻撃+30% のみで体感を一本化、移動�E通常速度のまま、E        // Stage 3 汚染パッチ接触中は移動-25%
        if (corruptionSlowActive) effectiveMoveSpeed *= CorruptionSlowMultiplier;
        // Stage 4 結霜パッチ接触中も移動-25% (ダメージなし版)
        if (frostPatchSlowActive) effectiveMoveSpeed *= FrostPatchSlowMultiplier;
        var next = (Vector2)player.position + input * effectiveMoveSpeed * Time.deltaTime;
        if (next.magnitude > ArenaRadius - 0.45f)
            next = next.normalized * (ArenaRadius - 0.45f);
        player.position = new Vector3(next.x, next.y, 0);

        // Movement trail
        if (input.sqrMagnitude > 0.05f)
        {
            trailTimer -= Time.deltaTime;
            if (trailTimer <= 0f)
            {
                var tc = fusionActive ? GetFormAccentColor() : new Color(0.32f, 0.88f, 1f);
                var tg = CreateSpriteObject("Trail", squareSprite, player.position, WithAlpha(tc, 0.55f), 0.09f);
                var tl = Mathf.Lerp(0.12f, 0.22f, (float)rng.NextDouble());
                sparks.Add(new Spark { transform = tg.transform, velocity = Vector2.zero, life = tl, maxLife = tl });
                trailTimer = 0.038f;
            }
        }

        if (input.sqrMagnitude > 0.05f)
            aimDirection = input.normalized;

        if (orbitShield)
        {
            var pulse = 0.88f + Mathf.Sin(Time.time * 8f) * 0.08f;
            for (var i = enemies.Count - 1; i >= 0; i--)
            {
                var e = enemies[i];
                if (e == null || e.transform == null) continue;
                if (Vector2.Distance(e.transform.position, player.position) < pulse)
                {
                    DamageEnemy(e, Time.deltaTime * orbitDamage, false);
                    // Knockback (push enemy away from player)
                    if (orbitKnockback > 0f && e.transform != null && e.type != EnemyType.Boss)
                    {
                        var pushDir = ((Vector2)(e.transform.position - player.position)).normalized;
                        if (pushDir.sqrMagnitude < 0.01f) pushDir = Vector2.up;
                        e.transform.position += (Vector3)(pushDir * orbitKnockback * Time.deltaTime);
                    }
                }
            }
        }

        if (playerAuraDamage > 0f)
        {
            var auraRadius = Mathf.Max(0.75f, playerAuraRadius);
            for (var i = enemies.Count - 1; i >= 0; i--)
            {
                if (Vector2.Distance(enemies[i].transform.position, player.position) < auraRadius)
                    DamageEnemy(enemies[i], Time.deltaTime * playerAuraDamage, false);
            }

            // STELLAR HALO 進匁E 周期的なショチE��ウェーブパルス (auraRadius ×2.2 範囲)
            if (evoStellarHalo)
            {
                stellarHaloPulseTimer -= Time.deltaTime;
                if (stellarHaloPulseTimer <= 0f)
                {
                    stellarHaloPulseTimer = StellarHaloPulseInterval;
                    var pulseRadius = auraRadius * 2.2f;
                    var pulseDamage = playerAuraDamage * 1.4f;
                    SplashDamage(player.position, pulseRadius, pulseDamage, null);
                    var pulseColor = new Color(1f, 0.78f, 0.32f);
                    SpawnRingSparks(player.position, pulseColor, 22, pulseRadius);
                    SpawnSparks(player.position, pulseColor, 14, 0.9f);
                    Flash(new Color(1f, 0.78f, 0.32f, 0.10f), 0.15f);
                    PlaySfx("Hit", 260f, 0.08f, 0.18f);
                }
            }
        }
    }

    void HandleShooting()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer > 0f)
            return;

        var target = FindNearestEnemy();
        if (target != null)
            aimDirection = ((Vector2)(target.transform.position - player.position)).normalized;

        shootTimer = 1f / fireRate;
        playerAttackPulse = Mathf.Max(playerAttackPulse, 1f);

        // ── Firing pattern ─────────────────────────────────────────
        // Odd bulletCount →classic fan (1 center + spread). Direct hits always land.
        // Even bulletCount →PARALLEL shots with lateral spawn offset, all aimed straight at target.
        //   This guarantees direct hits because at least one bullet path covers the aim line.
        if (bulletCount == 1)
        {
            SpawnBullet(aimDirection);
        }
        else if (bulletCount % 2 == 0)
        {
            // Parallel pattern  Eall bullets fly in aim direction, offset perpendicular to it
            // Wider spacing so the salvo covers more enemies, not just a single line
            var perp = new Vector2(-aimDirection.y, aimDirection.x);
            const float spacing = 0.38f;
            var totalWidth = (bulletCount - 1) * spacing;
            for (var i = 0; i < bulletCount; i++)
            {
                var lateral = i * spacing - totalWidth * 0.5f;
                SpawnBullet(aimDirection, perp * lateral);
            }
        }
        else
        {
            // Odd count fan (center bullet handles direct hits)
            var spread = Mathf.Min(38f, 9f * (bulletCount - 1));
            for (var i = 0; i < bulletCount; i++)
            {
                var t = i / (float)(bulletCount - 1);
                var angle = Mathf.Lerp(-spread, spread, t);
                SpawnBullet(Rotate(aimDirection, angle));
            }
        }
    }

    void HandleAlly()
    {
        if (!allyNova && !allyBulwark && !allySiphon)
            return;

        allyShotTimer -= Time.deltaTime;
        if (allyShotTimer > 0f)
            return;

        allyShotTimer = fusionActive ? 0.58f : 1.35f;
        if (allyNova || fusionActive)
        {
            var target = FindNearestEnemy();
            if (target != null)
            {
                var direction = ((Vector2)(target.transform.position - player.position)).normalized;
                var origin = linkCompanions[0] != null && linkCompanions[0].gameObject.activeSelf ? linkCompanions[0].position : player.position + Vector3.right * 0.45f;
                var sprite = fusionActive && bulletFusionSprite != null ? bulletFusionSprite : bulletSpeedSprite != null ? bulletSpeedSprite : diamondSprite;
                var generatedProjectile = sprite != diamondSprite;
                var go = CreateSpriteObject("Ally Bolt", sprite, origin, generatedProjectile ? Color.white : fusionActive ? new Color(1f, 0.38f, 0.95f) : new Color(0.45f, 1f, 0.8f), generatedProjectile ? bulletSize * (fusionActive ? 2.2f : 2.35f) : fusionActive ? bulletSize * 0.95f : 0.12f);
                go.transform.rotation = generatedProjectile ? DirectionRotationRight(direction) : DirectionRotation(direction);
                EnforceBulletCap();
                bullets.Add(new Bullet { transform = go.transform, direction = direction, damage = bulletDamage * (fusionActive ? 0.62f : 0.25f), life = 1.05f, pierce = fusionActive ? 1 : 0, fromEnemy = false, speedMultiplier = fusionActive ? 1.1f : 0.9f });
            }
        }

        if (allyBulwark && fusionActive)
            lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 0.015f);
    }

    // ── Auto-aim: threat-weighted target selection ──
    // Instead of "nearest enemy", pick the most THREATENING target:
    //   - Boss always wins (huge priority)
    //   - Bomber close to player/lantern (about to explode)
    //   - Shooter (can hit at range)
    //   - Phantom while phasing →DEPRIORITIZE (bullets pass through)
    //   - Low-HP enemies get small finisher priority
    //   - Otherwise nearer = better
    Enemy FindNearestEnemy()
    {
        Enemy best = null;
        var bestScore = float.MaxValue;
        for (var i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            if (enemy == null || enemy.transform == null) continue;
            var distPlayer = ((Vector2)(enemy.transform.position - player.position)).sqrMagnitude;
            var distLantern = ((Vector2)(enemy.transform.position - lantern.position)).sqrMagnitude;

            // Base score = sqrt-ish distance to player (smaller = better)
            var score = distPlayer;

            // ── Boss is NOT prioritized anymore. Player wants freedom to shoot adds during boss fights. ──
            // (Old behavior: boss had -9999 score, forcing all shots to target boss only.)

            // Bomber close to player or lantern: about to explode, prioritize
            if (enemy.type == EnemyType.Bomber)
            {
                if (distPlayer < 9f) score -= 80f;     // ~3 units →critical
                if (distLantern < 9f) score -= 60f;
            }

            // Shooter in firing range: dangerous
            if (enemy.type == EnemyType.Shooter && distPlayer < 36f) score -= 25f;  // 6 units

            // Dasher about to dash (cooldown low): priority
            if (enemy.type == EnemyType.Dasher && enemy.shootCooldown < 0.6f && !enemy.isDashing) score -= 20f;

            // Phantom while phasing: bullets pass through →SKIP unless nothing else
            if (enemy.type == EnemyType.Phantom && enemy.isPhasing) score += 200f;

            // Low HP finisher: small priority for nearly dead enemies (within 2 hits)
            if (enemy.hp > 0f && enemy.maxHp > 0f && enemy.hp / enemy.maxHp < 0.25f) score -= 4f;

            if (score < bestScore)
            {
                best = enemy;
                bestScore = score;
            }
        }
        return best;
    }

    void SpawnBullet(Vector2 direction, Vector2 spawnOffset = default)
    {
        var attackStyle = GetAttackStyle();
        var sprite = GetProjectileSprite(attackStyle, out var generatedProjectile);
        var scale = GetFormBulletScale();
        var renderScale = generatedProjectile ? scale * (attackStyle == 4 ? 2.45f : 3.35f) : scale;
        var spawnPos = player.position + (Vector3)spawnOffset;
        var go = CreateSpriteObject("Bolt", sprite, spawnPos, generatedProjectile ? Color.white : GetBulletColor(), renderScale);
        if (generatedProjectile)
        {
            go.transform.rotation = DirectionRotationRight(direction);
        }
        else if (attackStyle == 1)
        {
            go.transform.localScale = new Vector3(scale * 0.55f, scale * 1.85f, 1f);
            go.transform.rotation = DirectionRotation(direction);
        }
        else if (attackStyle == 2)
        {
            go.transform.localScale = Vector3.one * scale * 1.35f;
        }
        else if (attackStyle == 3)
        {
            go.transform.localScale = Vector3.one * scale * 1.12f;
            go.transform.rotation = DirectionRotation(direction);
        }
        else if (attackStyle == 4)
        {
            go.transform.localScale = Vector3.one * scale * 1.2f;
            go.transform.rotation = DirectionRotation(direction);
        }

        EnforceBulletCap();
        bullets.Add(new Bullet
        {
            transform = go.transform,
            direction = direction,
            damage = bulletDamage * GetFormDamageMultiplier(),
            life = 1.35f * bulletLifeMultiplier * GetFormBulletLifeMultiplier(),
            pierce = bulletPierce + GetFormPierceBonus(),
            fromEnemy = false,
            speedMultiplier = GetFormBulletSpeedMultiplier(),
            hitRadius = GetFormHitRadius(),
            splashRadius = GetFormSplashRadius(),
            style = attackStyle,
            chainJumps = GetFormChainJumps(),
            chainDamageMultiplier = GetFormChainDamageMultiplier()
        });
        PlaySfx("Shoot", 760f, 0.03f, 0.08f);
    }

    Sprite GetProjectileSprite(int attackStyle, out bool generated)
    {
        generated = true;
        if (attackStyle == 1 && bulletSpeedSprite != null)
            return bulletSpeedSprite;
        if (attackStyle == 2 && bulletPowerSprite != null)
            return bulletPowerSprite;
        if (attackStyle == 3 && bulletGuardSprite != null)
            return bulletGuardSprite;
        if (attackStyle == 4 && bulletFusionSprite != null)
            return bulletFusionSprite;

        generated = false;
        return attackStyle == 1 || attackStyle == 3 || attackStyle == 4 ? diamondSprite : circleSprite;
    }

    int GetAttackStyle()
    {
        if (fusionActive)
            return 4;
        return formStyle;
    }

    float GetFormBulletScale()
    {
        if (fusionActive)
            return bulletSize * 1.22f;
        if (formStyle == 1)
            return bulletSize * 0.82f;
        if (formStyle == 2)
            return bulletSize * 1.28f;
        if (formStyle == 3)
            return bulletSize * 1.08f;
        return bulletSize;
    }

    float GetFormDamageMultiplier()
    {
        if (fusionStyle == 1)
            return 1.12f;
        if (fusionStyle == 3)
            return 1.06f;
        // SPEED: removed damage penalty (0.88→.00). Compensated by chain damage buff.
        if (formStyle == 1)
            return 1.00f;
        if (formStyle == 2)
            return 1.24f;
        // GUARD: slight damage bonus (0.96→.10) for clearing power
        if (formStyle == 3)
            return 1.10f;
        return 1f;
    }

    float GetFormBulletSpeedMultiplier()
    {
        if (fusionStyle == 2)
            return 1.42f;
        if (fusionActive)
            return 1.18f;
        if (formStyle == 1)
            return 1.48f;
        if (formStyle == 2)
            return 0.82f;
        return 1f;
    }

    float GetFormBulletLifeMultiplier()
    {
        if (formStyle == 1 || fusionStyle == 2)
            return 1.18f;
        if (formStyle == 2)
            return 0.96f;
        return 1f;
    }

    int GetFormPierceBonus()
    {
        if (fusionStyle == 1)
            return 1;
        if (formStyle == 1 && evolutionStage >= 2)
            return 1;
        if (formStyle == 3)
            return 1;
        return 0;
    }

    float GetFormHitRadius()
    {
        if (formStyle == 2 || fusionStyle == 3)
            return 0.62f;
        // SPEED hit radius 0.36→.44 (faster small bullets but still hit consistently)
        if (formStyle == 1)
            return 0.44f;
        // GUARD hit radius 0.48→.55 (chunky reliable shots)
        if (formStyle == 3)
            return 0.55f;
        return 0.48f;
    }

    float GetFormSplashRadius()
    {
        if (fusionStyle == 1)
            return 0.75f;
        if (formStyle == 2)
            return 0.68f;
        return 0f;
    }

    int GetFormChainJumps()
    {
        if (fusionStyle == 2)
            return 3 + speedChainBonus;
        // SPEED: bumped baseline +1 jump and cap raised 4→
        if (formStyle == 1)
            return Mathf.Clamp(2 + speedChainBonus + (evolutionStage >= 3 ? 1 : 0), 1, 5);
        return 0;
    }

    float GetFormChainDamageMultiplier()
    {
        if (fusionStyle == 2)
            return 0.72f;
        // SPEED chain damage 0.46→.62 (compensates removed base damage penalty)
        if (formStyle == 1)
            return 0.62f;
        return 0.4f;
    }

    void UpdateRouteAbilities()
    {
        if (formStyle == 1 || fusionStyle == 2)
            UpdateSpeedRouteAbility();
        if (formStyle == 3 || fusionStyle == 3 || guardPulseUpgrade)
            UpdateGuardRouteAbility();
    }

    void UpdateSpeedRouteAbility()
    {
        if (!speedEchoActive && fusionStyle != 2 && evolutionStage < 3)
            return;

        speedEchoTimer -= Time.deltaTime;
        if (speedEchoTimer > 0f || enemies.Count == 0)
            return;

        speedEchoTimer = fusionStyle == 2 ? 0.48f : speedEchoActive ? 0.62f : 0.9f;
        var target = FindNearestEnemy();
        if (target == null)
            return;

        var offset = Rotate(aimDirection.sqrMagnitude > 0.01f ? aimDirection : Vector2.up, 90f) * 0.32f;
        var origin = player.position + (Vector3)offset;
        var direction = ((Vector2)(target.transform.position - origin)).normalized;
        var color = fusionStyle == 2 ? new Color(0.8f, 1f, 0.28f) : new Color(0.28f, 0.95f, 1f);
        var sprite = bulletSpeedSprite != null ? bulletSpeedSprite : diamondSprite;
        var generatedProjectile = sprite != diamondSprite;
        var go = CreateSpriteObject("Mirage Bolt", sprite, origin, generatedProjectile ? Color.white : color, generatedProjectile ? bulletSize * 2.6f : bulletSize * 0.9f);
        go.transform.rotation = generatedProjectile ? DirectionRotationRight(direction) : DirectionRotation(direction);
        EnforceBulletCap();
        bullets.Add(new Bullet
        {
            transform = go.transform,
            direction = direction,
            damage = bulletDamage * (fusionStyle == 2 ? 0.48f : 0.34f),
            life = 0.9f,
            pierce = 0,
            fromEnemy = false,
            speedMultiplier = 1.7f,
            hitRadius = 0.34f,
            style = 1,
            chainJumps = Mathf.Max(0, GetFormChainJumps() - 1),
            chainDamageMultiplier = 0.42f
        });
        SpawnSparks(origin, color, 5, 0.34f);
    }

    void UpdateGuardRouteAbility()
    {
        guardPulseTimer -= Time.deltaTime;
        if (guardPulseTimer > 0f)
            return;

        guardPulseTimer = fusionStyle == 3 ? 2.25f : guardPulseUpgrade ? 2.65f : 3.35f;
        EmitGuardPulse();
    }

    void EmitGuardPulse()
    {
        var accent = fusionStyle == 3 ? new Color(1f, 0.82f, 0.28f) : new Color(0.44f, 1f, 0.58f);
        var radius = 1.85f + evolutionStage * 0.42f + (guardPulseUpgrade ? 0.68f : 0f) + (fusionStyle == 3 ? 0.9f : 0f);
        var damage = 0.55f + bulletDamage * (fusionStyle == 3 ? 0.5f : 0.32f) + evolutionStage * 0.14f;
        var center = lantern.position;

        SpawnRingSparks(center, accent, fusionStyle == 3 ? 42 : 28, radius);
        lanternHp = Mathf.Min(lanternMaxHp, lanternHp + (fusionStyle == 3 ? 0.55f : 0.28f));

        for (var i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];
            var distance = Vector2.Distance(enemy.transform.position, center);
            if (distance > radius)
                continue;

            var away = ((Vector2)(enemy.transform.position - center)).normalized;
            if (away.sqrMagnitude < 0.01f)
                away = Vector2.up;
            enemy.transform.position += (Vector3)(away * (fusionStyle == 3 ? 0.26f : 0.16f));
            DamageEnemy(enemy, enemy.type == EnemyType.Boss ? damage * 0.45f : damage, false);
        }
    }

    void SpawnRingSparks(Vector3 center, Color color, int count, float radius)
    {
        // Share the MaxSparks cap with SpawnSparks
        if (sparks.Count >= MaxSparks) return;
        if (bossEnemy != null && bossEnemy.transform != null && wave >= MaxWave)
            count = Mathf.Min(count, enhancedVisuals ? 14 : 8);
        else if (sparks.Count > MaxSparks * 0.7f)
            count = Mathf.Min(count, 8);
        count = Mathf.Min(count, MaxSparks - sparks.Count);
        for (var i = 0; i < count; i++)
        {
            var angle = Mathf.PI * 2f * i / Mathf.Max(1, count);
            var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            var position = center + (Vector3)(direction * radius * Mathf.Lerp(0.82f, 1.02f, (float)rng.NextDouble()));
            var go = CreateSpriteObject("Pulse Spark", squareSprite, position + Vector3.back * 0.05f, color, 0.075f);
            sparks.Add(new Spark
            {
                transform = go.transform,
                velocity = direction * Mathf.Lerp(0.36f, 0.72f, (float)rng.NextDouble()),
                life = 0.42f,
                maxLife = 0.42f
            });
        }
    }

    void HandleSpawning()
    {
        if (enemiesToSpawn <= 0)
            return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f)
            return;

        // ── Performance: defer spawning if too many enemies already on screen ──
        // Prevents object accumulation when player can't keep up with kills
        if (enemies.Count >= MaxEnemies)
        {
            spawnTimer = 0.25f; // retry soon
            return;
        }

        var lateSpeedBoost = wave >= 7 ? (wave - 6) * 0.02f : 0f;
        spawnTimer = Mathf.Max(0.16f, 0.78f - wave * 0.045f - lateSpeedBoost);
        enemiesToSpawn--;
        SpawnEnemy();
    }

    void SpawnEnemy() { SpawnEnemy(null, null); }

    // forceType / forcePos が指定されれば波チE�Eブル/外周配置を上書ぁE(Phase 2 Glitch Summon 等で使用)
    void SpawnEnemy(EnemyType? forceType, Vector2? forcePos)
    {
        var angle = (float)rng.NextDouble() * Mathf.PI * 2f;
        var pos = forcePos.HasValue
            ? new Vector3(forcePos.Value.x, forcePos.Value.y, 0f)
            : new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * (ArenaRadius + 0.4f);
        var roll = rng.NextDouble();
        EnemyType type;
        if (forceType.HasValue)
        {
            type = forceType.Value;
        }
        else
        // ── Stage 別の敵絁E�E差別匁E(B4: ステージ選択�E旨味を�EぁE ──
        // Stage 0 (Lantern Field): 既存テーブル (標準)
        // Stage 1 (Lava Cache): 専用のLavaCrawlerを混ぜ、�E源を踏み荒らす重め�E群れにする
        // Stage 2 (Broken Core): Phantom 大釁E/ Dasher 墁E(汚染ネットワーク = スチE��ス&高送E
        // Stage 3 (Frost Vault): Brute 多め (氷の世界に重戦士)、Bomber 少、E(クリスタル誘発)
        // Stage 4 (Storm Spire): Dasher 主役 + Shooter 墁E(高速回避忁E��、E��誘導戦術性)
        if (currentStageId == 4)
        {
            // Storm Spire: Dasher 主役 + VoltDasher 専用敵
            if (wave >= 7)
                type = roll < 0.26 ? EnemyType.Runner : roll < 0.40 ? EnemyType.Brute : roll < 0.58 ? EnemyType.Shooter
                     : roll < 0.74 ? EnemyType.Dasher : roll < 0.88 ? EnemyType.VoltDasher : roll < 0.94 ? EnemyType.Bomber : EnemyType.Phantom;
            else if (wave >= 5)
                type = roll < 0.32 ? EnemyType.Runner : roll < 0.46 ? EnemyType.Brute : roll < 0.66 ? EnemyType.Shooter
                     : roll < 0.84 ? EnemyType.Dasher : roll < 0.96 ? EnemyType.VoltDasher : EnemyType.Bomber;
            else if (wave >= 4)
                type = roll < 0.46 ? EnemyType.Runner : roll < 0.64 ? EnemyType.Brute : roll < 0.80 ? EnemyType.Shooter
                     : roll < 0.92 ? EnemyType.Dasher : EnemyType.VoltDasher;
            else if (wave >= 3)
                type = roll < 0.60 ? EnemyType.Runner : roll < 0.78 ? EnemyType.Brute : roll < 0.92 ? EnemyType.Shooter : EnemyType.VoltDasher;
            else
                type = roll < 0.78 ? EnemyType.Runner : EnemyType.Brute;
        }
        else if (currentStageId == 3)
        {
            // Frost Vault: Brute 多め + FrostKnight 専用敵
            if (wave >= 7)
                type = roll < 0.32 ? EnemyType.Runner : roll < 0.54 ? EnemyType.Brute : roll < 0.72 ? EnemyType.FrostKnight : roll < 0.86 ? EnemyType.Shooter
                     : roll < 0.92 ? EnemyType.Dasher : roll < 0.98 ? EnemyType.Bomber : EnemyType.Phantom;
            else if (wave >= 5)
                type = roll < 0.38 ? EnemyType.Runner : roll < 0.58 ? EnemyType.Brute : roll < 0.74 ? EnemyType.FrostKnight : roll < 0.90 ? EnemyType.Shooter
                     : roll < 0.96 ? EnemyType.Dasher : EnemyType.Bomber;
            else if (wave >= 4)
                type = roll < 0.56 ? EnemyType.Runner : roll < 0.76 ? EnemyType.Brute : roll < 0.90 ? EnemyType.FrostKnight : EnemyType.Shooter;
            else if (wave >= 3)
                type = roll < 0.68 ? EnemyType.Runner : roll < 0.86 ? EnemyType.Brute : roll < 0.96 ? EnemyType.FrostKnight : EnemyType.Shooter;
            else
                type = roll < 0.78 ? EnemyType.Runner : EnemyType.Brute;
        }
        else if (currentStageId == 2)
        {
            // Broken Core: Phantom 多い + CorruptionDrone 専用敵
            if (wave >= 7)
                type = roll < 0.30 ? EnemyType.Runner : roll < 0.46 ? EnemyType.Brute : roll < 0.60 ? EnemyType.Shooter
                     : roll < 0.74 ? EnemyType.Dasher : roll < 0.80 ? EnemyType.Bomber : roll < 0.92 ? EnemyType.CorruptionDrone : EnemyType.Phantom;
            else if (wave >= 5)
                type = roll < 0.36 ? EnemyType.Runner : roll < 0.50 ? EnemyType.Brute : roll < 0.68 ? EnemyType.Shooter
                     : roll < 0.80 ? EnemyType.Dasher : roll < 0.94 ? EnemyType.CorruptionDrone : EnemyType.Phantom;
            else if (wave >= 4)
                type = roll < 0.50 ? EnemyType.Runner : roll < 0.68 ? EnemyType.Brute : roll < 0.82 ? EnemyType.Shooter
                     : roll < 0.94 ? EnemyType.CorruptionDrone : EnemyType.Phantom;
            else if (wave >= 3)
                type = roll < 0.68 ? EnemyType.Runner : roll < 0.84 ? EnemyType.Brute : roll < 0.93 ? EnemyType.Shooter
                     : roll < 0.98 ? EnemyType.CorruptionDrone : EnemyType.Dasher;
            else
                type = roll < 0.80 ? EnemyType.Runner : EnemyType.Brute;
        }
        else if (currentStageId == 1)
        {
            // Lava Cache: LavaCrawler (Codex 追加渁E + MagmaTitan (新規重裁E��敵)
            if (wave >= 7)
                type = roll < 0.24 ? EnemyType.Runner : roll < 0.42 ? EnemyType.LavaCrawler : roll < 0.54 ? EnemyType.Brute
                     : roll < 0.66 ? EnemyType.MagmaTitan : roll < 0.78 ? EnemyType.Shooter : roll < 0.82 ? EnemyType.Dasher : roll < 0.93 ? EnemyType.Bomber : EnemyType.Phantom;
            else if (wave >= 5)
                type = roll < 0.28 ? EnemyType.Runner : roll < 0.46 ? EnemyType.LavaCrawler : roll < 0.56 ? EnemyType.Brute
                     : roll < 0.68 ? EnemyType.MagmaTitan : roll < 0.84 ? EnemyType.Shooter : roll < 0.90 ? EnemyType.Dasher : roll < 0.97 ? EnemyType.Bomber : EnemyType.Phantom;
            else if (wave >= 4)
                type = roll < 0.44 ? EnemyType.Runner : roll < 0.62 ? EnemyType.LavaCrawler : roll < 0.74 ? EnemyType.Brute
                     : roll < 0.84 ? EnemyType.MagmaTitan : roll < 0.94 ? EnemyType.Shooter : EnemyType.Phantom;
            else if (wave >= 3)
                type = roll < 0.56 ? EnemyType.Runner : roll < 0.76 ? EnemyType.LavaCrawler : roll < 0.86 ? EnemyType.Brute
                     : roll < 0.94 ? EnemyType.MagmaTitan : EnemyType.Shooter;
            else
                type = roll < 0.66 ? EnemyType.Runner : roll < 0.88 ? EnemyType.LavaCrawler : EnemyType.Brute;
        }
        else
        {
            // Stage 0 (チE��ォルチE  E既存テーブル
            if (wave >= 7)
                type = roll < 0.42 ? EnemyType.Runner : roll < 0.64 ? EnemyType.Brute : roll < 0.86 ? EnemyType.Shooter
                     : roll < 0.94 ? EnemyType.Dasher : roll < 0.98 ? EnemyType.Bomber : EnemyType.Phantom;
            else if (wave >= 5)
                type = roll < 0.48 ? EnemyType.Runner : roll < 0.62 ? EnemyType.Brute : roll < 0.90 ? EnemyType.Shooter
                     : roll < 0.98 ? EnemyType.Dasher : EnemyType.Bomber;
            else if (wave >= 4)
                type = roll < 0.70 ? EnemyType.Runner : roll < 0.86 ? EnemyType.Brute : EnemyType.Shooter;
            else if (wave >= 3)
                type = roll < 0.78 ? EnemyType.Runner : roll < 0.93 ? EnemyType.Brute : EnemyType.Shooter;
            else
                type = roll < 0.82 ? EnemyType.Runner : EnemyType.Brute;
        }

        // ── Wave trait による spawn 構�Eバイアス ──
        // 既孁Etrait は stat multiplier しか効ぁE��ぁE��かったびで、敵タイプにも個性を持たせめE        // (forceType 持E��時はバイアスしなぁE��専用敵 4種は維持されやすいよう Runner/Brute/Shooter 系のみ書き換ぁE
        if (!forceType.HasValue && (type == EnemyType.Runner || type == EnemyType.Brute || type == EnemyType.Shooter
            || type == EnemyType.Dasher || type == EnemyType.Phantom))
        {
            var biasRoll = rng.NextDouble();
            switch (waveTrait)
            {
                case "Shooter Raid":
                    // Wave 5: Shooter 大量投入で封E��中忁E�E防衛戦
                    if (biasRoll < 0.55) type = EnemyType.Shooter;
                    break;
                case "Rush":
                    if (biasRoll < 0.55) type = EnemyType.Runner;
                    else if (biasRoll < 0.75) type = EnemyType.Dasher;
                    break;
                case "Iron Skin":
                    if (biasRoll < 0.50) type = EnemyType.Brute;
                    break;
                case "Berserk":
                    if (biasRoll < 0.35) type = EnemyType.Dasher;
                    else if (biasRoll < 0.55) type = EnemyType.Runner;
                    break;
                case "Dark Field":
                    // Wave 4, 9: スチE��ス系 (Phantom) で奁E��
                    if (biasRoll < 0.45) type = EnemyType.Phantom;
                    break;
                case "Elite Swarm":
                    // Wave 7: Brute + Shooter 混成で硬く撃ってくる
                    if (biasRoll < 0.32) type = EnemyType.Brute;
                    else if (biasRoll < 0.60) type = EnemyType.Shooter;
                    break;
            }
        }

        // ── Per-type stats ──────────────────────────────────────────────
        float scale; Sprite sprite; Color glowColor; float hp; float speed; float touchDmg; float shotCd;
        switch (type)
        {
            case EnemyType.Dasher:
                scale = 0.44f; sprite = dasherSprite;
                glowColor = new Color(0.12f, 0.88f, 1f, 0.18f);
                hp = (1.8f + wave * 0.18f) * GetWaveHpMultiplier();
                speed = (1.7f + wave * 0.04f) * GetWaveSpeedMultiplier();
                touchDmg = 1f; shotCd = 2.2f + (float)rng.NextDouble() * 0.6f;
                break;
            case EnemyType.Bomber:
                scale = 0.65f; sprite = bomberSprite;
                glowColor = new Color(1f, 0.54f, 0.08f, 0.20f);
                hp = (2.5f + wave * 0.22f) * GetWaveHpMultiplier();
                speed = (1.2f + wave * 0.025f) * GetWaveSpeedMultiplier();
                touchDmg = 1.5f; shotCd = 0f;
                break;
            case EnemyType.LavaCrawler:
                scale = 0.62f; sprite = lavaCrawlerSprite;
                glowColor = new Color(1f, 0.34f, 0.08f, 0.22f);
                hp = (2.8f + wave * 0.28f) * GetWaveHpMultiplier();
                speed = (1.42f + wave * 0.025f) * GetWaveSpeedMultiplier();
                touchDmg = 1.35f; shotCd = 2.8f + (float)rng.NextDouble() * 0.8f;
                break;
            case EnemyType.Phantom:
                scale = 0.52f; sprite = phantomSprite;
                glowColor = new Color(0.48f, 0.10f, 0.82f, 0.18f);
                hp = (1.6f + wave * 0.16f) * GetWaveHpMultiplier();
                speed = (1.85f + wave * 0.04f) * GetWaveSpeedMultiplier();
                touchDmg = 1f; shotCd = 2.5f + (float)rng.NextDouble() * 0.8f;
                break;
            case EnemyType.Brute:
                scale = 0.82f; sprite = bruteSprite;
                glowColor = new Color(0.9f, 0.18f, 1f, 0.18f);
                hp = (4f + wave * 0.38f) * GetWaveHpMultiplier();
                speed = (1.25f + wave * 0.035f) * GetWaveSpeedMultiplier();
                touchDmg = 2f; shotCd = 0f;
                break;
            case EnemyType.Shooter:
                scale = 0.55f; sprite = shooterSprite;
                glowColor = new Color(1f, 0.78f, 0.15f, 0.16f);
                hp = (1.45f + wave * 0.19f) * GetWaveHpMultiplier();
                speed = (1.6f + wave * 0.035f) * GetWaveSpeedMultiplier();
                touchDmg = 1f; shotCd = 1.5f + (float)rng.NextDouble();
                break;
            // ── Stage 別の専用敵 (見た目+スデータス微調整、挙動�E派生�Eと同じ) ──
            case EnemyType.MagmaTitan:        // Stage2 Brute 派甁E(HP↑、E���E、橙発允E
                scale = 0.90f; sprite = magmaTitanSprite;
                glowColor = new Color(1f, 0.42f, 0.10f, 0.24f);
                hp = (5.2f + wave * 0.42f) * GetWaveHpMultiplier();
                speed = (1.10f + wave * 0.025f) * GetWaveSpeedMultiplier();
                touchDmg = 2.2f; shotCd = 0f;
                break;
            case EnemyType.CorruptionDrone:   // Stage3 Phantom 派甁E(高速、紫マゼンタ)
                scale = 0.50f; sprite = corruptionDroneSprite;
                glowColor = new Color(0.85f, 0.18f, 1f, 0.22f);
                hp = (1.4f + wave * 0.14f) * GetWaveHpMultiplier();
                speed = (2.05f + wave * 0.05f) * GetWaveSpeedMultiplier();
                touchDmg = 1f; shotCd = 2.2f + (float)rng.NextDouble() * 0.6f;
                break;
            case EnemyType.FrostKnight:       // Stage4 Brute 派甁E(重裁E��E��白)
                scale = 0.86f; sprite = frostKnightSprite;
                glowColor = new Color(0.55f, 0.85f, 1f, 0.22f);
                hp = (4.8f + wave * 0.42f) * GetWaveHpMultiplier();
                speed = (1.10f + wave * 0.025f) * GetWaveSpeedMultiplier();
                touchDmg = 2.1f; shotCd = 0f;
                break;
            case EnemyType.VoltDasher:        // Stage5 Dasher 派甁E(超加速、紫黁E
                scale = 0.48f; sprite = voltDasherSprite;
                glowColor = new Color(1f, 0.82f, 0.32f, 0.22f);
                hp = (2.0f + wave * 0.19f) * GetWaveHpMultiplier();
                speed = (1.95f + wave * 0.05f) * GetWaveSpeedMultiplier();
                touchDmg = 1f; shotCd = 1.8f + (float)rng.NextDouble() * 0.5f;
                break;
            default: // Runner
                scale = 0.48f; sprite = runnerSprite;
                glowColor = new Color(1f, 0.12f, 0.25f, 0.16f);
                hp = (1.45f + wave * 0.19f) * GetWaveHpMultiplier();
                speed = (2.35f + wave * 0.055f) * GetWaveSpeedMultiplier();
                touchDmg = 1f; shotCd = 0f;
                break;
        }

        // ── Elite 強化判宁E──
        // 基本3種 (Runner/Brute/Shooter) のみ Elite 化対象、E        // 確率�E wave 5 = 4% / wave 7 = 8% / wave 9+ = 12%。Stage 別敵・特殊敵は対象夁E        // Elite Swarm wave は +6% boost
        var eliteEligible = !forceType.HasValue && (type == EnemyType.Runner || type == EnemyType.Brute || type == EnemyType.Shooter);
        var eliteChance = wave >= 9 ? 0.12 : wave >= 7 ? 0.08 : wave >= 5 ? 0.04 : 0;
        if (waveTrait == "Elite Swarm") eliteChance += 0.06;
        var isElite = eliteEligible && rng.NextDouble() < eliteChance;
        if (isElite)
        {
            // 派手な金色 glow + 強化スチE            scale *= 1.18f;
            hp *= 1.75f;
            speed *= 1.06f;
            touchDmg *= 1.10f;
            glowColor = new Color(1f, 0.82f, 0.22f, 0.32f);  // 金色オーラ
        }

        var go = CreateSpriteObject((isElite ? "Elite " : "") + type.ToString(), sprite, pos, Color.white, scale);
        var bodyRenderer = CreateBodyRenderer(go.transform, go.GetComponent<SpriteRenderer>(), type + " Body", 0);
        var heavyKind = type == EnemyType.Brute || type == EnemyType.MagmaTitan || type == EnemyType.FrostKnight;
        var midKind = type == EnemyType.LavaCrawler;
        CreateGlow(type + " Glow", go.transform, glowColor, heavyKind ? 1.45f : midKind ? 1.2f : 1.05f);
        CreateDropShadow(go.transform, heavyKind ? 1.12f : midKind ? 0.95f : 0.82f, heavyKind ? 0.34f : 0.25f, 0.32f);
        var trimShape = (type == EnemyType.Shooter || type == EnemyType.Phantom || type == EnemyType.CorruptionDrone) ? diamondSprite : circleSprite;
        var trimScale = heavyKind ? new Vector3(0.22f, 0.22f, 1f) : new Vector3(0.16f, 0.16f, 1f);
        var coreTrim = CreateVisualTrim(go.transform, type + " Core Trim", trimShape, new Vector3(0f, -0.02f, -0.12f), trimScale, Color.Lerp(Color.white, glowColor, 0.45f), 43);
        SpriteRenderer armorTrim = null;
        if (heavyKind)
        {
            // Brute / MagmaTitan / FrostKnight に共通�E鎧トリム (色は glowColor で差別匁E
            var armorColor = type == EnemyType.MagmaTitan ? new Color(1f, 0.55f, 0.16f, 0.9f)
                           : type == EnemyType.FrostKnight ? new Color(0.6f, 0.92f, 1f, 0.9f)
                           : new Color(1f, 0.32f, 0.92f, 0.9f); // Brute (允E�E色)
            armorTrim = CreateVisualTrim(go.transform, type + " Armor Trim", diamondSprite, new Vector3(0f, 0.18f, -0.13f), new Vector3(0.18f, 0.18f, 1f), armorColor, 44);
        }

        var hpBarW = heavyKind ? 1.0f : (type == EnemyType.Bomber || type == EnemyType.LavaCrawler) ? 0.88f : type == EnemyType.Shooter ? 0.76f : 0.62f;
        var enemy = new Enemy
        {
            transform = go.transform,
            visualBody = bodyRenderer.transform,
            renderer = bodyRenderer,
            visualCore = coreTrim.transform,
            visualCoreRenderer = coreTrim,
            visualArmor = armorTrim != null ? armorTrim.transform : null,
            visualArmorRenderer = armorTrim,
            type = type,
            hp = hp, maxHp = hp,
            baseScale = go.transform.localScale,
            visualPhase = (float)rng.NextDouble() * Mathf.PI * 2f,
            lastVisualPosition = go.transform.position,
            speed = speed,
            touchDamage = touchDmg * waveTouchDamageMult * 0.85f * GetDangerTouchDamageMultiplier(),
            shootCooldown = shotCd,
            isElite = isElite
        };
        CreateEnemyHpBar(enemy, hpBarW, type == EnemyType.Brute ? 0.72f : 0.56f, glowColor);
        // Elite は追加で派手な金色オーラリングを重ねて識別性UP
        if (isElite)
        {
            CreateGlow("Elite Ring", go.transform, new Color(1f, 0.78f, 0.18f, 0.42f), 1.55f);
            SpawnRingSparks(pos, new Color(1f, 0.86f, 0.28f), 16, 0.85f);
            // ── プロ改喁E スポ�Eン時に「ELITE」テキストで予告 (見送E��防止) ──
            CreateFloatingText("⚡ ELITE", pos + new Vector3(0, 0.85f, 0), new Color(1f, 0.86f, 0.28f), 0.16f);
            PlaySfx("Boss", 220f, 0.10f, 0.20f);  // 短い金属音で警告
        }
        enemies.Add(enemy);

        var spawnAccent = new Color(glowColor.r, glowColor.g, glowColor.b, 1f);
        SpawnSparks(pos, spawnAccent, type == EnemyType.Brute ? 12 : (type == EnemyType.Bomber || type == EnemyType.LavaCrawler) ? 10 : 6,
            type == EnemyType.Brute ? 0.85f : (type == EnemyType.Bomber || type == EnemyType.LavaCrawler) ? 0.72f : 0.55f);
    }

    void SpawnBoss(bool isMidBoss = false)
    {
        // Mid-boss: smaller, lower HP, orange-red theme, name "Pulswyrm"
        var bossName = GetBossEncounterName(isMidBoss);
        var bossScale = isMidBoss ? 1.32f : 1.65f;
        var glowColor = isMidBoss ? new Color(1f, 0.46f, 0.12f, 0.22f) : new Color(1f, 0.18f, 0.9f, 0.22f);
        var coreColor = isMidBoss ? new Color(1f, 0.62f, 0.18f, 0.95f) : new Color(1f, 0.34f, 0.95f, 0.95f);
        var spawnAccent1 = isMidBoss ? new Color(1f, 0.58f, 0.10f) : new Color(1f, 0.34f, 0.96f);
        var spawnAccent2 = isMidBoss ? new Color(1f, 0.86f, 0.22f) : new Color(0.4f, 0.95f, 1f);
        var spawnAccent3 = isMidBoss ? new Color(1f, 0.42f, 0.05f) : new Color(1f, 0.18f, 0.9f);
        var hpBase = (isMidBoss ? Mathf.Min(78f, 58f + level * 1.4f) : (260f + level * 20f)) * GetDangerHpMultiplier();
        var bossSpeed = isMidBoss ? 2.0f : 2.35f;
        var touchDmg = isMidBoss ? 1.35f : 2.05f;
        var shootCd = isMidBoss ? 1.15f : 1.05f;
        var flashColor = isMidBoss ? new Color(1f, 0.52f, 0.12f, 0.42f) : new Color(0.9f, 0.1f, 1f, 0.45f);

        // Stage 1 keeps the original shared boss sprite; later stages use individual boss sprites.
        var spriteForBoss = GetBossEncounterSprite(isMidBoss);
        var go = CreateSpriteObject(bossName, spriteForBoss, new Vector3(0, ArenaRadius + 0.6f, 0), GetBossEncounterTint(isMidBoss), bossScale);
        var bodyRenderer = CreateBodyRenderer(go.transform, go.GetComponent<SpriteRenderer>(), bossName + " Body", 0);
        CreateGlow(bossName + " Glow", go.transform, glowColor, isMidBoss ? 2.1f : 2.6f);
        CreateDropShadow(go.transform, isMidBoss ? 1.25f : 1.55f, isMidBoss ? 0.36f : 0.45f, 0.42f);
        var bossCore = CreateVisualTrim(go.transform, bossName + " Core Trim", diamondSprite, new Vector3(0f, -0.08f, -0.14f), new Vector3(0.26f, 0.26f, 1f), coreColor, 48);
        bossEnemy = new Enemy
        {
            transform = go.transform,
            visualBody = bodyRenderer.transform,
            renderer = bodyRenderer,
            visualCore = bossCore.transform,
            visualCoreRenderer = bossCore,
            type = EnemyType.Boss,
            hp = hpBase,
            maxHp = hpBase,
            baseScale = go.transform.localScale,
            visualPhase = (float)rng.NextDouble() * Mathf.PI * 2f,
            lastVisualPosition = go.transform.position,
            speed = bossSpeed,
            touchDamage = touchDmg,
            shootCooldown = shootCd,
            // 特殊攻撃�E初回クール (登場直後にぁE��なり撃たれなぁE��ぁE��亁E
            isMidBoss = isMidBoss,
            bossSpecialCooldown = isMidBoss ? 4.5f : 6.0f,
            bossChargeTimer = 0f,
            bossCoreMarkTimer = 0f,
            // Phase 2-only values, activated later by TryTriggerBossPhase2.
            phase2Triggered = false,
            phase2TransitionGrace = 0f,
            spiralCooldown = 9f,
            spiralTelegraphTimer = 0f,
            summonCooldown = 12f,
            firstSummonDone = false
        };
        enemies.Add(bossEnemy);
        // Phase 2 で色を変える�Eで、新ボス出現時に Phase 1 色へリセチE��
        if (bossBarFill != null)
            bossBarFill.color = new Color(0.95f, 0.18f, 0.78f);
        // ボス HP バー UI はユーザー持E��で非表示匁E(見た目と体感だけでボス戦を演�Eする方釁E
        // bossBarRoot.SetActive(true);
        ShowMessage((isMidBoss ? "中ボス出現 " : "ボス出現 ") + bossName);
        Flash(flashColor, isMidBoss ? 0.7f : 0.9f);
        Shake(isMidBoss ? 0.5f : 0.7f, isMidBoss ? 0.18f : 0.28f);
        PlaySfx("Boss", isMidBoss ? 120f : 92f, isMidBoss ? 0.22f : 0.28f, isMidBoss ? 0.5f : 0.6f);
        SpawnRingSparks(go.transform.position, spawnAccent1, isMidBoss ? 42 : 56, 1.4f);
        SpawnRingSparks(go.transform.position, spawnAccent2, isMidBoss ? 28 : 36, 2.2f);
        SpawnSparks(go.transform.position, spawnAccent3, isMidBoss ? 36 : 48, isMidBoss ? 2.0f : 2.6f);
    }

    // ─────────────────────────────────────────────────────────
    // ボス専用シグネチャ攻撃    // Pulswyrm (中ボス, isMidBoss=true): 突進アタチE��  E5秒毎にプレイヤーに向かって 0.45秒ダチE��ュ
    // Nullwyrm (ラスボス, isMidBoss=false): Core Mark  E8秒毎にコアを狙ぁE��イン攻撃(1.0秒予告→発勁E
    //   + HP<50% で Phase 2 移衁E(Spiral Corruption / Glitch Summon / 攻撃破匁E
    // ─────────────────────────────────────────────────────────
    void UpdateBossSpecialAttack(Enemy boss, Vector2 dirToPlayer, float distToPlayer)
    {
        if (boss == null || boss.transform == null) return;
        var dt = Time.deltaTime;

        if (boss.isMidBoss)
        {
            // ── Pulswyrm mini-enrage 検査 (HP <= 50%) ──
            if (!boss.phase2Triggered && boss.hp <= boss.maxHp * 0.5f)
                TriggerMidBossEnrage(boss);

            // ── Pulswyrm 突進アタチE�� ──
            if (boss.bossChargeTimer > 0f)
            {
                // 突進中: 直進、強ぁEknockback
                boss.bossChargeTimer -= dt;
                boss.transform.position += (Vector3)(boss.bossChargeDir * 8.5f * dt);
                if (Vector2.Distance(boss.transform.position, player.position) < 0.9f)
                {
                    DamagePlayer(2.2f, "Pulswyrm 突進");
                    // 命中演�E + 即終亁E(連続ヒチE��防止)
                    Shake(0.30f, 0.15f);
                    HitFreeze(0.08f);
                    SpawnSparks(player.position, new Color(1f, 0.62f, 0.18f), 12, 1.0f);
                    boss.bossChargeTimer = 0f;
                }
                if (boss.bossChargeTimer <= 0f)
                {
                    SpawnSparks(boss.transform.position, new Color(1f, 0.62f, 0.18f), 16, 1.2f);
                }
            }
            else
            {
                boss.bossSpecialCooldown -= dt;
                if (boss.bossSpecialCooldown <= 0f && distToPlayer > 1.8f && distToPlayer < 8f)
                {
                    boss.bossSpecialCooldown = 5.0f;
                    boss.bossChargeTimer = 0.45f;
                    boss.bossChargeDir = dirToPlayer;
                    SpawnRingSparks(boss.transform.position, new Color(1f, 0.58f, 0.12f), 18, 0.9f);
                    SpawnAttackFlash(boss.transform.position, dirToPlayer, new Color(1f, 0.62f, 0.18f), 1.4f, true);
                    Shake(0.15f, 0.10f);
                    PlaySfx("Hit", 180f, 0.20f, 0.32f);
                    AddEventLog("PULSWYRM 突進!");
                }
            }
        }
        else
        {
            // ── Nullwyrm Core Mark ──
            if (boss.bossCoreMarkTimer > 0f)
            {
                // チE��グラフ中: ライン視要E+ カウントダウン
                boss.bossCoreMarkTimer -= dt;
                if (UnityEngine.Random.value < 0.4f)
                {
                    var t = UnityEngine.Random.value;
                    var pos = Vector2.Lerp(boss.transform.position, boss.bossCoreMarkTarget, t);
                    SpawnSparks(pos, new Color(1f, 0.32f, 0.92f), 1, 0.18f);
                }
                if (boss.bossCoreMarkTimer <= 0f)
                {
                    // 発勁E コアまでの直線ダメージ
                    var startPos = (Vector2)boss.transform.position;
                    var endPos = boss.bossCoreMarkTarget;
                    var lineColor = new Color(1f, 0.32f, 0.95f);
                    // コアにダメージ (ラスボスの主攻撃  EDamageLantern を通すと
                    // gameOver/victory ガード�ECORE 表示・eggHitPulse が�E通で動く
                    var coreLanternDist = Vector2.Distance(endPos, lantern.position);
                    if (coreLanternDist < 1.5f)
                    {
                        DamageLantern(2.5f);
                        Flash(new Color(1f, 0.32f, 0.92f, 0.30f), 0.40f);
                    }
                    // 直線上�Eプレイヤーにダメージ
                    if (DistancePointToSegment(player.position, startPos, endPos) < 0.6f)
                        DamagePlayer(1.5f, "Nullwyrm Core Mark");
                    // 直線上�E敵にもダメージ (副次効极E
                    for (var e = enemies.Count - 1; e >= 0; e--)
                    {
                        var en = enemies[e];
                        if (en == boss || en == null || en.transform == null) continue;
                        if (DistancePointToSegment(en.transform.position, startPos, endPos) < 0.5f)
                            DamageEnemy(en, 2.0f, false);
                    }
                    // 派手な発動演�E
                    SpawnRingSparks(endPos, lineColor, 24, 1.6f);
                    SpawnSparks(endPos, lineColor, 22, 1.4f);
                    Shake(0.35f, 0.20f);
                    HitFreeze(0.10f);
                    PlaySfx("Boss", 100f, 0.30f, 0.45f);
                    AddEventLog("NULLWYRM CORE MARK 発動");
                }
            }
            else
            {
                boss.bossSpecialCooldown -= dt;
                // mercy: コア HP < 35% で Core Mark スキップ (詰み回避)
                if (boss.bossSpecialCooldown <= 0f && lanternHp > lanternMaxHp * 0.35f)
                {
                    // Phase 2 は cooldown 8→0s、telegraph 1.0→.35s に伸ばして
                    // spec の coreMarkCooldown=10 / coreMarkTelegraphSeconds=1.35 に合わせる
                    boss.bossSpecialCooldown = boss.phase2Triggered ? 10f : 8.0f;
                    boss.bossCoreMarkTimer = boss.phase2Triggered ? 1.35f : 1.0f;
                    boss.bossCoreMarkTarget = lantern.position;
                    Flash(new Color(1f, 0.32f, 0.92f, 0.12f), 0.30f);
                    SpawnRingSparks(boss.transform.position, new Color(1f, 0.32f, 0.95f), 22, 1.4f);
                    ShowMessage("⚠ CORE MARK");
                    AddEventLog("NULLWYRM CORE MARK 予告");
                    PlaySfx("Hit", 140f, 0.20f, 0.35f);
                }
            }

            // ── Phase 2 トリガー検査 (HP < 50% で 1度だぁE ──
            TryTriggerBossPhase2(boss);

            // ── Phase 2 専用攻撃 Spiral Corruption + Glitch Summon ──
            if (boss.phase2Triggered)
            {
                if (boss.phase2TransitionGrace > 0f)
                    boss.phase2TransitionGrace -= dt;
                UpdateBossPhase2Attacks(boss, dt);
            }
        }
    }

    // ─────────────────────────────────────────────────────────
    // Boss Phase 2 (Nullwyrm のみ)
    // docs/BOSS_PHASE2_SPEC.md 準拠
    // ─────────────────────────────────────────────────────────
    void TryTriggerBossPhase2(Enemy boss)
    {
        if (boss == null || boss.transform == null) return;
        if (boss.isMidBoss || boss.phase2Triggered) return;
        if (boss.hp > boss.maxHp * 0.5f) return;
        // 選抁E/ リザルチE/ カチE��イン中はトリガーしなぁE(spec: ユーザー操作征E��中の不�E平回避)
        if (paused || gameOver || victory || choosingUpgrade || choosingRelic) return;
        if (evolutionCutsceneTimer > 0f) return;
        TriggerBossPhase2(boss);
    }

    void TriggerBossPhase2(Enemy boss)
    {
        boss.phase2Triggered = true;
        boss.phase2TransitionGrace = 3.0f;  // spec: first 3s low pressure only
        // Spiral/Summon は grace 後に初回が来るよぁE��ールを調整
        boss.spiralCooldown = 3.5f;
        boss.summonCooldown = 5.0f;
        boss.firstSummonDone = false;

        // ── 視覚変化: スケール +14%、色シフト、持続オーラ ──
        if (boss.transform != null)
        {
            boss.baseScale = boss.transform.localScale * 1.14f;
            boss.transform.localScale = boss.baseScale;
            // プロ改喁E HP バー非表示なので body tint + 持続オーラで Phase 2 を恒乁E��覚化
            if (boss.renderer != null)
                boss.renderer.color = new Color(0.85f, 0.65f, 1f, 1f);  // 紫マゼンタ寁E��に body tint
            CreateGlow("Phase 2 Aura", boss.transform, new Color(1f, 0.32f, 0.92f, 0.45f), 3.2f);
            CreateGlow("Phase 2 Aura Cyan", boss.transform, new Color(0.6f, 0.92f, 1f, 0.32f), 4.0f);
        }
        // ── HP バー色めEhot magenta に固宁E(Phase 2 識別、現在は非表示だが封E��復活時�E保険) ──
        if (bossBarFill != null)
            bossBarFill.color = new Color(1f, 0.18f, 0.92f);

        // ── Transition Pulse: 拡張警告リング (低ダメージ + knockback) ──
        var pulsePos = (Vector2)boss.transform.position;
        SpawnRingSparks(pulsePos, new Color(0.6f, 0.92f, 1f), 48, 2.4f);
        SpawnRingSparks(pulsePos, new Color(1f, 0.32f, 0.92f), 36, 1.6f);
        SpawnSparks(pulsePos, new Color(1f, 0.18f, 0.92f), 40, 2.6f);
        // grace=0.65s の範囲冁E��ら�Eレイヤーは外�Eへ避けやすい。中忁E2m 以冁E�Eみ軽ダメ
        if (Vector2.Distance(player.position, pulsePos) < 2.0f)
            DamagePlayer(0.45f);

        // ── HUD/演�E ──
        Flash(new Color(1f, 0.32f, 0.92f, 0.42f), 0.55f);
        Shake(0.45f, 0.25f);
        HitFreeze(0.12f);
        PlaySfx("Boss", 70f, 0.45f, 0.7f);
        ShowMessage("NULLWYRM PHASE 2");
        AddEventLog("NULLWYRM PHASE 2 突入");
    }

    // ─────────────────────────────────────────────────────────
    // Pulswyrm mini-enrage (中ボス専用、軽量版)
    // spec: 移動×.12, 弾 3→, 一度の警告pulse, クールはほぼ同じ
    // ─────────────────────────────────────────────────────────
    void TriggerMidBossEnrage(Enemy boss)
    {
        boss.phase2Triggered = true;  // 共通フラグを�E利用 (boss.isMidBoss と絁E��合わせて刁E��E

        // 移動速度 ×.12 (本体�E speed フィールドを直接ブ�EスチE
        boss.speed *= 1.12f;

        // ── プロ改喁E HP バー非表示なので enrage 持続視覚化が忁E��E──
        if (boss.renderer != null)
            boss.renderer.color = new Color(1f, 0.72f, 0.45f, 1f);
        // 持続オーラ glow を追加 (毎フレーム持つ視覚�Eーカー)
        if (boss.transform != null)
            CreateGlow("Enrage Aura", boss.transform, new Color(1f, 0.42f, 0.10f, 0.42f), 2.6f);

        // 警告pulse 演�E (Phase 2 ほど派手にしなぁE
        var pulsePos = (Vector2)boss.transform.position;
        SpawnRingSparks(pulsePos, new Color(1f, 0.62f, 0.18f), 32, 1.8f);
        SpawnSparks(pulsePos, new Color(1f, 0.86f, 0.22f), 26, 1.6f);
        Flash(new Color(1f, 0.52f, 0.12f, 0.28f), 0.35f);
        Shake(0.22f, 0.14f);
        PlaySfx("Boss", 95f, 0.28f, 0.5f);
        ShowMessage("PULSWYRM ENRAGE");
        AddEventLog("PULSWYRM enrage 突入");
    }

    void UpdateBossPhase2Attacks(Enemy boss, float dt)
    {
        // grace 中は新規攻撃破起こさなぁE(既に進行中のチE��グラフ�E続衁E
        if (boss.phase2TransitionGrace > 0f) return;

        // ── Spiral Corruption ──
        // 14発スパイラル、Core Mark チE��グラフ中は発動しなぁE(重�E回避)
        if (boss.spiralTelegraphTimer > 0f)
        {
            boss.spiralTelegraphTimer -= dt;
            // チE��グラフ視要E ボス周りに回転リング
            if (UnityEngine.Random.value < 0.5f)
            {
                var a = Time.time * 6f;
                var r = 1.2f + 0.4f * Mathf.Sin(Time.time * 8f);
                var p = (Vector2)boss.transform.position + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * r;
                SpawnSparks(p, new Color(0.6f, 0.92f, 1f), 1, 0.18f);
            }
            if (boss.spiralTelegraphTimer <= 0f)
                FireBossSpiral(boss);
        }
        else if (boss.bossCoreMarkTimer <= 0f)  // Core Mark 中は spiral 控える
        {
            boss.spiralCooldown -= dt;
            if (boss.spiralCooldown <= 0f)
            {
                boss.spiralCooldown = 5.5f;
                boss.spiralTelegraphTimer = 0.75f;
                SpawnRingSparks(boss.transform.position, new Color(0.6f, 0.92f, 1f), 18, 1.0f);
                PlaySfx("Hit", 220f, 0.16f, 0.25f);
                AddEventLog("SPIRAL CORRUPTION 予告");
            }
        }

        // ── Glitch Summon ──
        // 11秒毎、敵総数 > 14 でスキップ、�E回�E Runner のみ
        boss.summonCooldown -= dt;
        if (boss.summonCooldown <= 0f)
        {
            var liveCount = 0;
            for (var i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] != null && enemies[i].transform != null && enemies[i] != boss)
                    liveCount++;
            }
            if (liveCount > 14)
            {
                // Mercy: skip while crowded, then retry soon.
                boss.summonCooldown = 4f;
            }
            else
            {
                boss.summonCooldown = 11f;
                if (!boss.firstSummonDone)
                {
                    SpawnBossMinions(boss, 2, 0);  // 初回 Runner ×
                    boss.firstSummonDone = true;
                }
                else
                {
                    // 通常: Runner × + Phantom × (敵数 < 10 のときだぁEPhantom)
                    var phantomCount = liveCount < 10 ? 1 : 0;
                    SpawnBossMinions(boss, 2, phantomCount);
                }
            }
        }
    }

    void FireBossSpiral(Enemy boss)
    {
        if (boss == null || boss.transform == null) return;
        const int spiralBulletCount = 14;
        const float spiralBulletSpeed = 2.8f;
        const float spiralBulletDamage = 0.45f;
        const float spiralRotationStep = 360f / spiralBulletCount;
        var origin = (Vector2)boss.transform.position;
        var baseAngle = Time.time * 60f;
        for (var i = 0; i < spiralBulletCount; i++)
        {
            var angle = baseAngle + i * spiralRotationStep;
            var rad = angle * Mathf.Deg2Rad;
            var dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            var go = CreateSpriteObject("Spiral Bolt", diamondSprite, origin, new Color(0.6f, 0.92f, 1f), 0.24f);
            EnforceBulletCap();
            bullets.Add(new Bullet { transform = go.transform, direction = dir, damage = spiralBulletDamage, life = 3.2f, fromEnemy = true, speedMultiplier = spiralBulletSpeed / 4.5f });
        }
        SpawnRingSparks(origin, new Color(0.6f, 0.92f, 1f), 28, 1.4f);
        Shake(0.18f, 0.10f);
        PlaySfx("Boss", 130f, 0.22f, 0.40f);
        AddEventLog("SPIRAL CORRUPTION 発動");
    }

    void SpawnBossMinions(Enemy boss, int runnerCount, int phantomCount)
    {
        if (boss == null || boss.transform == null) return;
        const float minRadiusFromCore = 4.5f;
        // コアから minRadiusFromCore 以上離れたランダム地点を選ぶ
        for (var n = 0; n < runnerCount + phantomCount; n++)
        {
            var attempts = 0;
            Vector2 spawnPos;
            do
            {
                var theta = (float)(rng.NextDouble() * Mathf.PI * 2);
                var r = minRadiusFromCore + (float)rng.NextDouble() * 2.5f;
                spawnPos = (Vector2)lantern.position + new Vector2(Mathf.Cos(theta) * r, Mathf.Sin(theta) * r);
                attempts++;
            } while (attempts < 6 && Vector2.Distance(spawnPos, lantern.position) < minRadiusFromCore);

            var minionType = n < runnerCount ? EnemyType.Runner : EnemyType.Phantom;
            SpawnEnemy(minionType, spawnPos);
            // スポ�Eン視要E(グリチE��愁E
            SpawnSparks(spawnPos, new Color(1f, 0.32f, 0.92f), 10, 0.7f);
            SpawnRingSparks(spawnPos, new Color(0.6f, 0.92f, 1f), 12, 0.6f);
        }
        AddEventLog($"GLITCH SUMMON: Runner x{runnerCount}" + (phantomCount > 0 ? $" / Phantom x{phantomCount}" : ""));
    }

    void UpdateEnemies()
    {
        if (playerKnockbackCooldown > 0f)
            playerKnockbackCooldown -= Time.deltaTime;
        for (var i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];
            UpdateEnemyVisual(enemy);

            // ── Apply smooth knockback velocity (slides enemy back instead of teleporting) ──
            if (enemy.knockbackVelocity.sqrMagnitude > 0.001f)
            {
                enemy.transform.position += (Vector3)(enemy.knockbackVelocity * Time.deltaTime);
                // Exponential decay so velocity dies off in ~0.22s regardless of framerate.
                // decayFactor^Time.deltaTime where decayFactor=0.0008 →0.22s half-life-ish
                enemy.knockbackVelocity *= Mathf.Pow(0.0008f, Time.deltaTime);
                // Cut-off below threshold to fully stop (avoid tiny lingering drift)
                if (enemy.knockbackVelocity.sqrMagnitude < 0.04f)
                    enemy.knockbackVelocity = Vector2.zero;
            }

            var target = enemy.type == EnemyType.Boss
                ? (Vector2)player.position
                : (Vector2.Distance(enemy.transform.position, lantern.position) < Vector2.Distance(enemy.transform.position, player.position)
                    ? (Vector2)lantern.position
                    : (Vector2)player.position);

            if (enemy.type == EnemyType.Boss)
            {
                // ── Active boss AI: dynamic approach + strafe + dash bursts ──
                var toPlayer = (Vector2)player.position - (Vector2)enemy.transform.position;
                var distToPlayer = toPlayer.magnitude;
                var dirToPlayer = distToPlayer > 0.01f ? toPlayer / distToPlayer : Vector2.right;
                // Strafe: perpendicular sway scaled by time, creates dynamic dodge motion
                var perpend = new Vector2(-dirToPlayer.y, dirToPlayer.x);
                var strafe = perpend * Mathf.Sin(Time.time * 1.6f + enemy.visualPhase) * 0.55f;
                // Maintain ideal range: if too far, charge in; if too close, back off slightly
                var idealRange = enemy.phase2Triggered ? 3.6f : 3.2f;
                var rangeFactor = distToPlayer > idealRange ? 1.0f : distToPlayer < idealRange * 0.7f ? -0.35f : 0.5f;
                var moveDir = (dirToPlayer * rangeFactor + strafe).normalized;
                // Phase 2 速度補正 ×.08
                var phaseSpeedMul = enemy.phase2Triggered ? 1.08f : 1f;
                enemy.transform.position += (Vector3)(moveDir * enemy.speed * phaseSpeedMul * Time.deltaTime);

                enemy.shootCooldown -= Time.deltaTime;
                // Phase 2 突入直後�Eグレース中は封E��しなぁE(低圧劁E
                var p2Grace = enemy.phase2TransitionGrace > 0f;
                if (enemy.shootCooldown <= 0f && !p2Grace)
                {
                    // Boss pressure is readable instead of forcing only top-tier partners.
                    // Pulswyrm (mid): 3発, Nullwyrm Phase1: 5発, Nullwyrm Phase2: 6発
                    var bigBoss = !enemy.isMidBoss;
                    var phase2 = enemy.phase2Triggered;
                    // Pulswyrm enrage 時�E 3→発に増加、E��封E��少し短縮
                    enemy.shootCooldown = enemy.isMidBoss
                        ? (phase2 ? 1.02f : 1.18f)
                        : (phase2 ? 0.92f : 1.05f);
                    var shotCount = enemy.isMidBoss ? (phase2 ? 4 : 3) : (phase2 ? 6 : 5);
                    var spreadDeg = enemy.isMidBoss ? (phase2 ? 22f : 18f) : (phase2 ? 34f : 26f);
                    for (var shot = 0; shot < shotCount; shot++)
                    {
                        var t = shotCount == 1 ? 0.5f : shot / (float)(shotCount - 1);
                        var angle = Mathf.Lerp(-spreadDeg, spreadDeg, t);
                        var shotDirection = Rotate(dirToPlayer, angle);
                        var bulletColor = phase2 ? new Color(0.6f, 0.92f, 1f) : new Color(1f, 0.18f, 0.82f);
                        var go = CreateSpriteObject("Virus Bolt", diamondSprite, enemy.transform.position, bulletColor, 0.28f);
                        EnforceBulletCap();
                        bullets.Add(new Bullet { transform = go.transform, direction = shotDirection, damage = bigBoss ? 0.92f : 0.72f, life = 2.8f, fromEnemy = true });
                    }
                    enemy.attackPulse = 1f;
                    SpawnAttackFlash(enemy.transform.position, dirToPlayer, phase2 ? new Color(0.6f, 0.92f, 1f) : new Color(1f, 0.18f, 0.82f), 0.88f, true);
                }

                // ── ボス専用シグネチャ攻撃──
                UpdateBossSpecialAttack(enemy, dirToPlayer, distToPlayer);
            }
            else if (enemy.type == EnemyType.Shooter && Vector2.Distance(enemy.transform.position, player.position) < 5.6f)
            {
                enemy.shootCooldown -= Time.deltaTime;
                if (enemy.shootCooldown <= 0f)
                {
                    enemy.shootCooldown = 2.45f;
                    var direction = ((Vector2)(player.position - enemy.transform.position)).normalized;
                    var go = CreateSpriteObject("Hex", diamondSprite, enemy.transform.position, new Color(1f, 0.7f, 0.25f), 0.2f);
                    EnforceBulletCap();
                    bullets.Add(new Bullet { transform = go.transform, direction = direction, damage = 0.72f, life = 2.3f, fromEnemy = true });
                    enemy.attackPulse = 1f;
                    SpawnAttackFlash(enemy.transform.position, direction, new Color(1f, 0.74f, 0.2f), 0.48f, false);
                }
            }
            else if (enemy.type == EnemyType.Dasher)
            {
                // Periodically dash at 4×speed for 0.5s
                if (enemy.isDashing)
                {
                    var dashDir = (target - (Vector2)enemy.transform.position).normalized;
                    enemy.transform.position += (Vector3)(dashDir * enemy.speed * 4.2f * Time.deltaTime);
                    enemy.dashDuration -= Time.deltaTime;
                    if (enemy.dashDuration <= 0f)
                        enemy.isDashing = false;
                }
                else
                {
                    var walkDir = (target - (Vector2)enemy.transform.position).normalized;
                    enemy.transform.position += (Vector3)(walkDir * enemy.speed * 0.55f * Time.deltaTime);
                    enemy.shootCooldown -= Time.deltaTime;
                    if (enemy.shootCooldown <= 0f)
                    {
                        enemy.isDashing = true;
                        enemy.dashDuration = 0.48f;
                        enemy.attackPulse = 0.75f;
                        enemy.shootCooldown = 2.2f + (float)rng.NextDouble() * 0.8f;
                        SpawnSparks(enemy.transform.position, new Color(0.2f, 1f, 1f), 8, 0.7f);
                    }
                }
            }
            else if (enemy.type == EnemyType.Bomber)
            {
                // Slow and steady march toward nearest target
                var bombDir = (target - (Vector2)enemy.transform.position).normalized;
                var rushMult = Vector2.Distance(enemy.transform.position, target) < 1.5f ? 1.5f : 1f;
                enemy.transform.position += (Vector3)(bombDir * enemy.speed * rushMult * Time.deltaTime);
            }
            else if (enemy.type == EnemyType.LavaCrawler)
            {
                var crawlDir = (target - (Vector2)enemy.transform.position).normalized;
                enemy.transform.position += (Vector3)(crawlDir * enemy.speed * 0.82f * Time.deltaTime);
                enemy.shootCooldown -= Time.deltaTime;
                if (enemy.shootCooldown <= 0f)
                {
                    enemy.shootCooldown = 3.0f + (float)rng.NextDouble() * 0.8f;
                    enemy.attackPulse = 0.72f;
                    SpawnRingSparks(enemy.transform.position, new Color(1f, 0.34f, 0.08f, 0.86f), 8, 0.5f);
                }
            }
            else if (enemy.type == EnemyType.Phantom)
            {
                // Phase in/out: shootCooldown drives the cycle
                enemy.shootCooldown -= Time.deltaTime;
                if (enemy.shootCooldown <= 0f)
                {
                    enemy.isPhasing = !enemy.isPhasing;
                    enemy.shootCooldown = enemy.isPhasing ? 1.2f : 2.5f + (float)rng.NextDouble() * 0.8f;
                    if (enemy.isPhasing)
                        SpawnSparks(enemy.transform.position, new Color(0.6f, 0.18f, 1f), 10, 0.6f);
                }
                // Move faster while phasing, normal otherwise
                var phantomDir = (target - (Vector2)enemy.transform.position).normalized;
                var phantomMult = enemy.isPhasing ? 1.8f : 1f;
                enemy.transform.position += (Vector3)(phantomDir * enemy.speed * phantomMult * Time.deltaTime);
            }
            else
            {
                var direction = (target - (Vector2)enemy.transform.position).normalized;
                enemy.transform.position += (Vector3)(direction * enemy.speed * Time.deltaTime);
            }

            var contactRange = enemy.type == EnemyType.Boss ? 1.05f : (enemy.type == EnemyType.Bomber || enemy.type == EnemyType.LavaCrawler) ? 0.68f : 0.55f;
            if (Vector2.Distance(enemy.transform.position, player.position) < contactRange)
            {
                // ── Knockback mode: hard radial burst + instant push ─────────
                // Only triggers if player has knockback (GUARD form / dedicated modules).
                // Bosses get a smaller push; bombers are now eligible (so you can punt them away).
                if (playerKnockback > 0f)
                {
                    if (enemy.type != EnemyType.Boss)
                    {
                        // ── Smooth knockback: SET velocity (decayed each frame), NOT instant teleport ──
                        // Enemies are pushed back over ~0.2s instead of warping 1 frame, fixing motion sickness.
                        DamagePlayer(enemy.touchDamage * 0.30f);
                        if (contactBurst > 0f)
                            DamageEnemy(enemy, contactBurst, true);
                        var pushDir = ((Vector2)(enemy.transform.position - player.position)).normalized;
                        if (pushDir.sqrMagnitude < 0.01f) pushDir = Vector2.up;
                        // Convert former "instant 1m push" into a velocity that decays.
                        // Target = ~1m total travel over ~0.22s, so initial vel ≁E6 m/s with 80%/s decay.
                        var contactStrength = (enemy.type == EnemyType.Brute ? 0.45f : (enemy.type == EnemyType.Bomber || enemy.type == EnemyType.LavaCrawler) ? 0.75f : 1f)
                                              * playerKnockback * 3.2f;  // 3.2 = m/s coefficient
                        enemy.knockbackVelocity += pushDir * contactStrength;
                        enemy.hitPulse = Mathf.Max(enemy.hitPulse, 0.65f);

                        // Radial AoE burst  Esets knockback velocity on nearby enemies too.
                        if (playerKnockbackCooldown <= 0f)
                        {
                            playerKnockbackCooldown = PlayerKnockbackInterval;
                            var radius = 2.2f + playerKnockback * 0.22f;
                            for (var k = enemies.Count - 1; k >= 0; k--)
                            {
                                if (k >= enemies.Count) continue;
                                var ne = enemies[k];
                                if (ne == null || ne.transform == null || ne == enemy) continue;
                                if (ne.type == EnemyType.Boss) continue;
                                var nd = Vector2.Distance(ne.transform.position, player.position);
                                if (nd > radius) continue;
                                var nDir = ((Vector2)(ne.transform.position - player.position)).normalized;
                                if (nDir.sqrMagnitude < 0.01f) nDir = Vector2.up;
                                var falloff = Mathf.Lerp(0.55f, 1.0f, 1f - (nd / radius));
                                var strength = (ne.type == EnemyType.Brute ? 0.45f : 1f)
                                               * playerKnockback * 2.6f * falloff;  // m/s
                                ne.knockbackVelocity += nDir * strength;
                                ne.hitPulse = Mathf.Max(ne.hitPulse, 0.4f);
                                if (contactBurst > 0f)
                                    DamageEnemy(ne, contactBurst * 0.4f, false);
                            }
                            SpawnRingSparks(player.position, new Color(0.42f, 1f, 0.65f, 0.92f), 18, radius * 0.65f);
                            Flash(new Color(0.45f, 1f, 0.65f, 0.16f), 0.18f);
                            Shake(0.15f, 0.06f);
                            PlaySfx("Hit", 220f, 0.07f, 0.22f);
                        }

                        if (i < enemies.Count && enemies[i] == enemy)
                            CheckLose();
                        continue;
                    }
                    else
                    {
                        // Boss: small steady velocity bump (no teleport, no big push)
                        var bDir = ((Vector2)(enemy.transform.position - player.position)).normalized;
                        if (bDir.sqrMagnitude < 0.01f) bDir = Vector2.up;
                        enemy.knockbackVelocity += bDir * playerKnockback * 0.35f;
                        // fall through to normal boss contact damage logic
                    }
                }

                DamagePlayer(enemy.type == EnemyType.Boss ? enemy.touchDamage * Time.deltaTime : enemy.touchDamage,
                    (enemy.isElite ? "Elite " : "") + enemy.type + " 接触");
                if (contactBurst > 0f && enemy.type == EnemyType.Boss)
                    DamageEnemy(enemy, contactBurst, true);
                if (enemy.type != EnemyType.Boss)
                {
                    KillEnemy(enemy, false);
                    enemies.RemoveAt(i);
                }
                CheckLose();
                continue;
            }

            if (Vector2.Distance(enemy.transform.position, lantern.position) < (enemy.type == EnemyType.Boss ? 1.2f : 0.78f))
            {
                DamageLantern(enemy.type == EnemyType.Boss ? enemy.touchDamage * 1.05f * Time.deltaTime : enemy.touchDamage * 1.25f);
                if (enemy.type != EnemyType.Boss)
                {
                    KillEnemy(enemy, false);
                    enemies.RemoveAt(i);
                }
                CheckLose();
                continue;
            }

            // Phantom manages its own alpha (phase flicker); skip standard light-fade for it
            if (enemy.type != EnemyType.Phantom)
            {
                var effectiveLight = waveTrait == "Dark Field" ? lightRadius * 0.76f : lightRadius;
                if (Vector2.Distance(enemy.transform.position, lantern.position) > effectiveLight + 3.8f && enemy.type != EnemyType.Boss)
                {
                    SetEnemyBodyAlpha(enemy, 0.28f);
                }
                else
                {
                    SetEnemyBodyAlpha(enemy, 1f);
                }
            }

            UpdateEnemyHpBar(enemy);
        }
    }

    void UpdateEnemyVisual(Enemy enemy)
    {
        if (enemy == null || enemy.transform == null)
            return;

        var time = Time.time + enemy.visualPhase;
        if (enemy.hitPulse > 0f)
            enemy.hitPulse = Mathf.MoveTowards(enemy.hitPulse, 0f, Time.deltaTime * 3.6f);

        // Phantom: flicker alpha when phasing
        if (enemy.type == EnemyType.Phantom)
        {
            var phaseAlpha = enemy.isPhasing ? 0.28f + Mathf.Sin(time * 14f) * 0.14f : 1f;
            SetEnemyBodyAlpha(enemy, phaseAlpha);
        }
        // Dasher: scale spike during dash
        var dasherBoost = (enemy.type == EnemyType.Dasher && enemy.isDashing) ? 0.12f : 0f;
        var typePulse = enemy.type == EnemyType.Runner
            ? Mathf.Sin(time * 12f) * 0.045f
            : enemy.type == EnemyType.Brute
                ? Mathf.Sin(time * 4.5f) * 0.035f
                : enemy.type == EnemyType.Shooter
                    ? Mathf.Sin(time * 7f) * 0.04f
                    : enemy.type == EnemyType.Dasher
                        ? Mathf.Sin(time * 18f) * 0.055f
                        : enemy.type == EnemyType.Bomber
                            ? Mathf.Sin(time * 5f) * 0.06f
                            : enemy.type == EnemyType.LavaCrawler
                                ? Mathf.Sin(time * 7f) * 0.05f
                                : enemy.type == EnemyType.Phantom
                                    ? Mathf.Sin(time * 6f) * 0.04f
                                    : Mathf.Sin(time * 3.4f) * 0.04f;
        enemy.transform.localScale = enemy.baseScale * (1f + typePulse + enemy.hitPulse * 0.16f + dasherBoost);
        UpdateEnemyMotionVisual(enemy, time);

        if (!enhancedVisuals || enemy.visualCore == null || enemy.visualCoreRenderer == null)
            return;

        var charge = enemy.type == EnemyType.Shooter ? Mathf.Clamp01(1f - enemy.shootCooldown / 2.1f) : 0f;
        var dashCharge = enemy.type == EnemyType.Dasher ? (enemy.isDashing ? 1f : Mathf.Clamp01(1f - enemy.shootCooldown / 2.2f)) : 0f;
        var bomberPulse = enemy.type == EnemyType.Bomber ? Mathf.Sin(time * 8f) * 0.5f + 0.5f : 0f;
        var lavaPulse = enemy.type == EnemyType.LavaCrawler ? Mathf.Sin(time * 6.4f) * 0.5f + 0.5f : 0f;
        var alpha = enemy.type == EnemyType.Shooter ? 0.38f + charge * 0.52f
            : enemy.type == EnemyType.Boss ? 0.68f
            : enemy.type == EnemyType.Dasher ? 0.35f + dashCharge * 0.55f
            : enemy.type == EnemyType.Bomber ? 0.45f + bomberPulse * 0.42f
            : enemy.type == EnemyType.LavaCrawler ? 0.46f + lavaPulse * 0.36f
            : enemy.type == EnemyType.Phantom ? (enemy.isPhasing ? 0.18f : 0.52f)
            : 0.52f;
        var coreScale = enemy.type == EnemyType.Brute ? 0.2f : enemy.type == EnemyType.Boss ? 0.28f
            : enemy.type == EnemyType.Bomber ? 0.18f + bomberPulse * 0.06f
            : enemy.type == EnemyType.LavaCrawler ? 0.17f + lavaPulse * 0.05f : 0.15f;
        coreScale += charge * 0.12f + dashCharge * 0.08f + enemy.hitPulse * 0.08f;
        enemy.visualCore.localScale = new Vector3(coreScale, coreScale, 1f);
        var rotSpeed = enemy.type == EnemyType.Runner ? 220f : enemy.type == EnemyType.Brute ? -72f
            : enemy.type == EnemyType.Shooter ? 130f : enemy.type == EnemyType.Dasher ? (enemy.isDashing ? 620f : 90f)
            : enemy.type == EnemyType.Bomber ? 48f : enemy.type == EnemyType.LavaCrawler ? 64f : enemy.type == EnemyType.Phantom ? -180f : -96f;
        enemy.visualCore.Rotate(0f, 0f, Time.deltaTime * rotSpeed);
        var baseColor = enemy.type == EnemyType.Runner ? new Color(1f, 0.24f, 0.32f)
            : enemy.type == EnemyType.Brute ? new Color(1f, 0.3f, 0.94f)
            : enemy.type == EnemyType.Shooter ? Color.Lerp(new Color(1f, 0.82f, 0.24f), new Color(1f, 0.28f, 0.12f), charge)
            : enemy.type == EnemyType.Dasher ? Color.Lerp(new Color(0.18f, 0.78f, 1f), new Color(0.8f, 1f, 1f), dashCharge)
            : enemy.type == EnemyType.Bomber ? Color.Lerp(new Color(1f, 0.42f, 0.08f), new Color(1f, 0.88f, 0.22f), bomberPulse)
            : enemy.type == EnemyType.LavaCrawler ? Color.Lerp(new Color(1f, 0.28f, 0.05f), new Color(1f, 0.72f, 0.16f), lavaPulse)
            : enemy.type == EnemyType.Phantom ? new Color(0.62f, 0.22f, 1f)
            : new Color(1f, 0.32f, 0.96f);
        enemy.visualCoreRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

        if (enemy.visualArmor != null && enemy.visualArmorRenderer != null)
        {
            enemy.visualArmor.Rotate(0f, 0f, Time.deltaTime * -64f);
            enemy.visualArmor.localScale = Vector3.one * (0.18f + Mathf.Sin(time * 4.2f) * 0.025f + enemy.hitPulse * 0.08f);
            enemy.visualArmorRenderer.color = new Color(1f, 0.36f, 0.92f, 0.62f + enemy.hitPulse * 0.25f);
        }
    }

    SpriteRenderer GetEnemyRenderer(Enemy enemy)
    {
        if (enemy == null)
            return null;
        if (enemy.renderer != null)
            return enemy.renderer;
        return enemy.transform != null ? enemy.transform.GetComponent<SpriteRenderer>() : null;
    }

    void SetEnemyBodyAlpha(Enemy enemy, float alpha)
    {
        var renderer = GetEnemyRenderer(enemy);
        if (renderer == null)
            return;

        var color = renderer.color;
        color.a = Mathf.Clamp01(alpha);
        renderer.color = color;
    }

    void UpdateEnemyMotionVisual(Enemy enemy, float time)
    {
        var body = enemy.visualBody;
        var renderer = GetEnemyRenderer(enemy);
        if (body == null || renderer == null)
            return;

        var dt = Mathf.Max(0.0001f, Time.deltaTime);
        var delta = enemy.transform.position - enemy.lastVisualPosition;
        enemy.lastVisualPosition = enemy.transform.position;
        var velocity = (Vector2)delta / dt;
        enemy.visualVelocity = Vector2.Lerp(enemy.visualVelocity, velocity, 1f - Mathf.Exp(-dt * 12f));
        enemy.attackPulse = Mathf.MoveTowards(enemy.attackPulse, 0f, dt * 4.8f);
        enemy.afterimageTimer -= dt;
        enemy.footstepTimer -= dt;
        var speed01 = Mathf.Clamp01(velocity.magnitude / Mathf.Max(0.25f, enemy.speed * (enemy.type == EnemyType.Dasher ? 2.6f : 1.45f)));

        if (Mathf.Abs(velocity.x) > 0.04f)
            renderer.flipX = velocity.x < 0f;

        if (!enhancedVisuals)
        {
            body.localPosition = Vector3.zero;
            body.localRotation = Quaternion.identity;
            body.localScale = Vector3.one;
            return;
        }

        var bodyBeat = enemy.type == EnemyType.Runner ? 13f
            : enemy.type == EnemyType.Dasher ? 17f
            : enemy.type == EnemyType.Brute ? 5.6f
            : enemy.type == EnemyType.Bomber ? 6.2f
            : enemy.type == EnemyType.LavaCrawler ? 6.8f
            : enemy.type == EnemyType.Boss ? 3.2f
            : 8.2f;
        var step = Mathf.Sin(time * bodyBeat);
        var dashStretch = enemy.type == EnemyType.Dasher && enemy.isDashing ? 0.16f : 0f;
        var bruteWeight = enemy.type == EnemyType.Brute || enemy.type == EnemyType.Boss ? 0.48f : 1f;
        var lean = Mathf.Clamp(-velocity.x * 4.2f * bruteWeight, -12f, 12f);
        var bob = Mathf.Abs(step) * Mathf.Lerp(0.014f, 0.05f, bruteWeight) * speed01;
        var squash = step * 0.045f * speed01;
        var hitKick = Mathf.Clamp01(enemy.hitPulse) * 0.065f;
        var directionOffset = velocity.sqrMagnitude > 0.001f ? -velocity.normalized * (0.026f * speed01) : Vector2.zero;
        var targetDirection = enemy.visualVelocity.sqrMagnitude > 0.001f ? enemy.visualVelocity.normalized : Vector2.down;
        var attackRecoil = -targetDirection * (0.045f * enemy.attackPulse);

        body.localPosition = new Vector3(directionOffset.x + attackRecoil.x, bob + directionOffset.y * 0.3f + attackRecoil.y * 0.45f, 0f);
        body.localRotation = Quaternion.Euler(0f, 0f, lean + step * 2.4f * speed01 * bruteWeight + enemy.attackPulse * 3.5f * Mathf.Sign(velocity.x == 0f ? 1f : velocity.x));
        body.localScale = new Vector3(1f + Mathf.Abs(squash) + dashStretch + hitKick + enemy.attackPulse * 0.07f, 1f - squash * 0.45f - dashStretch * 0.28f + hitKick - enemy.attackPulse * 0.03f, 1f);

        var fastType = enemy.type == EnemyType.Dasher || enemy.type == EnemyType.Phantom || enemy.type == EnemyType.Runner || enemy.type == EnemyType.Boss;
        if (fastType && speed01 > 0.5f && enemy.afterimageTimer <= 0f)
        {
            var ghostColor = enemy.type == EnemyType.Phantom ? new Color(0.72f, 0.22f, 1f, 0.26f)
                : enemy.type == EnemyType.Dasher ? new Color(0.22f, 0.95f, 1f, 0.24f)
                : enemy.type == EnemyType.Boss ? new Color(1f, 0.28f, 0.9f, 0.22f)
                : new Color(1f, 0.22f, 0.28f, 0.14f);
            CreateSpriteGhost(renderer, ghostColor, enemy.type == EnemyType.Boss ? 0.28f : 0.16f, -enemy.visualVelocity.normalized * 0.16f);
            enemy.afterimageTimer = enemy.type == EnemyType.Dasher && enemy.isDashing ? 0.045f : enemy.type == EnemyType.Boss ? 0.18f : 0.12f;
        }

        var heavyStride = enemy.type == EnemyType.Brute || enemy.type == EnemyType.Bomber || enemy.type == EnemyType.LavaCrawler || enemy.type == EnemyType.Boss;
        if (speed01 > 0.28f && enemy.footstepTimer <= 0f && (heavyStride || enemy.type == EnemyType.Runner))
        {
            var strideColor = enemy.type == EnemyType.Brute ? new Color(0.86f, 0.22f, 1f)
                : enemy.type == EnemyType.Bomber ? new Color(1f, 0.56f, 0.08f)
                : enemy.type == EnemyType.LavaCrawler ? new Color(1f, 0.34f, 0.08f)
                : enemy.type == EnemyType.Boss ? new Color(1f, 0.24f, 0.88f)
                : new Color(1f, 0.24f, 0.28f);
            SpawnStrideSparks(enemy.transform.position, enemy.visualVelocity.normalized, strideColor, heavyStride ? 0.75f : 0.45f);
            enemy.footstepTimer = heavyStride ? 0.18f : 0.13f;
        }
    }

    void UpdateBullets()
    {
        for (var i = bullets.Count - 1; i >= 0; i--)
        {
            var bullet = bullets[i];
            bullet.life -= Time.deltaTime;
            bullet.transform.position += (Vector3)(bullet.direction * (bullet.fromEnemy ? 5.3f : bulletSpeed * bullet.speedMultiplier) * Time.deltaTime);
            if (!bullet.fromEnemy && (bullet.style == 3 || bullet.style == 4))
                bullet.transform.Rotate(0, 0, Time.deltaTime * (bullet.style == 4 ? 520f : 260f));
            if (bullet.life <= 0f || ((Vector2)bullet.transform.position).magnitude > ArenaRadius + 2f)
            {
                Destroy(bullet.transform.gameObject);
                bullets.RemoveAt(i);
                continue;
            }

            if (bullet.fromEnemy)
            {
                if (TryReflectEnemyBullet(bullet))
                    continue;

                if (Vector2.Distance(bullet.transform.position, player.position) < 0.42f)
                {
                    DamagePlayer(bullet.damage, "敵弾");
                    Destroy(bullet.transform.gameObject);
                    bullets.RemoveAt(i);
                    CheckLose();
                }
                continue;
            }

            for (var e = enemies.Count - 1; e >= 0; e--)
            {
                if (Vector2.Distance(bullet.transform.position, enemies[e].transform.position) > bullet.hitRadius)
                    continue;

                var hitPosition = enemies[e].transform.position;
                var primary = enemies[e];
                var damage = enemies[e].type == EnemyType.Boss ? bullet.damage * bossDamageMultiplier : bullet.damage;
                DamageEnemy(enemies[e], damage, true);
                if (bullet.splashRadius > 0f)
                    SplashDamage(hitPosition, bullet.splashRadius, bullet.damage * 0.42f, primary);
                TrySpawnChainBolt(bullet, hitPosition, primary);
                bullet.pierce--;
                if (bullet.pierce < 0)
                {
                    Destroy(bullet.transform.gameObject);
                    bullets.RemoveAt(i);
                }
                else
                {
                    bullet.transform.position += (Vector3)(bullet.direction * 0.34f);
                }
                break;
            }
        }
    }

    bool TryReflectEnemyBullet(Bullet bullet)
    {
        if (!(reflectShield || fusionStyle == 3 || formStyle == 3 && evolutionStage >= 3))
            return false;

        var radius = reflectShield || fusionStyle == 3 ? 1.45f : 1.05f;
        var closeToPlayer = Vector2.Distance(bullet.transform.position, player.position) < radius;
        var closeToCore = Vector2.Distance(bullet.transform.position, lantern.position) < radius + 0.2f;
        if (!closeToPlayer && !closeToCore)
            return false;

        var target = FindNearestEnemyFrom(bullet.transform.position, null, 8.5f);
        if (target == null)
            return false;

        var accent = GetFormAccentColor();
        ConvertEnemyBulletToFriendly(bullet, accent, bulletDamage * (fusionStyle == 3 ? 0.72f : 0.52f));
        SpawnSparks(bullet.transform.position, accent, 8, 0.55f);
        lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 0.08f);
        return true;
    }

    void ConvertEnemyBulletToFriendly(Bullet bullet, Color color, float damage)
    {
        var target = FindNearestEnemyFrom(bullet.transform.position, null, 8.5f);
        var direction = target != null
            ? ((Vector2)(target.transform.position - bullet.transform.position)).normalized
            : -bullet.direction;
        bullet.fromEnemy = false;
        bullet.direction = direction;
        bullet.damage = damage;
        bullet.life = 0.85f;
        bullet.pierce = 0;
        bullet.speedMultiplier = 1.45f;
        bullet.hitRadius = 0.42f;
        bullet.splashRadius = 0f;
        bullet.style = 3;
        bullet.chainJumps = 0;
        bullet.transform.rotation = DirectionRotation(direction);
        var renderer = bullet.transform.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.color = color;
        CreateFloatingText("BLOCK", bullet.transform.position + Vector3.up * 0.22f, color, 0.085f);
    }

    void TrySpawnChainBolt(Bullet source, Vector3 hitPosition, Enemy primary)
    {
        if (source.chainJumps <= 0)
            return;

        var target = FindNearestEnemyFrom(hitPosition, primary, 4.8f + source.chainJumps * 0.55f);
        if (target == null)
            return;

        var color = fusionStyle == 2 ? new Color(0.9f, 1f, 0.24f) : new Color(0.25f, 0.94f, 1f);
        var direction = ((Vector2)(target.transform.position - hitPosition)).normalized;
        var go = CreateSpriteObject("Chain Spark", diamondSprite, hitPosition + Vector3.back * 0.04f, color, bulletSize * 0.72f);
        go.transform.rotation = DirectionRotation(direction);
        EnforceBulletCap();
        bullets.Add(new Bullet
        {
            transform = go.transform,
            direction = direction,
            damage = source.damage * source.chainDamageMultiplier,
            life = 0.72f,
            pierce = 0,
            fromEnemy = false,
            speedMultiplier = 1.85f,
            hitRadius = 0.34f,
            style = 1,
            chainJumps = source.chainJumps - 1,
            chainDamageMultiplier = Mathf.Max(0.28f, source.chainDamageMultiplier * 0.78f)
        });
        SpawnSparks(hitPosition, color, 6, 0.46f);
    }

    Enemy FindNearestEnemyFrom(Vector3 position, Enemy ignore, float maxDistance)
    {
        Enemy best = null;
        var bestDistance = maxDistance * maxDistance;
        foreach (var enemy in enemies)
        {
            if (enemy == ignore)
                continue;

            var distance = ((Vector2)(enemy.transform.position - position)).sqrMagnitude;
            if (distance < bestDistance)
            {
                best = enemy;
                bestDistance = distance;
            }
        }
        return best;
    }

    // ── Recursion guard ─────────────────────────────────────────────
    // SplashDamage →DamageEnemy →(shockwaveOnHit) →SplashDamage …
    // and Bomber death →SplashDamage →DamageEnemy →KillEnemy →(next Bomber's
    // SplashDamage) can cascade exponentially when enemies cluster, blowing the
    // C# stack with StackOverflowException at Transform.get_position in late waves.
    // Cap the recursive depth so cascades terminate gracefully.
    int splashDamageDepth;
    const int MaxSplashDepth = 3;

    void SplashDamage(Vector3 position, float radius, float damage, Enemy primary)
    {
        if (splashDamageDepth >= MaxSplashDepth) return;
        splashDamageDepth++;
        try
        {
            SpawnSparks(position, GetBulletColor(), 8, 0.62f);
            for (var i = enemies.Count - 1; i >= 0; i--)
            {
                if (i >= enemies.Count) continue; // list may shrink mid-loop via kills
                var enemy = enemies[i];
                if (enemy == primary || enemy == null || enemy.transform == null) continue;
                if (Vector2.Distance(enemy.transform.position, position) > radius) continue;
                DamageEnemy(enemy, enemy.type == EnemyType.Boss ? damage * bossDamageMultiplier : damage, false);
            }
        }
        finally
        {
            splashDamageDepth--;
        }
    }

    // 死因記録用 overload: 呼び出し�Eで source を渡せ�E lastDamageSource に保存される
    void DamagePlayer(float amount, string source)
    {
        if (!string.IsNullOrEmpty(source)) lastDamageSource = source;
        DamagePlayer(amount);
    }

    void DamagePlayer(float amount)
    {
        if (amount <= 0f || gameOver || victory)
            return;

        // Last Stand 無敵時間中はノ�Eダメ
        if (lastStandInvulnTimer > 0f)
            return;

        // アクセシビリチE��補正: 入力ダメージめE0.5、E.5x に減衰/増加
        // 触ダメ / 弾ダメ / lava ダメ 全てに一律適用 (player intake で一允E��)
        amount *= accessibilityEnemyDmgMul;
        if (amount <= 0f) return;

        playerHp -= amount;
        if (playerHp <= 0f && deathShieldActive)
        {
            // D: DeathShield revive amount 1.0 →0.5 (half HP, less safety)
            playerHp = 0.5f;
            deathShieldActive = false;
            Flash(new Color(1f, 0.92f, 0.22f, 0.65f), 0.5f);
            Shake(0.4f, 0.18f);
            SpawnRingSparks(player.position, new Color(1f, 0.88f, 0.28f), 28, 2.0f);
            CreateFloatingText("DEATHLESS", player.position + Vector3.up * 1.0f, new Color(1f, 0.92f, 0.28f), 0.19f);
            PlaySfx("Evolve", 440f, 0.25f, 0.35f);
        }
        // Last Stand 発勁E(ラン1囁E: deathShield と独立、両方使え�E 2回救済される
        else if (playerHp <= 0f && !lastStandUsed)
        {
            lastStandUsed = true;
            playerHp = LastStandReviveHp;
            lastStandInvulnTimer = LastStandInvulnSeconds;
            Flash(new Color(1f, 0.32f, 0.18f, 0.72f), 0.85f);
            Shake(0.55f, 0.30f);
            SpawnRingSparks(player.position, new Color(1f, 0.45f, 0.15f), 40, 2.8f);
            SpawnRingSparks(player.position, new Color(1f, 0.72f, 0.22f), 28, 3.8f);
            SpawnSparks(player.position, new Color(1f, 0.72f, 0.22f), 36, 1.8f);
            CreateFloatingText("LAST STAND", player.position + Vector3.up * 1.2f, new Color(1f, 0.55f, 0.18f), 0.24f);
            // 中央に大きく「LAST STAND」表示で「救済された瞬間」を強調
            ShowMessage("LAST STAND");
            AddEventLog("LAST STAND: 致死救済(1HP + 無敵)");
            HitFreeze(0.30f);
            PlaySfx("Evolve", 180f, 0.40f, 0.55f);
            PlaySfx("Hit", 320f, 0.18f, 0.32f);
        }
        playerHitPulse = Mathf.Max(playerHitPulse, amount >= 0.45f ? 0.5f : 0.22f);
        if (amount >= 0.18f && showDamageNumbers)
            CreateFloatingText("HP -" + Mathf.CeilToInt(amount), player.position + Vector3.up * 0.78f, new Color(1f, 0.32f, 0.24f), 0.095f);
        if (amount >= 0.45f)
        {
            Flash(new Color(1f, 0.08f, 0.06f, 0.2f), 0.2f);
            Shake(0.12f, 0.06f);
        }
        // ── 死因確宁E ここで HP ぁE0 になり救済も使ぁE�Eったらリザルト用に記録 ──
        if (playerHp <= 0f && string.IsNullOrEmpty(runDeathCause))
            runDeathCause = string.IsNullOrEmpty(lastDamageSource) ? "不明" : lastDamageSource;
    }

    void DamageLantern(float amount)
    {
        if (amount <= 0f || gameOver || victory)
            return;

        lanternHp -= amount;
        eggHitPulse = Mathf.Max(eggHitPulse, amount >= 0.45f ? 0.65f : 0.28f);
        if (amount >= 0.18f && showDamageNumbers)
            CreateFloatingText("CORE -" + Mathf.CeilToInt(amount), lantern.position + Vector3.up * 0.92f, new Color(1f, 0.78f, 0.22f), 0.095f);
        if (amount >= 0.45f)
        {
            Flash(new Color(1f, 0.56f, 0.08f, 0.22f), 0.24f);
            Shake(0.16f, 0.08f);
        }
    }

    void DamageEnemy(Enemy enemy, float damage, bool allowExplosion)
    {
        // Phantom is immune while phasing
        if (enemy.isPhasing)
        {
            SpawnSparks(enemy.transform.position, new Color(0.6f, 0.18f, 1f), 3, 0.3f);
            return;
        }

        // Critical hit
        var isCrit = critChance > 0f && rng.NextDouble() < critChance;
        if (isCrit) damage *= 2.5f;

        // エッグレゾナンス: コアHP満タン時�E +20% dmg (Sage Hare 専用シグネチャ)
        if (eggResonanceActive && lanternHp >= lanternMaxHp - 0.01f)
            damage *= 1.20f;

        // Solar Anchor: コア近接晁E(3m以冁E は全dmg +25%
        if (coreBondNearActive)
            damage *= CoreBondNearBonus;

        enemy.hp -= damage;
        runDamageDealt += Mathf.Max(0f, damage);

        var bossHit = enemy.type == EnemyType.Boss;
        var now = Time.unscaledTime;
        if (isCrit && (!bossHit || now - lastBossHitFreezeAt > 0.12f))
        {
            HitFreeze(0.035f);
            if (bossHit) lastBossHitFreezeAt = now;
        }
        if (bossHit && damage >= 4f && now - lastBossHitFreezeAt > 0.16f)
        {
            HitFreeze(0.035f);
            lastBossHitFreezeAt = now;
        }

        // Shockwave on every hit
        if (shockwaveOnHit && enemy.hp > 0f)
            SplashDamage(enemy.transform.position, 0.7f, damage * 0.28f, enemy);

        // Lifesteal
        if (bulletLifesteal)
            playerHp = Mathf.Min(playerMaxHp, playerHp + lifestealAmount);
        enemy.hitPulse = Mathf.Max(enemy.hitPulse, damage >= 1f ? 1f : 0.55f);
        UpdateEnemyHpBar(enemy);
        var hitColor = enemy.type == EnemyType.Runner ? new Color(1f, 0.32f, 0.32f)
            : enemy.type == EnemyType.Brute ? new Color(0.82f, 0.22f, 1f)
            : enemy.type == EnemyType.Shooter ? new Color(1f, 0.92f, 0.28f)
            : enemy.type == EnemyType.Dasher ? new Color(0.22f, 0.92f, 1f)
            : enemy.type == EnemyType.Bomber ? new Color(1f, 0.58f, 0.12f)
            : enemy.type == EnemyType.LavaCrawler ? new Color(1f, 0.36f, 0.08f)
            : enemy.type == EnemyType.Phantom ? new Color(0.62f, 0.22f, 1f)
            : new Color(1f, 0.28f, 0.9f);
        var showHitFx = !bossHit || isCrit || damage >= 3f || now - lastBossHitFxAt > 0.075f;
        if (showHitFx)
        {
            if (bossHit) lastBossHitFxAt = now;
            SpawnSparks(enemy.transform.position, hitColor, bossHit ? 2 : 5, bossHit ? 0.35f : 0.5f);
            PlaySfx("Hit", bossHit ? 150f : 420f, 0.035f, 0.12f);
            if (damage >= 0.65f && (!bossHit || isCrit || damage >= 2f))
            {
                var textColor = isCrit ? new Color(1f, 0.92f, 0.18f) : bossHit ? new Color(1f, 0.42f, 0.92f) : new Color(0.66f, 1f, 1f);
                var textSize = isCrit ? 0.145f : bossHit ? 0.13f : 0.105f;
                CreateFloatingText((isCrit ? "CRIT " : "") + Mathf.CeilToInt(damage), enemy.transform.position + Vector3.up * 0.42f, textColor, textSize);
            }
        }
        if (enemy.hp > 0f)
            return;

        var index = enemies.IndexOf(enemy);
        if (index >= 0)
            enemies.RemoveAt(index);
        KillEnemy(enemy, true, allowExplosion);
    }

    void KillEnemy(Enemy enemy, bool reward, bool allowExplosion = false)
    {
        var killColor = enemy.type == EnemyType.Boss ? new Color(1f, 0.2f, 0.9f)
            : enemy.type == EnemyType.Brute ? new Color(0.78f, 0.18f, 1f)
            : enemy.type == EnemyType.Bomber ? new Color(1f, 0.58f, 0.08f)
            : enemy.type == EnemyType.LavaCrawler ? new Color(1f, 0.36f, 0.08f)
            : enemy.type == EnemyType.Dasher ? new Color(0.18f, 0.92f, 1f)
            : enemy.type == EnemyType.Phantom ? new Color(0.55f, 0.12f, 1f)
            : new Color(0.2f, 1f, 0.75f);
        var killCount = enemy.type == EnemyType.Boss ? 34 : enemy.type == EnemyType.Brute ? 20 : (enemy.type == EnemyType.Bomber || enemy.type == EnemyType.LavaCrawler) ? 18 : 10;
        var killForce = enemy.type == EnemyType.Boss ? 1.8f : enemy.type == EnemyType.Brute ? 1.25f : (enemy.type == EnemyType.Bomber || enemy.type == EnemyType.LavaCrawler) ? 1.4f : 0.9f;
        SpawnSparks(enemy.transform.position, killColor, killCount, killForce);
        PlaySfx("Kill", enemy.type == EnemyType.Boss ? 74f : enemy.type == EnemyType.Brute ? 160f : 260f,
            enemy.type == EnemyType.Boss ? 0.42f : enemy.type == EnemyType.Brute ? 0.14f : enemy.type == EnemyType.LavaCrawler ? 0.11f : 0.08f,
            enemy.type == EnemyType.Boss ? 0.65f : enemy.type == EnemyType.Brute ? 0.32f : enemy.type == EnemyType.LavaCrawler ? 0.28f : 0.22f);
        if (enemy.type == EnemyType.Boss || enemy.type == EnemyType.Brute)
            CreateFloatingText("BREAK", enemy.transform.position + Vector3.up * 0.54f, enemy.type == EnemyType.Boss ? new Color(1f, 0.82f, 0.28f) : new Color(0.38f, 1f, 0.78f), 0.13f);
        if (enemy.type == EnemyType.Boss)
        {
            // ── プロ改喁E ラスボス撃破は最も派手なクライマックスにする ──
            // 中ボス (Pulswyrm) は控えめ、ラスボス (Nullwyrm) は二重リング + 強ぁE��ェイク
            var isFinal = !enemy.isMidBoss;
            Flash(new Color(0.35f, 1f, 0.8f, isFinal ? 0.55f : 0.42f), isFinal ? 1.1f : 0.8f);
            Shake(isFinal ? 0.85f : 0.6f, isFinal ? 0.32f : 0.22f);
            HitFreeze(isFinal ? 0.22f : 0.15f);  // long freeze on boss kill  Epeak impact
            // ラスボスのみ: 多重 ring sparks で勝利を強調
            if (isFinal)
            {
                var bossPos = enemy.transform.position;
                SpawnRingSparks(bossPos, new Color(0.6f, 1f, 0.92f), 64, 3.2f);
                SpawnRingSparks(bossPos, new Color(1f, 0.82f, 0.28f), 48, 2.2f);
                SpawnRingSparks(bossPos, new Color(1f, 0.42f, 0.96f), 36, 1.4f);
                SpawnSparks(bossPos, new Color(1f, 0.92f, 0.55f), 80, 3.6f);
                PlaySfx("Boss", 55f, 0.55f, 0.85f);  // 追加の重低音 SFX
            }
            // Boss victory cutscene (mid vs final は isMidBoss フラグで判宁E
            BeginBossVictoryCutscene(enemy.isMidBoss);
        }
        else if (enemy.type == EnemyType.Brute)
        {
            Flash(new Color(0.52f, 0.08f, 0.92f, 0.14f), 0.2f);
            Shake(0.06f, 0.04f);
            HitFreeze(0.05f);  // small freeze on brute kill
        }
        else if (enemy.type == EnemyType.Bomber)
        {
            // ── Bomber death explosion ──────────────────────────────
            var bombPos = enemy.transform.position;
            Flash(new Color(1f, 0.52f, 0.08f, 0.22f), 0.3f);
            Shake(0.15f, 0.08f);
            SpawnSparks(bombPos, new Color(1f, 0.72f, 0.14f), 22, 1.6f);
            SpawnRingSparks(bombPos, new Color(1f, 0.45f, 0.06f), 20, 1.1f);
            CreateFloatingText("BOOM!", bombPos + Vector3.up * 0.6f, new Color(1f, 0.82f, 0.18f), 0.16f);
            PlaySfx("Kill", 110f, 0.25f, 0.48f);
            SplashDamage(bombPos, 1.65f, 2.0f + bulletDamage * 0.25f, enemy);
            if (Vector2.Distance(bombPos, player.position) < 1.65f)
                DamagePlayer(1.8f, "Bomber 爆発");
            if (Vector2.Distance(bombPos, lantern.position) < 1.65f)
                DamageLantern(2.0f);
        }

        if (reward)
        {
            enemiesKilledThisWave++;
            runEnemiesKilled++;
            // ── Kill Streak 計箁E──
            // 0.85 秒以冁E��追加キル →ストリーク延長、E��ぎためE1 にリセチE��
            var now = Time.time;
            if (now - lastKillTime <= KillStreakWindow)
                killStreak++;
            else
                killStreak = 1;
            lastKillTime = now;
            if (killStreak > runMaxKillStreak) runMaxKillStreak = killStreak;
            // マイルスト�EンでフローチE��ングチE��スチE+ 軽ぁESFX
            if (killStreak == 5 || killStreak == 10 || killStreak == 20 || killStreak == 35 || killStreak == 50 || (killStreak > 50 && killStreak % 25 == 0))
            {
                var streakColor = killStreak >= 50 ? new Color(1f, 0.45f, 0.95f)
                    : killStreak >= 20 ? new Color(1f, 0.78f, 0.22f)
                    : killStreak >= 10 ? new Color(1f, 0.92f, 0.42f)
                    : new Color(0.6f, 1f, 0.85f);
                CreateFloatingText(killStreak + " STREAK!", player.position + Vector3.up * 2.3f, streakColor, 0.18f);
                PlaySfx("Pickup", killStreak >= 20 ? 940f : 720f, 0.05f, 0.12f);
            }
            if (enemy.type == EnemyType.Boss)
            {
                runBossKilled++;
                // Clear boss reference + hide boss bar on death (mid-boss or final)
                if (enemy == bossEnemy)
                {
                    bossEnemy = null;
                    if (bossBarRoot != null) bossBarRoot.SetActive(false);
                }
            }
            var baseDrop = enemy.type == EnemyType.Boss ? 14f : enemy.type == EnemyType.Brute ? 3f : enemy.type == EnemyType.Bomber ? 2f : 1f;
            // Elite 撃破は ×.5 (リスク報酬)
            SpawnPickup(enemy.transform.position, enemy.isElite ? baseDrop * 2.5f : baseDrop);
            if (enemy.isElite)
            {
                SpawnSparks(enemy.transform.position, new Color(1f, 0.86f, 0.28f), 18, 1.1f);
                SpawnRingSparks(enemy.transform.position, new Color(1f, 0.78f, 0.18f), 14, 1.0f);
                CreateFloatingText("ELITE +", enemy.transform.position + Vector3.up * 0.62f, new Color(1f, 0.86f, 0.28f), 0.13f);
            }

            // Wave burst trigger
            if (waveKillActive)
            {
                waveKillCounter++;
                if (waveKillCounter % 8 == 0)
                {
                    for (var wb = 0; wb < 8; wb++)
                    {
                        var wbDir = new Vector2(Mathf.Cos(Mathf.PI * 2f * wb / 8f), Mathf.Sin(Mathf.PI * 2f * wb / 8f));
                        var wbGo = CreateSpriteObject("Wave Bolt", diamondSprite, player.position, GetFormAccentColor(), 0.14f);
                        wbGo.transform.rotation = DirectionRotationRight(wbDir);
                        EnforceBulletCap();
                        bullets.Add(new Bullet { transform = wbGo.transform, direction = wbDir, damage = bulletDamage * 0.6f, life = 0.8f, pierce = bulletPierce, fromEnemy = false, speedMultiplier = 0.85f, hitRadius = 0.36f });
                    }
                    SpawnSparks(player.position, GetFormAccentColor(), 12, 1.0f);
                }
            }

            // Bonus data on kill
            if (bonusDataOnKill > 0f)
                dataChips += bonusDataOnKill * (enemy.type == EnemyType.Boss ? 5f : enemy.type == EnemyType.Brute ? 2f : enemy.type == EnemyType.Bomber ? 1.5f : 1f);

            // ヴァンパイアコア: kill-bonus heal (only if base lifesteal is unlocked)
            if (bulletLifesteal && lifestealKillBonus > 0f)
            {
                var healAmt = lifestealKillBonus * (enemy.type == EnemyType.Boss ? 4f : enemy.type == EnemyType.Brute ? 2f : 1f);
                playerHp = Mathf.Min(playerMaxHp, playerHp + healAmt);
                CreateFloatingText("+" + healAmt.ToString("F1") + " HP", player.position + Vector3.up * 0.55f, new Color(0.8f, 1f, 0.55f), 0.12f);
            }
            if (allowExplosion && explodeChance > 0f && rng.NextDouble() < explodeChance)
            {
                for (var i = enemies.Count - 1; i >= 0; i--)
                {
                    if (Vector2.Distance(enemies[i].transform.position, enemy.transform.position) < 1.55f)
                        DamageEnemy(enemies[i], 1.4f + bulletDamage * 0.55f, false);
                }
            }
        }
        Destroy(enemy.transform.gameObject);
    }

    void SpawnPickup(Vector3 position, float value)
    {
        var go = CreateSpriteObject("Data Chip", dataChipSprite != null ? dataChipSprite : circleSprite, position, dataChipSprite != null ? Color.white : new Color(0.3f, 1f, 0.72f), dataChipSprite != null ? 0.28f : 0.14f);
        if (dataChipSprite != null)
            go.transform.rotation = Quaternion.Euler(0f, 0f, rng.Next(0, 4) * 90f);
        EnforcePickupCap();
        pickups.Add(new Pickup { transform = go.transform, value = value });
    }

    void UpdatePickups()
    {
        if (vacuumSurgeTimer > 0f)
            vacuumSurgeTimer -= Time.deltaTime;

        for (var i = pickups.Count - 1; i >= 0; i--)
        {
            var pickup = pickups[i];
            var distance = Vector2.Distance(pickup.transform.position, player.position);
            if (vacuumSurgeTimer > 0f)
            {
                var dir = ((Vector2)(player.position - pickup.transform.position)).normalized;
                pickup.transform.position += (Vector3)(dir * Time.deltaTime * 32f);
            }
            else if (distance < pickupRange)
            {
                var direction = ((Vector2)(player.position - pickup.transform.position)).normalized;
                pickup.transform.position += (Vector3)(direction * Time.deltaTime * Mathf.Lerp(2f, 10f, 1f - distance / pickupRange));
            }

            if (distance < 0.28f)
                CollectPickupAt(i, 1f);
        }
    }

    void CollectPickupAt(int index, float valueMultiplier)
    {
        if (index < 0 || index >= pickups.Count)
            return;

        var pickup = pickups[index];
        var gained = pickup.value * valueMultiplier;
        xp += gained;
        dataChips += pickup.value * dataMultiplier * valueMultiplier;
        if (pickupHealChance > 0f && rng.NextDouble() < pickupHealChance)
            playerHp = Mathf.Min(playerMaxHp, playerHp + 0.7f);
        SpawnSparks(pickup.transform.position, new Color(0.3f, 1f, 0.72f), 5, 0.42f);
        CreateFloatingText("DATA +" + Mathf.CeilToInt(gained), pickup.transform.position + Vector3.up * 0.24f, new Color(0.42f, 1f, 0.68f), 0.08f);
        PlaySfx("Pickup", 660f, 0.045f, 0.16f);
        Destroy(pickup.transform.gameObject);
        pickups.RemoveAt(index);
        CheckLevelUp();
        UpdateUi();
    }

    void CheckLevelUp()
    {
        while (xp >= xpToLevel)
        {
            xp -= xpToLevel;
            level++;
            xpToLevel = Mathf.Round(xpToLevel * 1.28f + 1.6f);
            vacuumSurgeTimer = 1.8f;
            Flash(new Color(0.72f, 1f, 0.48f, 0.10f), 0.25f);
            SpawnRingSparks(player.position, new Color(0.78f, 1f, 0.4f), 16, 1.6f);
            AddEventLog("LEVEL UP: Lv " + level);
            CreateFloatingText("LEVEL UP", player.position + Vector3.up * 1.05f, new Color(1f, 0.9f, 0.32f), 0.16f);
            if (ShouldEvolve())
                OpenEvolution();
            else
                OpenUpgrade();
            if (choosingUpgrade)
                break;
        }
    }

    // Hard cap to prevent runaway spark accumulation during heavy combat
    const int MaxSparks = 280;

    // ── Trim oldest entries from a tracked list when over cap, destroying their GameObjects.
    // Used by Add* helpers below so we never exceed limits even when many spawns happen at once.
    // NOTE: bullet cap uses SOFT removal (sets life to expire) so we don't corrupt
    // the UpdateBullets iteration when KillEnemy →wave burst →Add chain hits cap.
    // The marked bullets get cleaned up by UpdateBullets' normal life<=0 path next frame.
    void EnforceBulletCap()
    {
        if (bullets.Count < MaxBullets) return;
        var overflow = bullets.Count - MaxBullets + 1;
        for (var k = 0; k < overflow && k < bullets.Count; k++)
        {
            if (bullets[k].life > 0f) bullets[k].life = -1f;
        }
    }

    void EnforcePickupCap()
    {
        while (pickups.Count >= MaxPickups)
        {
            var oldest = pickups[0];
            if (oldest.transform != null) Destroy(oldest.transform.gameObject);
            pickups.RemoveAt(0);
        }
    }

    void EnforceSpriteGhostCap()
    {
        while (spriteGhosts.Count >= MaxSpriteGhosts)
        {
            var oldest = spriteGhosts[0];
            if (oldest.transform != null) Destroy(oldest.transform.gameObject);
            spriteGhosts.RemoveAt(0);
        }
    }

    void SpawnSparks(Vector3 position, Color color, int count, float force)
    {
        // Skip if we're already at the cap (keeps frame rate stable in chaos)
        if (sparks.Count >= MaxSparks) return;
        if (bossEnemy != null && bossEnemy.transform != null && wave >= MaxWave)
            count = Mathf.Min(count, enhancedVisuals ? 6 : 3);
        else if (sparks.Count > MaxSparks * 0.7f)
            count = Mathf.Min(count, 4);
        // Trim requested count so we don't overshoot the cap
        count = Mathf.Min(count, MaxSparks - sparks.Count);
        for (var i = 0; i < count; i++)
        {
            var angle = (float)rng.NextDouble() * Mathf.PI * 2f;
            var speed = force * Mathf.Lerp(0.55f, 1.35f, (float)rng.NextDouble());
            var sparkLife = Mathf.Lerp(0.22f, 0.68f, (float)rng.NextDouble());
            var go = CreateSpriteObject("Data Spark", squareSprite, position + Vector3.back * 0.05f, color, Mathf.Lerp(0.06f, 0.14f, (float)rng.NextDouble()));
            sparks.Add(new Spark
            {
                transform = go.transform,
                velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed,
                life = sparkLife,
                maxLife = sparkLife
            });
        }
    }

    void SpawnAttackFlash(Vector3 origin, Vector2 direction, Color color, float scale, bool generatedBurst)
    {
        if (!enhancedVisuals)
            return;

        if (direction.sqrMagnitude < 0.001f)
            direction = Vector2.up;
        direction.Normalize();

        var flashPosition = origin + (Vector3)(direction * (0.38f + scale * 0.18f)) + Vector3.back * 0.08f;
        var flash = CreateSpriteObject("Attack Flash", generatedBurst ? diamondSprite : circleSprite, flashPosition, WithAlpha(Color.Lerp(Color.white, color, 0.45f), 0.62f), 0.16f + scale * 0.18f);
        flash.transform.rotation = generatedBurst ? DirectionRotationRight(direction) : Quaternion.identity;
        sparks.Add(new Spark
        {
            transform = flash.transform,
            velocity = direction * (0.35f + scale * 0.2f),
            life = 0.11f,
            maxLife = 0.11f
        });

        var side = new Vector2(-direction.y, direction.x);
        for (var i = 0; i < (generatedBurst ? 5 : 3); i++)
        {
            var spread = Mathf.Lerp(-0.55f, 0.55f, i / Mathf.Max(1f, (generatedBurst ? 4f : 2f)));
            var sparkDir = (direction + side * spread).normalized;
            var go = CreateSpriteObject("Muzzle Spark", squareSprite, flashPosition, WithAlpha(color, 0.72f), Mathf.Lerp(0.045f, 0.085f, (float)rng.NextDouble()));
            sparks.Add(new Spark
            {
                transform = go.transform,
                velocity = sparkDir * Mathf.Lerp(0.8f, 1.45f, (float)rng.NextDouble()) * scale,
                life = Mathf.Lerp(0.12f, 0.2f, (float)rng.NextDouble()),
                maxLife = 0.2f
            });
        }
    }

    void SpawnStrideSparks(Vector3 position, Vector2 direction, Color color, float force)
    {
        if (!enhancedVisuals)
            return;

        if (direction.sqrMagnitude < 0.001f)
            direction = Vector2.down;
        direction.Normalize();
        var side = new Vector2(-direction.y, direction.x);
        for (var i = 0; i < 2; i++)
        {
            var sideSign = i == 0 ? -1f : 1f;
            var footPosition = position - (Vector3)(direction * 0.28f) + (Vector3)(side * sideSign * 0.12f) + Vector3.forward * 0.02f;
            var go = CreateSpriteObject("Stride Spark", squareSprite, footPosition, WithAlpha(color, 0.42f), Mathf.Lerp(0.045f, 0.075f, (float)rng.NextDouble()));
            sparks.Add(new Spark
            {
                transform = go.transform,
                velocity = (-direction * 0.45f + side * sideSign * 0.18f) * force,
                life = Mathf.Lerp(0.12f, 0.22f, (float)rng.NextDouble()),
                maxLife = 0.22f
            });
        }
    }

    void CreateSpriteGhost(SpriteRenderer source, Color color, float life, Vector2 velocity)
    {
        if (!enhancedVisuals || source == null || source.sprite == null || spriteGhosts.Count >= MaxSpriteGhosts)
            return;

        var ghostGo = new GameObject("Sprite Afterimage");
        ghostGo.transform.position = source.transform.position + Vector3.forward * 0.04f;
        ghostGo.transform.rotation = source.transform.rotation;
        ghostGo.transform.localScale = source.transform.lossyScale;

        var renderer = ghostGo.AddComponent<SpriteRenderer>();
        renderer.sprite = source.sprite;
        renderer.flipX = source.flipX;
        renderer.flipY = source.flipY;
        renderer.color = color;
        renderer.sortingOrder = source.sortingOrder - 1;

        EnforceSpriteGhostCap();
        spriteGhosts.Add(new SpriteGhost
        {
            transform = ghostGo.transform,
            renderer = renderer,
            baseColor = color,
            baseScale = ghostGo.transform.localScale,
            velocity = velocity,
            life = life,
            maxLife = life
        });
    }

    void UpdateSparks()
    {
        for (var i = sparks.Count - 1; i >= 0; i--)
        {
            var spark = sparks[i];
            spark.life -= Time.deltaTime;
            spark.velocity *= 1f - Time.deltaTime * 2.4f;
            spark.transform.position += (Vector3)(spark.velocity * Time.deltaTime);
            spark.transform.Rotate(0, 0, 360f * Time.deltaTime);
            var renderer = spark.transform.GetComponent<SpriteRenderer>();
            var c = renderer.color;
            c.a = Mathf.Clamp01(Mathf.Sqrt(spark.life / spark.maxLife));
            renderer.color = c;

            if (spark.life <= 0f)
            {
                Destroy(spark.transform.gameObject);
                sparks.RemoveAt(i);
            }
        }
    }

    void UpdateSpriteGhosts()
    {
        for (var i = spriteGhosts.Count - 1; i >= 0; i--)
        {
            var ghost = spriteGhosts[i];
            ghost.life -= Time.deltaTime;
            ghost.velocity *= 1f - Time.deltaTime * 3.2f;
            ghost.transform.position += (Vector3)(ghost.velocity * Time.deltaTime);
            var ratio = Mathf.Clamp01(ghost.life / Mathf.Max(0.001f, ghost.maxLife));
            ghost.transform.localScale = ghost.baseScale * (0.82f + ratio * 0.18f);
            if (ghost.renderer != null)
                ghost.renderer.color = WithAlpha(ghost.baseColor, ghost.baseColor.a * ratio * ratio);

            if (ghost.life <= 0f)
            {
                Destroy(ghost.transform.gameObject);
                spriteGhosts.RemoveAt(i);
            }
        }
    }

    void CreateFloatingText(string text, Vector3 position, Color color, float size)
    {
        // Hard cap to prevent runaway accumulation in heavy combat
        if (floatingTexts.Count >= MaxFloatingTexts) return;

        // NOTE: TextMesh has [RequireComponent(MeshRenderer)] so we MUST NOT pass
        // typeof(MeshRenderer) here  Edoing so spams "Can't add component" warnings
        // every frame, which clogs the Console and freezes the Editor.
        var go = new GameObject("Floating Text", typeof(TextMesh));
        go.transform.position = position + Vector3.back * 0.35f;
        var mesh = go.GetComponent<TextMesh>();
        mesh.text = text;
        mesh.font = GetUiFont();
        mesh.fontSize = 42;
        mesh.characterSize = size;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = color;
        var renderer = go.GetComponent<MeshRenderer>();
        renderer.sortingOrder = 95;
        if (mesh.font != null)
            renderer.material = mesh.font.material;
        floatingTexts.Add(new FloatingText
        {
            transform = go.transform,
            mesh = mesh,
            velocity = new Vector2(0f, 0.62f),
            life = 0.78f,
            maxLife = 0.78f,
            baseScale = Vector3.one
        });
    }

    void UpdateFloatingTexts()
    {
        for (var i = floatingTexts.Count - 1; i >= 0; i--)
        {
            var item = floatingTexts[i];
            item.life -= Time.deltaTime;
            item.transform.position += (Vector3)(item.velocity * Time.deltaTime);
            item.velocity *= 1f - Time.deltaTime * 1.6f;
            var t = Mathf.Clamp01(item.life / Mathf.Max(0.01f, item.maxLife));
            item.transform.localScale = item.baseScale * Mathf.Lerp(1.18f, 0.92f, t);
            var c = item.mesh.color;
            c.a = Mathf.SmoothStep(0f, 1f, t);
            item.mesh.color = c;

            if (item.life > 0f)
                continue;

            Destroy(item.transform.gameObject);
            floatingTexts.RemoveAt(i);
        }
    }

    void UpdateWave()
    {
        waveTimer += Time.deltaTime;
        if (enemiesToSpawn > 0 || enemies.Count > 0)
            return;
        if (choosingRelic)
            return;

        if (wave >= MaxWave)
        {
            // Final boss reward module removed by user request  Ego straight to victory
            victory = true;
            SaveProgress(true);
            ShowResult(true);
            return;
        }

        if (enemiesKilledThisWave > runMaxWaveKills)
        {
            runMaxWaveKills = enemiesKilledThisWave;
            runMaxKillWave = wave;
        }

        // Relic milestones at wave 3 and 6
        if ((wave == 3 || wave == 6) && BuildRelicPool().Count > 0)
        {
            OpenRelicSelect();
            return;
        }
        // Data Lab milestones at wave 4 and 8 (Brotato shop 風、�E回�E data 蓁E��を征E��て Wave 4)
        if (ShouldOpenDataLab())
        {
            OpenDataLab();
            return;
        }
        BeginWave(wave + 1);
        OpenUpgrade();
    }

    void BeginWave(int nextWave)
    {
        // ── プロ改喁E Wave 開始時に既孁Epickup を�E自動回叁E(Brotato 流�E取り送E��対筁E ──
        if (pickups.Count > 0)
        {
            vacuumSurgeTimer = 2.5f;
            CreateFloatingText("✦ AUTO COLLECT", player.position + Vector3.up * 2.2f, new Color(0.55f, 1f, 0.85f), 0.20f);
        }

        wave = nextWave;
        waveTimer = 0f;
        enemiesKilledThisWave = 0;
        bossEnemy = null;
        // Wave 刁E��替え時に kill streak をリセチE�� (連戦の途�Eれ判宁E
        killStreak = 0;
        lastKillTime = 0f;
        waveTrait = GetWaveTrait(wave);
        // ── Trait-specific setup ──────────────────────────────────────
        waveTouchDamageMult = waveTrait == "Berserk" ? 1.2f : 1f;
        var lateExtra = wave >= 7 ? (wave - 6) * 8 : 0;
        var extraSpawn = wave == 7 ? 3 : waveTrait == "Iron Skin" ? 2 : waveTrait == "Berserk" ? 3 : 0;
        // Wave 5: mid-boss arrives, so reduce regular spawn count heavily to keep pressure manageable.
        var midBossWave = wave == 5;
        enemiesToSpawn = wave == MaxWave ? 0 : 7 + wave * 3 + extraSpawn + lateExtra;
        var spawnMult = wave <= 2 ? 0.9f : wave == 3 ? 1.0f : 1.05f;
        if (wave != MaxWave) enemiesToSpawn = Mathf.RoundToInt(enemiesToSpawn * spawnMult);
        if (midBossWave) enemiesToSpawn = Mathf.RoundToInt(enemiesToSpawn * 0.55f);
        // Danger Level: 敵数も増やぁE(D5 で +50%)
        if (wave != MaxWave) enemiesToSpawn = Mathf.RoundToInt(enemiesToSpawn * GetDangerSpawnMultiplier());
        enemiesTotalThisWave = wave == MaxWave ? 1 : (midBossWave ? enemiesToSpawn + 1 : enemiesToSpawn);
        spawnTimer = 0.5f;
        lightRadius = Mathf.Max(3.2f, lightRadius - 0.12f);
        // Dark Field: shrink lantern glow for this wave only
        var glowScale = waveTrait == "Dark Field" ? lightRadius * 1.75f * 0.76f : lightRadius * 1.75f;
        lantern.GetChild(0).localScale = Vector3.one * glowScale;
        bossBarRoot.SetActive(false);
        var dangerSuffix = dangerLevel > 0 ? "  [D" + dangerLevel + "]" : "";
        var displayTrait = GetStageDisplayTrait(waveTrait);
        var traitHint = GetWaveTraitHint(waveTrait);
        var traitSuffix = string.IsNullOrEmpty(traitHint) ? "" : "\n" + traitHint;
        ShowMessage("侵食" + wave + " / " + displayTrait + dangerSuffix + traitSuffix);
        AddEventLog("WAVE " + wave + ": " + displayTrait + dangerSuffix);
        // Trait-specific flash effects
        if (waveTrait == "Berserk")   { Flash(new Color(1f, 0.12f, 0.08f, 0.18f), 0.4f); Shake(0.06f, 0.04f); }
        if (waveTrait == "Dark Field"){ Flash(new Color(0.04f, 0.04f, 0.22f, 0.25f), 0.5f); }
        if (waveTrait == "Iron Skin") { Flash(new Color(0.4f, 0.4f, 0.4f, 0.15f), 0.3f); }
        var ringColor = wave == MaxWave ? new Color(1f, 0.22f, 0.9f)
            : waveTrait == "Berserk" ? new Color(1f, 0.22f, 0.22f)
            : waveTrait == "Dark Field" ? new Color(0.22f, 0.28f, 1f)
            : waveTrait == "Iron Skin" ? new Color(0.72f, 0.72f, 0.72f)
            : new Color(0.32f, 1f, 0.9f);
        var ringCount = wave == MaxWave ? 36 : 24;
        for (var j = 0; j < ringCount; j++)
        {
            var a = Mathf.PI * 2f * j / ringCount;
            var dir = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            var rpos = (Vector3)(dir * ArenaRadius) + Vector3.back * 0.05f;
            var waveRingSprite = wave == MaxWave && spawnRingBossSprite != null ? spawnRingBossSprite : spawnRingNormalSprite != null ? spawnRingNormalSprite : squareSprite;
            var rgo = CreateSpriteObject("Wave Ring", waveRingSprite, rpos, WithAlpha(ringColor, 0.85f), waveRingSprite == squareSprite ? 0.1f : 0.42f);
            var rlife = Mathf.Lerp(0.5f, 0.75f, (float)rng.NextDouble());
            sparks.Add(new Spark { transform = rgo.transform, velocity = -dir * Mathf.Lerp(1.2f, 2.0f, (float)rng.NextDouble()), life = rlife, maxLife = rlife });
        }
        if (wave == MaxWave)
            BeginBossCutscene(false);
        else if (midBossWave)
            BeginBossCutscene(true);
    }

    // Wave trait の敵編成バイアスを�Eレイヤー向け 1 行ヒントにする (SpawnEnemy の type バイアスと一致させめE
    string GetWaveTraitHint(string trait)
    {
        switch (trait)
        {
            case "Shooter Raid":  return "→Shooter 多め。弾幕管理が鍵";
            case "Rush":          return "→Runner / Dasher 速攻。回り込み注意";
            case "Iron Skin":     return "→Brute 多め。火力ビルド有利";
            case "Berserk":       return "→Dasher 多め。範囲攻撃が刺さる";
            case "Dark Field":    return "→Phantom 出現。近距離注意";
            case "Elite Swarm":   return "→Elite 増加。撃破でデータ ×1.5";
            case "Boss":          return "→ボス戦";
        }
        return "";
    }

    string GetWaveTrait(int waveNumber)
    {
        if (waveNumber == MaxWave) return "Boss";
        if (waveNumber == 9) return "Dark Field";   // Final challenge before boss
        if (waveNumber == 8) return "Berserk";      // Late-game repeat
        if (waveNumber == 7) return "Elite Swarm";  // Existing
        if (waveNumber == 6) return "Iron Skin";    // Tanky wave
        if (waveNumber == 5) return "Shooter Raid"; // Existing
        if (waveNumber == 4) return "Dark Field";   // First darkness
        if (waveNumber == 3) return "Rush";         // Existing
        if (waveNumber == 2) return "Berserk";      // First berserker wave
        return "Normal";
    }

    float GetWaveHpMultiplier()
    {
        var traitMult = waveTrait == "Elite Swarm" ? 1.20f : waveTrait == "Shooter Raid" ? 1.05f
            : waveTrait == "Iron Skin" ? 1.30f : waveTrait == "Berserk" ? 0.68f : 1f;
        var globalBoost = wave <= 2 ? 0.9f : wave == 3 ? 1.0f : 1.08f;
        var lateMult = wave >= 9 ? 1.32f : wave >= 8 ? 1.22f : wave >= 7 ? 1.12f : wave >= 6 ? 1.05f : wave >= 4 ? 1.02f : 1f;
        return traitMult * lateMult * globalBoost * GetDangerHpMultiplier() * GetStageHpMultiplier();
    }

    float GetWaveSpeedMultiplier()
    {
        var traitMult = waveTrait == "Rush" ? 1.15f : waveTrait == "Elite Swarm" ? 1.02f
            : waveTrait == "Berserk" ? 1.20f : waveTrait == "Iron Skin" ? 0.85f : 1f;
        var globalBoost = wave <= 2 ? 0.92f : wave == 3 ? 1.0f : 1.05f;
        var lateMult = wave >= 8 ? 1.10f : wave >= 7 ? 1.05f : wave >= 6 ? 1.02f : wave >= 4 ? 1.0f : 1f;
        return traitMult * lateMult * globalBoost * GetDangerSpeedMultiplier() * GetStageSpeedMultiplier();
    }

    // Stage-based difficulty scaling: each stage ramps enemy HP and speed
    // Stage 0 (Lantern Field) = baseline / Stage 4 (Storm Spire) = +30% HP / +10% speed
    float GetStageHpMultiplier()
    {
        return currentStageId == 4 ? 1.30f
             : currentStageId == 3 ? 1.20f
             : currentStageId == 2 ? 1.12f
             : currentStageId == 1 ? 1.06f : 1f;
    }

    float GetStageSpeedMultiplier()
    {
        return currentStageId == 4 ? 1.10f
             : currentStageId == 3 ? 1.06f
             : currentStageId == 2 ? 1.04f
             : currentStageId == 1 ? 1.02f : 1f;
    }

    // ── Danger Level scaling (Brotato 風 Danger 0-5) ──
    // D0 = 等倁E(チE��ォルチE、D5 = HP×.0 / 速度×.30 / 接触ダメ×.50
    // Accessibility 補正は Danger と独立軸:
    //   - 敵HP / 敵速度 は enemy 生�E時に乗箁E(Danger と合箁E
    //   - 敵ダメージ は DamagePlayer の入口で一括減衰 (touch/弾/lava 全てを対象)
    float GetDangerHpMultiplier()       { return (1f + 0.20f * dangerLevel) * accessibilityEnemyHpMul; }
    float GetDangerSpeedMultiplier()    { return (1f + 0.06f * dangerLevel) * accessibilityEnemySpdMul; }
    float GetDangerTouchDamageMultiplier() { return 1f + 0.10f * dangerLevel; }
    float GetDangerSpawnMultiplier()    { return 1f + 0.10f * dangerLevel; }

    string GetWaveHint(int waveNum)
    {
        if (waveNum >= MaxWave) return "⚠ FINAL BOSS";
        if (waveNum == 5) return "中ボス Pulswyrm + Shooter多め";
        var t = GetWaveTrait(waveNum);
        var ts = t == "Rush" ? " [速↑]" : t == "Elite Swarm" ? " [HP&数↑]"
            : t == "Berserk" ? " [速↑ HP↓]" : t == "Dark Field" ? " [光-24%]" : t == "Iron Skin" ? " [HP→速↑]" : "";
        if (waveNum >= 7) return "全6種混成" + ts;
        if (waveNum >= 5) return "Bomber+Dasher出現" + ts;
        if (waveNum >= 4) return "Shooter混成" + ts;
        if (waveNum >= 3) return "Brute出現" + ts;
        return "Runner中心" + ts;
    }

    void CreateModuleStatusSidePanel()
    {
        // ── Side panel anchored to CANVAS RIGHT EDGE (independent of upgradePanel size)
        // so we can expand the upgrade panel without colliding with the module status panel.
        // Visibility is still tied to upgradePanel active state via the standard show/hide flow.
        moduleStatusSidePanel = new GameObject("Module Status Side Panel", typeof(Image));
        moduleStatusSidePanel.transform.SetParent(canvas.transform, false);
        var img = moduleStatusSidePanel.GetComponent<Image>();
        img.color = new Color(0.005f, 0.022f, 0.038f, 0.96f);
        // This panel is display-only; disable raycasts so it never blocks card clicks.
        img.raycastTarget = false;
        var outline = moduleStatusSidePanel.AddComponent<Outline>();
        outline.effectColor = new Color(0.30f, 0.90f, 1f, 0.36f);
        outline.effectDistance = new Vector2(1.2f, -1.2f);
        var sideRect = moduleStatusSidePanel.GetComponent<RectTransform>();
        // Anchor to right edge of canvas, vertically centered.
        sideRect.anchorMin = sideRect.anchorMax = new Vector2(1f, 0.5f);
        sideRect.pivot = new Vector2(1f, 0.5f);
        // Sit 10px from the right edge, centered vertically.
        // Keep it tucked to the right edge so it does not overlap the choice cards.
        sideRect.anchoredPosition = new Vector2(-10f, 0f);
        sideRect.sizeDelta = new Vector2(180f, 480f);

        // Helper to make TOP-LEFT-anchored text at exact Y offset from panel top
        Text MakeTopText(string name, float yFromTop, float height, int fontSize, Color color, string text, FontStyle style = FontStyle.Bold)
        {
            var t = CreateText(name, moduleStatusSidePanel.transform, Vector2.zero, TextAnchor.UpperLeft, fontSize, color);
            t.text = text;
            t.fontStyle = style;
            var rt = t.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);  // top-center of parent
            rt.pivot = new Vector2(0.5f, 1f);                     // own top-center
            rt.anchoredPosition = new Vector2(0, -yFromTop);
            rt.sizeDelta = new Vector2(168, height);
            t.alignment = TextAnchor.UpperCenter;
            return t;
        }

        // ── Header (y from top = 8px)  Ebumped to font 16 for readability
        var header = MakeTopText("Module Status Header", 8f, 22f, 16, new Color(0.50f, 1f, 1f), "── 取得済みモジュール ──");

        // ── Empty fallback text (y from top = 50px, shown only when zero modules)
        moduleStatusText = MakeTopText("Module Status Empty", 50f, 22f, 14, new Color(0.68f, 0.85f, 0.92f), "(まだ何も取得していない)", FontStyle.Italic);

        // ── 12 hoverable module slots stacked vertically starting at y=38 from top
        moduleSlotTexts = new Text[ModuleSlotCount];
        moduleSlotBgs = new Image[ModuleSlotCount];
        const float slotHeight = 18f;
        const float slotsStartY = 38f;
        for (var i = 0; i < ModuleSlotCount; i++)
        {
            var slotGo = new GameObject("Module Slot " + i, typeof(Image));
            slotGo.transform.SetParent(moduleStatusSidePanel.transform, false);
            var slotRect = slotGo.GetComponent<RectTransform>();
            slotRect.anchorMin = slotRect.anchorMax = new Vector2(0.5f, 1f);  // top-center anchor
            slotRect.pivot = new Vector2(0.5f, 1f);                            // own top-center
            slotRect.anchoredPosition = new Vector2(0, -(slotsStartY + i * slotHeight));
            slotRect.sizeDelta = new Vector2(168, slotHeight);
            var bg = slotGo.GetComponent<Image>();
            bg.color = new Color(0.10f, 0.32f, 0.42f, 0f);
            bg.raycastTarget = false;
            moduleSlotBgs[i] = bg;

            var slotText = CreateText("Slot Text", slotGo.transform, Vector2.zero, TextAnchor.MiddleLeft, 14, new Color(0.90f, 1f, 0.96f));
            slotText.text = "";
            var textRect = slotText.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 0);
            textRect.offsetMax = new Vector2(-6, 0);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            moduleSlotTexts[i] = slotText;

            var trigger = slotGo.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var capturedIndex = i;
            var enter = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter };
            enter.callback.AddListener(_ => OnModuleSlotHover(capturedIndex, true));
            trigger.triggers.Add(enter);
            var exit = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit };
            exit.callback.AddListener(_ => OnModuleSlotHover(capturedIndex, false));
            trigger.triggers.Add(exit);
        }

        // ── Link evo section  Efonts bumped for readability, panel is 480 tall now
        var linkHeader = MakeTopText("Link Evo Header", 268f, 22f, 15, new Color(1f, 0.88f, 0.45f), "── リンク進化条件 / ヒント ──");

        linkEvolutionConditionText = MakeTopText("Link Evo Text", 296f, 170f, 13, new Color(1f, 0.94f, 0.78f), "", FontStyle.Normal);
        linkEvolutionConditionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        linkEvolutionConditionText.verticalOverflow = VerticalWrapMode.Overflow;
    }

    void OnModuleSlotHover(int slotIndex, bool entering)
    {
        if (entering && slotIndex < currentModuleSlotTitles.Count)
        {
            moduleSlotHovering = true;
            if (moduleSlotBgs != null && slotIndex < moduleSlotBgs.Length)
                moduleSlotBgs[slotIndex].color = new Color(0.10f, 0.32f, 0.42f, 0.55f);
            var title = currentModuleSlotTitles[slotIndex];
            var desc = LookupModuleDescription(title);
            if (linkEvolutionConditionText != null)
            {
                linkEvolutionConditionText.text = "<b>" + title + "</b>\n" + desc;
                linkEvolutionConditionText.color = new Color(0.78f, 1f, 1f);
                linkEvolutionConditionText.supportRichText = true;
            }
        }
        else
        {
            moduleSlotHovering = false;
            if (moduleSlotBgs != null && slotIndex < moduleSlotBgs.Length)
                moduleSlotBgs[slotIndex].color = new Color(0.10f, 0.32f, 0.42f, 0f);
            if (linkEvolutionConditionText != null)
            {
                linkEvolutionConditionText.text = BuildFusionGuide();
                linkEvolutionConditionText.color = new Color(1f, 0.92f, 0.72f);
            }
        }
    }

    string LookupModuleDescription(string title)
    {
        for (var u = 0; u < upgradePool.Count; u++)
            if (upgradePool[u].title == title) return upgradePool[u].description;
        for (var u = 0; u < fusionUpgradePool.Count; u++)
            if (fusionUpgradePool[u].title == title) return fusionUpgradePool[u].description;
        return "(説明が見つかりません)";
    }

    // Force a RectTransform to use centre anchor/pivot with explicit position+size,
    // overriding whatever CreateText() decided based on text alignment.
    void ForceCenterRect(RectTransform rt, Vector2 anchoredPos, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
    }

    void UpdateModuleStatusSidePanel()
    {
        if (moduleSlotTexts == null) return;

        // Build unique list of picked modules (dedupe in order of first pick)
        currentModuleSlotTitles.Clear();
        var seen = new HashSet<string>();
        for (var i = 0; i < runPickedModules.Count; i++)
        {
            var t = runPickedModules[i];
            if (seen.Add(t)) currentModuleSlotTitles.Add(t);
        }

        // Fill slots: first N show modules, rest cleared/hidden
        for (var i = 0; i < ModuleSlotCount; i++)
        {
            var slotText = moduleSlotTexts[i];
            var slotBg = moduleSlotBgs[i];
            if (slotText == null) continue;

            if (i < currentModuleSlotTitles.Count)
            {
                var title = currentModuleSlotTitles[i];
                var lvl = moduleLevels.TryGetValue(title, out var v) ? v : 0;
                slotText.text = "• " + title + (lvl > 0 ? "  Lv" + lvl : "");
                if (slotBg != null) slotBg.raycastTarget = true;
            }
            else
            {
                slotText.text = "";
                if (slotBg != null)
                {
                    slotBg.raycastTarget = false;
                    slotBg.color = new Color(0.10f, 0.32f, 0.42f, 0f);
                }
            }
        }

        // Empty fallback text
        if (moduleStatusText != null)
            moduleStatusText.gameObject.SetActive(currentModuleSlotTitles.Count == 0);

        // Reset tooltip / link evo area unless currently hovering
        if (!moduleSlotHovering && linkEvolutionConditionText != null)
        {
            linkEvolutionConditionText.text = BuildFusionGuide();
            linkEvolutionConditionText.color = new Color(1f, 0.92f, 0.72f);
        }
    }

    void OpenUpgrade()
    {
        choosingUpgrade = true;
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        UpdateModuleStatusSidePanel();
        SetChoiceFocus(true);
        SetUpgradePanelTheme(false);
        if (bossModuleReady)
        {
            panelTitleText.text = "ボス撃破ボーナス";
            panelTitleText.color = new Color(1f, 0.82f, 0.28f);
            if (panelSubtitleText != null)
            {
                panelSubtitleText.text = "貴重モジュールを1つ獲得";
                panelSubtitleText.color = new Color(1f, 0.93f, 0.56f);
            }
        }
        else
        {
            var nextWave = wave + 1;
            panelTitleText.text = "進化モジュールを選ぶ";
            panelTitleText.color = new Color(1f, 0.82f, 0.42f);
            if (panelSubtitleText != null)
            {
                // プロ改喁E 次 Wave の trait + Elite 比率 + 危険度めE1 行に圧縮
                var hint = GetWaveHint(nextWave);
                var nextTrait = GetWaveTrait(nextWave);
                var traitHint = GetWaveTraitHint(nextTrait);
                var eliteHint = nextWave >= 9 ? "  Elite 12%" : nextWave >= 7 ? "  Elite 8%" : nextWave >= 5 ? "  Elite 4%" : "";
                panelSubtitleText.text = "次 Wave " + nextWave + ": " + hint + eliteHint
                    + (string.IsNullOrEmpty(traitHint) ? "" : "\n" + traitHint);
                panelSubtitleText.color = new Color(0.72f, 1f, 0.96f);
            }
        }
        cardAppearStartTime = Time.unscaledTime;
        choiceClickGuardUntil = Time.unscaledTime + ChoiceClickGuardDuration;
        RenderUpgradeChoices(PickUpgrades(bossModuleReady ? 2 : 3));
    }

    // ── プロ改喁E Lock / Banish のラン中状態管琁E──
    void TryToggleLockSlot(int slot)
    {
        if (slot < 0 || slot >= lockedSlots.Length || slot >= currentUpgradeChoices.Count) return;
        lockedSlots[slot] = !lockedSlots[slot];
        PlaySfx("Pickup", lockedSlots[slot] ? 920f : 540f, 0.04f, 0.10f);
        // Refresh the card label so the lock marker appears immediately.
        UpdateLockVisual(slot);
        // リロール表示の更新
        if (currentUpgradeChoices.Count > 0)
            RefreshRerollLabel();
    }

    void TryBanishSlot(int slot)
    {
        if (slot < 0 || slot >= currentUpgradeChoices.Count) return;
        // ロック中スロックは Banish 不可 (誤操作防止)
        if (slot < lockedSlots.Length && lockedSlots[slot]) return;
        var title = currentUpgradeChoices[slot].title;
        if (string.IsNullOrEmpty(title)) return;
        runBannedTitles.Add(title);
        AddEventLog("BANISH: " + title);
        PlaySfx("Hit", 220f, 0.08f, 0.18f);
        // そ�Eスロックだけを新規抽選 (他�E維持E
        RerollWithLocks();
    }

    void UpdateLockVisual(int slot)
    {
        if (slot < 0 || slot >= upgradeButtons.Count) return;
        var button = upgradeButtons[slot];
        if (button == null) return;
        var labelTr = button.transform.Find("Label");
        if (labelTr == null) return;
        var label = labelTr.GetComponent<Text>();
        if (label == null) return;
        // ロック中は label に頭符 🔒 を追加 (簡易、�E render で復允E��れる)
        if (lockedSlots[slot] && !label.text.StartsWith("🔒"))
            label.text = "🔒 " + label.text;
    }

    void RefreshRerollLabel()
    {
        if (rerollButton == null) return;
        var rerollLabel = rerollButton.GetComponentInChildren<Text>();
        if (rerollLabel == null) return;
        var chipCount = Mathf.FloorToInt(dataChips);
        var rerollCost = GetRerollCost();
        var lockedCount = 0;
        for (var li = 0; li < lockedSlots.Length; li++) if (lockedSlots[li]) lockedCount++;
        var lockHint = lockedCount > 0 ? "  🔒" + lockedCount : "";
        rerollLabel.text = "リロール " + rerollCost + "   所持" + chipCount + lockHint + "  [Q/W/E ロック  Z/X/C 除外]";
    }

    // 現在の choices のぁE��ロック済みは維持、それ以外を再抽選して入れ替える
    void RerollWithLocks()
    {
        var keep = new List<Upgrade>();
        var keepIndices = new List<int>();
        for (var i = 0; i < currentUpgradeChoices.Count && i < lockedSlots.Length; i++)
        {
            if (lockedSlots[i])
            {
                keep.Add(currentUpgradeChoices[i]);
                keepIndices.Add(i);
            }
        }
        var newPicksNeeded = 3 - keep.Count;
        var fresh = PickUpgrades(Mathf.Max(0, newPicksNeeded));
        var result = new List<Upgrade>(3);
        var freshIdx = 0;
        for (var i = 0; i < 3; i++)
        {
            if (i < lockedSlots.Length && lockedSlots[i] && i < currentUpgradeChoices.Count)
                result.Add(currentUpgradeChoices[i]);
            else if (freshIdx < fresh.Count)
            {
                result.Add(fresh[freshIdx]);
                freshIdx++;
            }
        }
        RenderUpgradeChoices(result);
    }

    void RenderUpgradeChoices(List<Upgrade> choices)
    {
        // ── プロ改喁E ロック/除外用に現在の choices を保持 ──
        currentUpgradeChoices.Clear();
        for (var ci = 0; ci < choices.Count; ci++) currentUpgradeChoices.Add(choices[ci]);
        // Reposition buttons based on how many will be shown so they stay centered.
        // Cards are now 280 wide →spacings widened so they don't overlap.
        // 3 buttons at x=-310, 0, +310 (spacing 310)
        // 2 buttons at x=-160, +160 (centered pair, 320 apart)
        // 1 button at x=0
        var count = Mathf.Min(choices.Count, upgradeButtons.Count);
        for (var i = 0; i < upgradeButtons.Count; i++)
        {
            var rt = upgradeButtons[i].GetComponent<RectTransform>();
            if (rt != null)
            {
                float x;
                if (count == 1)      x = 0f;
                else if (count == 2) x = -160f + i * 320f;
                else                 x = -310f + i * 310f;
                var newPos = new Vector2(x, -8f);
                rt.anchoredPosition = newPos;
                // Sync the bob-animation cache so the next AnimateCardButtons frame
                // doesn't snap buttons back to their old cached layout position.
                cardBasePositions[upgradeButtons[i]] = newPos;
            }
        }

        for (var i = 0; i < upgradeButtons.Count; i++)
        {
            if (i >= choices.Count)
            {
                upgradeButtons[i].gameObject.SetActive(false);
                continue;
            }

            upgradeButtons[i].gameObject.SetActive(true);
            upgradeButtons[i].interactable = true;
            var upgrade = choices[i];
            // NOTE: Use Find("Label") explicitly. GetComponentInChildren<Text>() returns
            // "Card Rarity Text" (first Text child) instead of the main Label.
            var label = upgradeButtons[i].transform.Find("Label").GetComponent<Text>();
            label.text = BuildUpgradeLabel(upgrade);
            SetUpgradeCardTheme(upgradeButtons[i], upgrade, label);
            // ロック状態を視覚に反映 (theme 上書き後に呼ぶ)
            if (i < lockedSlots.Length && lockedSlots[i])
                label.text = "🔒 " + label.text;
            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() =>
            {
                if (IsChoiceClickGuarded()) return;
                RegisterModuleLevel(upgrade);
                RecordModulePick(upgrade.title);
                upgrade.apply();
                PlayCardSelectEffect(upgrade, false);
                AddEventLog("MODULE: " + upgrade.title);
                // ACTIVE BUILD パネルへの注目誘封E(プレイヤーから少し上に出して HP バーと被らなぁE��置)
                // プロ改喁E y=1.8 で player sprite 上端より十�E上に表示、frequency 0.20 で読みめE��ぁE��度
                CreateFloatingText("+ " + upgrade.title, player.position + Vector3.up * 1.8f, new Color(0.65f, 1f, 0.95f), 0.20f);
                CheckSynergies();
                upgradePanel.SetActive(false);
                choosingUpgrade = false;
                SetChoiceFocus(false);
                if (queuedCutscene)
                    BeginQueuedCutscene();
                else if (bossModuleReady)
                    Time.timeScale = 1f;  // UpdateWave will trigger victory next frame
                else if (pendingFusionApply != null)
                    OpenFusionChoicePanel();
                else
                {
                    Time.timeScale = 1f;
                    ShowMessage("侵食" + wave);
                }
                UpdateUi();
            });
        }

        rerollButton.gameObject.SetActive(!bossModuleReady);
        var rerollLabel = rerollButton.GetComponentInChildren<Text>();
        var chipCount = Mathf.FloorToInt(dataChips);
        var rerollCost = GetRerollCost();
        // プロ改喁E リロール表示にロック中スロック数も表示 (ロック済みは再抽選されなぁE
        var lockedCount = 0;
        for (var li = 0; li < lockedSlots.Length; li++) if (lockedSlots[li]) lockedCount++;
        var lockHint = lockedCount > 0 ? "  🔒" + lockedCount : "";
        rerollLabel.text = "リロール " + rerollCost + "   所持" + chipCount + lockHint + "  [Q/W/E ロック  Z/X/C 除外]";
        rerollLabel.color = dataChips >= rerollCost ? new Color(0.86f, 1f, 0.96f) : new Color(0.55f, 0.68f, 0.72f);
        rerollButton.interactable = dataChips >= rerollCost;
        rerollButton.onClick.RemoveAllListeners();
        rerollButton.onClick.AddListener(() =>
        {
            if (IsChoiceClickGuarded()) return;
            var cost = GetRerollCost();
            if (dataChips < cost)
                return;
            dataChips -= cost;
            PlaySfx("Pickup", 700f, 0.05f, 0.11f);
            RerollWithLocks();
            UpdateUi();
        });

        if (skipRewardButton != null)
        {
            var skipLabel = skipRewardButton.GetComponentInChildren<Text>();
            var reward = GetSkipReward();
            if (skipLabel != null)
                skipLabel.text = "スキップ  +データ " + reward;
            skipRewardButton.gameObject.SetActive(!bossModuleReady);
            skipRewardButton.interactable = true;
            skipRewardButton.onClick.RemoveAllListeners();
            skipRewardButton.onClick.AddListener(SkipUpgradeReward);
        }
    }

    int GetSkipReward()
    {
        return Mathf.RoundToInt(8f + wave * 2f + dangerLevel * 2f);
    }

    void SkipUpgradeReward()
    {
        if (!choosingUpgrade || choosingRelic || bossModuleReady)
            return;
        if (IsChoiceClickGuarded())
            return;

        var reward = GetSkipReward();
        dataChips += reward;
        AddEventLog("SKIP: データ +" + reward);
        CreateFloatingText("DATA +" + reward, player.position + Vector3.up * 0.9f, new Color(0.42f, 1f, 0.86f), 0.12f);
        PlaySfx("Pickup", 760f, 0.08f, 0.16f);
        Flash(new Color(0.25f, 1f, 0.92f, 0.10f), 0.22f);
        upgradePanel.SetActive(false);
        choosingUpgrade = false;
        SetChoiceFocus(false);
        Time.timeScale = 1f;
        ShowMessage("モジュールをデータ化 +" + reward);
        UpdateUi();
    }

    string BuildUpgradeLabel(Upgrade upgrade)
    {
        var levelText = "";
        if (UsesModuleLevel(upgrade))
        {
            var level = GetModuleLevel(upgrade);
            var next = Mathf.Min(MaxModuleLevel, level + 1);
            levelText = "\nLv " + level + " > " + next + " / " + MaxModuleLevel;
        }

        return upgrade.title + levelText + "\n\n" + upgrade.description;
    }

    bool UsesModuleLevel(Upgrade upgrade)
    {
        return upgrade.title != "緊急補給" && !upgrade.title.Contains("仲間リンク");
    }

    string GetModuleKey(Upgrade upgrade)
    {
        return upgrade.title;
    }

    int GetModuleLevel(Upgrade upgrade)
    {
        return moduleLevels.TryGetValue(GetModuleKey(upgrade), out var level) ? level : 0;
    }

    void RegisterModuleLevel(Upgrade upgrade)
    {
        if (!UsesModuleLevel(upgrade))
            return;

        var key = GetModuleKey(upgrade);
        moduleLevels[key] = Mathf.Min(MaxModuleLevel, GetModuleLevel(upgrade) + 1);
    }

    void RecordModulePick(string title)
    {
        if (string.IsNullOrEmpty(title))
            return;

        runPickedModules.Add(title);
        runModulePicks[title] = (runModulePicks.TryGetValue(title, out var count) ? count : 0) + 1;
    }

    void SetUpgradeCardTheme(Button button, Upgrade upgrade, Text label)
    {
        var title = upgrade.title;
        var normal = new Color(0.025f, 0.1f, 0.15f, 0.98f);
        var highlight = new Color(0.06f, 0.25f, 0.32f, 1f);
        var textColor = new Color(0.82f, 1f, 1f);
        var accentColor = new Color(0.18f, 0.88f, 1f, 1f);
        var badge = "MODULE";
        var premium = 0f;

        if (title.Contains("仲間リンク") || title.Contains("クロス進化専用"))
        {
            if (title.Contains("クロス進化専用"))
            {
                normal = new Color(0.12f, 0.035f, 0.18f, 0.98f);
                highlight = new Color(0.28f, 0.09f, 0.36f, 1f);
                textColor = new Color(1f, 0.82f, 1f);
                accentColor = new Color(1f, 0.38f, 0.96f, 1f);
                badge = "CROSS";
                premium = 1.6f;
            }
            else
            {
                accentColor = GetUpgradeAccentColor(upgrade, false);
                normal = title.Contains("Bulwark") ? new Color(0.035f, 0.13f, 0.07f, 0.98f) : title.Contains("Siphon") ? new Color(0.13f, 0.14f, 0.035f, 0.98f) : new Color(0.035f, 0.1f, 0.15f, 0.98f);
                highlight = title.Contains("Bulwark") ? new Color(0.08f, 0.31f, 0.15f, 1f) : title.Contains("Siphon") ? new Color(0.28f, 0.3f, 0.08f, 1f) : new Color(0.08f, 0.28f, 0.38f, 1f);
                textColor = Color.Lerp(new Color(0.76f, 1f, 0.96f), accentColor, 0.35f);
                badge = "LINK";
                premium = 0.9f;
            }
        }
        // ── 明示プレフィチE��ス優先判宁E──
        // "SPEED専用:" / "GUARD専用:" / "POWER専用:" などはキーワード検査より先にバッジを確定させる
        // (侁E 「SPEED専用: ミラージュバースト」が「バースト」キーワードで POWER 誤判定されるのを防ぁE
        else if (title.StartsWith("SPEED専用"))
        {
            normal = new Color(0.025f, 0.085f, 0.18f, 0.98f);
            highlight = new Color(0.055f, 0.2f, 0.42f, 1f);
            textColor = new Color(0.7f, 0.94f, 1f);
            accentColor = new Color(0.22f, 0.78f, 1f, 1f);
            badge = "SPEED+";
            premium = 0.55f;
        }
        else if (title.StartsWith("GUARD専用"))
        {
            normal = new Color(0.04f, 0.13f, 0.055f, 0.98f);
            highlight = new Color(0.09f, 0.31f, 0.12f, 1f);
            textColor = new Color(0.78f, 1f, 0.68f);
            accentColor = new Color(0.48f, 1f, 0.34f, 1f);
            badge = "GUARD+";
            premium = 0.55f;
        }
        else if (title.Contains("パワー") || title.Contains("バースト") || title.Contains("ヘビー") || title.Contains("炎") || title.Contains("ボス") || title.Contains("巨弾") || title.Contains("Break"))
        {
            normal = new Color(0.16f, 0.105f, 0.025f, 0.98f);
            highlight = new Color(0.34f, 0.22f, 0.055f, 1f);
            textColor = new Color(1f, 0.9f, 0.55f);
            accentColor = new Color(1f, 0.78f, 0.18f, 1f);
            badge = "POWER";
            premium = 0.35f;
        }
        else if (title.Contains("ガード") || title.Contains("HP") || title.Contains("コア") || title.Contains("修復") || title.Contains("回復") || title.Contains("リカバリー") || title.Contains("リング") || title.Contains("Sync") || title.Contains("Data"))
        {
            normal = new Color(0.04f, 0.13f, 0.055f, 0.98f);
            highlight = new Color(0.09f, 0.31f, 0.12f, 1f);
            textColor = new Color(0.78f, 1f, 0.68f);
            accentColor = new Color(0.48f, 1f, 0.34f, 1f);
            badge = "GUARD";
            premium = 0.25f;
        }
        else if (title.Contains("スピード") || title.Contains("クイック") || title.Contains("クロック") || title.Contains("デュアル") || title.Contains("ピアス") || title.Contains("ロング") || title.Contains("SPEED"))
        {
            normal = new Color(0.025f, 0.085f, 0.18f, 0.98f);
            highlight = new Color(0.055f, 0.2f, 0.42f, 1f);
            textColor = new Color(0.7f, 0.94f, 1f);
            accentColor = new Color(0.22f, 0.78f, 1f, 1f);
            badge = "SPEED";
            premium = 0.25f;
        }

        if (UsesModuleLevel(upgrade))
        {
            var level = GetModuleLevel(upgrade);
            badge += "  Lv " + Mathf.Min(MaxModuleLevel, level + 1) + "/" + MaxModuleLevel;
        }

        ApplyCardVisual(button, label, normal, highlight, textColor, accentColor, badge, premium, upgrade.stars);
        ApplyModuleCardIcon(button, upgrade.title, accentColor);
        SetCardText(button, label, upgrade.title, GetUpgradeLevelLine(upgrade), upgrade.description, textColor, Color.Lerp(new Color(0.78f, 0.94f, 1f), textColor, 0.42f), accentColor, premium);
    }

    void SetEvolutionCardTheme(Button button, Upgrade upgrade, Text label)
    {
        var accentColor = GetUpgradeAccentColor(upgrade, true);
        var normal = new Color(0.14f, 0.105f, 0.035f, 0.98f);
        var highlight = new Color(0.34f, 0.24f, 0.07f, 1f);
        var textColor = new Color(1f, 0.92f, 0.54f);
        var badge = "EVOLVE";

        if (upgrade.title.Contains("パワー") || upgrade.title.Contains("POWER"))
        {
            normal = new Color(0.18f, 0.07f, 0.02f, 0.98f);
            highlight = new Color(0.42f, 0.16f, 0.035f, 1f);
            textColor = new Color(1f, 0.82f, 0.42f);
            badge = "EVOLVE / POWER";
        }
        else if (upgrade.title.Contains("ガード") || upgrade.title.Contains("GUARD"))
        {
            normal = new Color(0.035f, 0.16f, 0.06f, 0.98f);
            highlight = new Color(0.08f, 0.36f, 0.13f, 1f);
            textColor = new Color(0.78f, 1f, 0.64f);
            badge = "EVOLVE / GUARD";
        }
        else if (upgrade.title.Contains("スピード") || upgrade.title.Contains("SPEED"))
        {
            normal = new Color(0.02f, 0.075f, 0.2f, 0.98f);
            highlight = new Color(0.045f, 0.2f, 0.48f, 1f);
            textColor = new Color(0.68f, 0.96f, 1f);
            badge = "EVOLVE / SPEED";
        }

        ApplyCardVisual(button, label, normal, highlight, textColor, accentColor, badge, 1.9f, upgrade.stars);
        SetCardText(button, label, upgrade.title, "進化段隁E" + Mathf.Min(3, evolutionStage + 1) + " / 3", upgrade.description, textColor, Color.Lerp(new Color(0.95f, 0.98f, 1f), textColor, 0.42f), accentColor, 1.9f);
    }

    string GetUpgradeLevelLine(Upgrade upgrade)
    {
        if (!UsesModuleLevel(upgrade))
            return upgrade.title.Contains("仲間リンク") ? "LINK MODULE" : "";

        var level = GetModuleLevel(upgrade);
        var next = Mathf.Min(MaxModuleLevel, level + 1);
        // Cleaner arrow + max stage callout. e.g. "Lv 0 →Lv 1   (Max 3)"
        return "Lv " + level + " →Lv " + next + "   (Max " + MaxModuleLevel + ")";
    }

    void SetCardText(Button button, Text label, string title, string levelLine, string description, Color titleColor, Color bodyColor, Color accentColor, float premium)
    {
        if (button == null)
            return;

        var titlePlate = button.transform.Find("Card Title Plate");
        if (titlePlate != null)
        {
            var titlePlateImage = titlePlate.GetComponent<Image>();
            if (titlePlateImage != null)
            {
                if (UseCardPartsV2 && cardTitlePlateV2Sprite != null)
                    ApplySimpleSprite(titlePlateImage, cardTitlePlateV2Sprite, Color.white);
                else
                    titlePlateImage.color = Color.Lerp(new Color(0.002f, 0.012f, 0.018f, 0.94f), WithAlpha(accentColor, 0.92f), Mathf.Clamp01(0.10f + premium * 0.12f));
            }
            titlePlate.SetAsLastSibling();
        }

        var titleTransform = button.transform.Find("Card Title Text");
        if (titleTransform != null)
        {
            var titleText = titleTransform.GetComponent<Text>();
            if (titleText != null)
            {
                titleText.text = title;
                titleText.color = Color.Lerp(Color.white, titleColor, 0.58f);
                // Font size / resize bounds come from CreateButton  Edo NOT overwrite here,
                // otherwise the bigger upgraded card fonts get reset every render.
            }
            titleTransform.SetAsLastSibling();
        }

        var levelTransform = button.transform.Find("Card Level Text");
        var levelPlate = button.transform.Find("Card Level Plate");
        if (levelPlate != null)
        {
            var levelPlateImage = levelPlate.GetComponent<Image>();
            if (levelPlateImage != null)
            {
                if (UseCardPartsV2 && cardLevelPlateV2Sprite != null)
                    ApplySimpleSprite(levelPlateImage, cardLevelPlateV2Sprite, Color.white);
                else
                    levelPlateImage.color = string.IsNullOrEmpty(levelLine) ? Color.clear : Color.Lerp(new Color(0.004f, 0.02f, 0.026f, 0.64f), WithAlpha(accentColor, 0.58f), Mathf.Clamp01(0.10f + premium * 0.08f));
            }
            levelPlate.gameObject.SetActive(!string.IsNullOrEmpty(levelLine));
            levelPlate.SetAsLastSibling();
        }

        if (levelTransform != null)
        {
            var levelText = levelTransform.GetComponent<Text>();
            if (levelText != null)
            {
                levelText.text = levelLine;
                levelText.color = Color.Lerp(new Color(0.72f, 1f, 0.96f), accentColor, 0.46f);
                // Font size kept from CreateButton (16pt)  Eno override.
            }
            levelTransform.SetAsLastSibling();
        }

        if (label != null)
        {
            label.text = description;
            label.color = bodyColor;
            label.fontStyle = FontStyle.Bold;
            label.lineSpacing = 1.0f;
            // Font size, position, sizeDelta intentionally NOT overridden here  E            // CreateButton sets the proper defaults (font 15, position (0,-90), size 248×0)
            // and OpenPartnerSelect overrides differently for partner cards.
            label.transform.SetAsLastSibling();
        }

        var rarityPlate = button.transform.Find("Card Rarity Plate");
        if (rarityPlate != null)
            rarityPlate.SetAsLastSibling();

        var rarityText = button.transform.Find("Card Rarity Text");
        if (rarityText != null)
            rarityText.SetAsLastSibling();

        var badgeText = button.transform.Find("Card Badge");
        if (badgeText != null)
            badgeText.SetAsLastSibling();
    }

    int CountFilledStars(string stars)
    {
        if (string.IsNullOrEmpty(stars))
            return 0;

        var filled = 0;
        for (var i = 0; i < stars.Length; i++)
            if (stars[i] == '★')
                filled++;
        return filled;
    }

    string FormatCardRarity(string stars)
    {
        if (string.IsNullOrEmpty(stars))
            return "";

        var filled = CountFilledStars(stars);
        var label = filled >= 3 ? "EPIC" : filled == 2 ? "RARE" : "BASIC";
        return stars + "  " + label;
    }

    Sprite GetCardHeaderV2Sprite(string rarity, string badge, float premium)
    {
        var filled = CountFilledStars(rarity);
        if (badge.Contains("CROSS") || premium >= 1.55f || filled >= 3)
            return cardHeaderEpicV2Sprite;
        if (filled == 2 || premium > 1.15f)
            return cardHeaderRareV2Sprite;
        return cardHeaderBasicV2Sprite;
    }

    Sprite GetCardRarityPlateV2Sprite(string rarity)
    {
        var filled = CountFilledStars(rarity);
        if (filled >= 3) return cardRarityPlateEpicV2Sprite;
        if (filled == 2) return cardRarityPlateRareV2Sprite;
        if (filled == 1) return cardRarityPlateBasicV2Sprite;
        return null;
    }

    Sprite GetCardBottomRailV2Sprite(string badge, float premium)
    {
        if (badge.Contains("CROSS") || badge.Contains("EVOLVE") || premium >= 1.45f)
            return cardBottomRailGoldV2Sprite;
        return cardBottomRailCyanV2Sprite;
    }

    Sprite GetCardSpecialCornerV2Sprite(string badge, float premium)
    {
        if (badge.Contains("CROSS"))
            return cardSpecialCornerCrossV2Sprite;
        if (badge.Contains("EVOLVE") || premium >= 1.45f)
            return cardSpecialCornerEpicV2Sprite;
        return null;
    }

    void ApplyCardVisual(Button button, Text label, Color normal, Color highlight, Color textColor, Color accentColor, string badge, float premium, string rarity = "")
    {
        SetButtonTint(button, normal, highlight);
        ApplyGeneratedCardFrame(button, badge, accentColor, premium);
        label.color = textColor;
        // Font size / position / sizeDelta come from CreateButton  Edo NOT overwrite here,
        // otherwise the bigger module/partner card layout breaks every render.

        var outline = button.GetComponent<Outline>();
        if (outline != null)
            outline.effectColor = WithAlpha(accentColor, Mathf.Clamp01(0.48f + premium * 0.18f));

        var header = button.transform.Find("Card Header Plate");
        if (header != null)
        {
            var headerImage = header.GetComponent<Image>();
            if (headerImage != null)
            {
                var headerSprite = UseCardPartsV2 ? GetCardHeaderV2Sprite(rarity, badge, premium) : null;
                if (headerSprite != null)
                    ApplySimpleSprite(headerImage, headerSprite, Color.white);
                else
                    headerImage.color = Color.Lerp(new Color(0.004f, 0.018f, 0.024f, 0.9f), WithAlpha(accentColor, 0.82f), Mathf.Clamp01(0.16f + premium * 0.12f));
            }
        }

        var strip = button.transform.Find("Card Neon Strip");
        if (strip != null)
        {
            var stripImage = strip.GetComponent<Image>();
            if (stripImage != null)
                stripImage.color = WithAlpha(accentColor, Mathf.Clamp01(0.48f + premium * 0.16f));

            var stripRect = strip.GetComponent<RectTransform>();
            if (stripRect != null)
                // Rescaled for 280-wide card (was for 210-wide). Range bumped from 86-174 to 114-230.
                stripRect.sizeDelta = new Vector2(Mathf.Lerp(114f, 230f, Mathf.Clamp01(premium * 0.52f)), premium > 1.2f ? 6f : 4f);
        }

        var icon = button.transform.Find("Card Icon");
        if (icon != null)
        {
            var iconImage = icon.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = badge.Contains("GUARD") ? squareSprite : premium > 1.2f || badge.Contains("CROSS") ? diamondSprite : circleSprite;
                iconImage.color = WithAlpha(accentColor, Mathf.Clamp01(0.18f + premium * 0.11f));
            }
        }

        var iconCore = button.transform.Find("Card Icon Core");
        if (iconCore != null)
        {
            var iconCoreImage = iconCore.GetComponent<Image>();
            if (iconCoreImage != null)
                iconCoreImage.color = WithAlpha(Color.Lerp(Color.white, accentColor, 0.48f), Mathf.Clamp01(0.46f + premium * 0.16f));
        }

        var energyRing = button.transform.Find("Card Energy Ring");
        if (energyRing != null)
        {
            var energyRingImage = energyRing.GetComponent<Image>();
            if (energyRingImage != null)
                energyRingImage.color = WithAlpha(accentColor, Mathf.Clamp01(0.06f + premium * 0.075f));
            var energyRingRect = energyRing.GetComponent<RectTransform>();
            if (energyRingRect != null)
                // Rescaled for bigger card. Range bumped from 62-92 to 88-130.
                energyRingRect.sizeDelta = Vector2.one * Mathf.Lerp(88f, 130f, Mathf.Clamp01(0.25f + premium * 0.36f));
        }

        var raritySpike = button.transform.Find("Card Rarity Spike");
        if (raritySpike != null)
        {
            var raritySpikeImage = raritySpike.GetComponent<Image>();
            var cornerSprite = UseCardPartsV2 ? GetCardSpecialCornerV2Sprite(badge, premium) : null;
            if (raritySpikeImage != null)
            {
                if (cornerSprite != null)
                    ApplySimpleSprite(raritySpikeImage, cornerSprite, Color.white);
                else
                {
                    raritySpikeImage.sprite = premium > 1.2f || badge.Contains("CROSS") ? diamondSprite : circleSprite;
                    raritySpikeImage.color = WithAlpha(accentColor, Mathf.Clamp01(0.26f + premium * 0.18f));
                }
            }
            var raritySpikeRect = raritySpike.GetComponent<RectTransform>();
            if (raritySpikeRect != null)
                // Rescaled for bigger card. 22/30 →28/38.
                raritySpikeRect.sizeDelta = cornerSprite != null ? Vector2.one * 40f : Vector2.one * (premium > 1.2f ? 38f : 28f);
        }

        var bottomRail = button.transform.Find("Card Bottom Rail");
        if (bottomRail != null)
        {
            var bottomRailImage = bottomRail.GetComponent<Image>();
            var railSprite = UseCardPartsV2 ? GetCardBottomRailV2Sprite(badge, premium) : null;
            if (bottomRailImage != null)
            {
                if (railSprite != null)
                    ApplySimpleSprite(bottomRailImage, railSprite, Color.white);
                else
                    bottomRailImage.color = WithAlpha(accentColor, Mathf.Clamp01(0.34f + premium * 0.12f));
            }

            var bottomRailRect = bottomRail.GetComponent<RectTransform>();
            if (bottomRailRect != null)
                bottomRailRect.sizeDelta = railSprite != null
                    ? new Vector2(210f, 8f)
                    : new Vector2(Mathf.Lerp(180f, 244f, Mathf.Clamp01(premium * 0.5f)), premium > 1.2f ? 5f : 4f);
        }

        var glow = button.transform.Find("Card Glow Fill");
        if (glow != null)
        {
            var glowImage = glow.GetComponent<Image>();
            if (glowImage != null)
                glowImage.color = WithAlpha(accentColor, Mathf.Clamp01(0.055f + premium * 0.055f));
        }

        var badgeTransform = button.transform.Find("Card Badge");
        if (badgeTransform != null)
        {
            var badgeText = badgeTransform.GetComponent<Text>();
            if (badgeText != null)
            {
                badgeText.text = badge;
                // Bigger badge font for the bigger card (12 →16).
                badgeText.fontSize = 16;
                badgeText.resizeTextForBestFit = true;
                badgeText.resizeTextMinSize = 12;
                badgeText.resizeTextMaxSize = 16;
                badgeText.color = Color.Lerp(new Color(0.84f, 1f, 0.98f), accentColor, 0.54f);
            }
        }

        var rarityPlate = button.transform.Find("Card Rarity Plate");
        if (rarityPlate != null)
        {
            var rarityPlateImage = rarityPlate.GetComponent<Image>();
            if (rarityPlateImage != null)
            {
                var raritySprite = UseCardPartsV2 ? GetCardRarityPlateV2Sprite(rarity) : null;
                if (raritySprite != null)
                    ApplySimpleSprite(rarityPlateImage, raritySprite, Color.white);
                else
                {
                    rarityPlateImage.sprite = null;
                    rarityPlateImage.color = string.IsNullOrEmpty(rarity) ? Color.clear : Color.Lerp(new Color(0.004f, 0.018f, 0.024f, 0.88f), WithAlpha(accentColor, 0.8f), Mathf.Clamp01(0.12f + premium * 0.1f));
                }
            }
        }

        var rarityTransform = button.transform.Find("Card Rarity Text");
        if (rarityTransform != null)
        {
            var rarityText = rarityTransform.GetComponent<Text>();
            if (rarityText != null)
            {
                rarityText.text = FormatCardRarity(rarity);
                rarityText.fontSize = 16;
                rarityText.resizeTextForBestFit = true;
                rarityText.resizeTextMinSize = 11;
                rarityText.resizeTextMaxSize = 16;
                rarityText.color = Color.Lerp(new Color(1f, 0.88f, 0.32f), accentColor, premium > 1.2f ? 0.42f : 0.18f);
            }
        }
    }

    void ApplyGeneratedCardFrame(Button button, string badge, Color accentColor, float premium)
    {
        if (!UseGeneratedCards)
        {
            var cleanImage = button.GetComponent<Image>();
            if (cleanImage != null)
            {
                cleanImage.sprite = null;
                cleanImage.type = Image.Type.Simple;
                cleanImage.preserveAspect = false;
            }
            return;
        }

        var frame = GetGeneratedCardFrame(badge, accentColor, premium);
        if (frame == null)
            return;

        var image = button.GetComponent<Image>();
        if (image == null)
            return;

        image.sprite = frame;
        image.color = Color.white;
        image.type = Image.Type.Simple;
        image.preserveAspect = false;

        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.Lerp(Color.white, accentColor, 0.18f);
        colors.selectedColor = colors.highlightedColor;
        colors.pressedColor = Color.Lerp(Color.white, accentColor, 0.45f);
        button.colors = colors;
    }

    Sprite GetGeneratedCardFrame(string badge, Color accentColor, float premium)
    {
        if (badge.Contains("CROSS") && cardMagentaSprite != null)
            return cardMagentaSprite;
        if (badge.Contains("GUARD") && cardGreenSprite != null)
            return cardGreenSprite;
        if (badge.Contains("POWER") && cardGoldSprite != null)
            return cardGoldSprite;
        if (badge.Contains("SPEED") && cardCyanSprite != null)
            return cardCyanSprite;
        if (accentColor.r > 0.9f && accentColor.g < 0.45f && accentColor.b < 0.45f && cardRedSprite != null)
            return cardRedSprite;
        if (accentColor.r > 0.75f && accentColor.b > 0.72f && cardMagentaSprite != null)
            return cardMagentaSprite;
        if (accentColor.g > 0.8f && accentColor.r < 0.72f && cardGreenSprite != null)
            return cardGreenSprite;
        if (premium >= 1.55f && cardGoldSprite != null)
            return cardGoldSprite;
        return cardCyanSprite;
    }

    void SetUpgradePanelTheme(bool evolution)
    {
        var accent = evolution ? new Color(1f, 0.78f, 0.22f, 1f) : new Color(0.25f, 0.95f, 1f, 1f);
        var side = evolution ? new Color(1f, 0.26f, 0.95f, 1f) : new Color(0.52f, 1f, 0.38f, 1f);
        var panelImage = upgradePanel.GetComponent<Image>();
        if (panelImage != null)
        {
            var sprite = evolution && panelFrameGoldSprite != null ? panelFrameGoldSprite : panelFrameCyanSprite;
            if (UseGeneratedHudPanels && sprite != null)
                ApplySimpleSprite(panelImage, sprite, Color.white);
            else
            {
                panelImage.sprite = null;
                panelImage.color = evolution ? new Color(0.032f, 0.018f, 0.04f, 0.97f) : new Color(0.006f, 0.018f, 0.03f, 0.95f);
            }
        }

        var outline = upgradePanel.GetComponent<Outline>();
        if (outline != null)
            outline.effectColor = WithAlpha(accent, evolution ? 0.72f : 0.42f);

        if (panelTitleText != null)
            panelTitleText.color = evolution ? new Color(1f, 0.86f, 0.38f) : new Color(0.62f, 1f, 1f);

        SetPanelAccentColor(upgradePanel.transform, accent, side);
    }

    void SetPanelAccentColor(Transform parent, Color top, Color side)
    {
        SetChildImageColor(parent, "Panel Neon Top", WithAlpha(top, 0.56f));
        SetChildImageColor(parent, "Panel Neon Side", WithAlpha(side, 0.42f));
        SetChildImageColor(parent, "Panel Neon Bottom", WithAlpha(top, 0.24f));
        SetChildImageColor(parent, "Panel Corner TL", WithAlpha(top, 0.5f));
        SetChildImageColor(parent, "Panel Corner TL V", WithAlpha(top, 0.5f));
        SetChildImageColor(parent, "Panel Corner TR", WithAlpha(top, 0.34f));
        SetChildImageColor(parent, "Panel Corner TR V", WithAlpha(top, 0.34f));
        SetChildImageColor(parent, "Panel Corner BL", WithAlpha(side, 0.34f));
        SetChildImageColor(parent, "Panel Corner BL V", WithAlpha(side, 0.34f));
        SetChildImageColor(parent, "Panel Corner BR", WithAlpha(top, 0.26f));
        SetChildImageColor(parent, "Panel Corner BR V", WithAlpha(top, 0.26f));
    }

    void SetChildImageColor(Transform parent, string childName, Color color)
    {
        var child = parent.Find(childName);
        if (child == null)
            return;

        var image = child.GetComponent<Image>();
        if (image != null)
            image.color = color;
    }

    Color WithAlpha(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    Color GetUpgradeAccentColor(Upgrade upgrade, bool evolutionCard)
    {
        var title = upgrade.title;
        if (evolutionCard)
        {
            if (title.Contains("パワー") || title.Contains("POWER") || title.Contains("炎") || title.Contains("巨弾") || title.Contains("ボス"))
                return new Color(1f, 0.55f, 0.16f, 1f);
            if (title.Contains("ガード") || title.Contains("GUARD") || title.Contains("コア") || title.Contains("反撃") || title.Contains("リカバリー"))
                return new Color(0.48f, 1f, 0.38f, 1f);
            return new Color(0.26f, 0.9f, 1f, 1f);
        }

        if (title.Contains("クロス進化専用"))
            return new Color(1f, 0.38f, 0.96f, 1f);
        if (title.Contains("Nova"))
            return new Color(0.3f, 0.95f, 1f, 1f);
        if (title.Contains("Bulwark"))
            return new Color(0.42f, 1f, 0.56f, 1f);
        if (title.Contains("Siphon"))
            return new Color(0.95f, 1f, 0.35f, 1f);
        if (title.Contains("仲間リンク"))
            return new Color(0.25f, 1f, 0.92f, 1f);
        if (title.Contains("パワー") || title.Contains("バースト") || title.Contains("ヘビー") || title.Contains("爆") || title.Contains("ボス") || title.Contains("Break"))
            return new Color(1f, 0.78f, 0.18f, 1f);
        if (title.Contains("ガード") || title.Contains("HP") || title.Contains("コア") || title.Contains("修復") || title.Contains("回復") || title.Contains("リカバリー") || title.Contains("リング") || title.Contains("Sync") || title.Contains("Data"))
            return new Color(0.48f, 1f, 0.34f, 1f);
        return new Color(0.22f, 0.78f, 1f, 1f);
    }

    void PlayCardSelectEffect(Upgrade upgrade, bool evolutionCard)
    {
        var accent = GetUpgradeAccentColor(upgrade, evolutionCard);
        var isCross = upgrade.title.Contains("クロス進化専用");
        var isLink = upgrade.title.Contains("仲間リンク");
        var isSpecial = evolutionCard || isCross || isLink || upgrade.stars.Contains("★★★");

        SpawnSparks(player.position, accent, evolutionCard ? 24 : isCross ? 44 : isLink ? 28 : isSpecial ? 22 : 12, evolutionCard ? 1.15f : isCross ? 1.75f : isLink ? 1.25f : 0.75f);

        if (evolutionCard)
        {
            SpawnRingSparks(player.position, accent, 38, 1.55f);
            Flash(new Color(accent.r, accent.g, accent.b, 0.34f), 0.58f);
            Shake(0.22f, 0.09f);
            PlaySfx("Evolve", 360f, 0.18f, 0.28f);
            return;
        }

        Flash(new Color(accent.r, accent.g, accent.b, isSpecial ? 0.28f : 0.14f), isSpecial ? 0.52f : 0.28f);
        Shake(isSpecial ? 0.18f : 0.08f, isSpecial ? 0.08f : 0.035f);
        PlaySfx(isCross ? "Fusion" : isLink ? "Pickup" : "LevelUp", isCross ? 520f : isLink ? 760f : 680f, isSpecial ? 0.16f : 0.07f, isSpecial ? 0.24f : 0.11f);
    }

    void UpdateChoiceCardEffects()
    {
        if (upgradePanel != null && upgradePanel.activeSelf)
            AnimateCardButtons(upgradeButtons);
        if (partnerPanel != null && partnerPanel.activeSelf)
            AnimateCardButtons(partnerButtons);
    }

    void AnimateCardButtons(List<Button> buttons)
    {
        var time = Time.unscaledTime;
        var appearGlobal = time - cardAppearStartTime;
        for (var i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i];
            if (button == null || !button.gameObject.activeInHierarchy)
                continue;

            var badgeText = button.transform.Find("Card Badge")?.GetComponent<Text>();
            var badge = badgeText != null ? badgeText.text : "";
            var premium = badge.Contains("EVOLVE") || badge.Contains("CROSS") || badge.Contains("LINK") || badge.Contains("RELIC");
            var pulse = 0.5f + Mathf.Sin(time * (premium ? 5.4f : 3.6f) + i * 0.85f) * 0.5f;

            var cardElapsed = Mathf.Max(0f, appearGlobal - i * 0.08f);
            var appearT = Mathf.Clamp01(cardElapsed / 0.42f);
            var ease = 1f - Mathf.Pow(1f - appearT, 3f);
            var appearScale = Mathf.Lerp(0.78f, 1f, ease);
            var bob = Mathf.Sin(time * 2.4f + i * 1.1f) * 4f * ease;
            var rect = button.GetComponent<RectTransform>();
            if (rect != null)
            {
                if (!cardBasePositions.TryGetValue(button, out var basePos))
                {
                    basePos = rect.anchoredPosition;
                    cardBasePositions[button] = basePos;
                }
                rect.anchoredPosition = basePos + new Vector2(0f, bob);
                rect.localScale = Vector3.one * appearScale;
            }

            var strip = button.transform.Find("Card Neon Strip");
            if (strip != null)
            {
                var stripImage = strip.GetComponent<Image>();
                if (stripImage != null)
                {
                    var color = stripImage.color;
                    color.a = premium ? Mathf.Lerp(0.46f, 0.92f, pulse) : Mathf.Lerp(0.28f, 0.56f, pulse);
                    stripImage.color = color;
                }
            }

            var header = button.transform.Find("Card Header Plate");
            if (header != null)
            {
                var headerImage = header.GetComponent<Image>();
                if (headerImage != null)
                {
                    var color = headerImage.color;
                    color.a = premium ? Mathf.Lerp(0.68f, 0.88f, pulse) : Mathf.Lerp(0.54f, 0.74f, pulse);
                    headerImage.color = color;
                }
            }

            var icon = button.transform.Find("Card Icon");
            if (icon != null)
                icon.localScale = Vector3.one * (premium ? Mathf.Lerp(0.96f, 1.16f, pulse) : Mathf.Lerp(0.98f, 1.06f, pulse));

            var iconCore = button.transform.Find("Card Icon Core");
            if (iconCore != null)
                iconCore.localScale = Vector3.one * Mathf.Lerp(0.92f, 1.18f, pulse);

            var energyRing = button.transform.Find("Card Energy Ring");
            if (energyRing != null)
            {
                energyRing.localScale = Vector3.one * (premium ? Mathf.Lerp(0.94f, 1.22f, pulse) : Mathf.Lerp(0.98f, 1.08f, pulse));
                energyRing.Rotate(0f, 0f, Time.unscaledDeltaTime * (premium ? 80f : 32f));
                var ringImage = energyRing.GetComponent<Image>();
                if (ringImage != null)
                {
                    var color = ringImage.color;
                    color.a = premium ? Mathf.Lerp(0.08f, 0.28f, pulse) : Mathf.Lerp(0.035f, 0.11f, pulse);
                    ringImage.color = color;
                }
            }

            var raritySpike = button.transform.Find("Card Rarity Spike");
            if (raritySpike != null)
            {
                raritySpike.localScale = Vector3.one * (premium ? Mathf.Lerp(0.92f, 1.18f, pulse) : Mathf.Lerp(0.96f, 1.04f, pulse));
                raritySpike.Rotate(0f, 0f, Time.unscaledDeltaTime * (premium ? -90f : -28f));
            }

            var bottomRail = button.transform.Find("Card Bottom Rail");
            if (bottomRail != null)
            {
                var railImage = bottomRail.GetComponent<Image>();
                if (railImage != null)
                {
                    var color = railImage.color;
                    color.a = premium ? Mathf.Lerp(0.4f, 0.88f, pulse) : Mathf.Lerp(0.22f, 0.44f, pulse);
                    railImage.color = color;
                }
            }

            var glow = button.transform.Find("Card Glow Fill");
            if (glow != null)
            {
                glow.localScale = Vector3.one * (premium ? Mathf.Lerp(0.96f, 1.08f, pulse) : Mathf.Lerp(0.98f, 1.03f, pulse));
                var glowImage = glow.GetComponent<Image>();
                if (glowImage != null)
                {
                    var color = glowImage.color;
                    color.a = premium ? Mathf.Lerp(0.08f, 0.2f, pulse) : Mathf.Lerp(0.035f, 0.09f, pulse);
                    glowImage.color = color;
                }
            }

            var outline = button.GetComponent<Outline>();
            if (outline != null)
            {
                var color = outline.effectColor;
                color.a = premium ? Mathf.Lerp(0.5f, 0.95f, pulse) : Mathf.Lerp(0.28f, 0.6f, pulse);
                outline.effectColor = color;
            }
        }
    }

    bool ShouldEvolve()
    {
        return evolutionStage == 0 && level >= 3 || evolutionStage == 1 && level >= 6 || evolutionStage == 2 && level >= 9;
    }

    void OpenEvolution()
    {
        choosingUpgrade = true;
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        SetChoiceFocus(true);
        SetUpgradePanelTheme(true);
        panelTitleText.text = evolutionStage == 0 ? "相棒の進化ルートを決める" : GetRouteLabel() + "ルートを伸ばす";
        if (panelSubtitleText != null)
        {
            panelSubtitleText.text = evolutionStage == 0 ? "以後の成長方向を選択" : "Lv " + level + " 到達 /  進化段階 " + (evolutionStage + 1) + " / 3";
            panelSubtitleText.color = new Color(1f, 0.9f, 0.56f);
        }
        rerollButton.gameObject.SetActive(false);
        if (skipRewardButton != null)
            skipRewardButton.gameObject.SetActive(false);
        cardAppearStartTime = Time.unscaledTime;
        choiceClickGuardUntil = Time.unscaledTime + ChoiceClickGuardDuration;
        var nextStage = evolutionStage + 1;
        var choices = BuildEvolutionChoices(nextStage);

        for (var i = 0; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].gameObject.SetActive(true);
            upgradeButtons[i].interactable = true;
            var upgrade = choices[i];
            var label = upgradeButtons[i].transform.Find("Label").GetComponent<Text>();
            label.text = upgrade.title + "\n\n" + upgrade.description;
            SetEvolutionCardTheme(upgradeButtons[i], upgrade, label);
            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() =>
            {
                if (IsChoiceClickGuarded()) return;
                RecordModulePick(upgrade.title);
                upgrade.apply();
                PlayCardSelectEffect(upgrade, true);
                AddEventLog("EVOLVE: " + upgrade.title);
                CheckSynergies();
                upgradePanel.SetActive(false);
                choosingUpgrade = false;
                SetChoiceFocus(false);
                if (queuedCutscene)
                    BeginQueuedCutscene();
                else if (pendingFusionApply != null)
                    OpenFusionChoicePanel();
                else
                {
                    Time.timeScale = 1f;
                    ShowMessage("進化完亁E " + formName);
                }
                UpdateUi();
            });
        }
    }

    Upgrade[] BuildEvolutionChoices(int nextStage)
    {
        Upgrade[] pool;
        if (evolutionStage == 0 || formStyle == 0)
        {
            pool = new[]
            {
                new Upgrade("スピード進化", "ルート固定 SPEED\n移動と連射に優れた細身の翼型へ。", () => Evolve(nextStage, "スピードフォーム", 1.06f, 1.07f, 1.02f, 0f, 1, 1)),
                new Upgrade("パワー進化", "ルート固定 POWER\n重い一撃で押し切る角と爪の姿へ。", () => Evolve(nextStage, "パワーフォーム", 1.02f, 1.02f, 1.11f, 0.6f, 2, 1)),
                new Upgrade("ガード進化", "ルート固定 GUARD\nコアを守る装甲と盾の姿へ。\nノックバックも開始。", () => { Evolve(nextStage, "ガードフォーム", 1.05f, 1.04f, 1.08f, 2.5f, 3, 1); playerKnockback += 1.5f; })
            };
        }
        else if (formStyle == 1)
        {
            pool = new[]
            {
                new Upgrade("SPEED: 連射翼", "同ルート進化\n攻撃速度と弾数を伸ばす。", () => { Evolve(nextStage, "連射翼フォーム", 1.03f, 1.07f, 1.01f, 0f, 1, 1); bulletCount++; }),
                new Upgrade("SPEED: 貫通レーザー", "同ルート進化\nレーザーが敵を貫く。", () => { Evolve(nextStage, "貫通レーザーフォーム", 1.03f, 1.04f, 1.02f, 0f, 1, 2); bulletPierce++; bulletSpeed *= 1.03f; }),
                new Upgrade("SPEED: ミラージュ", "同ルート進化\n移動速度と回収範囲を伸ばす。", () => { Evolve(nextStage, "ミラージュフォーム", 1.06f, 1.02f, 1.00f, 0f, 1, 3); pickupRange *= 1.06f; }),
                new Upgrade("SPEED: ホーミングビット", "同ルート進化\n弾が敵を追尾し、連鎖が伸びる。", () => { Evolve(nextStage, "ホーミングフォーム", 1.04f, 1.04f, 1.01f, 0f, 1, 4); speedChainBonus++; bulletLifeMultiplier *= 1.08f; })
            };
        }
        else if (formStyle == 2)
        {
            pool = new[]
            {
                new Upgrade("POWER: 爆発コア", "同ルート進化\n爆発範囲と火力を伸ばす。", () => { Evolve(nextStage, "爆発コアフォーム", 1.01f, 1.00f, 1.07f, 0.5f, 2, 1); explodeChance = Mathf.Min(0.48f, explodeChance + 0.07f); }),
                new Upgrade("POWER: 巨弾アーム", "同ルート進化\n弾のサイズと一撃破強化。", () => { Evolve(nextStage, "巨弾アームフォーム", 1.00f, 0.97f, 1.10f, 0.5f, 2, 2); bulletSize *= 1.05f; }),
                new Upgrade("POWER: ボスブレイカー", "同ルート進化\nボスへの火力を伸ばす。", () => { Evolve(nextStage, "ボスブレイカーフォーム", 1.02f, 1.01f, 1.06f, 0.5f, 2, 3); bossDamageMultiplier *= 1.10f; }),
                new Upgrade("POWER: シージモード", "同ルート進化\n発射は遅いが大型の貫通弾を放つ。", () => { Evolve(nextStage, "シージフォーム", 0.95f, 0.86f, 1.18f, 0.7f, 2, 4); bulletPierce++; bulletSize *= 1.10f; })
            };
        }
        else
        {
            pool = new[]
            {
                new Upgrade("GUARD: コアシールド", "同ルート進化\nコアHPと光の範囲を伸ばす。\n接触敵を強くノックバック。", () => { Evolve(nextStage, "コアシールドフォーム", 1.03f, 1.03f, 1.05f, 3f, 3, 1); lightRadius *= 1.07f; lantern.GetChild(0).localScale = Vector3.one * lightRadius * 1.75f; playerKnockback += 3.2f; orbitKnockback += 2.4f; }),
                new Upgrade("GUARD: 反撃盾", "同ルート進化\n接触反撃とリング威力を伸ばす。\n接触敵を強くノックバック。", () => { Evolve(nextStage, "反撃盾フォーム", 1.03f, 1.04f, 1.06f, 2f, 3, 2); contactBurst += 0.8f; orbitDamage *= 1.1f; playerKnockback += 4.0f; orbitKnockback += 3.0f; }),
                new Upgrade("GUARD: リカバリーコア", "同ルート進化\n回復と耐久を伸ばす。\n接触敵をノックバック。", () => { Evolve(nextStage, "リカバリーコアフォーム", 1.03f, 1.03f, 1.04f, 2.5f, 3, 3); pickupHealChance = Mathf.Min(0.45f, pickupHealChance + 0.1f); playerKnockback += 2.2f; }),
                new Upgrade("GUARD: 鏡盾フォーム", "同ルート進化\n反射シールドと盾リングを起動。\n接触敵を強くノックバック。", () => { Evolve(nextStage, "鏡盾フォーム", 1.02f, 1.02f, 1.04f, 2.5f, 3, 4); reflectShield = true; orbitShield = true; orbitDamage *= 1.06f; playerKnockback += 3.6f; orbitKnockback += 2.6f; })
            };
        }

        if (pool.Length <= 3)
            return pool;
        var picked = new List<Upgrade>(pool);
        var result = new Upgrade[3];
        for (var i = 0; i < 3; i++)
        {
            var idx = rng.Next(picked.Count);
            result[i] = picked[idx];
            picked.RemoveAt(idx);
        }
        return result;
    }

    void Evolve(int nextStage, string chosenForm, float speedMultiplier, float rateMultiplier, float damageMultiplier, float guardBonus, int chosenStyle, int visualVariant = 0)
    {
        evolutionStage = nextStage;
        formStyle = chosenStyle;
        evolutionVariantStyle = Mathf.Clamp(visualVariant, 0, 4);
        formName = GetStageName(evolutionStage) + " / " + chosenForm;
        moveSpeed *= speedMultiplier;
        fireRate *= rateMultiplier;
        bulletDamage *= damageMultiplier;
        bulletCount = Mathf.Max(bulletCount, evolutionStage + 1);
        playerMaxHp += 1.5f + guardBonus;
        playerHp = Mathf.Min(playerMaxHp, playerHp + 2.5f + guardBonus);
        lanternMaxHp += guardBonus;
        lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 2f + guardBonus);
        ApplyEvolutionRouteBonus(chosenStyle);
        ApplyPlayerVisuals(true);
        SpawnSparks(player.position, GetFormAccentColor(), 52, 1.9f);
        Flash(new Color(GetFormAccentColor().r, GetFormAccentColor().g, GetFormAccentColor().b, 0.5f), 0.9f);
        Shake(0.35f, 0.16f);
        PlaySfx("Evolve", 880f, 0.32f, 0.5f);
        QueueEvolutionCutscene("EVOLUTION", formName, GetEvolutionRouteDescription(), GetFormAccentColor());
    }

    void ApplyEvolutionRouteBonus(int chosenStyle)
    {
        if (chosenStyle == 1)
        {
            // SPEED route bonuses  Enerfed from 1.08 to 1.05 (SPEED was too easy)
            bulletLifeMultiplier *= 1.05f;
            bulletSpeed *= 1.05f;
            speedEchoTimer = 0f;
        }
        else if (chosenStyle == 2)
        {
            // POWER route bonuses  Enerfed (POWER was too dominant)
            bulletSize *= 1.05f;
            explodeChance = Mathf.Min(0.55f, explodeChance + 0.05f);
        }
        else if (chosenStyle == 3)
        {
            // GUARD route bonuses  Eslight buff (knockback now grants extra stack)
            orbitShield = true;
            orbitDamage *= 1.28f;
            lightRadius *= 1.10f;
            lantern.GetChild(0).localScale = Vector3.one * lightRadius * 1.75f;
            guardPulseTimer = 0f;
            playerKnockback += 1.0f;  // every GUARD evolution adds knockback
        }
    }

    string GetEvolutionRouteDescription()
    {
        if (formStyle == 1)
            return "高速レーザービット解放 / 弾速・連射強化";
        if (formStyle == 2)
            return "重撃プラズマコア解放 / 大型弾・爆発特化";
        if (formStyle == 3)
            return "守護リングアーマー解放 / 貫通・防衛特化";
        return "新しい戦闘フォームを獲得";
    }

    void ApplyPlayerVisuals(bool burst)
    {
        if (playerRenderer == null)
        {
            playerRenderer = playerVisualBody != null ? playerVisualBody.GetComponent<SpriteRenderer>() : player.GetComponent<SpriteRenderer>();
            if (playerVisualBody == null && playerRenderer != null)
                playerVisualBody = playerRenderer.transform;
        }

        var spriteIndex = Mathf.Clamp(evolutionStage, 0, playerFormSprites.Length - 1);
        // クロス進化は最終形のみ存在する設計。融合中は実段階に関わらず常に L3 の融合アートを使う
        // (融合は evolutionStage>=2 で発動するが、見た目は最終フォームに統一)。よって素材は F*_L3 のみで足りる。
        var visualStage = fusionStyle > 0 ? 3 : evolutionStage;
        playerRenderer.sprite = partnerStyle > 0
            ? GetPartnerVariantSprite(visualStage, formStyle, fusionStyle, partnerStyle, evolutionVariantStyle)
            : formStyle == 0 && fusionStyle == 0
                ? playerFormSprites[spriteIndex]
                : MakePartnerVariantSprite(visualStage, formStyle, fusionStyle, 1, evolutionVariantStyle);

        var styleScale = formStyle == 2 ? 0.09f : formStyle == 1 ? -0.02f : formStyle == 3 ? 0.06f : 0f;
        var partnerScale = partnerStyle == 3 ? 0.06f : partnerStyle == 4 ? 0.02f : partnerStyle == 5 ? -0.02f : 0f;
        var fusionScale = fusionActive ? 0.24f : 0f;
        var stageScale = evolutionStage == 0 ? 0.62f : evolutionStage == 1 ? 0.8f : evolutionStage == 2 ? 0.98f : 1.18f;
        player.localScale = Vector3.one * (stageScale + styleScale + partnerScale + fusionScale);

        var glowColor = GetFormAccentColor();
        glowColor.a = fusionActive ? 0.24f : 0.16f;
        if (playerGlowRenderer != null)
        {
            playerGlowRenderer.color = glowColor;
            playerGlowRenderer.transform.localScale = Vector3.one * (2.0f + evolutionStage * 0.52f + (fusionActive ? 0.95f : 0f));
        }

        if (fusionAura != null)
        {
            fusionAura.gameObject.SetActive(fusionActive);
            fusionAuraRenderer.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0.18f);
            fusionAura.localScale = Vector3.one * (1.85f + evolutionStage * 0.32f);
        }

        if (shieldAura != null)
            shieldAura.gameObject.SetActive(orbitShield || fusionActive);

        if (burst)
        {
            SpawnSparks(player.position, glowColor, fusionActive ? 70 : 32, fusionActive ? 2.3f : 1.2f);
            Shake(fusionActive ? 0.42f : 0.2f, fusionActive ? 0.2f : 0.1f);
        }
    }

    void UpdatePlayerVisualEffects()
    {
        var accent = GetFormAccentColor();
        if (playerGlowRenderer != null)
        {
            var glow = accent;
            glow.a = (fusionActive ? 0.18f : 0.12f) + Mathf.Sin(Time.time * (fusionActive ? 8f : 5f)) * 0.035f;
            playerGlowRenderer.color = glow;
        }

        if (shieldAura != null)
        {
            shieldAura.gameObject.SetActive(orbitShield || fusionActive);
            shieldAura.Rotate(0, 0, Time.deltaTime * (fusionActive ? -90f : -36f));
            shieldAura.localScale = Vector3.one * (1.35f + Mathf.Sin(Time.time * 5f) * 0.08f);
            if (shieldAuraRenderer != null)
                shieldAuraRenderer.color = new Color(0.18f, 1f, 0.68f, orbitShield ? 0.11f : 0.04f);
        }

        if (fusionAura != null && fusionActive)
        {
            fusionAura.Rotate(0, 0, Time.deltaTime * 120f);
            fusionAura.localScale = Vector3.one * (1.55f + evolutionStage * 0.18f + Mathf.Sin(Time.time * 7f) * 0.08f);
            if (fusionAuraRenderer != null)
                fusionAuraRenderer.color = new Color(accent.r, accent.g, accent.b, 0.16f + Mathf.Sin(Time.time * 9f) * 0.04f);
        }

        UpdatePlayerEvolutionDecor(accent);
        UpdateLinkCompanions();
        UpdatePlayerMotionVisual();
    }

    void UpdatePlayerMotionVisual()
    {
        if (player == null || playerVisualBody == null || playerRenderer == null)
            return;

        var dt = Mathf.Max(0.0001f, Time.deltaTime);
        var delta = (Vector2)(player.position - lastPlayerVisualPosition);
        var instantVelocity = delta / dt;
        lastPlayerVisualPosition = player.position;
        playerVisualVelocity = Vector2.Lerp(playerVisualVelocity, instantVelocity, 1f - Mathf.Exp(-dt * 14f));
        playerAttackPulse = Mathf.MoveTowards(playerAttackPulse, 0f, dt * 6.8f);
        playerAfterimageTimer -= dt;
        playerFootstepTimer -= dt;

        var speed01 = Mathf.Clamp01(playerVisualVelocity.magnitude / Mathf.Max(0.1f, moveSpeed));
        if (speed01 > 0.02f)
            playerMotionPhase += dt * Mathf.Lerp(6.5f, 12.5f, speed01);
        else
            playerMotionPhase += dt * 2.2f;

        if (Mathf.Abs(aimDirection.x) > 0.12f)
            playerRenderer.flipX = aimDirection.x < 0f;

        if (!enhancedVisuals)
        {
            playerVisualBody.localPosition = Vector3.zero;
            playerVisualBody.localRotation = Quaternion.identity;
            playerVisualBody.localScale = Vector3.one;
            return;
        }

        var step = Mathf.Sin(playerMotionPhase);
        var lift = Mathf.Abs(step) * 0.045f * speed01;
        var lean = Mathf.Clamp(-playerVisualVelocity.x * 3.2f, -8f, 8f);
        var squash = step * 0.035f * speed01;
        var hitKick = Mathf.Clamp01(playerHitPulse) * 0.045f;
        var recoil = -aimDirection.normalized * (0.075f * playerAttackPulse);

        playerVisualBody.localPosition = new Vector3(-playerVisualVelocity.normalized.x * 0.028f * speed01 + recoil.x, lift + recoil.y * 0.55f, 0f);
        playerVisualBody.localRotation = Quaternion.Euler(0f, 0f, lean + step * 1.8f * speed01);
        playerVisualBody.localScale = new Vector3(1f + Mathf.Abs(squash) + hitKick + playerAttackPulse * 0.055f, 1f - squash * 0.55f + hitKick - playerAttackPulse * 0.025f, 1f);

        if (speed01 > 0.52f && playerAfterimageTimer <= 0f)
        {
            var accent = fusionActive || formStyle > 0 ? GetFormAccentColor() : new Color(0.34f, 0.9f, 1f);
            CreateSpriteGhost(playerRenderer, WithAlpha(accent, fusionActive ? 0.34f : 0.18f), fusionActive ? 0.24f : 0.16f, -playerVisualVelocity.normalized * 0.22f);
            playerAfterimageTimer = fusionActive || formStyle == 1 ? 0.07f : 0.12f;
        }

        if (speed01 > 0.32f && playerFootstepTimer <= 0f)
        {
            var strideColor = fusionActive ? GetFormAccentColor() : new Color(0.25f, 0.9f, 1f);
            SpawnStrideSparks(player.position, playerVisualVelocity.normalized, strideColor, 0.55f);
            playerFootstepTimer = 0.12f;
        }
    }

    void UpdatePlayerEvolutionDecor(Color accent)
    {
        var visible = enhancedVisuals && evolutionStage > 0 && player != null;
        SetEvolutionDecorActive(playerStageHalo, visible);
        SetEvolutionDecorActive(playerLeftAccent, visible);
        SetEvolutionDecorActive(playerRightAccent, visible);
        SetEvolutionDecorActive(playerCrestAccent, visible);
        if (!visible)
            return;

        var pulse = 0.5f + Mathf.Sin(Time.time * (fusionActive ? 8f : 5.8f)) * 0.5f;
        var stageScale = 0.72f + evolutionStage * 0.22f + (fusionActive ? 0.38f : 0f);
        playerStageHalo.localScale = Vector3.one * (stageScale + pulse * 0.06f);
        playerStageHalo.Rotate(0f, 0f, Time.deltaTime * (fusionActive ? 120f : 52f));
        if (playerStageHaloRenderer != null)
            playerStageHaloRenderer.color = new Color(accent.r, accent.g, accent.b, fusionActive ? 0.22f + pulse * 0.08f : 0.08f + pulse * 0.05f);

        var sideSprite = formStyle == 3 || fusionStyle == 3 ? squareSprite : diamondSprite;
        var sideY = formStyle == 1 || fusionStyle == 2 ? -0.04f : 0.06f;
        var sideX = fusionActive ? 0.68f : formStyle == 2 || fusionStyle == 3 ? 0.5f : 0.58f;
        var fusionBoost = fusionActive ? 0.12f : 0f;
        var sideScale = formStyle == 1 || fusionStyle == 2
            ? new Vector3(0.18f + evolutionStage * 0.025f + fusionBoost, 0.56f + evolutionStage * 0.06f + fusionBoost * 1.6f, 1f)
            : formStyle == 3 || fusionStyle == 3
                ? new Vector3(0.24f + evolutionStage * 0.035f + fusionBoost, 0.42f + evolutionStage * 0.04f + fusionBoost * 1.2f, 1f)
                : new Vector3(0.26f + evolutionStage * 0.04f + fusionBoost, 0.32f + evolutionStage * 0.035f + fusionBoost, 1f);
        ConfigureEvolutionAccent(playerLeftAccent, playerLeftAccentRenderer, sideSprite, new Vector3(-sideX, sideY, -0.16f), sideScale, accent, 0.68f + pulse * 0.16f, -18f);
        ConfigureEvolutionAccent(playerRightAccent, playerRightAccentRenderer, sideSprite, new Vector3(sideX, sideY, -0.16f), sideScale, accent, 0.68f + pulse * 0.16f, 18f);

        var crestSprite = fusionActive ? diamondSprite : formStyle == 3 ? squareSprite : diamondSprite;
        var crestColor = fusionActive ? Color.Lerp(accent, new Color(1f, 0.38f, 0.96f), 0.4f) : formStyle == 2 ? new Color(1f, 0.84f, 0.24f) : accent;
        ConfigureEvolutionAccent(playerCrestAccent, playerCrestAccentRenderer, crestSprite, new Vector3(0f, -0.56f - evolutionStage * 0.025f, -0.17f), new Vector3(0.16f + evolutionStage * 0.035f, 0.24f + evolutionStage * 0.055f, 1f), crestColor, 0.82f + pulse * 0.16f, 0f);
    }

    void SetEvolutionDecorActive(Transform target, bool active)
    {
        if (target != null)
            target.gameObject.SetActive(active);
    }

    void ConfigureEvolutionAccent(Transform target, SpriteRenderer renderer, Sprite sprite, Vector3 position, Vector3 scale, Color color, float alpha, float zRotation)
    {
        if (target == null || renderer == null)
            return;

        renderer.sprite = sprite;
        renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha));
        target.localPosition = position;
        target.localScale = scale;
        target.localRotation = Quaternion.Euler(0f, 0f, zRotation);
    }

    void UpdateLinkCompanions()
    {
        UpdateLinkCompanion(0, allyNova, new Color(0.35f, 0.95f, 1f));
        UpdateLinkCompanion(1, allyBulwark, new Color(0.42f, 1f, 0.56f));
        UpdateLinkCompanion(2, allySiphon, new Color(0.95f, 1f, 0.35f));
        UpdateLinkCompanion(3, allyPhase, new Color(0.78f, 0.48f, 1f));
        UpdateBulwarkDefense();
        UpdateSiphonCollection();
        UpdatePhaseDodge();
        UpdateFunnelBits();
        UpdateLaser();
    }

    void UpdateLinkCompanion(int index, bool active, Color color)
    {
        if (linkCompanions[index] == null)
            return;

        var visible = active && !fusionActive;
        linkCompanions[index].gameObject.SetActive(visible);
        if (linkCompanionGlows[index] != null)
            linkCompanionGlows[index].gameObject.SetActive(visible);
        if (!visible)
            return;

        var bob = Mathf.Sin(Time.time * 5.2f + index) * 0.06f;
        var target = GetLinkRoleTarget(index, bob);
        linkCompanions[index].position = Vector3.Lerp(linkCompanions[index].position, target, Time.deltaTime * (index == 2 ? 10.5f : 13f));

        var facing = index == 1 ? (Vector2)(lantern.position - linkCompanions[index].position) : aimDirection;
        if (index == 0)
        {
            var enemy = FindNearestEnemy();
            if (enemy != null)
                facing = (Vector2)(enemy.transform.position - linkCompanions[index].position);
        }
        else if (index == 2)
        {
            var pickup = FindNearestPickupFrom(linkCompanions[index].position, 9f);
            if (pickup != null)
                facing = (Vector2)(pickup.transform.position - linkCompanions[index].position);
        }
        var tilt = Mathf.Clamp(facing.x, -1f, 1f) * -10f + Mathf.Sin(Time.time * 4f + index) * 5f;
        linkCompanions[index].rotation = Quaternion.Euler(0, 0, tilt);
        linkCompanions[index].localScale = Vector3.one * (fusionActive ? 0.62f : 0.52f);

        var pulse = 0.72f + Mathf.Sin(Time.time * 7f + index) * 0.28f;
        if (linkCompanionRenderers[index] != null)
            linkCompanionRenderers[index].color = Color.Lerp(Color.white, color, fusionActive ? 0.36f : 0.18f);
        if (linkCompanionGlows[index] != null)
        {
            linkCompanionGlows[index].position = linkCompanions[index].position + Vector3.forward * 0.07f;
            linkCompanionGlows[index].localScale = Vector3.one * (fusionActive ? 0.9f + pulse * 0.2f : 0.68f + pulse * 0.12f);
        }
        if (linkCompanionGlowRenderers[index] != null)
            linkCompanionGlowRenderers[index].color = new Color(color.r, color.g, color.b, fusionActive ? 0.22f + pulse * 0.08f : 0.12f + pulse * 0.06f);
    }

    Vector3 GetLinkRoleTarget(int index, float bob)
    {
        if (index == 0)
        {
            var enemy = FindNearestEnemy();
            if (enemy != null)
            {
                var direction = ((Vector2)(enemy.transform.position - player.position)).normalized;
                var side = Rotate(direction, 90f) * 0.38f;
                return player.position + (Vector3)(direction * 0.88f + side) + new Vector3(0, bob, -0.2f);
            }
            return player.position + (Vector3)(aimDirection.normalized * 0.78f + Rotate(aimDirection.normalized, 90f) * 0.34f) + new Vector3(0, bob, -0.2f);
        }

        if (index == 1)
        {
            var threat = FindCoreThreat();
            if (threat != null)
            {
                var direction = ((Vector2)(threat.transform.position - lantern.position)).normalized;
                return lantern.position + (Vector3)(direction * 0.86f) + new Vector3(0, bob, -0.22f);
            }
            return lantern.position + new Vector3(0.72f, -0.36f + bob, -0.22f);
        }

        if (index == 2)
        {
            var pickup = FindNearestPickupFrom(player.position, fusionStyle == 2 ? 10f : 7.5f);
            if (pickup != null)
                return pickup.transform.position + new Vector3(0, bob, -0.21f);
            return Vector3.Lerp(player.position, lantern.position, 0.35f) + new Vector3(-0.72f, -0.28f + bob, -0.21f);
        }

        var orbitAngle = Time.time * (fusionStyle == 4 || fusionStyle == 5 || fusionStyle == 6 ? 4.2f : 2.6f);
        var orbitRadius = fusionActive ? 1.08f : 0.86f;
        var orbitX = Mathf.Cos(orbitAngle) * orbitRadius;
        var orbitY = Mathf.Sin(orbitAngle) * orbitRadius;
        return player.position + new Vector3(orbitX, orbitY + bob, -0.23f);
    }

    Enemy FindCoreThreat()
    {
        Enemy best = null;
        var bestDistance = 4.8f * 4.8f;
        foreach (var enemy in enemies)
        {
            var distance = ((Vector2)(enemy.transform.position - lantern.position)).sqrMagnitude;
            if (distance < bestDistance)
            {
                best = enemy;
                bestDistance = distance;
            }
        }
        return best;
    }

    Pickup FindNearestPickupFrom(Vector3 position, float maxDistance)
    {
        Pickup best = null;
        var bestDistance = maxDistance * maxDistance;
        foreach (var pickup in pickups)
        {
            if (pickup.transform == null)
                continue;

            var distance = ((Vector2)(pickup.transform.position - position)).sqrMagnitude;
            if (distance < bestDistance)
            {
                best = pickup;
                bestDistance = distance;
            }
        }
        return best;
    }

    void UpdateSiphonCollection()
    {
        if (!allySiphon || linkCompanions[2] == null || !linkCompanions[2].gameObject.activeSelf)
            return;

        var siphonPosition = linkCompanions[2].position;
        var pullRange = pickupRange * (fusionStyle == 2 ? 1.08f : 0.72f);
        for (var i = pickups.Count - 1; i >= 0; i--)
        {
            var pickup = pickups[i];
            var distance = Vector2.Distance(pickup.transform.position, siphonPosition);
            if (distance < pullRange)
            {
                var direction = ((Vector2)(siphonPosition - pickup.transform.position)).normalized;
                pickup.transform.position += (Vector3)(direction * Time.deltaTime * Mathf.Lerp(1.6f, 6.5f, 1f - distance / pullRange));
            }

            if (distance < 0.24f)
                CollectPickupAt(i, fusionStyle == 2 ? 1.04f : 1f);
        }
    }

    // ── Funnel bit system (Halo Caster) ─────────────────────────
    // Spawns N orbital drones around the player. Each drone auto-fires
    // weaker bullets at the nearest enemy on its own cooldown.
    void EnsureFunnelBits(int count, Color color)
    {
        funnelBitCount = Mathf.Clamp(count, 0, FunnelMaxBits);
        for (var i = 0; i < FunnelMaxBits; i++)
        {
            if (i < funnelBitCount)
            {
                if (funnelBitTransforms[i] == null)
                {
                    var go = CreateSpriteObject("Funnel Bit " + i, diamondSprite, Vector3.zero, color, 0.20f);
                    funnelBitTransforms[i] = go.transform;
                }
                else
                {
                    funnelBitTransforms[i].gameObject.SetActive(true);
                    var sr = funnelBitTransforms[i].GetComponent<SpriteRenderer>();
                    if (sr != null) sr.color = color;
                }
                // Stagger initial fire timing so bits don't all shoot at the same instant
                funnelBitTimers[i] = i * 0.18f;
                funnelBitStates[i] = FunnelBitState.Orbit;
                funnelBitTargets[i] = null;
            }
            else if (funnelBitTransforms[i] != null)
            {
                funnelBitTransforms[i].gameObject.SetActive(false);
                funnelBitStates[i] = FunnelBitState.Orbit;
                funnelBitTargets[i] = null;
            }
        }
        funnelDetachTimer = 4f;
    }

    void UpdateFunnelBits()
    {
        if (funnelBitCount <= 0 || player == null) return;

        funnelBitAngleBase += Time.deltaTime * 1.4f;  // orbit rotation speed (rad/s)
        var twoPi = Mathf.PI * 2f;
        var tNow = Time.time;

        // ── Detach attack scheduling ──
        // Every funnelDetachInterval sec, ALL orbiting bits launch as a coordinated salvo.
        // Each bit picks its OWN nearest enemy (true multi-target wave), with a slight
        // per-bit stagger so the visual reads as a sweeping barrage rather than a single
        // blob. Timer only resets when at least one bit launches.
        funnelDetachTimer -= Time.deltaTime;
        if (funnelDetachTimer <= 0f && !AnyBitDetached())
        {
            var primary = FindNearestEnemy();
            if (primary != null && primary.transform != null
                && Vector2.Distance(primary.transform.position, player.position) <= FunnelDetachMaxRange)
            {
                var launched = 0;
                for (var k = 0; k < funnelBitCount; k++)
                {
                    if (funnelBitStates[k] != FunnelBitState.Orbit) continue;
                    var bitPos = funnelBitTransforms[k] != null ? (Vector2)funnelBitTransforms[k].position : (Vector2)player.position;
                    // Each bit picks the enemy nearest to ITSELF (spreads attacks across the field).
                    var bitTarget = FindNearestEnemyFrom(bitPos, null, FunnelDetachMaxRange);
                    if (bitTarget == null) bitTarget = primary;
                    funnelBitStates[k] = FunnelBitState.ChargeOut;
                    funnelBitTargets[k] = bitTarget;
                    // Repurpose timer as wave-stagger entry delay (70ms per bit).
                    funnelBitTimers[k] = launched * 0.07f;
                    if (funnelBitTransforms[k] != null)
                        SpawnSparks(funnelBitTransforms[k].position, new Color(0.65f, 0.95f, 1f, 1f), 8, 0.55f);
                    launched++;
                }
                if (launched > 0)
                {
                    // One unified salvo cue rather than per-bit micro-flashes
                    Flash(new Color(0.55f, 0.92f, 1f, 0.20f), 0.20f);
                    Shake(0.13f, 0.10f);
                    PlaySfx("Hit", 380f, 0.12f, 0.22f);
                    funnelDetachTimer = funnelDetachInterval;
                }
            }
        }

        // ── Per-bit update ──
        for (var i = 0; i < funnelBitCount; i++)
        {
            var bit = funnelBitTransforms[i];
            if (bit == null) continue;

            // Compute "home" orbit position with per-bit independent wobble.
            // Each bit has its own breathing radius + slight angular drift so the
            // ring doesn't read as a rigid rotating star.
            var phase = i * 1.7f;
            var radiusWobble = funnelOrbitRadius * (1f + 0.10f * Mathf.Sin(tNow * 1.6f + phase));
            var angleWobble  = funnelBitAngleBase + (twoPi * i / funnelBitCount) + 0.10f * Mathf.Sin(tNow * 0.9f + phase * 1.4f);
            var orbitPos = (Vector2)player.position + new Vector2(Mathf.Cos(angleWobble), Mathf.Sin(angleWobble)) * radiusWobble;

            switch (funnelBitStates[i])
            {
                case FunnelBitState.Orbit:
                {
                    bit.position = new Vector3(orbitPos.x, orbitPos.y, -0.1f);
                    bit.Rotate(0f, 0f, Time.deltaTime * 280f);

                    funnelBitTimers[i] -= Time.deltaTime;
                    if (funnelBitTimers[i] > 0f) break;

                    // Each bit fires at the enemy nearest to ITSELF (not the global nearest).
                    // This makes bits on different sides of the orbit hit different targets,
                    // giving the swarm a true independent feel rather than rigid synchronized fire.
                    var fireTarget = FindNearestEnemyFrom(bit.position, null, 12f);
                    if (fireTarget == null || fireTarget.transform == null) break;
                    var fireDir = ((Vector2)(fireTarget.transform.position - bit.position)).normalized;
                    if (fireDir.sqrMagnitude < 0.01f) fireDir = Vector2.up;
                    SpawnFunnelBullet(bit.position, fireDir);
                    funnelBitTimers[i] = funnelBitFireRate;
                    break;
                }

                case FunnelBitState.ChargeOut:
                {
                    // Wave-stagger entry delay: hold position with a visible wind-up spin
                    if (funnelBitTimers[i] > 0f)
                    {
                        funnelBitTimers[i] -= Time.deltaTime;
                        bit.position = new Vector3(orbitPos.x, orbitPos.y, -0.1f);
                        bit.Rotate(0f, 0f, Time.deltaTime * 1100f);  // visible charging spin
                        break;
                    }

                    var tgt = funnelBitTargets[i];
                    // Target might have died  Etry to retarget nearest before giving up
                    if (tgt == null || tgt.transform == null || !enemies.Contains(tgt))
                    {
                        var fallback = FindNearestEnemyFrom(bit.position, null, FunnelDetachMaxRange);
                        if (fallback == null)
                        {
                            funnelBitStates[i] = FunnelBitState.ChargeReturn;
                            funnelBitTargets[i] = null;
                            break;
                        }
                        tgt = fallback;
                        funnelBitTargets[i] = fallback;
                    }

                    var toTarget = (Vector2)tgt.transform.position - (Vector2)bit.position;
                    var dist = toTarget.magnitude;
                    var moveDir = dist > 0.01f ? toTarget / dist : Vector2.up;
                    bit.position += (Vector3)(moveDir * FunnelDetachSpeed * Time.deltaTime);
                    bit.Rotate(0f, 0f, Time.deltaTime * 720f);

                    if (dist <= FunnelDetachHitRadius)
                    {
                        // IMPACT  Ebig damage + visual
                        var impactDamage = bulletDamage * funnelDetachDamageMultiplier;
                        DamageEnemy(tgt, impactDamage, true);
                        var hitColor = new Color(0.7f, 0.96f, 1f);
                        SpawnSparks(bit.position, hitColor, 18, 1.1f);
                        SpawnRingSparks(bit.position, hitColor, 14, 0.6f);
                        Flash(new Color(0.6f, 0.95f, 1f, 0.18f), 0.18f);
                        Shake(0.10f, 0.05f);
                        HitFreeze(0.06f);
                        PlaySfx("Hit", 320f, 0.10f, 0.24f);
                        CreateFloatingText("+" + Mathf.CeilToInt(impactDamage), bit.position + Vector3.up * 0.32f, new Color(1f, 0.95f, 0.55f), 0.15f);

                        // CASCADE STORM 進匁E 命中時に追加範囲ダメージ
                        if (cascadeStormSplashRadius > 0f)
                        {
                            SplashDamage(bit.position, cascadeStormSplashRadius, impactDamage * 0.55f, tgt);
                            SpawnRingSparks(bit.position, new Color(0.85f, 1f, 1f), 18, cascadeStormSplashRadius);
                        }

                        funnelBitStates[i] = FunnelBitState.ChargeReturn;
                        funnelBitTargets[i] = null;
                    }
                    break;
                }

                case FunnelBitState.ChargeReturn:
                {
                    var toHome = orbitPos - (Vector2)bit.position;
                    var distHome = toHome.magnitude;
                    if (distHome <= 0.18f)
                    {
                        // Snap home, resume orbit
                        bit.position = new Vector3(orbitPos.x, orbitPos.y, -0.1f);
                        funnelBitStates[i] = FunnelBitState.Orbit;
                        // Slight cooldown after return so it doesn't immediately fire
                        funnelBitTimers[i] = funnelBitFireRate * 0.5f;
                    }
                    else
                    {
                        var moveDir = distHome > 0.01f ? toHome / distHome : Vector2.up;
                        bit.position += (Vector3)(moveDir * FunnelDetachReturnSpeed * Time.deltaTime);
                        bit.Rotate(0f, 0f, Time.deltaTime * 460f);
                    }
                    break;
                }
            }
        }
    }

    bool AnyBitDetached()
    {
        for (var i = 0; i < funnelBitCount; i++)
            if (funnelBitStates[i] != FunnelBitState.Orbit) return true;
        return false;
    }

    // ── Pulse Hydra laser system ──────────────────────────────
    // Creates or shows/hides the beam transform.
    void EnsureLaser(bool active, Color color)
    {
        laserActive = active;
        laserBeamColor = color;
        if (active)
        {
            if (laserBeamTransform == null)
            {
                var go = CreateSpriteObject("Laser Beam", squareSprite, Vector3.zero, color, 1f);
                laserBeamTransform = go.transform;
                laserBeamRenderer = go.GetComponent<SpriteRenderer>();
                laserBeamRenderer.sortingOrder = 5;  // above floor, below bullets/effects
            }
            laserBeamTransform.gameObject.SetActive(true);
            if (laserBeamRenderer != null) laserBeamRenderer.color = color;
            laserTickTimer = 0f;
        }
        else if (laserBeamTransform != null)
        {
            laserBeamTransform.gameObject.SetActive(false);
        }
    }

    void UpdateLaser()
    {
        if (!laserActive || laserBeamTransform == null || player == null) return;

        // Find nearest enemy *within laser range* (FindNearestEnemy is unbounded  Ewould let beam
        // visually clip at maxLength while still applying damage to far target. Bug fix.)
        Enemy target = null;
        var bestSqr = laserMaxLength * laserMaxLength;
        var px = (Vector2)player.position;
        for (var i = 0; i < enemies.Count; i++)
        {
            var e = enemies[i];
            if (e == null || e.transform == null) continue;
            var sqr = ((Vector2)e.transform.position - px).sqrMagnitude;
            if (sqr <= bestSqr)
            {
                bestSqr = sqr;
                target = e;
            }
        }
        var hasValidTarget = target != null;

        if (!hasValidTarget)
        {
            if (laserBeamRenderer != null)
            {
                var c = laserBeamRenderer.color;
                c.a = 0f;
                laserBeamRenderer.color = c;
            }
            return;
        }

        // Compute beam geometry: player →target
        var start = (Vector2)player.position;
        var end = (Vector2)target.transform.position;
        var delta = end - start;
        var dist = delta.magnitude;
        if (dist < 0.05f) return;
        var length = dist;  // target already within range, so no clamp needed
        var normDir = delta / dist;
        var beamEnd = end;

        // Position beam at midpoint, stretch to length, rotate to face direction
        var mid = start + normDir * (length * 0.5f);
        laserBeamTransform.position = new Vector3(mid.x, mid.y, -0.05f);
        laserBeamTransform.localScale = new Vector3(length, 0.22f, 1f);
        var angle = Mathf.Atan2(normDir.y, normDir.x) * Mathf.Rad2Deg;
        laserBeamTransform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Pulsing alpha for "active beam" feel
        if (laserBeamRenderer != null)
        {
            var pulse = 0.78f + Mathf.Sin(Time.time * 16f) * 0.2f;
            var c = laserBeamColor;
            c.a = pulse;
            laserBeamRenderer.color = c;
        }

        // Tick damage
        laserTickTimer -= Time.deltaTime;
        if (laserTickTimer > 0f) return;
        laserTickTimer = LaserTickInterval;
        var tickDamage = bulletDamage * laserDamageMultiplier * LaserTickInterval;

        // Primary target = full damage
        DamageEnemy(target, tickDamage, false);
        SpawnSparks(target.transform.position, laserBeamColor, 3, 0.45f);

        // ── Line damage: also hit any enemy intersecting the beam line ──
        // beam half-width = 0.30m. Chained enemies get laserChainDamageMul ×primary damage.
        const float beamHalfWidth = 0.30f;
        for (var i = enemies.Count - 1; i >= 0; i--)
        {
            if (i >= enemies.Count) continue;
            var e = enemies[i];
            if (e == null || e == target || e.transform == null) continue;
            var ep = (Vector2)e.transform.position;
            var d = DistancePointToSegment(ep, start, beamEnd);
            if (d > beamHalfWidth) continue;
            DamageEnemy(e, tickDamage * laserChainDamageMul, false);
            SpawnSparks(ep, laserBeamColor, 2, 0.32f);
        }
    }

    // Shortest distance from point p to line segment a→b
    static float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        var ab = b - a;
        var sqr = ab.sqrMagnitude;
        if (sqr < 0.0001f) return Vector2.Distance(p, a);
        var t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / sqr);
        var closest = a + ab * t;
        return Vector2.Distance(p, closest);
    }

    void SpawnFunnelBullet(Vector3 origin, Vector2 direction)
    {
        var color = new Color(0.55f, 0.92f, 1f);
        var go = CreateSpriteObject("Funnel Bolt", diamondSprite, origin, color, 0.16f);
        go.transform.rotation = DirectionRotationRight(direction);
        EnforceBulletCap();
        bullets.Add(new Bullet
        {
            transform = go.transform,
            direction = direction,
            damage = bulletDamage * 0.80f,   // 0.5 →0.8 (火力強匁E
            life = 0.9f,
            pierce = 0,
            fromEnemy = false,
            speedMultiplier = 1.55f,
            hitRadius = 0.34f,
            style = 1,
            chainJumps = 0,
            chainDamageMultiplier = 0.4f
        });
    }

    void UpdatePhaseDodge()
    {
        if (!allyPhase && fusionStyle != 4 && fusionStyle != 5 && fusionStyle != 6)
            return;

        phaseDodgeTimer -= Time.deltaTime;
        if (phaseDodgeTimer > 0f)
            return;

        var radius = fusionStyle == 4 || fusionStyle == 5 || fusionStyle == 6 ? 1.4f : 1.05f;
        for (var i = bullets.Count - 1; i >= 0; i--)
        {
            var bullet = bullets[i];
            if (!bullet.fromEnemy)
                continue;
            if (Vector2.Distance(bullet.transform.position, player.position) > radius)
                continue;

            var blinkColor = new Color(0.85f, 0.5f, 1f);
            SpawnSparks(bullet.transform.position, blinkColor, 6, 0.55f);
            CreateFloatingText("PHASE", bullet.transform.position + Vector3.up * 0.24f, blinkColor, 0.085f);
            Destroy(bullet.transform.gameObject);
            bullets.RemoveAt(i);
            phaseDodgeTimer = fusionActive ? 0.65f : 1.1f;
            playerHp = Mathf.Min(playerMaxHp, playerHp + (fusionActive ? 0.25f : 0.12f));
            return;
        }
    }

    void UpdateBulwarkDefense()
    {
        if (!allyBulwark || linkCompanions[1] == null || !linkCompanions[1].gameObject.activeSelf)
            return;

        lanternHp = Mathf.Min(lanternMaxHp, lanternHp + Time.deltaTime * (fusionStyle == 3 ? 0.065f : 0.018f));
        var defenseRadius = fusionStyle == 3 ? 1.18f : 0.78f;
        var guardPosition = linkCompanions[1].position;
        for (var i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];
            if (Vector2.Distance(enemy.transform.position, guardPosition) > defenseRadius)
                continue;

            var away = ((Vector2)(enemy.transform.position - lantern.position)).normalized;
            enemy.transform.position += (Vector3)(away * Time.deltaTime * 0.28f);
            DamageEnemy(enemy, Time.deltaTime * orbitDamage * (fusionStyle == 3 ? 0.42f : 0.18f), false);
        }

        for (var i = bullets.Count - 1; i >= 0; i--)
        {
            var bullet = bullets[i];
            if (!bullet.fromEnemy || Vector2.Distance(bullet.transform.position, guardPosition) > defenseRadius)
                continue;

            ConvertEnemyBulletToFriendly(bullet, new Color(0.42f, 1f, 0.56f), bulletDamage * 0.22f);
            SpawnSparks(guardPosition, new Color(0.42f, 1f, 0.56f), 5, 0.34f);
        }
    }

    Color GetFormAccentColor()
    {
        if (fusionStyle > 0)
            return GetFusionAccentColor(fusionStyle);
        if (formStyle == 1)
            return new Color(0.32f, 1f, 1f, 1f);
        if (formStyle == 2)
            return new Color(1f, 0.58f, 0.16f, 1f);
        if (formStyle == 3)
            return new Color(0.42f, 1f, 0.62f, 1f);
        return partnerStyle > 0 ? GetPartnerAccentColor(partnerStyle) : new Color(0.45f, 1f, 1f, 1f);
    }

    Color GetBulletColor()
    {
        var accent = GetFormAccentColor();
        return Color.Lerp(new Color(0.78f, 0.95f, 1f), accent, fusionActive ? 0.65f : 0.28f);
    }

    int GetFusionStyle(string currentFusion)
    {
        if (currentFusion == "Nova Aegis")
            return 1;
        if (currentFusion == "Photon Siphon")
            return 2;
        if (currentFusion == "Core Bastion")
            return 3;
        if (currentFusion == "Nova Phantom")
            return 4;
        if (currentFusion == "Aegis Drift")
            return 5;
        if (currentFusion == "Photon Wraith")
            return 6;
        return 0;
    }

    void CheckSynergies()
    {
        if (!scatterSynergy && bulletCount >= 4 && fireRate >= 3.2f)
        {
            scatterSynergy = true;
            fireRate *= 1.15f;
            bulletLifeMultiplier *= 1.15f;
            AnnounceSynergy("OVERDRIVE: Scatter Storm");
        }

        if (!lanceSynergy && bulletPierce >= 2 && bulletDamage >= 2.0f)
        {
            lanceSynergy = true;
            bossDamageMultiplier *= 1.35f;
            bulletSpeed *= 1.12f;
            AnnounceSynergy("OVERDRIVE: Core Lance");
        }

        if (!recoverySynergy && pickupHealChance > 0f && dataMultiplier >= 1.25f)
        {
            recoverySynergy = true;
            pickupRange *= 1.25f;
            playerMaxHp += 2f;
            playerHp = Mathf.Min(playerMaxHp, playerHp + 3f);
            AnnounceSynergy("OVERDRIVE: Regen Loop");
        }

        if (!aegisSynergy && orbitShield && lanternMaxHp >= 25f)
        {
            aegisSynergy = true;
            orbitDamage *= 1.5f;
            lightRadius *= 1.12f;
            lantern.GetChild(0).localScale = Vector3.one * lightRadius * 1.75f;
            AnnounceSynergy("OVERDRIVE: Aegis Ring");
        }

        CheckEvolutions();
        CheckFusion();
    }

    // ─────────────────────────────────────────────────────────
    // Evolution combos  EVampire Survivors 風の「特定モジュール絁E��合わせ」で上位融合解放
    // 吁E��E��合わせ�E run 冁E��1度だけ発動。\nnounceEvolution が派手な演�Eで通知する、E    // 発動した絁E��合わせ�E PlayerPrefs に永続記録され、コンボ図鑑で確認可能、E    // ─────────────────────────────────────────────────────────
    sealed class EvolutionComboRecipe
    {
        public readonly string key;
        public readonly string displayName;
        public readonly string description;
        public readonly string[] requirements;
        public readonly Color accent;
        public EvolutionComboRecipe(string key, string displayName, string description, string[] requirements, Color accent)
        {
            this.key = key; this.displayName = displayName; this.description = description;
            this.requirements = requirements; this.accent = accent;
        }
    }

    const string ComboKeyStellarHalo     = "Combo_StellarHalo";
    const string ComboKeyPhotonLance     = "Combo_PhotonLance";
    const string ComboKeyHyperSwarm      = "Combo_HyperSwarm";
    const string ComboKeyCascadeStorm    = "Combo_CascadeStorm";
    const string ComboKeyCrimsonHunter   = "Combo_CrimsonHunter";
    const string ComboKeyCriticalCascade = "Combo_CriticalCascade";
    const string ComboKeyBurstStorm      = "Combo_BurstStorm";
    const string ComboKeyDataHarvester   = "Combo_DataHarvester";

    EvolutionComboRecipe[] BuildEvolutionComboRecipes()
    {
        return new[]
        {
            new EvolutionComboRecipe(ComboKeyStellarHalo, "STELLAR HALO",
                "オーラDPS +6 / 半径+0.5m / 2秒毎にショックウェーブ発生",
                new[] { "プラズマオーラ", "フィールドプロジェクター", "インフェルノハロー" },
                new Color(1f, 0.78f, 0.32f)),
            new EvolutionComboRecipe(ComboKeyPhotonLance, "PHOTON LANCE",
                "レーザーDPS x1.6 / 射程+5m / 副次ダメ 100%",
                new[] { "フォトンサージ", "スプリットビーム", "ホライゾンレンズ" },
                new Color(1f, 0.45f, 0.92f)),
            new EvolutionComboRecipe(ComboKeyHyperSwarm, "HYPER SWARM",
                "ファンネル発射間隔 -30% / 弾dmg +10% / 貫通+1",
                new[] { "スウォームプロトコル", "ハイパーサイクル" },
                new Color(0.55f, 0.95f, 1f)),
            new EvolutionComboRecipe(ComboKeyCascadeStorm, "CASCADE STORM",
                "突撃dmg ×1.5 / クール -20% / 命中時0.85m範囲ダメ",
                new[] { "ストライクキャスケード", "キネティックスパイク" },
                new Color(0.70f, 1f, 1f)),
            new EvolutionComboRecipe(ComboKeyCrimsonHunter, "CRIMSON HUNTER",
                "吸血 ×1.5 / kill heal +0.5 / ボスdmg +25%",
                new[] { "ヴァンパイアコア", "HP吸収弾", "鎧貫通" },
                new Color(1f, 0.32f, 0.42f)),
            new EvolutionComboRecipe(ComboKeyCriticalCascade, "CRITICAL CASCADE",
                "クリ率+10% / 貫通+1 / 弾dmg +15%",
                new[] { "クリティカルコード", "波紋弾", "鎧貫通" },
                new Color(1f, 0.96f, 0.32f)),
            new EvolutionComboRecipe(ComboKeyBurstStorm, "BURST STORM",
                "爆発確率+20% / 弾速+10% / 弾dmg +10%",
                new[] { "波動砲", "クラッシュバースト", "超加速弾" },
                new Color(1f, 0.58f, 0.12f)),
            new EvolutionComboRecipe(ComboKeyDataHarvester, "DATA HARVESTER",
                "データ獲得×1.3 / 回収範囲×1.3 / kill bonus +1.0",
                new[] { "データフォージ", "データ圧縮", "マグネット回収" },
                new Color(0.40f, 1f, 0.65f)),
        };
    }

    bool IsComboDiscovered(string key) => PlayerPrefs.GetInt(key, 0) == 1;

    void MarkComboDiscovered(string key)
    {
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }

    int CountDiscoveredCombos()
    {
        var n = 0;
        var recipes = BuildEvolutionComboRecipes();
        for (var i = 0; i < recipes.Length; i++)
            if (IsComboDiscovered(recipes[i].key)) n++;
        return n;
    }

    bool HasPickedModule(string title)
    {
        return runModulePicks.ContainsKey(title);
    }

    void CheckEvolutions()
    {
        // 1. STELLAR HALO  Eオーラ3点合佁E(Wraith Lynx 系プレイの完�E形)
        if (!evoStellarHalo
            && HasPickedModule("プラズマオーラ")
            && HasPickedModule("フィールドプロジェクター")
            && HasPickedModule("インフェルノハロー"))
        {
            evoStellarHalo = true;
            playerAuraDamage += 6.0f;
            playerAuraRadius += 0.5f;
            stellarHaloPulseTimer = StellarHaloPulseInterval;
            MarkComboDiscovered(ComboKeyStellarHalo);
            AnnounceEvolution("EVOLUTION: STELLAR HALO", new Color(1f, 0.78f, 0.32f));
        }

        // 2. PHOTON LANCE  EPulse Hydra 系レーザー完�E形
        if (!evoPhotonLance
            && HasPickedModule("フォトンサージ")
            && HasPickedModule("スプリットビーム")
            && HasPickedModule("ホライゾンレンズ"))
        {
            evoPhotonLance = true;
            laserDamageMultiplier *= 1.60f;
            laserMaxLength += 5.0f;
            laserChainDamageMul = 1.0f;
            MarkComboDiscovered(ComboKeyPhotonLance);
            AnnounceEvolution("EVOLUTION: PHOTON LANCE", new Color(1f, 0.45f, 0.92f));
        }

        // 3. HYPER SWARM  EHalo Caster 常駐火力ビルド�E完�E形
        if (!evoHyperSwarm
            && HasPickedModule("スウォームプロトコル")
            && HasPickedModule("ハイパーサイクル"))
        {
            evoHyperSwarm = true;
            funnelBitFireRate *= 0.70f;
            bulletDamage *= 1.10f;
            bulletPierce++;
            MarkComboDiscovered(ComboKeyHyperSwarm);
            AnnounceEvolution("EVOLUTION: HYPER SWARM", new Color(0.55f, 0.95f, 1f));
        }

        // 4. CASCADE STORM  EHalo Caster 突撃特化ビルド�E完�E形
        if (!evoCascadeStorm
            && HasPickedModule("ストライクキャスケード")
            && HasPickedModule("キネティックスパイク"))
        {
            evoCascadeStorm = true;
            funnelDetachDamageMultiplier *= 1.50f;
            funnelDetachInterval *= 0.80f;
            cascadeStormSplashRadius = 0.85f;
            MarkComboDiscovered(ComboKeyCascadeStorm);
            AnnounceEvolution("EVOLUTION: CASCADE STORM", new Color(0.70f, 1f, 1f));
        }

        // 5. CRIMSON HUNTER  E吸血+貫通ハイブリチE��
        if (!evoCrimsonHunter
            && HasPickedModule("ヴァンパイアコア")
            && HasPickedModule("HP吸収弾")
            && HasPickedModule("鎧貫通"))
        {
            evoCrimsonHunter = true;
            lifestealAmount *= 1.5f;
            lifestealKillBonus += 0.5f;
            bossDamageMultiplier *= 1.25f;
            MarkComboDiscovered(ComboKeyCrimsonHunter);
            AnnounceEvolution("EVOLUTION: CRIMSON HUNTER", new Color(1f, 0.32f, 0.42f));
        }

        // 6. CRITICAL CASCADE  Eクリ+ピアス+衝撃波の連鎖型
        if (!evoCriticalCascade
            && HasPickedModule("クリティカルコード")
            && HasPickedModule("波紋弾")
            && HasPickedModule("鎧貫通"))
        {
            evoCriticalCascade = true;
            critChance = Mathf.Min(0.9f, critChance + 0.10f);
            bulletPierce++;
            bulletDamage *= 1.15f;
            MarkComboDiscovered(ComboKeyCriticalCascade);
            AnnounceEvolution("EVOLUTION: CRITICAL CASCADE", new Color(1f, 0.96f, 0.32f));
        }

        // 7. BURST STORM  E爆発系の完�E形
        if (!evoBurstStorm
            && HasPickedModule("波動砲")
            && HasPickedModule("クラッシュバースト")
            && HasPickedModule("超加速弾"))
        {
            evoBurstStorm = true;
            explodeChance = Mathf.Min(0.75f, explodeChance + 0.20f);
            bulletSpeed *= 1.10f;
            bulletDamage *= 1.10f;
            MarkComboDiscovered(ComboKeyBurstStorm);
            AnnounceEvolution("EVOLUTION: BURST STORM", new Color(1f, 0.58f, 0.12f));
        }

        // 8. DATA HARVESTER  E経済特化�E完�E形 (Brotato Greed 風)
        if (!evoDataHarvester
            && HasPickedModule("データフォージ")
            && HasPickedModule("データ圧縮")
            && HasPickedModule("マグネット回収"))
        {
            evoDataHarvester = true;
            dataMultiplier *= 1.30f;
            pickupRange *= 1.30f;
            bonusDataOnKill += 1.0f;
            MarkComboDiscovered(ComboKeyDataHarvester);
            AnnounceEvolution("EVOLUTION: DATA HARVESTER", new Color(0.40f, 1f, 0.65f));
        }
    }

    void AnnounceEvolution(string title, Color color)
    {
        AddEventLog(title);
        // ── 進化コンボ専用カチE��イン (1.6秒、中央に大きく表示) ──
        // Use a dedicated banner so combo evolution reads as a major moment.
        ShowEvolutionComboBanner(title, color, 1.6f);
        Flash(new Color(color.r, color.g, color.b, 0.55f), 1.0f);
        Shake(0.32f, 0.22f);
        HitFreeze(0.30f);
        SpawnSparks(player.position, color, 64, 2.0f);
        SpawnRingSparks(player.position, color, 42, 1.8f);
        SpawnRingSparks(player.position, color, 28, 2.6f);
        PlaySfx("Evolve", 180f, 0.45f, 0.55f);
        PlaySfx("Hit", 280f, 0.18f, 0.32f);
    }

    // ─────────────────────────────────────────────────────────
    // Solar Anchor (style 11)  Eコア共鳴型�E Update (シンプル匁E
    // ① Core Aura: コア周りに常時�Eるダメージリング (4 DPS / 半征E4m)
    //    視要E ゴールドリングが常に光ってぁE��
    //    効极E リング冁E��入った敵は毎秒削れる
    // ② Core Bond: プレイヤーがコア近接 (≤ 3m) で自身の攻撃+30%
    //    DamageEnemy で coreBondNearActive 参�E
    // ─────────────────────────────────────────────────────────
    void UpdateSolarAnchor()
    {
        coreBondNearActive = false;
        if (!solarAnchorActive || player == null || lantern == null) return;

        // ── Core Bond 判宁E(常時、視覚フィードバチE��なしでも体感できる) ──
        var dist = Vector2.Distance(player.position, lantern.position);
        coreBondNearActive = dist <= CoreBondNearRadius;

        if (gameOver || victory || paused || choosingUpgrade || choosingRelic) return;

        // ── Core Aura 視要E(リング表示、�E回作�E + 常時パルス) ──
        if (coreAuraVisual == null)
        {
            var auraGo = CreateSpriteObject("Core Aura Ring", circleSprite,
                lantern.position + new Vector3(0, 0, 0.06f),
                new Color(1f, 0.86f, 0.28f, 0.18f),
                (CoreAuraRadius + coreAuraRadiusBonus) * 2.0f);
            auraGo.GetComponent<SpriteRenderer>().sortingOrder = -2;
            coreAuraVisual = auraGo.transform;
        }
        if (coreAuraVisual != null)
        {
            coreAuraVisual.position = lantern.position + new Vector3(0, 0, 0.06f);
            var pulse = 0.85f + 0.18f * Mathf.Sin(Time.time * 2.2f);
            coreAuraVisual.localScale = Vector3.one * ((CoreAuraRadius + coreAuraRadiusBonus) * 2.0f) * pulse;
            var sr = coreAuraVisual.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                var alpha = 0.16f + 0.06f * Mathf.Sin(Time.time * 1.6f);
                sr.color = new Color(1f, 0.86f, 0.28f, alpha);
            }
        }

        // ── Core Aura ダメージ: 0.25秒毁Etick (4Hz) でリング冁E�E敵に DPS ──
        coreAuraTickAcc += Time.deltaTime;
        const float tickInterval = 0.25f;
        if (coreAuraTickAcc >= tickInterval)
        {
            coreAuraTickAcc = 0f;
            var corePos = lantern.position;
            var effectiveRadius = CoreAuraRadius + coreAuraRadiusBonus;
            var effectiveDps = CoreAuraDps + coreAuraDpsBonus;
            for (var i = enemies.Count - 1; i >= 0; i--)
            {
                var e = enemies[i];
                if (e == null || e.transform == null) continue;
                if (Vector2.Distance(e.transform.position, corePos) <= effectiveRadius)
                    DamageEnemy(e, effectiveDps * tickInterval, false);
            }
        }
    }

    // 進化コンボ専用の中央バナー (フェーチEin/out)
    float evolutionComboBannerTimer;
    float evolutionComboBannerDuration;
    Color evolutionComboBannerColor;
    GameObject evolutionComboBannerRoot;
    Text evolutionComboBannerTitle;
    Text evolutionComboBannerSubtitle;

    void ShowEvolutionComboBanner(string title, Color color, float duration)
    {
        if (evolutionComboBannerRoot == null)
            BuildEvolutionComboBanner();
        if (evolutionComboBannerRoot == null) return;

        evolutionComboBannerTimer = duration;
        evolutionComboBannerDuration = duration;
        evolutionComboBannerColor = color;
        evolutionComboBannerRoot.SetActive(true);
        evolutionComboBannerRoot.transform.SetAsLastSibling();
        if (evolutionComboBannerTitle != null)
        {
            evolutionComboBannerTitle.text = title;
            evolutionComboBannerTitle.color = new Color(color.r, color.g, color.b, 1f);
        }
        if (evolutionComboBannerSubtitle != null)
            evolutionComboBannerSubtitle.text = "進化コンボ発動";
    }

    void BuildEvolutionComboBanner()
    {
        if (canvas == null) return;
        evolutionComboBannerRoot = new GameObject("Evolution Combo Banner", typeof(Image));
        evolutionComboBannerRoot.transform.SetParent(canvas.transform, false);
        var img = evolutionComboBannerRoot.GetComponent<Image>();
        img.color = new Color(0.02f, 0.04f, 0.07f, 0.85f);
        img.raycastTarget = false;
        var rt = evolutionComboBannerRoot.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(900, 160);
        var outline = evolutionComboBannerRoot.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 0.92f, 0.42f, 0.6f);
        outline.effectDistance = new Vector2(2.5f, -2.5f);
        evolutionComboBannerSubtitle = CreateText("Combo Subtitle", evolutionComboBannerRoot.transform, new Vector2(0, 52), TextAnchor.MiddleCenter, 18, new Color(0.92f, 1f, 0.96f));
        evolutionComboBannerSubtitle.fontStyle = FontStyle.Bold;
        evolutionComboBannerSubtitle.rectTransform.sizeDelta = new Vector2(880, 26);
        evolutionComboBannerTitle = CreateText("Combo Title", evolutionComboBannerRoot.transform, new Vector2(0, -10), TextAnchor.MiddleCenter, 44, new Color(1f, 0.92f, 0.42f));
        evolutionComboBannerTitle.fontStyle = FontStyle.Bold;
        evolutionComboBannerTitle.rectTransform.sizeDelta = new Vector2(880, 70);
        var titleOutline = evolutionComboBannerTitle.gameObject.AddComponent<Outline>();
        titleOutline.effectColor = new Color(0f, 0f, 0f, 0.92f);
        titleOutline.effectDistance = new Vector2(2.5f, -2.5f);
        evolutionComboBannerRoot.SetActive(false);
    }

    void UpdateEvolutionComboBanner()
    {
        if (evolutionComboBannerRoot == null || !evolutionComboBannerRoot.activeSelf) return;
        evolutionComboBannerTimer -= Time.unscaledDeltaTime;
        if (evolutionComboBannerTimer <= 0f)
        {
            evolutionComboBannerRoot.SetActive(false);
            return;
        }
        // フェーチEin (最刁E0.2s) + フェーチEout (最征E0.35s)
        var t = 1f - (evolutionComboBannerTimer / Mathf.Max(0.01f, evolutionComboBannerDuration));
        var fadeIn = Mathf.Clamp01(t / 0.18f);
        var fadeOut = Mathf.Clamp01(evolutionComboBannerTimer / 0.35f);
        var alpha = Mathf.Min(fadeIn, fadeOut);
        var bg = evolutionComboBannerRoot.GetComponent<Image>();
        if (bg != null) bg.color = new Color(0.02f, 0.04f, 0.07f, 0.85f * alpha);
        if (evolutionComboBannerTitle != null)
            evolutionComboBannerTitle.color = new Color(evolutionComboBannerColor.r, evolutionComboBannerColor.g, evolutionComboBannerColor.b, alpha);
        if (evolutionComboBannerSubtitle != null)
            evolutionComboBannerSubtitle.color = new Color(0.92f, 1f, 0.96f, alpha);
        // 微妙なスケールパルス
        var pulse = 1f + 0.04f * Mathf.Sin(Time.unscaledTime * 8f);
        evolutionComboBannerRoot.transform.localScale = new Vector3(pulse, pulse, 1f);
    }

    // ─────────────────────────────────────────────────────────
    // Stage system (STAGE_DESIGN_SPEC.md 準拠)
    // Stage hazard notes.
    // - 形・色で読める警告 全画面チE��ント禁止、ローカル発光�Eみ
    // - ハザーチEdamage: grace 0.55s 経過後、~4Hz tick (低HP時�E遁E��)
    // - cutscene/upgrade/pause/relic 中はハザーチEdamage 停止
    // ─────────────────────────────────────────────────────────
    void ApplyStageConfig(int stageId)
    {
        currentStageId = Mathf.Clamp(stageId, 0, MaxStageId);
        if (currentStageId == 1)
            stageName = "Lava Cache";
        else if (currentStageId == 2)
            stageName = "Broken Core Network";
        else if (currentStageId == 3)
            stageName = "Frost Vault";
        else if (currentStageId == 4)
            stageName = "Storm Spire";
        else
            stageName = "Lantern Field";
    }

    string GetStageDisplayName(int stageId)
    {
        if (stageId == 4) return "Storm Spire";
        if (stageId == 3) return "Frost Vault";
        if (stageId == 2) return "Broken Core Network";
        if (stageId == 1) return "Lava Cache";
        return "Lantern Field";
    }

    void BuildStageEnvironment()
    {
        ClearStageObjects();
        if (currentStageId == 0) return;

        stageRandom = new System.Random();  // 軽ぁE��定論的バリエーション (run単佁E

        if (currentStageId == 1)
        {
            // ── Stage 2 (Lava Cache) ──
            CreateStage2MapSkin();
            var attempts = 0;
            var placed = 0;
            while (placed < LavaPoolCount && attempts < 250)
            {
                attempts++;
                var angle = (float)stageRandom.NextDouble() * Mathf.PI * 2f;
                var ringT = (float)stageRandom.NextDouble();
                var r = Mathf.Lerp(LavaPoolMinRingRadius, LavaPoolMaxRingRadius, ringT);
                var pos = new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
                var poolRadius = Mathf.Lerp(LavaPoolSizeMin, LavaPoolSizeMax, (float)stageRandom.NextDouble());
                if (!IsHazardPlacementValid(pos, poolRadius)) continue;
                CreateLavaPool(pos, poolRadius);
                placed++;
            }
            var vents = 0;
            attempts = 0;
            while (vents < HeatVentCount && attempts < 250)
            {
                attempts++;
                var angle = (float)stageRandom.NextDouble() * Mathf.PI * 2f;
                var ringT = (float)stageRandom.NextDouble();
                var r = Mathf.Lerp(HeatVentMinRingRadius, HeatVentMaxRingRadius, ringT);
                var pos = new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
                var ventRadius = 1.10f;
                if (!IsHazardPlacementValid(pos, ventRadius)) continue;
                CreateHeatVent(pos, ventRadius, vents);
                vents++;
            }
        }
        else if (currentStageId == 2)
        {
            // ── Stage 3 (Broken Core Network) ──
            CreateStage3MapSkin();
            // 汚染パッチ(Lava より弱罰、移動も低丁E
            var attempts = 0;
            var placed = 0;
            while (placed < CorruptionPatchCount && attempts < 250)
            {
                attempts++;
                var angle = (float)stageRandom.NextDouble() * Mathf.PI * 2f;
                var ringT = (float)stageRandom.NextDouble();
                var r = Mathf.Lerp(CorruptionPatchMinRingRadius, CorruptionPatchMaxRingRadius, ringT);
                var pos = new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
                var patchRadius = Mathf.Lerp(CorruptionPatchSizeMin, CorruptionPatchSizeMax, (float)stageRandom.NextDouble());
                if (!IsHazardPlacementValid(pos, patchRadius)) continue;
                CreateCorruptionPatch(pos, patchRadius);
                placed++;
            }
            // リレー裁E�� (起動で報酬)
            var relays = 0;
            attempts = 0;
            while (relays < RelayDeviceCount && attempts < 300)
            {
                attempts++;
                var angle = (float)stageRandom.NextDouble() * Mathf.PI * 2f;
                var ringT = (float)stageRandom.NextDouble();
                var r = Mathf.Lerp(RelayDeviceMinRingRadius, RelayDeviceMaxRingRadius, ringT);
                var pos = new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
                if (!IsHazardPlacementValid(pos, RelayDeviceRadius)) continue;
                CreateRelayDevice(pos);
                relays++;
            }
        }
        else if (currentStageId == 3)
        {
            // ── Stage 4 (Frost Vault) ──
            CreateStage4MapSkin();
            var attempts = 0;
            var placed = 0;
            while (placed < FrostPatchCount && attempts < 250)
            {
                attempts++;
                var angle = (float)stageRandom.NextDouble() * Mathf.PI * 2f;
                var ringT = (float)stageRandom.NextDouble();
                var r = Mathf.Lerp(FrostPatchMinRingRadius, FrostPatchMaxRingRadius, ringT);
                var pos = new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
                var patchRadius = Mathf.Lerp(FrostPatchSizeMin, FrostPatchSizeMax, (float)stageRandom.NextDouble());
                if (!IsHazardPlacementValid(pos, patchRadius)) continue;
                CreateFrostPatch(pos, patchRadius);
                placed++;
            }
            var crystals = 0;
            attempts = 0;
            while (crystals < FrostCrystalCount && attempts < 300)
            {
                attempts++;
                var angle = (float)stageRandom.NextDouble() * Mathf.PI * 2f;
                var ringT = (float)stageRandom.NextDouble();
                var r = Mathf.Lerp(FrostCrystalMinRingRadius, FrostCrystalMaxRingRadius, ringT);
                var pos = new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
                if (!IsHazardPlacementValid(pos, FrostCrystalRadius)) continue;
                CreateFrostCrystal(pos);
                crystals++;
            }
        }
        else if (currentStageId == 4)
        {
            // ── Stage 5 (Storm Spire) ──
            // 落雷はランダムタイミングで動的生�E (BuildStageEnvironment では何も配置しなぁE
            CreateStage5MapSkin();
            lightningNextStrikeTimer = LightningStrikeInterval * 0.6f;
        }
    }

    void CreateStage2MapSkin()
    {
        var heatWash = CreateSpriteObject("Stage2 Thermal Wash", squareSprite,
            new Vector3(0f, 0f, 1.0f),
            new Color(0.24f, 0.055f, 0.012f, 0.30f),
            ArenaRadius * 2.44f);
        heatWash.transform.localScale = new Vector3(ArenaRadius * 2.44f, ArenaRadius * 2.44f, 1f);
        heatWash.GetComponent<SpriteRenderer>().sortingOrder = -9;
        stageObjects.Add(heatWash);

        var outerHeat = CreateSpriteObject("Stage2 Outer Heat Field", circleSprite,
            new Vector3(0f, 0f, 0.99f),
            new Color(1f, 0.34f, 0.04f, 0.08f),
            ArenaRadius * 2.02f);
        outerHeat.GetComponent<SpriteRenderer>().sortingOrder = -8;
        stageObjects.Add(outerHeat);

        for (var i = 0; i < 18; i++)
        {
            var angle = (float)stageRandom.NextDouble() * Mathf.PI * 2f;
            var radius = Mathf.Lerp(2.8f, ArenaRadius - 1.2f, (float)stageRandom.NextDouble());
            var pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0.98f);
            var plate = CreateSpriteObject("Stage2 Scorched Plate", squareSprite, pos,
                i % 5 == 0 ? new Color(0.36f, 0.10f, 0.025f, 0.30f) : new Color(0.11f, 0.045f, 0.028f, 0.42f),
                1f);
            plate.transform.localScale = new Vector3(
                Mathf.Lerp(0.75f, 1.75f, (float)stageRandom.NextDouble()),
                Mathf.Lerp(0.22f, 0.56f, (float)stageRandom.NextDouble()),
                1f);
            plate.transform.rotation = Quaternion.Euler(0f, 0f, stageRandom.Next(0, 8) * 22.5f);
            plate.GetComponent<SpriteRenderer>().sortingOrder = -7;
            stageObjects.Add(plate);
        }

        for (var i = 0; i < 7; i++)
        {
            var angle = Mathf.PI * 2f * i / 7f + Mathf.Lerp(-0.18f, 0.18f, (float)stageRandom.NextDouble());
            var distance = Mathf.Lerp(3.8f, 7.8f, (float)stageRandom.NextDouble());
            var pos = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance, 0.92f);
            var seam = CreateSpriteObject("Stage2 Heat Conduit", squareSprite, pos,
                new Color(1f, 0.38f, 0.06f, 0.26f),
                1f);
            seam.transform.localScale = new Vector3(Mathf.Lerp(2.2f, 4.8f, (float)stageRandom.NextDouble()), 0.035f, 1f);
            seam.transform.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg + 90f + stageRandom.Next(-1, 2) * 18f);
            var seamRenderer = seam.GetComponent<SpriteRenderer>();
            seamRenderer.sortingOrder = -5;
            TrackAmbientNeon(seamRenderer);
            stageObjects.Add(seam);

            var core = CreateSpriteObject("Stage2 Conduit Core", squareSprite, pos + Vector3.back * 0.01f,
                new Color(1f, 0.78f, 0.18f, 0.18f),
                1f);
            core.transform.localScale = new Vector3(seam.transform.localScale.x * 0.38f, 0.018f, 1f);
            core.transform.rotation = seam.transform.rotation;
            var coreRenderer = core.GetComponent<SpriteRenderer>();
            coreRenderer.sortingOrder = -4;
            TrackAmbientNeon(coreRenderer);
            stageObjects.Add(core);
        }

        for (var i = 0; i < 24; i++)
        {
            var angle = Mathf.PI * 2f * i / 24f;
            var pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.82f) * (ArenaRadius - 0.12f);
            var marker = CreateSpriteObject("Stage2 Boundary Heat Marker", diamondSprite, pos,
                i % 3 == 0 ? new Color(1f, 0.62f, 0.08f, 0.62f) : new Color(0.52f, 0.12f, 0.04f, 0.44f),
                i % 3 == 0 ? 0.30f : 0.20f);
            marker.transform.rotation = Quaternion.Euler(0f, 0f, i * 15f);
            var markerRenderer = marker.GetComponent<SpriteRenderer>();
            markerRenderer.sortingOrder = -1;
            TrackAmbientNeon(markerRenderer);
            stageObjects.Add(marker);
        }

        for (var i = 0; i < 4; i++)
        {
            var angle = Mathf.PI * 0.5f * i + Mathf.PI * 0.25f;
            var pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.86f) * 2.65f;
            var coolant = CreateSpriteObject("Stage2 Coolant Safe Plate", squareSprite, pos,
                new Color(0.08f, 0.62f, 0.70f, 0.20f),
                1f);
            coolant.transform.localScale = new Vector3(0.62f, 0.18f, 1f);
            coolant.transform.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
            var coolantRenderer = coolant.GetComponent<SpriteRenderer>();
            coolantRenderer.sortingOrder = -4;
            TrackAmbientNeon(coolantRenderer);
            stageObjects.Add(coolant);
        }
    }

    bool IsHazardPlacementValid(Vector2 pos, float radius)
    {
        // Safe distance from the core at the origin.
        if (pos.magnitude < HazardSafetyFromCore + radius) return false;
        // Safe distance from the player spawn near the origin.
        if (pos.magnitude < HazardSafetyFromPlayerSpawn + radius) return false;
        for (var i = 0; i < stageHazards.Count; i++)
        {
            var h = stageHazards[i];
            if (Vector2.Distance(pos, h.center) < (radius + h.radius + HazardSafeLaneWidth))
                return false;
        }
        // Keep hazards inside the arena.
        if (pos.magnitude + radius > ArenaRadius - 0.3f) return false;
        return true;
    }

    void CreateLavaPool(Vector2 pos, float radius)
    {
        // Sprite priority: Stage 2 dedicated art > shared lava hazard > circle fallback.
        var lavaSprite = stage2LavaPoolSprite != null ? stage2LavaPoolSprite
            : floorHazardLavaSprite != null ? floorHazardLavaSprite : circleSprite;
        var lavaColor = stage2LavaPoolSprite != null ? new Color(1f, 1f, 1f, 0.92f)
            : floorHazardLavaSprite != null ? new Color(1f, 0.78f, 0.50f, 0.62f)
            : new Color(1f, 0.42f, 0.08f, 0.34f);
        var go = CreateSpriteObject("Lava Pool", lavaSprite,
            new Vector3(pos.x, pos.y, 0.05f),
            lavaColor,
            radius * 2.0f);   // sprite is 1-unit circle, × fills "radius"
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sortingOrder = -2;
        TrackAmbientNeon(sr);
        stageObjects.Add(go);
        // Add a small glowing rim so the hazard reads clearly.
        var rim = CreateSpriteObject("Lava Pool Rim", circleSprite,
            new Vector3(pos.x, pos.y, 0.04f),
            new Color(1f, 0.78f, 0.22f, 0.22f),
            radius * 2.25f);
        var rimSr = rim.GetComponent<SpriteRenderer>();
        rimSr.sortingOrder = -3;
        stageObjects.Add(rim);

        stageHazards.Add(new StageHazardZone
        {
            transform = go.transform,
            renderer = sr,
            center = pos,
            radius = radius,
            kind = StageHazardKind.Lava,
            isActive = true,
            visualPhase = (float)stageRandom.NextDouble() * Mathf.PI * 2f
        });
        SeedHazardRewardData(pos, radius, 2, 1.35f);
    }

    void SeedHazardRewardData(Vector2 center, float radius, int count, float value)
    {
        for (var i = 0; i < count; i++)
        {
            var angle = (float)stageRandom.NextDouble() * Mathf.PI * 2f;
            var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            var rewardPos = center + dir * (radius + Mathf.Lerp(0.34f, 0.62f, (float)stageRandom.NextDouble()));
            if (rewardPos.magnitude > ArenaRadius - 0.65f)
                rewardPos = rewardPos.normalized * (ArenaRadius - 0.65f);
            SpawnPickup(new Vector3(rewardPos.x, rewardPos.y, -0.02f), value);
        }
    }

    void CreateHeatVent(Vector2 pos, float radius, int index)
    {
        // Sprite priority: Stage 2 dedicated vent > square fallback.
        var ventSprite = stage2HeatVentSprite != null ? stage2HeatVentSprite : circleSprite;
        var ventColor = stage2HeatVentSprite != null ? new Color(1f, 1f, 1f, 0.55f)
            : new Color(1f, 0.62f, 0.18f, 0.18f);
        var go = CreateSpriteObject("Heat Vent", ventSprite,
            new Vector3(pos.x, pos.y, 0.05f),
            ventColor,
            radius * 2.0f);
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sortingOrder = -2;
        stageObjects.Add(go);

        // Stagger telegraphs so vents do not all fire at once.
        var initialDelay = HeatVentPulseInterval * 0.4f + index * (HeatVentPulseInterval * 0.5f);
        stageHazards.Add(new StageHazardZone
        {
            transform = go.transform,
            renderer = sr,
            center = pos,
            radius = radius,
            kind = StageHazardKind.HeatVent,
            isActive = false,
            telegraphTimer = initialDelay,
            visualPhase = (float)stageRandom.NextDouble() * Mathf.PI * 2f
        });
    }

    void CreateStage3MapSkin()
    {
        var corruptionWash = CreateSpriteObject("Stage3 Corruption Wash", squareSprite,
            new Vector3(0f, 0f, 1.0f),
            new Color(0.16f, 0.04f, 0.22f, 0.28f),
            ArenaRadius * 2.44f);
        corruptionWash.transform.localScale = new Vector3(ArenaRadius * 2.44f, ArenaRadius * 2.44f, 1f);
        corruptionWash.GetComponent<SpriteRenderer>().sortingOrder = -9;
        stageObjects.Add(corruptionWash);

        var outerHaze = CreateSpriteObject("Stage3 Outer Haze", circleSprite,
            new Vector3(0f, 0f, 0.99f),
            new Color(0.78f, 0.20f, 0.92f, 0.08f),
            ArenaRadius * 2.02f);
        outerHaze.GetComponent<SpriteRenderer>().sortingOrder = -8;
        stageObjects.Add(outerHaze);
    }

    void CreateCorruptionPatch(Vector2 pos, float radius)
    {
        // Prefer dedicated sprite, then fall back to a diamond marker.
        var corruptionSprite = stage3CorruptionSprite != null ? stage3CorruptionSprite : circleSprite;
        var corruptionColor = stage3CorruptionSprite != null
            ? new Color(1f, 1f, 1f, 0.78f)
            : new Color(0.78f, 0.22f, 0.92f, 0.32f);
        var go = CreateSpriteObject("Corruption Patch", corruptionSprite,
            new Vector3(pos.x, pos.y, 0.05f), corruptionColor, radius * 2.0f);
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sortingOrder = -2;
        stageObjects.Add(go);

        var rim = CreateSpriteObject("Corruption Patch Rim", circleSprite,
            new Vector3(pos.x, pos.y, 0.04f),
            new Color(0.85f, 0.32f, 1f, 0.22f),
            radius * 2.25f);
        rim.GetComponent<SpriteRenderer>().sortingOrder = -3;
        stageObjects.Add(rim);

        stageHazards.Add(new StageHazardZone
        {
            transform = go.transform,
            renderer = sr,
            center = pos,
            radius = radius,
            kind = StageHazardKind.CorruptionPatch,
            isActive = true,
            visualPhase = (float)stageRandom.NextDouble() * Mathf.PI * 2f
        });
    }

    void CreateRelayDevice(Vector2 pos)
    {
        // 専用 sprite > ダイヤフォールバック
        var relaySprite = stage3RelayDeviceSprite != null ? stage3RelayDeviceSprite : diamondSprite;
        var relayColor = stage3RelayDeviceSprite != null
            ? new Color(1f, 1f, 1f, 0.92f)
            : new Color(0.42f, 1f, 0.85f, 0.85f);
        var go = CreateSpriteObject("Relay Device", relaySprite,
            new Vector3(pos.x, pos.y, 0.04f), relayColor, RelayDeviceRadius * 2.0f);
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sortingOrder = -1;
        stageObjects.Add(go);

        // 起動進捗用リング (透�Eから始まり、起動中に光る)
        var ring = CreateSpriteObject("Relay Charge Ring", circleSprite,
            new Vector3(pos.x, pos.y, 0.03f),
            new Color(0.42f, 1f, 0.85f, 0f),
            RelayDeviceRadius * 2.6f);
        ring.GetComponent<SpriteRenderer>().sortingOrder = -2;
        stageObjects.Add(ring);

        stageHazards.Add(new StageHazardZone
        {
            transform = go.transform,
            renderer = sr,
            center = pos,
            radius = RelayDeviceRadius,
            kind = StageHazardKind.RelayDevice,
            isActive = true,
            relayActivated = false,
            relayChargeTimer = 0f,
            relayChargeRing = ring.transform,
            visualPhase = (float)stageRandom.NextDouble() * Mathf.PI * 2f
        });
    }

    // ─────────────────────────────────────────────────────────
    // Stage 4 (Frost Vault) helpers
    // ─────────────────────────────────────────────────────────
    void CreateStage4MapSkin()
    {
        var iceWash = CreateSpriteObject("Stage4 Ice Wash", squareSprite,
            new Vector3(0f, 0f, 1.0f),
            new Color(0.12f, 0.22f, 0.34f, 0.28f),
            ArenaRadius * 2.44f);
        iceWash.transform.localScale = new Vector3(ArenaRadius * 2.44f, ArenaRadius * 2.44f, 1f);
        iceWash.GetComponent<SpriteRenderer>().sortingOrder = -9;
        stageObjects.Add(iceWash);

        var outerCool = CreateSpriteObject("Stage4 Outer Cool", circleSprite,
            new Vector3(0f, 0f, 0.99f),
            new Color(0.55f, 0.85f, 1f, 0.08f),
            ArenaRadius * 2.02f);
        outerCool.GetComponent<SpriteRenderer>().sortingOrder = -8;
        stageObjects.Add(outerCool);
    }

    void CreateFrostPatch(Vector2 pos, float radius)
    {
        var patchSprite = stage4FrostPatchSprite != null ? stage4FrostPatchSprite : circleSprite;
        var patchColor = stage4FrostPatchSprite != null ? new Color(1f, 1f, 1f, 0.42f) : new Color(0.62f, 0.85f, 1f, 0.30f);
        var go = CreateSpriteObject("Frost Patch", patchSprite,
            new Vector3(pos.x, pos.y, 0.05f),
            patchColor,
            radius * 2.0f);
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sortingOrder = -2;
        stageObjects.Add(go);

        var rim = CreateSpriteObject("Frost Patch Rim", circleSprite,
            new Vector3(pos.x, pos.y, 0.04f),
            new Color(0.85f, 0.95f, 1f, 0.22f),
            radius * 2.20f);
        rim.GetComponent<SpriteRenderer>().sortingOrder = -3;
        stageObjects.Add(rim);

        stageHazards.Add(new StageHazardZone
        {
            transform = go.transform,
            renderer = sr,
            center = pos,
            radius = radius,
            kind = StageHazardKind.FrostPatch,
            isActive = true,
            visualPhase = (float)stageRandom.NextDouble() * Mathf.PI * 2f
        });
    }

    void CreateFrostCrystal(Vector2 pos)
    {
        var crystalSprite = stage4FrostCrystalSprite != null ? stage4FrostCrystalSprite : diamondSprite;
        var crystalColor = stage4FrostCrystalSprite != null ? Color.white : new Color(0.62f, 0.92f, 1f, 0.95f);
        var go = CreateSpriteObject("Frost Crystal", crystalSprite,
            new Vector3(pos.x, pos.y, 0.04f),
            crystalColor,
            FrostCrystalRadius * 2.0f);
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sortingOrder = -1;
        stageObjects.Add(go);

        var aura = CreateSpriteObject("Frost Crystal Aura", circleSprite,
            new Vector3(pos.x, pos.y, 0.03f),
            new Color(0.85f, 0.95f, 1f, 0.15f),
            FrostCrystalRadius * 3.0f);
        aura.GetComponent<SpriteRenderer>().sortingOrder = -2;
        stageObjects.Add(aura);

        stageHazards.Add(new StageHazardZone
        {
            transform = go.transform,
            renderer = sr,
            center = pos,
            radius = FrostCrystalRadius,
            kind = StageHazardKind.FrostCrystal,
            isActive = true,
            crystalHp = FrostCrystalHp,
            crystalMaxHp = FrostCrystalHp,
            visualPhase = (float)stageRandom.NextDouble() * Mathf.PI * 2f
        });
    }

    // ─────────────────────────────────────────────────────────
    // Stage 5 (Storm Spire) helpers
    // ─────────────────────────────────────────────────────────
    void CreateStage5MapSkin()
    {
        var stormWash = CreateSpriteObject("Stage5 Storm Wash", squareSprite,
            new Vector3(0f, 0f, 1.0f),
            new Color(0.10f, 0.06f, 0.20f, 0.30f),
            ArenaRadius * 2.44f);
        stormWash.transform.localScale = new Vector3(ArenaRadius * 2.44f, ArenaRadius * 2.44f, 1f);
        stormWash.GetComponent<SpriteRenderer>().sortingOrder = -9;
        stageObjects.Add(stormWash);

        var outerSpark = CreateSpriteObject("Stage5 Outer Spark", circleSprite,
            new Vector3(0f, 0f, 0.99f),
            new Color(0.78f, 0.62f, 1f, 0.08f),
            ArenaRadius * 2.02f);
        outerSpark.GetComponent<SpriteRenderer>().sortingOrder = -8;
        stageObjects.Add(outerSpark);
    }

    void SpawnLightningMarker()
    {
        var attempts = 0;
        while (attempts < 30)
        {
            attempts++;
            var angle = (float)stageRandom.NextDouble() * Mathf.PI * 2f;
            var dist = 2.0f + (float)stageRandom.NextDouble() * (ArenaRadius - 3.0f);
            var pos = new Vector2(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist);
            var ok = true;
            for (var i = 0; i < stageHazards.Count; i++)
            {
                if (stageHazards[i].kind != StageHazardKind.LightningMarker) continue;
                if (Vector2.Distance(pos, stageHazards[i].center) < LightningStrikeRadius * 1.5f) { ok = false; break; }
            }
            if (!ok) continue;
            CreateLightningMarker(pos);
            return;
        }
    }

    void CreateLightningMarker(Vector2 pos)
    {
        var markerSprite = stage5LightningMarkerSprite != null ? stage5LightningMarkerSprite : circleSprite;
        var markerColor = stage5LightningMarkerSprite != null ? new Color(1f, 1f, 1f, 0.55f) : new Color(1f, 0.92f, 0.32f, 0.30f);
        var go = CreateSpriteObject("Lightning Marker", markerSprite,
            new Vector3(pos.x, pos.y, 0.04f),
            markerColor,
            LightningStrikeRadius * 2.0f);
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sortingOrder = -2;
        stageObjects.Add(go);

        stageHazards.Add(new StageHazardZone
        {
            transform = go.transform,
            renderer = sr,
            center = pos,
            radius = LightningStrikeRadius,
            kind = StageHazardKind.LightningMarker,
            isActive = true,
            lightningStrikeAt = Time.time + LightningTelegraphSeconds,
            lightningResolved = false,
            visualPhase = (float)stageRandom.NextDouble() * Mathf.PI * 2f
        });
        PlaySfx("Hit", 320f, 0.06f, 0.12f);
    }

    void ClearStageObjects()
    {
        for (var i = 0; i < stageObjects.Count; i++)
            if (stageObjects[i] != null) Destroy(stageObjects[i]);
        stageObjects.Clear();
        stageHazards.Clear();
        playerLavaContactTimer = 0f;
        playerLavaTickTimer = 0f;
        bossThermalSurgeTimer = 0f;
        corruptionSlowActive = false;
        frostPatchSlowActive = false;
        lightningNextStrikeTimer = LightningStrikeInterval;
    }

    // ハザード状態更新 + プレイヤーへの damage 適用
    // - Vent: telegraph →active →cooldown のサイクル
    // - Lava: always active
    // - Damage is applied after grace; no damage during cutscenes, upgrades, pause, or relic screens.
    void UpdateStageHazards(float dt)
    {
        if (currentStageId == 0 || stageHazards.Count == 0) return;

        // ── Vent state machine + 視覚更新 ──
        for (var i = 0; i < stageHazards.Count; i++)
        {
            var h = stageHazards[i];
            if (h.transform == null || h.renderer == null) continue;

            var hasSpriteVent = stage2HeatVentSprite != null;
            var hasSpriteLava = stage2LavaPoolSprite != null;

            if (h.kind == StageHazardKind.HeatVent)
            {
                if (h.isActive)
                {
                    h.pulseTimer -= dt;
                    var aPulse = 0.85f + 0.12f * Mathf.Sin(Time.time * 14f + h.visualPhase);
                    h.renderer.color = hasSpriteVent
                        ? new Color(1f, 1f, 1f, aPulse)
                        : new Color(1f, 0.42f, 0.08f, aPulse * 0.65f);
                    if (h.pulseTimer <= 0f)
                    {
                        h.isActive = false;
                        h.telegraphTimer = HeatVentPulseInterval - HeatVentTelegraphSeconds;
                    }
                }
                else
                {
                    h.telegraphTimer -= dt;
                    if (h.telegraphTimer <= 0f)
                    {
                        var into = -h.telegraphTimer;
                        if (into < HeatVentTelegraphSeconds)
                        {
                            var t = Mathf.Clamp01(into / HeatVentTelegraphSeconds);
                            var alpha = Mathf.Lerp(0.55f, 0.85f, t) + 0.08f * Mathf.Sin(Time.time * 22f + h.visualPhase);
                            h.renderer.color = hasSpriteVent
                                ? new Color(1f, Mathf.Lerp(1f, 0.78f, t), Mathf.Lerp(1f, 0.78f, t), alpha)
                                : new Color(1f, Mathf.Lerp(0.62f, 0.32f, t), 0.16f, alpha);
                        }
                        else
                        {
                            h.isActive = true;
                            h.pulseTimer = HeatVentPulseDuration;
                            SpawnRingSparks(h.transform.position, new Color(1f, 0.42f, 0.08f), 18, h.radius * 1.5f);
                            PlaySfx("Hit", 220f, 0.10f, 0.18f);
                        }
                    }
                    else
                    {
                        h.renderer.color = hasSpriteVent
                            ? new Color(1f, 1f, 1f, 0.45f)
                            : new Color(1f, 0.62f, 0.18f, 0.18f);
                    }
                }
            }
            else if (h.kind == StageHazardKind.Lava)
            {
                var aBreath = 0.78f + 0.12f * Mathf.Sin(Time.time * 1.8f + h.visualPhase);
                h.renderer.color = hasSpriteLava
                    ? new Color(1f, 1f, 1f, aBreath)
                    : new Color(1f, 0.42f, 0.08f, aBreath * 0.40f);
            }
            else if (h.kind == StageHazardKind.CorruptionPatch)
            {
                // 紫マゼンタの呼吸アニメ
                var aBreath = 0.70f + 0.14f * Mathf.Sin(Time.time * 1.4f + h.visualPhase);
                var hasSpriteCorruption = stage3CorruptionSprite != null;
                h.renderer.color = hasSpriteCorruption
                    ? new Color(1f, 1f, 1f, aBreath)
                    : new Color(0.78f, 0.22f, 0.92f, aBreath * 0.42f);
            }
            else if (h.kind == StageHazardKind.RelayDevice)
            {
                // 起動渁E= シアン強発光、未起動= 黁E��パルス
                if (h.relayActivated)
                {
                    var aPulse = 0.78f + 0.10f * Mathf.Sin(Time.time * 4.5f + h.visualPhase);
                    h.renderer.color = new Color(0.45f, 1f, 0.85f, aPulse);
                }
                else
                {
                    var aPulse = 0.65f + 0.15f * Mathf.Sin(Time.time * 2.6f + h.visualPhase);
                    h.renderer.color = new Color(1f, 0.95f, 0.45f, aPulse);
                }
                if (h.relayChargeRing != null)
                {
                    var ringSr = h.relayChargeRing.GetComponent<SpriteRenderer>();
                    if (ringSr != null)
                    {
                        if (h.relayActivated)
                            ringSr.color = new Color(0.45f, 1f, 0.85f, 0f);
                        else if (h.relayChargeTimer > 0f)
                        {
                            var chargePct = Mathf.Clamp01(h.relayChargeTimer / RelayActivationSeconds);
                            ringSr.color = new Color(0.42f, 1f, 0.78f, 0.30f + 0.40f * chargePct);
                            h.relayChargeRing.localScale = Vector3.one * (RelayDeviceRadius * 2.6f * (0.85f + 0.30f * chargePct));
                        }
                        else
                        {
                            ringSr.color = new Color(0.42f, 1f, 0.78f, 0f);
                        }
                    }
                }
            }
            else if (h.kind == StageHazardKind.FrostPatch)
            {
                // 氷の青白パッチ呼吸アニメ
                var aBreath = 0.65f + 0.12f * Mathf.Sin(Time.time * 1.2f + h.visualPhase);
                h.renderer.color = new Color(0.62f, 0.85f, 1f, aBreath * 0.45f);
            }
            else if (h.kind == StageHazardKind.FrostCrystal)
            {
                // クリスタルの輝きアニメ (HP に応じて色変化)
                var aPulse = 0.85f + 0.10f * Mathf.Sin(Time.time * 3.5f + h.visualPhase);
                var hpRatio = Mathf.Clamp01(h.crystalHp / Mathf.Max(0.01f, h.crystalMaxHp));
                h.renderer.color = new Color(
                    Mathf.Lerp(1f, 0.62f, hpRatio),
                    0.92f,
                    1f, aPulse);
                h.transform.Rotate(0f, 0f, dt * 25f);  // めE��くり回転
            }
            else if (h.kind == StageHazardKind.LightningMarker)
            {
                if (!h.lightningResolved)
                {
                    var remaining = h.lightningStrikeAt - Time.time;
                    if (remaining > 0f)
                    {
                        var t = 1f - Mathf.Clamp01(remaining / LightningTelegraphSeconds);
                        var flicker = 0.4f + 0.5f * Mathf.Abs(Mathf.Sin(Time.time * Mathf.Lerp(8f, 28f, t)));
                        h.renderer.color = new Color(1f, Mathf.Lerp(0.92f, 1f, t), Mathf.Lerp(0.32f, 0.85f, t), flicker);
                    }
                    // 落雷の発動は player damage 適用ブロックで処理
                }
                else
                {
                    h.renderer.color = new Color(1f, 1f, 1f, 0f);  // 解決後�E透�E
                }
            }
        }

        // ── プレイヤー damage 適用 (cutscene/upgrade/pause/relic 中は完�E停止) ──
        if (player == null || gameOver || victory || paused || choosingUpgrade || choosingRelic
            || evolutionCutsceneTimer > 0f
            || (bossCutscenePanel != null && bossCutscenePanel.activeSelf))
        {
            playerLavaContactTimer = 0f;
            playerLavaTickTimer = 0f;
            return;
        }

        var px = (Vector2)player.position;
        // ハザード接触チェチE�� (Lava / HeatVent / CorruptionPatch のみ damage 対象)
        var inDamageHazard = false;
        var inCorruption = false;
        var inFrostPatch = false;
        for (var i = 0; i < stageHazards.Count; i++)
        {
            var h = stageHazards[i];
            if (h.kind == StageHazardKind.RelayDevice
                || h.kind == StageHazardKind.FrostCrystal
                || h.kind == StageHazardKind.LightningMarker) continue;
            if (h.kind == StageHazardKind.HeatVent && !h.isActive) continue;
            if (Vector2.Distance(px, h.center) < h.radius)
            {
                if (h.kind == StageHazardKind.FrostPatch) { inFrostPatch = true; continue; }  // damage なし、slow のみ
                inDamageHazard = true;
                if (h.kind == StageHazardKind.CorruptionPatch) inCorruption = true;
                break;
            }
        }
        corruptionSlowActive = inCorruption;
        frostPatchSlowActive = inFrostPatch;

        if (inDamageHazard)
        {
            playerLavaContactTimer += dt;
            var grace = inCorruption ? CorruptionGraceSeconds : LavaGraceSeconds;
            if (playerLavaContactTimer >= grace)
            {
                playerLavaTickTimer -= dt;
                if (playerLavaTickTimer <= 0f)
                {
                    var lowHp = playerHp <= playerMaxHp * 0.35f;
                    var tickInterval = lowHp ? 0.55f : 0.25f;
                    playerLavaTickTimer = tickInterval;
                    var dps = inCorruption ? CorruptionDamagePerSecond : LavaDamagePerSecond;
                    DamagePlayer(dps * tickInterval, inCorruption ? "汚染パッチ" : "溶岩");
                    var hazColor = inCorruption ? new Color(0.85f, 0.32f, 1f) : new Color(1f, 0.62f, 0.18f);
                    Flash(new Color(hazColor.r, hazColor.g, hazColor.b, 0.10f), 0.10f);
                    SpawnSparks(player.position, hazColor, 4, 0.55f);
                }
            }
        }
        else
        {
            playerLavaContactTimer = 0f;
            playerLavaTickTimer = 0f;
        }

        // ── Stage 3 リレー裁E��: プレイヤーが乗ってぁE��間に允E�� →起動で報酬 ──
        if (currentStageId == 2)
        {
            for (var i = 0; i < stageHazards.Count; i++)
            {
                var h = stageHazards[i];
                if (h.kind != StageHazardKind.RelayDevice) continue;
                if (h.relayActivated) continue;
                if (Vector2.Distance(px, h.center) < h.radius + 0.3f)
                {
                    h.relayChargeTimer += dt;
                    if (h.relayChargeTimer >= RelayActivationSeconds)
                    {
                        h.relayActivated = true;
                        dataChips += RelayDataReward;
                        playerHp = Mathf.Min(playerMaxHp, playerHp + RelayHealReward);
                        lanternHp = Mathf.Min(lanternMaxHp, lanternHp + RelayCoreHealReward);
                        var relayColor = new Color(0.45f, 1f, 0.85f);
                        SpawnRingSparks(h.transform.position, relayColor, 28, h.radius * 2.5f);
                        SpawnSparks(h.transform.position, relayColor, 20, 1.2f);
                        Flash(new Color(relayColor.r, relayColor.g, relayColor.b, 0.18f), 0.30f);
                        CreateFloatingText("+" + (int)RelayDataReward + " DATA + HEAL", h.transform.position + Vector3.up * 0.8f, relayColor, 0.18f);
                        AddEventLog("RELAY 起動 +" + (int)RelayDataReward + "データ / +" + (int)RelayHealReward + "HP");
                        PlaySfx("Evolve", 380f, 0.18f, 0.30f);
                    }
                }
                else if (h.relayChargeTimer > 0f)
                {
                    h.relayChargeTimer = Mathf.Max(0f, h.relayChargeTimer - dt * 1.5f);  // 離脱で減衰
                }
            }
        }

        // ── Stage 4 凍結クリスタル: 弾/レーザー/オーラで HP 減少、Eで破壊�EAoE凍絁E報酬 ──
        if (currentStageId == 3)
        {
            for (var i = stageHazards.Count - 1; i >= 0; i--)
            {
                var h = stageHazards[i];
                if (h.kind != StageHazardKind.FrostCrystal) continue;
                if (!h.isActive) continue;
                // 1) 弾の当たり判宁E(既孁E
                for (var b = bullets.Count - 1; b >= 0; b--)
                {
                    var bullet = bullets[b];
                    if (bullet.fromEnemy || bullet.transform == null) continue;
                    if (Vector2.Distance(bullet.transform.position, h.center) > h.radius + bullet.hitRadius) continue;
                    h.crystalHp -= bullet.damage;
                    SpawnSparks(bullet.transform.position, new Color(0.85f, 0.95f, 1f), 4, 0.45f);
                    if (bullet.pierce <= 0)
                    {
                        Destroy(bullet.transform.gameObject);
                        bullets.RemoveAt(b);
                    }
                    if (h.crystalHp <= 0f) break;
                }
                // 2) オーラ接触 (Wraith Lynx / プラズマオーラ系)
                if (h.isActive && playerAuraDamage > 0f)
                {
                    var auraRadius = Mathf.Max(0.75f, playerAuraRadius);
                    if (Vector2.Distance(player.position, h.center) < auraRadius + h.radius)
                        h.crystalHp -= playerAuraDamage * dt;
                }
                // 3) レーザー接触 (Pulse Hydra)
                if (h.isActive && laserActive)
                {
                    if (Vector2.Distance(player.position, h.center) < laserMaxLength + h.radius
                        && DistancePointToSegment(h.center, player.position, player.position + (Vector3)aimDirection * laserMaxLength) < h.radius + 0.3f)
                        h.crystalHp -= bulletDamage * laserDamageMultiplier * dt;
                }
                // 4) プレイヤー直接接触 (近接 contactBurst)
                if (h.isActive && contactBurst > 0f
                    && Vector2.Distance(player.position, h.center) < h.radius + 0.6f)
                    h.crystalHp -= contactBurst * dt * 2f;

                if (h.crystalHp <= 0f && h.isActive)
                {
                    h.isActive = false;
                    var freezeColor = new Color(0.62f, 0.92f, 1f);
                    // AoE凍絁E 周囲の敵をknockback で吹き飛�EぁE+ brief slow
                    for (var e = enemies.Count - 1; e >= 0; e--)
                    {
                        var enemy = enemies[e];
                        if (enemy == null || enemy.transform == null) continue;
                        var d = Vector2.Distance(enemy.transform.position, h.center);
                        if (d > FrostCrystalFreezeRadius) continue;
                        var pushDir = ((Vector2)(enemy.transform.position - (Vector3)h.center)).normalized;
                        enemy.knockbackVelocity = pushDir * 3.5f;
                        DamageEnemy(enemy, 2f, false);
                    }
                    // 報酬: データ
                    dataChips += FrostCrystalDataReward;
                    SpawnRingSparks(h.transform.position, freezeColor, 36, FrostCrystalFreezeRadius * 1.8f);
                    SpawnRingSparks(h.transform.position, freezeColor, 22, FrostCrystalFreezeRadius * 2.6f);
                    SpawnSparks(h.transform.position, freezeColor, 28, 1.6f);
                    Flash(new Color(freezeColor.r, freezeColor.g, freezeColor.b, 0.22f), 0.30f);
                    Shake(0.18f, 0.10f);
                    CreateFloatingText("CRYSTAL +" + (int)FrostCrystalDataReward + " DATA", h.transform.position + Vector3.up * 0.6f, freezeColor, 0.18f);
                    AddEventLog("FROST CRYSTAL 破壊 +" + (int)FrostCrystalDataReward + "データ / 周囲凍結");
                    PlaySfx("Evolve", 540f, 0.15f, 0.28f);
                    // クリスタル sprite 透�E匁E(削除はしなぁE���E生�E回避)
                    h.renderer.color = new Color(0.62f, 0.92f, 1f, 0f);
                }
            }
        }

        // ── Stage 5 雷撁E 周期生戁E+ チE��グラフ後�E発勁E──
        if (currentStageId == 4)
        {
            lightningNextStrikeTimer -= dt;
            if (lightningNextStrikeTimer <= 0f)
            {
                lightningNextStrikeTimer = LightningStrikeInterval;
                SpawnLightningMarker();
            }
            for (var i = 0; i < stageHazards.Count; i++)
            {
                var h = stageHazards[i];
                if (h.kind != StageHazardKind.LightningMarker) continue;
                if (h.lightningResolved) continue;
                if (Time.time >= h.lightningStrikeAt)
                {
                    h.lightningResolved = true;
                    var strikeColor = new Color(1f, 0.92f, 0.32f);
                    var strikePos = h.transform.position;
                    if (stage5LightningStrikeSprite != null)
                    {
                        var strikeGo = CreateSpriteObject("Lightning Strike VFX", stage5LightningStrikeSprite,
                            new Vector3(strikePos.x, strikePos.y, 0.02f),
                            new Color(1f, 0.97f, 0.7f, 0.95f),
                            LightningStrikeRadius * 1.4f);
                        strikeGo.GetComponent<SpriteRenderer>().sortingOrder = 40;
                        Destroy(strikeGo, 0.24f);
                    }
                    // 敵に大ダメージ + knockback (スタン表現)
                    for (var e = enemies.Count - 1; e >= 0; e--)
                    {
                        var enemy = enemies[e];
                        if (enemy == null || enemy.transform == null) continue;
                        var d = Vector2.Distance(enemy.transform.position, strikePos);
                        if (d > LightningStrikeRadius) continue;
                        DamageEnemy(enemy, LightningEnemyDamage, false);
                        var pushDir = ((Vector2)(enemy.transform.position - strikePos)).normalized;
                        if (pushDir.sqrMagnitude < 0.01f) pushDir = Vector2.up;
                        enemy.knockbackVelocity = pushDir * 2.5f;
                    }
                    // プレイヤーに軽ダメ (落雷範囲冁E��めE
                    if (Vector2.Distance(px, (Vector2)strikePos) <= LightningStrikeRadius)
                        DamagePlayer(LightningPlayerDamage, "落雷");
                    // 派手な視覚演�E
                    SpawnRingSparks(strikePos, strikeColor, 40, LightningStrikeRadius * 1.2f);
                    SpawnRingSparks(strikePos, strikeColor, 28, LightningStrikeRadius * 1.8f);
                    SpawnSparks(strikePos, strikeColor, 36, 1.8f);
                    Flash(new Color(1f, 0.95f, 0.55f, 0.35f), 0.45f);
                    Shake(0.45f, 0.22f);
                    HitFreeze(0.10f);
                    PlaySfx("Hit", 80f, 0.45f, 0.55f);
                    PlaySfx("Evolve", 220f, 0.30f, 0.40f);
                    AddEventLog("LIGHTNING STRIKE: 範囲ダメ+スタン");
                }
            }
        }

        // ── Stage 2 Thermal Surge (Phase 2 が未起動�E時�E代替プレチE��ャー) ──
        // Phase 2 が起動すれ�E Spiral/Summon が代わりに走る�Eでスキップ (重褁E��力回避)
        if (bossEnemy != null && bossEnemy.transform != null
            && bossEnemy.type == EnemyType.Boss
            && wave == MaxWave
            && !bossEnemy.phase2Triggered
            && bossEnemy.hp <= bossEnemy.maxHp * 0.5f)
        {
            bossThermalSurgeTimer -= dt;
            if (bossThermalSurgeTimer <= 0f)
            {
                bossThermalSurgeTimer = BossThermalSurgeCooldown;
                var surgePos = bossEnemy.transform.position;
                // Telegraph is handled visually; resolve the lightweight surge immediately here.
                SpawnRingSparks(surgePos, new Color(1f, 0.42f, 0.08f), 28, 2.2f);
                Flash(new Color(1f, 0.42f, 0.08f, 0.12f), 0.20f);
                Shake(0.10f, 0.08f);
                if (Vector2.Distance(player.position, surgePos) < 2.2f)
                    DamagePlayer(0.6f, "Thermal Surge");  // Light pressure: about 0.6 damage per 8s.
            }
        }
    }

    // Rename wave traits per stage theme.
    // (冁E��ロジチE��は既存�E trait のまま流用、UI 表示斁E��だけ書き換ぁE
    string GetStageDisplayTrait(string baseTrait)
    {
        if (currentStageId == 1)
        {
            switch (baseTrait)
            {
                case "Berserk":      return "Overheat";
                case "Dark Field":   return "Smoke Field";
                case "Elite Swarm":  return "Magma Swarm";
            }
        }
        else if (currentStageId == 2)
        {
            switch (baseTrait)
            {
                case "Berserk":      return "Glitch Rage";
                case "Dark Field":   return "Static Veil";
                case "Elite Swarm":  return "Corruption Swarm";
                case "Iron Skin":    return "Hardened Shell";
            }
        }
        else if (currentStageId == 3)
        {
            switch (baseTrait)
            {
                case "Berserk":      return "Cold Snap";
                case "Dark Field":   return "Blizzard";
                case "Elite Swarm":  return "Frost Pack";
                case "Iron Skin":    return "Glacier Hide";
                case "Rush":         return "Snow Drift";
            }
        }
        else if (currentStageId == 4)
        {
            switch (baseTrait)
            {
                case "Berserk":      return "Static Surge";
                case "Dark Field":   return "Thunder Shroud";
                case "Elite Swarm":  return "Storm Swarm";
                case "Iron Skin":    return "Charged Shell";
                case "Rush":         return "Voltage Rush";
            }
        }
        return baseTrait;
    }

    // ── Stage Selector UI ─────────────────────────────────────
    void SelectStage(int stageId)
    {
        if (stageId < 0 || stageId > MaxStageId) return;
        if (stageId > stageMaxUnlocked) return;  // ロック中
        currentStageId = stageId;
        PlayerPrefs.SetInt(StageSelectedKey, currentStageId);
        PlayerPrefs.Save();
        ApplyStageConfig(currentStageId);
        RefreshStageChips();
        PlaySfx("Pickup", 700f, 0.04f, 0.10f);
    }

    void RefreshStageChips()
    {
        if (stageChipButtons.Count == 0) return;
        for (var i = 0; i < stageChipButtons.Count; i++)
        {
            var chip = stageChipButtons[i];
            if (chip == null) continue;
            var unlocked = i <= stageMaxUnlocked;
            var selected = i == currentStageId;
            chip.interactable = unlocked;
            var img = chip.GetComponent<Image>();
            if (img != null)
            {
                // If a thumb sprite exists, keep texture colors readable with a white tint.
                // If missing, use the traditional fallback color fill.
                Sprite thumbForIndex = null;
                if (i == 0) thumbForIndex = stageThumbArenaSprite;
                else if (i == 1) thumbForIndex = stageThumbLavaSprite;
                else if (i == 2) thumbForIndex = stageThumbBrokenCoreSprite;
                else if (i == 3) thumbForIndex = stageThumbFrostSprite;
                else if (i == 4) thumbForIndex = stageThumbStormSprite;
                // Stage 4/5 (Frost/Storm) thumbs connected; any missing sprite falls back to color fill.
                var hasThumb = img.sprite != null && thumbForIndex != null;
                if (hasThumb)
                {
                    if (!unlocked)        img.color = new Color(0.30f, 0.34f, 0.36f, 0.55f);
                    else if (selected)    img.color = new Color(1f, 1f, 1f, 0.95f);
                    else                  img.color = new Color(0.92f, 1f, 1f, 0.55f);
                }
                else
                {
                    if (!unlocked)        img.color = new Color(0.06f, 0.08f, 0.10f, 0.7f);
                    else if (selected)    img.color = new Color(0.05f, 0.30f, 0.40f, 0.98f);
                    else                  img.color = new Color(0.05f, 0.14f, 0.18f, 0.95f);
                }
            }
            var txt = chip.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.color = !unlocked ? new Color(0.35f, 0.4f, 0.42f)
                    : selected ? new Color(1f, 0.96f, 0.62f)
                    : new Color(0.92f, 1f, 1f);
                txt.fontStyle = FontStyle.Bold;
            }
        }
        RefreshSelectionInfoLine();
    }

    // STAGE と DANGER の現在選択を 1行に統合表示
    // RunConfig パネル中央 + Main Menu サマリ行�E両方を更新
    void RefreshSelectionInfoLine()
    {
        var stageStr = currentStageId == 4 ? "Stage: Storm Spire"
                     : currentStageId == 3 ? "Stage: Frost Vault"
                     : currentStageId == 2 ? "Stage: Broken Core Network"
                     : currentStageId == 1 ? "Stage: Lava Cache"
                     : "Stage: Lantern Field";
        var dangerStr = "Danger: D" + dangerLevel;
        string suffix;
        if (dangerLevel == 0)
        {
            suffix = "標準難度";
        }
        else
        {
            var hpMul = GetDangerHpMultiplier();
            var spdMul = GetDangerSpeedMultiplier();
            var spawnMul = GetDangerSpawnMultiplier();
            suffix = "敵HP×" + hpMul.ToString("0.00")
                + " 速×" + spdMul.ToString("0.00")
                + " 数×" + spawnMul.ToString("0.00");
        }
        var combined = stageStr + "    " + dangerStr + "    ( " + suffix + " )";
        if (stageSelectorLabel != null) stageSelectorLabel.text = combined;
        if (mainMenuSelectionBriefText != null) mainMenuSelectionBriefText.text = combined;
    }

    // ── Danger Level UI ───────────────────────────────────────
    void SelectDangerLevel(int level)
    {
        var selectableMax = Mathf.Min(MaxDangerLevel, dangerMaxCleared + 1);
        if (level < 0 || level > selectableMax) return;
        dangerLevel = level;
        PlayerPrefs.SetInt(DangerSelectedKey, dangerLevel);
        PlayerPrefs.Save();
        RefreshDangerChips();
        PlaySfx("Pickup", 720f, 0.04f, 0.10f);
    }

    void RefreshDangerChips()
    {
        if (dangerChipButtons.Count == 0) return;
        var selectableMax = Mathf.Min(MaxDangerLevel, dangerMaxCleared + 1);
        for (var i = 0; i < dangerChipButtons.Count; i++)
        {
            var chip = dangerChipButtons[i];
            if (chip == null) continue;
            var unlocked = i <= selectableMax;
            var selected = i == dangerLevel;
            chip.interactable = unlocked;
            var img = chip.GetComponent<Image>();
            if (img != null)
            {
                if (!unlocked)
                    img.color = new Color(0.06f, 0.08f, 0.10f, 0.7f);
                else if (selected)
                    img.color = new Color(0.55f, 0.18f, 0.05f, 0.98f);
                else
                    img.color = new Color(0.05f, 0.14f, 0.18f, 0.95f);
            }
            var txt = chip.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.color = !unlocked ? new Color(0.35f, 0.4f, 0.42f)
                    : selected ? new Color(1f, 0.95f, 0.55f)
                    : new Color(0.8f, 1f, 1f);
                txt.fontStyle = selected ? FontStyle.Bold : FontStyle.Normal;
            }
        }
        RefreshSelectionInfoLine();
    }

    void CheckFusion()
    {
        if (fusionActive || evolutionStage < 2)
            return;
        // 既にキュー済みなら新規上書きしなぁE(プレイヤーが選択中)
        if (pendingFusionApply != null)
            return;

        if (allyNova && allyBulwark && bulletCount >= 3)
        {
            RequestFusion("Nova Aegis", "CROSS EVOLVE: Nova Aegis", () =>
            {
                fusionName = "Nova Aegis";
                bulletCount++;
                orbitShield = true;
                orbitDamage *= 1.45f;
                bossDamageMultiplier *= 1.25f;
            });
            return;
        }

        if (allyNova && allySiphon && dataMultiplier >= 1.25f)
        {
            RequestFusion("Photon Siphon", "CROSS EVOLVE: Photon Siphon", () =>
            {
                fusionName = "Photon Siphon";
                fireRate *= 1.22f;
                pickupRange *= 1.35f;
                pickupHealChance = Mathf.Min(0.7f, pickupHealChance + 0.22f);
            });
            return;
        }

        if (allyBulwark && allySiphon && lanternMaxHp >= 25f)
        {
            RequestFusion("Core Bastion", "CROSS EVOLVE: Core Bastion", () =>
            {
                fusionName = "Core Bastion";
                lanternMaxHp += 8f;
                lanternHp += 8f;
                lightRadius *= 1.18f;
                orbitShield = true;
            });
            return;
        }

        if (allyNova && allyPhase && moveSpeed >= 5.4f)
        {
            RequestFusion("Nova Phantom", "CROSS EVOLVE: Nova Phantom", () =>
            {
                fusionName = "Nova Phantom";
                fireRate *= 1.18f;
                moveSpeed *= 1.08f;
                speedChainBonus++;
                speedEchoActive = true;
            });
            return;
        }

        if (allyBulwark && allyPhase && playerMaxHp >= 8f)
        {
            RequestFusion("Aegis Drift", "CROSS EVOLVE: Aegis Drift", () =>
            {
                fusionName = "Aegis Drift";
                moveSpeed *= 1.1f;
                lanternMaxHp += 4f;
                lanternHp += 4f;
                reflectShield = true;
                contactBurst += 0.6f;
            });
            return;
        }

        if (allySiphon && allyPhase && pickupRange >= 1.4f)
        {
            RequestFusion("Photon Wraith", "CROSS EVOLVE: Photon Wraith", () =>
            {
                fusionName = "Photon Wraith";
                moveSpeed *= 1.06f;
                pickupRange *= 1.3f;
                dataMultiplier *= 1.18f;
                pickupHealChance = Mathf.Min(0.6f, pickupHealChance + 0.18f);
            });
        }
    }

    // ─────────────────────────────────────────────────────────
    // クロス進化選択キュー (ユーザー持E��で auto-trigger →選択制に変更)
    // 条件が揃った段階で「進化すめE/ 見送る」�E 2 択を出ぁE    // 「見送る」を選んでも条件は満たしたままなので、後で再オファーされめE    // ─────────────────────────────────────────────────────────
    void RequestFusion(string name, string message, System.Action apply)
    {
        pendingFusionName = name;
        pendingFusionMessage = message;
        pendingFusionApply = apply;
        // メチE��ージで「準備完亁E��を即伝えるが、E��択パネルは安�Eな場所で開く
        ShowMessage("CROSS EVOLVE 準備完了");
    }

    // クロス進化�E効果を 1 行で要紁E(UI 表示用)
    // 吁Efusion の apply() で行ってぁE��スデータス変更とミラー
    string GetFusionEffectSummary(string fusionName)
    {
        switch (fusionName)
        {
            case "Nova Aegis":    return "弾数 +1\nガードリング起動/ 威力 +45%\nボスdmg +25%";
            case "Photon Siphon": return "連射+22%\n回収範囲 +35%\n拾い回復確率+22%";
            case "Core Bastion":  return "コアHP最大 +8 (即回復)\n光半径+18%\nガードリング起動";
            case "Nova Phantom":  return "連射+18% / 移動+8%\nチェイン +1 hop\n残像追撃起動";
            case "Aegis Drift":   return "移動+10% / コアHP最大 +4\n反射E起動\n接触反撃 +0.6";
            case "Photon Wraith": return "移動+6% / 回収範囲 +30%\nデータ獲得+18%\n拾い回復確率+18%";
        }
        return "専用フォームへ進化する";
    }

    // クロス進化�E絁E��合わせ説昁E(侁E "Nova + Bulwark")
    string GetFusionPairDescription(string fusionName)
    {
        switch (fusionName)
        {
            case "Nova Aegis":    return "Nova + Bulwark (火力 + 防衛)";
            case "Photon Siphon": return "Nova + Siphon (連射+ 回収)";
            case "Core Bastion":  return "Bulwark + Siphon (コア防衛特化)";
            case "Nova Phantom":  return "Nova + Phase (高速連鎖)";
            case "Aegis Drift":   return "Bulwark + Phase (機動防衛)";
            case "Photon Wraith": return "Siphon + Phase (回収粘り)";
        }
        return "";
    }

    void OpenFusionChoicePanel()
    {
        if (pendingFusionApply == null || string.IsNullOrEmpty(pendingFusionName)) return;
        if (gameOver || victory) return;

        choosingUpgrade = true;
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        UpdateModuleStatusSidePanel();
        SetChoiceFocus(true);
        SetUpgradePanelTheme(false);

        var fusionStyleInt = GetFusionStyle(pendingFusionName);
        var fusionAccent = GetFusionAccentColor(fusionStyleInt);

        panelTitleText.text = "⚡ CROSS EVOLVE READY";
        panelTitleText.color = fusionAccent;
        if (panelSubtitleText != null)
        {
            panelSubtitleText.text = pendingFusionName + " / " + GetFusionPairDescription(pendingFusionName);
            panelSubtitleText.color = new Color(0.85f, 1f, 0.92f);
        }
        cardAppearStartTime = Time.unscaledTime;
        choiceClickGuardUntil = Time.unscaledTime + ChoiceClickGuardDuration;

        // 2択カーチE [進化する] / [見送る]
        // 進化カーチE 実効果を箁E��書きで表示してプレイヤーが判断できるように
        var evolveChoice = new Upgrade(
            "CROSS EVOLVE: " + pendingFusionName,
            GetFusionEffectSummary(pendingFusionName) + "\n+ クロス進化専用モジュール解禁",
            "★★★",
            null);
        var skipChoice = new Upgrade(
            "見送る (温存)",
            "今は進化しない。\n条件は維持されるので、後で再オファーされる。",
            "★★★",
            null);

        var choices = new List<Upgrade> { evolveChoice, skipChoice };
        // RenderUpgradeChoices で 2 カード�E置
        RenderUpgradeChoices(choices);

        // ── 進化カード�E見た目めEfusion accent カラーで上書ぁE──
        var evolveButton = upgradeButtons[0];
        if (evolveButton != null)
        {
            var fusionSprite = LoadOptionalSprite("Skins/Fusion_" + fusionStyleInt, null);
            // 派手な fusion 色チE�Eマで再描画
            var deepBg = Color.Lerp(new Color(0.08f, 0.02f, 0.12f, 0.98f), fusionAccent, 0.15f);
            var hiBg = Color.Lerp(new Color(0.20f, 0.06f, 0.28f, 1f), fusionAccent, 0.30f);
            var label = evolveButton.transform.Find("Label").GetComponent<Text>();
            ApplyCardVisual(evolveButton, label, deepBg, hiBg, new Color(1f, 0.92f, 1f), fusionAccent, "FUSION " + fusionStyleInt, 1.4f, "★★★");
            SetCardText(evolveButton, label, "CROSS EVOLVE\n" + pendingFusionName, "", GetFusionEffectSummary(pendingFusionName) + "\n\n+ クロス進化専用モジュール解禁", new Color(1f, 0.92f, 1f), new Color(0.85f, 1f, 0.95f), fusionAccent, 1.4f);
            if (fusionSprite != null)
            {
                var icon = evolveButton.transform.Find("Card Icon");
                if (icon != null)
                {
                    var iconImage = icon.GetComponent<Image>();
                    if (iconImage != null)
                    {
                        iconImage.sprite = fusionSprite;
                        iconImage.color = Color.white;
                        iconImage.preserveAspect = true;
                    }
                    var iconRect = icon.GetComponent<RectTransform>();
                    if (iconRect != null)
                        iconRect.sizeDelta = new Vector2(108, 108);
                }
            }
        }

        // ── 見送りカード�Eあえてグレー寁E��にしてヒエラルキー差を作る ──
        var skipButton = upgradeButtons[1];
        if (skipButton != null)
        {
            var skipAccent = new Color(0.55f, 0.62f, 0.7f, 1f);
            var skipLabel = skipButton.transform.Find("Label").GetComponent<Text>();
            ApplyCardVisual(skipButton, skipLabel, new Color(0.05f, 0.07f, 0.09f, 0.98f), new Color(0.12f, 0.16f, 0.20f, 1f), new Color(0.78f, 0.86f, 0.92f), skipAccent, "PASS", 0.0f, "★★★");
            SetCardText(skipButton, skipLabel, "見送る\n(温存)", "", "今は進化しない。\n条件は維持されるので、後で再オファー。", new Color(0.92f, 0.96f, 1f), new Color(0.7f, 0.8f, 0.88f), skipAccent, 0.0f);
        }

        for (var i = 0; i < upgradeButtons.Count && i < 2; i++)
        {
            var idx = i;
            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() =>
            {
                if (IsChoiceClickGuarded()) return;
                upgradePanel.SetActive(false);
                choosingUpgrade = false;
                SetChoiceFocus(false);
                if (idx == 0)
                {
                    var apply = pendingFusionApply;
                    var msg = pendingFusionMessage;
                    pendingFusionApply = null;
                    pendingFusionMessage = null;
                    pendingFusionName = null;
                    ActivateFusion(msg, apply);
                    // ActivateFusion 冁E�� QueueEvolutionCutscene が走めEqueuedCutscene=true 化されるので、E                    // 既孁Eupgrade pick handler と同じぁEcutscene 開姁Eor 通常復帰
                    if (queuedCutscene)
                        BeginQueuedCutscene();
                    else
                        Time.timeScale = 1f;
                }
                else
                {
                    // [見送る] →クリアして温存。次の Lv UP / モジュール選択時に再オファー
                    pendingFusionApply = null;
                    pendingFusionMessage = null;
                    pendingFusionName = null;
                    AddEventLog("クロス進化を見送り (条件は維持)");
                    // プロ改喁E 再オファー条件を�E示してプレイヤーが「どぁE��れ�E再表示できるか」を琁E��
                    ShowMessage("クロス進化見送り\n次の Lv UP で再オファー");
                    Time.timeScale = 1f;
                }
                UpdateUi();
            });
        }
    }

    void ActivateFusion(string message, Action apply)
    {
        var absorbPositions = new List<Vector3>();
        for (var i = 0; i < linkCompanions.Length; i++)
        {
            if (linkCompanions[i] != null && linkCompanions[i].gameObject.activeSelf)
                absorbPositions.Add(linkCompanions[i].position);
        }

        fusionActive = true;
        apply();
        fusionStyle = GetFusionStyle(fusionName);
        ApplyPlayerVisuals(true);
        AddEventLog(message);
        var accent = GetFormAccentColor();
        QueueEvolutionCutscene("CROSS EVOLVE", partnerName + " / " + fusionName, "メインキャラがリンクを取り込み、専用フォームへ", accent);
        Flash(new Color(accent.r, accent.g, accent.b, 0.52f), 1f);
        Shake(0.45f, 0.18f);
        SpawnSparks(player.position, accent, 76, 2.4f);
        PlaySfx("Fusion", 520f, 0.36f, 0.5f);

        foreach (var pos in absorbPositions)
        {
            SpawnSparks(pos, accent, 24, 1.6f);
            for (var t = 1; t <= 4; t++)
            {
                var lerpPos = Vector3.Lerp(pos, player.position, t / 5f);
                SpawnSparks(lerpPos, accent, 6, 0.7f);
            }
        }
    }

    void AnnounceSynergy(string name)
    {
        ShowMessage(name);
        AddEventLog(name);
        Flash(new Color(0.25f, 1f, 0.86f, 0.36f), 0.65f);
        SpawnSparks(player.position, new Color(0.25f, 1f, 0.86f), 28, 1.25f);
    }

    string GetStageName(int stage)
    {
        switch (stage)
        {
            case 1:
                return "バトルフォーム";
            case 2:
                return "アーマーフォーム";
            case 3:
                return "オーバーフォーム";
            default:
                return "プチフォーム";
        }
    }

    List<Upgrade> PickUpgrades(int count)
    {
        var copy = new List<Upgrade>();
        foreach (var upgrade in upgradePool)
        {
            if (CanOfferUpgrade(upgrade))
                copy.Add(upgrade);
        }

        if (fusionActive)
        {
            foreach (var upgrade in fusionUpgradePool)
            {
                if ((upgrade.requiredFusionStyle == 0 || upgrade.requiredFusionStyle == fusionStyle) && CanOfferUpgrade(upgrade))
                    copy.Add(upgrade);
            }
        }

        var result = new List<Upgrade>();
        for (var i = 0; i < count && copy.Count > 0; i++)
        {
            var index = rng.Next(copy.Count);
            result.Add(copy[index]);
            copy.RemoveAt(index);
        }
        if (result.Count == 0 && count > 0)
            result.Add(BuildEmergencySupplyUpgrade());
        return result;
    }

    // ─────────────────────────────────────────────────────────
    // Data Lab  EBrotato shop 風の wave 間ミニショチE�E
    // wave 2 / 8 終亁E��に3アイチE��から1つ購入 (data チップで支払い) or スキップ
    // 永続レリックよりライト、即晁E小規模ブ�Eスト中忁E    // ─────────────────────────────────────────────────────────
    sealed class DataLabItem
    {
        public readonly string title;
        public readonly string description;
        public readonly int cost;
        public readonly Action apply;
        public DataLabItem(string title, string description, int cost, Action apply)
        {
            this.title = title; this.description = description; this.cost = cost; this.apply = apply;
        }
    }

    List<DataLabItem> BuildDataLabPool()
    {
        var pool = new List<DataLabItem>
        {
            new DataLabItem("修復モジュール", "自分とコアを大きく修復。\n自分HP +3 / コアHP +4。", 20,
                () => { playerHp = Mathf.Min(playerMaxHp, playerHp + 3f); lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 4f); }),
            new DataLabItem("シールド強化", "防衛容量を恒久的に底上げ。\n自分最大HP +2 / コア最大HP +3 (現値も回復)。", 30,
                () => {
                    playerMaxHp += 2f; playerHp = Mathf.Min(playerMaxHp, playerHp + 2f);
                    lanternMaxHp += 3f; lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 3f);
                }),
            new DataLabItem("照射回路", "メイン弾を恒久強化。\n攻撃+10% / 弾速+6%。", 35,
                () => { bulletDamage *= 1.10f; bulletSpeed *= 1.06f; }),
            new DataLabItem("加速プロトコル", "機動と連射を強化。\n移動+8% / 連射+8%。", 35,
                () => { moveSpeed *= 1.08f; fireRate *= 1.08f; }),
            new DataLabItem("マグネット拡張", "データ回収を強化。\n回収範囲 +20% / データ取得+12%。", 25,
                () => { pickupRange *= 1.20f; dataMultiplier *= 1.12f; }),
            new DataLabItem("緊急バッテリー", "光源を拡張。\n光半径+12% / 自分HP +1 / コアHP +2。", 20,
                () => {
                    lightRadius *= 1.12f;
                    if (lantern != null) lantern.GetChild(0).localScale = Vector3.one * lightRadius * 1.75f;
                    playerHp = Mathf.Min(playerMaxHp, playerHp + 1f);
                    lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 2f);
                }),
        };
        return pool;
    }

    // 最初�EショチE�Eは Wave 4 に遁E��せる (Wave 2 だと最安アイチE�� 20 data も買えず体感が悪ぁE
    // Wave 4 starts around 25-30 data on average.
    bool dataLabShownWave4;
    bool dataLabShownWave8;

    bool ShouldOpenDataLab()
    {
        if (wave == 4 && !dataLabShownWave4) return true;
        if (wave == 8 && !dataLabShownWave8) return true;
        return false;
    }

    void MarkDataLabShown()
    {
        if (wave == 4) dataLabShownWave4 = true;
        else if (wave == 8) dataLabShownWave8 = true;
    }

    void OpenDataLab()
    {
        choosingRelic = true;  // ロジチE��共朁E 同じ「カード選択ロック」状態を再利用
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        UpdateModuleStatusSidePanel();
        SetChoiceFocus(true);
        // ── ショチE�EらしぁE��ールド系チE�Eマでモジュール選択と一目で区別 ──
        // モジュール選抁E= シアン系 (技術アチE�EグレーチE
        // Data Lab      = ゴールド系 (お��で買ぁE��ョチE�E)
        var goldBright = new Color(1f, 0.86f, 0.28f);
        var goldDim = new Color(1f, 0.74f, 0.34f);
        panelTitleText.text = "DATA LAB / SHOP";
        panelTitleText.color = goldBright;
        if (panelSubtitleText != null)
        {
            var balance = Mathf.FloorToInt(dataChips);
            var nextHint = wave == 4 ? "次のショップは Wave 8" : "ラン最後のショップ";
            panelSubtitleText.text = "所持データ: " + balance + "    1つ購入 or スキップ可    (" + nextHint + ")";
            panelSubtitleText.color = new Color(1f, 0.94f, 0.72f);
        }
        cardAppearStartTime = Time.unscaledTime;
        choiceClickGuardUntil = Time.unscaledTime + ChoiceClickGuardDuration;
        rerollButton.gameObject.SetActive(false);
        if (skipRewardButton != null)
            skipRewardButton.gameObject.SetActive(false);

        var pool = BuildDataLabPool();
        for (var r = pool.Count - 1; r > 0; r--)
        {
            var j = rng.Next(r + 1);
            var tmp = pool[r]; pool[r] = pool[j]; pool[j] = tmp;
        }

        var labCount = Mathf.Min(3, pool.Count);
        for (var p = 0; p < upgradeButtons.Count; p++)
        {
            var rt = upgradeButtons[p].GetComponent<RectTransform>();
            if (rt != null)
            {
                float x;
                if (labCount == 1)      x = 0f;
                else if (labCount == 2) x = -160f + p * 320f;
                else                    x = -300f + p * 300f;
                var newPos = new Vector2(x, -8f);
                rt.anchoredPosition = newPos;
                cardBasePositions[upgradeButtons[p]] = newPos;
            }
        }

        for (var i = 0; i < upgradeButtons.Count; i++)
        {
            if (i >= labCount)
            {
                upgradeButtons[i].gameObject.SetActive(false);
                continue;
            }
            upgradeButtons[i].gameObject.SetActive(true);
            var item = pool[i];
            var label = upgradeButtons[i].transform.Find("Label").GetComponent<Text>();
            var canAfford = dataChips >= item.cost;
            // ゴールド系カラー (モジュール選択�Eシアンと完�E刁E��)
            var baseColor = canAfford ? new Color(0.18f, 0.10f, 0.04f, 0.98f) : new Color(0.10f, 0.08f, 0.06f, 0.85f);
            var hoverColor = canAfford ? new Color(0.32f, 0.18f, 0.05f, 1f) : baseColor;
            var accent = canAfford ? goldBright : new Color(0.50f, 0.42f, 0.30f, 1f);
            var titleColor = canAfford ? new Color(1f, 0.94f, 0.65f) : new Color(0.60f, 0.55f, 0.42f);
            var priceTag = canAfford ? "💰 " + item.cost + " データ" : "[ 不足 ] " + item.cost + " データ";
            label.text = item.title + "\n" + priceTag + "\n\n" + item.description;
            ApplyCardVisual(upgradeButtons[i], label, baseColor, hoverColor, titleColor, accent, "SHOP", 1.5f);
            ApplyModuleCardIcon(upgradeButtons[i], item.title, accent);
            SetCardText(upgradeButtons[i], label, item.title, priceTag, item.description, titleColor, canAfford ? new Color(1f, 0.86f, 0.32f) : new Color(0.6f, 0.5f, 0.4f), accent, 1.5f);
            upgradeButtons[i].interactable = canAfford;
            upgradeButtons[i].onClick.RemoveAllListeners();
            var captured = item;
            upgradeButtons[i].onClick.AddListener(() =>
            {
                if (IsChoiceClickGuarded()) return;
                if (dataChips < captured.cost) return;
                dataChips -= captured.cost;
                captured.apply();
                AddEventLog("SHOP: " + captured.title + " (-" + captured.cost + " データ)");
                PlaySfx("Pickup", 720f, 0.10f, 0.20f);
                SpawnSparks(player.position, goldBright, 22, 1.2f);
                Flash(new Color(goldBright.r, goldBright.g, goldBright.b, 0.18f), 0.35f);
                CloseDataLabAndAdvance();
            });
        }

        // Skip ボタン (常に有効、ゴールドアクセンチE
        if (skipRewardButton != null)
        {
            skipRewardButton.gameObject.SetActive(true);
            var skipLabel = skipRewardButton.GetComponentInChildren<Text>();
            if (skipLabel != null) { skipLabel.text = "スキップ (購入しなぁE"; skipLabel.color = goldDim; }
            skipRewardButton.interactable = true;
            skipRewardButton.onClick.RemoveAllListeners();
            skipRewardButton.onClick.AddListener(() =>
            {
                if (IsChoiceClickGuarded()) return;
                AddEventLog("SHOP: スキップ");
                PlaySfx("Pickup", 540f, 0.05f, 0.10f);
                CloseDataLabAndAdvance();
            });
        }
    }

    void CloseDataLabAndAdvance()
    {
        MarkDataLabShown();
        upgradePanel.SetActive(false);
        choosingRelic = false;
        SetChoiceFocus(false);
        if (skipRewardButton != null)
            skipRewardButton.gameObject.SetActive(false);
        BeginWave(wave + 1);
        OpenUpgrade();
    }

    Upgrade BuildEmergencySupplyUpgrade()
    {
        return new Upgrade("緊急補給",
            "候補が尽きた時の保証。\nデータを回収し、自分とコアを少し修復する。",
            "★★★",
            () =>
            {
                var reward = GetSkipReward();
                dataChips += reward;
                playerHp = Mathf.Min(playerMaxHp, playerHp + 1f);
                lanternHp = Mathf.Min(lanternMaxHp, lanternHp + 2f);
                AddEventLog("SUPPLY: データ +" + reward);
            });
    }

    bool CanOfferUpgrade(Upgrade upgrade)
    {
        var title = upgrade.title;
        // プロ改喁E Banish 済みモジュールは除夁E(Brotato 流�E「永乁E��見なぁE��機�E)
        if (runBannedTitles != null && runBannedTitles.Contains(title))
            return false;
        if (UsesModuleLevel(upgrade) && GetModuleLevel(upgrade) >= MaxModuleLevel)
            return false;

        if (title.Contains("仲間リンク"))
        {
            if (fusionActive || CountActiveLinks() >= 2)
                return false;
            if (title.Contains("Nova") && allyNova)
                return false;
            if (title.Contains("Bulwark") && allyBulwark)
                return false;
            if (title.Contains("Siphon") && allySiphon)
                return false;
            if (title.Contains("Phase") && allyPhase)
                return false;
        }

        if (title.Contains("SPEED専用"))
        {
            if (formStyle != 1)
                return false;
            if (title.Contains("チェインスパーク") && speedChainBonus >= 2)
                return false;
            if (title.Contains("ミラージュビット") && speedEchoActive)
                return false;
        }

        if (title.Contains("GUARD専用"))
        {
            if (formStyle != 3)
                return false;
            if (title.Contains("コアパルス") && guardPulseUpgrade)
                return false;
            if (title.Contains("リフレクトウォール") && reflectShield)
                return false;
        }

        // ── Prerequisite gating: enhancement modules require their base ──
        if (title == "リング増幅"       && !orbitShield)                            return false;
        if (title == "バースト確率"    && explodeChance <= 0f)                     return false;
        if (title == "チェイン増幅"     && speedChainBonus <= 0)                    return false;
        if (title == "エコーレゾナンス" && !speedEchoActive)                        return false;
        if (title == "パルスチャージ"   && explodeChance <= 0f && !guardPulseUpgrade) return false;
        if (title == "フェーズリーチ"   && !allyPhase)                              return false;
        if (title == "カウンターブースト" && !reflectShield && contactBurst <= 0f)   return false;

        // ── Signature gating (キャラ固有機構を持ってる時だけ�E現) ──
        // Funnel系: Halo Caster (funnelBitCount > 0) のみ
        if (title == "スウォームプロトコル" && funnelBitCount <= 0)                  return false;
        if (title == "スウォームプロトコル" && funnelBitCount >= FunnelMaxBits)      return false; // 上限
        if (title == "ハイパーサイクル"     && funnelBitCount <= 0)                  return false;
        if (title == "ストライクキャスケード" && funnelBitCount <= 0)                return false;
        if (title == "キネティックスパイク" && funnelBitCount <= 0)                  return false;
        // Laser系: Pulse Hydra (laserActive) のみ
        if (title == "フォトンサージ"       && !laserActive)                         return false;
        if (title == "ホライゾンレンズ"     && !laserActive)                         return false;
        if (title == "スプリットビーム"     && !laserActive)                         return false;
        // Aura系: playerAuraDamage > 0 (Wraith Lynx / プラズマオーラ取得時)
        if (title == "フィールドプロジェクター" && playerAuraDamage <= 0f)           return false;
        if (title == "インフェルノハロー"   && playerAuraDamage <= 0f)               return false;
        // 吸血系: bulletLifesteal フラグ ON 時�Eみ
        if (title == "ヴァンパイアコア"     && !bulletLifesteal)                     return false;

        // ── パ�Eトナー固有シグネチャ (該彁EpartnerStyle のみ出現) ──
        if (title == "バランスチャージ"     && partnerStyle != 1)                    return false;
        if (title == "フレアバレット"       && partnerStyle != 2)                    return false;
        if (title == "エッグレゾナンス"     && partnerStyle != 3)                    return false;
        if (title == "チェインプロトコル"   && partnerStyle != 4)                    return false;
        if (title == "ファントムステップ"   && partnerStyle != 5)                    return false;
        if (title == "アーマーローチ"       && partnerStyle != 6)                    return false;
        if (title == "スラッシュサイクル"   && partnerStyle != 7)                    return false;
        if (title == "センチネルピッチ"   && funnelBitCount <= 0)                  return false;
        if (title == "コアハーモニクス"     && !laserActive)                         return false;
        if (title == "コアシンク"           && !solarAnchorActive)                   return false;

        return true;
    }

    void UpdateCamera()
    {
        var target = Vector3.Lerp(player.position, lantern.position, 0.35f);
        var shake = Vector3.zero;
        if (shakeTimer > 0f)
        {
            shake = new Vector3(Mathf.Sin(Time.time * 57f), Mathf.Cos(Time.time * 43f), 0f) * shakePower * (shakeTimer / Mathf.Max(0.01f, shakeDuration));
        }
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(target.x, target.y, -10f) + shake, Time.deltaTime * 2f);
    }

    void Flash(Color color, float duration)
    {
        if (!screenFlashEnabled || IsChoiceFocusActive())
            return;

        flashImage.color = color;
        flashTimer = duration;
        flashDuration = duration;
    }

    void ClearScreenFlash()
    {
        flashTimer = 0f;
        flashDuration = 0f;
        if (flashImage != null)
            flashImage.color = Color.clear;
    }

    void ClearDangerOverlay()
    {
        if (dangerImage != null)
            dangerImage.color = Color.clear;
    }

    void Shake(float duration, float power)
    {
        if (!screenShakeEnabled)
            return;

        shakeTimer = duration;
        shakeDuration = duration;
        shakePower = power;
    }

    void ApplyCameraZoom()
    {
        if (mainCamera != null)
            mainCamera.orthographicSize = CameraBaseSize / Mathf.Max(0.1f, cameraZoom);
    }

    // ── HitFreeze: brief time-stop for impact ──
    // Called on big hits (crit, boss damage, brute/boss kill)
    // durationSec: typical 0.03-0.08, real-time stop length
    void HitFreeze(float durationSec)
    {
        if (!hitFreezeEnabled) return;
        if (paused || choosingUpgrade || choosingRelic || gameOver || victory) return;
        if (durationSec <= 0f) return;
        // Extend if a longer freeze is already running
        if (hitFreezeTimer < durationSec)
        {
            hitFreezeTimer = durationSec;
            Time.timeScale = 0f;
        }
    }

    void UpdateScreenEffects()
    {
        UpdateAmbientNeon();

        if (IsChoiceFocusActive())
        {
            ClearScreenFlash();
        }
        else if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            var c = flashImage.color;
            c.a = Mathf.Lerp(0f, c.a, Mathf.Clamp01(flashTimer / Mathf.Max(0.01f, flashDuration)));
            flashImage.color = c;
        }
        else if (flashImage.color.a > 0f)
        {
            flashImage.color = Color.clear;
        }

        if (shakeTimer > 0f)
            shakeTimer -= Time.deltaTime;

        if (playerHitPulse > 0f)
            playerHitPulse -= Time.deltaTime;
        if (eggHitPulse > 0f)
            eggHitPulse -= Time.deltaTime;

        // Core Pulse relic
        if (corePulseRelicActive && !gameOver && !victory && !choosingUpgrade)
        {
            corePulseTimer -= Time.deltaTime;
            if (corePulseTimer <= 0f)
            {
                corePulseTimer = 6.5f;
                SpawnRingSparks(lantern.position, new Color(1f, 0.72f, 0.18f), 16, 2.1f);
                Flash(new Color(1f, 0.66f, 0.12f, 0.08f), 0.2f);
                for (var ci = enemies.Count - 1; ci >= 0; ci--)
                {
                    if (Vector2.Distance(enemies[ci].transform.position, lantern.position) < 2.65f)
                        DamageEnemy(enemies[ci], 1.45f, false);
                }
            }
        }

        // Eggcore damage state glow
        if (lanternGlowRenderer != null && !gameOver && !victory)
        {
            var hp01 = lanternHp / Mathf.Max(1f, lanternMaxHp);
            float ps, pa, ba; Color bc;
            if (hp01 < 0.3f)      { bc = new Color(1f, 0.16f, 0.06f); ps = 11f; pa = 0.16f; ba = 0.30f; }
            else if (hp01 < 0.6f) { bc = new Color(1f, 0.40f, 0.06f); ps = 6.5f; pa = 0.09f; ba = 0.22f; }
            else                  { bc = new Color(1f, 0.68f, 0.20f); ps = 3.5f; pa = 0.04f; ba = 0.17f; }
            bc.a = ba + Mathf.Sin(Time.time * ps) * pa + eggHitPulse * 0.20f;
            lanternGlowRenderer.color = bc;
        }

        UpdateCoreDamageCrackVisuals();
    }

    void UpdateCoreDamageCrackVisuals()
    {
        if (coreDamageCrack1Renderer == null && coreDamageCrack2Renderer == null && coreDamageCrack3Renderer == null)
            return;

        var hp01 = lanternHp / Mathf.Max(1f, lanternMaxHp);
        SetCoreDamageCrack(coreDamageCrack1Renderer, hp01 < 0.72f, Mathf.InverseLerp(0.72f, 0.42f, hp01), 0.34f);
        SetCoreDamageCrack(coreDamageCrack2Renderer, hp01 < 0.48f, Mathf.InverseLerp(0.48f, 0.22f, hp01), 0.48f);
        SetCoreDamageCrack(coreDamageCrack3Renderer, hp01 < 0.26f, Mathf.InverseLerp(0.26f, 0.02f, hp01), 0.66f);
    }

    void SetCoreDamageCrack(SpriteRenderer renderer, bool visible, float t, float maxAlpha)
    {
        if (renderer == null)
            return;

        renderer.enabled = visible;
        if (!visible)
            return;

        t = Mathf.Clamp01(t);
        var pulse = 0.72f + Mathf.Sin(Time.time * 8f + maxAlpha * 7f) * 0.28f;
        renderer.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.08f, maxAlpha, t) * pulse);
    }

    void UpdateAmbientNeon()
    {
        var time = Time.time;
        for (var i = ambientNeons.Count - 1; i >= 0; i--)
        {
            var neon = ambientNeons[i];
            if (neon.renderer == null)
            {
                ambientNeons.RemoveAt(i);
                continue;
            }

            var pulse = enhancedVisuals ? 0.72f + Mathf.Sin(time * 1.7f + neon.phase) * 0.28f : 0.34f;
            var color = neon.baseColor;
            color.a = neon.baseColor.a * pulse;
            neon.renderer.color = color;
        }
    }

    void PlayBgm()
    {
        if (!bgmEnabled || bgmSource == null)
            return;

        string bgmKey;
        if      (currentStageId == 1 && audioClips.ContainsKey("BGM_Stage2")) bgmKey = "BGM_Stage2";
        else if (currentStageId == 2 && audioClips.ContainsKey("BGM_Stage3")) bgmKey = "BGM_Stage3";
        else if (currentStageId == 3 && audioClips.ContainsKey("BGM_Stage4")) bgmKey = "BGM_Stage4";
        else if (currentStageId == 4 && audioClips.ContainsKey("BGM_Stage5")) bgmKey = "BGM_Stage5";
        else                                                                    bgmKey = "BGM";
        if (audioClips.TryGetValue(bgmKey, out var clip))
        {
            if (bgmSource.clip != clip)
            {
                bgmSource.clip = clip;
                if (bgmSource.isPlaying)
                    bgmSource.Stop();
            }
            if (!bgmSource.isPlaying)
                bgmSource.Play();
        }
    }

    void PlayBossEncounterBgm(bool isMidBoss)
    {
        if (!bgmEnabled || bgmSource == null)
            return;

        var bgmKey = GetBossEncounterBgmKey(isMidBoss);
        if (!audioClips.TryGetValue(bgmKey, out var clip))
            audioClips.TryGetValue(GetBossEncounterLegacyBgmKey(isMidBoss), out clip);

        if (clip != null)
        {
            if (bgmSource.clip != clip)
            {
                bgmSource.clip = clip;
                if (bgmSource.isPlaying)
                    bgmSource.Stop();
            }
            if (!bgmSource.isPlaying)
                bgmSource.Play();
            return;
        }

        PlayBgm();
    }

    void PlaySfx(string name, float fallbackFrequency, float fallbackDuration, float fallbackVolume)
    {
        if (!sfxEnabled)
            return;

        if (!CanPlaySfx(name))
            return;

        var volumeScale = GetSfxVolumeScale(name);
        if (sfxSource != null && audioClips.TryGetValue(name, out var clip))
        {
            sfxSource.PlayOneShot(clip, volumeScale);
            return;
        }

        PlayTone(fallbackFrequency, fallbackDuration, fallbackVolume * volumeScale);
    }

    bool CanPlaySfx(string name)
    {
        var minInterval = GetSfxMinInterval(name);
        if (minInterval <= 0f)
            return true;

        var now = Time.unscaledTime;
        if (sfxLastPlayedAt.TryGetValue(name, out var lastPlayedAt) && now - lastPlayedAt < minInterval)
            return false;

        sfxLastPlayedAt[name] = now;
        return true;
    }

    float GetSfxMinInterval(string name)
    {
        switch (name)
        {
            case "Hit": return 0.065f;
            case "Shoot": return 0.035f;
            case "Kill": return 0.045f;
            case "Pickup": return 0.028f;
            case "Boss": return 0.22f;
            case "Evolve": return 0.18f;
            case "Fusion": return 0.24f;
            case "LevelUp": return 0.16f;
            case "GameOver": return 0.25f;
            default: return 0.04f;
        }
    }

    float GetSfxVolumeScale(string name)
    {
        switch (name)
        {
            case "Hit": return 0.34f;
            case "Shoot": return 0.42f;
            case "Kill": return 0.55f;
            case "Pickup": return 0.45f;
            case "LevelUp": return 0.75f;
            case "Boss": return 0.72f;
            case "Evolve": return 0.86f;
            case "Fusion": return 0.90f;
            case "GameOver": return 0.75f;
            default: return 0.65f;
        }
    }

    void PlayTone(float frequency, float duration, float volume)
    {
        if (sfxSource == null)
            return;

        var sampleRate = 22050;
        var sampleCount = Mathf.Max(64, Mathf.RoundToInt(sampleRate * duration));
        var samples = new float[sampleCount];
        for (var i = 0; i < sampleCount; i++)
        {
            var t = i / (float)sampleRate;
            var fade = 1f - i / (float)sampleCount;
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * fade * volume;
        }

        var clip = AudioClip.Create("Tone", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        sfxSource.PlayOneShot(clip);
    }

    // ── Procedural Audio ──────────────────────────────────────────
    float PcmSin(float freq, float t) => Mathf.Sin(2f * Mathf.PI * freq * t);
    float PcmTri(float freq, float t) { var x = freq * t % 1f; return x < 0.5f ? 4f * x - 1f : 3f - 4f * x; }
    float PcmSq(float freq, float t) => Mathf.Sin(2f * Mathf.PI * freq * t) >= 0f ? 1f : -1f;
    float PcmNoise(int i) { var v = Mathf.Sin(i * 127.1f + 0.3f) * 43758.5453f; return 2f * (v - Mathf.Floor(v)) - 1f; }

    void GenerateProceduralAudio()
    {
        if (!audioClips.ContainsKey("BGM"))
            audioClips["BGM"] = MakeBgmClip();
        if (!audioClips.ContainsKey("BGM_Stage2"))
            audioClips["BGM_Stage2"] = MakeStage2BgmClip();
        if (!audioClips.ContainsKey("BGM_Stage3"))
            audioClips["BGM_Stage3"] = MakeStage3BgmClip();
        if (!audioClips.ContainsKey("BGM_Stage4"))
            audioClips["BGM_Stage4"] = MakeStage4BgmClip();
        if (!audioClips.ContainsKey("BGM_Stage5"))
            audioClips["BGM_Stage5"] = MakeStage5BgmClip();
        var seNames = new[] { "Shoot", "Hit", "Kill", "Pickup", "LevelUp", "Evolve", "Fusion", "Boss", "GameOver" };
        foreach (var seName in seNames)
            if (!audioClips.ContainsKey(seName))
                audioClips[seName] = MakeSeClip(seName);
    }

    AudioClip MakeBgmClip()
    {
        const int sr = 22050;
        const float duration = 32f;
        var n = (int)(sr * duration);
        var s = new float[n];

        for (var i = 0; i < n; i++)
        {
            var t = i / (float)sr;
            var beat = t % 1.15f;
            var breath = 0.72f + Mathf.Sin(t * 0.42f) * 0.28f;
            float sample = 0f;
            sample += PcmTri(82.41f, t) * 0.07f * breath;
            sample += PcmSin(110f, t) * 0.035f * breath;
            sample += PcmSin(164.81f, t) * 0.018f;
            if (beat < 0.08f)
                sample += PcmSin(62f - beat * 180f, t) * Mathf.Exp(-beat * 32f) * 0.12f;
            if ((t % 0.57f) < 0.024f)
                sample += PcmNoise(i) * 0.01f;
            s[i] = Mathf.Clamp(sample, -0.9f, 0.9f);
        }

        // Fade loop boundaries to avoid click
        var fade = (int)(sr * 0.04f);
        for (var i = 0; i < fade; i++)
        {
            var ft = i / (float)fade;
            s[i] *= ft;
            s[n - 1 - i] *= ft;
        }

        var clip = AudioClip.Create("BGM_Proc", n, 1, sr, false);
        clip.SetData(s, 0);
        return clip;
    }

    AudioClip MakeStage2BgmClip()
    {
        const int sr = 22050;
        const float duration = 32f;
        var n = (int)(sr * duration);
        var s = new float[n];
        for (var i = 0; i < n; i++)
        {
            var t = i / (float)sr;
            var pulse = t % 1.35f;
            var heat = 0.64f + Mathf.Sin(t * 0.31f) * 0.36f;
            float sample = 0f;
            sample += PcmTri(55f, t) * 0.075f * heat;
            sample += PcmSin(82.41f, t) * 0.035f;
            sample += PcmNoise(i) * 0.018f * heat;
            if (pulse < 0.12f)
                sample += PcmSin(46f - pulse * 80f, t) * Mathf.Exp(-pulse * 20f) * 0.13f;
            if ((t % 2.7f) < 0.22f)
                sample += PcmTri(138.59f, t) * Mathf.Exp(-(t % 2.7f) * 4.2f) * 0.022f;
            s[i] = Mathf.Clamp(sample, -0.9f, 0.9f);
        }

        var fade = (int)(sr * 0.04f);
        for (var i = 0; i < fade; i++)
        {
            var ft = i / (float)fade;
            s[i] *= ft;
            s[n - 1 - i] *= ft;
        }

        var clip = AudioClip.Create("BGM_Stage2_Proc", n, 1, sr, false);
        clip.SetData(s, 0);
        return clip;
    }

    // Stage 3 (Broken Core Network) — glitchy, corrupted electronic
    AudioClip MakeStage3BgmClip()
    {
        const int sr = 22050;
        const float duration = 32f;
        var n = (int)(sr * duration);
        var s = new float[n];
        for (var i = 0; i < n; i++)
        {
            var t = i / (float)sr;
            float sample = 0f;
            // Corrupted drone: slightly detuned triangle
            var corrupt = 1f + 0.04f * Mathf.Sin(t * 7.3f);
            sample += PcmTri(73.42f * corrupt, t) * 0.055f;
            sample += PcmSq(146.83f, t) * 0.018f * (0.5f + 0.5f * Mathf.Sin(t * 1.1f));
            // Glitch burst: irregular noise spike 1
            var glitchPhase = t % 0.73f;
            if (glitchPhase < 0.03f)
                sample += PcmNoise(i) * 0.08f * Mathf.Exp(-glitchPhase * 60f);
            // Glitch burst 2 (denser rhythm)
            var glitchPhase2 = t % 1.17f;
            if (glitchPhase2 < 0.015f)
                sample += PcmNoise(i + 3333) * 0.045f * Mathf.Exp(-glitchPhase2 * 100f);
            // Digital stutter on off-beat
            var beat = t % 0.85f;
            if (beat < 0.06f)
                sample += PcmSq(220f, t) * Mathf.Exp(-beat * 25f) * 0.025f;
            s[i] = Mathf.Clamp(sample, -0.9f, 0.9f);
        }
        var fade = (int)(sr * 0.04f);
        for (var i = 0; i < fade; i++) { var ft = i / (float)fade; s[i] *= ft; s[n - 1 - i] *= ft; }
        var clip = AudioClip.Create("BGM_Stage3_Proc", n, 1, sr, false);
        clip.SetData(s, 0);
        return clip;
    }

    // Stage 4 (Frost Vault) — cold, crystalline, sparse
    AudioClip MakeStage4BgmClip()
    {
        const int sr = 22050;
        const float duration = 32f;
        var n = (int)(sr * duration);
        var s = new float[n];
        for (var i = 0; i < n; i++)
        {
            var t = i / (float)sr;
            float sample = 0f;
            // Ice breath: slow low rumble
            var breath = 0.4f + 0.2f * Mathf.Sin(t * 0.18f);
            sample += PcmTri(36.71f, t) * 0.04f * breath;
            // Crystal bell: periodic high-frequency ping
            var bell = t % 3.8f;
            if (bell < 1.2f)
            {
                sample += PcmSin(1046.5f, t) * Mathf.Exp(-bell * 3.5f) * 0.06f;
                sample += PcmSin(784.0f, t) * Mathf.Exp(-bell * 4.8f) * 0.028f;
            }
            // Secondary bell (offset)
            var bell2 = (t + 1.9f) % 3.8f;
            if (bell2 < 0.8f)
                sample += PcmSin(1318.5f, t) * Mathf.Exp(-bell2 * 5.5f) * 0.032f;
            // Icy shimmer: high-freq tremolo
            sample += PcmSin(880f, t) * 0.008f * (0.5f + 0.5f * Mathf.Sin(t * 6.7f));
            s[i] = Mathf.Clamp(sample, -0.9f, 0.9f);
        }
        var fade = (int)(sr * 0.04f);
        for (var i = 0; i < fade; i++) { var ft = i / (float)fade; s[i] *= ft; s[n - 1 - i] *= ft; }
        var clip = AudioClip.Create("BGM_Stage4_Proc", n, 1, sr, false);
        clip.SetData(s, 0);
        return clip;
    }

    // Stage 5 (Storm Spire) — intense, electric, driving chaos
    AudioClip MakeStage5BgmClip()
    {
        const int sr = 22050;
        const float duration = 32f;
        var n = (int)(sr * duration);
        var s = new float[n];
        for (var i = 0; i < n; i++)
        {
            var t = i / (float)sr;
            float sample = 0f;
            // Driving pulse: fast rhythm base
            var pulse = t % 0.6f;
            sample += PcmTri(110f, t) * Mathf.Exp(-pulse * 12f) * 0.10f;
            // Electric crackle on every half-beat
            var crackPhase = t % 0.3f;
            if (crackPhase < 0.02f)
                sample += PcmNoise(i) * 0.07f * Mathf.Exp(-crackPhase * 80f);
            // Power chord layer (storm sustain)
            var storm = 0.7f + 0.3f * Mathf.Sin(t * 0.55f);
            sample += PcmSin(110f, t)    * 0.04f * storm;
            sample += PcmSin(164.81f, t) * 0.025f * storm;
            sample += PcmSin(220f, t)    * 0.02f  * storm;
            // Lightning accent: displaced transient
            var lightning = (t + 0.45f) % 1.2f;
            if (lightning < 0.04f)
                sample += (PcmSin(440f, t) + PcmNoise(i + 999)) * 0.5f * Mathf.Exp(-lightning * 50f) * 0.05f;
            s[i] = Mathf.Clamp(sample, -0.9f, 0.9f);
        }
        var fade = (int)(sr * 0.04f);
        for (var i = 0; i < fade; i++) { var ft = i / (float)fade; s[i] *= ft; s[n - 1 - i] *= ft; }
        var clip = AudioClip.Create("BGM_Stage5_Proc", n, 1, sr, false);
        clip.SetData(s, 0);
        return clip;
    }

    AudioClip MakeSeClip(string seName)
    {
        const int sr = 22050;
        int n; float[] s;

        switch (seName)
        {
            case "Shoot":
            {
                n = (int)(sr * 0.07f); s = new float[n];
                for (var i = 0; i < n; i++)
                {
                    var t = i / (float)sr;
                    var prog = t / 0.07f;
                    var freq = Mathf.Lerp(860f, 300f, prog * prog);
                    s[i] = PcmSq(freq, t) * (1f - prog) * 0.12f;
                }
                break;
            }
            case "Hit":
            {
                n = (int)(sr * 0.09f); s = new float[n];
                for (var i = 0; i < n; i++)
                {
                    var t = i / (float)sr;
                    var env = Mathf.Exp(-t * 18f);
                    s[i] = (PcmSin(180f, t) * 0.55f + PcmNoise(i) * 0.45f) * env * 0.19f;
                }
                break;
            }
            case "Kill":
            {
                n = (int)(sr * 0.18f); s = new float[n];
                for (var i = 0; i < n; i++)
                {
                    var t = i / (float)sr;
                    var freq = 420f + 60f * Mathf.Exp(-t * 22f);
                    var env = Mathf.Exp(-t * 9f);
                    s[i] = (PcmSin(freq, t) * 0.5f + PcmNoise(i) * 0.5f) * env * 0.21f;
                }
                break;
            }
            case "Pickup":
            {
                float[] chord = { 783.99f, 659.25f, 523.25f };
                n = (int)(sr * 0.18f); s = new float[n];
                for (var i = 0; i < n; i++)
                {
                    var t = i / (float)sr;
                    var seg = Mathf.Min((int)(t / 0.06f), chord.Length - 1);
                    var segT = t % 0.06f;
                    var env = 1f - segT / 0.06f;
                    s[i] = PcmSin(chord[seg], t) * env * 0.17f;
                }
                break;
            }
            case "LevelUp":
            {
                float[] notes = { 880f, 739.99f, 659.25f, 523.25f };
                n = (int)(sr * 0.42f); s = new float[n];
                for (var i = 0; i < n; i++)
                {
                    var t = i / (float)sr;
                    var seg = Mathf.Min((int)(t / 0.084f), notes.Length - 1);
                    var segT = t % 0.084f;
                    var env = segT < 0.01f ? segT / 0.01f : Mathf.Exp(-(segT - 0.01f) * 14f);
                    s[i] = (PcmTri(notes[seg], t) * 0.7f + PcmSin(notes[seg] * 2f, t) * 0.3f) * env * 0.17f;
                }
                break;
            }
            case "Evolve":
            {
                float[] chord = { 440f, 330f, 220f, 165f };
                n = (int)(sr * 0.58f); s = new float[n];
                for (var i = 0; i < n; i++)
                {
                    var t = i / (float)sr;
                    var env = t < 0.04f ? t / 0.04f : Mathf.Clamp01(1.2f - t * 1.95f);
                    var wave = 0f;
                    foreach (var f in chord)
                        wave += PcmTri(f, t);
                    s[i] = wave * env * 0.055f;
                }
                break;
            }
            case "Fusion":
            {
                n = (int)(sr * 0.52f); s = new float[n];
                for (var i = 0; i < n; i++)
                {
                    var t = i / (float)sr;
                    var kp = 2f * Mathf.PI * (55f * t + 2.8f * (1f - Mathf.Exp(-28f * t)));
                    var boom = Mathf.Sin(kp) * Mathf.Exp(-t * 5.5f) * 0.38f;
                    var noise = PcmNoise(i) * Mathf.Exp(-t * 13f) * 0.24f;
                    var chirp = PcmSq(Mathf.Lerp(720f, 220f, t * 4f), t) * Mathf.Exp(-t * 20f) * 0.055f;
                    s[i] = Mathf.Clamp(boom + noise + chirp, -0.9f, 0.9f);
                }
                break;
            }
            case "Boss":
            {
                n = (int)(sr * 0.72f); s = new float[n];
                for (var i = 0; i < n; i++)
                {
                    var t = i / (float)sr;
                    var env = t < 0.06f ? t / 0.06f : Mathf.Clamp01(1.3f - t * 1.6f);
                    var tremolo = 1f + 0.22f * Mathf.Sin(2f * Mathf.PI * 5.5f * t);
                    s[i] = (PcmTri(55f, t) * 0.5f + PcmSin(82.41f, t) * 0.3f + PcmSin(58.27f, t) * 0.2f)
                        * env * tremolo * 0.26f;
                }
                break;
            }
            case "GameOver":
            {
                float[] notes = { 329.63f, 293.66f, 261.63f, 220f };
                n = (int)(sr * 0.58f); s = new float[n];
                for (var i = 0; i < n; i++)
                {
                    var t = i / (float)sr;
                    var seg = Mathf.Min((int)(t / 0.145f), notes.Length - 1);
                    var segT = t % 0.145f;
                    var env = segT < 0.01f ? segT / 0.01f : Mathf.Exp(-(segT - 0.01f) * 7.5f);
                    s[i] = PcmTri(notes[seg], t) * env * 0.21f;
                }
                break;
            }
            default:
                n = (int)(sr * 0.08f); s = new float[n];
                for (var i = 0; i < n; i++)
                {
                    var t = i / (float)sr;
                    s[i] = PcmSin(440f, t) * (1f - t / 0.08f) * 0.14f;
                }
                break;
        }

        var clip = AudioClip.Create("SE_" + seName, n, 1, sr, false);
        clip.SetData(s, 0);
        return clip;
    }
    // ── End Procedural Audio ─────────────────────────────────────

    void AddEventLog(string message)
    {
        eventLog.Insert(0, message);
        while (eventLog.Count > 3)
            eventLog.RemoveAt(eventLog.Count - 1);
        UpdateEventLog();
    }

    void UpdateEventLog()
    {
        if (eventLogText == null)
            return;

        eventLogText.text = eventLog.Count == 0 ? "" : string.Join("\n", eventLog);
    }

    void ShowMessage(string text)
    {
        centerText.text = text;
        centerText.color = new Color(1f, 0.85f, 0.45f);
        messageTimer = 1.2f;
    }

    void QueueEvolutionCutscene(string title, string form, string desc, Color color)
    {
        queuedCutscene = true;
        queuedCutsceneTitle = title;
        queuedCutsceneName = form;
        queuedCutsceneDesc = desc;
        queuedCutsceneColor = color;
    }

    void BeginQueuedCutscene()
    {
        queuedCutscene = false;
        evolutionCutsceneTimer = evolutionCutsceneDuration;
        Time.timeScale = 0f;
        centerText.text = "";
        // 開幕�E派手な閁E�E + リングウェーチE+ SFX で「進化した」瞬間を強調
        Flash(new Color(queuedCutsceneColor.r, queuedCutsceneColor.g, queuedCutsceneColor.b, 0.55f), 0.50f);
        Shake(0.35f, 0.20f);
        if (player != null)
        {
            SpawnRingSparks(player.position, queuedCutsceneColor, 48, 2.4f);
            SpawnRingSparks(player.position, queuedCutsceneColor, 28, 3.4f);
            SpawnSparks(player.position, queuedCutsceneColor, 56, 1.8f);
        }
        PlaySfx("Evolve", 160f, 0.50f, 0.60f);
        PlaySfx("Hit", 320f, 0.20f, 0.32f);

        if (evolutionCutscenePanel == null)
        {
            Time.timeScale = 1f;
            return;
        }

        evolutionCutscenePanel.SetActive(true);
        evolutionCutsceneTitleText.text = queuedCutsceneTitle;
        evolutionCutsceneNameText.text = queuedCutsceneName;
        evolutionCutsceneDescText.text = queuedCutsceneDesc;
        evolutionCutsceneTitleText.color = Color.Lerp(Color.white, queuedCutsceneColor, 0.45f);
        evolutionCutsceneNameText.color = Color.Lerp(new Color(1f, 0.86f, 0.42f), queuedCutsceneColor, 0.35f);
        evolutionCutsceneDescText.color = new Color(0.86f, 1f, 0.96f);
        evolutionCutscenePortrait.sprite = playerRenderer != null ? playerRenderer.sprite : playerSprite;
        evolutionCutscenePortrait.preserveAspect = true;
        var cutinSprite = GetCurrentCutinSprite();
        if (cutinSprite != null)
        {
            ApplyScreenBackgroundSprite(evolutionCutsceneBack, cutinSprite, new Color(1f, 1f, 1f, 0.92f));
        }
        else
        {
            evolutionCutsceneBack.sprite = null;
            evolutionCutsceneBack.color = new Color(0.005f, 0.01f, 0.02f, 0.86f);
        }
        evolutionCutsceneRing.color = new Color(queuedCutsceneColor.r, queuedCutsceneColor.g, queuedCutsceneColor.b, 0.24f);
        if (evolutionCutsceneBeam != null)
            evolutionCutsceneBeam.color = new Color(queuedCutsceneColor.r, queuedCutsceneColor.g, queuedCutsceneColor.b, 0.26f);
        evolutionCutscenePortrait.color = Color.white;
    }

    void UpdateEvolutionCutscene()
    {
        var dt = Time.unscaledDeltaTime;
        evolutionCutsceneTimer -= dt;
        var t = 1f - Mathf.Clamp01(evolutionCutsceneTimer / Mathf.Max(0.01f, evolutionCutsceneDuration));
        var pulse = Mathf.Sin(t * Mathf.PI);

        if (evolutionCutscenePanel != null)
        {
            var backAlpha = Mathf.Lerp(0.92f, 0.7f, Mathf.Clamp01((t - 0.7f) / 0.3f));
            evolutionCutsceneBack.color = evolutionCutsceneBack.sprite != null
                ? new Color(1f, 1f, 1f, backAlpha)
                : new Color(0.005f, 0.01f, 0.02f, backAlpha);
            evolutionCutscenePortrait.rectTransform.localScale = Vector3.one * (0.72f + t * 0.48f + pulse * 0.12f);
            evolutionCutsceneRing.rectTransform.Rotate(0, 0, dt * 180f);
            evolutionCutsceneRing.rectTransform.localScale = Vector3.one * (0.85f + pulse * 0.25f);
            if (evolutionCutsceneBeam != null)
            {
                var beamRect = evolutionCutsceneBeam.rectTransform;
                beamRect.anchoredPosition = new Vector2(Mathf.Lerp(-70f, 70f, t), 20f + pulse * 8f);
                beamRect.localScale = new Vector3(1.02f + pulse * 0.18f, 0.72f + pulse * 0.34f, 1f);
                evolutionCutsceneBeam.color = new Color(queuedCutsceneColor.r, queuedCutsceneColor.g, queuedCutsceneColor.b, 0.12f + pulse * 0.32f);
            }
            var ringColor = queuedCutsceneColor;
            ringColor.a = 0.18f + pulse * 0.24f;
            evolutionCutsceneRing.color = ringColor;
        }

        if (evolutionCutsceneTimer > 0f)
            return;

        evolutionCutsceneTimer = 0f;
        if (evolutionCutscenePanel != null)
            evolutionCutscenePanel.SetActive(false);
        Time.timeScale = 1f;
        ShowMessage(queuedCutsceneName);
    }

    void UpdateUi()
    {
        messageTimer -= Time.deltaTime;
        if (messageTimer <= 0f && !gameOver && !victory)
            centerText.text = "";

        nameText.text = partnerName + " [" + partnerTrait + "]\n" + formName;
        waveText.text = "ウェーブ " + wave + " / " + MaxWave + "\n経過時間 " + FormatTime(elapsedTime);
        var playerRatio = Mathf.Clamp01(playerHp / Mathf.Max(1f, playerMaxHp));
        var eggRatio = Mathf.Clamp01(lanternHp / Mathf.Max(1f, lanternMaxHp));
        UpdateHpDisplay(playerHpFill, playerHpLagFill, playerHpValueText, playerWorldHpRoot, playerWorldHpFill, playerWorldHpFillRenderer, playerRatio, playerHp, playerMaxHp, new Color(0.22f, 0.83f, 1f), playerHitPulse, ref playerHpVisual);
        UpdateHpDisplay(eggHpFill, eggHpLagFill, eggHpValueText, eggWorldHpRoot, eggWorldHpFill, eggWorldHpFillRenderer, eggRatio, lanternHp, lanternMaxHp, new Color(1f, 0.8f, 0.18f), eggHitPulse, ref eggHpVisual);
        var xpRatio = Mathf.Clamp01(xp / Mathf.Max(1f, xpToLevel));
        dataFill.fillAmount = xpRatio;
        dataFill.color = Color.Lerp(new Color(0.34f, 0.9f, 0.34f), new Color(0.86f, 1f, 0.36f), Mathf.Clamp01(xpRatio * 0.8f + 0.2f));
        var chipCount = Mathf.FloorToInt(dataChips);
        resourceText.text = "データチップ  " + chipCount;
        if (chipHudText != null)
            chipHudText.text = "◀データチップ " + chipCount;
        levelText.text = "レベル " + level + "\nEXP " + Mathf.FloorToInt(xp) + " / " + Mathf.FloorToInt(xpToLevel);
        if (expValueText != null)
            expValueText.text = Mathf.FloorToInt(xp) + " / " + Mathf.FloorToInt(xpToLevel);
        statsText.text =
            "ATK       " + bulletDamage.ToString("0.0") + "      SHOT " + bulletCount + "\n" +
            "RATE      " + fireRate.ToString("0.0") + "      PIERCE " + bulletPierce + "\n" +
            "LIGHT     " + lightRadius.ToString("0.0") + "      PICK " + pickupRange.ToString("0.0") + "\n" +
            "BURST     " + Mathf.RoundToInt(explodeChance * 100f) + "%     SYNC " + CountActiveSynergies();
        UpdateStatusPanel();
        UpdateLoadoutPanel();
        UpdateEventLog();
        UpdateWaveProgress();
        UpdateDangerOverlay(playerRatio, eggRatio);
        controlText.text = "WASD / 矢印キー / 左クリック長押しで移動   攻撃は自動";
        ApplyOptions();

        if (bossEnemy != null && bossBarRoot.activeSelf)
            bossBarFill.fillAmount = Mathf.Clamp01(bossEnemy.hp / bossEnemy.maxHp);
    }

    void UpdateHpDisplay(Image fill, Image lagFill, Text valueText, Transform worldRoot, Transform worldFill, SpriteRenderer worldFillRenderer, float ratio, float current, float max, Color healthyColor, float hitPulse, ref float visualRatio)
    {
        if (ratio > visualRatio)
            visualRatio = ratio;
        else
            visualRatio = Mathf.MoveTowards(visualRatio, ratio, Time.deltaTime * 0.42f);

        var critical = ratio <= 0.32f;
        var warningPulse = critical ? 0.5f + Mathf.Sin(Time.time * 12f) * 0.5f : 0f;
        var damagePulse = Mathf.Clamp01(hitPulse * 2f);
        var dangerColor = Color.Lerp(new Color(1f, 0.52f, 0.16f), new Color(1f, 0.08f, 0.06f), warningPulse);
        var targetColor = critical ? dangerColor : healthyColor;
        targetColor = Color.Lerp(targetColor, Color.white, damagePulse * 0.55f);

        fill.fillAmount = ratio;
        fill.color = targetColor;
        if (lagFill != null)
            lagFill.fillAmount = visualRatio;
        if (valueText != null)
        {
            valueText.text = Mathf.CeilToInt(Mathf.Max(0f, current)) + " / " + Mathf.CeilToInt(max);
            valueText.color = critical ? Color.Lerp(Color.white, dangerColor, 0.65f) : Color.white;
        }

        UpdateWorldHpBar(worldRoot, worldFill, worldFillRenderer, ratio, targetColor);
    }

    void UpdateLoadoutPanel()
    {
        if (loadoutText == null)
            return;

        var ringState = orbitShield ? "RING ON" : "RING -";
        var burstState = explodeChance > 0f ? "BURST " + Mathf.RoundToInt(explodeChance * 100f) + "%" : "BURST -";
        var crossState = fusionActive ? "CROSS " + fusionName : IsFusionReady() ? "CROSS READY" : "CROSS -";
        loadoutText.text = GetRouteLabel() + "   BIT x" + bulletCount + "   ATK " + bulletDamage.ToString("0.0") + "\n" + ringState + "   " + burstState + "   " + crossState;
        loadoutText.color = fusionActive
            ? new Color(1f, 0.78f, 1f)
            : IsFusionReady()
                ? new Color(0.82f, 1f, 0.45f)
                : new Color(0.88f, 1f, 0.95f);
    }

    string GetRouteLabel()
    {
        if (fusionActive)
            return "CROSS";
        if (formStyle == 1)
            return "SPEED";
        if (formStyle == 2)
            return "POWER";
        if (formStyle == 3)
            return "GUARD";
        return "BASE";
    }

    void UpdateWorldHpBar(Transform root, Transform fill, SpriteRenderer renderer, float ratio, Color color)
    {
        if (root == null || fill == null)
            return;

        if (root.parent != null)
        {
            var parentScale = Mathf.Max(0.01f, root.parent.localScale.x);
            root.localScale = Vector3.one / parentScale;
        }

        var baseWidth = root == playerWorldHpRoot ? 1.08f : 1.55f;
        fill.localScale = new Vector3(Mathf.Max(0.02f, baseWidth * ratio), 0.048f, 1f);
        fill.localPosition = new Vector3(-baseWidth * (1f - ratio) * 0.5f, 0f, 0f);
        if (renderer != null)
            renderer.color = color;
        root.gameObject.SetActive(ratio < 0.995f || root == eggWorldHpRoot || gameOver || victory);
    }

    void UpdateWaveProgress()
    {
        if (waveProgressRoot == null)
            return;

        var remaining = enemiesToSpawn + enemies.Count;
        // remaining-as-fraction: bar starts full, drains as wave progresses
        var remainRatio = 1f;
        if (wave == MaxWave && bossEnemy != null)
            remainRatio = Mathf.Clamp01(bossEnemy.hp / Mathf.Max(1f, bossEnemy.maxHp));
        else if (enemiesTotalThisWave > 0)
            remainRatio = 1f - Mathf.Clamp01(enemiesKilledThisWave / (float)enemiesTotalThisWave);

        waveProgressFill.fillAmount = remainRatio;
        var useHud9WaveProgress = UseHud9SliceCandidates && hud9WaveProgressFrameSprite != null;
        var progressSprite = useHud9WaveProgress
            ? (waveTrait == "Boss" && hud9WaveProgressBossSprite != null
                ? hud9WaveProgressBossSprite
                : hud9WaveProgressNormalSprite)
            : (UseWaveProgressHudV2
                ? (waveTrait == "Boss" && hudWaveProgressBossV2Sprite != null
                    ? hudWaveProgressBossV2Sprite
                    : hudWaveProgressNormalV2Sprite)
                : (waveTrait == "Boss" && hudWaveProgressBossSprite != null
                    ? hudWaveProgressBossSprite
                    : hudWaveProgressNormalSprite));
        var useProgressSprite = useHud9WaveProgress || UseWaveProgressHudV2 || UseGeneratedHudBars;
        if (useProgressSprite && progressSprite != null && waveProgressFill.sprite != progressSprite)
            waveProgressFill.sprite = progressSprite;
        else if (!useProgressSprite && waveProgressFill.sprite != null)
            waveProgressFill.sprite = null;
        // Color shifts as bar depletes: full=trait color, low=green (almost done)
        var baseColor = waveTrait == "Boss" ? new Color(1f, 0.24f, 0.88f, 0.95f)
            : waveTrait == "Rush" ? new Color(1f, 0.64f, 0.22f, 0.95f)
            : waveTrait == "Berserk" ? new Color(1f, 0.18f, 0.18f, 0.95f)
            : waveTrait == "Dark Field" ? new Color(0.28f, 0.32f, 1f, 0.95f)
            : waveTrait == "Iron Skin" ? new Color(0.65f, 0.65f, 0.75f, 0.95f)
            : new Color(0.2f, 0.88f, 1f, 0.95f);
        // When < 25% remaining, shift toward green to signal "almost cleared"
        var clearColor = new Color(0.42f, 1f, 0.4f, 0.95f);
        var lerpT = remainRatio < 0.25f ? Mathf.InverseLerp(0.25f, 0.05f, remainRatio) : 0f;
        waveProgressFill.color = Color.Lerp(baseColor, clearColor, lerpT);
        waveProgressText.text = "WAVE " + wave + " / " + MaxWave + "   " + waveTrait + "   残り " + remaining;
    }

    void UpdateDangerOverlay(float playerRatio, float eggRatio)
    {
        if (dangerImage == null)
            return;
        if (IsChoiceFocusActive())
        {
            ClearDangerOverlay();
            return;
        }

        var danger = Mathf.Clamp01((0.38f - Mathf.Min(playerRatio, eggRatio)) / 0.38f);
        var pulse = 0.72f + Mathf.Sin(Time.time * 9f) * 0.28f;
        var coreCritical = eggRatio < playerRatio;
        var dangerSprite = coreCritical ? hudDangerCoreSprite : hudDangerPlayerSprite;
        if (UseGeneratedOverlayImages && dangerSprite != null)
        {
            dangerImage.sprite = dangerSprite;
            dangerImage.type = Image.Type.Simple;
            dangerImage.preserveAspect = false;
            dangerImage.color = new Color(1f, 1f, 1f, 0.42f * danger * pulse);
        }
        else
        {
            dangerImage.sprite = null;
            dangerImage.color = coreCritical
                ? new Color(1f, 0.62f, 0.02f, 0.16f * danger * pulse)
                : new Color(1f, 0.04f, 0.02f, 0.16f * danger * pulse);
        }
    }

    string BuildSynergyText()
    {
        var text = "";
        if (scatterSynergy)
            text += "Scatter Storm\n";
        if (lanceSynergy)
            text += "Core Lance\n";
        if (recoverySynergy)
            text += "Regen Loop\n";
        if (aegisSynergy)
            text += "Aegis Ring\n";
        return string.IsNullOrEmpty(text) ? "未発動" : text.TrimEnd();
    }

    void UpdateStatusPanel()
    {
        for (var i = 0; i < evolutionDots.Count; i++)
        {
            evolutionDots[i].color = i <= evolutionStage
                ? new Color(0.3f, 1f, 0.95f, 1f)
                : new Color(0.1f, 0.16f, 0.18f, 0.9f);
        }

        UpdateLinkSlot(0, "NOVA", allyNova, new Color(0.4f, 0.95f, 1f));
        UpdateLinkSlot(1, "BULWARK", allyBulwark, new Color(0.4f, 1f, 0.55f));
        UpdateLinkSlot(2, "SIPHON", allySiphon, new Color(0.95f, 1f, 0.35f));
        UpdateLinkSlot(3, "PHASE", allyPhase, new Color(0.78f, 0.48f, 1f));

        var linkCount = CountActiveLinks();
        var fusionReady = IsFusionReady();
        var fusionRatio = fusionActive ? 1f : Mathf.Clamp01(linkCount * 0.2f + (evolutionStage >= 2 ? 0.2f : 0f) + (fusionReady ? 0.3f : 0f));
        if (fusionProgressFill != null)
        {
            fusionProgressFill.fillAmount = fusionRatio;
            fusionProgressFill.color = fusionActive
                ? new Color(1f, 0.38f, 0.96f, 0.98f)
                : fusionReady
                    ? new Color(0.75f, 1f, 0.35f, 0.98f)
                    : new Color(0.22f, 0.64f, 0.82f, 0.9f);
        }

        if (fusionProgressText != null)
            fusionProgressText.text = fusionActive ? "CROSS EVOLVE: " + fusionName : fusionReady ? "CROSS EVOLVE READY" : "CROSS EVOLVE BUILDING";

        statusText.text = "FORM " + GetStageName(evolutionStage) + "   LINKS " + linkCount + (fusionActive ? " / LOCK" : " / 2");
        synergyText.text = BuildFusionGuide();

        if (relicDisplayText != null)
        {
            if (relicSet.Count == 0)
            {
                relicDisplayText.text = "";
            }
            else
            {
                var names = new System.Text.StringBuilder("◀");
                var first = true;
                var relicNames = new System.Collections.Generic.Dictionary<string, string>
                {
                    {"magnet","磁力核"}, {"deathless","不滅の証"}, {"core_pulse","コアの鼓動"},
                    {"explosion","爆発の紋章"}, {"storm","嵐の記憶"}, {"mirror","鏡の誓約"},
                    {"titan","鋼鉄の盟約"}, {"overdrive","オーバードライブ"}, {"data_surge","データサージ"},
                    {"apex","APEX CORE"}
                };
                foreach (var id in relicSet)
                {
                    if (!first) names.Append("  ◀");
                    names.Append(relicNames.TryGetValue(id, out var n) ? n : id);
                    first = false;
                }
                relicDisplayText.text = names.ToString();
            }
        }
        UpdateRelicHudSlots();
    }

    void UpdateRelicHudSlots()
    {
        if (relicHudSlotFrames.Count == 0)
            return;

        var ids = new List<string>();
        foreach (var id in relicSet)
            ids.Add(id);

        for (var i = 0; i < relicHudSlotFrames.Count; i++)
        {
            var hasRelic = i < ids.Count;
            var frame = relicHudSlotFrames[i];
            if (frame != null)
                frame.color = hasRelic ? Color.white : new Color(0.15f, 0.2f, 0.16f, 0.58f);

            if (i >= relicHudSlotIcons.Count || relicHudSlotIcons[i] == null)
                continue;

            var icon = relicHudSlotIcons[i];
            if (!hasRelic)
            {
                icon.color = Color.clear;
                continue;
            }

            var sprite = GetRelicSprite(ids[i]);
            icon.sprite = sprite != null ? sprite : diamondSprite;
            icon.color = Color.white;
        }
    }

    void UpdateLinkSlot(int index, string name, bool active, Color activeColor)
    {
        if (index >= linkSlotImages.Count || index >= linkSlotTexts.Count)
            return;

        var pulse = active ? 0.8f + Mathf.Sin(Time.time * 6f + index) * 0.2f : 0f;
        linkSlotImages[index].color = active
            ? Color.Lerp(new Color(activeColor.r * 0.16f, activeColor.g * 0.18f, activeColor.b * 0.2f, 0.95f), activeColor, 0.28f + pulse * 0.12f)
            : new Color(0.035f, 0.055f, 0.07f, 0.9f);
        var outline = linkSlotImages[index].GetComponent<Outline>();
        if (outline != null)
            outline.effectColor = active ? WithAlpha(activeColor, 0.66f + pulse * 0.18f) : new Color(0.24f, 0.42f, 0.5f, 0.45f);
        if (index < linkSlotIconImages.Count)
        {
            linkSlotIconImages[index].color = active ? Color.Lerp(Color.white, activeColor, 0.36f) : new Color(0.18f, 0.25f, 0.28f, 0.9f);
            linkSlotIconImages[index].transform.localScale = Vector3.one * (active ? 1f + pulse * 0.08f : 0.92f);
        }
        var displayName = name == "BULWARK" ? "BULW" : name == "SIPHON" ? "SIPH" : name;
        var role = index == 0 ? "ATK" : index == 1 ? "DEF" : index == 2 ? "GET" : "EVADE";
        linkSlotTexts[index].text = displayName + "\n" + (active ? role : "EMPTY");
        linkSlotTexts[index].color = active ? Color.white : new Color(0.55f, 0.65f, 0.68f);
    }

    int CountActiveLinks()
    {
        var count = 0;
        if (allyNova) count++;
        if (allyBulwark) count++;
        if (allySiphon) count++;
        if (allyPhase) count++;
        return count;
    }

    bool IsFusionReady()
    {
        if (fusionActive || evolutionStage < 2)
            return fusionActive;

        return allyNova && allyBulwark && bulletCount >= 3
            || allyNova && allySiphon && dataMultiplier >= 1.25f
            || allyBulwark && allySiphon && lanternMaxHp >= 25f
            || allyNova && allyPhase && moveSpeed >= 5.4f
            || allyBulwark && allyPhase && playerMaxHp >= 8f
            || allySiphon && allyPhase && pickupRange >= 1.4f;
    }

    string BuildFusionGuide()
    {
        if (fusionActive)
            return "相棒がリンクを吸収。専用モジュール出現";
        if (evolutionStage < 2)
            return "Lv6進化でクロス進化解禁";
        if (CountActiveLinks() < 2)
            return "仲間リンクは2種類まで。組み合わせでクロス先が固定";
        if (allyNova && allyBulwark)
            return bulletCount >= 3 ? "Nova Aegis へクロス進化可能" : "条件 Nova+Bulwark: 弾数3が必要";
        if (allyNova && allySiphon)
            return dataMultiplier >= 1.25f ? "Photon Siphon へクロス進化可能" : "条件 Nova+Siphon: データ強化が必要";
        if (allyBulwark && allySiphon)
            return lanternMaxHp >= 25f ? "Core Bastion へクロス進化可能" : "条件 Bulwark+Siphon: コアHP25が必要";
        if (allyNova && allyPhase)
            return moveSpeed >= 5.4f ? "Nova Phantom へクロス進化可能" : "条件 Nova+Phase: 移動5.4以上が必要";
        if (allyBulwark && allyPhase)
            return playerMaxHp >= 8f ? "Aegis Drift へクロス進化可能" : "条件 Bulwark+Phase: 自分HP8以上が必要";
        if (allySiphon && allyPhase)
            return pickupRange >= 1.4f ? "Photon Wraith へクロス進化可能" : "条件 Siphon+Phase: 回収範囲1.4以上が必要";
        return "別タイプの仲間リンクを相棒へ接続";
    }

    int CountActiveSynergies()
    {
        var count = 0;
        if (scatterSynergy) count++;
        if (lanceSynergy) count++;
        if (recoverySynergy) count++;
        if (aegisSynergy) count++;
        return count;
    }

    string FormatTime(float seconds)
    {
        var total = Mathf.FloorToInt(seconds);
        return (total / 60).ToString("00") + ":" + (total % 60).ToString("00");
    }

    void LoadProgress()
    {
        LoadOptions();
        bestWave = PlayerPrefs.GetInt("BestWave", 0);
        var clears = PlayerPrefs.GetInt("Clears", 0);
        // Danger Level: max unlocked plus last selected value.
        dangerMaxCleared = Mathf.Clamp(PlayerPrefs.GetInt(DangerMaxClearedKey, 0), 0, MaxDangerLevel);
        var savedSelected = PlayerPrefs.GetInt(DangerSelectedKey, 0);
        // 選択可能上限 = クリア渁E1 (まだ未クリアの次の難度まで挑戦可)
        var selectableMax = Mathf.Min(MaxDangerLevel, dangerMaxCleared + 1);
        dangerLevel = Mathf.Clamp(savedSelected, 0, selectableMax);
        var savedStageMax = PlayerPrefs.GetInt(StageMaxUnlockedKey, MaxStageId);
        var legacyStageMax = clears > 0 || bestWave >= MaxWave ? MaxStageId : 0;
        stageMaxUnlocked = Mathf.Clamp(Mathf.Max(savedStageMax, legacyStageMax), 0, MaxStageId);
        if (stageMaxUnlocked != savedStageMax)
            PlayerPrefs.SetInt(StageMaxUnlockedKey, stageMaxUnlocked);
        var savedStage = PlayerPrefs.GetInt(StageSelectedKey, 0);
        currentStageId = Mathf.Clamp(savedStage, 0, stageMaxUnlocked);
        ApplyStageConfig(currentStageId);
        collectionSummary = "Clears " + clears + "    Missions " + CountCompletedMissions() + " / " + BuildMissionDefinitions().Length;
    }

    void SaveProgress(bool cleared)
    {
        runUnlockedMissions.Clear();
        if (wave > PlayerPrefs.GetInt("BestWave", 0))
            PlayerPrefs.SetInt("BestWave", wave);
        if (cleared)
        {
            PlayerPrefs.SetInt("Clears", PlayerPrefs.GetInt("Clears", 0) + 1);
            // Danger 解放: 今回クリアした難度めEmax として記録 →次囁EN+1 まで選択可
            if (dangerLevel > dangerMaxCleared)
            {
                dangerMaxCleared = Mathf.Min(MaxDangerLevel, dangerLevel);
                PlayerPrefs.SetInt(DangerMaxClearedKey, dangerMaxCleared);
            }
            // Stage 解放: 現在ステージをクリアすると次ステージ解放 (Stage 1 クリア →Stage 2 解放)
            if (currentStageId >= stageMaxUnlocked && stageMaxUnlocked < MaxStageId)
            {
                stageMaxUnlocked = Mathf.Min(MaxStageId, currentStageId + 1);
                PlayerPrefs.SetInt(StageMaxUnlockedKey, stageMaxUnlocked);
            }
        }
        // 選択状態も保孁E(タイトル戻り時に維持E
        PlayerPrefs.SetInt(DangerSelectedKey, dangerLevel);
        PlayerPrefs.SetInt(StageSelectedKey, currentStageId);
        PlayerPrefs.SetInt("Partner_" + partnerName, 1);
        if (formStyle == 1)
            PlayerPrefs.SetInt("Route_SPEED", 1);
        if (formStyle == 2)
            PlayerPrefs.SetInt("Route_POWER", 1);
        if (formStyle == 3)
            PlayerPrefs.SetInt("Route_GUARD", 1);
        if (fusionActive)
            PlayerPrefs.SetInt("Fusion_" + fusionName, 1);
        EvaluateMissions(cleared);
        PlayerPrefs.Save();
        LoadProgress();
    }

    void EvaluateMissions(bool cleared)
    {
        var defs = BuildMissionDefinitions();
        for (var i = 0; i < defs.Length; i++)
        {
            var def = defs[i];
            if (IsMissionCompleted(def.id) || def.isComplete == null || !def.isComplete(cleared))
                continue;
            PlayerPrefs.SetInt(GetMissionPrefKey(def.id), 1);
            runUnlockedMissions.Add(def.title);
        }
    }

    void ShowResult(bool cleared)
    {
        Time.timeScale = 0f;
        paused = false;
        choosingUpgrade = false;
        centerText.text = "";
        ClearScreenFlash();
        ClearDangerOverlay();
        if (bossBarRoot != null)
            bossBarRoot.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
        if (partnerPanel != null)
            partnerPanel.SetActive(false);
        if (choiceFocusOverlay != null)
            choiceFocusOverlay.SetActive(false);

        if (resultPanel != null)
            resultPanel.SetActive(true);
        if (resultRetryButton != null)
            resultRetryButton.interactable = true;
        if (resultMenuButton != null)
            resultMenuButton.interactable = true;
        if (resultBackgroundImage != null)
        {
            var resultSprite = UseGeneratedBackgrounds ? (cleared ? resultClearBackgroundSprite : resultGameOverBackgroundSprite) : null;
            if (UseGeneratedBackgrounds && resultSprite != null)
                ApplyScreenBackgroundSprite(resultBackgroundImage, resultSprite, Color.white);
            else
            {
                resultBackgroundImage.sprite = null;
                resultBackgroundImage.color = cleared ? new Color(0f, 0.02f, 0.018f, 0.94f) : new Color(0.03f, 0.006f, 0.012f, 0.94f);
            }
        }

        var accent = GetFormAccentColor();
        if (resultTitleText != null)
        {
            resultTitleText.text = cleared ? "PROTOCOL COMPLETE" : playerHp <= 0f ? "LINK LOST" : "EGGCORE BROKEN";
            resultTitleText.color = cleared ? new Color(0.55f, 1f, 0.74f) : new Color(1f, 0.45f, 0.36f);
        }

        if (resultPortraitImage != null)
        {
            resultPortraitImage.sprite = playerRenderer != null && playerRenderer.sprite != null ? playerRenderer.sprite : playerSprite;
            resultPortraitImage.color = Color.white;
        }
        if (resultPortraitGlow != null)
            resultPortraitGlow.color = new Color(accent.r, accent.g, accent.b, fusionActive ? 0.36f : 0.24f);

        if (resultRouteBadgePanel != null)
        {
            var routeAccent = GetRouteAccentColor();
            if (resultRouteBadgeImage != null)
                resultRouteBadgeImage.color = routeAccent;
            if (resultRouteBadgeText != null)
            {
                resultRouteBadgeText.color = Color.Lerp(Color.white, routeAccent, 0.4f);
                resultRouteBadgeText.text = GetRouteLabel() + " / " + GetStageName(evolutionStage);
            }
            var routeOutline = resultRouteBadgePanel.GetComponent<Outline>();
            if (routeOutline != null)
                routeOutline.effectColor = WithAlpha(routeAccent, 0.58f);
        }

        if (resultFusionBadgePanel != null)
        {
            var fusionAccent = fusionActive ? GetFormAccentColor() : new Color(0.4f, 0.5f, 0.55f);
            resultFusionBadgePanel.SetActive(true);
            if (resultFusionBadgeImage != null)
                resultFusionBadgeImage.color = fusionAccent;
            if (resultFusionBadgeText != null)
            {
                resultFusionBadgeText.color = fusionActive ? Color.Lerp(Color.white, fusionAccent, 0.5f) : new Color(0.55f, 0.7f, 0.74f);
                resultFusionBadgeText.text = fusionActive ? "CROSS / " + fusionName : "CROSS / LOCKED";
            }
            var fusionOutline = resultFusionBadgePanel.GetComponent<Outline>();
            if (fusionOutline != null)
                fusionOutline.effectColor = WithAlpha(fusionAccent, fusionActive ? 0.62f : 0.28f);
        }

        SetResultStat(0, "WAVE", wave + " / " + MaxWave);
        SetResultStat(1, "TIME", FormatTime(elapsedTime));
        SetResultStat(2, "KILLS", runEnemiesKilled.ToString());
        SetResultStat(3, "DATA", Mathf.FloorToInt(dataChips).ToString());
        SetResultStat(4, "DAMAGE", Mathf.FloorToInt(runDamageDealt).ToString());
        var topWaveStr = runMaxWaveKills > 0 ? "W" + runMaxKillWave + " / " + runMaxWaveKills : "--";
        var resultRank = BuildResultRank(cleared);
        SetResultStat(5, "RANK", resultRank);
        if (resultRankMedalImage != null)
        {
            var rankSprite = GetResultRankMedalSprite(resultRank);
            resultRankMedalImage.sprite = rankSprite;
            resultRankMedalImage.gameObject.SetActive(rankSprite != null);
            resultRankMedalImage.color = new Color(1f, 1f, 1f, cleared ? 0.48f : 0.34f);
        }
        if (resultRunSummaryText != null)
        {
            var coreRatio = Mathf.RoundToInt(lanternHp / Mathf.Max(1f, lanternMaxHp) * 100);
            var bossStr = runBossKilled > 0 ? "BOSS撃破" : "BOSS未撃破";
            // プロ改喁E 最大ストリーク + 死因 (敗北晁E をサマリに追加
            var streakStr = runMaxKillStreak >= 5 ? "  ·  MAX " + runMaxKillStreak + " STREAK" : "";
            var deathLine = "";
            if (!cleared && !string.IsNullOrEmpty(runDeathCause))
                deathLine = "\n💀 死因: " + runDeathCause;
            resultRunSummaryText.text = "コア " + coreRatio + "%  ·  " + bossStr + "  ·  BEST " + topWaveStr + streakStr +
                                        deathLine +
                                        "\nNEXT  " + BuildResultNextGoal(cleared);
            resultRunSummaryText.color = coreRatio <= 0 ? new Color(1f, 0.38f, 0.28f) : coreRatio < 40 ? new Color(1f, 0.78f, 0.28f) : new Color(0.72f, 1f, 0.9f);
        }

        var mvp = BuildMvpList();
        for (var i = 0; i < resultMvpCardRoots.Length; i++)
        {
            if (resultMvpCardRoots[i] == null)
                continue;

            if (i >= mvp.Count)
            {
                resultMvpCardRoots[i].SetActive(false);
                continue;
            }

            resultMvpCardRoots[i].SetActive(true);
            var entry = mvp[i];
            var mvpAccent = entry.accent;
            if (resultMvpIcons[i] != null)
            {
                resultMvpIcons[i].sprite = entry.iconSprite;
                resultMvpIcons[i].color = mvpAccent;
            }
            if (resultMvpTitleTexts[i] != null)
            {
                resultMvpTitleTexts[i].text = entry.title;
                resultMvpTitleTexts[i].color = Color.Lerp(new Color(1f, 0.96f, 0.78f), mvpAccent, 0.45f);
            }
            if (resultMvpDetailTexts[i] != null)
            {
                resultMvpDetailTexts[i].text = entry.detail;
                resultMvpDetailTexts[i].color = new Color(0.8f, 0.94f, 1f);
            }
            var outline = resultMvpCardRoots[i].GetComponent<Outline>();
            if (outline != null)
                outline.effectColor = WithAlpha(mvpAccent, 0.6f);
        }

        if (resultBodyText != null)
        {
            var summary = partnerName + " [" + partnerTrait + "]  ·  " + formName +
                          "  ·  STAGE " + stageName + "  D" + dangerLevel;
            var buildLine = "LINKS " + CountActiveLinks() + " / 4" +
                            "  ·  SYNC " + CountActiveSynergies() +
                            (fusionActive ? "  ·  " + fusionName : "  ·  CROSS未完了") +
                            (lastStandUsed ? "  ·  LAST STAND使用済" : "  ·  LAST STAND温存") +
                            (cleared ? "  ·  図鑑登録" : "  ·  記録保存");
            if (runUnlockedMissions.Count > 0)
                buildLine += "  ·  MISSION +" + runUnlockedMissions.Count;
            resultBodyText.text = summary + "\n" + buildLine;
            resultBodyText.rectTransform.sizeDelta = new Vector2(820, 42);
            resultBodyText.color = runUnlockedMissions.Count > 0 ? new Color(1f, 0.95f, 0.55f) : cleared ? new Color(0.86f, 1f, 0.95f) : new Color(1f, 0.86f, 0.78f);
        }
        ApplyOptions();
    }

    string BuildResultRank(bool cleared)
    {
        var waveRatio = Mathf.Clamp01(wave / (float)MaxWave);
        var coreRatio = Mathf.Clamp01(lanternHp / Mathf.Max(1f, lanternMaxHp));
        var score = waveRatio * 45f
                    + coreRatio * 18f
                    + evolutionStage * 6f
                    + CountActiveLinks() * 3.5f
                    + CountActiveSynergies() * 2.5f
                    + (fusionActive ? 8f : 0f)
                    + (runBossKilled > 0 ? 8f : 0f)
                    + (cleared ? 18f : 0f)
                    + Mathf.Min(6f, dangerLevel * 1.5f);

        if (score >= 98f) return "S";
        if (score >= 82f) return "A";
        if (score >= 64f) return "B";
        if (score >= 44f) return "C";
        return "D";
    }

    Sprite GetResultRankMedalSprite(string rank)
    {
        if (rank == "S") return resultRankMedalSSprite;
        if (rank == "A") return resultRankMedalASprite;
        if (rank == "B") return resultRankMedalBSprite;
        if (rank == "C") return resultRankMedalCSprite;
        if (rank == "D") return resultRankMedalDSprite;
        return null;
    }

    string BuildResultNextGoal(bool cleared)
    {
        if (cleared)
        {
            if (currentStageId < MaxStageId)
                return "次ステージを試す / " + GetStageDisplayName(Mathf.Min(MaxStageId, currentStageId + 1));
            if (dangerLevel < MaxDangerLevel)
                return "Danger " + Mathf.Min(MaxDangerLevel, dangerLevel + 1) + " で火力と防衛の両立を試す";
            if (!fusionActive)
                return "クロス進化を完成させて別ビルドを記録";
            return "別相棒・別ルートで図鑑と高ランク更新";
        }

        if (lanternHp <= 0f)
            return "Bulwark / GUARD / 回復系でコア防衛を厚くする";
        if (playerHp <= 0f)
            return "移動速度・HP・回収範囲を早めに確保する";
        if (wave >= MaxWave - 1)
            return "最終ボス用に単体火力かクロス進化を伸ばす";
        if (evolutionStage < 2)
            return "Lv6進化まで育成速度を優先する";
        return "リンク2種を揃えてクロス進化条件を狙う";
    }

    // 取得モジュール一覧めE1行�Eコンパクトな斁E���Eに雁E��E    // 同名モジュールは ×N 表記、最大 8件で省略 (画面幁E��允E
    string BuildResultModulesLine()
    {
        if (runModulePicks == null || runModulePicks.Count == 0) return "";
        var entries = new List<string>();
        foreach (var kv in runModulePicks)
        {
            if (string.IsNullOrEmpty(kv.Key)) continue;
            entries.Add(kv.Value > 1 ? kv.Key + "×" + kv.Value : kv.Key);
        }
        if (entries.Count == 0) return "";
        const int maxShow = 8;
        var shown = entries.Count > maxShow ? entries.GetRange(0, maxShow) : entries;
        var extra = entries.Count - shown.Count;
        var joined = string.Join(" / ", shown.ToArray());
        if (extra > 0) joined += " ... +" + extra + "件";
        return "MODULES (" + entries.Count + "件): " + joined;
    }

    void SetResultStat(int index, string label, string value)
    {
        if (index < 0 || index >= resultStatValues.Length)
            return;
        if (resultStatLabels[index] != null)
            resultStatLabels[index].text = label;
        if (resultStatValues[index] != null)
            resultStatValues[index].text = value;
    }

    Color GetRouteAccentColor()
    {
        if (fusionActive)
            return GetFormAccentColor();
        if (formStyle == 1)
            return new Color(0.22f, 0.82f, 1f);
        if (formStyle == 2)
            return new Color(1f, 0.62f, 0.18f);
        if (formStyle == 3)
            return new Color(0.46f, 1f, 0.45f);
        return new Color(0.45f, 1f, 1f);
    }

    sealed class MvpEntry
    {
        public string title;
        public string detail;
        public Color accent;
        public Sprite iconSprite;
    }

    List<MvpEntry> BuildMvpList()
    {
        var list = new List<MvpEntry>();
        var seen = new HashSet<string>();
        var ordered = new List<KeyValuePair<string, int>>();
        foreach (var kv in runModulePicks)
            ordered.Add(kv);
        ordered.Sort((a, b) =>
        {
            var compare = b.Value.CompareTo(a.Value);
            if (compare != 0)
                return compare;
            return runPickedModules.IndexOf(a.Key).CompareTo(runPickedModules.IndexOf(b.Key));
        });

        foreach (var pair in ordered)
        {
            if (list.Count >= 3)
                break;
            if (!seen.Add(pair.Key))
                continue;
            list.Add(BuildMvpEntry(pair.Key, pair.Value));
        }

        if (list.Count == 0)
            list.Add(new MvpEntry { title = "STANDBY", detail = "モジュール未選択", accent = new Color(0.45f, 1f, 1f), iconSprite = diamondSprite });

        return list;
    }

    MvpEntry BuildMvpEntry(string title, int count)
    {
        var entry = new MvpEntry { title = title };
        var accent = GetModuleTitleAccent(title);
        entry.accent = accent;
        entry.iconSprite = GetModuleTitleIcon(title);
        var detail = "x" + count;
        if (title.Contains("クロス進化専用"))
            detail += "    CROSS RARE";
        else if (title.Contains("仲間リンク"))
            detail += "    LINK";
        else if (title.Contains("SPEED専用"))
            detail += "    SPEED EXT";
        else if (title.Contains("GUARD専用"))
            detail += "    GUARD EXT";
        else
            detail += "    MODULE";
        entry.detail = detail;
        return entry;
    }

    Color GetModuleTitleAccent(string title)
    {
        if (title.Contains("クロス進化専用"))
            return new Color(1f, 0.42f, 0.96f);
        if (title.Contains("Nova"))
            return new Color(0.32f, 0.95f, 1f);
        if (title.Contains("Bulwark"))
            return new Color(0.42f, 1f, 0.56f);
        if (title.Contains("Siphon"))
            return new Color(0.95f, 1f, 0.36f);
        if (title.Contains("仲間リンク"))
            return new Color(0.28f, 1f, 0.92f);
        if (title.Contains("パワー") || title.Contains("バースト") || title.Contains("ヘビー") || title.Contains("炎") || title.Contains("ボス") || title.Contains("巨弾") || title.Contains("POWER"))
            return new Color(1f, 0.78f, 0.22f);
        if (title.Contains("ガード") || title.Contains("HP") || title.Contains("コア") || title.Contains("修復") || title.Contains("回復") || title.Contains("リカバリー") || title.Contains("リング") || title.Contains("GUARD"))
            return new Color(0.48f, 1f, 0.34f);
        if (title.Contains("スピード") || title.Contains("クイック") || title.Contains("クロック") || title.Contains("ピアス") || title.Contains("ロング") || title.Contains("SPEED"))
            return new Color(0.24f, 0.82f, 1f);
        return new Color(0.45f, 1f, 1f);
    }

    Sprite GetModuleTitleIcon(string title)
    {
        if (title.Contains("ガード") || title.Contains("GUARD"))
            return squareSprite;
        if (title.Contains("クロス進化専用") || title.Contains("仲間リンク") || title.Contains("POWER") || title.Contains("パワー"))
            return diamondSprite;
        return circleSprite;
    }

    void RetryRun()
    {
        // Quick retry skips partner select and reuses the saved partner style.
        PlayerPrefs.SetInt(QuickRetryKey, 1);
        PlayerPrefs.Save();
        ReloadRuntime(autoStartRun: true);
    }

    void ReturnToMainMenu()
    {
        ReloadRuntime(autoStartRun: false);
    }

    void ReloadRuntime(bool autoStartRun)
    {
        if (runtimeReloading)
            return;

        runtimeReloading = true;
        Time.timeScale = 1f;
        if (resultRetryButton != null)
            resultRetryButton.interactable = false;
        if (resultMenuButton != null)
            resultMenuButton.interactable = false;
        if (restartButton != null)
            restartButton.interactable = false;
        PlayerPrefs.SetInt(AutoStartRunKey, autoStartRun ? 1 : 0);
        PlayerPrefs.Save();

        StartCoroutine(ReloadGeneratedRuntimeRoutine());
    }

    System.Collections.IEnumerator ReloadGeneratedRuntimeRoutine()
    {
        var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        for (var i = 0; i < roots.Length; i++)
        {
            var root = roots[i];
            if (root == null || root == gameObject)
                continue;
            Destroy(root);
        }

        yield return null;

        var next = new GameObject("Core Lantern Game");
        next.AddComponent<CoreLanternGame>();
        Destroy(gameObject);
    }

    void CheckLose()
    {
        if (playerHp > 0f && lanternHp > 0f)
            return;

        gameOver = true;
        SaveProgress(false);
        ShowResult(false);
        PlaySfx("GameOver", 110f, 0.42f, 0.45f);
    }

    Font GetUiFont()
    {
        if (uiFont != null)
            return uiFont;

        var candidates = new[]
        {
            "Yu Gothic UI",
            "Yu Gothic",
            "Meiryo UI",
            "Meiryo",
            "BIZ UDGothic",
            "BIZ UDPGothic",
            "Noto Sans CJK JP",
            "Noto Sans JP",
            "Hiragino Sans",
            "Hiragino Kaku Gothic ProN",
            "MS Gothic",
            "MS UI Gothic",
            "Arial Unicode MS",
            "Arial"
        };
        uiFont = Font.CreateDynamicFontFromOSFont(candidates, 20);
        if (uiFont == null)
            uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return uiFont;
    }

    static Vector2 Rotate(Vector2 vector, float degrees)
    {
        var radians = degrees * Mathf.Deg2Rad;
        var sin = Mathf.Sin(radians);
        var cos = Mathf.Cos(radians);
        return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos).normalized;
    }

    static Quaternion DirectionRotation(Vector2 direction)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
    }

    static Quaternion DirectionRotationRight(Vector2 direction)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    sealed class Enemy
    {
        public Transform transform;
        public Transform visualBody;
        public Transform hpBarRoot;
        public Transform hpBarFill;
        public Transform visualCore;
        public Transform visualArmor;
        public SpriteRenderer renderer;
        public SpriteRenderer hpBarFillRenderer;
        public SpriteRenderer visualCoreRenderer;
        public SpriteRenderer visualArmorRenderer;
        public EnemyType type;
        public float hp;
        public float maxHp;
        public float hpBarWidth;
        public Vector3 baseScale;
        public Vector3 lastVisualPosition;
        public Vector2 visualVelocity;
        public float visualPhase;
        public float hitPulse;
        public float afterimageTimer;
        public float footstepTimer;
        public float attackPulse;
        public float speed;
        public float touchDamage;
        public float shootCooldown;
        public bool isDashing;
        public float dashDuration;
        public bool isPhasing;
        // ── Smooth knockback velocity (m/s). Decays each frame.
        // Avoids the "teleport" feel of instant position += pushDir * X.
        public Vector2 knockbackVelocity;
        // Elite upgrade: low chance from Wave 5+, with stronger stats and visual treatment.
        public bool isElite;                  // Elite 強化中ぁE(撃破時にデータ ×)
        // Boss-specific behavior fields.
        public bool isMidBoss;                 // Explicit flag instead of guessing from HP.
        public float bossSpecialCooldown;    // 次の特殊攻撃破での秒数
        public float bossChargeTimer;         // Pulswyrm 突進中の残り時間
        public Vector2 bossChargeDir;
        public float bossCoreMarkTimer;
        public Vector2 bossCoreMarkTarget;
        // ── Nullwyrm Phase 2 専用 (docs/BOSS_PHASE2_SPEC.md) ──
        public bool phase2Triggered;
        public float phase2TransitionGrace;
        public float spiralCooldown;
        public float spiralTelegraphTimer;
        public float summonCooldown;
        public bool firstSummonDone;          // Whether the first runner-only summon has fired.
    }

    sealed class Bullet
    {
        public Transform transform;
        public Vector2 direction;
        public float damage;
        public float life;
        public int pierce;
        public bool fromEnemy;
        public float speedMultiplier = 1f;
        public float hitRadius = 0.48f;
        public float splashRadius;
        public int style;
        public int chainJumps;
        public float chainDamageMultiplier = 0.4f;
    }

    sealed class Pickup
    {
        public Transform transform;
        public float value;
    }

    sealed class Spark
    {
        public Transform transform;
        public Vector2 velocity;
        public float life;
        public float maxLife;
    }

    sealed class BinaryDigit
    {
        public Transform transform;
        public SpriteRenderer renderer;
        public float fallSpeed;
        public float flickerTimer;
        public float baseAlpha;
    }

    sealed class SpriteGhost
    {
        public Transform transform;
        public SpriteRenderer renderer;
        public Color baseColor;
        public Vector3 baseScale;
        public Vector2 velocity;
        public float life;
        public float maxLife;
    }

    sealed class FloatingText
    {
        public Transform transform;
        public TextMesh mesh;
        public Vector2 velocity;
        public float life;
        public float maxLife;
        public Vector3 baseScale;
    }

    sealed class AmbientNeon
    {
        public SpriteRenderer renderer;
        public Color baseColor;
        public float phase;
    }

    sealed class OptionButton
    {
        public Button button;
        public Text label;
        public string name;
        public Func<bool> getter;
        public Action<bool> setter;
    }

    sealed class RelicData
    {
        public readonly string id, title, description;
        public readonly Action apply;
        public RelicData(string id, string title, string description, Action apply)
        { this.id = id; this.title = title; this.description = description; this.apply = apply; }
    }

    sealed class Upgrade
    {
        public readonly string title;
        public readonly string description;
        public readonly string stars;
        public readonly int requiredFusionStyle;
        public readonly Action apply;

        public Upgrade(string title, string description, Action apply)
            : this(title, description, "★★★", apply)
        {
        }

        public Upgrade(string title, string description, string stars, Action apply)
            : this(title, description, stars, 0, apply)
        {
        }

        public Upgrade(string title, string description, string stars, int requiredFusionStyle, Action apply)
        {
            this.title = title;
            this.description = description;
            this.stars = stars;
            this.requiredFusionStyle = requiredFusionStyle;
            this.apply = apply;
        }
    }

    sealed class PartnerChoice
    {
        public readonly string name;
        public readonly string description;
        public readonly Action apply;

        public PartnerChoice(string name, string description, Action apply)
        {
            this.name = name;
            this.description = description;
            this.apply = apply;
        }
    }

    // Stage hazard zone: shared state for hazards and stage devices.
    // Lava/HeatVent: Stage 1 / CorruptionPatch/RelayDevice: Stage 2
    // FrostCrystal: Stage 3 reward + freeze AoE / FrostPatch: Stage 3 movement slow.
    // LightningMarker: Stage 4 telegraph and strike marker.
    enum StageHazardKind { Lava, HeatVent, CorruptionPatch, RelayDevice, FrostCrystal, FrostPatch, LightningMarker }

    sealed class StageHazardZone
    {
        public Transform transform;
        public SpriteRenderer renderer;
        public Vector2 center;
        public float radius;
        public StageHazardKind kind;
        public bool isActive;           // Whether this hazard currently deals damage.
        public float pulseTimer;         // Vent active remaining time.
        public float telegraphTimer;     // Vent: 次パルスまでの秒数 (負ならテレグラフ中)
        public float visualPhase;        // Visual animation phase.
        public bool relayActivated;
        public float relayChargeTimer;
        public Transform relayChargeRing;
        public float crystalHp;
        public float crystalMaxHp;       // FrostCrystal: 最大 HP
        public float lightningStrikeAt;
        public bool lightningResolved;
    }

    enum EnemyType
    {
        Runner,
        Brute,
        Shooter,
        Boss,
        Dasher,           // Cyan arrowhead  Edashes at 4×speed every 2.5s
        Bomber,           // Orange bomb    Eslow, explodes on death in AoE
        Phantom,          // Violet wisp    Eperiodically phases (immune + fast)
        LavaCrawler,      // Stage2 heat-armored crawler with furnace-core pulses
        MagmaTitan,
        CorruptionDrone,
        FrostKnight,
        VoltDasher        // Stage5 Dasher派甁E E紫黁E��超加速で雷の残像
    }
}
