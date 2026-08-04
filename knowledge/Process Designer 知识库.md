<a id="v1-s1"></a>

# 1. 工艺设计器简介（Process Designer Introduction）

<a id="v1-s2"></a>

## 工艺设计器规划（Process Designer Planning）

工艺设计器（Process Designer）将 eMServer 及其规划工具与三维查看器（3D viewer）集成在一起。工艺设计器由导航树（Navigation Tree）和图形查看器（Graphic Viewer）组成，导航树涵盖全部数据（产品树、操作树、对象树、制造特征树和资源树，即 Product、Operation、Object、Mfg 与 Resource Trees）。您可以在树和查看器中同时查看数据，并在 eMServer 中修改这些数据。这些数据包括资源（Resource）、零件（Part）、操作（Operation）以及制造特征（manufacturing feature，如焊点、焊缝、螺母等）。

工艺设计器为用户提供快速的查看性能和基础分析工具，从而在规划过程中提升工作质量与生产效率。

关于如何访问工艺设计器，请参阅"启动工艺设计器（Launching Process Designer）"。

典型的工艺设计器工作流程如下：

1. 打开项目。
2. 使用过滤器检查状态，并针对 eBOM 与 mBOM 调整数据：
   - 在数据库上执行高级查询与搜索，并在三维查看器中查看结果。
   - 从 eMServer 检入/检出（Check in / Check out）对象并进行修改。
   - 向工作流中添加对象。
   - 分配在查询/搜索结果中或属性（Properties）中发现的未分配零件/焊枪/资源。
   - 对数据应用变体过滤器（variant filter）。
3. 使用焊接分析工具（Weld Analysis）和自动零件分配工具（Automatic Parts Assignment）优化焊接操作。
4. 分析整体工艺：
   - 以不同视图和模式显示对象。
   - 操控显示内容及其中的对象。
   - 对各类资源执行运动学（kinematics）操作。
   - 显示与零件相关的制造特征（Mfgs）的三维表达。
   - 检查、剖切和操控剖面（section）。
   - 测量距离和角度。
   - 向显示内容添加注释。
   - 将三维图像导出为多种图形格式。
   - 自定义用户界面，例如键盘快捷键和上下文菜单。
   - 显示所选对象的相邻零件，以便更清晰地了解附近零件的概况。
   - 标记并保存三维图像。
   - 修改所显示对象的颜色。
   - 将离线工程数据保存到 XML 文件中。

<a id="v1-s3"></a>

## 工艺设计器接口（Process Designer Interface）

<a id="v1-s4"></a>

### 帮助文件与文档（Help File and Documentation）

工艺设计器接口用户指南（Guideline DetailedSimulation_Help_G.pdf 或 DetailedSimulation_Help_E.pdf）可通过选择 **File** 选项卡 → **Help** 获取。

系统将该文档安装在以下位置：

```
~\Tecnomatix\eM-Power\eM-Planner\Applications\DetailedSimulation
```

请使用常规 PDF 阅读器从该文件位置打开并阅读文档。

<a id="v1-s5"></a>

### 已知问题（Known Problems）

如果在使用工艺设计器装配模块（Assembly module）时，装配树是从操作 PERT 图创建的，则工艺设计器接口将无法正常工作。其原因在于：在流（flow）上创建的零件与操作同名，而工艺设计器接口无法处理这种情况。

<a id="v1-s6"></a>

### 总体介绍（General Introduction）

工艺设计器接口允许您在工艺设计器中定义的工艺模型之上使用离散事件仿真（discrete event simulation）。工艺模型将制造系统的静态组成部分描述为零件或资源，将动态组成部分描述为生产特定产品所必需的操作。用户可以通过将资源和零件分配给操作，并定义（部分）操作顺序，来获得对生产过程的精确描述。

基于 Plant Simulation 的离散事件仿真使用户能够评估：

- 生产系统的动态行为
- 生产线可达到的产能或生产率
- 缓冲区与仓储容量对系统性能的影响
- 系统与资源的可用性
- 系统性能中的瓶颈
- 生产资源的利用率
- 人工作业人员到任务的分配
- 产品的平均产能或生产时间

仿真还可以增强已定义的工艺模型，帮助确定控制策略并权衡被评估生产系统的各类备选方案。

仿真研究（Simulation Study）与基于工艺设计器的工作流集成在一起，其结果（如统计属性）可以写回工艺模型。这些内容会自动传递到 Plant Simulation，使工艺定义能够构建可直接执行的仿真模型。由于工艺设计器/Plant Simulation 集成通常并非"开箱即用"，因此在构建可运行的仿真模型时，务必牢记一些经验法则。

为了让 Plant Simulation 广泛的仿真功能易于使用，本应用通过清晰简洁的对话框引导用户。因此，运行和评估工艺模型无需具备专门的 Plant Simulation 专业知识。

Plant Simulation 仿真主要聚焦于生产线。在一条生产线内，工位（station）定义了主要的生产步骤，仿真在该层级评估物料流和资源利用率。在许多情况下，工位内部的操作会根据自动化机器人单元、手工单元或两者的组合而高度细化和专业化。用户可以通过"黑盒（black-box）"机制对仿真隐藏这一细节层级，从而规定仿真中只处理复合操作（compound operation）的总体时间，而不处理其嵌套的子操作。

用户可以对复杂的制造结构进行仿真，其中的装配与拆卸操作将主线和支线连接成操作序列网络。

所生成的仿真模型可作为高层级起点，供仿真专家开展更详细、更深入的仿真研究。更新机制（Update mechanism）允许将修改后的工艺数据从工艺设计器移植到增强后的 Plant Simulation 仿真模型中。借助可配置的对象映射和属性集，甚至可以将客户特定对象集成到 Plant Simulation 仿真研究中。

由于离散事件仿真的固有特性，工艺设计器的工艺模型必须满足一些要求才能具备可仿真性：

- 对于进入和离开的零件，都应使用源（Source）和汇（Sink）对象
- 必须定义与仿真相关的资源之间的零件流（part flow）
- 零件流不得存在循环
- 必须正确定义装配与拆卸操作

更多详细信息，请参阅"附加建模约束（Additional Modeling Constraints）"。

<a id="v1-s7"></a>

### 为 Plant Simulation 准备架构（Preparing a Schema for Plant Simulation）

Plant Simulation 仿真研究需要符合 Plant Simulation 特定取值的属性和设置。详细仿真定制（Detailed Simulation Customization）会将这些属性添加到某个架构（schema）内的对象模型中。

**操作步骤**

1. 选择 **Preparation** 选项卡 → **Import** 组 → **Import eBOP Customization**。
2. 在工艺设计器客户端安装目录中，选择 `DetailedCustomization` 文件夹。

该定制为产品、操作和资源添加了属性，这些属性显示在附加选项卡上，而这些选项卡在安装仿真接口后才可见。有关资源、操作和零件对象的仿真选项卡内容的详细说明，请参阅"操作的仿真选项卡（Simulation Tab for Operations）"。

<a id="v1-s8"></a>

### 演示模型（The Demonstration Model）

<a id="v1-s9"></a>

#### 模型中操作的详细解析（Detailed Look at Operation in Model）

本演示模型并非基于真实的规划流程。该模型的基本作用是演示若干用例。下图展示了整个规划模型的概览。后续各节将更详细地介绍复合操作中的各个子操作。

几乎所有操作都构建为孪生对象（twin objects），即为每个操作创建一个资源并分配给该操作。

在本演示模型中，资源具有与操作相同的层次结构。这样做是为了便于识别操作及其对应的资源。请注意，资源实际上可以采用任意结构。

图中更详细地展示了操作 OP100。四个操作用于加工三种不同的零件。所有零件均由一个源（source）创建。在对应的仿真模型中，三种零件均以 10 件为一批创建。因此先创建十件 Component_B 产品，然后创建十件 Component_C，随后创建十件 Component_D，之后该序列重新开始。

操作 OP10 未分配资源。在这种情况下，Plant Simulation 会创建一个虚拟资源（通常是一个对产品进行操作的工位）。

操作 OP20 分配了一个人力资源（human resource）。我们假定这是对搬运操作进行建模的方式。人力资源（工人）用于将一件产品从一个位置搬运到另一个位置。在 Plant Simulation 中，人力资源无法搬运产品，因此我们同时创建一个虚拟资源来搬运产品。在操作开始之前，工人必须先到达该工位。请注意，产品的实际搬运时长可能长于您在操作时间中定义的时间，原因可能是工人在执行搬运操作之前需要先完成另一项工作。

操作 OP30 分配了一个资源。在仿真模型中，将根据所分配资源的资源定义创建相应的 Plant Simulation 对象。产品将在该资源上按分配给操作的加工时间进行加工（使用分配时间，allocated time）。

在最后一个操作中，同时向操作分配了一个"普通"资源和一个人力资源。一旦产品到达该资源，就会请求人力资源执行该操作。产品在资源上等待，直到工人到达资源并开始作业。如果人力资源必须先完成另一项工作，则会延长加工时间。

从操作 OP40 输出的产品沿着流（flow）继续被加工。为此，我们需要知道产品被移动到何处。接口（interface）使我们能够沿流向上追踪到更高层级。在那里，我们还可以沿流移动到下一个操作 "Comp_processing"。

<a id="v1-s10"></a>

#### 操作 "Comp_processing"（The Operation "Comp_processing"）

在这个复合操作中，所有零件首先由操作 OP701 加工。下一个操作被构建了三次，因为每种产品的加工时间不同。由于工艺设计器只允许为一个操作设置一个加工时间，因此我们必须为每种产品创建一个操作。OP702、OP703 和 OP704 这三个操作分配了同一个资源。

在生成的 Plant Simulation 模型中，只会构建一个资源，但该资源针对不同产品具有不同的加工时间。

操作 OP705 对所有产品同样使用相同的加工时间。

操作 OP706 加工三种产品中的两种，OP707 加工其中一种。这两个操作分配了不同的资源。在对应的 Plant Simulation 模型中将构建两个资源，在完成操作 OP705 后，产品将根据其类型被移动到下一个工位。

在上一层级中，产品以不同方式被加工，因此我们需要两个接口来将产品路由到后续操作。

<a id="v1-s11"></a>

#### 分流/合流操作（The Operation split / merge）

我们的两种产品沿流进入复合操作 OP200。在该操作内部，零件由多个操作加工。其中三个操作 "Transport1"、"Transport2" 和 "Transport3" 分配了资源，且其 Plant Simulation 类型设置为 "conveyor"（输送机）。该设置将在模型生成过程中创建一台输送机，用于将产品从一个工位传送到另一个工位。这种建模方式可以对两个加工操作之间的物流操作进行建模。同理，也可以对缓冲区和其他物流对象进行建模。

在操作 "Transfer2" 之后，操作 "OP230"、"OP240" 与 "OP250"、"OP260" 被并行构建，并分配了不同的资源。此处的问题在于：相对于操作序列中其余操作较短的加工时间，这些操作的加工时间非常长。因此我们需要两个并行的工位来达到期望的产能。操作 "Transfer2" 的类型为 "split"（分流），该类型会被自动识别，无需手动定义此操作类型。分流操作会将产品移动到下一个空闲的后续工位。

<a id="v1-s12"></a>

#### OP300 产品分流（OP300 Diverging the Products）

产品 "Component_B" 和 "Component_C" 由不同的操作和不同的资源加工。如下图所示，您只需定义不同的操作，Plant Simulation 侧的物料路由将自动完成。这些操作未分配资源，因此在生成的 Plant Simulation 模型中，将使用默认加工工位来对这些资源建模。资源的名称将与操作相同，因此便于识别这些资源。

<a id="v1-s13"></a>

#### 操作黑盒（Operation Black Box）

深入查看复合操作 "Blackbox" 下的各个操作，可以看到如下图所示的三个操作。这里定义了三个复合操作，它们被定义为"黑盒（Blackbox）"操作，这意味着这些复合操作之下的所有内容与仿真模型无关。要定义黑盒操作，请勾选复选框 **omit suboperation**（忽略子操作）。我们并不关心这三个操作的内部细节。如果未定义资源，则会在 Plant Simulation 模型中构建 "Blackbox" 资源。

可以为分配给黑盒操作的资源指定 Plant Simulation 类型。在这种情况下，将使用所定义的资源类型而非黑盒资源来构建仿真模型。在下图所示的 PERT 图中，操作 "BB_1" 分配了一个资源。该资源未定义特定的 Plant Simulation 类型，只将资源容量（capacity）设置为 5。其结果是在 Plant Simulation 模型中生成一个容量为 5 的黑盒资源。无法假定进入黑盒资源的产品与离开黑盒资源的产品之间存在任何对应关系。

操作 "BB_2" 也分配了一个资源。这次我们将资源的 Plant Simulation 类型定义为 "assembly"（装配）。在 Plant Simulation 模型中将构建一个装配对象。

第三个操作 "BB_3" 同样分配了资源。这次资源的 Plant Simulation 类型设置为 "station"（工位），在 Plant Simulation 模型中的结果将是默认的工位对象。

<a id="v1-s14"></a>

#### 复杂操作（The Complex Operation）

有时，在特定资源上执行的操作会被更详细地建模。例如：该操作被划分为三个子操作，第一个子操作需要额外的工人为工位上料，然后工位自动加工零件，第三步再由工人为工位下料。在工位自动加工期间，工人可以执行其他工作。这是将复合操作定义为复杂操作（complex operation）的典型场景。在 Plant Simulation 侧，PERT 图用于构建一个框架（frame）。该框架具有与复杂操作的子操作相同的行为。可以为复杂操作分配一个资源，并为该资源定义容量，该容量决定并行加工的产品数量。此容量在 Plant Simulation 侧同样会被考虑。

> **注意**
>
> 在复杂操作中，同样必须为流分配产品，这是识别操作类型所必需的。

<a id="v1-s15"></a>

#### Plant Simulation 专用设置与工艺模型前提条件（Plant Simulation Specific Settings and Process Model Prerequisites）

Plant Simulation 分析工艺模型的物料流以及相应的资源利用率。为了在 Plant Simulation 中实现自动模型生成，用户必须在工艺设计器的工艺模型中遵循一些规则。操作 PERT 图（Operation PERT）能够最好地展现工艺模型上的仿真约束以及对象之间已定义的关系。所有规则都可以通过该视图检查，同时需牢记以下必要步骤：

- 使用工艺设计器的源（source）对象在操作序列的起点创建零件
- 使用工艺设计器的汇（sink）对象在操作序列的终点消耗零件/产品
- 为连接仿真相关节点的流分配零件
- 为仿真相关的资源和操作确定 Plant Simulation 类型
- 当多个零件进入或离开某个复合操作时，使用接口（interface）对象

下面的 PERT 图表示了用户可在 Plant Simulation 中仿真的典型工艺模型片段。

**工艺设计器中的源（Sources in Process Designer）**

必须使用工艺设计器模型中的源，因为源对象的输出流与该源创建的产品相关联。

当 Plant Simulation 检测到源操作时，会创建一个源对象。分配到源输出流上的所有产品都会被收集，产品树中该产品的属性 `mixpercentage` 决定所创建零件的百分比。如果源的输出流上只有一种产品，则 `mixpercentage` 不起作用，被设为 100%。如果未将 `mixpercentage` 定义为产品属性，则 Plant Simulation 会按 100% 除以待生产的不同零件数量来计算该百分比。

如果一个源产出多种产品，请注意每种产品均以 5 件为一批创建（默认值），您也可以在产品树中该产品的属性 `batchsize` 中更改此数值。

**操作的资源分配（Resource Assignment to Operations）**

每个操作至少应分配一个与仿真相关的资源。您还可以添加一名工人（工艺设计器原型 human）。如果向操作分配了多个此类资源，则应将其中一个资源标记为主资源（primary resource），请使用资源仿真选项卡上对应的复选框。

主资源即加工产品的资源。如果未定义主资源或定义了多个主资源，则将使用找到的第一个资源作为主资源。

**孪生对象的使用（Use of Twin Objects）**

当复合操作与相应的复合资源之间存在一一映射关系时，通常使用孪生对象（twin objects）。在这种情况下，复合操作的资源分配是隐式的。

**操作未分配资源（No Resource Assignment to Operations）**

如果某个操作没有分配资源，Plant Simulation 会生成一个虚拟资源来执行该操作。

**仅分配人力资源（Only Human Resource Assignment）**

如果某个操作仅分配了人力资源，则会创建一个虚拟资源。

如果您希望在规划模型和仿真模型中都使用人力资源，请注意以下几点：

人力资源的数量由资源树中 human 类型的数量决定。您可以使用拖放功能将一个人力资源分配给一个或多个操作，因为一个人力资源通常执行多个操作。

只定义一个人力资源并把该资源分配给所有操作是没有意义的。

<a id="v1-s16"></a>

#### 流的零件分配（Part Assignment to Flows）

所有与仿真相关的操作流都必须至少分配一个零件。这使得 Plant Simulation 的模型生成模块能够跟踪零件流经各个加工它的操作。可以为一个流分配多个零件。Plant Simulation 对流上零件分配的解释方式取决于操作类型。在默认配置下，Plant Simulation 区分以下操作类型：

- **Normal operation（普通操作）** —— 一个零件进入、被加工、然后离开
- **Assembly operation（装配操作）** —— 多个零件进入并被装配，一个零件离开
- **Dismantle (disassembly) operation（拆卸操作）** —— 一个零件进入，多个零件离开

操作类型在操作对象的 Simulation 选项卡上定义。请使用下拉列表选择某个预定义的操作类型。

**普通操作中的零件流（Part Flow in Normal Operations）**

"普通"操作指对零件进行车削、铣削等加工的操作。每个零件流经该操作，且零件类型不发生变化。进入和离开的零件数量必须相同。即使输入流中包含多个零件，一次也只加工一个零件。

**装配操作中的零件流（Part Flow in Assembly Operations）**

装配操作至少需要 2 个输入零件，且每个输入零件必须位于单独的流上。只有一个零件（即装配后的零件）离开。一旦所有必需的输入零件均可用，装配操作即开始。

> **注意**
>
> 系统不允许在一个操作中合并两个（子）产品的装配。请为每个产品创建单独的装配操作。

**拆卸操作中的零件流（Part Flow in Dismantle Operations）**

拆卸操作需要一个输入零件和至少两个输出零件。每个离开的零件必须分配到单独的流上。

> **注意**
>
> 系统不允许在一个操作中合并两个（子）产品的拆卸。请为每个产品创建单独的拆卸操作。

<a id="v1-s17"></a>

#### 复合操作中的零件流与接口对象（Part Flow and Interface Objects in Compound Operations）

通常，您会创建复合操作的层次结构，并使用流连接所有层级上的全部复合操作。您还必须为复合操作之间的流分配零件。这样做是必要的，以便在工艺模型导出到 Plant Simulation 后，能够确定零件在后续复合操作中的下一个操作。

必须使用接口对象来确定零件正在进入或离开某个复合操作。

<a id="v1-s18"></a>

#### 资源层次结构（Resource Hierarchy）

包含仿真相关资源的复合资源会自动映射到 Plant Simulation 的框架（frame）对象。Plant Simulation 框架可以嵌套，例如将一条生产线的所有机器汇集到一个组中。

您可以将资源层次结构用于不同目的：

- 对工厂或生产线中的机器进行分组
- 按保护回路（protective circuits）对机器进行分组

无需为嵌套的复合资源定义仿真相关设置。一旦存在子级（即已分配给某个仿真相关操作的子资源），这些设置将被忽略。

无需为操作和资源建立相同的层次结构。

> **注意**
>
> 使用工艺对象（process objects）建模时，与仿真相关的资源不得位于孪生资源（twin resource）之外。

**资源的属性（Attributes of Resources）**

在每个资源的 Simulation 选项卡上，您可以使用 **Plant Simulation Type** 字段来定义用于构建仿真模型的相关 Plant Simulation 资源。

如果未定义类型，e-Plant 将使用对象 `station` 创建该资源。如果相应的操作类型为 assembly 或 disassembly，则使用 Plant Simulation 的装配和拆卸对象来创建仿真模型。

所有变更都会记录在日志文件中。

要了解在模型生成过程中如何检测资源，请参阅"映射到 Plant Simulation 目标对象（Mapping to Plant Simulation Target Objects）"。

虽然您可以为一个操作分配多个资源，但与该操作相连的资源中只能有一个勾选 **Primary resource** 选项。该资源将用于加工产品。如果未指定主资源，系统会将第一个非人力资源的仿真相关资源用作主资源。

在本版本中，次级资源（与仿真相关但非主资源的资源）与仿真模型无关。在后续版本中，它们将被视为必要的服务资源。

<a id="v1-s19"></a>

#### 将子操作标记为与仿真无关（Marking Sub-Operations as Non Simulation Relevant）

若要阻止某个复合操作被细化并针对其嵌套子操作进行详细评估，请在操作的 Simulation 选项卡上勾选 **Omit Sub operations** 选项。

您可以设置应用程序忽略嵌套操作的 Plant Simulation 专用建模限制（甚至包括操作 PERT）。这使得应用程序能够处理自由定义的操作图，例如具有任意零件和资源分配的操作图。

<a id="v1-s20"></a>

### 创建 Plant Simulation 研究对象（Creating Plant Simulation Study Objects）

**操作步骤**

1. 从项目树（Project tree）上下文菜单中选择 **New**，创建一个 StudyFolder。
2. 从 StudyFolder 的上下文菜单中选择 **SimpleDetailedStudy**。
3. 打开 SimpleDetailedStudy 对象，它会收集与仿真研究相关的信息。

<a id="v1-s21"></a>

### 定义仿真研究的范围（Defining the Scope of a Simulation Study）

SimpleDetailedStudy 对象的主要组成部分显示在最左侧的树视图以及 **Order** 和 **Import** 选项卡中。

使用该树视图定义仿真研究的范围，并通过拖放条目为以下内容定义快捷方式：

- **操作树（Operation tree）** —— 包含与仿真相关的操作集合
- **资源树（Resource tree）** —— 包含分配给仿真相关操作的资源
- **零件树（Part tree）** —— 包含仿真相关操作所使用/所需的零件（可选）

> **注意**
>
> 资源对象和操作对象各自只允许使用一棵树，因为正确分析工艺模型需要父/子关系。请始终导出完整的资源树和操作树（不得只使用树的一部分）。

- 所有资源对象必须是同一棵资源树中的节点
- 所有操作对象必须是同一棵操作树中的节点

使用不同的操作树时，您可以定义不同的仿真研究，例如若干棵各含多种备选方案的操作树，或在项目不同阶段创建的不同操作树。

<a id="v1-s22"></a>

### 准备 Plant Simulation 仿真研究（Preparing a Plant Simulation Simulation Study）

在 Plant Simulation 中开始仿真研究之前，您必须在工艺设计器中准备一些数据。第一步是创建一个研究文件夹（study folder）和一个 SimpleDetailedStudy 对象，以便为后续在 Plant Simulation 中的仿真收集工艺数据。定义仿真范围之后，您还必须设置仿真专用属性，并验证工艺模型定义符合"Plant Simulation 专用设置与工艺模型前提条件"中描述的规则。

<a id="v1-s23"></a>

### 执行仿真研究（Performing a Simulation Study）

正确定义工艺模型后，您可以使用 DetailedStudy 对象的 **Order** 选项卡将工艺模型导出到 Plant Simulation。

您可以使用 **State** 和 **Date** 下拉列表添加管理信息，并在 **Simulation Purpose** 文本框中添加注释。

单击 **Export** 可将分配给 DetailedStudy 对象的对象导出为 `.ppd` 或 `.xml` 文件。系统会在 File Select 对话框中要求您指定导出文件的位置。如果您是管理员用户或拥有相应权限，可以选择 **Include customization** 选项，以便在导出研究中的对象之外同时导出定制数据。系统会为数据文件名添加 `_customization` 后缀。

> **注意**
>
> 您必须为该研究至少分配一棵操作树和一棵资源树，且不得只导出树的一部分。

排除定制数据可缩短导出所需时间。

如果勾选 **with automatic detailed simulation** 选项，Plant Simulation 将随导出的工艺模型自动启动。Plant Simulation 读取工艺模型信息并构建仿真模型。

若未勾选 **with automatic detailed simulation** 复选框，则只会将导出文件写入指定的文件位置。Plant Simulation 应用程序可以在离线模式下加载并处理该导出文件，无需运行工艺设计器。

在自动模式下，仿真在模型生成阶段之后直接启动。从 eMS Interface 图标的上下文菜单中选择 **Open**，可打开 Process Designer Plant Simulation Integration 对话框。您可以使用该对话框控制仿真、显示仿真图表并打开仿真模型 —— 参见"使用 Plant Simulation 仿真模型（Working with the Plant Simulation Simulation Model）"。

单击 DetailedStudy 的 Order 选项卡底部的 **Preferences**，可打开 Preferences 对话框。详情参见"仿真选项卡参考（Simulation Tabs Reference）"。

> **注意**
>
> 本版本不支持 DetailedStudy 对象上的 **Levels** 下拉列表，该属性的设置尚未启用。

<a id="v1-s24"></a>

### 使用 Plant Simulation 仿真模型（Working with the Plant Simulation Simulation Model）

<a id="v1-s25"></a>

#### 创建仿真模型（Creating a Simulation Model）

使用非自动的离线模式时，您必须启动 Plant Simulation 并加载模型生成器 `DetailedSimulation.spp`。随后会打开一个对话框，您可在其中控制模型生成以及仿真本身。

选择对话框的 **Generate** 选项卡，按 **Load** 按钮加载先前生成的工艺设计器导出文件。此时会打开一个文件选择框，您可以在其中选择 `.ppd` 或 `.xml` 文件。

所选并已加载文件的路径和名称将显示在 **Loaded data file** 下方的行中。

单击 **Generate simulation model** 开始生成仿真模型。数秒后会显示消息 "Model generation finished"，单击 **OK**。此时 Plant Simulation 的类库中会显示 AMG 文件夹，其中包含一个名为 AMGxx 的新框架（其中 xx 为用于区分各次模型生成所得仿真模型的编号）。

<a id="v1-s26"></a>

#### Plant Simulation 仿真的自动模式（Automatic Mode of Plant Simulation Simulation）

当您从工艺设计器自动启动 Plant Simulation 时，Plant Simulation 仿真会在模型生成之后直接开始。Plant Simulation 按照预定义的时长创建仿真，并生成用于将仿真结果导入工艺设计器的 `.ppd` 文件。该文件按所选导出文件的名称命名（`Export_file_name.ppd` / `Export_file_nameINP.ppd`）。

此步骤结束后，Plant Simulation 会自动关闭，结果文件将被导入到工艺设计器。

<a id="v1-s27"></a>

#### 控制仿真运行（Controlling the simulation run）

选择 **Control** 选项卡以控制仿真运行。**Current selected model** 一行中显示您当前正在控制的模型名称。在下方的下拉菜单中，您可以选择任何其他现有仿真模型，以便通过该选项卡进行控制。

单击 **Reset and Start** 可复位并启动仿真。您可以观察仿真过程 —— 在 Plant Simulation 对话框中单击 **Open current simulation model** 打开仿真模型的框架。

双击事件控制器（event controller）并打开 **Settings** 选项卡，其中包含开始时间和仿真运行时长设置。**Statistics** 设置定义了开始采集统计值的时刻，使您可以在预热期（warm-up period）之后再开始模型评估。

**Controls** 选项卡提供了用于启动和运行仿真以及修改仿真速度的附加设置。

有关资源和操作的统计仿真结果信息，请参阅"仿真选项卡参考（Simulation Tabs Reference）"。

您可以通过单击 **Show resource utilization** 查看资源利用率、单击 **Show worker utilization** 查看工人利用率、单击 **Show part statistics** 查看零件统计信息。这些结果可以在仿真运行期间实时观察。

相关图表包括：资源统计（Resource statistics）、缓冲区直方图（Buffer Histogram）和工人统计（Worker statistics）。

<a id="v1-s28"></a>

#### 仿真结果（Simulation results）

仿真时长的默认值设为 5 天。仿真完成该时长后，将显示仿真结果报告。该 HTML 报告会被存储，并可附加到工艺设计器项目中。资源利用率、工人利用率等仿真结果同样会被存储，并可自动或手动导入。导入机制可以自动或手动启动（参见"调整详细仿真模块（Adapting the Detailed Simulation Module）"）。

如果在模型生成过程中出现任何问题，请查看日志文件（Log file）以获取更多信息。

仿真运行的默认时长为 5 天。仿真运行结束后会打开一个文件选择框，您可在其中选择用于写入仿真结果的文件。该文件可导入到工艺设计器。

您可以在 Plant Simulation 事件控制器中定义仿真运行的时长。要修改此设置，请通过 Process Designer Plant Simulation Integration 对话框打开仿真模型，事件控制器位于左上角。

<a id="v1-s29"></a>

#### 将仿真结果导入工艺设计器（Importing Simulation Results to Process Designer）

启动工艺设计器并打开相应项目。使用 DetailedStudy 对象并选择 **Import** 选项卡，即可导入仿真运行的结果。

详细仿真运行的所有结果显示在资源、操作和零件对象的仿真选项卡右侧。有关结果值的详细说明，请参阅"仿真选项卡参考（Simulation Tabs Reference）"。

<a id="v1-s30"></a>

#### 日志文件（The Log file）

如果在模型生成过程中出现问题，您可以查阅日志文件，以了解系统未能生成仿真模型的原因。单击 AMG 对话框 **Tools** 菜单中的 **Log file**，可打开包含以下若干页面的 HTML 报告：

- **Info**：包含关于生成过程的一般信息。
- **Error**：报告严重错误消息 —— 通常是导致模型生成失败的致命错误。如果未发生错误，日志文件中不会生成此页面。
- **Products**：包含关于产品及其结构的信息。
- **Operations**：包含关于操作的信息。在模型生成阶段，AMG 进程会分析各操作并尝试识别操作类型（operation、assembly、disassembly）。如果用户未定义任何操作类型，AMG 会设置类型并报告该定义。

<a id="v1-s31"></a>

### 更新仿真模型（Updating the Simulation Model）

<a id="v1-s32"></a>

#### 属性更新（Attribute Update）

通过此功能，您可以更新仿真模型的属性。当规划人员更改了工艺设计器中的数据，而您希望用新值更新仿真模型时，就应执行此操作。此功能不会更改仿真模型的任何结构。

选择对话框的 **Update** 选项卡以更新仿真模型的属性。

按 **Load export file for update** 加载包含新属性值的 ppd 或 xml 文件。使用下拉菜单选择仿真模型，这将激活属性的 **Update** 按钮。在工艺设计器中定义的所有属性都将被更新。如果某属性在 Plant Simulation 对象中不存在，则会创建该属性。更新后的模型可以直接打开。

在属性更新过程中，会使用工艺设计器与 Plant Simulation 属性的映射表，将工艺设计器属性的值赋给 Plant Simulation 属性。

<a id="v1-s33"></a>

#### 更新模型的结构（Updating the Structure of the Model）

仿真模型可以随时修改，这在需要添加控制策略和/或物流资源时是必要的。

这对属性更新而言不成问题，因为只有由 AMG 创建的那些对象的属性才会被更新。

更新仿真模型的结构时，所有手动完成的修改都将被保留，只有结构性修改会被更新。

要更新仿真模型的结构，请选择 **Update attribute** 选项卡。按 **Load export file for update** 加载先前导出的数据。数据加载完毕且内部表创建完成后，**Update structure** 按钮将被激活。按此按钮即可使用刚加载的数据更新仿真模型。结构更新机制不会影响您在仿真模型中所做的修改。

<a id="v1-s34"></a>

### 仿真模型的修改（Modifications of the simulation model）

您可以通过选择 **Model changes** 选项卡并按 **Show report of modifications** 按钮来创建修改报告。

<a id="v1-s35"></a>

### 属性映射（Attribute Mapping）

有时我们既有一个工艺设计器模型，同时又有一个并非通过工艺设计器接口自动创建的 Plant Simulation 模型。如果需要用工艺设计器模型的数据更新该 Plant Simulation 模型，就必须定义一套映射机制，将工艺设计器资源匹配到 Plant Simulation 对象，并将资源的属性匹配到任意 Plant Simulation 属性或变量。属性映射（Attribute Mapping）机制正是为此设计的。

要使用属性映射，必须将当前版本的 AMG 加载到仿真模型的类库中。为工艺设计器模型创建一个 SimpleDetailedStudy 并生成导出文件。

选择 AMG 对话框的 **Attribute Mapping** 选项卡。

在输入框中输入 Plant Simulation 模型的路径，或按输入框旁边的按钮选择一个 Plant Simulation 模型。第二步，按 **Load Process Designer export file** 按钮加载 ppd 文件。文件加载完毕且内部表构建完成后，会有消息框提示您。按 **Start attribute mapping** 启动属性映射对话框。

属性映射首先会检测 Plant Simulation 模型中是否已存储映射信息。您可以决定是复用这些数据，还是创建新的映射表。

首次启动属性映射时，映射表为空。工艺设计器导出文件中的所有类定义与 Plant Simulation 类库中的所有类定义都会被收集并显示在表中。

主对话框左侧显示工艺设计器模型的所有类定义，右侧显示 Plant Simulation 类库的所有类定义。如果之后添加了某些类，您可以使用 **Add classes** 按钮追加这些新类。

属性映射包含两个阶段。第一阶段连接类，即定义一个工艺设计器类与一个或多个 Plant Simulation 类之间的属性映射。先双击工艺设计器类，再双击 Plant Simulation 类，即可将两者连接起来。

工艺设计器类与 Plant Simulation 类之间不必是一对一关系，每个类都可以与对侧任意数量的类相关联。

两个类之间的关系显示在两张表的第二列中，其中显示关联类的行号。例如，工艺设计器类 "Robot"（第 2 行）与 Plant Simulation 类 "Station"（第 2 行）相关联。Plant Simulation 表中行号后的字符 "i" 表示继承（inherited）。

这表明该属性映射继承自默认设置。默认设置可通过属性映射对话框上的 **Default Mapping** 按钮更改，这将打开一张映射表，您可以在左侧输入任意工艺设计器属性，在右侧输入任意 Plant Simulation 属性。

<a id="v1-s36"></a>

#### 移除连接（Removing a connection）

如果需要移除两个类之间的关系，请选中该类并按 **Delete** 按钮。这会删除所选类与其关联类之间的关系。如果与所选类关联的类不止一个，将弹出对话框询问应移除哪一个关系。

<a id="v1-s37"></a>

#### 移除 Plant Simulation 类（Removing Plant Simulation classes）

如果 Plant Simulation 类列表很长，可以移除其中一些类。选中要移除的类并按 **Delete class** 按钮。

<a id="v1-s38"></a>

#### 调整默认属性映射（Adapting the default attribute mapping）

按 **Default Mapping** 按钮可修改资源类的默认映射表。

将打开一张表，您可以在其中定义 eM Plant 属性的名称。使用 **OK** 按钮关闭该表后，此表将应用于所有带有继承标记（行号后带 "i"）的已定义关系。

<a id="v1-s39"></a>

#### 调整两个类之间的属性映射（Adapting the attribute mapping between two classes）

如果两个类已建立连接，则可以修改该关系的属性映射。在类表中双击其中一个类将其选中，然后按 **Mapping of attributes** 按钮。同样会打开一张表，您可以在其中定义该关系的属性映射。关闭映射表时，如果其中一个类与对侧的多个类相连，将弹出对话框询问应修改哪些连接。

在上图所示的表中，工艺设计器的一些属性已定义，eM Plant 属性 "Availability"、"MTTR" 和 "MTBF" 也已完成映射。eM Plant 属性名称中可以包含点号，从而定义指向子框架（sub frame）属性的路径。如果 eM Plant 侧的字段为空，则不进行映射，之后也不会写入任何值。

请注意，如果您移除两个类之间的连接，针对该关系的属性映射也会被一并移除。

此映射仅影响所定义的该关系，这一点通过行号后缺少字符 "i" 得以体现。

<a id="v1-s40"></a>

#### 实例之间的关系（Relation between instances）

按 **Go to mapping of instances** 按钮可进入相应对话框。这是属性映射的第二阶段，在此阶段连接各个实例。您所连接的每个实例都会自动使用类的映射关系。



打开的对话框中，左上和右上窗口显示类，左下和右下窗口显示相应的实例。一旦在左上网格中选中某个工艺设计器类对象，该类的实例就会显示在左下网格中。同时，相关联的 eM Plant 类对象会显示在右上网格中。如果有多个 eM Plant 对象与该工艺设计器对象相关联，则所有相关的 eM Plant 类都会被显示。

在右上网格中选中一个 eM Plant 类对象，该类对象的所有实例将显示在右下网格中。

此时可以通过双击左下和右下网格中的对象来连接实例。

两个实例之间的连接以坐标形式显示，例如 (2,3) 表示该对象是第 2 行类的实例，并与第 3 行的实例相连。坐标后的字符 "i" 表示属性映射继承自相应的类。

<a id="v1-s41"></a>

#### 显示全部或选定的 Plant Simulation 类（Displaying all or selected Plant Simulation classes）

如果未勾选 **Show all classes** 复选框，则只显示与对应工艺设计器类对象相关联的 eM Plant 类对象。如果勾选该复选框，网格中将显示所有可用的 eM Plant 类对象。

<a id="v1-s42"></a>

#### 调整关联实例之间的属性映射（Adapting the attribute mapping between related instances）

关联两个实例之后，属性映射表默认为相应类的映射表。如果您希望对实例使用专门的映射表，必须先双击工艺设计器实例，再双击 eM Plant 实例以选中要更改的关系，然后按 **Mapping of attributes** 按钮。这将打开一张映射表，您可以在其中定义工艺设计器属性及其对应的 eM Plant 属性。

编辑某对实例的属性映射表会清除实例标志。可以使用网格右下角的 **on/off** 按钮重新开启继承。开启继承时，该实例的映射表将丢失，并被类的映射表所取代。

<a id="v1-s43"></a>

#### 向 Plant Simulation 模型写入属性值（Writing attribute values to the Plant Simulation model）

在工艺设计器中，加工时间（分配时间，allocated time）不是资源的属性，而是在该资源上执行的操作的属性。因此，也可以使用属性映射将与某资源相关操作的分配时间写入 eM Plant 对象。勾选复选框 **Transfer allocated time of operations** 后，此功能即被激活。

<a id="v1-s44"></a>

#### 导出与导入映射信息（Exporting and importing mapping information）

属性映射会将全部映射信息写入仿真模型。您可以在其中找到一个保存了所有必要表格的框架，并可将该框架另存为对象。

映射信息也可以通过按 **Export mapping** 按钮进行存储，届时会显示文件菜单供您输入文件名。

已存储的映射信息可随时通过按 **Import mapping** 按钮读入。导入过程中会执行若干合理性检查，如果检测到错误条目或缺失条目，会生成报告汇总所有这些信息，并弹出对话框询问是否需要调整映射表。

> **注意**
>
> 如果出现错误，请检查映射表是否存在不一致之处。

<a id="v1-s45"></a>

### 调整详细仿真模块（Adapting the Detailed Simulation Module）

<a id="v1-s46"></a>

#### 工艺设计器接口 Tools 菜单（The Process Designer Interface Tools Menu）

使用下拉菜单 **Tools** 可打开下图所示的菜单。该菜单中的各项可用于根据您的业务需求配置详细仿真。

**参考表（The reference table）**

使用此表添加您自己的资源。在 `plantType` 列中为您的 eM Plant 对象添加自选的标识符，并在配置文件中添加相同的标识符（参见"配置文件（Configuration File）"）。在 `path` 列中添加所要添加对象的路径。

有时需要在创建对象后执行某个方法。该方法必须位于 AMG 的 **Constructors** 框架中。请在 `method` 列中输入方法名称，该方法将在所有对象创建完成后执行。**Constructors** 框架中已有一些方法可供参考。

如果无需进行属性映射（eM Plant 属性名与工艺设计器属性名相同），则无需在 `Attributes` 列中输入值。在该列中输入 `default` 可使用预定义的默认属性映射；输入其他任意值则可定义您自己的属性映射。

**映射表（The mapping table）**

此表仅供内部使用，用户不应对其进行修改。

**日志文件（The log file）**

日志文件列出模型生成过程中执行的所有活动，您可以从中了解出现的问题以及仿真模型无法创建的原因。

**配置（Configuration）**

选择 **Configuration** 菜单项会打开一个独立对话框，可在其中完成多项设置。这些设置将被保存，下次启动应用程序时仍然可用。这些设置也会与创建的仿真模型一并存储，以便使用具有不同设置的仿真模型。

**Setting 选项卡**

- **Generate model name**（生成模型名称）
    
  勾选此复选框可由 AMG 自动生成所创建仿真模型的名称。否则，生成过程中会弹出对话框要求输入仿真模型名称。若勾选此复选框，所创建仿真模型的名称将为 AMGxx，其中 xx 为所创建模型的连续编号。
- **Create import file for Process Designer**（创建工艺设计器导入文件）
    
  若勾选此复选框，仿真运行结束后将创建用于导入工艺设计器的结果文件。
- **Stop eM Plant in automatic mode**（自动模式下停止 eM Plant）
    
  若勾选此复选框，在自动模式下将创建仿真模型、执行一次仿真实验，随后停止 eM Plant。否则只执行一次仿真实验，eM Plant 会一直等待直到您手动停止它。这样可以保存所创建的仿真模型或进行多次不同的仿真实验。
- **Launch importing results**（启动结果导入）
    
  勾选此复选框后，仿真实验结束时将启动一个接口程序，把结果文件附加到工艺设计器的工艺模型上。
- **Attach result report to**（结果报告附加位置）
    
  选择结果文件的附加位置：附加到项目，或附加到研究对象。

**Directories 选项卡**

在此选项卡上可以定义若干目录路径。

- **System root directory**（系统根目录）
    
  在下方输入框中输入您的系统根目录。如果您希望创建三维仿真模型，eM Plant 会自动加载几何文件（.co 文件）。
- **Path of 3D geometries**（三维几何文件路径）
    
  如果您的计算机上没有安装工艺设计器，可以指定其他任意目录用于搜索三维几何文件。
- **Object library**（对象库）
    
  输入相应目录的路径，当类库中找不到且参考表中未定义某对象时，eM Plant 可从该目录加载对象。更多信息请参阅"使用您自己的 Plant Simulation 资源对象（Using Your Own Plant Simulation Resource Objects）"。

**Times 选项卡**

在此选项卡上可以进行与仿真运行相关的设置。

- **Simulation time**（仿真时间）
    
  输入一次仿真运行的时长。该时间值将在模型生成过程中传递给事件控制器。
- **Start of statistic collection**（统计采集起始时刻）
    
  在此字段中输入统计数据复位的时间。该值将在模型生成过程中传递给事件控制器。

**Modelling 选项卡**

在此选项卡上进行的设置将直接影响所创建的模型。

- **Frame scaling factor**（框架缩放系数）
    
  创建框架时将使用该缩放系数，资源随后会按其坐标定位在该框架中。
- **Unit of sink statistic**（汇统计单位）
    
  选择用于评估汇（sink）对象所采集数据的单位。通常数据以小时为基准进行评估，例如每小时产量。而对于加工时间非常长的生产过程（例如航空工业），则可能希望选择日或月等其他时间基准。
- **Positioning of resources**（资源定位方式）
    
  资源通常按其坐标定位。在某些情况下资源没有有效坐标，因此所有资源会位于同一位置。为避免这种情况，您也可以选择使用相应操作的坐标来定位资源。

<a id="v1-s47"></a>

#### 工艺设计器接口 3D 菜单（The Process Designer Interface 3D Menu）

在创建仿真模型之后，使用此菜单功能可创建三维模型。

<a id="v1-s48"></a>

#### 工艺设计器接口 Help 菜单（The Process Designer Interface Help Menu）

- **Help**
    
  此菜单项打开在线帮助的 pdf 文件，其内容与本节"工艺设计器 eM Plant 集成"相同。
- **About AMG**
    
  此菜单项打开一个消息框，其中显示当前 AMG 程序的版本和日期。

<a id="v1-s49"></a>

### 使用您自己的 Plant Simulation 资源对象（Using Your Own Plant Simulation Resource Objects）

在很多情况下需要使用具有特殊行为的资源，因此必须构建专用资源。要创建可用于模型生成过程的 eM Plant 对象，您需要执行以下步骤。

使用框架（frame）构建您的资源对象。将物料流的接口命名为 In1、In2、…（输入接口）和 Out1、Out2、…（输出接口）。

如果该对象有特殊属性，请使用与工艺设计器中相同的属性名称。属性的使用方式必须在初始化方法中定义。

默认要求以下属性：

- **proctime**
    
  该属性定义资源的加工时间。属性类型必须为 "time" 或 "real"，加工时间以秒为单位定义。
- **availability**
    
  该属性保存资源的可用率，取值为 1 到 100 之间的实数。
- **MTTR**
    
  该属性表示平均修复时间（mean time to repair），以秒为单位。
- **MTBF**
    
  有时使用平均故障间隔时间（mean time between failures）来定义资源的故障。时间以秒为单位。

要将新资源添加到 AMG，可在 eM Plant 中加载 `DetailedSimulation.spp` 文件，在类库中单击鼠标右键并选择 **Load object**。对象加载完成后，打开资源映射表，输入该资源的标识符、对象的路径，并在必要时输入构造方法和属性映射。

如果不需要属性映射，则无需自行加载对象。在生成过程中，AMG 首先在资源映射表中查找该资源。如果 AMG 未找到与所定义的 eM Plant Type（在工艺设计器中定义）相符的条目，AMG 将在对象目录（参见 Object library）中搜索与工艺设计器 **eM Plant Type** 字段中所定义名称相同的对象文件。如果该对象文件存在，AMG 将自动加载该对象。

<a id="v1-s50"></a>

### 仿真选项卡参考（Simulation Tabs Reference）

<a id="v1-s51"></a>

#### 操作的仿真选项卡（Simulation Tab for Operations）

打开操作树并选择 **Simulation** 选项卡，其中包含关于详细仿真的附加信息。

**操作：Plant Simulation 输入参数**

| 参数（Parameter）         | 是否使用（Used） | 说明（Explanation）                                                                                      |
| --------------------- | ---------- | ---------------------------------------------------------------------------------------------------- |
| Allocated time        | 是          | 操作时间，以秒或分钟为单位（取决于 Simulation Preferences 对话框中的设置）。                                                   |
| Verified time         | -          | 如果未定义分配时间（allocated time），则使用已核验时间。                                                                  |
| Accuracy              | -          | 尚不支持。                                                                                                |
| Defect                | -          | 尚不支持。                                                                                                |
| Plant Simulation type | 是          | 从预定义列表中选择的操作类型（assembly 等）；如果未选择设置，Plant Simulation 会根据连接到该操作的流上的零件分配情况确定操作类型。默认值为空，即表示 "operation"。 |

**支持的操作类型（Supported Operation Types）**

| 操作类型        | 说明                                                                            |
| ----------- | ----------------------------------------------------------------------------- |
| operation   | 默认操作类型，当 Plant Simulation Type 字段为空时也使用此类型。该操作每次加工一个零件。若输入流上有多个零件，则按到达顺序逐个加工。 |
| assembly    | 装配操作；所有输入零件均被消耗，加工结束时恰有一个零件离开该操作。                                             |
| disassembly | 拆卸操作；恰有一个零件进入该操作，多个零件离开。                                                      |

<a id="v1-s52"></a>

#### 资源的仿真选项卡（Simulation Tab for Resources）

打开资源树并选择 **Simulation** 选项卡，其中包含若干文本框供您输入详细仿真所需的值，同时也显示由详细仿真得出的若干数值。

**资源：Plant Simulation 输入参数**

| 参数（Parameter）         | 是否使用（Used） | 说明（Explanation）                                                                                       |
| --------------------- | ---------- | ----------------------------------------------------------------------------------------------------- |
| Plant Simulation type | 是          | 映射到某个 Plant Simulation 对象类；资源实例将使用该类在 Plant Simulation 中创建。                                           |
| Simulation relevant   | 是          | 将该资源标记为与详细仿真相关。默认值为 "simulation relevant"（仿真相关）。                                                      |
| Primary Resource      | 是          | 如果一个操作分配了多个仿真相关资源，可使用此复选框在执行该操作时将某个资源与零件关联。                                                           |
| Availability          | 是          | 定义资源的可用率；取值范围 1 – 100。如果此字段为空，则默认为 100%。                                                              |
| MTTR                  | 是          | 平均修复时间；整数值，按 Preferences 对话框中的设置表示秒或分钟。当可用率介于 >0 与 <100 之间时，必须定义 MTTR 或 MTBF 之一。若两者均未定义，则假定可用率为 100%。 |
| MTBF                  | 是          | 平均故障间隔时间；整数值，按 Preferences 对话框中的设置表示秒或分钟。取值定义同上。                                                      |
| Max Throughput        | -          | 尚未使用。                                                                                                 |
| Capacity              | 是          | 整数值，表示 Plant Simulation 目标对象的容量；在缓冲区中直接使用，否则该值仅复制到 Plant Simulation 自定义属性 Capacity 中。                 |
| Cycletime             | -          | 尚未使用。                                                                                                 |
| Cycle                 | -          | 尚未使用。                                                                                                 |
| Amount                | -          | 尚未使用。                                                                                                 |
| Protective circuit    | 是          | 字符串值，定义保护回路的名称；在生成的 Plant Simulation 模型中，会在模型根框架中创建相应对象。                                              |

**支持的 Plant Simulation 资源类型（Supported Plant Simulation Resource Types）**

Plant Simulation 资源类型定义了为分配给操作的仿真相关工艺设计器资源创建哪些 Plant Simulation 目标对象。资源映射是可配置的。

> **注意**
>
> 标记为仿真相关但未在仿真相关操作中使用的资源，不会在 Plant Simulation 侧创建。

| 资源类型         | 说明                                         |
| ------------ | ------------------------------------------ |
| SingleProc   | 使用 Plant Simulation 基本对象 SingleProc。       |
| ParallelProc | 使用 Plant Simulation 基本对象 ParallelProc。     |
| Buffer       | 使用 Plant Simulation 基本对象 IOBuffer。         |
| Line         | 使用 Plant Simulation 基本对象 Line。             |
| Assembly     | 使用 Plant Simulation 基本对象 Assembly。         |
| Dismantle    | 使用 Plant Simulation 基本对象 DismantleStation。 |
| Station      | 使用应用对象 Station，其功能与 SingleProc 相当。         |

**资源：Plant Simulation 结果参数**

Simulation 选项卡右列中的值为仿真结果重新导入工艺设计器后所确定的统计结果。其中仅部分参数用于详细仿真。

| 参数（Parameter）            | 是否使用（Used） | 说明（Explanation）                  |
| ------------------------ | ---------- | -------------------------------- |
| Calculated availability  | -          | 尚不支持。                            |
| Sim. average utilization | -          | 用于工艺设计器接口。                       |
| Sim. Throughput rate     | -          | 用于工艺设计器接口。                       |
| Sim Throughput time      | -          | 用于工艺设计器接口。                       |
| Working percentage       | 是          | 资源上加工零件的时间占比。                    |
| Blocked percentage       | 是          | 因后续工位或资源被阻塞/忙碌，已完工零件滞留在资源上的时间占比。 |
| Blocked missing worker   | 是          | 资源因缺少人工作业人员而无法工作。                |
| Waiting percentage       | 是          | 资源已就绪并等待零件的时间占比。                 |
| Disrupted percentage     | 是          | 资源处于故障模式的时间占比。                   |
| Pause percentage         | 是          | 资源处于暂停模式的时间占比。                   |
| Stopped percentage       | -          | 未使用。                             |
| Unplanned percentage     | 是          | 非计划状态的时间占比。                      |
| Simulated cycle time     | -          | 尚不支持。                            |
| Calculated cycle time    | -          | 尚不支持。                            |

<a id="v1-s53"></a>

#### 产品的仿真选项卡（Simulation Tab for Products）

打开产品树并选择 **Simulation** 选项卡。

下述功能目前尚不支持，计划在后续版本中提供（当前版本中所有属性字段均为非活动状态）。

该选项卡包含以下文本框：

- **Mix. percentage**（混合百分比）：当存在多种产品时，可定义所选产品在全部产量中所占的百分比。仿真中只有一种产品时，无需输入值。存在多种产品但未指定值时，所有产品将均分混合百分比。
- **Batch size**（批量大小）：此字段中的值用于创建零件批次。若不输入值，默认为 10。
- **Product type**（产品类型）：从下拉控件中选择产品类型。
  - **S** —— 产品以单一流串行生产。
  Product type 的默认值为 S，目前不支持其他类型。

运行详细仿真后，右侧字段由 Plant Simulation 创建的导入文件填充：

- **Amount per day**：显示每天生产的产品数量。
- **Throughput per hour**：显示每小时的零件数量。

<a id="v1-s54"></a>

### SimpleDetailedStudy 首选项（SimpleDetailedStudy Preferences）

Simulation Preferences 对话框既用于工艺设计器的粗略仿真，也用于详细仿真。因此，其中只有部分设置用于详细仿真。

| 设置（Setting）              | 是否使用（Used） | 说明（Explanation）                                                                                     |
| ------------------------ | ---------- | --------------------------------------------------------------------------------------------------- |
| Plant Simulation version | 是          | 定义所使用的 Plant Simulation 版本；推荐值：7.0。                                                                 |
| License Type             | 是          | 定义 Plant Simulation 以 Development、Application 还是 Runtime 模式启动；必须具备 Plant Simulation 和 AMG 模块的相应许可证。 |
| Start Excel              | -          | 仅用于工艺设计器粗略仿真。                                                                                       |
| Excel Export             | -          | 仅用于工艺设计器粗略仿真。                                                                                       |
| Time Unit                | 是          | 仿真模型中操作时间数值按秒或分钟解释。                                                                                 |
| Statistics Offset Type   | -          | 仅用于工艺设计器粗略仿真。                                                                                       |
| Statistics Offset Value  | -          | 仅用于工艺设计器粗略仿真。您可以在 Plant Simulation 事件控制器对话框的 Settings 选项卡中进行相应定义。                                   |

**MeanTimeDistribution** 选项卡的设置不用于详细仿真。

<a id="v1-s55"></a>

### 详细仿真配置（Detailed Simulation Configuration）

<a id="v1-s56"></a>

#### 定义新的仿真细节（Defining New Simulation Details）

对于使用 Plant Simulation 开发许可证的仿真专家而言，可以定义供详细仿真使用的新类。关于新 Plant Simulation 目标对象结构的要求说明超出了本文档的范围。尽管如此，仍然可以定义传输到 Plant Simulation 对象的属性集合。

<a id="v1-s57"></a>

#### 配置文件（Configuration File）

配置文件 `SimulationTab.cfg` 位于以下文件夹中：

```
~\Process Designer\Applications\DetailedSimulation
```

该文件包含仿真选项卡中下拉列表所显示值的定义列表。专家用户可根据需要添加或修改这些设置。

```
[Resource:eMPlantType]
station
assembly
disassembly
conveyor
buffer
[Operation:eMPlantType]
operation
assembly
disassembly
[Product:ProductType]
S
```

> **注意**
>
> 修改这些定义时，必须同步修改位于 Plant Simulation 模型 `DetailedSimulation.spp` 中的 Plant Simulation 映射表。请参阅"映射到 Plant Simulation 目标对象（Mapping to Plant Simulation Target Objects）"。

<a id="v1-s58"></a>

#### 映射到 Plant Simulation 目标对象（Mapping to Plant Simulation Target Objects）

Plant Simulation 模型中包含带有 **Configuration** 选项卡的 Process Designer Plant Simulation Integration 对话框，其中有一个按钮可打开用于配置资源类型和相关工艺设计器属性的表格。

`plantType` 列定义仿真对象类的名称。为规划器资源的 Plant Simulation 类型所配置的每种资源类型都应在此维护。

`Path` 列包含指向 Plant Simulation 类库中类对象的指针。所有已配置的类对象都位于以下 Plant Simulation 文件夹中：

```
.ApplicationObjects.AMG.ModelGeneration.Objects
```

也可以指向位于其他位置的 Plant Simulation 类。

如果未指定对象类，Plant Simulation 将在整个类库（Internal 文件夹除外）中搜索同名对象。如果未找到任何对象，则使用默认对象（station）。

`Method` 列指向一个 Plant Simulation 方法，该方法在仿真模型生成后执行。此功能仅应由 Plant Simulation 专家使用。

> **注意**
>
> 属性可以在 Plant Simulation 侧查看、使用和评估，但在当前版本中，将仿真结果导入工艺设计器时不会更新这些属性。该功能计划在下一版本中提供。

您可以在此表中为每个对象定义一组属性（名称和数据类型）。生成对象时，AMG 会检查所定义的属性是否存在。如果不存在，AMG 将使用给定的数据类型创建该属性。AMG 会尝试在从工艺设计器传输来的数据中定位该属性的值，并将其赋给该属性。

如果您希望在工艺设计器中定义该属性，必须将其添加到项目的定制（customization）中。

**复制到仿真目标对象的属性集**

您在 `AttrName` 和 `AttrTypeX` 列中输入工艺设计器属性名，这些属性将被传输到 Plant Simulation 目标对象。Plant Simulation 模型生成过程会创建相应类型的自定义属性，并将工艺设计器属性值复制到仿真对象。

该配置表预定义了 8 个属性，但也可以向配置表中添加更多的属性列对。

在默认实现中只使用预定义属性。仿真专家需要自行将附加属性的使用集成到 Plant Simulation 模型中。

<a id="v1-s59"></a>

### 附加建模约束（Additional Modeling Constraints）

除了"准备 Plant Simulation 仿真研究"中所述的约束之外，对于打算在 Plant Simulation 中进行仿真的工艺模型，用户还应确保其结构满足以下要求：

- 不得将 human 对象置于已映射到 Plant Simulation 目标对象的仿真相关资源之下。
- 不得将某个仿真相关资源置于另一个已映射到 Plant Simulation 目标对象的仿真相关资源之下。
- 在工艺设计器中可以将仿真相关资源和 human 分配给孪生资源，但该信息不会被导出。

<a id="v1-s60"></a>

## 附加功能及与 Tecnomatix 应用的集成（Additional Functionality and Integration with Tecnomatix Applications）

<a id="v1-s61"></a>

### 扩展能力（Extended Capabilities）

工艺设计器提供扩展能力，可访问 Unite Data 与 Tecnomatix API 功能。有关管理工具的信息，请参阅 Tecnomatix 管理文档中的 Tecnomatix Doctor 一节。

<a id="v1-s62"></a>

### 统一数据（Unite Data）

Tecnomatix 组件的统一数据表达（United Data Representation）是一种包含较少组件内部对象（世界模型实体等）的表达形式。通过减少内部对象数量，组件的加载时间和内存占用同时降低，图形性能也得到提升。

仅工艺设计器应用支持加载统一表达。UNIX 应用和其他 Tecnomatix 应用无法加载统一表达，但也不会受其影响。

工程迁移工具（Engineering Migration Utility，见 Tecnomatix 管理文档的 Data Migration 一节）可将早期版本的单元格式（`ppinteg.ce`）数据转换为 7.1 及更高版本所使用的 xml 格式。

<a id="v1-s63"></a>

### Tecnomatix API

借助 Tecnomatix API，开发人员可以在工艺设计器之上构建应用程序。功能相关信息请参阅《Tecnomatix API Manual》。

<a id="v1-s64"></a>

### JT 读取（JT Read）

JT Read 允许您在工艺设计器和 Process Simulate 中加载并读取原生 `*.jt` 文件，无需将其转换为 `*.cojt` 格式。过去，Tecnomatix 应用使用 `*.co` 格式表示三维数据，要求将导入的 CAD 数据从原始 CAD 格式（例如 `*.jt`）转换为 `*.co` 格式。

`*.jt` 文件作为 CC 流程的一部分被复制到系统根目录（System Root）。

在 `*.jt` 格式下使用工艺设计器 / Process Simulate 的功能，与使用 `*.cojt` 格式几乎等效。

用户还可以在同一个研究/会话/树中混合使用 `*.jt` 和 `*.cojt` 数据，并可为叶节点包含 `*.jt` 数据的子树创建最终项（End Items）。

目前 JT Read 项目的实现存在以下限制：

- 仅能将 `*.jt` 文件加载到工艺设计器 / Process Simulate
- 只读（不支持建模）
- 只能从系统根目录读取 `*.jt` 文件（".cojt" 文件夹中包含 `*.jt` 文件）
- 不支持 JT 装配体
- 不支持 `*.jt` 文件中的运动学信息。需要运动学的组件必须转换为 `*.cojt` 格式
- 不支持 `*.jt` 精确几何 —— 仅支持近似几何

<a id="v1-s65"></a>

## 升级到当前版本（Upgrade to Version）

<a id="v1-s66"></a>

### 升级组件（Upgrading Components）

**Upgrade to Version** 选项是一种管理工具，用于将在早期版本工艺设计器中创建的组件升级到当前版本。如果您正在使用当前版本，但需要使用早期版本建模的组件，此选项尤为重要。要更新组件，请选择 **Upgrade to Version** 并选择所需目录。所选目录中的所有组件都会升级到当前版本，并补充 .JT 文件以适配 Direct Model 图形引擎的使用。

**Upgrade to Version** 不作用于原生 JT 组件。

升级组件文件有三种方式：

- 从命令行升级，参见"从命令行升级组件文件（Upgrading Component Files from the Command Line）"。
- 使用 **Upgrade CO Prototypes to Version**。
- 使用独立应用程序，参见"使用独立应用程序升级组件（Upgrading Components Using the Standalone Application）"。

**Upgrade to Version** 还具备以下功能：

- 在升级组件时自动导入 Robcad 材质。如有需要，您可以关闭此选项。
- 为已升级为 .co 或 .cojt 格式的组件提供仅导入 Robcad 材质的选项，无需执行完整升级。此选项通常用于最初在 Robcad 中创建的组件。

> **注意**
>
> 西门子建议从工艺设计器或 Process Simulate 中使用 **Upgrade CO Prototypes to Version** 来升级原型。升级为 COJT 时，此操作还会更新零件原型属性 Physical 选项卡中的 **3D File** 字段，使其指向升级后的 COJT 对象（从 CO 升级到 CO 时不需要此类更新）。

<a id="v1-s67"></a>

### JT 组件建模（Modeling JT Components）

默认情况下，**Upgrade CO Prototypes to Version**（按定义即 CO 到 CO 的升级）会在 .co 组件文件下添加 JT 文件。

使用 `-co2jt` 标志会将 .co 层次结构"折叠"为 .cojt 文件，并将 JT 文件作为子项放置在 .cojt 组件之下。

此选项使您能够在 Process Simulate 中对该组件建模。但如果将该组件加载到 Robcad 应用中，建模功能将被禁用，因为 Robcad 无法识别 .cojt 格式（它会查找 .co 组件）。

**Upgrade to Version** 功能通过位于 Tecnomatix 安装目录中的 `UpgradeToVersion` 可执行文件启动。

> **注意**
>
> 您必须对组件文件拥有写权限，否则升级过程将失败。

<a id="v1-s68"></a>

### 从命令行升级组件文件（Upgrading Component Files from the Command Line）

**操作步骤**

1. 在 Windows 开始菜单中选择 **Run**，显示 shell 窗口。
2. 输入 `cmd` 并按键盘上的 `<Enter>` 键，命令控制台随之打开。
3. 手动输入 `UpgradeToVersion` 可执行文件的完整路径，或将其从 Windows 资源管理器拖到 shell 窗口中。请确保路径位置以引号开头和结尾。
4. 按键盘上的 Enter 键，将显示语法用法信息。
5. 再次手动输入 `UpgradeToVersion` 可执行文件的完整路径，或将该文件从 Windows 资源管理器拖到 shell 窗口中。
   > **注意**
   >
   > 请确保路径两端有引号。
   >
   > 为保留旧数据，请在使用本工具之前将其复制到其他位置。如果仅选择了 `-co2jt` 选项，则只需复制 .cojt 文件。
6. 使用以下参数之一指定要更新的文件：
   - `-dir <dirname>`，其中 dirname 为待升级目录的完整路径和名称，例如 `"E:\Cells\cells5.0\Kinematics"`。当您希望更新整个目录时应使用此参数。
     > **注意**
     >
     > 您也可以从 Windows 资源管理器拖动目录，但要确保路径两端有引号。
   - 使用以下目标格式之一：
     - `[-co2co <upgrade components to new version>]` —— 默认选项。
     - `[-co2jt <write components as .cojt>]` —— 仅适用于 `-comp` 和 `-dir` 选项。
     - `[-updateMaterialsOnly <update the materials of the selected components without performing an upgrade>]` —— 接受 .co 或 .cojt 输入目录或两者混合。可以与 `-CleanInterval` 和 `-log` 选项一起使用，但使用其他任何选项都会返回错误。
   - `-comp <compname>`，其中 compname 为待升级组件文件的完整路径和名称。当您希望更新特定组件时应使用此参数，必要时可以指定多个组件。
   - `-op <tolerance>`，其中 tolerance 为所需公差。此选项使用 SGI Optimizer 通过依据组件几何重新计算其网格化（tessellation）来优化几何数据。新计算得到的网格化通常优于原有网格化，即在保持组件外观的同时包含更少的三角面，使数据更易于由显卡渲染（绘制），从而提升图形性能。
     > **注意**
     >
     > - 优化器无法处理 CAD 集成组件，因为 TUNE 没有它们的几何数据。
     > - 网格化（tessellation）即近似表达：对象几何的近似多边形表达。
   - `[-libroot <library root full path>]`：对包含库组件的超级组件执行版本升级时，必须指明库根路径，以便 Upgrade to Version 工具定位这些组件。
   - `[-EntityLevel <include detailed representation of component in .jt in addition to united representation>]`：仅与 `-co2co` 选项相关。默认情况下 .jt 文件中只写入统一表达。
   - `[-DetailedOnly <include only detailed representation of component in .jt>]`：仅与 `-co2jt` 选项相关。默认情况下 .jt 文件中同时写入统一表达和详细表达。
   - `[-Exclude2DInUnited <exclude 2D objects in united representation>]`：将原 .co 组件中存在的二维实体（例如直线、点和曲线）从 .jt 文件的统一表达中排除。默认情况下这些实体会同时进入 .jt 的统一表达和详细表达。
   - `[-ExcludeFramesInUnited <exclude frames in united representation>]`：将原 .co 组件中存在的坐标系（frame）从 .jt 文件的统一表达中排除。默认情况下坐标系会同时进入 .jt 的统一表达和详细表达。仅与非运动学组件相关。
   - `[-RemoveGmsimperf <Remove the united representation intermediate file (.gmsimperf) from the upgraded .co component>]`
   - `[-ExcludeMaterialDefinitions]`：默认情况下系统会执行 Update Materials，将所选组件的 Robcad 材质导入升级后的组件。选择此选项后系统将跳过该动作。
   - `[-CleanInterval <Number of uses before UpgradeToVersion process is automatically reloaded>]`：定义在进程自动重启之前升级到当前版本的组件数量。升级大量大型组件时，这是一个需要设置的重要参数。
   - `[-ForceUpgradeApproximation <recreate united representation approximation>]`：重新生成 .gmsimperf，例如以便能够使用 `-Exclude2DInUnited` 和 `-ExcludeFramesInUnited` 标志。此操作可能较为耗时。
   - `[-LODRatio <LOD Level 1 ratio value>]`：以基础细节层级（Level 0，使用最多的三角面以生成最平滑的图形）与下一细节层级（Level 1）之间的比值形式，定义图形查看器中细节层级之间的过渡。两者比值越小，缩放时这两个细节层级之间的过渡越明显。Level 1 与 Level 0 的默认比值（定义为"Level 1 三角面数 / Level 0 三角面数"）为 0.5。如果 .co 下已存在 .gmsimperf 文件，则忽略此标志。
   - `[-skip <当使用 -co2co 升级时：跳过 co 下已有 jt 的组件> <当使用 -co2jt 升级时：跳过已有相关 cojt 的组件>]`
   - `[-log <file name>]`：创建包含 Upgrade to Version 过程信息的日志文件。西门子建议始终带此参数运行 Upgrade To Version。
7. 按键盘上的 Enter 键，系统开始升级过程。

<a id="v1-s69"></a>

## 启动工艺设计器（Launching Process Designer）

<a id="v1-s70"></a>

### 直接启动工艺设计器（Direct Launch of Process Designer）

直接打开工艺设计器时，您将登录 eMServer 并打开一个项目。

**操作步骤**

1. 双击 Process Designer 图标，或选择 **Start→Programs→Tecnomatix→Planning Applications→Process Designer**。此时打开 Login to eMServer 窗口。
2. 使用您的用户名和密码登录，Open Project 窗口随之打开。
3. 选择一个项目并单击 **Open**，主窗口打开。
4. 要将某个研究加载到图形查看器，请在导航树中选中它并选择 **eMServer→Open in Graphic Window**，或单击 **Open** 图标，或右键单击并选择 **Open**。

> **注意**
>
> - 可以同时从两台计算机启动工艺设计器并使用同一用户名登录 eMServer 帐户。例如，如果某个静默崩溃的会话仍然存活，您仍可从另一台计算机登录该用户帐户。
> - 如果您在两台计算机上登录了同一用户帐户，则在一台计算机会话上修改 eMServer 数据的任何操作，都将延迟到另一会话上已在运行的同类操作完成之后执行。
> - 如果该延迟超过一小时，被延迟的操作将自动结束，且不会在 eMServer 上进行任何更新。您可以再次尝试执行该用户操作。

<a id="v1-s71"></a>

### 打开现有项目（Open an Existing Project）

> **注意**
>
> 另请参阅"创建新项目（Create New Project）"。

要打开现有项目：

**操作步骤**

1. 选择 **File→Project** 组 **→Open Project**。

   系统会打开与启动工艺设计器时相同的 Open Project 窗口，其中列出了可用项目。项目文件名上方的路径指明了当前的项目位置。
2. 从列表中选择所需项目。
3. 从组合框中选择项目发布版本。默认情况下，选择项目时系统会显示最新的发布版本。
4. 双击该项目或单击 **OK** 即可打开。

   系统将打开导航树视图，显示所选项目的顶层节点。

<a id="v1-s72"></a>

### 设置系统根目录（Setting the System Root）

<a id="v1-s73"></a>

#### 创建新的系统根目录（安装过程中）（Creating a New System Root (during Setup)）

本节适用于首次安装工艺设计器的情形（无需卸载先前版本）。

在安装工艺设计器的过程中，系统会要求管理员设置一个文件夹路径作为系统根目录（system root）。除数据之外，系统根目录还包含若干配置文件，这些文件在创建时带有默认内容。您可以稍后通过编辑这些配置文件来更改它们，也可以通过使用工艺设计器间接导致这些文件发生变化。

安装程序会在系统根目录下创建 `General`、`RulesImages` 和 `xmlFiles` 子文件夹（`RulesImages` 与 `xmlFiles` 文件夹会被安装程序完全覆盖，此处不作讨论）。

安装程序会将 `LibraryBrowser` 文件夹以及以下文件复制到 `General` 文件夹下：

- `ColorIndicationQueryCnfg.xml` —— 查询颜色配置文件，存储规则及其匹配的颜色。
- `ListOfValues.xml` —— 包含不同属性有效值的映射列表，供 PowerBar、Table View、Schematic Viewer 和 Quotation 等应用使用。
- `ListOfValuesExtension.xml` —— 包含 Schematic Viewer 的映射扩展。
- `NewCommandConfiguration.xml` —— 确定用户可以在特定对象下按层次创建（使用 New 命令）哪些对象。
- `NewCommandConfiguration.xmlEXAMPLE` —— New Command 配置文件的示例。

安装程序会将以下文件复制到 `LibraryBrowser` 文件夹下：

- `LBCustomizedFields.xml` —— 包含用户在（库浏览器的）NEW/Edit 对话框中所见的特定类和字段。
- `LBSearchFields.xml` —— 包含（库浏览器中）每个类的搜索字段定义。

用户在使用工艺设计器的过程中可能会陆续添加其他文件，例如：

- `PublicApplicationViews.xml` —— 存储所有公共布局。打开 Layout Manager 时创建，添加或修改布局时更新。
- `SavedSearches.xml` —— 存储 Power Bar 的已保存搜索。存储 Power Bar 搜索时创建。

<a id="v1-s74"></a>

#### 重建系统根目录（安装过程中）（Recreating a System Root (during Setup)）

本节适用于卸载先前版本工艺设计器并安装新版本的情形。

从旧版本升级到新版本时，卸载过程会删除旧版本安装程序所安装的全部配置文件，但不会删除此后手动修改过的文件。因此强烈建议在卸载旧版本之前，完整备份 `RulesImages` 和 `xmlFiles` 文件夹。

在 `General` 文件夹中，安装程序的行为有所不同：

若 `ListOfValues.xml` 文件先前不存在，将以 `ListOfValues.xml` 名称创建。无论如何都会创建 `ListOfValues_80.xml` 文件。如有需要，管理员必须将旧的 `ListOfValues.xml` 文件合并到新的 `ListOfValues_80.xml` 文件中。这同样适用于 `ListOfValuesExtension_80.xml` 和 `QMTypeValues_80.xml`。

`General` 文件夹中的其余文件若已存在则保持不变，若缺失则会被创建。

<a id="v1-s75"></a>

#### 更改系统根目录（Changing a System Root）

本节适用于重定向现有系统根目录的情形（非安装过程中）。

在安装之后的某个时刻更改系统根目录时，不会将任何信息从旧系统根目录复制到新系统根目录。因此，管理员必须将相关配置文件复制到新位置，即复制 `General`、`RulesImages` 和 `xmlFiles` 文件夹。

<a id="v1-s76"></a>

### 退出工艺设计器（Exiting Process Designer）

要退出工艺设计器：

- 选择 **File → Exit**；或
- 单击关闭按钮以关闭窗口。

<a id="v1-s77"></a>

## 设备（Equipment）

工艺设计器支持使用称为设备（Equipment）的层次化原型。设备原型可以包含其他设备原型或工具原型（Tool Prototypes）的实例作为子项，这些子项称为原型引用（prototype occurrences）。请注意，这些原型引用本身不能再拥有子项。

您可以像实例化其他库对象一样实例化设备原型。实例化设备原型时，也会同时实例化其子项（原型引用）。如果这些引用的原型也有子项，则这些"孙级"对象同样会被实例化。

例如，用户现在可以对由机器人（Robot）原型和导轨（Rail）原型组合而成的设备建模。可以定义机器人与导轨之间的运动学关系，为设备添加信号（也可使用机器人和导轨中已有的信号），然后将该设备保存到库中。之后即可在工厂布局中复用该设备，而无需重新定义机器人与导轨之间的运动学、信号、相对位置等。

您无法直接修改设备实例的层次结构，而必须将更改（例如添加或删除子项）应用于设备原型，然后使用 **Reconcile Instances**（协调实例）将这些更改更新到设备实例。

<a id="v1-s78"></a>

## 概览（Overview）

工艺设计器应用程序窗口使您能够访问全部可用的三维查看和操控功能，包含以下主要元素：

- 功能区（Ribbon）
- 默认键盘快捷键（Default Keyboard Shortcuts）
- 状态报告（Status Reports）
- 状态栏（Status Bar）
- 版本（Versions）
- 图形查看器（Graphic Viewer）
- 各类查看器（Viewers）：
  - 碰撞查看器（Collision Viewer）
  - 比较查看器（Compare Viewer）
  - eMS 库浏览器（eMS Library Browser）
  - IPA 查看器（IPA Viewer）
  - 制造特征树（Mfg Tree）
  - 对象树（Object Tree）
  - 操作树（Operation Tree）
  - 工艺模块（Process Modules）
  - 产品树（Product Tree）
  - 资源树（Resource Tree）
  - 原理图查看器（Schematic Viewer）
  - 快照编辑器（Snapshot Editor）
- 导航树（Navigation Tree）
- 属性查看器（Properties Viewer）
- 关系查看器（Relations Viewer）
- 甘特图查看器（Gantt Viewer）
- PERT 查看器（PERT Viewer）
- 表格视图（Table View）
- 模块（Modules）
- 多用户并发访问（Multi-User Concurrent Access）
- 报价（Quotation）
- 生产线平衡（Line Balancing）
- 装配模块（Assembly Module）
- 可定制选项卡（Customizable Tabs）
- 选项卡顺序管理器（Tab Order Manager）
- 任务监督器（Task Supervisor）
- 加载实体层级（Load Entity Level）
- 卸载实体层级（Unload Entity Level）
- 包装容器（Packing Containers）

按照"启动工艺设计器"中所述启动工艺设计器后，主应用程序窗口将如下图所示打开。

工艺设计器的界面与许多 Windows 应用程序类似。这意味着您必须先选择要操作的对象，才能激活所需的选项。对象可以在某个树中或在图形查看器中选择，各项功能则通过菜单、工具栏和上下文菜单访问。

<a id="v1-s79"></a>

## 搜索（Search）

搜索（Search）选项位于应用程序右上角附近，可用于搜索研究中对象的名称以及应用程序中命令的名称。您可以输入完整单词或单词的一部分，搜索名称中包含指定单词或字母的所有对象或命令。

**操作步骤**

1. 单击 **Search** 框，显示 Search 对话框。
2. 在文本框中输入一个或多个单词、单词的一部分或文本字符串，然后按键盘上的 Enter 键或单击放大镜图标。

   搜索结果会列在展开的对话框中，同时在图形查看器中高亮显示，并在相关树中以粗体显示。单击列表中的某个命令会打开该命令并关闭 Search 对话框。搜索结果的总数显示在对话框中 **Commands** 和 **Objects** 旁边的括号内。
   > **注意**
   >
   > 如果找到的对象在树中处于隐藏状态，树会展开以显示该对象（前提是已在 General 选项卡中设置了该选项）。
   将文本框留空并单击 **Search** 图标，可列出研究中的所有对象（以及所有命令）。
3. 您可以配置搜索范围，仅包含对象（Objects）、仅包含命令（Commands），或两者全部。

<a id="v1-s80"></a>

## 功能区（Ribbon）

工艺设计器的功能区位于标题栏下方，由若干选项卡组成，用于对工艺设计器的命令进行分组。本文档各章节详细介绍了各个功能区选项卡：

- File
- Home
- View
- Applications
- Study
- Kin
- Log
- Special Data
- Preparation
- Help 菜单（包含在线帮助以及当前工艺设计器版本的信息）

工艺设计器还包含许多不同的上下文菜单，可通过在用户界面的不同位置单击右键来显示。每个上下文菜单的内容取决于所选内容以及所在的查看器。

功能区和上下文菜单可以根据个人和组织的需求进行自定义，具体参见"自定义（Customize）"。

<a id="v1-s81"></a>

## 图形查看器工具栏（Graphic Viewer toolbar）

图形查看器工具栏显示在当前活动的图形查看器中（当打开了多个查看器时），默认位于查看器的上部中央。用户可以将其拖动到图形查看器边界内的任意位置。该工具栏包含视图变更命令（如 Zoom、View Center 等），以及 Pick Level、Measurements、Dimensions 和其他用于操控图形查看器中对象的命令，例如放置操控器（Placement Manipulator）。

> **注意**
>
> 图形查看器工具栏默认显示。您可以通过取消勾选 Graphic Viewer 选项卡中的 **Display viewer toolbar** 复选框来将其隐藏。

图形查看器工具栏在未使用时保持可见但呈灰显状态。

此外，将光标置于图形查看器中并按键盘空格键，会打开一个三段式的快捷工具栏（Quick toolbar）。该工具栏包含拾取和选择方面最常用的命令，只要持续按住空格键就会保持打开。

工具栏功能区可显著减少鼠标从图形查看器向上移动到功能区区域的频率。

要启用此功能，请在 Customize 对话框中勾选 **Toolbar**。

<a id="v1-s82"></a>

### 缩放到所选对象（Zoom to Selection）

**Zoom to Selection** 选项会调整图形查看器中的图像，使所选对象以特写方式显示。此选项便于对小型所选对象进行放大查看。

<a id="v1-s83"></a>

### 缩放至适合（Zoom to Fit）

**Zoom to Fit** 选项会调整图形查看器中的图像，使所有可见对象都能显示出来。此选项可方便地撤销缩放和平移所造成的大幅变化，并可用于判断图形查看器中是否存在远离目标内容的杂散对象。被隐藏（blanked）的对象会被忽略。

<a id="v1-s84"></a>

### 视图中心（View Center）

**View Center** 选项使您能够选择图形查看器中的任意一点作为视图中心。视图中心是对象旋转时所围绕的枢轴点。默认情况下，空间原点即为视图中心。

在图形查看器中选择一点并选择 **Graphic Viewer Toolbar → View Center**，即可平移视图，将所选点置于图形查看器的中心。

您也可以在单击目标点的同时按下 `<Ctrl>+<Shift>`。

例如，若视图中心为默认位置而某对象远离空间原点，则通过改变视点位置来观察对象的不同侧面时，该对象会在显示区域中大幅扫掠。如果将视图中心设置在该对象上，则视点位置的变化会使视点绕对象移动，而对象在显示中呈现为原地旋转。

<a id="v1-s85"></a>

### 视点（View Point）

**View Point** 选项决定您从哪个角度观察图形查看器中的图像。您可以从一系列特定角度中进行选择。从不同角度观察对象有助于发现装配体中的问题。

1. 转到 **Graphic Viewer Toolbar → Views** 组。
2. 选择一个视点。

图形查看器随即更新，从所选视点显示其内容，View Point 对话框关闭。

各种视点如下：

| 视点（View Point）       | 说明                                                                                 |
| -------------------- | ---------------------------------------------------------------------------------- |
| Normal to View Point | 将视点设置为垂直于您在图形查看器中拾取的位置。保留当前缩放比例。                                                   |
| Back                 | 将视点位置的方位角改为 90°、高度角改为 0°。此视图沿 Y 轴正向的负方向朝原点观察。键盘等效操作：按 `<Home>`，然后按左箭头或右箭头六次。       |
| Top                  | 将视点位置的高度角改为 90°，并旋转视图使 X 轴水平、Y 轴竖直。此视图沿 Z 轴正向的负方向朝原点观察。键盘等效操作：按 `<Home>`，然后按上箭头三次。 |
| Bottom               | 将视点位置的高度角改为 90°，并旋转视图使 X 轴水平、Y 轴竖直。此视图沿 Z 轴负向的正方向朝原点观察。键盘等效操作：按 `<Home>`，然后按下箭头三次。 |
| Front                | 将视点位置的方位角改为 270°、高度角改为 0°。此视图沿 Y 轴负向的正方向朝原点观察。键盘等效操作：按 `<Home>`。                   |
| Right                | 将视点位置的方位角和高度角均改为 0°。此视图沿 X 轴正向的负方向朝原点观察。键盘等效操作：按 `<Home>`，然后按右箭头三次。                |
| Left                 | 将视点位置的方位角改为 180°、高度角改为 0°。此视图沿 X 轴负向的正方向朝原点观察。键盘等效操作：按 `<Home>`，然后按左箭头三次。          |
| Q1                   | 将视点置于第 1 卦限（+X +Y +Z），高度角 30°、方位角 30°。键盘等效操作：按 `<Home>`，上箭头一次，右箭头四次。               |
| Q2                   | 将视点置于第 2 卦限，高度角 30°、方位角 120°。键盘等效操作：按 `<Home>`，上箭头一次，左箭头五次。                        |
| Q3                   | 将视点置于第 3 卦限，高度角 30°、方位角 210°。键盘等效操作：按 `<Home>`，上箭头一次，左箭头两次。                        |
| Q4                   | 将视点置于第 4 卦限，高度角 30°、方位角 300°。键盘等效操作：按 `<Home>`，上箭头一次，右箭头一次。默认视图即将视点置于第 4 卦限（Q4）。   |

<a id="v1-s86"></a>

### 着色模式（Shaded Mode）

**Shaded Mode** 选项将图形查看器中的所有对象显示为实体对象。

您可以通过选择 **Graphic Viewer Toolbar → Style** 组 **→ Shaded Mode** 更改图形查看器中所有对象的显示方式。图形查看器中的所有对象都会经过着色处理而呈现为实体。此后被显示出来的隐藏对象同样以着色模式呈现。

> **注意**
>
> 按 `<F10>` 可在 Shaded、Feature Lines over Solid、Feature Lines 和 Wireframe 显示方式之间循环切换。

<a id="v1-s87"></a>

### 实体上的特征线（Feature Lines over Solid）

**Feature Lines over Solid** 选项为图形查看器中的所有对象同时显示着色模式与黑色特征线。

> **注意**
>
> 按 `<F10>` 可在 Shaded、Feature Lines over Solid、Feature Lines 和 Wireframe 显示方式之间循环切换。

<a id="v1-s88"></a>

### 特征线（Feature Lines）

用户可以通过按键盘上的 `<F10>` 显示特征线。此视图对于创建聚焦装配过程的文档化工作流非常有用。

> **注意**
>
> - 按 `<F10>` 可在 Shaded、Feature Lines over Solid、Feature Lines 和 Wireframe 显示方式之间循环切换。
> - 对象会遮挡其后方的线条和对象。
> - 轮廓线（Silhouette Lines）始终显示。
> - 如果圆柱形对象显示不清晰，可尝试减小特征线之间的最小夹角和/或增加其线宽。有关如何配置特征线的信息，请参见 Graphic Viewer 选项卡。

<a id="v1-s89"></a>

### 线框模式（Wireframe Mode）

**Wireframe Mode** 选项将图形查看器中的所有对象显示为线框对象。

您可以通过选择 **Graphic Viewer Toolbar → Style** 组 **→ Wireframe Mode** 更改图形查看器中所有对象的显示方式，所有对象都将以线框形式呈现。此后被显示出来的隐藏对象同样以线框模式呈现。

> **注意**
>
> - 按 `<F10>` 可在 Shaded、Feature Lines over Solid、Feature Lines 和 Wireframe 显示方式之间循环切换。
> - 如果对象在图形查看器的线框模式下显示不清晰，可尝试以下各项的不同组合：
>   - 启用轮廓线（Silhouette Lines）。
>   - 将 Feature line angle 设置为较小的角度。
>   - 将 Feature line width 设置为较大的线宽。

有关如何配置这些参数的信息，请参见 Graphic Viewer 选项卡。

<a id="v1-s90"></a>

### 按类型显示（Display by Type）

**Display By Type** 选项使您能够选择在图形查看器当前视图中显示哪些类型的已加载对象。

1. 选择 **Graphic Viewer Toolbar → Visibility** 组 **→ Display By Type**，显示 Display By Type 列表。
2. 从 Display By Type 列表中选择要显示的对象，说明如下：

| 名称（Name）                    | 说明                                                                                                                                                                                                                       |
| --------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Show Selected Types         | 选择一种或多种类型后单击此图标，可显示所选类型（包括先前被隐藏的类型）。                                                                                                                                                                                     |
| Hide Selected Types         | 选择一种或多种类型后单击此图标，可隐藏所选类型。                                                                                                                                                                                                 |
| Display Only Selected Types | 单击此图标可显示所选类型（包括先前被隐藏的类型）并隐藏所有其他类型。                                                                                                                                                                                       |
| Display All                 | 单击此图标可显示所有类型（包括先前被隐藏的类型）。                                                                                                                                                                                                |
| Hide All                    | 单击此图标可隐藏所有类型。                                                                                                                                                                                                            |
| Delete Selected Types       | 单击此图标可删除所选类型（包括先前被隐藏的类型）。                                                                                                                                                                                                |
| Filter by Color             | 此过滤器可根据对象颜色显示和隐藏对象。可对所有对象应用颜色过滤，也可仅对一种或多种所选类型的对象应用。步骤：①从下拉菜单中；②选择 **Full color match**（选择仅以所选颜色着色的对象）或 **Partial color match**（选择部分以所选颜色着色、同时也带有其他颜色的对象）；③从调色板中选择一种颜色，参见"修改颜色（Modify Color）"。此后显示和隐藏类型的操作将仅影响以所选颜色着色的对象。 |

> **注意**
>
> 吸管取色器（eye-dropper color sampler）允许您根据图形查看器中的特定对象来选取过滤颜色。

> **注意**
>
> - 当您在图形查看器中显示所有对象时，树中的相关节点会以蓝色实心图标表示，指示其为已显示状态。更多信息请参阅"隐藏/显示对象（Blanking/Displaying Objects）"。
> - **Display By Type** 命令会显示所有表达形式中的实体 —— Detailed（详细）、Modeling（建模）以及 United 中的 Preserve 实体。

<a id="v1-s91"></a>

### 隐藏（Blank）

**Blank** 选项可在图形查看器中隐藏所选对象或操作（包括任何附加的注释）。该对象并未从数据库或对象树中移除，可以随时重新显示。

要隐藏对象或操作，可在图形查看器、对象树或操作树中选中它，然后选择 **Graphic Viewer Toolbar → Visibility** 组 **→ Blank**。所选对象或操作将在图形查看器中隐藏，并在对象查看器中其名称左侧显示为空心方块。

> **提示**
>
> 您也可以通过单击项目名称左侧的方块，在隐藏与显示状态之间切换。当复合操作中包含部分被隐藏的操作，或组件中包含部分被隐藏的实体时，该方块会呈现为半填充状态。

<a id="v1-s92"></a>

### 显示（Display）

**Display** 选项将被隐藏的对象或操作恢复为可见状态。

要重新显示被隐藏的对象或操作，可在对象树中选中它并选择 **Graphic Viewer Toolbar → Visibility** 组 **→ Display**。该对象或操作将显示在图形查看器中。当对象或操作在图形查看器中显示时，它在对象查看器中的名称左侧会带有一个实心方块。

<a id="v1-s93"></a>

### 全部显示（Display All）

**Display All** 选项在图形查看器中显示工程数据中包含的所有对象，包括先前被隐藏的对象。

选择 **Graphic Viewer Toolbar → Visibility** 组 **→ Display All**，即可在图形查看器中显示工程数据中的所有对象。所有对象（包括先前被隐藏的对象）都会显示出来，并在对象树中带有实心方块。

<a id="v1-s94"></a>

### 仅显示（Display Only）

**Display Only** 选项仅在图形查看器中显示所选对象，并隐藏工程数据中的所有其他对象。

> **注意**
>
> 按住 `<Ctrl>` 键并在图形查看器或对象树中单击所需对象，可选择多个对象。

要在图形查看器中单独显示特定对象，请在图形查看器或对象树中选中该对象，然后选择 **Graphic Viewer Toolbar → Visibility** 组 **→ Display Only**。所选对象将单独显示在图形查看器中，所有其他对象被隐藏。

之后您可以按需重新显示单个对象，或显示工程数据中的全部对象。

<a id="v1-s95"></a>

### 切换显示（Toggle Display）

**Toggle Display** 选项使图形查看器中的显示在被隐藏对象与已显示对象之间交替切换。

<a id="v1-s96"></a>

### 拾取层级（Pick Level）

选择拾取层级（Component 或 Entity），说明如下：

- **Component（组件）**：选中组件的任何部分时，整个组件都会被选中。
  - **Whole Component** —— 对所有组件而言，只能选中整个对象。
  - **Engineering Data** —— 每个对象都可独立选中（即坐标系、剖面、注释标记、尺寸等）。
- **Entity（实体）**：仅选中实体（即整个组件的一部分）。可选中的内容包括：
  - **Whole Component** —— 对非运动学组件而言，只能选中整个对象。
  - **Kinematic Link** —— 对运动学组件而言，每个连杆都可独立选中。
  - **Engineering Data** —— 每个对象都可独立选中（即坐标系、剖面、注释标记、尺寸等）。
- **Surface（面）**：仅选中曲面。
- **Edge（边）**：仅选中边。

<a id="v1-s97"></a>

### 拾取意图（Pick Intent）

单击以下某个 Pick Intent 图标，以确定单击对象时的精确取点位置：

- **Snap（捕捉）**：选择顶点、边的中点或面的中心中距离实际单击点最近者。这是默认的拾取意图。在图形查看器中使用 Minimal Distance 命令测量两个对象之间的距离时，此选项非常有用。
- **Self-Origin（自身原点）**：这是唯一依赖于拾取层级设置的拾取意图。如果拾取层级设为 component，则无论在何处拾取对象，都始终选中该组件的自身原点；如果拾取层级设为 entity，则选中当前所拾取实体的自身原点。
- **On Edge（边上）**：选择边上距离实际单击点最近的点。
- **Where Picked（拾取处）**：选择实际单击的点。

<a id="v1-s98"></a>

### 选择类型（Selection Type）

**Selection Type** 选项是一个编辑工具，使您能够按对象类型过滤图形查看器中显示的实体。

可以通过在 Selection 选项中选择子选项来应用过滤器，也可以使用 Selection 工具栏上的按钮（见下表）。您可以按需选择多个过滤器。

| 过滤器类型（Filter Type）     | 说明                     |
| ---------------------- | ---------------------- |
| Select with Filter     | 启用过滤选项。                |
| Select All             | 选中图形查看器中与所选过滤器相关的全部对象。 |
| Select Type Part       | 仅选择零件。                 |
| Select Solids/Surfaces | 仅选择实体和/或曲面。            |
| Select Type Resource   | 仅选择资源。                 |
| Select Type Frames     | 仅选择坐标系。                |

| 过滤器类型（Filter Type）           | 说明                      |
| ---------------------------- | ----------------------- |
| Select Type Global Locations | 仅选择全局位置。                |
| Select Type Lines/Curves     | 仅选择直线和/或曲线。             |
| Select Type Mfg              | 仅选择制造特征。                |
| Select Type Notes            | 仅选择注释。                  |
| Select Type Path             | 仅选择路径。                  |
| Select Type PMI              | 仅选择 PMI。                |
| Select Type All              | 选中所有过滤器，即选中图形查看器中的所有实体。 |
| Select Type None             | 取消所有过滤器，即图形查看器中不选中任何实体。 |

1. 选择 **Graphic Viewer Toolbar → Selection** 组 **→ Selection Filter**。
2. 选择所需的一个或多个过滤器，例如 **Selection Type Mfg**。

> **注意**
>
> 之后您可以按需操控所选对象。例如，可以隐藏所选对象，或仅显示所选对象。

要选中图形查看器中的所有对象，请选择 **Select All** 过滤器。

<a id="v1-s99"></a>

### 放置操控器（Placement Manipulator）

> **注意**
>
> 本节配套视频演示了如何使用 Fast placement、Placement manipulator 和 Relocate 命令定位对象。视频不包含在 PDF 中，需通过 HTML 版本访问。

放置操控器（Placement Manipulator）工具使您能够沿 X、Y 或 Z 轴移动对象，并绕 Rx、Ry 或 Rz 轴旋转对象。

**操作步骤**

1. 在图形查看器或对象树中选择一个（或多个）对象。如果您选择的是实体，放置操控器将出现在该实体所属的组件上；如果组件处于建模（Modeling）模式，放置操控器则出现在实体本身上。
   > **注意**
   >
   > 按住 `<Ctrl>` 键在对象查看器中选择所需对象，或在选择模式下于图形查看器中拖出选择框框选所需对象，均可选择多个对象。
2. 选择 **Graphic Viewer Toolbar** 选项卡 **→ Placement Manipulator**。

   Placement Manipulator 对话框随即显示，并在所选对象中心出现带有圆弧的操控器坐标系。

   默认情况下 X 轴处于活动状态，由白色虚线指示被操控对象可移动的方向。视图中其他（未被操控的）对象会被淡化显示。

   **Dimming Mode**（淡化模式）命令使您能够控制使用放置操控器时对象的淡化效果：
   - 切换该命令的开/关可启用或禁用淡化。
   - 单击 Dimming Mode 图标中的箭头并设置 **Color Dimming Mode**，可使用彩色淡化。这是默认模式。
   - 单击 Dimming Mode 图标中的箭头并设置 **Gray Dimming Mode**，可使用灰度淡化。
3. 通过以下任一方式沿 X、Y 或 Z 轴移动所选对象：
   > **提示**
   >
   > 您可以单击 Step size 超链接并指定新的步长来更改步长。
   - 在 **Translate** 区域中选择 X、Y 或 Z，单击前进按钮使对象沿所选轴前进一步，或单击后退按钮使其后退一步。
   - 在 **Translate** 区域中选择 X、Y 或 Z，然后输入新的平移值。
   - 在 **Translate** 区域中选择 X、Y 或 Z，单击相应按钮使对象沿所选轴向前或向后移动直至发生碰撞。
   - 在图形查看器中选中放置操控器的 X、Y 或 Z 轴。当鼠标接触（或接近）目标轴时，该轴会延长并以黄色显示，同时出现白色虚线。此时仅显示放置操控器的活动元素，其他所有元素（原点除外）被隐藏。按住鼠标按钮，将对象拖动到所选轴上的目标位置，或在所选轴旁的输入框中输入平移值。
   松开该轴后，整个放置操控器会在新位置重新完整显示。
4. 通过以下任一方式绕 Rx、Ry 或 Rz 轴旋转所选对象：
   - 在 **Rotate** 区域中选择 Rx、Ry 或 Rz，单击相应按钮使对象绕所选轴顺时针或逆时针旋转一步。
   - 在 **Rotate** 区域中选择 Rx、Ry 或 Rz，然后输入新的旋转值。
   - 在 **Rotate** 区域中选择 Rx、Ry 或 Rz，单击相应按钮使对象绕所选轴顺时针或逆时针旋转直至发生碰撞。请注意，碰撞可能表现为接近未碰（near miss）、接触（contact）或穿透（penetration），具体取决于 Collision 选项卡中的定义。
   - 选中放置操控器的 X、Y 或 Z 圆弧。当鼠标接触（或接近）目标圆弧时，该圆弧以黄色显示。单击圆弧后，仅显示放置操控器的活动元素，其他所有元素（原点除外）被隐藏。按住鼠标按钮拖动即可旋转对象，可以坐标轴原点（当前显示为一个小球）沿代表旋转圆弧的虚线移动作为参考；或在所选圆弧旁的输入框中输入旋转值。
   松开圆弧后，整个放置操控器会重新完整显示。
5. 按以下方式在 XY、XZ 或 YZ 平面内移动所选对象：

   在图形查看器中选中放置操控器的某个平面。当鼠标接触（或接近）目标平面时，平面符号会从三角形变为方形。单击并拖动该平面即可移动组件。拖动过程中仅显示活动平面，放置操控器的其他所有部件（原点除外）被隐藏。

   松开平面后，整个放置操控器会在新位置重新完整显示。
   > **注意**
   >
   > 如果您不希望使用平面手柄，可以在 Graphic Viewer 选项卡中取消勾选 **Display manipulator plane handles** 将其隐藏。
6. 从 **Frame of Reference** 下拉列表中选择对象移动或旋转所围绕的坐标系，选项如下：
   - **Self**：对象的自身原点坐标系。这是默认值。
   - **Geometric center**：位于对象几何中心的参考坐标系。选择多个对象时，该几何坐标系位于包围所有对象的边界框的几何中心。
   - **Working frame**：工程数据中所有对象的参考坐标系。工作坐标系在创建新数据时创建。
   > **注意**
   >
   > 您可以单击 Frame of Reference 按钮旁的下拉箭头，并使用四种可用方法之一指定坐标系位置，从而创建临时的替代参考坐标系。
   >
   > 如果您从列表中选择上述三种坐标系之一，该选择会在下次会话中保留；如果选择列表之外的坐标系，则不会保留，下次会话打开对话框时将使用默认的 Self 坐标系。
7. 默认情况下，**Initial manipulator position** 设置为 **Reference frame**，即打开 Placement Manipulator 对话框时，放置操控器定位于 Frame of Reference 所指示的坐标系处。您可以将其更改为以下之一：
   - 被操控对象的 **Self** 坐标系。
   - 被操控对象的 **Geometric center**（几何中心）。
   - 您在系统查看器中拾取的任意对象或位置。
   - **Working frame**（工作坐标系）。
8. 要测量对象相对于不同坐标系的位置，请单击 **Expand**（展开）。Placement Manipulator 对话框随即展开，表中给出所选对象参考坐标系的精确位置。
9. 从 **Location relative to** 下拉列表中选择一个坐标系，所显示的测量值即相对于该坐标系。
10. 如有需要，勾选 **Snap by step size**，以指定对象的移动或测量仅按步长进行。

    **Manipulated Objects** 表列出了当前由放置操控器操控的所有对象。
    > **注意**
    >
    > 若要折叠已展开的对话框，请单击 **Collapse**。
11. 单击 **Reset** 可将对象恢复到打开 Placement Manipulator 对话框时的位置；单击 **Close** 关闭该对话框。

<a id="v1-s100"></a>

### 重定位（Relocate）

> **注意**
>
> 本节配套视频演示了如何使用 Fast placement、Placement manipulator 和 Relocate 命令定位对象。视频不包含在 PDF 中，需通过 HTML 版本访问。

重定位（Relocate）工具使您能够将对象移动到精确位置。您可以在放置对象时保持其原有姿态，也可以让对象采用目标坐标系的姿态。

如果您选择的是实体，Relocate 将作用于该实体所属的组件；如果组件处于建模模式，Relocate 则作用于实体本身。

> **注意**
>
> 仅在选中对象时才会启用 Relocate 选项。

**操作步骤**

1. 选择 **Graphic Viewer Toolbar** 选项卡 **→ Pick Level** 组 **→ Component**。
2. 在图形查看器或对象查看器的对象树中选择一个（或多个）对象。
   > **注意**
   >
   > 按住 `<Ctrl>` 键在对象查看器中选择所需对象，或在选择模式下于图形查看器中拖出选择框框选所需对象，均可选择多个对象。
3. 选择 **Graphic Viewer Toolbar** 选项卡 **→ Relocate**，显示 Relocate 对话框。所选对象的名称显示在 **Object** 字段中。
4. 从 **From frame** 下拉列表中选择所选对象上的参考坐标系，该坐标系将被定位到目标坐标系（在 **To Frame** 字段中选择）上。可通过以下方式之一选择：
   - 通过六个数值选择坐标系（Selecting Frame by Six Values）
   - 通过圆心选择坐标系（Selecting Frame by Circle Center）
   - 通过三点选择坐标系（Selecting Frame by Three Points）
   - 在两点之间选择坐标系（Selecting Frame Between Two Points）
   如果第一个对象已设置为可建模，则 **From frame** 下拉列表中除 Self、Geometric Center 和 Working Frame 之外，还会包含被重定位的第一个对象的所有坐标系；如果该对象未处于建模状态，列表中仅显示其保留（preserved）坐标系。
5. 在 **To frame**（目标坐标系）下拉列表中，选择要将对象的参考坐标系定位到的目标坐标系，可通过以下方式之一选择：
   - 通过六个数值选择坐标系（Selecting Frame by Six Values）
   - 通过圆心选择坐标系（Selecting Frame by Circle Center）
   - 通过三点选择坐标系（Selecting Frame by Three Points）
   - 在两点之间选择坐标系（Selecting Frame Between Two Points）
   目标坐标系将显示在图形查看器中，并用一条线段连接参考坐标系与目标坐标系。
6. 根据需要，选择下列复选框之一以进一步调整重定位操作：
   - 选择 **Copy Object(s)**（复制对象）以重定位对象的副本，并使所选对象保留在原始位置。
   - 选择 **Maintain orientation**（保持方向）以将所选对象沿参考坐标系到目标坐标系的线性距离移动，而不改变其方向。如果未选中此复选框，对象将采用目标坐标系（在 **To Frame** 字段中选择）的方向。
   - 选择 **Translate only on**（仅沿……平移）以将移动限制在所选的一个或多个坐标轴上。可以选择 X、Y 和 Z，限制移动如下：
     - **X**：对象与目标坐标系的 X 轴位置对齐。
     - **Y**：对象与目标坐标系的 Y 轴位置对齐。
     - **Z**：对象与目标坐标系的 Z 轴位置对齐。
7. 单击 **Apply**（应用）。所选对象按指定方式移动，使所选参考坐标系与目标坐标系对齐。
8. 通过以下方式之一继续操作：
   - 单击 **Reset**（重置）将重定位后的对象恢复到原始位置。
   - 单击 **Flip**（翻转）翻转重定位后的对象并反转其 Z 轴方向。
   - 单击 **Close**（关闭）关闭 Relocate 对话框。

> **注意**
>
> Self 坐标系是默认参考坐标系，在 Relocate 对话框打开时显示在所选对象上。如果从列表中选择三个坐标系中的任意一个，它将在下一次会话中保留；如果选择列表中不存在的坐标系，则不会被保留，下次会话打开对话框时将使用默认的 Self 坐标系。

<a id="v1-s101"></a>

#### Select a frame by six values（通过六个数值选择坐标系）

通过六个数值选择坐标系，是指为 Relocate 工具的参考坐标系或目标坐标系指定精确位置，方式为指定 X、Y、Z 轴以及旋转的 X、Y、Z 轴（即 Rx、Ry、Rz）。

**操作步骤**

1. 在 Relocate 对话框中，单击 **From frame** 或 **To frame** 字段右侧的 **Create Frame of Reference**（创建参考坐标系）下拉箭头。

   显示以下菜单。
2. 选择 **Frame by 6 values**（通过 6 个数值选择坐标系）。

   **From frame** 或 **To frame** 字段中的按钮显示如下，并弹出 Location 对话框。
3. 在 X、Y、Z、Rx、Ry 和 Rz 字段中指定坐标系的位置与方向。

   坐标系的位置在图形查看器中实时反映。
   > **注意**
   >
   > 如有需要，可单击相应按钮将坐标系沿其 Z 轴翻转到相反方向。
4. 单击 **OK**（确定）。所指定的坐标系即成为 Relocate 操作的所选参考坐标系（From frame）或目标坐标系（To frame）。

<a id="v1-s102"></a>

#### Select a frame by circle center（通过圆心选择坐标系）

通过圆心选择坐标系，是指为 Relocate 工具的参考坐标系或目标坐标系指定精确位置，方式为指定圆周上的任意三个点，圆心将自动计算。当需要将圆形组件（例如将圆锥形状）重定位到圆柱形形状顶部时，此功能非常有用。

**操作步骤**

1. 在 Relocate 对话框中，单击 **From frame** 或 **To frame** 字段右侧的 **Create Frame of Reference** 下拉箭头。

   显示以下菜单。
2. 选择 **Frame by circle center**（通过圆心选择坐标系）。

   **From frame** 或 **To frame** 字段中的按钮显示如下，并弹出 **Circle Center by 3 Points**（通过 3 点确定圆心）对话框。
3. 通过在图形查看器中选择点，或在 **Circle Center by 3 Points** 对话框中为每个点指定 X、Y、Z 轴位置，来指定圆周上的三个点。圆心被自动定义。坐标系的位置在图形查看器中实时反映。坐标系的方向为：Z 轴垂直于由三点所确定的平面，坐标系的 X 轴指向第一个点的方向。
   > **注意**
   >
   > 如有需要，可单击相应按钮将坐标系沿其 Z 轴翻转到相反方向。
4. 单击 **OK**（确定）。所指定的坐标系即成为 Relocate 操作的所选参考坐标系（From frame）或目标坐标系（To frame）。

<a id="v1-s103"></a>

#### Select a frame by three points（通过三点选择坐标系）

通过三点选择坐标系，是指为 Relocate 工具的参考坐标系或目标坐标系指定精确位置，方式为指定任意三个点。当需要在平面上重定位组件时，此功能非常有用。

**操作步骤**

1. 在 Relocate 对话框中，单击 **From frame** 或 **To frame** 字段右侧的 **Create Frame of Reference** 下拉箭头。

   显示以下菜单。
2. 选择 **Frame by 3 points**（通过 3 点选择坐标系）。

   **From frame** 或 **To frame** 字段中的按钮显示如下，并弹出 **Frame by 3 Points**（通过 3 点确定坐标系）对话框。
3. 通过在图形查看器中选择三个点，或在 **Frame by 3 Points** 对话框中指定三个点的 X、Y、Z 坐标来定义一个平面。第一个点确定坐标系的原点，第二个点确定 X 轴位置，第三个点确定 Z 轴位置。坐标系的位置在图形查看器中实时反映。
   > **注意**
   >
   > 如有需要，可单击相应按钮将坐标系沿其 Z 轴翻转到相反方向。
4. 单击 **OK**（确定）。所指定的坐标系即成为 Relocate 操作的所选参考坐标系（From frame）或目标坐标系（To frame）。

<a id="v1-s104"></a>

#### Select a frame between two points（在两点之间选择坐标系）

在两点之间选择坐标系，是指为 Relocate 工具的参考坐标系或目标坐标系指定精确位置，方式为指定两个特定点之间的距离。当需要在两点之间重定位组件时，此功能非常有用。

**操作步骤**

1. 在 Relocate 对话框中，单击 **From frame** 或 **To frame** 字段右侧的 **Create Frame of Reference** 下拉箭头。

   显示以下菜单。
2. 选择 **Frame Between 2 Points**（在两点之间选择坐标系）。

   **From frame** 或 **To frame** 字段中的按钮显示如下，并弹出 **Frame Between Two Points**（两点之间坐标系）对话框。
3. 通过在图形查看器中选择两个点，或在 **Frame Between Two Points** 对话框中指定两个点的坐标来定义一段线段。
4. 通过以下方式之一，定义在所指定的两个点之间创建坐标系的距离：
   - 拖动滑块。
   - 在文本框中手动输入数值。
   - 使用上、下箭头指定所需距离。
   > **注意**
   >
   > 默认情况下，距离位于两个指定点的中点。
   坐标系的位置在图形查看器中实时反映。
   > **注意**
   >
   > 如有需要，可单击相应按钮将坐标系沿其 Z 轴翻转到相反方向。
5. 单击 **OK**（确定）。所指定的坐标系即成为 Relocate 操作的所选参考坐标系（From frame）或目标坐标系（To frame）。

<a id="v1-s105"></a>

### Location Manipulator（位置操控器）

Location Manipulator（位置操控器）选项是一个路径编辑工具，可用于调整经由点（via location）的位置。当希望同时调整多个位置（即使它们类型不同）时，此功能非常有用。

如果在选择单个位置后启动 Location Manipulator，系统将以单位置模式启动 Location Manipulator，参见下文操作步骤。

如果在选择两个或更多位置后启动 Location Manipulator，系统将以多位置模式启动 Location Manipulator，参见下文操作步骤。

> **注意**
>
> 在 Line Simulation（产线仿真）中，仅使用多位置模式（即使选择的是单个位置）。
>
> 在 Line Simulation 中，以下位置锁定被停用：
>
> - 根据选项限制位置操作（Limit locations manipulation according to options）
> - 操作至最大允许限制（Manipulate to maximum allowed limitation）
> - 位置操作——重置绝对位置（Actions on locations - Reset absolute location）
> - 位置操作——对齐至最大允许值（Actions on locations - Snap to maximum allowed）
> - 在图形中显示位置限制（Show location limitations in graphics）

**操作单个位置：**

**操作步骤**

1. 在图形查看器或对象查看器中，选择单个位置，并选择 **Graphic Viewer Toolbar**（图形查看器工具栏）选项卡 **→ Location Manipulator**。

   出现 Location Manipulation（位置操作）对话框。

   **Location** 字段显示所选位置的名称。
2. 使用相应按钮在当前操作中的各个位置之间导航。
3. 将 **Frame of reference**（参考坐标系）设置为以下之一：
   - **Relative to its own self frame**（相对于其自身坐标系）——位置相对于其自身的 self 坐标系进行操作。这是默认设置。
   - **Relative to its original projection**（相对于其原始投影）——位置相对于其原始投影进行操作。此选项仅对缝焊位置（seam location）和焊接位置（weld location）操作启用，当位置操作的方向已被修改且希望将所有位置移动相同距离时非常有用。
   - **Relative to**（相对于）——位置相对于单个坐标系进行操作。默认使用工作坐标系（working frame）作为参考坐标系，但可通过选择任何其他坐标系进行覆盖。如果选择某个对象，则使用该对象的 self 坐标系。
4. （可选）单击 **Step Size**（步长）链接，以设置操作位置时的步长。
5. 默认已选中 **Rotation**（旋转）。单击 **Rx**、**Ry** 或 **Rz** 选择执行旋转的轴。

   或者使用 **Select Axis**（选择轴）按钮选择 **Perpendicular**（垂直）、**Movement**（运动）或 **Third axis**（第三轴）作为旋转轴。
   > **注意**
   >
   > 对于缝焊位置，请选择 **Normal**（法向）、**Movement**（运动）或 **Third axis**（第三轴）作为旋转轴。
6. 移动滑块、设置数值，或单击箭头以执行所选位置的旋转。
7. 如果希望移动位置，选择 **Translation**（平移）。
8. 单击 **X**、**Y** 或 **Z** 选择移动位置的方向。
   > **注意**
   >
   > 如果在执行平移时单击 **Select Axis** 按钮，则仅 **Normal**（法向）选项可用。
9. 移动滑块、设置数值，或单击箭头以移动所选位置。
   > **注意**
   >
   > 对于无限位置（例如平移经由点时），将滑块移动到其范围末端会将滑块重置到中间位置，但会修改其范围。
10. 还可以对位置执行以下操作：
    - **Reset Absolute Location**（重置绝对位置）——将所选位置的绝对位置重置为投影时的旋转和平移值。
    - **Snap to Maximum Allowed**（对齐至最大允许值）——如果位置已超过其最大值，此功能将超出值设置为最大允许值。
11. 如果希望图形查看器显示表示位置允许偏差限制的锥形图标，请单击 **Show Location Limits in Graphic Viewer**（在图形查看器中显示位置限制）。
12. 如果对所做的更改不满意，单击 **Reset**（重置）将所有所选位置恢复到启动多位置操作之前的状态。
13. 单击 **Follow mode**（跟随模式）将机器人放置到所选位置（即操控器所在位置）。这将改变机器人的姿态，并为位置增加进一步限制。位置的运动受机器人运动的限制。如果机器人无法到达该位置，将创建一个虚拟焊枪（ghost gun）并放置在该位置，且不再有进一步限制。

**操作多个位置：**

**操作步骤**

1. 选择两个或更多位置，并选择 **Operation** 选项卡 **→ Edit Path** 组 **→ Location Manipulator**。

   出现 Multiple Locations Manipulation（多位置操作）对话框。
   > **注意**
   >
   > 如果在启动命令时未选择任何位置，也会显示 Multiple Locations Manipulation 对话框。
2. 如果在启动命令前已预选位置，它们将列在 **Locations**（位置）列表中。如果未预选，请单击 **Locations** 列表并从某个查看器中选择位置。在 **Locations** 列表中单击某个位置时，图形查看器将显示该位置的操控器。
3. 在 **Translate**（平移）区域中，选择希望平移位置的轴并输入平移值。

   或者单击 **Apply axis**（应用轴）并根据其角色和位置选择轴，系统会自动选择正确的轴（X、Y 或 Z）。

   所输入的值将加到每个位置的当前值上，系统为每个位置计算新的位置。也可以单击平移值旁边的箭头使其递增或递减。如果步长不合适，单击 **Step size**（步长）进行调整。
   > **注意**
   >
   > 也可以使用图形查看器中的操控器执行平移。
4. 在 **Rotate**（旋转）区域中，选择希望旋转位置的轴并输入旋转值。

   或者单击 **Apply axis** 并根据其角色和位置选择轴，系统会自动选择正确的轴（Rx、Ry 或 Rz）。

   所输入的值将加到每个位置的当前值上，系统为每个位置计算新的位置。也可以单击旋转值旁边的箭头使其递增或递减。如果步长不合适，单击 **Step size** 进行调整。
   > **注意**
   >
   > 也可以使用图形查看器中的操控器执行旋转。
5. 将 **Frame of Reference**（参考坐标系）设置为以下之一：
   - **Each location relative to its own self frame**（每个位置相对于其自身的 self 坐标系）——每个位置相对于其自身的 self 坐标系进行操作。这是默认设置。
   - **Each location relative to its original projection**（每个位置相对于其原始投影）——所有缝焊位置和焊接位置操作各自相对于其原始投影进行操作。当位置操作的方向已被修改且希望将所有位置移动相同距离时非常有用。所有没有投影位置的操作将各自相对于其 self 坐标系进行操作。
   - **All locations relative to**（所有位置相对于）——所有所选位置相对于单个坐标系进行操作。默认使用工作坐标系作为参考坐标系，但可通过选择任何其他坐标系进行覆盖。如果选择某个对象，则使用该对象的 self 坐标系。
6. 默认已选中 **Limit locations manipulation according to options**（根据选项限制位置操作）。如果希望忽略系统限制，请清除此复选框。
7. 如果选中了 **Limit locations manipulation according to options**，则 **Manipulate to maximum allowed limitation**（操作至最大允许限制）被激活。选中此选项将指示系统将位置移动到最大允许位置而不超出系统限制。如果清除该复选框，超出系统限制的位置将保留在其原始位置。

   某些新位置的位置和方向可能无效。Multiple Locations Manipulation 对话框中 **Locations** 列表的 **Status**（状态）列针对每个位置显示以下状态之一：
   - 绿色图标——系统已按指令移动了该位置。
   - 红色图标——系统未移动该位置，它保留在原始位置。这仅在选中 **Limit locations manipulation according to options** 时发生。
   - 黄色图标——系统已尽可能移动该位置，但系统限制阻止了指令的完全执行。这仅在选中 **Manipulate to maximum allowed limitation** 时发生。
8. 还可以对位置执行以下操作：
   - **Reset Absolute Location**（重置绝对位置）——将所选位置的绝对位置重置为投影时的旋转和平移值。
   - **Snap to Maximum Allowed**（对齐至最大允许值）——如果位置已超过其最大值，此功能将超出值设置为最大允许值。
9. 如果希望图形查看器显示表示位置允许偏差限制的锥形图标，请单击 **Show Location Limits in Graphic Viewer**（在图形查看器中显示位置限制）。
10. 如果对所做的更改不满意，单击 **Reset**（重置）将所有所选位置恢复到启动多位置操作之前的状态。
11. 单击 **Close**（关闭）应用更改并关闭对话框。
12. 单击 **Follow mode**（跟随模式）将机器人放置到所选位置（即操控器所在位置）。这将改变机器人的姿态，并为位置增加进一步限制。位置的运动受机器人运动的限制。如果机器人无法到达该位置，将创建一个虚拟焊枪（ghost gun）并放置在该位置，且不再有进一步限制。
    > **注意**
    >
    > 当所选位置进入机器人可达范围时，虚拟焊枪消失，机器人跳转到所选位置。位置的运动受机器人运动的限制。

<a id="v1-s106"></a>

### Measurements（测量）

<a id="v1-s107"></a>

#### Measurements（测量工具）

Measurements（测量）选项包含便于测量组件之间距离的对话框——对话框将显示测量结果。此外，Dimensions（尺寸标注）命令可用于在图形查看器中创建持久的尺寸标注。它包含以下工具：

| 工具                        | 说明                                                                                     |
| ------------------------- | -------------------------------------------------------------------------------------- |
| Minimal Distance（最小距离）    | 打开 Minimal Distance 对话框，用于测量两个组件之间的最小距离。参见 Measure Minimal Distance。                   |
| PTP Distance（点对点距离）       | 打开 Point To Point Distance 对话框，用于测量两个组件上指定点之间的距离。参见 Measure Point-to-Point Distance。   |
| Linear Distance（线性距离）     | 打开 Linear Distance 对话框，用于测量两个平行面或平行边之间的线性距离。参见 Linear Distance。                        |
| Angular Distance（角度距离）    | 打开 Angular Distance 对话框，用于测量两个相交面或边之间的角度。参见 Angular Measurement。                       |
| Curve Length（曲线长度）        | 打开 Curve Length 对话框，用于测量曲线长度。参见 Curve Length。                                          |
| Angle by 3 Points（三点角度）   | 打开 Angle by 3 Points 对话框，用于通过指定中心点及另外两个点来测量两个向量之间的角度。参见 Measure Angle by Three Points。 |
| Change Color（更改颜色）        | Change Color 图标包含在上述所有对话框中，便于设置在更改颜色后所创建的测量线的颜色。                                       |
| Copy to Clipboard（复制到剪贴板） | Copy to Clipboard 图标包含在上述所有对话框中，便于将任意测量对话框中的所有测量结果复制到剪贴板并粘贴到任意位置。                      |

> **注意**
>
> 可以在 **Appearance**（外观）选项卡中更改尺寸标注和测量文本的颜色与大小。

<a id="v1-s108"></a>

#### Measure minimal distance（测量最小距离）

Minimal Distance（最小距离）工具可用于测量图形查看器中两个组件之间的最小（或最短）距离。

> **注意**
>
> - 在 New Section Viewer（新剖切查看器）中工作时，请使用 Minimal Distance 选项。
> - 最小距离可在所有拾取级别（Component、Entity、Face 或 Edge）的对象之间进行测量。

**操作步骤**

1. 选择 **Graphic Viewer Toolbar**（图形查看器工具栏）**→ Measurements** 组 **→ Minimal Distance**。

   显示 Minimal Distance 对话框。
2. 在图形查看器或对象树中选择第一个对象。所选对象的名称显示在 **First Object**（第一个对象）字段中，对象的坐标显示在下方。
3. 在图形查看器或对象查看器中选择第二个对象。所选对象的名称显示在 **Second Object**（第二个对象）字段中，对象的坐标显示在下方。
4. 单击 **Create Dimension**（创建尺寸标注）。

   连接两个对象的距离线出现在图形查看器中。两个对象之间的精确距离被自动计算并显示在 **Distance**（距离）字段中，矢量距离显示在下方。对话框的 **Result**（结果）区域显示矢量距离（dX 为第二个对象的 X 值减去第一个对象的 X 值，dY 为第二个对象的 Y 值减去第一个对象的 Y 值，dZ 为第二个对象的 Z 值减去第一个对象的 Z 值）。如果在图形查看器中选中了 **Show XYZ delta**（显示 XYZ 增量），还会显示增量距离线（dX 为红色、dY 为绿色、dZ 为黄色）。
   > **注意**
   >
   > 也可以先选择第一个对象，再选择 Minimal Distance 以显示 Minimal Distance 对话框，所选对象显示在 **First Object** 字段中。单击 **Second Object** 字段并在图形查看器或对象查看器中选择第二个对象。
   >
   > 默认情况下，系统不会测量到两个为最小距离测量所选对象之间介入的坐标系和点。要测量到或从某个坐标系/点测量，请在 Minimal Distance 对话框中将其选为第一个或第二个对象。坐标系和点不会包含在组件几何中，除非你专门为最小距离计算选择了这些实体。
5. 单击 **Close**（关闭）关闭 Minimal Distance 对话框。

<a id="v1-s109"></a>

#### Measure point to point distance（测量点对点距离）

Point to Point Distance（点对点距离）工具可用于测量工程数据中两个对象上所选点之间的精确距离。这些点可以在同一对象上、不同对象上，或位于任意位置。

**操作步骤**

1. 选择 **Home** 选项卡 **→ Pick** 组 **→ Component**。
2. 选择 **Graphic Viewer Toolbar** **→ Measurements** 组 **→ Point to Point Distance**。

   显示 Point to Point Distance 对话框。
3. 在图形查看器或对象树中单击第一个对象上的一个点。

   该点所在对象的名称及其精确位置显示在 **First Object** 字段中。
4. 如果需要，可通过在单击某个坐标时显示的向上/向下箭头调整 X、Y、Z 坐标，来微调该点的位置。
   > **注意**
   >
   > 如果测量的不是实体或组件之间的点对点距离，建议先在所需位置创建一个坐标系，以便测量点对点距离。
5. 在图形查看器中单击第二个对象上的一个点。

   该点所在对象的名称及其精确位置显示在 **Second Object** 字段中。
6. 单击 **Create Dimension**（创建尺寸标注）。

   连接两点的点对点尺寸标注出现在图形查看器中。两点之间的精确距离被自动计算并显示在 **Distance** 字段中，矢量距离显示在下方。对话框的 **Result** 区域显示矢量距离（dX 为第二个对象的 X 值减去第一个对象的 X 值，dY 为第二个对象的 Y 值减去第一个对象的 Y 值，dZ 为第二个对象的 Z 值减去第一个对象的 Z 值）。**Result** 区域还会显示 X、Y、Z 各轴的旋转增量差值。如果在图形查看器中选中了 **Show XYZ delta**，还会显示增量距离线（dX 为红色、dY 为绿色、dZ 为黄色）。
7. 单击 **Close**（关闭）关闭 Point to Point Distance 对话框。

<a id="v1-s110"></a>

#### Measure linear distance（测量线性距离）

Linear Distance（线性距离）工具可用于测量工程数据中两个平行面或平行边，或一个面与一条边之间的正交距离。该命令仅支持平面（planar face）和线性边（linear edge）。

**操作步骤**

1. 选择 **Graphic Viewer Toolbar** **→ Measurements** 组 **→ Linear Distance**。

   显示 Linear Distance 对话框。
2. 在图形查看器中选择第一个对象。

   所选对象的名称显示在 **First object**（第一个对象）字段中。
3. 在图形查看器中选择第二个对象。

   所选对象的名称显示在 **Second object**（第二个对象）字段中。
4. 单击 **Create Dimension**（创建尺寸标注）。

   两个对象之间的精确距离被自动计算并显示在 **Distance** 字段中。
5. 单击 **Close**（关闭）关闭 Linear Distance 对话框。

<a id="v1-s111"></a>

#### Angular measurement（角度测量）

Angular Measurement（角度测量）工具可用于测量工程数据中位于相交平面/直线上的两个相交面或边，或一个面与一条边之间的角度。对象本身可能并不相交。该命令仅支持平面和线性边。

**操作步骤**

1. 选择 **Graphic Viewer Toolbar** **→ Measurements** 组 **→ Angular Measurement**。

   显示 Angular Measurement 对话框。
2. 在图形查看器中选择第一个对象。

   所选对象的名称显示在 **First object** 字段中。
3. 在图形查看器中选择第二个对象。

   所选对象的名称显示在 **Second object** 字段中。
4. 单击 **Create Dimension**（创建尺寸标注）。

   两个对象之间的角度被自动计算并显示在 **Angle**（角度）字段中。
5. 单击 **Close**（关闭）关闭 Angular Measurement 对话框。

<a id="v1-s112"></a>

#### Curve length measurement（曲线长度测量）

Curve Length Measurement（曲线长度测量）工具可用于测量图形查看器中显示的曲线长度。

**操作步骤**

1. 选择 **Graphic Viewer Toolbar** **→ Measurements** 组 **→ Curve Length Dimension**。

   显示 Curve Length 对话框。
2. 在图形查看器或对象树中选择曲线对象。

   所选对象的名称显示在 **Curve object**（曲线对象）字段中，其长度被自动计算并显示在 **Length**（长度）字段和图形查看器中。

   也可以在选择命令之前预先选择一条曲线。
3. 如果希望将所显示的测量结果转换为对象树中的尺寸标注对象，请单击 **Create Dimension**（创建尺寸标注）。
4. 还可以：
   - 单击相应按钮并选择一种颜色，以在图形查看器中显示该测量。如果已将该测量转换为尺寸标注对象，则此操作无效。
   - 单击相应按钮将测量复制到剪贴板。
   - 单击另一条曲线以进行另一次测量。
5. 单击 **Close**（关闭）关闭 Curve Length 对话框。
   > **注意**
   >
   > 可以使用 Curve Length Dimension 命令测量不自相交的机器人 TCP 轨迹（trajectory）。

<a id="v1-s113"></a>

#### Measure an angle by three points（通过三点测量角度）

Angle by 3 Points（三点角度）工具可用于测量由三个点构成的角度，其中所选三个点之一被指定为中心点。

所有三个点可以在同一对象上、不同对象上，或位于任意位置。可以使用此工具来帮助规划工厂中工作站的布局。

**操作步骤**

1. 选择 **Home** 选项卡 **→ Pick** 组 **→ Component**。
2. 选择 **Graphic Viewer Toolbar** **→ Measurements** 组 **→ Angle by 3 Points**。

   显示 Angle by 3 Points 对话框。
3. 在图形查看器中单击希望指定为中心点的一个点。

   该点所在对象的名称显示在 **Center**（中心）字段中，该点的精确坐标显示在下方。
4. 如果需要，可通过向上/向下箭头调整 X、Y、Z 坐标来微调该点的位置。
5. 在图形查看器中单击希望指定为从中心点出发的第一条射线的第二个点。

   该点所在对象的名称显示在 **Ray #1**（射线 1）字段中，该点的精确坐标显示在下方。中心点与第二点之间绘制了一条线。
6. 如果需要，可通过向上/向下箭头调整 X、Y、Z 坐标来微调第二个点的位置。
7. 在图形查看器中单击希望指定为从中心点出发的第二条射线的第三个点。

   该点所在对象的名称显示在 **Ray #2**（射线 2）字段中，该点的精确坐标显示在下方。中心点与第三点之间绘制了一条线。
8. 如果需要，可通过向上/向下箭头调整 X、Y、Z 坐标来微调第三个点的位置。

   第一条射线与第二射线之间角度的度数被自动计算并显示在对话框底部。
9. 单击 **Close**（关闭）关闭 Angle by 3 Points 对话框。

<a id="v1-s114"></a>

## Modify Color（修改颜色）

Modify Color（修改颜色）选项可用于更改图形查看器中所选对象的颜色，以满足个性化需求。通过选择多个对象，可以同时更改多个对象的颜色。

- 在图形查看器或对象查看器中选择对象，并选择 **Graphic Viewer Toolbar** **→ Modify Color**。

  显示 Modify Color 对话框。
- 从调色板中选择一种颜色。

  图形查看器中所选对象变为所选颜色。
- 如果需要使用自定义颜色，请单击 **Other**（其他）并指定所需颜色。

  所指定的颜色显示在调色板的“最近使用的颜色”（Recent Colors）中。
- 使用 **Select**（选择）工具在图形查看器中选择一种颜色。

  该工具选择对象的真实颜色，并忽略用于创建特殊效果（例如用于产生纵深感的明暗 shading）的其他颜色。
  > **注意**
  >
  > - 要更改对象、事件和操作的默认颜色，请选择 **File** 选项卡 **→ Options**，并单击 **Color** 选项卡。
  > - 在 PMI 上运行 Modify Color 时，系统仅修改其前景色。
  > - `ColorDef.xml` 文件（位于安装文件夹中）支持使用颜色代码或颜色名称（定义参见 <http://www.w3schools.com）添加颜色。>
  >
  > 按颜色名称定义颜色的示例：
  >
  > ```xml
  > <ColorDef>
  >   <ColorPalette>
  >     <UserCustom />
  >     <Predefined>
  >       <Color value="AliceBlue" />
  >       <Color value="AntiqueWhite" />
  >       <Color value="Aqua" />
  >       ...
  > ```
  >
  > 如果 `colorDef.xml` 文件存在错误，将显示默认调色板。

<a id="v1-s115"></a>

## Default Keyboard Shortcuts（默认键盘快捷键）

下表列出了 Process Designer 中可用的常用快捷键。请勿将这些按键分配给其他功能。

| 快捷键       | 操作                           |
| --------- | ---------------------------- |
| Ctrl+A    | 全选（Select all）               |
| Ctrl+C    | 复制（Copy）                     |
| Ctrl+N    | 新建（New）                      |
| Ctrl+O    | 打开（Open）                     |
| Ctrl+S    | 保存（Save）                     |
| Ctrl+V    | 粘贴（Paste）                    |
| Ctrl+Z    | 撤销（Undo）                     |
| Ctrl+Y    | 重做（Redo）                     |
| F1        | 目录（Contents）                 |
| Alt+F4    | 退出（Exit）                     |
| F6        | 选项（Options）                  |
| F10       | 线框模式（Wireframe mode）         |
| F11       | 拾取意图（Pick intent）            |
| F12       | 拾取级别（Pick level）             |
| Delete    | 删除（Delete）                   |
| Home      | 机器人回零位（Robot home）           |
| Alt+P     | 放置操控器（Placement manipulator） |
| Alt+Z     | 缩放至适配（Zoom to fit）           |
| Num+      | 展开一级（Expand 1 level）         |
| Num*      | 展开全部（Expand all）             |
| Alt+L     | 加载（Load）                     |
| Alt+Enter | 属性（Properties）               |
| Alt+B     | 空白（Blank）                    |
| Alt+D     | 显示（Display）                  |

<a id="v1-s116"></a>

## Status Bar（状态栏）

状态栏显示在 Process Designer 应用程序窗口的底部。状态栏菜单可用于自定义显示哪些信息、隐藏哪些信息。

**配置状态栏：**

**操作步骤**

1. 右键单击状态栏。

   出现 Status Bar Configuration（状态栏配置）菜单。
2. 勾选希望在状态栏上显示的项，清除希望隐藏的项。

   某些项（如 Check In/Out State 和 Variant Filter）只有在有信息可显示时才会显示在状态栏上。例如，如果未应用任何筛选器，即使启用了 Variant Filter 项，它也不会显示在状态栏上。
   > **注意**
   >
   > 系统将状态栏配置存储在当前布局中，参见 Layout Manager（布局管理器）。

<a id="v1-s117"></a>

## Status Reports（状态报告）

当执行任何无效操作时（例如删除已检入的对象），系统会显示 Status Report（状态报告）对话框。

可以执行以下操作：

- 右键单击说明以打开一个列出可能解决方案的菜单。
- 单击 **Copy**（复制）将状态报告信息复制到剪贴板。
- 如果删除了带有嵌套子对象的节点，Status Report 对话框会创建一份包含每个已检入节点错误消息的列表。这样你可以修复所有问题并重复删除操作。
- 如果尝试删除节点且 Status Report 对话框仅显示了已检入节点的错误消息（而无其他消息），系统会自动检出所有节点并执行删除操作。

<a id="v1-s118"></a>

## Auto Save（自动保存）

Process Designer 会定期自动提示保存工作。

单击 **Yes**（是）在本地保存工作。

有关如何配置 Auto Save 功能频率的信息，请参阅 Options（选项）对话框的 General（常规）选项卡。

<a id="v1-s119"></a>

## Object toolbar（对象工具栏）

Object toolbar（对象工具栏）在选择对象时显示。它是上下文相关的，并随所选择对象的类型而变化，显示相关命令的图标。当鼠标移开时，Object toolbar 会淡出，进一步移开后工具栏不再出现，直到重新选择该对象。

> **注意**
>
> 默认显示 Object toolbar。可在 Graphic Viewer（图形查看器）选项卡中取消勾选 **Display contextual toolbar for selected object**（显示所选对象的上下文工具栏）复选框使其保持隐藏。

<a id="v1-s120"></a>

## Customizing the mouse（自定义鼠标）

Tecnomatix 应用程序中的默认鼠标行为类似于 NX 应用程序。要显示默认鼠标功能的图形表示，请右键单击功能区（ribbon）并选择 **Customize the Ribbon**（自定义功能区）以打开 Customize（自定义）对话框。选择 **Customize Mouse**（自定义鼠标）选项卡以选择三种配置之一：

- **Default**（默认）：提供常用鼠标行为，类似于 NX 应用程序。这是一种固定配置，所有参数均为只读。
- **Legacy**（传统）：提供类似于早期 Tecnomatix 应用程序版本的鼠标行为。这是一种固定配置，所有参数均为只读。
- **Custom**（自定义）：允许根据需求自定义每个按钮（起点为 Default 自定义）。你的自定义设置在对所有后续工作会话保持有效，直到你修改它们。在迁移到下一软件版本后，你也可以快速应用相同的自定义。

你可以自定义鼠标行为以适应工作习惯。每个鼠标按钮都可以自定义为在单击或按住并拖动鼠标时执行某个操作。鼠标按钮与滚轮的各种组合，连同 Shift、Alt 和 Control 按钮，为在图形查看器中定义常用操作的快捷键提供了灵活性。

<a id="v1-s121"></a>

### To customize mouse buttons（自定义鼠标按钮）

> **注意**
>
> 某些按钮组合无法自定义，因为它们被保留用于特定操作。例如，单击 MB3（鼠标右键）被保留用于打开上下文菜单，按住 MB1 拖动被保留用于图形查看器中的框选。

**操作步骤**

1. 右键单击功能区并选择 **Customize the Ribbon** 以打开上图所示的 Customize 对话框。
2. 单击 **Customize Mouse** 选项卡。
3. 如果选择了 **Custom**，可以在对话框中配置以下参数组：
   > **注意**
   >
   > **Mouse Map**（鼠标映射）：Customize 对话框右侧的该图定义了鼠标按钮名称。以下参数与这些按钮名称相关。
   >
   > - **Mouse drag**（鼠标拖动）：配置在拖动鼠标并按下键盘控制按钮时执行的操作。例如，可能希望使用 Shift + MB2 平移显示，使用 Alt + MB2 旋转显示。
   > - **Mouse wheel**（鼠标滚轮）：配置在滚动鼠标滚轮时按下键盘控制按钮所执行的操作。例如，如果习惯于其他程序中的相同设置，可能希望使用 Ctrl + 向上滚动以放大显示。
   > - **Mouse click**（鼠标单击）：配置单击鼠标中键（滚轮）时执行的操作。例如 View Center（视图中心，参见 View Center）。
   > - **Mouse + button drag**（鼠标 + 按钮拖动）：配置在单击各种鼠标按钮并按下键盘控制按钮的同时拖动鼠标时所执行的操作。
   以下示例显示了配置 Shift + MB1 的选项：
   - **Drag direction**（拖动方向）
     - **Zoom**（缩放）：设置拖动鼠标以缩放显示的方向。默认通过垂直拖动鼠标进行缩放。将 Mouse configuration 设置为 Legacy 可通过水平拖动鼠标进行缩放；如果选择 Custom，则可自行设置偏好。
     - **Flip rotation**（反转旋转）：更改在场景中拖动鼠标时对象旋转的默认方向。可将该选项设置为反转水平旋转、垂直旋转或两者的旋转方向。可针对 Walk around object（绕对象行走，Tecnomatix 方法）和 Rotate object（旋转对象，Vis 方法）两种旋转方法进行配置。
   > **注意**
   >
   > 此视频演示如何自定义鼠标中键。
   >
   > 视频未包含在 PDF 中。要访问视频，请使用 HTML。

<a id="v1-s122"></a>

### To implement your mouse customization after upgrading to a new software version（升级到新软件版本后应用鼠标自定义）

**操作步骤**

1. 关闭 Process Designer。
2. 找到存储在当前版本文件夹中的 `RibbonMouseConfiguration.xml` 文件，该文件位于用户配置文件的 GeneralConfiguration 嵌套目录下，例如：

   `C:\Users\JohnDoe\AppData\Local\Tecnomatix\GeneralConfiguration\13.0\RibbonMouseConfiguration.xml`
3. 复制该文件并覆盖新版本文件夹中的等效文件，例如：

   `C:\Users\JohnDoe\AppData\Local\Tecnomatix\GeneralConfiguration\13.0.1\`
4. 启动 Process Designer 并验证鼠标自定义是否已保留。

<a id="v1-s123"></a>

## Displaying and Docking Viewers（显示与停靠查看器）

通常可通过选择 **Home** 选项卡 **→ Viewers** 组 **→ Viewers** 并从列表中选择来显示查看器。未列出的查看器（如 Properties、Gantt、PERT、Table View）可通过 **Open With**（打开方式）命令访问。

查看器显示后，可以为浮动状态、隐藏状态（查看器名称显示在应用程序窗口底部或左侧/右侧的状态栏中），也可以停靠（dock）它们，即将其附着到应用程序窗口的边缘或另一个查看器。要取消停靠查看器（恢复为浮动），请双击其标题栏。

从 Home 选项卡选择查看器后，可以通过以下几种方式定位查看器：

- **Dockable**（可停靠）——查看器出现在应用程序窗口或另一个查看器的边缘。
- **Hide**（隐藏）——查看器不显示。你可以将应用程序窗口中的空间用于当前任务所需的查看器。
- **Floating**（浮动）——查看器的窗口显示在其他窗口之上。
- **Auto Hide**（自动隐藏）——查看器可用，但仅在需要时显示。

**停靠查看器：**

**操作步骤**

1. 单击查看器标题栏中的相应按钮，选择 **Docking**（停靠）并将查看器拖向所需位置。或者，如果查看器处于浮动状态，单击其标题栏并将其拖向所需位置。系统会高亮显示所需位置，并显示多停靠图标以及其他停靠图标。
2. 拖动查看器，直到鼠标指针触及以下图标之一：
   - 堆叠图标。查看器与所选位置的其他查看器堆叠在一起。单击选项卡可查看其中一个堆叠的查看器。

**隐藏查看器：**

- 单击查看器标题栏中的相应按钮并选择 **Hide**（隐藏）。

**显示隐藏的查看器：**

- 选择 **View** 选项卡 **→ Viewers** 组，并选择希望显示的查看器。

**显示为浮动查看器：**

- 单击查看器标题栏中的相应按钮，选择 **Floating**（浮动）并将查看器拖到所需位置。或者，如果查看器已停靠，单击其标题栏并将其拖到所需位置。

> **注意**
>
> 浮动查看器没有停靠或自动隐藏图标。

**自动隐藏查看器：**

- 单击查看器标题栏中的相应按钮，选择 **AutoHide**（自动隐藏）。或者单击查看器标题栏中的 **Docked**（已停靠）图标。查看器名称出现在应用程序窗口底部或侧面的状态栏中。将光标移开查看器会隐藏查看器；将光标置于状态栏中的查看器名称上会显示查看器。Auto Hide 图标出现在查看器标题栏中。

**禁用自动隐藏：**

- 单击查看器标题栏中的相应按钮。

<a id="v1-s124"></a>

## Copy（复制）

Copy（复制）选项可用于制作所选对象（实体、组件、组件组）的副本，并将副本置于剪贴板。然后你可以将对象粘贴到新位置。更多详细信息请参阅 Paste（粘贴）。

以下限制适用于 Cut and Paste（剪切并粘贴）、Copy and Paste（复制并粘贴）以及 Drag and Drop（拖放）功能：

- 不能将实体置于组下，坐标系（frames）除外。
- 不能将组件置于实体下。
- 不能将组置于实体下。
- 不能将组件置于另一个组件下。

如果尝试将对象置于不支持该操作的另一个对象下，光标将显示如下。

<a id="v1-s125"></a>

## Paste（粘贴）

Paste（粘贴）选项可用于将已剪切或复制到剪贴板的对象（实体、组件、组件组）放置到新位置。在图形查看器中单击一个点，或在对象树或操作树中选择一个元素，然后选择 **Paste**（粘贴）。

> **注意**
>
> 作为使用 Cut 和 Paste 命令的替代方法，你可以在对象树和操作树中拖放对象。

以下限制适用于 Cut and Paste、Copy and Paste 以及 Drag and Drop 功能：

- 不能将实体置于组下，坐标系除外。
- 不能将组件置于实体下。
- 不能将组置于实体下。
- 不能将组件置于另一个组件下。

如果尝试将对象置于不支持该操作的另一个对象下，光标将显示如下。

<a id="v1-s126"></a>

## Managing Versions and Releases（管理版本与发布）

<a id="v1-s127"></a>

### Versions（版本）

在 Process Designer 中，版本（versions）是增量式更改，而发布级别（release levels）是项目重大的开发更改。用户和管理员都可以创建新项目，但只有管理员管理项目版本和发布级别。

各组织对生成新版本和发布的标准不同。一般而言，版本跟踪项目内各个对象的更改，而发布涉及整个项目。创建新发布需要归档项目的先前发布。

> **注意**
>
> 本主题中描述的部分命令仅在 eM-Planner 中可用。

<a id="v1-s128"></a>

### Release Management（发布管理）

在 eM-Planner 中，项目编辑在活动发布（active release）上进行，而先前的发布被冻结（frozen）。被冻结的项目作为历史项目里程碑，因此不能被检出进行编辑。

要在 Process Planner 中创建新项目发布，所有当前已检出的项目节点必须检入。

**操作步骤**

1. 从 Open Project（打开项目）窗口中选择项目。请参阅 Open an Existing Project（打开现有项目）。
2. 包括管理员在内的所有用户，凡有已检出项目节点的，都必须将这些节点检入。本节其余步骤仅适用于管理员。在创建新发布之前，必须冻结所有当前已有的发布。
3. 选择希望冻结的发布。
4. 选择 **Tools → Versions → Freeze**，以冻结所选发布。出现以下窗口。
5. 单击 **OK**（确定）。现在可以创建新发布，如下所示。
6. 选择 **Tools → Versions → Create Release**。

   出现以下窗口。
7. 在相应字段中输入发布名称（Release Name）以及任何注释（Comment）。
8. 单击 **OK**（确定）。系统创建新发布。

   如果先前项目的 Project Tree（项目树）窗口仍打开，请将其关闭以启用对新发布的工作。

<a id="v1-s129"></a>

### Local Version（本地版本）

在规划过程中，规划人员可以捕获项目的快照并将其保存为本地版本（local version）。本地版本按用户存储在本地的 workstation 上。规划人员随后可以比较不同版本和本地保存，并决定通过使用 Change Management（变更管理）模块恢复已保存的本地版本。检入数据会清除本地版本。

在 eM-Planner 中保存特定节点的本地版本：

**操作步骤**

1. 右键单击希望保存的节点，并从上下文菜单中选择 **Save Local Version**（保存本地版本）。出现 Save 对话框。
2. 填写所需字段。如果需要，按相应按钮以显示将保存在本地版本中的对象。请注意，使用 Details（详细信息）按钮展开窗口会在编译“Related objects to be saved（将要保存的相关对象）”列表时占用机器资源并暂时影响系统性能。
3. **Save With Hierarchy**（随层级保存）复选框决定保存的分辨率级别。选中时，与所选节点关联并位于其层级之下的对象也将被保存。

<a id="v1-s130"></a>

### Node History（节点历史）

你可以查看和检查任何给定节点的版本与本地保存的历史记录，以供参考。

**操作步骤**

1. 选择希望查看的节点。
2. 单击工具栏上的相应按钮。

   出现 Node History 对话框。
   > **注意**
   >
   > 在 eM-Planner 中，也可以右键单击节点并从上下文菜单中选择 Node History。本地保存旁边标有保存图标。Process Designer 中当前打开的版本标有蓝色对勾图标。

<a id="v1-s131"></a>

### Version Management（版本管理）

你可以更改当前打开的版本，或编辑当前打开版本的属性。

**操作步骤**

1. 单击工具栏上的相应按钮。

   出现 Versions Manager（版本管理器）对话框。
   > **注意**
   >
   > 在 eM-Planner 中，单击 **Tools > Versions** 访问 Versions Manager。
2. 选择希望查看的版本并单击 **Change Version**（更改版本）。
3. （可选）如果希望编辑当前打开版本的属性，可以单击 **Edit Version**（编辑版本）。出现 Edit Version's Properties（编辑版本属性）对话框。
4. 根据需要编辑 Version Name（版本名称）和 Version Comment（版本注释），并单击 **OK**（确定）。
5. 单击 **Close**（关闭）退出 Versions Manager 对话框。

<a id="v1-s132"></a>

## Modules（模块）

<a id="v1-s133"></a>

### Process Designer Modules - Object Collections（Process Designer 模块——对象集合）

Process Designer 中的模块（module）是一个单一的编辑单元，包含用户根据工作范围定义的一组对象集合。用户无需使用整个 Project tree（可能非常庞大），而是可以定义自己的特定模块，在其中可引用特定对象。定义模块后，用户可以利用它仅检出其所包含的对象，进行更改，然后使用模块将对象检入。这不会导致 Process Designer 任何对象行为的改变。

<a id="v1-s134"></a>

### Creating a Module（创建模块）

可以使用 New Node（新建节点）命令在项目节点下或任何集合文件夹下创建模块。

**操作步骤**

1. 右键单击放置模块的文件夹，并从上下文菜单中选择 **New**（新建）；或者，在选中目录时单击相应按钮。
2. 勾选 Module（模块）图标旁的复选框，在 **Amount**（数量）列中设置所需模块数量，并单击 **OK**（确定）。

   必要时可以在其他模块内创建模块。

<a id="v1-s135"></a>

### Adding Objects to a Module（向模块添加对象）

模块可以包含项目中已存在的任何类型的对象。

**向模块添加对象：**

- 将对象从其所处的树拖放到模块节点。

  共享对象在模块窗口中以实心黑色文本显示，而单个对象以浅灰色显示（模块是其单一父级）。

  上图显示了一个打开的模块，其中包含单个对象“Pump with filter”和共享对象“Resource Library”。

  使用拖放是为模块分配对象的唯一方式。

  共享对象不会自动将其子对象包含在模块中。为了将带有递归层级的节点复制到模块中，必须首先展开该节点并使用多选选择所有要复制的对象，然后再将其拖到模块节点。

  模块具有扁平层级——所有对象处于同一级别。要查看对象层级，请在单独的窗口中打开父节点。

<a id="v1-s136"></a>

### Removing Objects from a Module（从模块移除对象）

**从模块移除对象：**

- 选择对象并单击相应按钮。

  只能从已检出的模块中移除对象。从模块移除时，相应的项目对象不受影响。

  如果模块内的共享对象从 Project tree 中移除，它仍存在于模块中。被移除对象的文本在模块窗口中从实心黑色变为浅灰色，表示原始对象已从树视图中移除。

<a id="v1-s137"></a>

### Copying a Module（复制模块）

将模块复制到 Project tree 中的任何位置时，模块的整个内容随之一并复制。要在某个节点下复制模块，请在拖放时按住 Ctrl 键。

<a id="v1-s138"></a>

### Module Check In/Out（模块检入/检出）

定义模块后，可以像其他任何项目对象一样将其检入或检出。Check In（检入）和 Check Out（检出）对话框都有一个名为“Include Module Content”（包含模块内容）的复选框。勾选此框可将模块与其包含的所有对象一起检入或检出。这意味着来自 Project tree 的原始对应对象被检入/检出。如果保持该框未勾选，则仅检入/检出模块本身。

检出不含其内容的模块会将模块的修改限制为仅添加和移除对象。选择“Check out with Hierarchy”（随层级检出）以检出所有子节点。

<a id="v1-s139"></a>

### Modules and Change Management（模块与变更管理）

使用 Change Management（变更管理）实用工具查看模块所包含对象的更改。要打开 Change Management 实用工具，请右键单击模块节点，从上下文菜单中选择“Open With”（打开方式），然后选择“Change Management”。Change Management 窗口打开并显示模块中的所有对象及其更改。

<a id="v1-s140"></a>

## Multi-user concurrent access（多用户并发访问）

多个用户可以同时在同一个项目上工作。Check In（检入）、Check Out（检出）和 Cancel Check Out（取消检出）命令构成了一种机制，在规范数据库编辑过程的同时创建了多用户环境。由于用户可以检出各个树节点进行编辑，因此他们可以在一个公共项目上协作。Check Out（检出）过程确保同一时间只有一个人可以编辑一个节点。其他用户可以查看已检出的节点，但在检出该节点的用户将其检入之前无法编辑它们。检入节点会更新当前版本。

当对象被加载到某个树中，且你选择了 Check In / Check Out（检入/检出）视图时，会出现一个状态图标，指示该特定节点的当前检入/检出状态：

- 此图标表示节点当前由你检出，可供你编辑。
- 此图标表示另一个用户已检出该节点，你只能以只读模式打开它。
- 如果该位置未出现图标，则节点既未检入也未检出。你只能以查看方式打开它，或将其检出以便编辑。

如果当前所选节点由你检出，则仅激活 Check In 选项。如果节点已检入，则激活 Check Out 和 Cancel Check Out。如果多选同时包含已检出和已检入的节点，则 Check In 和 Check Out 均被激活。

> **注意**
>
> 在编辑节点之前，必须首先将其检出。

<a id="v1-s141"></a>

## Assembly Module（装配模块）

<a id="v1-s142"></a>

### Product Assembly Module（产品装配模块）

根据定义，制造过程是指接收来自产品设计系统的、包含在物料清单（Bill of Material，BOM）中的产品数据，并将其转换为装配过程或制造物料清单（Manufacturing Bill of Material，MBOM），后者反映了产品的实际装配方式。

在早期的工艺规划阶段，过程主要是产品驱动的。通常，工艺工程师手中的初始输入是产品数据（结构和几何）。他们的任务是产品数据转换为过程数据，即定义特定产品将如何装配。随着工艺规划阶段的推进，重点转向过程或操作流。

Process Designer 装配模块通过提供产品数据与操作数据之间必要平滑且持续的过渡来支持这一过程。使用这种动态过程确保了两个领域之间的完全兼容。

<a id="v1-s143"></a>

### Abbreviations/Glossary（缩写/术语表）

- **AM**：Assembly Module（装配模块）。
- **Assembly tree**（装配树）：制造物料清单（MBOM）。它包含代表装配顺序的结构中的部件。
- **Assembly objects**（装配对象）：AssemblyPlant、AssemblyLine、AssemblyZone、AssemblyStation 以及根据映射表从 AssemblyPart 类类型派生的其他自定义类型。
- **BOM**：原始产品物料清单。它包含产品数据，包括结构、几何以及结构中所有部件的附加信息，但与装配过程无关。
- **MBOM**：制造物料清单——Assembly tree 的同义词。
- **Pre-Assembly tree (PreAT)**（预装配树）：BOM 的副本。用于定义装配过程。
- **Process**（过程）：操作计划——要执行的操作流。
- **Process objects**（过程对象）：PrPlantProcess、PrLineProcess、PrZoneProcess、PrStationProcess 以及根据映射表从 Process 类类型派生的其他自定义类型。

<a id="v1-s144"></a>

### Target Users（目标用户）

- **Administrator user**（管理员用户）：具有 Process Designer 管理员权限；管理员定义应用程序设置并在项目生命周期内维护它们。此外，管理员执行工作流中一些所需步骤，例如初始导入装配模块自定义并使用 Assembly Module Administration（装配模块管理）工具。
- **Process engineers**（工艺工程师）：典型的 Process Designer 用户，没有特殊权限。

<a id="v1-s145"></a>

### Assembly Module Workflow（装配模块工作流）

在用户可以开始使用装配模块之前，管理员用户必须导入支持模块中新数据对象的自定义，并设置各种操作参数，如 Administration Tool（管理工具）部分所述。

装配模块支持通过复制 BOM 从原始 BOM 创建预装配树（Pre-AT）。这使得工程师可以使用 Pre-AT，将部件从 Pre-AT 分配到装配树或过程，同时保持原始 BOM 不变。请注意，复制的复合部件在 Pre-AT 中成为“装配部件”（assembly parts），而复制的部件仍保持为部件。强烈推荐这种工作方式，并提供许多好处，例如提供参照，使从 Assembly tree 返回 Pre-AT 的部件可以放置在其原始位置。但是，此功能是可选的，装配模块的其他命令（Updating Pre-Assembly and Assembly Trees from BOM 除外）也可以与原始的、未复制的 BOM 一起使用。

在复制 BOM 时，分配给 BOM 部件和复合部件的 Mfgs 可以像部件本身一样被复制，也可以在原始部件与复制部件之间共享，由管理员决定。

假设已复制 BOM，复制的部件将从 Pre-AT 转移到 Assembly tree，可以直接转移，也可以通过分配到过程进行转移。如果 BOM 中的部件发生更改，用户通常希望将这些更改更新到 Assembly tree 或 Pre-AT 中的复制部件。通常，这些更改包括添加新对象、移除现有对象、层级更改或简单的字段更新（如名称、注释、位置等）。

用户应在 BOM 的根上激活 Updating Pre-Assembly and Assembly Trees from BOM（从 BOM 更新预装配树和装配树）命令，同时选择并行复制的对象，以及整个 Assembly tree 的根。然后该命令将 BOM 部件与复制部件（无论是在 Assembly tree 还是 Pre-AT 中）进行比较，并相应地更新复制部件。有关更新方案的更多详细信息，请参阅 Hierarchy Change Among BOM Objects（BOM 对象间的层级更改）。

装配模块包含一个过程和一个必须兼容、在装配过程中相互反映的 MBOM。装配模块使用户能够在两种不同模式下工作：

- 用户定义或编辑过程，然后创建或更新 Assembly tree（MBOM）——Creating/Updating MBOM（从过程创建/更新 MBOM）。他们手动编辑过程本身，并将部件从 Pre-AT 分配到过程。当从过程更新到装配时，已从 Pre-AT 移除的已分配部件被放置到 Assembly tree 中的正确位置，反映已编辑的过程。如果不存在 Assembly tree，该命令会创建一个。
- 用户创建或编辑 MBOM（Assembly tree），通过将部件直接从 Pre-AT 分配到 Assembly tree 中的复合装配对象（Plants、Lines、Zones、Stations 等）。然后用户可以通过装配模块自动更新过程，创建适当的过程对象、流等。

下图展示了一个兼容过程与 MBOM 的简单示例。

<a id="v1-s146"></a>

### Basic Assumptions（基本假设）

**在过程中**

- 同一过程级别下的所有对象必须属于同一类型。

**在 MBOM 中**

- 过程对象（例如 station）不能在不同的范围（例如 zones）之间共享。
- 部件实例（Part-instances）不能在不同的范围之间共享（例如位于不同 zone 中的两个 station）。
- 如果 BOM 已被复制为多个 Pre-AT，则只有来自一个 Pre-AT 的对象应填充 Assembly tree，同时填充相应的过程。重要提示：应用程序不会阻止……

用户将对象从多个 Pre-AT 分配到装配或过程，但在此情况下结果将不正确。

**限制（Limitations）**

如果存在中间层级隐藏类型（例如 zones），则不能通过修改 MBOM 在这些类型之间移动对象（这可以通过修改 Process 来实现）。在使用接口（interfaces）或范围流（scope flows）时也是如此。

以下示例说明用户无法执行此类操作：

**本地化（Localization）**

装配模块应用程序支持大多数 UI 消息、标题和命令的本地化。要使用此功能，请在 `BIWPlanningCommandsStringTable.csv` 文本文件中现有英文列的左侧添加一列，并在英文条目旁边添加所需语言的任何消息或字符串的翻译。

> **注意**
>
> 仅使用 Excel 编辑此文件。建议在编辑前备份原始文件。

<a id="v1-s147"></a>

### Model Objects（模型对象）

<a id="v1-s148"></a>

#### AssemblyPart

此类派生自 PmCompoundPart（Pm复合部件）。

将 BOM 复制到 Pre-AT 时，每个 PmCompoundPart 对象都被复制为一个 AssemblyPart 对象。

AssemblyPart 包含一个名为 **ProcessID** 的关系软字段（soft field）。该字段包含兼容过程对象（如果存在）的外部 ID。

<a id="v1-s149"></a>

#### AssemblyPlant

AssemblyPlant 类派生自 AssemblyPart。在 MBOM 中，它充当 Process 中 PmPlantProcess 的对应物。

<a id="v1-s150"></a>

#### AssemblyLine

AssemblyLine 类派生自 AssemblyPart。在 MBOM 中，它充当 Process 中 PmLineProcess 的对应物。

<a id="v1-s151"></a>

#### AssemblyZone

AssemblyZone 类派生自 AssemblyPart。在 MBOM 中，它充当 Process 中 PmZoneProcess 的对应物。

<a id="v1-s152"></a>

#### AssemblyStation

AssemblyStation 类派生自 AssemblyPart。在 MBOM 中，它充当 Process 中 PmStationProcess 的对应物。

<a id="v1-s153"></a>

#### Recycle_Bin

Recycle_Bin（回收站）是 eMServer 模型中 Module 的一个子类，用于存储运行装配模块命令时从 Pre-AT、MBOM 或过程移除的不同类型的对象（装配、过程、部件等）。

<a id="v1-s154"></a>

#### AssemblySub

AssemblySub 类派生自 AssemblyPart。

<a id="v1-s155"></a>

#### VariantedAssemblyFlow

VariantedAssemblyFlow 是 eMServer 模型中 AssemblyPart 的一个子类。此类属于 MBOM 结构，并反映 PERT 中流（flow）的功能：

PERT 中的流通常转换为 MBOM 中的父子关系。但是，如果流具有变量集（variant set），则父子关系不能在 MBOM 中反映这一点。此类在 MBOM 中反映变量集信息和功能。

<a id="v1-s156"></a>

#### PmProcess

PmProcess 类包含一个名为 **AssemblyID** 的软关系字段。该字段包含兼容装配对象（如果存在）的外部 ID。

<a id="v1-s157"></a>

#### New User Customized Types（新增用户自定义类型）

管理员可以添加其自己的自定义类型。类型名称必须根据下表定义：

| 装配结构对象（派生自 AssemblyPart） | 对应过程对象（派生自 Process） |
| ------------------------ | ------------------- |
| AssemblyXXXX             | PrXXXXProcess       |

示例：

| 装配结构对象       | 对应过程对象        |
| ------------ | ------------- |
| AssemblyNode | PrNodeProcess |

<a id="v1-s158"></a>

### Creating Pre-Assembly Tree from BOM（从 BOM 创建预装配树）

<a id="v1-s159"></a>

#### Maintaining Original BOM in Pre-Assembly Tree（在预装配树中保留原始 BOM）

复制 BOM 并创建预装配树（Pre-AT）使用户能够在将产品数据转换为过程/装配数据的整个过程中保持原始 BOM 不变。Pre-AT 存储尚未分配的部件。

因此，用户保留了完整的 BOM，即使在规划过程结束后也可以访问，并且如果部件从 Assembly tree 中移除，还可以将部件恢复到其在 Pre-AT 中的正确位置。

要执行此操作，用户：

**操作步骤**

1. 在 BOM 中选择所需范围（scope）。
2. 单击 **Create Pre-Assembly Tree from BOM**（从 BOM 创建预装配树）图标以运行命令。不会打开对话框，但光标会变为沙漏（由于 BOM 中数据量通常很大），直到命令完成。
   > **注意**
   >
   > BOM 是一个 CompoundPart（复合部件），仅包含复合部件或常规部件。
   >
   > 该命令使用相同名称从 BOM 中所选范围创建 Pre-AT。复制部件的 externalID 为“AssyX-[BOM_EXTERNALID]”，其中 X 表示连续计数器后缀。如果针对已复制到 Pre-AT 的 BOM 执行该命令，计数器会递增，直到达到唯一值。
   应用程序不依赖于 BOM 或复制部件的名称，使用户可以随意重命名它们，即使使用非唯一名称也可以。

<a id="v1-s160"></a>

#### First Time Duplicating a BOM（首次复制 BOM）

该命令使用范围根（root）的名称从 BOM 中所选范围创建 Pre-AT。Pre-AT 被放置在用户文件夹下的“Imported From BOM”（从 BOM 导入）特殊文件夹中。

Pre-AT 的类型为 AssemblyPart，其外部 ID 与 BOM 相同，并添加了“Assy1-”前缀。

类似地，BOM 中的所有 Parts 和 CompoundParts 都作为 AssemblyParts 在 Pre-AT 中创建，具有相同的外部 ID，并带有“Assy1-”前缀。

<a id="v1-s161"></a>

#### Subsequent Duplications of the BOM（BOM 的后续复制）

以与首次创建相同的方式更新 Pre-AT：

**操作步骤**

1. 在 BOM 中选择所需范围。
2. 单击 **Create Pre-Assembly Tree from BOM** 图标以运行命令。不会打开对话框，但光标会变为沙漏（由于 BOM 中数据量通常很大），直到命令完成。

   新 Pre-AT 的名称不需要唯一，并保留所选 BOM 根的原始名称。

   但是，它会根据 BOM 被复制的次数接收唯一的计数器作为后缀。即使所选 BOM 的范围与之前所选范围不同，计数器也会递增。

   树本身及其包含的所有部件的外部 ID 都被分配“AssyX-”前缀，其中 X 为上述计数器。

<a id="v1-s162"></a>

#### Create Copied Mfgs Option（创建复制的 Mfgs 选项）

装配模块管理工具通过复选框提供以下选项：

- **选项已勾选——Mfg 特征被复制（Option Checked - Mfg Features Duplicated）**

  分配给 BOM 中部件的任意 Mfg 特征也会被复制，使用相同的外部 ID 命名约定（“AssyX-”前缀），包括 Mfg 特征库中找到的内部层级。复制的 Mfgs 被分配给其各自的复制部件。

  例如，复制的“wp3”和“wp5”分别被分配给 Pre-AT 中复制的部件“db_lightbead”和“df_blinker”。

  原始 Mfg 库显示在中间窗格中。以下说明部分复制的 Mfg 库“Assy1-MfgLibrary”如何保留其原始层级：
- **选项未勾选——Mfg 特征不被复制（Option Unchecked - Mfg Features not Duplicated）**

  分配给 BOM 中部件的 Mfg 特征不被复制；除其原始分配外，它们还分别被分配给复制的部件。

<a id="v1-s163"></a>

### Updating Pre-Assembly and Assembly Trees from BOM（从 BOM 更新预装配树和装配树）

<a id="v1-s164"></a>

#### Updating from BOM（从 BOM 更新）

随着原始 BOM 的更改，用户必须更新复制的部件以保持一致性。此命令根据需要执行更新以反映 BOM 中的更改。

> **注意**
>
> 复制的部件可以位于预装配树或装配树中。
>
> BOM 更改可以包括部件的属性更改、不同部件之间层级的更改、新部件的添加以及现有部件的删除。

**操作步骤**

1. 单击 **Update Pre-Assembly Tree**（更新预装配树）图标。

   打开 Update from BOM（从 BOM 更新）对话框。
   > **注意**
   >
   > Source Product Tree（源产品树）区域显示所选 BOM 的名称。
2. 从组合框中选择要更新的 Pre-AT。可用 Pre-AT 的外部 ID 显示在组合框中，包含唯一的“AssyX-”前缀以避免歧义。
3. 选择一个要更新的 Assembly tree（这是必要的，因为只有 BOM 和 Pre-AT 如装配模块工作流中所述那样关联），并单击 Browse（“...”）打开 Select Assembly Tree（选择装配树）对话框。
4. 从树中选择一个复合装配对象（只有派生自 AssemblyPart 的对象才会启用 OK 按钮）。
5. 单击 **OK**（确定）。
   > **注意**
   >
   > 如果用户未选择 Assembly tree（步骤 3），该命令仅查找位于 Pre-AT 中的复制部件，并仅更新这些部件。
   >
   > **重要提示**：由于该命令无法执行部分更新，因此应仅选择 BOM 的根。同样，应仅选择 Assembly tree 的根。
   >
   > 所选 Assembly tree 与 Pre-AT 必须“匹配”，即所选 Assembly tree 中的部件只能来自所选 Pre-AT。如果 Assembly tree 包含来自不同 Pre-AT 的部件，或所选 Assembly tree 中的部件源自不同的 Pre-AT，则结果将不正确。

<a id="v1-s165"></a>

#### Attribute Change in BOM Objects（BOM 对象的属性更改）

从 BOM 更新会更改相应复制部件中的同一属性，无论是在 Pre-AT 还是 Assembly tree 中。

相应部件具有与 BOM 中部件相同的外部 ID，并带有“AssyX-”前缀，其中 X 表示 Pre-AT 的运行计数器。

<a id="v1-s166"></a>

#### Hierarchy Change Among BOM Objects（BOM 对象间的层级更改）

BOM 中的层级更改会反映在 Pre-AT 或 Assembly tree 中，具体取决于匹配对象的位置。例外是直接位于 station 下的部件。

以下是层级更改场景的描述：

- **部件（或 AssemblyPart）仅出现在 Pre-AT 中**

  部件的层级根据 BOM 中的更改精确更新。
- **部件（或 AssemblyPart）出现在 Assembly tree 中，但未直接连接到 station**

  部件的层级根据 BOM 中的更改精确更新，与前一场景相同。
- **部件（或 AssemblyPart）出现在 Assembly tree 中，并直接连接到一个或多个 station**

  部件的层级不发生变化——其父级仍为该 station。
  > **注意**
  >
  > 如果将来同一对象从该 station 取消分配，它将返回到 Pre-AT 中其在更改后 BOM 中的正确（新）位置（这只有在原始 BOM 已被复制时才能实现）。
- **部件在 Assembly tree 中共享，既直接连接到 station，又不直接连接到 station**

  直接连接到 station 的部件外观保持不变，但未直接连接到 station 的外观根据 BOM 中的更改更新，如以下示例所示：
  - 初始状态，在对 BOM 进行任何更改之前。注意“f_bottomplate”在 Assembly tree 中共享，并且既直接又间接连接到 station：

    最终状态，“f_bottomplate”已在 BOM 中被移动为根的直接后代。在 Assembly tree 中，“f_bottomplate”的外观已从 Comp 下移除，因为它未直接连接到 station。另一方面，它在连接到“PrStation1”的原始位置保留了其外观。
  - 初始状态，类似于前一示例，但“f_windowbead”作为共享部件，另一个复合部件“Comp2”连接到“PrStation2”：

    最终状态，“f_windowbead”已在 BOM 中被移动为“Comp2”复合部件的后代。因此，在 Assembly tree 中，其外观已从“Comp”下移除，因为它未直接连接到 station（其父级是复合装配）。但是，其外观保留在原始位置，即直接连接到“PrStation1”处。另一个“f_windowbead”的外观也已正确添加到“Comp2”下，在那里它同样不直接连接到 station。

<a id="v1-s167"></a>

#### New Part (or CompoundPart) Added to the BOM（向 BOM 添加新部件（或复合部件））

更新时，无论其父级当前位于 Pre-AT 还是 Assembly tree 中，都会在 BOM 中其父级的副本下复制一个匹配的、带有“AssyX-”前缀的新 Part（或 AssemblyPart）。

<a id="v1-s168"></a>

#### Part (or CompoundPart) Deleted from the BOM（从 BOM 删除部件（或复合部件））

当从 BOM 中删除 Part（或 CompoundPart）时，无论其是否直接分配给 station，它都会从 Pre-AT 和 Assembly tree 中移除。

<a id="v1-s169"></a>

#### Create Copied Mfgs Option（创建复制的 Mfgs 选项）

有关装配模块管理工具中此选项功能的说明，请参阅“从 BOM 创建预装配树”部分中的 Create Copied Mfgs Option。

BOM 中的更新也可能影响已分配的 Mfg 特征。因此，如果在创建 Pre-AT 期间启用了此选项，你可能还需要更新复制的 Mfg 库。

由于复制的 Mfg 库的层级仅由其原始 Mfg 库决定，BOM 中唯一相关的更改为：

- **向 BOM 部件（或 CompoundPart）分配新的 Mfg 特征**

  该 Mfg 特征将在与 Mfg 库中其原始 Mfg 特征相同的层级位置被复制（这可能包括创建新的 Mfg 库）。
- **从 BOM 删除/取消分配 Mfg 特征**

  复制的 Mfg 特征将从复制的 Mfg 库中移除，并移动到回收站（没有任何 Mfg 特征的 Mfg 库也会被删除）。

<a id="v1-s170"></a>

### Creating/Updating MBOM (from the Process)（创建/更新 MBOM（从过程））

<a id="v1-s171"></a>

#### Creating/Updating Assembly（创建/更新装配）

在修改 Process（添加或移除 station、更改流、分配或取消分配部件等）之后，用户可以根据修改自动更新 MBOM，或者如果 MBOM 不存在则创建一个新的 MBOM。

**操作步骤**

1. 在 Process 中选择一个范围（scope）。
2. 单击 **Create/Update Assembly**（创建/更新装配）图标。

   显示对话框。
3. 如有必要，通过单击新对象或在其 **Source Process**（源过程）字段中输入其名称来更改过程范围。
4. 单击 **Apply**（应用）执行更新。
   > **注意**
   >
   > “Include adjacent interactions”（包含相邻交互）选项仅与部分更新相关，且仅在部分更新时出现，它指示命令更新通过接口或范围流连接到当前范围的过程对象。
   >
   > 请参阅 Updating MBOM from Sub Process Object (Partial Update)（从子过程对象更新 MBOM（部分更新））。
   Status Report（状态报告）文本框显示已执行的主要更改的详细信息，例如创建新的 Assembly 对象或将 Assembly 对象移动到回收站。

<a id="v1-s172"></a>

#### Process Objects Added or Removed（添加或移除过程对象）

添加新 Process 对象后，更新 MBOM 会创建相应的 Assembly 对象（仅针对孪生对象（twin objects））。Name、Number 和 Variant 字段从 Process 对象复制到 Assembly 对象。

移除 Process 对象后，更新 MBOM 会将其相应的 Assembly 对象从 MBOM 移动到用户文件夹下的回收站。更新会将 Assembly 对象下的 Parts 移回 Pre-AT。对于不存在 Pre-AT 的情况（即 BOM 未被复制），这些部件将被移动到位于用户文件夹下的 Unassigned parts（未分配部件）文件夹。

示例：

修改前：

过程修改：

- Station_A2 从 Zone_A 中删除
- Station_B3 添加到 Zone_B

修改过程并执行命令后：[图示]

<a id="v1-s173"></a>

#### Parts (or Assembly Parts) Assigned or Unassigned（分配或取消分配部件（或装配部件））

**当用户将 Parts（或 Assembly Parts）分配给过程时：**

- 如果来自 Pre-AT 的部件被分配给 Process 对象（例如 station），更新 MBOM 会将该部件从 Pre-AT 移动到相应的 Assembly station（与过程兼容）。
- 如果部件被分配给多个 station，更新 MBOM 会使该部件出现在 MBOM 中所有相关的 station 下（在它们之间共享）。
- 如果部件在 Pre-AT 中出现多次（共享）并被分配给 Process 对象，更新 MBOM 会将该部件移动到 MBOM，并移除其在 Pre-AT 中的所有外观。

示例：

修改前：[图示]

过程修改：

- Part1 分配给 Station_B1
- Part2 分配给 Station_B1 和 Station_B2
- Part4 分配给 Station_A1

修改过程并执行命令后：Part1、Part2 和 Part4 从 Pre-AT 中移除。每个已分配的部件出现在其所分配的 station 下（Part2 在 Station_B1 和 Station_B2 之间共享）。

> **注意**
>
> 当直接从 BOM 分配部件时，行为相同——只是在这种情况下针对 BOM 而非 Pre-AT，且是 Compound parts（复合部件）而非 Assembly parts（装配部件）。

**当用户从过程取消分配 Parts（或 Assembly Parts）时：**

- 如果部件从 Process 对象（例如 station）取消分配，更新 MBOM 会从 Assembly station 移除该部件，并使其重新出现在 Pre-AT 中（BOM 表示原始结构，用于确定部件在 Pre-AT 中的正确位置）。
- 如果部件分配给多个 station，然后仅从其中一些取消分配，更新 MBOM 会从多余的 Assembly station 移除该部件。只要该部件仍分配给至少一个 station，它就不会重新出现在 Pre-AT 中。
- 如果部件最初在 Pre-AT 中出现多次（共享）并从其所分配的所有 station 取消分配，更新 MBOM 会从 MBOM 中的 Assembly station 移除该部件，并使其在 Pre-AT 中所有原始位置重新出现（它恢复为在 Pre-AT 中共享）。

> **注意**
>
> 如果取消分配的部件源自 BOM，则该部件不会在 BOM 中重新出现，而是被移动到位于用户文件夹下的 Unassigned parts 文件夹。

示例：

过程修改（接上一示例）：

- Part1 和 Part2 从 Station_B1 取消分配

修改过程并执行命令后：Part1 从 MBOM 中的 Station_B1 移除并返回 Pre-AT（在 Comp_A 和 Comp_B 之间共享）。Part2 从 MBOM 中的 Station_B1 移除，但仍出现在 Station_B2 下。

<a id="v1-s174"></a>

#### Process Flow Defined or Changed（定义或更改过程流）

过程中没有变量集的流（flow）在 MBOM 中被转换为后继者与前驱者之间的父子关系。

<a id="v1-s175"></a>

#### Support for Variant Information（对变量信息的支持）

变量信息可存在于过程的以下各项中：

- 部件（Parts）
- 过程对象（Stations、Zones 等）
- 流（过程对象之间，或源与过程对象之间）
- 汇点（Sinks）和源点（Sources）

部件在 MBOM 中按原样出现，保留其变量信息。过程对象在 MBOM 中具有兼容的 Assembly 对象。Name、Number 和 Variant 字段从 Process 对象复制到 Assembly 对象，同时保留变量信息。汇点和源点在 MBOM 中没有意义，因此其变量信息不相关。如果过程模型正确，汇点或源点的变量集与其流的变量集相同。

流是一种特殊情况。过程中没有变量集的流在 MBOM 中被转换为后继者与前驱者之间的父子关系。这种关系不能包含变量信息。以下示例说明了在 MBOM 中表示流的变量集的必要性：

在变量集 VS1 中，流直接从 Station_A1 到 Station_A3；而在 VS2 中，流从 Station_A1 经 Station_A2 到 Station_A3。

根据不同变量集进行筛选时，结果可能出乎意料。按 VS1 筛选按预期工作。按 VS2 筛选会得出正确的 PERT 表示，但 MBOM 表示不正确。

因此，对于变量集与后继者变量集不同的流，会创建 VariantedAssemblyFlow 对象以在导航树中正确显示此信息。

附加示例：

没有流上的变量集时：[图示]

有流上的变量集时：[图示]

<a id="v1-s176"></a>

#### Updating MBOM from Sub Process Object (Partial Update)（从子过程对象更新 MBOM（部分更新））

大多数用户在多用户环境中工作（工作通过让不同用户处理不同范围（例如 Zones）来划分）。因此，部分更新对于启用单个用户所做更改的更新是必要的。如果用户选择 PrZoneProcess，该命令会更新该 zone 及其所有后代（在本例中为 stations）。更新包括完整更新的所有操作，但仅限于所选范围（例如添加或移除 station、分配或取消分配部件等）。更新包括从源点进入所选对象的输入流。如果是这样，更新 station 涵盖了到特定 station 的部件分配。

部分更新可能包括来自其他范围的相邻交互（接口和范围流）。接口可以是输入接口或输出接口（输入 = 相邻装配对象被分配给当前范围中过程对象的传入范围流；输出 = 当前范围的装配对象被分配给位于不同范围中的范围流）。相邻交互可能要求特定对象未被其他用户检出（以将范围流转换为父子关系）；如果存在此类条件，命令无法运行，系统会提供高级通知。如果勾选了“Include adjacent interactions”选项，更新会包含范围流，并添加或移除接口。

> **注意**
>
> 在更改接口后执行更新，其中相邻 Assembly 对象已从接口流中移除，会将 Assembly 对象从当前范围的不相关父级断开连接，并将其移动到用户文件夹。

示例：

更改接口前：[图示]

断开装配与接口流的连接并更新后：[图示]

<a id="v1-s177"></a>

#### Process Flow with Parallel Stations（具有并行 station 的过程流）

**过程流包含用于变量差异的并行 station**

如果 Process 流包含并行 station，MBOM 将包含一个共享 station。

示例：[图示]

<a id="v1-s178"></a>

#### The Process Includes an Interface or a Scope Flow（过程包含接口或范围流）

如果过程包含指向 Process 对象的接口或范围流，更新 MBOM 会将连接到该接口的 Assembly 对象放置在对应于该 Process 对象的 Assembly 对象下（而不是在没有传入接口的非父级下）。

示例：

定义接口前：[图示]

定义接口并执行更新命令后：[图示]

可以定义多个接口，用户可以通过接口使 station 共享：

前：[图示]

后：[图示]

<a id="v1-s179"></a>

#### Support for Check-out（对检出的支持）

MBOM 更新命令会自动检出已检入且即将被更改的对象。在以下情况下，它会通知有关被其他用户检出的对象：

- 接口和/或范围流的更改需要更改其他用户的对象。
- 属于所选范围（过程、资源和 MBOM）的对象。

如果命令识别出上述类型的对象，则不会运行。

<a id="v1-s180"></a>

#### Assign In-Process-Assembly as Output Parts（将过程中装配分配为输出部件）

Assembly 对象应连接到相应 Process 对象的输出流。在这种情况下，Assembly 对象显示在 Properties（属性）查看器的 Products（产品）选项卡中，并带有方向属性（consumed/produced，消耗/生产）。

示例：[图示]

<a id="v1-s181"></a>

#### Mid-level hidden type and Lowest level hidden type Options（中间层级隐藏类型和最低层级隐藏类型选项）

Mid-level hidden type（中间层级隐藏类型）和 Lowest level hidden type（最低层级隐藏类型）是装配模块管理工具中的组合框，用于确定 Assembly tree 中哪些级别（如果有）不显示，分别对应不带其子级或带其子级。更多信息和示例请参阅 Administration Tool（管理工具）部分。

<a id="v1-s182"></a>

#### Rollback Support when Command Fails（命令失败时的回滚支持）

该命令要么完整地正确执行，要么在任何阶段失败时完全中止。它不会部分执行。

<a id="v1-s183"></a>

#### Creating Necessary Sinks and Delete Redundant Sources, Sinks and Flows（创建必要的汇点并删除多余的源点、汇点和流）

该命令更新 MBOM，同时也会整理 PERT：

- 创建必要的汇点（针对“out”对象）。
- 删除仅具有前驱者或仅具有后继者的流。
- 删除空的源点和多余的汇点。

<a id="v1-s184"></a>

### Creating/Updating Process (from the MBOM)（创建/更新过程（从 MBOM））

<a id="v1-s185"></a>

#### Creating/Updating Process（创建/更新过程）

在修改 MBOM（添加/移除 station、更改 Assembly 对象之间的层级以反映 Process 流、将部件从 Pre-AT 移动到 MBOM、将带有变量集的 AssemblyFlow 对象定位到预期位置等）之后，用户可以根据这些修改自动更新 Process。

该命令更新 Process（如果 Process 不存在则创建新的 Process）。

**操作步骤**

1. 在 MBOM 中选择所需范围。
2. 单击 **Create/Update Process**（创建/更新过程）图标。打开 Create/Update（创建/更新）对话框。
3. 如有必要，在“Source Assembly”（源装配）字段中输入要更新的 MBOM 范围的根，可通过单击该根或在该字段中输入其名称。
   > **注意**
   >
   > Status Report 文本框包含主要更改的详细信息，例如创建新的 Process 对象或将 Process 对象移动到回收站。

<a id="v1-s186"></a>

#### Assembly Objects Added or Removed（添加或移除装配对象）

添加新 Assembly 对象后，更新过程会在过程中创建兼容的 Process 对象（仅针对孪生对象）。Name、Number 和 Variant 字段从 Assembly 对象复制到 Process 对象。MBOM 的层级按如下方式决定过程的层级和流：

- 不同类型的 Assembly 对象之间的父子关系被转换为等效 Process 对象之间的父子关系。
- 相同类型的 Assembly 对象之间的父子关系被转换为等效 Process 对象之间的流。

以下说明一个模糊情况。在两个不一定属于同一 zone 的其他 station 之间向 MBOM 添加了一个新 station：

在这种情况下，如果新 station 的所有相邻对象（父级和子级）属于同一过程父级，则新 station 被添加到该过程父级。如果不是，则新 station 被添加到其父级的过程父级（如果 station 有多个父级，则随机添加到其中一个）。

在上例中，假设“Station_XY”属于“ZoneX”，则“New Station 1”添加到“Zone_B”，“New Station 2”添加到“Zone_B”，“New Station 3”添加到“Zone_A”。

如果移除了 Assembly 对象，更新过程会将该相应的 Process 对象从过程移动到用户文件夹下的回收站。

修改前：[图示]

过程修改：

- Station_A2 从 Zone_A 中删除
- Station_B3 添加到 Zone_B

修改过程并执行更新命令后：[图示]

<a id="v1-s187"></a>

#### Parts (or Assembly Parts) were Moved To or From the MBOM（部件（或装配部件）被移入或移出 MBOM）

**用户将 Parts（或 Assembly Parts）移动到 MBOM**

- 如果部件从 Pre-AT 移动到 MBOM 中某个 Assembly 对象（例如 Assembly station）下，更新会将该部件分配给过程中兼容的 Process station。
- 如果部件被移动到多个 Assembly station（在它们之间共享），更新会将该部件分配给过程中所有兼容的 station。

修改前：[图示]

MBOM 修改：

- Comp_A 移动到 Station_A1 下
- Comp_B 移动到 Station_A2 下
- Part4 移动到 Station_A1 和 Station_A2 下（在它们之间共享）

修改过程并执行命令后：[图示]

> **注意**
>
> 当直接从 BOM 分配部件时，行为相同——只是在这种情况下针对 BOM 而非 Pre-AT，且是 Compound parts 而非 Assembly parts。

**用户从 MBOM 移除 Parts（或 Assembly Parts）**

从 MBOM 移除部件（例如从某个 Assembly station 下）后，更新会将该部件从与 Assembly station 兼容的 Process station 取消分配。

示例：

MBOM 修改（接上一示例）：

- Comp_B 从 Station_A2 下移除

修改 MBOM 并执行命令后：[图示]

<a id="v1-s188"></a>

#### The Structure of the MBOM Changed (Hierarchy Change)（MBOM 结构更改（层级更改））

**不含 VariantedAssemblyFlows 的 MBOM 结构发生更改**

对 MBOM 层级的更改表明装配过程已更改。新的父子关系应在 Process 中表示为新的流（或范围流）。不再有效的父子关系在 Process 中具有相应的应被删除的流。

示例：

修改前：[图示]

MBOM 修改：Station_A2 与 Station_A1 的父子关系被反转。

修改过程并执行命令后：从 Station_A1 到 Station_A2 的流被删除。创建了从 Station_A2 到 Station_A1 的新流。移除了 Station_A2 的汇点。创建了 Station_A1 的新汇点。[图示]

**含 VariantedAssemblyFlows 的 MBOM 结构发生更改**

如果创建了 VariantedAssemblyFlow 对象作为 Assembly 父级与 Assembly 子级之间的链接，则 Process 对象之间的流应具有变量集（与分配给 VariantedAssemblyFlow 对象的相同）。

<a id="v1-s189"></a>

#### Support for Variant Information（对变量信息的支持）

变量信息可作为以下之一存在于 MBOM 中：

- 部件上
- 过程对象上（Stations、Zones 等）
- 作为两个过程对象之间或过程对象与 Part/CompoundPart 之间的 VariantedAssemblyFlow 对象

部件按原样分配给 Process 对象，从而保留其变量信息。此外，为每个已分配的 Part 在过程中创建一个单独的源点（GetPart），以便于向其流添加变量信息。

Assembly 对象在过程中具有相应的 Process 对象。Name、Number 和 Variant 字段从 Assembly 对象复制到 Process 对象，保留变量信息。

VariantedAssemblyFlows 用于表示 MBOM 中的变量化流。如果 VariantedAssemblyFlow 不存在，则在相应的父级和子级 Process 对象之间创建流（无变量集）。如果存在 VariantedAssemblyFlow，则在 VariantedAssemblyFlow 的父级和子级之间创建流，且该流包含与 VariantedAssemblyFlow 对象相同的变量信息。

示例：

修改前：[图示]

MBOM 修改：在 Station_A2 与 Station_A1 之间添加了带有变量集 VS1 的新 VariantedAssemblyFlow。

修改 MBOM 并执行更新命令后：[图示]

<a id="v1-s190"></a>

#### Updating Sub Assembly Object from MBOM (Partial Update)（从 MBOM 更新子装配对象（部分更新））

大多数用户在多用户环境中工作（工作通过让不同用户处理不同范围（例如 Zones）来划分）。因此，部分更新对于启用单个用户所做更改的更新是必要的。

MBOM 结构可能包含来自不同 zone、通过长链连接在一起的 station（由接口或范围流的使用引起）。在这种情况下，zone 的分离未在 MBOM 中表示。

例如，对于以下过程：[图示]

因此，来自 MBOM 的部分更新会影响所选对象及其所有过程同级，但不会影响所有后代（与从过程更新时不同）。用户可以选择 zone 中的任何 station，系统会显示一条消息指示正在更新哪个范围。

<a id="v1-s191"></a>

#### MBOM Containing Shared Stations（包含共享 station 的 MBOM）

对于包含共享 station 的 MBOM，过程包含并行 station。对于在两个不同对象之间共享的 station，过程包含从该 station 到两个对象的流。

示例：

前：[图示]

修改 MBOM 并执行命令后：[图示]

<a id="v1-s192"></a>

#### MBOM Includes Parent-child Relation between Objects of Different Process Parent（MBOM 包含属于不同过程父级的对象之间的父子关系）

MBOM 可能包含源自不同范围（例如 Zones）的相同类型装配（例如 Assembly stations）之间的父子关系，原因有以下之一：

- 兼容过程对象之间存在（或应该存在）范围流。
- 用户打算将 Assembly 从一个过程父级移动到另一个（例如，在 Zones 之间移动 Station）。

使用 Updating Process from MBOM（从 MBOM 更新过程）命令时，系统始终假定上述第一个原因（范围流），因为用户不能通过修改 MBOM 在 Zones 之间移动 Stations，只能通过过程来移动。

创建范围流的示例：

修改 MBOM 前：[图示]

修改 MBOM 并执行更新命令后：[图示]

该操作创建了一个接口，将 Station_A2 连接到 Station_B1。

> **注意**
>
> 在这种情况下，命令可以创建接口或范围流。它会检查是否已经存在其中之一来表示该关系，如果不存在，则创建接口。

<a id="v1-s193"></a>

#### Keeping Locations of Displayed Objects in the PERT（保留 PERT 中已显示对象的位置）

用户可以在 PERT 中排列对象（Process 对象、Sources、Sinks、Flows 等），以创建易于理解的复杂操作流图。更新命令会保留这种排列。

<a id="v1-s194"></a>

#### Support for Check-out（对检出的支持）

Process 更新命令会自动检出当前已检入、即将被修改的对象。在以下情况下，系统会提前通知有关被其他用户检出的对象：

- 需要更改其他用户对象的接口和/或范围流的创建。
- 属于所选范围（过程和 MBOM）的对象。

如果命令识别出上述类型的对象，则不会运行。

<a id="v1-s195"></a>

#### Rollback Support when Command Fails（命令失败时的回滚支持）

该命令要么完整地正确执行，要么在任何阶段失败时完全中止。它不会部分执行。

<a id="v1-s196"></a>

#### Creating Needed Sinks / Deleting Redundant Sources, Sinks and Flows（创建所需的汇点 / 删除多余的源点、汇点和流）

该命令根据需要自动整理 PERT，如下所示：

- 创建必要的汇点（针对“out”对象）
- 删除仅具有前驱者或仅具有后继者的流
- 删除空的源点和多余的汇点

<a id="v1-s197"></a>

## Privileges（权限）

<a id="v1-s198"></a>

### List of Privileges（权限列表）

默认情况下可以定义以下用户权限：

| 权限（Privilege）                                                         | 描述（Description）                                                                                                                                                                   |
| --------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Create Project                                                        | 创建项目                                                                                                                                                                              |
| Import Project                                                        | 以 PPV 或 XML 格式导入项目                                                                                                                                                                |
| Delete Project                                                        | 删除项目                                                                                                                                                                              |
| Check In                                                              | Check-in 命令后的工作流：在 GPS 应用中使用检入、创建新用户（包含隐藏检入）、某些导入操作、创建项目                                                                                                                          |
| Check Out                                                             | Check-out 命令后，在 GPS 应用中检出、带强制检出的导入操作                                                                                                                                              |
| Check In as New                                                       | Check-in 命令中的选项                                                                                                                                                                   |
| Delete                                                                | 删除任何对象（在其被检出且其父级被检出后）                                                                                                                                                             |
| Import                                                                | 导入项目、eBOP 或变量自定义                                                                                                                                                                  |
| Export                                                                | 导出项目、eBOP 或变量自定义                                                                                                                                                                  |
| Open Project（即 Project access，项目访问）                                   | 打开项目，以只读或编辑权限                                                                                                                                                                     |
| View Tab（即 Tab visibility，选项卡可见性）                                     | 查看选项卡所需，否则不显示                                                                                                                                                                     |
| Edit Field of Class（即 Meta data permissions，元数据权限）                    | 编辑（即更改）指定类的任意对象的指定字段的值                                                                                                                                                            |
| Run Query Wizard                                                      | 打开 Query Wizard 应用所需（仅 Process Designer）                                                                                                                                          |
| Restore                                                               | 恢复旧版本成为当前工作版本（即检出的数据）                                                                                                                                                             |
| Define query                                                          | 通过 QueryWizard 或 Gantt 定义查询                                                                                                                                                       |
| Run-query                                                             | 运行已发布的查询                                                                                                                                                                          |
| Transfer ownership                                                    | 使用 AdminConsole 中的所有权转移功能将对象的所有权从一个用户转移到另一个用户                                                                                                                                     |
| Create Configuration                                                  | 以现有配置的副本或新建方式创建 Table View 配置                                                                                                                                                     |
| Configure Alternatives On The Fly                                     | 执行相应命令时（即不使用预定义配置）动态配置 Alternative 或 Master 结构的更新                                                                                                                                 |
| Create Alternative Configuration                                      | 创建用于 UpdateAlternative/Update Master 命令的预定义配置                                                                                                                                     |
| Create Compare Configuration                                          | 创建用于 Compare Viewer 的预定义配置                                                                                                                                                        |
| Application Privileges（应用权限）: Application Administration（应用管理）        | 执行以下操作：创建/更改应用配置和数据（仅通过 client infra）；发布布局（Process Designer）；发布配置（Table View）；更改 eBOP 服务器设置；更改部件方向位图；设置机构的逻辑关节（例如 IPA 设置、Part Direction Bitmap、MLB 设置对话框、2D 草图器管理——包括 seedfile 等） |
| Version Administration（版本管理）                                          | 打开和使用 Version Manager 应用：打开新发布、编辑现有版本属性                                                                                                                                           |
| Data Acquisition（数据采集）                                                | 数据采集                                                                                                                                                                              |
| Data Acquisition Administration（数据采集管理）                               | 导出资源管理器首选项、更改导入设置（Process Designer: Tools > Administration Tools > Import Settings...）                                                                                            |
| System Root Administration + Application Administration（系统根管理 + 应用管理） | 用户需要同时具备这两个权限才能在选项中全局设置 Unite Representation 文件夹                                                                                                                                  |
| Library Administration（库管理）                                           | 使用 Update Library 应用，即触发更新                                                                                                                                                        |
| Robcad Integration（Robcad 集成）                                         | 运行 Robcad 集成并集成 Robcad 单元                                                                                                                                                         |
| Assembly Module（装配模块）                                                 | 从未使用。曾为装配模块准备，但后来装配模块被更新。尽管此权限不再相关，但保留它以避免导入/导出权限功能发生变化                                                                                                                           |
| Notes Authoring（注释编写）                                                 | 创建和修改注释，或阻止特定指定用户使用                                                                                                                                                               |
| Power Bar Search（Power Bar 搜索）                                        | 访问 Power Bar 搜索功能                                                                                                                                                                 |

<a id="v1-s199"></a>

## Customizable Tabs（可自定义选项卡）

Customizable Tab Designer（可自定义选项卡设计器）是一个独立模块，使你无需使用 C++、VB 或 C# 等编程工具即可在 Process Designer 中创建自己的选项卡。可自定义选项卡使你能够像访问核心产品附带的硬编码选项卡一样访问 eMServer 中的数据。

Customizable Tab Designer 应用由两个部分组成：

- Customizable Tab Designer
- 出现在 Process Designer 编辑器中的选项卡

Customizable Tab Designer 是一个编辑器，使你能够定义和格式化自己的自定义选项卡，用于规划应用的 Properties Viewer（属性查看器），无需使用编程工具。这些选项卡对用户而言与任何标准选项卡一样显示。你可以创建任意数量的可自定义选项卡，并将它们分组在 Master Tabs（主选项卡）下。

带有子选项卡的 Master Tab 会显示一个额外的标题栏，允许用户在各个子选项卡之间切换。你可以根据任何有用的标准将选项卡分组在 Master Tabs 下，避免在 Properties Viewer 中造成混乱。规划应用中的 Properties Viewer 每个节点最多支持 16 个 Master Tabs。

创建和设计选项卡后，将它们保存到所选位置，使用注册表编辑器（Registry Editor）设置该位置，并配置选项卡。

Process Designer 将每个自定义选项卡以 .ctc（Customizable Tab Configuration，可自定义选项卡配置）文件的形式、以 XML 格式保存在你选择的位置。

> **注意**
>
> Siemens Digital Industries Software 建议不要使用 XML 编辑器手动编辑 .ctc 文件，因为这可能导致编辑错误，使文件无法使用。

<a id="v1-s200"></a>

### Customizable Tab Designer（可自定义选项卡设计器）

**创建新选项卡：**

**操作步骤**

1. 在桌面上，单击 **Start → Programs → Tecnomatix → Customizable Tab Designer**。Customizable Tab Designer 出现，为空。

   以下表格描述了 Customizable Tab Designer 中的工具栏：
   | 图标                     | 名称                                                                                      | 描述                                                           |
   | ---------------------- | --------------------------------------------------------------------------------------- | ------------------------------------------------------------ |
   |                        | File（文件）                                                                                |                                                              |
   |                        | New（新建）                                                                                 | 开始设计新选项卡                                                     |
   |                        | Open（打开）                                                                                | 打开现有选项卡以进行更新                                                 |
   |                        | Save（保存）                                                                                | 保存当前选项卡                                                      |
   |                        | Save As（另存为）                                                                            | 以你选择的名称和位置保存当前选项卡。功能以 .ctc 格式保存文件                            |
   | Edit（编辑）               | Cut（剪切）                                                                                 | 从 Customizable Tab Designer 设计窗格中移除所选元素并将其置于剪贴板              |
   |                        | Copy（复制）                                                                                | 将所选元素复制到剪贴板                                                  |
   |                        | Paste（粘贴）                                                                               | 将元素从剪贴板粘贴到设计窗格                                               |
   | Properties（属性）         | Properties（属性）                                                                          | 打开所选元素的属性对话框                                                 |
   | Alignment（对齐）          | Align Tops / Align Middles / Align Bottoms / Align Lefts / Align Centers / Align Rights | 对齐所选元素的工具。选择多个对象时，最后选择的对象为参考对象并以白色控点标记。单击对齐图标可将所有所选对象与参考对象对齐 |
   | Dimensions（尺寸）         | Make Width Same Size / Make Height Same Size                                            | 将一个或多个控件的宽度/高度设置为你最后所选控件的大小                                  |
   | Order（顺序）              | Bring to Front / Send to Back                                                           | 将所选元素移到其他元素之前/之后                                             |
   | Tabulator Order（制表符顺序） | Set Tabulator Order                                                                     | 定义按 `<Tab>` 键时处理控件的顺序；每个控件的工具提示显示该顺序                         |
2. 要打开新文件，单击 **File → New** 或单击相应按钮。
3. 从 Controls Toolbar（控件工具栏）中，选择希望用于新选项卡的控件到 Customizable Tab Designer 设计窗格。控件列表及其描述请参阅 Controls（控件）。
4. 单击 Customizable Tab Designer 设计窗格以定位控件。按住鼠标按钮并拖动鼠标以按需调整控件大小。
5. 使用 Editing Toolbar（编辑工具栏）按需格式化新选项卡。
6. 保存新选项卡。这会在你选择的位置创建一个 ctc 文件。
   > **注意**
   >
   > 只有当选项卡位于 Windows 注册表中定义的某个文件夹中时（如下所述），它才会被加载到规划应用中。

**设置 CTC 文件的位置：**

**操作步骤**

1. 在桌面上，单击 **Start > Run** 并输入 `Regedit` 打开注册表编辑器。导航到 `My Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Tecnomatix\eM-Planner\CustomizableTabs\ConfigFolders`。
2. 右键单击注册表编辑器的键区域，并选择 **New > String** 创建新的字符串值。所创建键的名称与你存储 CTC 文件的文件夹名称相同。
3. 将字符串值设置为包含你的 ctc 文件的目录路径。
   > **注意**
   >
   > 如果希望将不同的 ctc 文件存储在不同的位置，可以添加多个位置。
4. 关闭注册表编辑器。

**配置可自定义选项卡：**

**操作步骤**

1. 在 Customizable Tab Designer 中，单击 **Configuration**（配置）。出现 Configuration 对话框。
2. 单击相应按钮。出现 Process Designer Login（登录）对话框。
3. 输入登录名和密码并单击 **OK**（确定）。Configuration 对话框打开，顶部为 Supported Classes（受支持类）选项卡，并填充来自 eMServer 的数据。
4. 左窗格中的 all Classes 层级显示所有可用节点和类，右窗格中的 Supported Classes 列表显示新选项卡相关的那些。你可以如下配置新选项卡相关的节点和类：
   - 双击 all Classes 列表中的类以将其添加到 Supported Classes 列表。
   - 在 all Classes 列表中选择类并单击相应按钮以将其添加到 Supported Classes 列表。
   - 双击 Supported Classes 列表中的类以将其移除。
   - 在 Supported Classes 列表中选择类并单击相应按钮以将其移除。
   > **注意**
   >
   > 左窗格中的 all Classes 层级树显示所有类，当前已配置的那些以粗体标记。
5. 如果希望新选项卡对所选类及其子类相关，请勾选 **Apply to Derived Classes**（应用于派生类）。
6. 单击 **Excluded Classes**（排除类）选项卡，并使用步骤 4 中描述的过程排除新选项卡不相关的类。
7. 如果希望排除所选类及其父类，请勾选 **Apply to Base Classes**（应用于基类）。
8. 单击 **Options**（选项）选项卡。
9. 在 **Master Tab** 区域，从 **Containing Tab**（所属选项卡）下拉列表中选择一个选项卡。这是新选项卡作为子选项卡出现在其下的 Master Tab。
   > **注意**
   >
   > 如果在特定 Master Tab 下仅添加单个新选项卡，则新选项卡作为 Master Tab 出现。Master Tab 采用新选项卡的标题。
10. 单击相应按钮以创建新的 Master Tab。出现 Create new Master Tab（创建新主选项卡）对话框。
11. 在 **Tab Id** 字段中，输入新选项卡的唯名称。
12. 在 **Title**（标题）区域，输入新选项卡的名称（每种支持的语言各一个）。
13. 单击 **OK**（确定）。
14. 要删除 Master Tab，在 Containing Tab 下拉列表中选择要删除的选项卡并单击相应按钮。出现以下消息。
15. 单击 **Yes**（是）。
16. 如果要编辑现有 Master Tab，单击相应按钮。这将打开与 Create New Tab（创建新选项卡）按钮相同的对话框，你可以编辑所选 Master Tab 的属性。
17. 在 Options 选项卡的 **Priority**（优先级）字段中，输入新选项卡的优先级编号。优先级编号越高的选项卡显示越靠前（更靠左）。
18. 单击 **OK**（确定）完成新选项卡的配置。

<a id="v1-s201"></a>

#### Label（标签）

使用 Label（标签）控件设置要显示的固定文本。该控件的 Properties（属性）对话框包含两个选项卡：Text 和 Appearance。Text 选项卡使你能够为所有当前配置的语言设置静态文本，Appearance 选项卡使你能够控制文本的自动调整大小。

要给字符加下划线，请在其前面加上与号（&）字符。例如，输入 `&Application` 会显示 Application。如果要在文本中使用与号字符，请将其重复。例如，输入 `&&Application` 会显示 `&Application`。

<a id="v1-s202"></a>

#### Text Box（文本框）

使用 Text Box（文本框）显示/编辑根节点的特定字段。Text Box 支持以下属性：String、Double、Integer、Float。你可以使用 Text Box Properties 对话框，通过单击 Browse（浏览）打开 Field Selector（字段选择器）对话框来设置控件所连接的字段。你可以设置小数位数（仅与 double 或 float 属性相关），这将覆盖全局单位设置；保持此值不变则应用全局单位设置。你还可以选择将字段设为只读或可编辑，以及字段是否为 Multiline（多行，用于较长的描述性字符串属性）。未选择 Multiline 选项时，文本框控件只能在 y 方向调整大小。

你可以使用 Appearance（外观）选项卡：定义文本框的颜色（如果非只读）；定义文本对齐方式。

**Field Selector（字段选择器）**：包含两个选项卡。第一个选项卡显示通用字段（即选项卡相关的所有类共有的字段）。第二个选项卡使你能够为特定子类选择附加字段，相关类与其层级一起显示。

<a id="v1-s203"></a>

#### Calculated Value Box（计算值框）

该控件显示根节点两个字段之间简单计算的结果，例如 `allocatedTime * cost`。通过单击 Browse 打开 Field Selector 对话框选择字段。两个字段可通过以下操作之一组合：加法（Sum）、减法（Difference）、乘法（Product）或除法（Division）。

<a id="v1-s204"></a>

#### Calculation Box（计算框）

该控件允许你基于选项卡根节点的任意数量字段执行复杂计算。Calculation Box 在运行时于只读字段中显示结果。要执行计算，你必须指定：

- 计算中使用的字段
- 计算本身的 Java 脚本定义

你可以使用任何与 Java 脚本代码兼容的方式执行计算。确保格式字符串符合 C# 标准。更多信息请参阅 MSDN 帮助（搜索“Standard Numeric Format Strings”）。

<a id="v1-s205"></a>

#### Check Box（复选框）

使用 Check Box（复选框）将整数字段的状态切换为 0/1。未选中时字段值为 0，选中时为 1。使用 Check Box Properties 对话框设置属性、是否只读，并输入显示在复选框旁边的文本。

你可以使用 Appearance 选项卡控制文本位置。要加下划线，在字符前加 &（例如 `&Application`，下划线字符成为该复选框的热键，按 ALT+A 激活）。要使用与号，请重复输入 `&&Application`。

<a id="v1-s206"></a>

#### Combo Box（组合框）

使用 Combo Box（组合框）使用户能够在备选项之间选择。在 Combo Box Properties 对话框中，你可以设置组合框所连接的字段以及是否写保护。你还可以提供运行时组合框中显示、供用户选择的值。文本内容不可编辑。你可以在 **Field Value**（字段值）列中定义要写入数据库的值，在 **Language**（语言）列中定义显示给用户的显示名称。

<a id="v1-s207"></a>

#### Group Box（分组框）

使用 Group Box（分组框）将其他控件分组在一起，使用户易于理解它们之间的联系。在选项卡中放置 Group Box 并将其他控件放入其中，控件会随 Group Box 一起移动。在 Group Box Properties 对话框中，你可以输入 Group Box 标签的文本。

<a id="v1-s208"></a>

#### Date Control（日期控件）

Date Control（日期控件）使你能够使用日历控件设置日期属性。在 Date Control Properties 对话框中设置属性（仅日期字段）并确定日期是否为只读。

<a id="v1-s209"></a>

#### Milestone（里程碑）

Milestone（里程碑）控件是结合了复选框的日期字段。通过勾选框设置日期；要重置日期，清除复选框并再次勾选。在 Milestone Properties 对话框中，你可以设置字段（仅日期字段）并确定里程碑是否为只读。

<a id="v1-s210"></a>

#### Delete Button（删除按钮）

该控件触发删除事件。激活时，它向所连接的控件发送删除信号，该控件随后删除其内容。

删除功能目前仅对 External Document（外部文档）控件有效，且必须在 Customizable Tab Designer 中定义此控件。在 Delete 按钮的 **Affected Control**（受影响控件）字段中输入 ID。鼠标光标悬停在该控件上时，系统会显示控件名称。

<a id="v1-s211"></a>

#### Usage Box（用法框）

Usage Box（用法框）控件使你能够通过从库中选择对象来创建用法（usages）。用法是数据库连接操作和资源的数据库对象，反之亦然。

将此控件添加到选项卡时，应指定包含相关库的父类的字段。这必须是一个字符串字段，在运行时当 Usage Box 查找指定类的第一个父级时包含相关库对象的外部 ID。Usage Box 采用此父级定义字段中指定的外部 ID。你还应指定要连接到哪种类型的对象。Usage Box 搜索找到的库并显示指定类的所有对象，供用户选择其中之一。你还可以输入要在对话框中显示的描述，说明将出现哪些对象的选择。

<a id="v1-s212"></a>

#### External Document Box（外部文档框）

External Document Box（外部文档框）显示链接到选项卡根对象的外部文档的路径。所显示的关系必须是 ExternalDocument 或其派生类到根节点的 1:1 关系。该字段以只读方式显示路径。值只能通过 Delete 按钮或 File Dialog Button 更改。使用 Properties 指定连接到控件的字段。“Field value cannot be changed”和“Multiline”参数为遗留值，对结果无影响。

<a id="v1-s213"></a>

#### Relation Box（关系框）

Relation Box（关系框）类似于上述 Usage Box，但作用于将对象链接到另一个对象的关系字段，而不是通过用法对象连接操作与资源。Relation Box 以与 Usage Box 相同的方式定位相关库，并允许你指定要创建的关系，而不是始终使用用法对象创建链接。因此，Relation Box 在 Properties 对话框中包含一个附加字段，用于指定一个字符串字段，使用户能够在运行时输入所选对象的外部 ID。结果，使用 Relation Box 创建的关系不是真正的过程模型关系，而是使用字符串字段和外部 ID 创建的单向链接。

<a id="v1-s214"></a>

#### File Dialog Button（文件对话框按钮）

该按钮在运行时按下时，会打开一个对话框，你可以在其中选择文件。单击 OK 会向选项卡上支持特定接口的第一个控件发送事件。目前仅 External Document Box 支持此功能，因此单击 OK 会将所选文件路径的事件发送到选项卡上的第一个 External Document Box。使用 Properties 指定按钮上显示的文本以及希望在对话框中显示的文件类型筛选器。筛选器包含文件扩展名和将出现在选择对话框组合框中的文本字符串。

<a id="v1-s215"></a>

#### Object Drop Box（对象拖放框）

Object Drop Box（对象拖放框）允许你通过简单地将所选对象（例如从树中）拖放到控件上来创建对象之间的链接。该控件可作用于单引用字段、字符串字段（这种情况下将外部 ID 输入字段）以及 PfShortcut 类型的字段。控件显示对象及其对象类型的图标。可以通过按键盘上的 Delete 移除链接。在 Properties 中，你必须指定控件所操作的字段。如果将控件标记为只读，你将无法将对象拖放到其上，只能显示是否已连接对象。

<a id="v1-s216"></a>

#### Radio Button（单选按钮）

该控件显示一个连接到单值字段的单选按钮。你可以在一个选项卡上使用多个单选按钮为某个属性配置互斥的选项。

<a id="v1-s217"></a>

#### Vector List View（矢量列表视图）

Vector List View（矢量列表视图）允许你在选项卡上显示矢量属性。使用 Properties 设置要显示的属性，并为相应列分配标题。

<a id="v1-s218"></a>

### Customizing Tabs for Instances of Specific Prototypes（为特定原型的实例自定义选项卡）

除了仅为通用工具或部件实例设计自定义选项卡外，用户还可以为特定原型的实例定义自定义选项卡。通过为实例提供的唯一自定义选项卡，用户可以为 conveyor（传送带）实例和 fixture（夹具）实例各创建一个不同的选项卡，并将相应原型的参数添加到选项卡中。

Configuration 对话框将受支持的原型类列为相应实例类的子级。

1. 在实例下选择一个原型。该选项卡仅对所选原型的实例显示。
   > **注意**
   >
   > 对于排除的类，对话框仅列出具有子级的特定原型实例。
2. 定义希望在选项卡激活时显示的字段。除实例字段外，这些还可以包括当你为特定原型实例指定选项卡时的特定原型字段。
3. 在 Field Selector 对话框中，使用“Select from class list”（从类列表选择）选项卡选择原型和实例字段。
   > **注意**
   >
   > 从 Common Fields（通用字段）选项卡选择字段仅选择实例字段。
   系统将特定原型字段中的数据更改以及实例字段中的更改一并存储。

<a id="v1-s219"></a>

### Customized Tabs in the Planning Applications（规划应用中的自定义选项卡）

定义自定义选项卡后，它们出现在规划应用中。

自定义选项卡的定义存储在注册表中的 `HKEY_LOCAL_MACHINE\SOFTWARE\Tecnomatix\eM-Planner\CustomizableTabs\ConfigFolder`。默认情况下，安装程序将此位置设置为 `<systemRoot>\CustomizableTabs`。可自定义选项卡设计器默认也将创建的文件存储在同一位置，你无需移动 ctc 文件。如果文件未保存到配置文件夹，请将它们移动或复制过去。

在规划应用中，你可以定义最多 16 个可自定义选项卡。它们可以是 Master Tabs，也可以是 Master Tabs 的子选项卡，如下图所示。

<a id="v1-s220"></a>

## Tab Order Manager（选项卡顺序管理器）

<a id="v1-s221"></a>

### Customizing Properties View Tab Order（自定义属性视图选项卡顺序）

Tab Order Manager（选项卡顺序管理器）允许你自定义所选对象 Properties View（属性视图）中选项卡出现的顺序。此功能使你能够及时访问最重要的信息。由于自定义中包含的每个对象的属性可能不同，选项卡顺序可以按对象设置。

只有管理员或属于管理员组的任何用户才能设置选项卡顺序。要定义新的选项卡顺序，只能使用已安装的、可通过 Tab Order Manager 重新排列的选项卡。任何节点类型只能定义一个选项卡顺序。一旦更改，新顺序将替换当前顺序，并对所有用户可用。

选项卡顺序可以导入和导出到/从 xml 文件，并应符合以下结构：

```xml
<ApplicationData>
<Application Name="TnxEmpApp_TabOrderMAnager">
<SubEntry Name="Attachments">
<Key Name="PmNode_SC" Type="System.Double" IsArray="false">
<Value>370</Value>
</Key>
<Key Name="TabName" Type="System.String" IsArray="false">
<Value>Attachments</Value>
</Key>
<Key Name="class PmCriterionValue_AP" Type="System.Double" IsArray="false">
<Value>10190</Value>
</Key>
</SubEntry>
</Application>
</ApplicationData>
```

<a id="v1-s222"></a>

### Setting the Tab Order（设置选项卡顺序）

应用必须运行在包含所有现有选项卡的客户端计算机上。你可以维护未被注册但之前由应用添加的选项卡。启动应用时必须登录。

该应用也适用于 SSO 环境。没有管理员权限的用户无法使用该应用。应用运行后，首先从 TabsConfiguration.xml 文件和注册表中读取有关现有选项卡的信息，然后从数据库读取当前保存的选项卡顺序。对于数据库中未定义顺序的任何选项卡，使用默认选项卡顺序。如果没有任何关于 ActiveX 选项卡的信息，将显示警告消息，指示应用未在客户端计算机上运行，而是在服务器计算机上运行。

<a id="v1-s223"></a>

### Tab Order Manager Main Window（选项卡顺序管理器主窗口）

选项卡顺序通过 Tab Order Manager 窗口自定义。

屏幕左侧是一个显示可用类的窗格。通过从树中进行选择，显示每个类的选项卡顺序。所执行的操作仅适用于所选类。窗口底部显示一个选项卡条带（tab strip）。该条带不包含选项卡本身，而是当前顺序的预览。选项卡条带仅显示所选类的顺序。窗口右侧有一组按钮，允许你操作选项卡条带。

| 按钮                        | 描述                                                  |
| ------------------------- | --------------------------------------------------- |
| Move Up（上移）               | 将所选选项卡向左移动到更靠近第一个选项卡。只要所选选项卡不是最左侧选项卡，此按钮就可用         |
| Move Down（下移）             | 将所选选项卡向右移动到更靠近最后一个选项卡。只要所选选项卡不是最右侧选项卡，此按钮就可用        |
| Inherit（继承）               | 显示弹出对话框，让你选择是将所选继承应用于所选选项卡，还是应用于当前类中所有在该类中被重新定义的选项卡 |
| Reset Tab Order（重置选项卡顺序）  | 重置当前类的完整选项卡顺序，使所有选项卡从更高级别或默认顺序获取其顺序                 |
| DeleteTab（删除选项卡）          | 从选项卡顺序中移除所选选项卡。该选项卡从所有类的选项卡顺序中移除。移除前会要求用户确认         |
| Clear Tab Order（清除选项卡顺序）  | 从当前 schema 的应用数据中移除完整的 Tab Order 部分                 |
| OK（确定）                    | 关闭应用并将选项卡顺序保存到数据库                                   |
| Cancel（取消）                | 关闭应用而不将选项卡顺序保存到数据库                                  |
| Apply（应用）                 | 通常在使用 OK 按钮退出应用时保存选项卡顺序。如果要执行中间保存，请使用此按钮            |
| Export（导出）                | 允许将当前选项卡顺序导出到 xml 文件。打开标准文件选择对话框                    |
| Import Tab Order（导入选项卡顺序） | 允许将选项卡顺序导入到应用。选择文件后，应用中的当前顺序从文件导入                   |

> **注意**
>
> 管理员只能影响选项卡的顺序，而不能影响它们是否显示。删除是指删除不再安装的前已安装选项卡的条目。仅当所选选项卡标记为红色时（参见“Color Markings”），此按钮才激活。如果数据库中某个当前未安装在该机器上的选项卡存在条目，就会发生这种情况。

**Color Markings（颜色标记）**

为了更好地查看选项卡条带和选项卡列表，选项卡标题以三种不同颜色着色：

- **黑色**——此选项卡的位置直接来自该类
- **蓝色**——此选项卡的位置来自更高级别的类
- **红色**——此选项卡当前未注册

<a id="v1-s224"></a>

### Moving Tabs（移动选项卡）

你可以一次移动当前所选类的选项卡顺序。通过选择选项卡并单击 Move Up 或 Move Down 按钮，选项卡向左或向右移动。选项卡保持选中状态直到选择另一个选项卡，因此只需后续单击即可将选项卡移动多个位置。

移动选项卡始终对层级中的所有派生类产生影响。如果选项卡在当前类中为蓝色（其位置来自更高级别的类），选项卡将变为黑色，因为选项卡现在取决于类本身的定义。如果选项卡为红色（缺少有关受支持类的信息），它们将始终仅显示在数据库中选项卡顺序出现的类中。如果移除红色选项卡，它将从所有选项卡顺序中完全移除。如果移动红色选项卡，它仅在当前类中移动。如果选项卡再次注册，则可能影响派生类。

<a id="v1-s225"></a>

## Load Entity Level（加载实体级别）

Load Entity Level（加载实体级别）命令使用户能够在详细表示（detailed representation）中加载组件，并在各种定义中使用实体。实体的实例信息存储在研究级别（study level）的工程数据中（类似于组件和链接）。

你可以为任何直接加载于 United Representation（统一表示）中的 JT、CO 或 COJT 组件（包括复合原型的实例）切换到实体级别表示。仅当 .co 组件同时包含 United 和 Detailed 表示时，才能将其切换到实体级别。

> **注意**
>
> 在实体级别工作可能会影响内存消耗和性能。
>
> 默认情况下，所有组件都加载在 United Representation 中。Robcad 超级组件的 CO 组件无法在实体级别加载。

**在实体级别加载组件：**

从 Customization（自定义）对话框中，为以下类型的组件选择 Load Entity Level 命令：

- Part 和 Tool 实例，包括复合原型的实例
- Part 和 Tool 原型分配
- 非空的复合部件（Compound Parts）或复合资源（Compound Resources）

运行命令后，所选组件的所有实体被加载以用于图形查看器。

<a id="v1-s226"></a>

## Unload Entity Level（卸载实体级别）

在实体级别加载组件后，用户可以将其重新加载回 United Representation。

> **注意**
>
> 默认情况下，所有组件都加载在 United Representation 中。

**从实体级别重新加载到统一表示：**

从 Customization 对话框中，为以下类型的组件选择 Unload Entity Level 命令：

- Part 和 Tool 实例，包括复合原型的实例
- Part 和 Tool 原型分配
- 非空的复合部件或复合资源

使用 Unload Entity Level 会影响 Part 和 Tool 实例以及原型分配的单个或多个选择中的所有所选实体级别对象，并影响所选复合原型的整个层级。

<a id="v1-s227"></a>

## Merge Sources（合并源点）

Merge Sources（合并源点）命令使你能够将单个原型的全部部件实例收集到单个源缓冲区流（source buffer flow）中，前提是这些部件位于全部连接到同一操作、且流的另一侧全部为源点的流上。参与命令的所有对象必须已检出。

**操作步骤**

1. 选择操作。
2. 单击相应按钮。出现 Merge Sources 对话框。
3. 如果希望合并所选操作及其所有嵌套操作，请设置 **Merge with Hierarchy**（随层级合并）。
4. 单击 **OK**（确定）。Process Designer 合并所选操作。
   > **注意**
   >
   > 另请参阅 PERT Chart Elements（PERT 图元素）中的 Folder 组件。

<a id="v1-s228"></a>

## Reconcile Instances（协调实例）

当从 Resource Prototype（资源原型）库修改原型时（例如，通过向 Equipment 原型添加原型实例或从中删除），原型与其在导航树中的实例之间存在差异，如下图所示。在右窗格的 Prototype 库中，Tp1 实例被添加到 EqProto3 设备原型。此更改未反映在左窗格中 EqProto3 的实例中。

**协调（同步）导航树的所有实例：**

- 单击相应按钮（从 Customization 对话框访问此命令图标）。下图显示导航树实例现已同步。

<a id="v1-s229"></a>

## End Items（最终项）

<a id="v1-s230"></a>

### Creating and Managing End Items（创建和管理最终项）

End Items（最终项）选项包含用于创建和管理最终项的子选项，最终项是被组合成单个项的装配。创建最终项通过简化在 Process Designer 中作为单个对象处理的装配的管理（例如由第三方制造的汽车座椅），大大提高了 Graphic Viewer 中的性能。

End Items 选项包含以下子选项：

| 子选项                                      | 描述                                                             |
| ---------------------------------------- | -------------------------------------------------------------- |
| Show Product Updates（显示产品更新）             | 将所选最终项与该最终项列出的各个部件进行比较。更多详情请参阅 Show Product Updates            |
| Retrieve Detailed Representation（检索详细表示） | 为当前会话加载各个组件部件，而不是统一表示。更多详情请参阅 Retrieve Detailed Representation |

<a id="v1-s231"></a>

### Show Product Updates（显示产品更新）

Show Product Updates（显示产品更新）选项将所选最终项与该最终项列出的各个部件进行比较。如果检测到最终项的任何更改，Process Designer 中的图形表示将重新创建。

**检查最终项中的更新：**

**操作步骤**

1. 从 Product Tree（产品树）或 Graphic Viewer 中选择一个最终项。
2. 选择 **Show Product Updates**。所选最终项与各个部件的列表进行比较。

<a id="v1-s232"></a>

### Retrieve Detailed Representation（检索详细表示）

Retrieve Detailed Representation（检索详细表示）选项加载各个组件部件，而不是统一表示。使用此命令仅影响当前会话，不会以任何方式更改底层数据。执行最终项的自动分解（explode），使你能够对属于最终项的部件执行操作。

> **注意**
>
> 当统一项已被检出，且统一对象的某个部件被另一应用中的用户需要时，会执行自动分解。如果对象已检入（checked in）但未被检出（checked out）且已被另一用户检出，则会执行自动最终项分解。

屏幕上最终项的表示会更改以反映分解。这在任何最终项被多个对象共享的情况下发生。

**检索最终项的详细表示：**

**操作步骤**

1. 从 Product Tree 或 Graphic Viewer 中选择一个最终项。
2. 选择 **Retrieve Detailed Representation**。各个组件部件被加载以替代统一表示。

<a id="v1-s233"></a>

## Query Color Wizard（查询颜色向导）

<a id="v1-s234"></a>

### Working with Color Indications（使用颜色指示）

Query Color Wizard（查询颜色向导）命令使你能够定义新的颜色指示并编辑现有的颜色指示。颜色指示（color indication）是根据一组定义的条件对树中对象进行的着色。

<a id="v1-s235"></a>

### Define a New Color Indication（定义新的颜色指示）

**操作步骤**

1. 选择 **Query Color Wizard**。Query Color Wizard 打开到 Color Indications（颜色指示）页面。
2. 选择 **Define new Color Indication**（定义新的颜色指示）。
3. 在 **Name** 文本框中，输入新颜色指示的名称。
4. 单击 **Next**（下一步）。打开 Object Types（对象类型）页面。
5. 在类树中，勾选希望包含在颜色指示中的对象类型。单击相应按钮以勾选所有对象类型，单击相应按钮以取消勾选所有对象类型。
6. 单击 **Next**（下一步）。打开 Condition Definition（条件定义）页面。
7. 对于希望指示包含的每个条件，向 Indication Colors（指示颜色）列表添加指示颜色。因此，当你激活指示时，满足你在下一步中定义条件的对象将以该颜色指示。要添加指示颜色，单击 **Add**（添加）。打开 Add Color Indication 对话框。
8. 从 Indication color 文本框中，选择一种颜色。
9. 在 Object Types 区域，执行以下操作之一：
   - 选择 **all**（全部），如果你希望与此颜色关联的条件对你在该向导 Object Types 页面中选择的所有类型的对象进行指示。
   - 选择 **Selected Types**（选定类型），如果你希望与此颜色关联的条件仅对特定类型的对象进行指示。选择特定对象类型。
10. 单击 **OK**（确定）。指示颜色被添加到 Condition Definition 页面的 Indication Colors 列表。你可以通过选择颜色并单击 **Remove**（移除）来移除指示颜色。
11. 选择你添加的指示颜色，并按下表完成 Color Condition（颜色条件）定义，以创建由该颜色指示的条件。要在每个单元格中输入值，单击箭头并从下拉列表中选择值。

| 列（Column）      | 描述（Description）                                                                        |
| -------------- | -------------------------------------------------------------------------------------- |
| AND/OR         | 设置上一行与当前行之间的逻辑条件。不适用于第一行                                                               |
| Attribute（属性）  | 设置要设置条件的属性。如果选择实例，也会显示原型的属性。如果将 Object Type 设置为 All，可用属性是分配给所选颜色的各对象类型共有的属性            |
| Condition（条件）  | 与 1st Value 和 2nd Value 列结合，对 Attribute 列中所选属性设置条件。可从下拉列表中选择条件，取决于 Attribute 列中选择的属性类型 |
| 1st Value（第一值） | 设置数值以完成 Condition 列中指定的条件                                                              |
| 2nd Value（第二值） | 如果指定的条件是“between（介于）”，设置第二个数值以完成条件；否则查询不考虑第二值                                          |

对于字符串属性，可用条件为“equal（等于）”和“not equal（不等于）”。对于数值属性，可用条件为“equal”、“not equal”、“between”、“bigger than（大于）”、“smaller than（小于）”和“contains（包含）”。

1. 重复步骤 7 到 11，直到定义完希望指示包含的所有条件。要移除指示颜色及其关联条件，单击 **Remove**。
2. 使用箭头排列指示颜色的优先级顺序。优先级确定对满足多个条件的对象使用哪种颜色。列表顶部的颜色优先级最高。

> **注意**
>
> 一旦为特定类定义了颜色条件，该类将在所有祖先类条件中被忽略。此行为有一个例外：当该类条件中指定的一个或多个字段与祖先类条件的字段相同时，则（也）会考虑祖先条件。

1. 完成指示颜色及其关联条件的定义后，单击 **Next**（下一步）。打开 Color Indication Options（颜色指示选项）页面。
2. 如果希望颜色指示在所有树查看器中可用于激活，请选择 **Make the color indication available in all relevant viewers**（使颜色指示在所有相关查看器中可用）。
3. 如果希望颜色指示仅在特定查看器中可用于激活，请选择 **Include the color indication only in the following viewers**（仅将颜色指示包含在以下查看器中）并勾选要包含的查看器。
4. 单击 **Finish**（完成）。新颜色指示被添加到 Color Indication 下拉列表，并可在任何所选查看器中激活。有关激活颜色指示的信息，请参阅 Using Color Indication Queries。

<a id="v1-s236"></a>

### Edit an Existing Color Indication（编辑现有的颜色指示）

**操作步骤**

1. 选择 **Query Color Wizard**。Query Color Wizard 打开到 Color Indications 页面。
2. 选择 **Edit existing Color Indication**（编辑现有的颜色指示）。
3. 选择希望编辑的颜色指示。
4. 如果希望重命名所选颜色指示，单击 **Rename**（重命名）并输入新名称。
5. 如果希望删除所选颜色指示，单击 **Remove**（移除）。
6. 如果对现有颜色指示没有进一步更改，单击 **Finish**（完成）。否则，单击 **Next**（下一步）。
7. 在类树中，勾选要包含在颜色指示中的对象类型。取消勾选要从指示中移除的任何对象类型。
8. 单击 **Next**（下一步）。打开 Condition Definition 页面。
9. 通过选择颜色并单击 **Remove** 来移除任何不需要的指示颜色及其关联条件。
10. 对于每个希望包含的新条件，向 Indication Colors 列表添加指示颜色。
11. 从 Indication color 文本框中，选择一种颜色。
12. 在 Object Types 区域，执行以下操作之一（与定义时相同）：选择 all 或 Selected Types。
13. 单击 **OK**（确定）。新指示颜色被添加到 Condition Definition 页面的 Indication Colors 列表。
14. 选择你添加的指示颜色，并完成 Color Condition 定义（参见上文表格）。
15. 重复步骤 7 到 11，直到定义完希望包含的所有新条件。
16. 要修改任何现有条件，在 Indication Colors 列表中选择其关联颜色并编辑下面的条件定义。
17. 使用箭头排列指示颜色的优先级顺序（顶部优先级最高）。
18. 完成修改指示颜色及其条件后，单击 **Next**（下一步）。打开 Color Indication Options 页面。
19. 如果希望在所有树查看器中可用，选择 Make the color indication available in all relevant viewers。
20. 如果仅在特定查看器中可用，选择 Include the color indication only in the following viewers 并勾选查看器。
21. 单击 **Finish**（完成）。更改被保存。编辑后的颜色指示在 Color Indication 下拉列表中可用，并可在任何所选查看器中激活。

<a id="v1-s237"></a>

### Using Color Indication Queries（使用颜色指示查询）

颜色指示是根据一组定义的条件对树中对象进行的着色。你可以一次激活一个颜色指示，使用以下各树中的 Color Indication 命令：Product、Resource、Operations 和 Mfg。

有一个预定义的颜色指示：Assignment Indication（分配指示）。它以绿色显示所选树中已分配的项，以红色显示未分配的项。如果复合节点被分配，其所有子节点自动被分配。当复合节点被取消分配时，每个子节点的分配状态单独确定。

你可以使用 Query Color Wizard 命令自定义定义颜色指示。颜色指示根据 eMServer 中数据的更改在线更新。

<a id="v1-s238"></a>

### Activate a Color Indication（激活颜色指示）

Color Indication 按钮有一个箭头，可打开可用颜色指示的列表。

**操作步骤**

1. 单击 Color Indication 按钮上的箭头。打开 Color Indication 下拉列表。
2. 下拉列表显示当前树查看器中可用的所有颜色指示。这包括用户定义的颜色指示和预定义的 Assignment Indication。选择希望激活的颜色指示。你选择的颜色指示在列表中被勾选并激活。Product Tree 和 Resource Tree 还拥有 Graphic View Mode（图形视图模式）按钮。
3. 单击相应按钮，以根据活动颜色指示对 Graphic Viewer 中的对象着色。

<a id="v1-s239"></a>

### Activating the Selected Color Indication（激活所选颜色指示）

如果希望使用的颜色指示已在 Color Indication 下拉列表中选中，可以通过单击 Color Indication 按钮激活它。

单击相应按钮以激活所选颜色指示：

- 所选颜色指示被激活。
- 树中的节点根据已激活颜色指示的条件着色。
- 活动颜色指示的名称显示在颜色指示工具栏中。

> **注意**
>
> 该图标在 Product Tree 和 Resource Tree 中独立工作。例如，如果在 Product Tree 中激活了 Graphic View Mode，且 Graphic Viewer 根据所选颜色查询显示颜色，将 Process Designer 的焦点移动到未激活 Graphic View Mode 的 Resource Tree 会导致 Graphic Viewer 取消颜色指示。
>
> 如果选择了分配指示或查询指示，单击相应图标会将 Graphic Viewer 中相应对象的颜色显示切换为与树中相同的颜色。

<a id="v1-s240"></a>

## Neighboring Search（相邻搜索）

Neighboring Search（相邻搜索）选项使你能够在 Graphic Viewer 中围绕所选对象显示包围盒（bounding box），然后搜索所有包围盒与包围盒体积的任何部分相交的部件和资源。你还可以选择多个对象，并搜索穿过组合对象包围盒体积的所有部件。搜索不限于当前显示的部件。

显示在 Graphic Viewer 中的包围盒的大小可以手动调整（仅用于查看目的），以扩大或缩小相邻部件的搜索范围。每个对象的原始包围盒参数是固定的，无法更改。

相邻搜索的范围可以涵盖 station 过程，包括所添加 station 中的所有部件。

> **注意**
>
> 在执行相邻搜索之前，必须确保已为所需 Resource 或 Product Tree 中的每个部件计算包围盒（更多信息请参阅 Bounding Box Calculation）。如果包围盒未正确定义，搜索可能不会成功。例如，如果根节点没有包围盒，它就无法与绘制的包围盒相交。因此，无需搜索树的其余部分，因为没有相邻对象。

**执行相邻搜索：**

**操作步骤**

1. 在 Graphic Viewer 中选择所需对象，然后选择 **Neighboring Search**。所选对象周围出现黄色包围盒，Neighboring Search 窗口也显示在 Graphic Viewer 中。
2. 单击 **Define Scope**（定义范围）。出现 Define Scope 窗口。
3. 选择要搜索的复合部件/资源和/或 station，并单击 **OK**（确定）。
4. 如果需要，按如下方式更改搜索参数：
   | 选项                         | 描述                                                                                                                   |
   | -------------------------- | -------------------------------------------------------------------------------------------------------------------- |
   | Boundary Box Search（边界框搜索） | 通过更改 X、Y、Z 参数和框的中心，使你能够更改包围盒的大小和位置。单击数字然后输入所需值，或使用上下箭头。如果需要，也可以输入负值。或使用双滑块调整大小。注意：无论对象方向如何，包围盒在 eMServer 中平行于世界坐标系创建 |
   | Proximity Search（邻近搜索）     | 使你能够搜索包围盒邻近的对象。此字段值为 0 时搜索接触或位于包围盒内部的对象。更大的值将指定数量（以默认测量单位）添加到搜索体积                                                    |
   | Reset（重置）                  | 将所有包围盒值重置为打开窗口时的原始值，并移除当前会话中添加的对象，仅保留上次打开窗口时（在当前更改之前）加载的对象                                                           |
5. 单击 **Load**（加载）。所有包围盒与所定义包围盒体积的任何部分相交的部件与最初选择的对象一起显示在 Graphic Viewer 中。
   > **注意**
   >
   > 你在 Process Designer 中对包围盒参数所做的任何更改仅用于查看目的，不影响对象的原始参数。
   随着你在 Graphic Viewer 中单击每个对象，该对象在其树中高亮，对象名称显示在 Process Designer 应用窗口的左下角。
6. 单击 **Close**（关闭）或相应按钮以关闭 Neighboring Search 窗口。黄色包围盒也从 Graphic Viewer 中移除。

<a id="v1-s241"></a>

## Customize（自定义）

你可以自定义键盘快捷键、功能区（ribbon）、快速访问工具栏和上下文菜单，以适应你的组织和/或工作环境。你可以使用安装应用时提供的出厂工具设置，在默认环境中工作，或者根据需要添加或删除选项来自定义特定于你个人需求的工具。

> **注意**
>
> 随时单击 **Reset**（重置）以取消所有自定义并返回出厂设置。
>
> Customize（自定义）对话框包含用于以下自定义的选项卡：
>
> - Customizing Keyboard Shortcuts（自定义键盘快捷键）
> - Customize the Mouse（自定义鼠标）
> - Customize the Ribbon（自定义功能区）
> - Customize the Quick Access Toolbar（自定义快速访问工具栏）
>
> 可选地，你可以执行以下任何操作：
>
> - 右键单击功能区并选择 **Minimize the Ribbon**（最小化功能区）以隐藏功能区。再次执行此操作会显示功能区。
> - 默认情况下，右面板 Customize the Ribbon 列表中列出的所有功能区选项卡均被勾选，因此会显示。你可以清除任何希望隐藏的选项卡对应的复选框。
> - 右键单击任何选项卡名称并选择 **Set As Toolbar**（设为工具栏）。所选选项卡从功能区移除并显示在浮动窗口中。例如，如果你希望在一个监视器上显示功能按钮而在另一个监视器上工作，这很有用。
> - 要取消 Set As Toolbar 并将选项卡返回功能区，请单击工具栏右上角的相应按钮，或在 Customize 对话框的 Customize the Ribbon 选项卡中勾选相关复选框。
> - 单击快速访问工具栏右端的箭头可执行以下操作：启动快速访问工具栏上的任何命令；通过单击 **More Commands** 自定义快速访问工具栏（请参阅 Customize Quick Access）；在快速访问工具栏下方或上方显示快速访问工具栏；通过选择 **Minimize the Ribbon** 隐藏功能区；在浮动窗口中显示快速访问工具栏。

<a id="v1-s242"></a>

### Customize the keyboard（自定义键盘）

你可以分配一个键或键的组合，在所按键时执行特定功能。你还可以为已分配键盘快捷键的功能分配额外的键，或者用你选择的快捷键替换已分配的键。

**操作步骤**

1. 在 Customize 对话框中选择 **Customize Keyboard**（自定义键盘）选项卡。
2. 在左面板中选择一个类别（Category）以在右面板中显示其命令（Commands）。
3. 在 Commands 面板中，选择要自定义快捷键的命令。选择命令时，Description（描述）字段显示该命令的简要描述，Current Keys（当前键）字段显示此命令任何先前分配的快捷键。
4. 单击 **Press new shortcut key**（按下新快捷键）字段，并按下你希望分配为命令快捷键的键盘按键组合。例如，你可能希望将 `<Ctrl+Alt+Insert>` 键分配给 Shaded Mode（着色模式）选项命令。
5. 如果需要，你可以启用 **Use this shortcut even when a dialog is active**（即使对话框处于活动状态也使用此快捷键）选项。
6. 单击 **Assign**（分配）。按键现在被分配来执行所选命令。

   如果所需的键组合已分配给另一个命令，会显示一条消息，说明该键组合之前分配给了哪个命令。完成该过程将覆盖先前的分配。

你可以通过以下方式从 Customize Keyboard 选项卡修改键盘快捷键：

- 通过在 Commands 面板中选择键盘快捷键名称并单击 **Remove**（移除）来移除键盘快捷键。
- 通过单击 **Reset All**（全部重置）将所有键盘快捷键恢复为默认设置。

<a id="v1-s243"></a>

### Customizing the mouse（自定义鼠标）

Tecnomatix 应用程序中的默认鼠标行为类似于 NX 应用程序。要显示默认鼠标功能的图形表示，请右键单击功能区并选择 **Customize the Ribbon** 以打开 Customize 对话框。选择 **Customize Mouse**（自定义鼠标）选项卡以选择三种配置之一：

- **Default**（默认）：提供常用鼠标行为，类似于 NX 应用程序。这是一种固定配置，所有参数均为只读。
- **Legacy**（传统）：提供类似于早期 Tecnomatix 应用程序版本的鼠标行为。这是一种固定配置，所有参数均为只读。
- **Custom**（自定义）：允许根据需求自定义每个按钮（起点为 Default 自定义）。你的自定义设置在对所有后续工作会话保持有效，直到你修改它们。在迁移到下一软件版本后，你也可以快速应用相同的自定义。

你可以自定义鼠标行为以适应工作习惯。每个鼠标按钮都可以自定义为在单击或按住并拖动鼠标时执行某个动作。鼠标按钮与滚轮的各种组合，连同 Shift、Alt 和 Control 按钮，为在图形查看器中定义常用动作的快捷键提供了灵活性。

<a id="v1-s244"></a>

#### To customize mouse buttons（自定义鼠标按钮）

> **注意**
>
> 某些按钮组合无法自定义，因为它们被保留用于特定动作。例如，单击 MB3（鼠标右键）被保留用于打开上下文菜单，按住 MB1 拖动被保留用于图形查看器中的框选。

**操作步骤**

1. 右键单击功能区并选择 **Customize the Ribbon** 以打开上面显示的 Customize 对话框。
2. 单击 **Customize Mouse** 选项卡。
3. 如果选择了 **Custom**，可以在对话框中配置以下参数组：
   > **注意**
   >
   > **Mouse Map**（鼠标映射）：Customize 对话框右侧的该图定义了鼠标按钮名称。以下参数与这些按钮名称相关。
   >
   > - **Mouse drag**（鼠标拖动）：配置在拖动鼠标并按下键盘控制按钮时执行的操作。
   > - **Mouse wheel**（鼠标滚轮）：配置在滚动鼠标滚轮时按下键盘控制按钮所执行的操作。
   > - **Mouse click**（鼠标单击）：配置单击鼠标中键（滚轮）时执行的操作。
   > - **Mouse + button drag**（鼠标 + 按钮拖动）：配置在单击各种鼠标按钮并按下键盘控制按钮的同时拖动鼠标时所执行的操作。
   以下示例显示了配置 Shift + MB1 的选项：
   - **Drag direction**（拖动方向）
     - **Zoom**（缩放）：设置拖动鼠标以缩放显示的方向。默认通过垂直拖动鼠标进行缩放。将 Mouse configuration 设置为 Legacy 可通过水平拖动鼠标进行缩放；如果选择 Custom，则可自行设置偏好。
     - **Flip rotation**（反转旋转）：更改在场景中拖动鼠标时对象旋转的默认方向。可将该选项设置为反转水平旋转、垂直旋转或两者的旋转方向。可针对 Walk around object 和 Rotate object 两种旋转方法进行配置。
   > **注意**
   >
   > 此视频演示如何自定义鼠标中键。
   >
   > 视频未包含在 PDF 中。要访问视频，请使用 HTML。

<a id="v1-s245"></a>

#### To implement your mouse customization after upgrading to a new software version（升级到新软件版本后应用鼠标自定义）

**操作步骤**

1. 关闭 Process Designer。
2. 找到存储在当前版本文件夹中的 `RibbonMouseConfiguration.xml` 文件，该文件位于用户配置文件的 GeneralConfiguration 嵌套目录下，例如：
     
   `C:\Users\JohnDoe\AppData\Local\Tecnomatix\GeneralConfiguration\13.0\RibbonMouseConfiguration.xml`
3. 复制该文件并覆盖新版本文件夹中的等效文件，例如：
     
   `C:\Users\JohnDoe\AppData\Local\Tecnomatix\GeneralConfiguration\13.0.1\`
4. 启动 Process Designer 并验证鼠标自定义是否已保留。

<a id="v1-s246"></a>

### Customize ribbon tabs（自定义功能区选项卡）

你可以根据需要从功能区选项卡添加和移除按钮，并创建和自定义新选项卡。你还可以隐藏功能区选项卡、指定是否显示工具提示，以及将自定义的功能区恢复为默认设置。

**操作步骤**

1. 右键单击功能区并选择 **Customize the Ribbon**。
2. 单击 **New Tab**（新建选项卡）。新的功能区选项卡被插入到 Main Tabs 列表，并且新选项卡填充了一个空的 New Group（新组）按钮。
3. 选择新选项卡并单击 **Rename**（重命名），或右键单击新选项卡并选择 Rename。
4. 输入新选项卡的名称，并单击 **OK**（确定）。
5. 以相同方式重命名新组。
6. 使用右侧的箭头按钮将新选项卡移动到功能区上的所需位置。
7. 向新组添加命令：
     
   a. 选择新组（或任何组）。
     
   b. 从 Choose command from 列表顶部的下拉列表中选择命令类别，或保留 All Commands 默认设置。
     
   c. 在 Choose command from 中选择一个命令。
     
   d. 单击 **Add**（添加）。命令被添加到所选组。
     
   e. 添加你希望该组包含的所有命令。
     
   f. 如有必要，在所选组中选择一个命令并单击 **Remove** 将其从组中删除。
8. 单击 **OK**（确定）。新选项卡显示在功能区上。

此外，在 Customize 对话框的 Toolbar（工具栏）选项卡中，你可以如下修改工具栏：

- 通过在 Toolbars 字段中选择或取消选择工具栏名称，在 Process Simulate 窗口中显示或隐藏工具栏。
- 通过在 Toolbars 字段中选择工具栏名称并单击 **Delete** 来删除工具栏。
- 通过在 Toolbars 字段中选择工具栏名称并单击 **Rename** 来重命名工具栏。
- 确定你添加到新选项卡组的命令在功能区中是显示为大型图标还是小型图标。对于小图标，你可以选择是否与图标一起显示命令名称文本。即使功能区在空间不足时将大图标更改为小图标，你也可以选择始终显示大图标（Always Large Icon and Text），反之，即使有足够空间也始终显示小图标。
- 通过将工具栏按钮拖离工具栏，或将其拖到 Customize 对话框的任何选项卡来从工具栏移除命令。
- 通过单击 **Default Settings** 将所有工具栏恢复为默认设置。

<a id="v1-s247"></a>

### Customize Quick Access Toolbar（自定义快速访问工具栏）

你可以在功能区下方显示快速访问工具栏，以快速访问常用命令。

要显示工具栏，右键单击功能区并选择 **Show Quick Access Toolbar Below the Ribbon**（在功能区下方显示快速访问工具栏）。如果再次右键单击功能区，可以选择 **Show Quick Access Toolbar Above the Ribbon**（在功能区上方显示快速访问工具栏）。

**操作步骤**

1. 右键单击功能区并选择 **Customize Quick Access Toolbar**。出现 Customize 对话框，且 Quick Access Toolbar 选项卡处于活动状态。
2. 要向快速访问工具栏添加命令：
     
   a. 从 Choose command from 列表顶部的下拉列表中选择命令类别。
     
   b. 在 Choose command from 中选择一个命令。
     
   c. 单击 **Add**（添加）。命令被添加到快速访问工具栏并出现在右面板中。
     
   d. 添加你希望在快速访问工具栏中显示的所有命令。
     
   e. 在右面板中选择一个命令，并使用右侧的箭头按钮更改其在工具栏上的位置。
     
   f. 如有必要，在所选组中选择一个命令并单击 **Remove** 将其从快速访问工具栏删除。
3. 可选地，执行以下任何操作：
   - 设置或清除 **Show Quick Access Toolbar Below the Ribbon** 以显示或隐藏快速访问工具栏（退出 Customize 对话框后）。
   - 单击 **Reset**（重置）将快速访问工具栏恢复为出厂设置。
4. 单击 **OK**（确定）。显示更新。
5. 你也可以右键单击功能区上的任何按钮并选择 **Add to Quick Access Toolbar**（添加到快速访问工具栏）。

<a id="v1-s248"></a>

## Libraries（库）

<a id="v1-s249"></a>

### Resource Library（资源库）

资源库不能包含孪生对象（twin objects），只能包含原型（prototypes）。这些原型的实例可以拖放到其他对象中，例如复合资源或操作。向资源库添加新节点时，除标准资源节点外，还可使用以下节点类型（参见 Resource Tree Node Types）。资源库也可以包含子库。

| 图标 | 描述                        |
| -- | ------------------------- |
|    | Clamp——工件的夹具              |
|    | Container——容器或支架          |
|    | Conveyor——移动输送机           |
|    | Device——除机器人外的机构          |
|    | Dock_System               |
|    | Fixture——除夹具和输送机外的任何夹具    |
|    | Flange                    |
|    | Gripper——机器人夹爪            |
|    | Gun——焊枪                   |
|    | Human                     |
|    | Robot                     |
|    | Security_Window           |
|    | ToolPrototype             |
|    | Turn_Table                |
|    | Work_Table——放置装配、部件或资源的台面 |

管理员可以添加新原型、删除或编辑现有原型，并定义新的节点图标和属性。请参阅 Tecnomatix Administration（管理）文档中的 Customizations（自定义）和 Adding Soft Classes（添加软类）部分。

<a id="v1-s250"></a>

### Manufacturing Features Library（制造特征库）

Process Designer 制造特征包含产品使用的所有制造特征；这些特征由可用于位置参考的几何点层级列表以及点焊点组成。使用下面描述的 Library Tree（库树）查看制造特征。

<a id="v1-s251"></a>

#### Displaying the Mfg Library（显示制造特征库）

要显示制造特征库：

**操作步骤**

1. 在导航树中选择一个 Manufacturing Features 库。然后右键单击并从打开的上下文菜单中选择 **Open** 或 **Open In > Library Tree**。
2. 从 Mfg Library Tree 中选择你想要的制造特征。Library Tree 的右窗格包含 Properties（属性）窗口，打开在 General（常规）选项卡上。

<a id="v1-s252"></a>

#### Inserting a New Mfg Node（插入新的制造特征节点）

新 Mfg 节点只能插入到 Manufacturing Features Library Tree 中的 Manufacturing Library 节点下。该过程与在 Process Designer 中任何位置插入新节点相同。可以使用 New Node（新建节点）对话框中的 amount（数量）列和复选框插入多个节点。

> **注意**
>
> eM-Planner 始终将新节点作为所选节点的子树（子节点）插入。

要在 Manufacturing Library Tree 中插入新节点：

**操作步骤**

1. 确保所需的制造库节点已检出，并选择该节点。
2. 右键单击并从上下文菜单中选择“New”（新建）。打开 New Node 窗口，其中列出可放置在所选 Manufacturing Features Library 节点下的节点类型列表。
3. 选择节点类型（勾选左侧相应的框），在 Amount 列中设置每种类型所需的节点数量，并单击 **OK**（确定）：新节点出现在 Library Tree 中所选节点下。有关更改新节点的名称和其他属性的信息，请参阅 Editing Mfg Node Properties in Manufacturing Features Library 以及与之链接的部分。

下表列出并简要描述了制造特征节点类型：

| 图标 | 类型                    | 表示                       |
| -- | --------------------- | ------------------------ |
|    | Manufacturing library | 包含制造特征的库                 |
|    | Manufacturing feature | 几何点                      |
|    | WeldPoint feature     | 点焊点。此特征对应于 WeldOperation |

<a id="v1-s253"></a>

#### Editing Mfg Node Properties（编辑制造特征节点属性）

Manufacturing Features Library 树节点的 Properties 窗口包含六个选项卡：General、Attachments 和 Attributes。单个制造特征的 Properties 窗口包含七个选项卡：General、Physical、Times、Products、Process、Attachments 和 Attributes。这些选项卡中的许多也出现在其他树的 Properties 窗口中。这些选项卡中的值可以如 Editing Product Node Properties（编辑产品节点属性）中所述进行编辑。

要打开 Manufacturing Features Library Tree 右侧节点的 Manufacturing Features Properties 窗口，请单击该节点。此窗口显示该节点的属性，可以对其进行编辑。

请单击以下链接了解各选项卡的说明。

**Manufacturing Library Tree - General Tab（制造库树——常规选项卡）**

General 选项卡的大多数参数字段也出现在 Product Properties（产品属性）窗口的 General 选项卡中。只有下面描述的 Type 和 Subtype 字段是 Manufacturing Features 窗口 General 选项卡所独有的。未灰显的字段可以编辑。请参阅 Mfg Tree Properties 中的 Manufacturing Features - General Tab。

- **Type**：对于制造库节点始终为 MfgLibrary。它在创建制造库节点时设置，无法更改。
- **Subtype**：对于几何点，Subtype 为 Dummy 或 Geo；对于点焊点，Subtype 为 Respot。这些子类型由用户设置。

**Manufacturing Library Tree - Remaining Tabs（制造库树——其余选项卡）**

其余选项卡与 Product Properties 窗口中同名的选项卡相同。唯一的例外是 Products 选项卡，它与 Operation Properties（操作属性）窗口中同名的选项卡相同。Products 和 Process 选项卡仅与制造特征相关。

<a id="v1-s254"></a>

### Principle Locating Points (PLPs) Library（主定位点（PLPs）库）

主定位点（Principle Locating Points，PLPs）也称为基准（datum），是一种制造特征。它们指示约束装置（例如夹具）将附着到部件上的位置，以防止其在焊接或装配操作期间移动。PLP 被分配给部件，有时在规划之前的 CAD 环境中进行。

在 Process Designer 中，PLP 通常在 station 下的 Datum 库（你可以指定不同的名称）中创建，类似于库原型。在 PLP 属性中，单击 Physical（物理）选项卡以显示并设置以下 PLP 属性的值：

- X、Y、Z 位置（在部件上）
- RX、RY、RZ 旋转轴（防止绕轴旋转）
- Primary Axis（主轴，只能选择一个方向）
- Control Direction（控制方向，防止部件在 X、Y、Z 一个或多个方向移动）

Locate operations（定位操作）用于将部件固定在其分配的 PLP 上。你在 station 下创建 Locate 操作并向其分配 PLP 用法（usages）。用法类似于实例，它们是 Locate 操作将部件固定以防止其在焊接或装配期间移动的点。

如果在库 PLP 中为控制方向轴设置了值，这些属性在 PLP 用法中处于活动状态。在 Locate 操作属性中，单击 PLP 选项卡以显示这些属性。你可以将同一个 PLP 分配给多个 Locate 操作，并为其中一个 Locate 操作锁定特定的控制方向轴，为另一个 Locate 操作锁定不同的轴。

> **注意**
>
> 如果未为库 PLP 的控制方向轴定义值，这些参数在其用法中显示为灰色。

Locate 操作作为 station 节点的子级包含在 Station 中。你可以从 New 对话框创建 LocateOperation 类的新节点。

<a id="v1-s255"></a>

## Working with Trees（使用树）

<a id="v1-s256"></a>

### Opening and Closing Trees（打开和关闭树）

通过选择 **Home** 选项卡 **→ Viewers** 组 **→ Viewers** 并选择树来打开树。通过单击位于树右上角的 **Close**（关闭）按钮来关闭树。

下次启动 Process Designer 时，应用会保留每个树从上一个会话的打开/关闭状态。

树是可停靠的，允许你根据需要移动和调整其大小。你可以使用 Layout Manager（布局管理器）排列和保存各种查看器的布局。

<a id="v1-s257"></a>

### Selecting Objects for Viewing（选择对象以查看）

在其中一个树中选择一个对象会同时在 Graphic Viewer 中高亮该对象。（导航树对象可能不以此方式表现。如果在未加载的子树中选择对象，其对象不会被高亮。）

任何节点都可以通过单击它来选择；先前选择的节点随后被取消选择。

要同时选择多个独立节点，按住 Ctrl 键并根据需要单击其他节点。要选择一系列连续节点，请选择第一个节点，按住 Shift 键并单击最后一个节点。第一个和最后一个节点之间的节点也被选中。

你也可以将对象拖到查看器以将其添加到已加载的对象中。此外，如果你选择一个复合节点，其所有子节点都将显示在 Graphic Viewer 中。

<a id="v1-s258"></a>

### Navigating Trees with the Mouse（使用鼠标浏览树）

- 要选择节点，单击它。
- 要选择多个不连续的节点，按 Ctrl 键并单击节点。
- 要选择多个连续节点，单击第一个节点；然后按 Shift 键并单击该系列的最后一个节点。
- 通过拖动重新排列节点顺序（不改变层级）。黑线指示放置节点时的位置。
- 要展开或折叠节点，双击它。或者，单击折叠节点标题旁的加号（+）图标将其展开；单击展开节点旁的减号（-）图标将其折叠。
  - 当双击无法展开或折叠的节点时，显示该节点的 Properties 窗口。
- 要垂直滚动树，拖动垂直滚动条中的方框上下移动，或单击滚动条两端的箭头。

<a id="v1-s259"></a>

### Navigating Trees with the Keyboard（使用键盘浏览树）

- 要每次向上或向下移动一个节点，请按相应的上下箭头键。如果选择了多个节点，此过程从最底部的选择向上或向下移动。
- 要展开折叠的节点，高亮它并按右箭头键。在标有加号的节点（表示其中含有其他节点）处连续按右箭头键，将选择移动到所选链的最内层节点。
- 要折叠展开的节点，高亮该节点并按左箭头键。在包含在其他节点中的节点处连续按左箭头键，会使你进一步向上移动树层级，直到整个树折叠并到达根节点。
- 要每次向上或向下移动一个窗口，请按 Page Up 和 Page Down 键。

<a id="v1-s260"></a>

### Searching Individual Trees（搜索单个树）

你可以通过激活搜索栏分别搜索以下树：

- Mfg
- Operation
- Resource
- Product

通过单击树工具栏中的 **Search Tree**（搜索树）图标打开树中的搜索栏。显示一个文本字段。在字段中输入你要搜索的字符或对象名称。如果已加载 3D 数据，该对象会在 Graphic Viewer 中标记。

单击 **Find Next**（查找下一个）以继续当前搜索。先前的搜索会保存在搜索字段的下拉列表中，直到你退出 Process Designer。

<a id="v1-s261"></a>

### Moving Nodes（移动节点）

各种树视图允许你通过从当前位置删除所选节点及其子树并将其粘贴到新位置来移动它们。

**操作步骤**

1. 检出源节点以及目标节点。
2. 选择要移动的节点。
3. 单击鼠标左键，并将节点拖动到其新的父节点。拖动的节点及其子树作为目标节点的子节点粘贴。

<a id="v1-s262"></a>

### Copying Nodes（复制节点）

树视图还允许你将所选节点（可以是对象或操作）及其子树复制到新位置。

**操作步骤**

1. 检出目标节点。
2. 选择要复制的节点。
3. 按住 Ctrl 键的同时按住鼠标左键，并将节点（对象或操作）拖动到其新的父节点。如果节点有子树，会要求确认是否也复制子树。拖动的节点及其子树作为目标节点的子节点复制。如果节点有链接，会要求确认是否复制链接。

<a id="v1-s263"></a>

### Inserting Nodes（插入节点）

参见 New（新建）。

<a id="v1-s264"></a>

### Sharing Nodes（共享节点）

所选节点可以指定为同一树中各种子树节点之间共享。但是，它不能与同一所选节点下的子节点共享。共享允许同一对象出现在多个装配或其他组类型中：它适用于 Product Tree（产品树）和 Resource Tree（资源树）。在产品树中，同一部件可以在各种装配之间共享；在资源树中，各种 line 可以共享同一资源。

**操作步骤**

1. 检出该节点。
2. 选择将被共享的节点。
3. 将节点拖动到将共享它的复合节点，并按住 Alt 键。如果该复合节点可以共享被拖动的节点，它会高亮。
4. 释放鼠标按钮，然后释放 Alt 键。如果共享操作成功（即被拖动的节点现在已共享），树中其图标下会出现一只手。

<a id="v1-s265"></a>

### Deleting Nodes（删除节点）

参见 Delete（删除）。

<a id="v1-s266"></a>

### Sorting Nodes By Caption Mode（按标题排序节点）

你可以通过单击树工具栏中的相应按钮，将单个树的节点按标题以字母数字顺序排序显示。在此模式下，树中的所有节点按字母数字顺序在……下排序，新节点按字母数字顺序添加。对树排序后，你可以将排序后的视图保存到 eMServer。**Save Sorted Tree**（保存排序树）命令可从 Customize 对话框的 Tree Commands（树命令）类别中获得。

> **注意**
>
> 如果排序树中的任何对象被其他人检出，系统无法保存排序后的树，并显示无法检出的节点列表。

此命令在以下树中可用：

- Mfg
- Operation
- Product
- Resource
- Process Module
- Navigation（仅 Sorting Nodes by Caption 命令与 Navigation 树相关，而非 Save Sorted Tree 命令）

<a id="v1-s267"></a>

### Copying from a Library to a Tree（从库复制到树）

除 New 命令外，还可以通过将节点从相关库拖动并将其放置到相关树中任何复合操作节点下来将节点添加到树。

你可以拖动：

- 从 Operation 库将操作拖到 Product 树中任何复合操作下
- 从 Resource 库将原型拖到 Resource 树中任何复合资源节点下
- 从 Parts 库将原型拖到 Product 树中任何复合部件节点下

> **注意**
>
> 复制的对象独立于源对象，对其中任一方的更改不会影响另一方。

要通过从库拖动添加新节点：

**操作步骤**

1. 打开 Library 树并选择一个节点。
2. 将其拖动到相关树中已检出的复合节点。如果系统能够复制该操作，它会高亮该复合节点。
3. 释放鼠标按钮——复制的节点出现在该复合节点下。

<a id="v1-s268"></a>

### Expanding and Collapsing Nodes（展开和折叠节点）

<a id="v1-s269"></a>

#### Expand Command（展开命令）

上下文菜单 Expand（展开）命令确定层级树显示的节点级别数。要访问上下文菜单，请右键单击所选节点。Expand 命令对单个节点起作用；如果选择了多个节点，它将对所选节点中最底部的节点操作。

**Expand→1 Level**（展开→1 级）命令显示所选节点下的单级子树。它对树中的单个节点操作。要么：

1. 选择要展开的节点。
2. 右键单击以显示上下文菜单，然后选择 **Expand→1 Level**。

或者单击节点图标左侧的加号（+）。

**展开两级或三级**

要展开所选节点两级或三级：

1. 选择要展开的节点。
2. 右键单击以显示上下文菜单，然后选择 **Expand→2 Levels** 或 **3 Levels**。

**展开所有级别**

要展开所选节点通过其所有级别：

1. 选择要展开的节点。
2. 右键单击以显示上下文菜单，选择 **Expand**，并从弹出菜单中选择 **all Levels**。

> **注意**
>
> Expand 选项也可在上下文菜单中获得。请参阅 Expand 了解可用命令的说明。

<a id="v1-s270"></a>

#### Collapse Level Command（折叠级别命令）

上下文菜单 Collapse Level（折叠级别）命令使你能够快速将树中的节点折叠到树中当前所选的节点级别。

例如，下图说明 ActrixFolder 节点已在树中被选中。当你右键单击此节点并选择 **Collapse Level** 时，树中的所有节点都折叠到此节点级别，如下图所示。整个树受此操作影响。在此示例中，StudyFolder 中的所有子节点都被折叠。

**Collapse All（全部折叠）**

Collapse All 命令隐藏所选节点下所有级别的节点，仅显示其顶级节点。所有其他节点保持当前显示状态。

当你在对某个节点执行 Collapse All 后展开它时，仅显示一个级别的节点。如果在执行 Collapse All 之前 Collection_1 下显示了多个节点级别，则在展开后不再显示这些级别。在此示例中，显示 cell 和 Collection_10，而 Collection_20、Collection_30 等保持隐藏。

<a id="v1-s271"></a>

### Viewing Assignment Indications and Variant Filter Information（查看分配指示和变量筛选器信息）

除导航树外，每个树都包含一个工具栏，带有以下一个或两个图标：

| 图标 | 工具                                       | 描述                                                                                                                                               |
| -- | ---------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
|    | Operation Assignment Indications（操作分配指示） | 按下时，将所选树中已分配的项显示为绿色，未分配的项显示为红色。如果复合节点被分配，其所有子节点自动被分配。当复合节点被取消分配时，每个子节点的分配状态单独表明。分配信息根据 eMServer 中的更改在线更新。此图标仅显示在 Product、Resource 和 Mfg 树中       |
|    | Assignment Indication Options（分配指示选项）    | 仅 Part 树：选择其中一个选项以指示在以下规划中哪些项被分配：Production（生产）、Logistic（物流）、Production for loaded operations（已加载操作的生产）、Logistic for loaded operations（已加载操作的物流） |
|    | Show/Hide Columns（显示/隐藏列）                | 显示/隐藏树中指示项所属变量的列                                                                                                                                 |

<a id="v1-s272"></a>

### Project Tree Node Properties（项目树节点属性）

Project Tree（项目树）属性仅在 Navigation Tree（导航树）中查看。根据所选对象类型，所有其他属性也可以在 Navigation Tree 中查看。

与项目相关的选项卡为：

- Project Tree - General Tab（项目树——常规选项卡）
- Project Tree - Global Parameters Tab（项目树——全局参数选项卡）
- Project Tree - Reports Tab（项目树——报告选项卡）
- Project Tree - Attachments Tab（项目树——附件选项卡）
- Project Tree - Attributes Tab（项目树——属性选项卡）

<a id="v1-s273"></a>

#### Project Tree - General Tab（项目树——常规选项卡）

General 选项卡显示下面强调的数据项；其中三个可编辑：Name、Status 和 Comment。

- **Name**：项目的名称。用户可以指定新名称。
- **Status**：Open、Approved 或 Released——可从列表中选择。
- **Comment**：使用此字段添加注释。

要更改属性，首先检出项目。

系统自动提供下面强调的其余数据字段的值。

- **Type**：节点类型（项目）。
- **External ID**：可与从外部数据库导入的项目关联的可选标识号或名称。
- **Created by**：创建项目的人的登录名。
- **Last modified by**：最后修改项目的人的登录名。
- **Date**：项目的创建日期，格式为 dd/mm/yyyy。
- **ID**：eM-Planner 分配给项目的内部标识号。

**Advanced Button**（高级按钮）出现在所有 General 选项卡中，旨在定位研究（studies）中的对象。Study Information（研究信息）窗格显示给定对象所用研究的列表。这样，用户可以查明对象是否为特定研究的一部分，以及它所用的其他研究的类型和数量。

**要在 Study 窗口中打开对象：**

1. 打开对象的 Properties 窗口。
2. 如果 Study Information 框未出现在 General 选项卡底部，请单击相应按钮。
3. 单击 Study Information 窗格下方的相应按钮。对象所用研究出现在 Study Information 窗格中。
4. 双击所需研究以查看其属性。

<a id="v1-s274"></a>

#### Project Tree - Global Parameters Tab（项目树——全局参数选项卡）

全局参数在项目期间设置一次。这些参数涉及项目的投资和使用货币等事宜。eM-Planner 中的成本核算根据预定义的小时费率比较资源的估计成本和实际成本。这些费率在项目 Properties 窗口的 Global Parameters 选项卡中定义，每个项目只需定义一次，因为费率适用于整个项目。

要显示 Global Parameters 字段（Product、Location、Status 等），请单击项目 Properties 窗口中的 **Global Parameters** 选项卡：

- **Product**：产品的名称。
- **Location**：工厂的地点。
- **Status**：New 或 Old。
- **Line throughput (sec./job)**：生产率（每单位时间）。此字段及其旁边的字段（Expected uptime）涉及性能问题。
- **Currency**：项目计算所用的货币。
- Currency 下方的字段表示所选货币中各类工作的小时成本。输入每类的费率。
- 在 Mechanical Design 字段下方的下拉列表中，选择适用费率的 CAD 软件。
- **Plan Labor Rate/Hrs**：一名工人每小时的计划成本。
- **Plan Maintenance Rate/Hrs**：每小时维护的计划成本。

定义全局参数后，为项目中的每个资源定义估计成本与实际成本。

<a id="v1-s275"></a>

#### Project Tree - Reports Tab（项目树——报告选项卡）

Reports（报告）选项卡显示与当前所选对象对象类型关联的所有报告的列表。有两种类型的报告：

- **Reports**——预定义报告，使你能够使用预配置模板以结构化和标准化的方式查看信息。
- **Dynamic Reports**（动态报告）——使你能够立即查看信息，无需模板即可自定义报告配置。你可以使新报告配置公开，使其他用户能够生成相同类型的报告。

对于任一类型的报告，你可以根据需要选择先前缓存的报告或生成反映新数据的新报告。Reports 选项卡显示指向缓存报告的链接列表。有关动态报告的更多信息，请参阅 Web-Based BOP Manager Reference Manual。

<a id="v1-s276"></a>

#### Exporting Reports to Excel（将报告导出到 Excel）

你可以使用 **Export to Excel**（导出到 Excel）按钮进一步分析报告结果，使用标准 MS-Excel 工具运行 VBA 宏、创建图表、应用模板等。

生成 Excel 报告后，确认 Reports 选项卡已打开，并单击 **Export to Excel** 按钮。显示一个包含可用模板列表的对话框：

该对话框包含带有报告模板组的下拉字段，并显示所选组的可用 Excel 模板列表。系统从 SystemRoot/General/Templates 文件夹检索此信息，并根据组名将每组模板存储在不同的文件夹下。

> **注意**
>
> 用户负责将模板存储在正确位置，并确保可用模板与生成的报告匹配。你还必须确保为当前显示的报告选择正确的模板。

1. 从列表中选择一个模板，或完全不选（在这种情况下仅将内容复制到 Excel 报告，不应用格式），并单击 **OK**（确定）。系统在 MS-Excel 应用中打开所选报告。
   > **注意**
   >
   > 如果模板包含名为 **eBOPAutoRun** 的宏，应用在导出到 Excel 完成后会在 MS-Excel 中自动运行此宏。

<a id="v1-s277"></a>

#### Project Tree - Attachments Tab（项目树——附件选项卡）

附件是与系统中任何对象关联的附加信息。附件可以包括文档文件、电子表格、图形文件、演示文稿，或任何其他文件或文件夹。

附件也可以在层级内的节点级别添加。Assembly、Operations 和 Resource 窗口中的 Attachments 选项卡允许将任意数量的文件附加到树中的特定节点。

项目的所有附件在 Attachments 选项卡中强调，其中包含以下命令：

- **Open Selected Attachment**（打开所选附件）
- **Remove Selected Attachment**（移除所选附件）
- **Attach File**（附加文件）
- **Attach Folder**（附加文件夹）

**Open Selected Attachment**：从文件列表中选择一个附件并单击相应按钮。你的计算机启动与所选文件类型关联的应用并显示该文件以供查看和/或编辑。

**Remove Selected Attachment**：高亮一个或多个要移除的附件，并单击相应按钮。系统提示你确认或取消该操作。

**Attach File**：单击相应按钮以添加附件文件。选择该功能以打开浏览器窗口，从中可以选择文件（或多选）以附加到项目节点。高亮要附加的文件并单击 OK。

**Attach Folder**：单击相应按钮以打开浏览器窗口，从中选择文件夹作为项目节点的附件。此浏览器窗口仅显示文件夹和驱动器，只能选择文件夹作为附件。高亮要附加的文件夹并单击 OK。所选文件夹中的所有文件和文件夹都附加到项目。

<a id="v1-s278"></a>

#### Project Tree - Attributes Tab（项目树——属性选项卡）

Attributes（属性）选项卡由一个表组成，显示与所选节点关联的所有属性。各种类别由管理员通过自定义功能定义。

每个对象实例都可以通过此选项卡修改其值，前提是在自定义中实例下强调了这些类别。每次创建实例时，它从原型继承其初始属性值。

Attribute 和 Value 列的大小通过拖动其边框调整。如果字段被无意中缩减为零大小，请关闭并重新打开 Properties 窗口以恢复其默认大小。

Attributes 选项卡包含一个表，强调每个属性名称及其值。

<a id="v1-s279"></a>

### Adding Compound Resources and Parts to Studies（将复合资源和部件添加到研究）

你可以从 product 文件夹选择复合资源和部件，并将它们拖放到 Navigation Tree 的 study（研究）文件夹中。Process Designer 还会在 Resource Tree 中显示拖放的复合资源，在 Product Tree 中显示复合部件。

> **注意**
>
> 如果所选复合的子级已存在于目标中，则新项作为现有项的父级添加。

执行 Save Scenario（保存场景）后，Process Designer 将快捷方式添加到 study 文件夹。

<a id="v1-s280"></a>

## Creating Equipment（创建设备）

用户可以创建 Equipment（设备）原型并在 Resource 库中构建其层级。

**操作步骤**

1. 在所需的 Resource 库下创建一个新的 Equipment 原型对象。
2. 将其他 Equipment 原型或 Tool 原型拖放到新的 Equipment 原型以创建子级。
3. Equipment 始终可以通过添加或删除子级进行修改。
4. Equipment 实例化（instantiation）的执行方式与 Resource 库中任何其他对象的实例化相同。
<a id="v2-s1"></a>
# 2. File（文件）

本卷介绍 Process Designer 中 **File（文件）** 选项卡下的功能，包括项目（Project）的打开/关闭/新建、导入/导出（Import/Export）、打包带走（Pack and Go）、CAD/JT 的导入/导出、选项（Options）各选项卡，以及保存/加载（Save/Load）等操作。

<a id="v2-s2"></a>
## Project（项目）

<a id="v2-s3"></a>
### Open Project（打开项目）

Open eMServer Project（打开 eMServer 项目）命令用于从 eMServer 打开已保存的项目并将其加载到 Graphic Viewer（图形浏览器）中。

要打开 eMServer 项目：

**操作步骤：**

1. 关闭当前已打开的项目（如有）（参见 Close Project）。
2. 选择 **File 选项卡 → Project 组 → Open Project**。
3. 滚动到所需项目并选中它。
4. 单击 **Open（打开）**。所选项目将在 Process Designer 窗口中打开。3D 数据会根据 Load on Demand（按需加载）特性加载到 Graphic Viewer。

<a id="v2-s4"></a>
### Change System Root per Project（按项目更改系统根）

Process Designer 允许您为每个项目设置特定的系统根（System Root）和文件夹路径。您可以在项目节点定义以下类型为字符串的属性——请确保属性名称严格按下述方式输入：

- **SystemRoot**
- **ImageFolderPath**：用于图像
- **MovieFolderPath**：用于影片
- **RobotMacrosFolderPath**：用于机器人宏
- **MotionVolumesFolderPath**：用于运动体
- **SpssFolderPath**：用于 Simulation Panel Signals Settings（SPSS，仿真面板信号设置）

系统根在应用程序启动时选择项目以及使用 Open Project 命令打开项目时设置。类似地，图像、影片、机器人宏、运动体和 SPSS 的路径会根据打开项目时各对应属性所指定的值进行设置。

如果 customization（定制）中不存在 SystemRoot 属性，或其值为空字符串，则系统根不会被更改。

<a id="v2-s5"></a>
### New Project（新建项目）

要在数据库中创建新项目：

**操作步骤：**

1. 选择 **File → Project 组 → New Project**。New Project（新建项目）窗口打开，显示可用项目列表。
2. 在 **New Project Name（新建项目名称）** 文本框中指定新项目的名称。系统将在当前登录用户的数据库中创建一个新项目。
3. 单击 **OK（确定）**。

> 项目一旦创建，只能通过 Admin Console（管理控制台）删除。相关信息请参阅 Tecnomatix Administration（Tecnomatix 管理）文档。

<a id="v2-s6"></a>
### Close Project（关闭项目）

Close Project 命令用于关闭当前已打开的项目。

**操作步骤：**

1. 选择 **File 选项卡 → Project 组 → Close Project**。将显示以下消息：
2. 单击 **Yes（是）** 以确认。

<a id="v2-s7"></a>
## Import/Export（导入/导出）

<a id="v2-s8"></a>
### Pack and Go Overview（打包带走概述）

Pack And Go（打包带走）命令用于在不同计算机之间交换 eMServer 数据。使用 Export Pack And Go（导出打包带走）命令，您可以将复制一套功能完整的数据到另一台计算机所需的全部数据导出。该命令会创建一个包含全部所需数据、配置和附件的 PGZ 归档文件。您还可以将 PGZ 归档文件拆分为多个分区以便于传输，和/或设置密码以增强安全性。

使用 Import Pack And Go（导入打包带走）命令，您可以从 PGZ 归档导入数据，从而在远程计算机上复制一份功能完备的副本。如果您已有该数据的副本，在执行导入前，可以将当前副本与 PGZ 归档进行比较，查看导入将产生哪些变更，并决定是否继续。

> **重要：** 在使用电池供电的计算机（例如笔记本电脑）上，使用 Pack and Go Import 创建的任务不会被执行。

**注意：**

- Pack And Go 会消耗大量计算资源，并包含调度（scheduling）选项。您应考虑将 Pack And Go 操作安排在计算机空闲和/或服务器低负荷时段（例如夜间）执行。
- Pack And Go 支持导出 customization 和 variant customization（变体定制）。但导入必须由系统管理员手动执行。
- Pack And Go 的应用场景包括：
  - 创建数据的备份副本。
  - 将项目的进一步开发委派给远程开发人员。下图说明了相关阶段：OEM 为供应商准备导出、OEM 导出数据、供应商导入数据、供应商处理数据、供应商导出数据、OEM 导入数据。

<a id="v2-s9"></a>
### Export Pack and Go（导出打包带走）

Export Pack And Go 命令根据您所设置的 scope（范围）从系统根和 eMServer 数据库收集数据。随后该命令会提示您输入配置信息。您可以输入所需的配置信息（并可选择保存新配置以备将来使用），或加载预定义配置。然后您可以创建一批范围（scopes），并将命令调度到计算机空闲和/或服务器低负荷时段（例如夜间）运行。该命令会生成一个 PGZ 归档文件。

要导出 Pack And Go 数据：

**操作步骤：**

1. 选择 **File 选项卡 → Import/Export 组 → Pack and Go Export**。Export Pack And Go 向导打开至 Scope（范围）页面。
2. 通过以下任一方式设置导出范围：
   - 单击 **Load Scope（加载范围）**。Load Scope 对话框出现。从列表中选择先前配置的范围。如需删除范围，系统仅允许删除您自己创建的范围，而不能删除其他用户创建的；管理员可删除任何范围。
   - 从 Navigation Tree（导航树）中选择要加入导出范围的节点，然后单击相应按钮。
     - 如果选中了项目节点，其所有子节点都会被加入范围（但项目本身除外）。
     - 如果在运行导出前已选中对象，它们会自动显示在 Exported Objects（已导出对象）窗格中。
   - 系统将所需节点加入导出范围并显示在 Exported Objects 列表中。
3. 展开 Exported Objects 层级，勾选要包含在当前导出中的节点，清除要省略的节点。
4. （可选）单击 **Save Scope（保存范围）**，按提示提供名称和位置。系统将当前范围保存在当前项目下以备将来使用。
5. 单击 **Next（下一步）**。Configuration（配置）页面出现。
6. 执行以下任一操作：
   - **加载已有配置：**
     A. 单击 **Load Configuration（加载配置）**，按提示提供先前生成的配置。系统加载导出配置并填充 Name 和 Description。如需删除配置，系统仅允许删除您自己创建的配置；管理员可删除任何配置。单击 **Import Configuration（导入配置）** 按钮可导入 Pack and Go 配置（以独立的 xml 文件形式），例如用于与供应商交换数据。
     B. 在 Using the loaded configuration（使用已加载配置）选项中，勾选以下之一：
       - **Use as is（原样使用）** — 选择此选项以不做任何修改地使用该配置。
       - **Edit the settings of this configuration（编辑此配置的设置）** — 如果您希望编辑该配置，选择此选项。
   - **创建新的导出配置：**
     > **注意：** 创建新配置时，Using the loaded configuration 选项不可用。
     a. 为新的导出配置输入 Name 和 Description。
     b. （可选）单击 **Save Configuration（保存配置）** 保存新配置以备将来使用。单击 **Export Configuration（导出配置）** 按钮可导出 Pack and Go 配置（以独立的 xml 文件形式）用于与供应商交换数据。
     > **注意：** 您可以从主 Configuration 页面或其任意二级页面保存配置。
   - 系统将节点加入导出范围并显示在 Exported Objects 列表中。
7. 单击 **Next**。如果在上一步勾选了 Use as is，Export Pack And Go 向导将跳过其余导出配置页面，直接跳转到 Action（操作）页面；否则会出现 Export Options（导出选项）页面。
8. 配置以下选项（导出时使用的选项存储在 `PGConfigExport.xml` 文件中）：

| 选项（Option） | 说明（Description） |
| --- | --- |
| **Check-In/Check-Out（签入/签出）** > Export if object checked-out by another user（若有对象被其他用户签出则导出） | 勾选后，即使一个或多个对象被其他用户签出，导出仍会继续。清除后，只要有任一对象被其他用户签出，整个导出都会失败。 |
| **Include in export（包含在导出中）** > Customization（定制） | 勾选以将 customization 包含在导出中。<br>**注意：** 尽管此信息会导出到 PGZ 归档文件，但导入操作不会导入 customization，必须由系统管理员完成。 |
| **Include in export** > Variant customization（变体定制） | 勾选以将 variant customization 包含在导出中。<br>**注意：** 尽管此信息会导出到 PGZ 归档文件，但导入操作不会导入 customization，必须由系统管理员完成。 |
| **Include in export** > Engineering and Simulation data（工程与仿真数据） | 勾选以将工程和仿真数据包含在导出中。 |
| **Include in export** > CAD data（CAD 数据） | CAD 数据包含以下字段：3DRep、JT3DRep、Web3DInfo、Picture、2DRep。如果未激活该复选框，这些字段不会包含在导出 zip 文件中。 |
| **Include in export** > Module content（模块内容） | 设置后，模块内容（例如备用范围）包含在导出中。模块的子类对象始终包含在导出中。 |
| **Include in export** > Export attachments（导出附件） | 设置后，附件包含在导出中。清除后，物理附件文件及其关系都不会包含在导出中。 |
| **Variant Filter（变体过滤器）** > Current variant filter（当前变体过滤器） | 设置后，导出中使用当前应用的变体过滤器。 |
| **Variant Filter** > Define variant filter（定义变体过滤器） | 设置后，指定导出中要使用的变体过滤器。单击 **Browse（浏览）** 按钮打开 Choose a variant filter（选择变体过滤器）对话框，其中包含项目中所有的变体过滤器库。<br>**注意：** 如果在调度并执行导出操作时项目中不存在所配置的变体过滤器，导出将失败。 |
| **ZIP file（ZIP 文件）** > Location（位置） | 指定 PGZ 归档文件的位置。对于 Windows 2008 Server 操作系统，必须选择本地目标，而非网络目标。 |
| **ZIP file** > Password（密码） | 为 PGZ 归档文件指定密码。 |
| **ZIP file** > Compression level（压缩级别） | 可为 PGZ 归档文件指定压缩级别：<br>• **Store（存储）** — 不压缩，速度最快。<br>• **Normal（常规）** — 中等压缩，是压缩级别与归档速度之间的折中。<br>• **High（高）** — 高压缩级别，速度最慢。 |
| **ZIP file** > Partition (MBs)（分区大小，MB） | 可将 PGZ 归档文件划分的分区大小。还提供以下预定义大小：Single file（单个文件）、DVD (4800)、CD (690)、Double layer DVD (8700)。系统会创建主 PGZ 文件以及带数字后缀（*.001、*.002 等）的附加文件。 |
| **Logging（日志记录）** > Log file location（日志文件位置） | 指定日志文件的位置。可访问此文件获取调试信息。 |
| **Post-processing（后处理）** > Executable（可执行文件） | 指定 Pack and Go 导出的可执行文件路径。它必须由任务在 Pack and Go 可执行文件之后启动。<br>环境变量扩展——使用环境变量可让 Pack and Go 配置独立于环境变化。 |
| **Post-processing** > Key word parameters（关键字参数） | 通过保留关键字指定某些参数，使 UI 中输入的变化反映在工具的调用中。两个最重要的参数是：<br>• 目标 .pgz 文件的路径<br>• 密码（如果用户已提供） |
| **Post-processing** > Command Parameters（命令参数） | 指定调用后处理可执行文件时将使用的命令行参数。 |
| **Post-processing** > Backup File（备份文件） | 保留应用自定义可执行文件之前状态的副本。 |

9. 单击 **Next**。Exclusions（排除项）页面出现。Available elements（可用元素）列表显示您可以从导出中排除的类、属性和关系；Excluded elements（已排除元素）列表显示您已选择排除的元素。
10. 配置排除项：
    - 在 Available elements 列表中选择一个元素并单击对应按钮，将其加入 Excluded elements 列表。
    - 单击对应按钮将全部可用元素加入 Excluded elements 列表。
    - 在 Excluded elements 列表中选择一个元素并单击对应按钮将其移除（返回 Available elements 列表）。
    - 单击对应按钮清空 Excluded elements 列表。

    > **注意：**
    > - 将元素加入 Excluded elements 列表后，这些元素在 Available elements 列表中以粗体显示。
    > - 如果排除了某个类类型，它在 Excluded elements 列表中显示为单个节点，其所有派生类也会被排除。
    > - 排除某个类类型时，其内容也会被排除。例如，若在以下层级中排除 CompoundPart，则只导出 PartLibrary：
    >   ```
    >   PartLibrary
    >     CompoundPart
    >       Part
    >   ```
    > - 如果排除的是关系或属性，它会与其父项一同显示在 Excluded elements 列表中，但父项不会被排除。该关系或属性仅从其特定类中排除，而不从其类的派生类中排除。

11. 单击 **Next**。Files（文件）页面出现，用于将计算机硬盘上的附件加入导出。
    > **注意：** 此页面用于处理不属于项目组成部分的文件。项目中层级内的类、属性和关系请使用 Exclusions 页面。
12. 在 Additional Files（附加文件）区域，按需执行以下任意操作（可多次）：
    - 单击 **Add Folder（添加文件夹）** 并选择要导出的目录。
    - 单击 **Add File（添加文件）** 并选择要导出的文件。
      > **注意：** 添加文件时，其父目录也会一并添加。导入时，文件会在其父目录指定的位置解压。若两个文件同名且父目录相同，则只导入第一个。
    - 在 Additional Files 区域中选择文件并单击 **Remove（移除）** 将其从导出中删除。
    - 单击 **New file or wildcard（新文件或通配符）** 链接并编辑路径和文件名。此选项支持 Windows 通配符，也可用于添加整个文件夹。
13. 在 Excluded Files（排除文件）区域，按需执行以下任意操作（可多次）：
    - 单击 **Add File** 并选择要排除的文件。例如，若已在 Additional Files 区域添加某文件夹，可选择该文件夹中要排除导出的文件。
    - 在 Excluded Files 区域中选择文件并单击 **Remove** 将其从排除文件中移除。
    - 单击 **New file or wildcard** 链接并编辑路径和文件名。此选项支持 Windows 通配符，也可用于添加整个文件夹。
    - 单击 **Add Folder** 并选择要排除的文件夹。
14. 单击 **Next**。Action 页面出现，可用于在 Export Pack And Go 向导的单次执行（任务）中创建一批要导出的操作。
15. 系统为当前操作分配一个名称并显示在 Actionname（操作名称）区域。如需可编辑该名称。
16. 在 **Do you wish to add another action?（是否要添加另一个操作？）** 下，执行以下任一操作：
    - 选择 **no, go to last step（否，转到最后一步）** 并单击 **Next**。Task（任务）页面出现。
    - 选择 **Yes, add another action（是，添加另一个操作）** 并按以下步骤：
      a. 单击 **Next**。向导返回 Scope 页面。
      b. 创建另一个操作。
      c. 当回到 Do you wish to add another action? 时，持续添加操作直到创建完所有所需操作。
      d. 勾选 **no, go to last step** 并单击 **Next**。Task 页面出现。
17. 在 Task Information（任务信息）区域，输入 Task name（任务名称）以及您的 eMServer User name（用户名）和 Password（密码），以及 Windows Task Scheduler User name（任务计划程序用户名）和 Password。
    > **注意：**
    > - 此信息经过加密以确保安全。
    > - 非管理员用户需要 Log on as batch job（作为批处理作业登录）权限才能执行任务调度，参见 Assigning Pack and Go Scheduling Rights。
    > - System Root 视图显示任务执行时使用的路径。
18. Actions 列表显示您创建的所有操作。您可用箭头按钮调整其顺序，并移除不希望处理的操作。
19. 选择以下 Task Scheduling（任务调度）选项之一：
    - **Execute task immediately（立即执行任务）** — 单击 Finish（完成）时执行任务。此选项仅建议用于小型任务，Process Designer 会继续运行。
    - **Schedule task start time（调度任务启动时间）** — 设置任务执行时间，并选择以下子选项之一：
      - **One time（一次）** — 选择执行任务的日期。
      - **Daily（每天）** — 系统允许您控制任务重复频率，设置所需的天数。
      - **Weekly（每周）** — 系统允许您控制任务重复频率，设置所需的周数以及星期几。
      > **注意：** 您无需保持登录系统。当到达计划的任务时间时，系统会自动登录 eMServer。
20. 单击 **Finish**。Export Pack And Go 向导关闭。

<a id="v2-s10"></a>
### Assigning Pack and Go Scheduling Rights（分配打包带走调度权限）

非管理员用户需要 Log on as batch job（作为批处理作业登录）权限才能调度 Pack and Go Export 作业：

**操作步骤：**

1. 在电脑桌面上，选择 **Start（开始） → Run（运行）** 并输入以下命令：`secpol.msc /s`。将显示 Windows Local Security Policy（本地安全策略）对话框。
2. 导航到 **Log on as a batch job（作为批处理作业登录）**。
3. 访问 Log on as a batch job 的属性。将显示 Log on as a batch job Properties（属性）对话框。
4. 单击 **Add User or Group（添加用户或组）**。将显示 Select Users（选择用户）对话框。
5. 添加所需用户并单击 **OK**。
6. 在 Log on as a batch job Properties 对话框中，单击 **OK**。

<a id="v2-s11"></a>
### Pack and Go Import（导入打包带走）

有关 Pack and Go 的常规信息，请参阅 Pack and Go Overview。

> **重要：** 在使用电池供电的计算机（例如笔记本电脑）上，使用 Pack and Go Import 创建的任务不会被执行。

Import Pack And Go 命令导入您此前导出到 PGZ 归档文件的 eMServer 数据，并将其存储到指定目标。在执行导入操作前，可将数据库中当前存储的数据与 PGZ 归档中存储的数据进行比较，并决定是否继续。随后导入命令会提示您输入配置信息。您可以输入所需的配置信息（并可选择保存新配置以备将来使用），或加载预定义配置。然后您可以创建一批范围，并将命令调度到计算机空闲和/或服务器低负荷时段（例如夜间）运行。

要导入 Pack And Go 数据：

> **注意：** 如果用户文件夹：
> - 运行 Import 时未签出，Pack and Go 会自动将其签出。
> - 被其他用户签出，导入失败，并在日志文件中写入一条消息。
> - 被其他用户签出，则在启动“Pack and Go Import”命令时会显示警告消息“Working folder is checked out by another user, do you want to continue?（工作文件夹已被其他用户签出，是否继续？）”。

**操作步骤：**

1. 选择 **File 选项卡 → Import/Export 组 → Pack and Go Import**。Import Pack And Go 向导打开至 Scope 页面。
2. 在 Imported ZIP File（已导入 ZIP 文件）区域设置以下内容：
   - **File（文件）** — 浏览或输入使用 Pack and Go Export 创建的 PGZ 归档文件的路径和名称。
   - **Password（密码）** — 如果在 Pack And Go 导出选项中配置了密码，则在此输入。
   - **Password again（再次输入密码）** — 再次输入密码。
3. 默认情况下，所有数据都会导入到工作文件夹中的导入库（import library）。但在 Destinations（目标）中，您可以覆盖默认值并为以下各项选择位置：
   - **Folder for imported orphan nodes（导入的孤立节点文件夹）** — 用于孤立对象（缺少父对象的对象）的文件夹。
   - **Libraries for imported prototypes（导入原型的库）** — 用于 Parts、Mfgs 和 Resources 原型的文件夹。

   当导入包含 study（研究）和 robot（机器人）对象且已存在工程（study、robot）数据的 PGZ 文件时，导入会创建具有新的、不同 ExternalID（外部 ID）的研究和机器人。
   > **注意：**
   > - 多次导入此类 PGZ 文件每次都会创建新的研究，但只有最新的研究才包含子类（快捷方式）。
   > - 多次导入此类 PGZ 文件会正确更新新的机器人，同时将较早的（前次导入创建的）机器人放置在工作文件夹中。
   > - 对于 ExternalID 已更改的研究和机器人，如果 PGZ 文件已被导入且对同一 PGZ 执行 Pack and Go Compare，Compare 无法正常工作。
   > - 连接到 Robcad cell（通过 Robcad Connectivity）的研究，其与 Robcad cell 的链接会被移除，因为 Robcad cell 一次只能连接一个研究。

4. （可选）单击 **Compare Imported and Existing Data（比较已导入数据与现有数据）**。此命令用于将您系统上当前存储的数据与您要导入的 PGZ 归档中存储的数据进行比较。有关 Compare 命令的更多信息，请参阅 Compare Pack and Go。
5. 单击 **Next**。Configuration 页面出现。
6. 执行以下任一操作：
   - **加载已有配置：**
     a. 单击 **Load Configuration** 并按提示提供先前生成的配置。系统加载导入配置并填充 Name 和 Description。如需删除配置，系统仅允许删除您自己创建的配置；管理员可删除任何配置。单击 **Import Configuration** 按钮可导入 Pack and Go 配置（以独立的 xml 文件形式）用于与供应商交换数据。
     b. 在 Using the loaded configuration 选项中，勾选以下之一：
       - **Use as is** — 选择此选项以不做任何修改地使用该配置。
       - **Edit the settings of this configuration** — 如果您希望编辑该配置，选择此选项。
   - **创建新的导入配置：**
     > **注意：** 创建新配置时，Using the loaded configuration 选项不可用。
     a. 为新的导入配置输入 Name 和 Description。
     b. （可选）单击 **Save Configuration** 保存新配置以备将来使用。单击 **Export Configuration** 按钮可导出 Pack and Go 配置（以独立的 xml 文件形式）用于与供应商交换数据。
     > **注意：** 您可以从主 Configuration 页面或其任意二级页面保存配置。
   - 系统将节点加入导入范围并显示在 Imported Objects（已导入对象）列表中。
7. 单击 **Next**。如果在上一步勾选了 Use as is，Import Pack And Go 向导将跳过其余导入配置页面，直接跳转到 Action 页面；否则会出现 Import Options（导入选项）页面。
8. 配置以下选项：

| 选项（Option） | 说明（Description） |
| --- | --- |
| **Check-In/Check-Out** > Stop import if object is checked out by other user（若对象被其他用户签出则停止导入） | 勾选后，如果一个或多个对象被其他用户签出，导入将中止。清除后，即使对象被其他用户签出，导入仍会继续。 |
| **Check-In/Check-Out** > Check in scope objects with hierarchy as new version（将范围对象及其层级作为新版本签入） | 勾选后，系统将对象作为新版本签入。清除后，对象按常规方式签入。 |
| **General（常规）** > Working folder not set（工作文件夹未设置） | 用于指示系统在尚未配置工作文件夹时如何处理。选择 **Create new automatically（自动创建新文件夹）** 或 **Stop import（停止导入）**。有关如何配置工作文件夹的信息，请参阅 Set as Working Folder。 |
| **General** > Target folder for non system root files（非系统根文件的目标文件夹） | 指定从系统根之外（自动或手动）导出的文件的导入目标位置。 |
| **Logging** > Log file location | 指定日志文件的位置。可访问此文件获取调试信息。 |
| **Back up existing files（备份现有文件）** | 设置后，系统会在导入过程中为被附件覆盖的每个文件创建备份副本。备份文件名与原文件名相同，并带有 BAK 后缀。 |

9. 单击 **Next**。Files 页面出现，用于从 PGZ 归档文件导入文件附件。
   > **注意：** 此页面用于处理不属于项目组成部分的文件。
10. Excluded Files 列表显示 PGZ 归档中包含的所有文件。勾选要排除导入的文件。
    > **注意：** 文件会连同其父目录一起加入 PGZ 归档。导入时，文件在其父目录指定的位置解压。若两个文件同名且父目录相同，则只导入第一个。
11. 单击 **Next**。Action 页面出现，可用于在 Import Pack And Go 向导的单次执行（任务）中创建一批要导入的操作。
12. 系统为当前操作分配一个名称并显示在 Actionname 区域。如需可编辑该名称。
13. 在 **Do you wish to add another action?** 下，执行以下任一操作：
    - 选择 **no, go to last step** 并单击 **Next**。Task 页面出现。
    - 选择 **Yes, add another action** 并按以下步骤：
      a. 单击 **Next**。向导返回 Scope 页面。
      b. 创建另一个操作。
      c. 当回到 Do you wish to add another action? 时，持续添加操作直到创建完所有所需操作。
      d. 勾选 **no, go to last step** 并单击 **Next**。Task 页面出现。
14. 在 Task Information 区域，输入 Task name 以及您的 eMServer User name 和 Password，以及 Windows Task Scheduler User name 和 Password。
    > **注意：**
    > - 此信息经过加密以确保安全。
    > - 非管理员用户需要 Log on as batch job 权限才能执行任务调度，参见 Assigning Pack and Go Scheduling Rights。
    > - System Root 视图显示任务执行时使用的路径。
15. Actions 列表显示您创建的所有操作。您可用箭头按钮调整其顺序，并移除不希望处理的操作。
16. 选择以下 Task Scheduling 选项之一：
    - **Execute task immediately** — 单击 Finish 时执行任务。此选项仅建议用于小型任务，Process Designer 会继续运行。
    - **Schedule task start time** — 设置任务执行时间，并选择以下子选项之一：
      - **One time** — 选择执行任务的日期。
      - **Daily** — 系统允许您控制任务重复频率，设置所需的天数。
      - **Weekly** — 系统允许您控制任务重复频率，设置所需的周数以及星期几。
      > **注意：** 您无需保持登录系统。当到达计划的任务时间时，系统会自动登录 eMServer。
17. 单击 **Finish**。Import Pack And Go 向导关闭。

    > **注意：** 在 Pack and Go 的 Task Information 页面的 Task Scheduling 中指定的 .pgz 文件所在网络驱动器必须可访问。若导入/导出失败，日志文件位置应位于本地计算机而非网络驱动器上，否则日志文件不会生成。

<a id="v2-s12"></a>
### Compare Pack and Go（比较打包带走）

Compare Pack And Go 命令用于将您系统上当前存储的数据与您要导入的 PGZ 归档中存储的数据进行比较。在执行导入前，您可以分析导入将产生的变更，并决定是否继续。

要比较 Pack And Go 数据：

**操作步骤：**

1. 在 Pack And Go Import 向导的 Scope 页面（参见 Pack and Go Import），单击 **Compare（比较）**。Compare 对话框出现。
   Imported data（已导入数据）列表显示您要导入的 PGZ 归档文件中的数据，Database（数据库）列表显示当前存储在数据库中的数据。两个列表是同步的，在一个列表中选择或展开会导致另一个列表产生相同的效果。选中节点时，不同属性的值显示在下方表格中。存在差异的项以红色标记。
2. 使用工具栏处理显示并查看感兴趣的信息：

| 图标（Icon） | 名称（Name） | 说明（Description） |
| --- | --- | --- |
| Show Different（显示差异） | 仅显示已导入数据与本地数据库之间不同的对象。<br>**注意：** 显示包含从根节点到差异节点的完整层级，即使两者在此部分相同。所有对象均标记为红色。 | |
| Show Same（显示相同） | 仅显示已导入数据与本地数据库共有的对象。<br>**注意：** 所有对象均标记为黑色。 | |
| Show All（显示全部） | 显示所有对象。 | |
| Show Orphans to the left（在左侧显示孤立对象） | 仅显示导入中的对象。 | |
| Show orphans to the right（在右侧显示孤立对象） | 仅显示数据库中的对象。 | |

3. 单击对应按钮关闭 Compare 对话框并继续导入。

<a id="v2-s13"></a>
### Import Object（导入对象）

当 Process Designer 导入数据文件时，它会将文件内容复制到所选节点下。用户只能将数据导入到已签出的节点。如果导入的节点与现有节点具有相同的 external ID（外部 ID），现有节点将使用导入节点的信息进行更新。

当导入的文件包含 part（零件）或 tool（工具）原型时，Process Designer 系统会执行以下操作：

- 将这些对象的实例放置在所选节点下。
- 在项目节点下创建一个新用户文件夹。该文件夹名称为当前登录用户的登录名。
- 在用户文件夹下创建一个名为 Imported from（导入自）后接导入文件名的文件夹。
- 将原型零件和工具作为库节点放置在 Imported from 文件夹下。
- 如果发生错误（例如，由于尝试将 Product Tree（产品树）导入 Operations Tree（操作树）），文件会将出错的节点放置在 Inappropriate Nodes（不适当节点）文件夹中。
- 在系统临时目录（通常为 `C:\temp`）中生成名为 `processplanner.log` 的日志文件。若文件已存在，则被覆盖。

要导入数据：

**操作步骤：**

1. 选择要导出的数据节点，并选择 **Home 选项卡 → Import/Export 组 → Import Object**。将显示 Import（导入）窗口。
   > **注意：** 如有必要，请签出所选节点，以及项目和用户文件夹节点。
2. 导航到所需目录并选择要导入的 `*.xml`、`*.ppd` 或 `*.csv` 文件。
3. 单击 **Import（导入）**。导入过程完成后，将显示以下消息：
4. 单击 **OK**。Process Designer 导入数据文件，并将其保存在所选项目下的所选节点中。在下方示例中，包含导入数据的 Part Library（零件库）文件夹已创建在 Data1 节点下。如果数据是在没有父文件夹的情况下导出的，系统会分配一个以用户名命名的父文件夹来包含导入的数据（如示例所示）。

<a id="v2-s14"></a>
### Export Object（导出对象）

导出数据文件时，导出的是数据库中现有的最新版本，并包含所选节点及其整个子树。

导出前无需签出节点。如果节点已被签出，导出前也无需签入。

当选择了单个节点时可以导出数据文件。如果选择了多个节点，导出命令将被停用。

要导出项目数据：

**操作步骤：**

1. 选择要导出的数据节点，并选择 **Home 选项卡 → Import/Export 组 → Export Object**。将显示 Export（导出）窗口。
2. 导航到所需的目标目录。
3. 在 **File name（文件名）** 字段中输入文件名。
4. 在 **Save as type（保存类型）** 下拉列表中，根据需要选择 `*.xml` 或 `*.ppd`。如果希望对导出的 eBOP 数据使用当前所选过滤器，请勾选 **Apply Selected Filter（应用所选过滤器）** 复选框。
5. 单击 **Save（保存）**。将显示确认消息。
6. 单击 **OK**。导出的数据保存在所选目标目录中。

<a id="v2-s15"></a>
### Import CAD Files（导入 CAD 文件）

> **重要：** 要使用 JT Import（JT 导入）命令，必须先从 CD 上的 CAD Translators（CAD 转换器）目录安装 CAD Translators。

**Importing JT Assemblies（导入 JT 装配）**

JT Import 命令允许您将 JT 装配导入 eMServer。

**操作步骤：**

1. 在 Navigation Tree 中选择任何非 Study 且非 Library 的集合类型节点。
   > **注意：** 此命令需要用户权限。请参阅 User Administration（用户管理）。
2. 选择 **File 选项卡 → Import/Export 组 → Import CAD Files**。Import CAD Files 对话框打开。
3. 选择选项（External ID 选项及其他），如下所述。您的选择将应用于对话框中列出的所有 CAD 文件。
   - **Standard（标准）**
     - 此方法始终生成可导入 eMServer 的有效 XML。
     - 对于相同的层级结构，它始终为相同对象生成相同的 External-ID。
     - 重复 ID 通过使用枚举机制变为唯一。
     - 当超过 1024 个字符时，它会缩短名称，并使用相同（或类似）的枚举机制使其唯一。
     - 更新仅在以下情况成功：
       - 所有对象路径唯一
       - External ID 不超过 1024 个字符
     - a. **External-ID for Prototypes（原型的外部 ID）** — External-ID = JT Geometry Path（JT 几何路径）+ 几何原型的文件名（不含 .jt 扩展名）。例如，`c:\Fender.jt` 变为：External-ID = `c:\Fender`。
     - b. **External-ID for Compounds（复合对象的外部 ID）** — External-ID = 'Main JT Assembly Private name'（主 JT 装配私有名）+ 'object hierarchy path'（对象层级路径）。例如，当 JT 装配为 `C:\myassembly.jt` 时，复合对象为：`car>door>rightdoor>handle>housing`，则 External-ID = `myassembly>car>door>rightdoor>handle>housing`。
     - c. **External-ID for Instances（实例的外部 ID）** — External-ID = 'JT Assembly Private name' + 'object hierarchy path'。例如，当 JT 装配为 `C:\myassembly.jt` 时，实例为：`car>door>rightdoor>handle>housing>screw`，则 External-ID = `myassembly>car>door>rightdoor>handle>housing>screw`。对于此类外部 ID 已存在的情况：`myassembly>car>door>rightdoor>handle>housing>screw_#number`。
   - **PDM Configuration（PDM 配置）**
     - 使用特殊属性 `UGS_TX_EXTERNAL_ID`，它包含作为字符串的 External-ID。系统防止一个零件的多个实例获得相同的外部 ID，通过用“$”分隔每个父项直至根节点来确保对象唯一。
       ```
       Root
         -Subassy1
           -Part1
           -Part2
         -Subassy2
           -Part1
           -Part2
       ```
       Subassy1 中的 Part1 获得 External-ID `Root$Subassy1$Part1`，Subassy2 中的 Part1 获得 External-ID `Root$Subassy2$Part1`。
     - 用户负责将值转换为 JT 的 External-ID。
     - 当用户设置了无效值（重复 ID 或 ID 超过 1024 个字符）时，应用程序不会更改 ID，而是将问题 ID 记录在日志中通知用户，并将 XML 文件名改为 `outputName_invalid.xml`，以提醒用户输出 XML 不正确。
   - **JT Node Attributes（JT 节点属性）**
     JT 属性实际上定义为 JtKProperty 对象。JtKProperty 节点可包含四种类型的属性：
     - **JtkPUBLIC_SHARED** — 指定在 JtkAssembly 或 JtkPart 的所有实例间可见的属性。这是所有属性的默认值。
     - **JtkHIDDEN_SHARED** — 指定与 JtkAssembly 或 JtkPart 的所有实例关联但不可见的属性。
     - **JtkPUBLIC_INSTANCE** — 指定仅在其所在的 JtkAssembly、JtkInstance 或 JtkPart 上可见的属性。
     - **JtkHIDDEN_INSTANCE** — 指定仅与所在的 JtkAssembly、JtkInstance 或 JtkPart 关联但不可见的属性。
     无法在根节点上定义 Instance 属性（JtkPUBLIC_INSTANCE 或 JtkHIDDEN_INSTANCE），因为该节点不能被实例化。因此，此方法对根节点读取 "UGS_TX_EXTERNAL_ID" 的 Shared 属性，而对所有其他节点读取 Instance 属性。尽管此方法对根节点使用 Shared 属性、对其他所有节点使用 Instance 属性，仍必须为每个节点将 "UGS_TX_EXTERNAL_ID" 属性定义为 SHARED 和 INSTANCE（值相同），原因如下：
     - 要将此方法用于子树，每个节点都必须具有 SHARED 属性，以便可作为根节点读取。
     - 要将此方法用于以根节点作为子树的情况，根节点需要 INSTANCE 属性。
   - **Always Unique（始终唯一）** — 此方法为每个节点生成唯一的 External ID。虽然非常安全，但不允许用户执行更新。
   - **Create new subfolder for each imported assembly（为每个导入的装配创建新子文件夹）** — 选择此选项可使用 CAD 装配文件名（仅文件名）在目标文件夹中创建额外的新文件夹。
   - **Accumulate matrices（累积矩阵）** — 如果 CAD 的装配结构在子装配中存在变换，则设置此选项后，矩阵会累积到 JT 装配的叶节点上，并从子装配节点中移除。
   - **Save XML（保存 XML）** — 默认情况下，XML 文件在导入后被删除。
4. 单击 **Add（添加）** 显示 Open（打开）对话框，以选择要设置导入类类型的其他 CAD 文件。
5. 单击 **Open**。File Import Settings（文件导入设置）对话框打开。
6. 对于所选 CAD 文件，选择目标文件夹，该文件夹应指示系统根下将包含 `.cojt` 文件的文件夹路径。在 Class types（类类型）中，对于 Product tree（产品树）的 Base class（基类），选择 Part；对于 Resource tree（资源树）的基类，选择 Resource。然后从下拉列表中选择 Compound Class（复合类）和 Prototype Class（原型类）。列表包含您 customization 中出现的类。激活 **Import as Equipment Prototype（作为设备原型导入）** 复选框会将 Base class 设置为 Resource。结果，Compound Class 仅显示 PmEquipmentPrototype 及其派生类，Prototype Class 仅显示 PmToolPrototype 及其派生类。所选装配或零件将作为 Equipment Prototype 派生对象导入。激活 **Set detailed eMServer classes at a later stage of the import（在导入的后续阶段设置详细的 eMServer 类）** 复选框会禁用 File Import Settings 对话框中的 Base Class、Compound Class 和 Prototype Class 字段。选择此选项会在初始转换过程完成后加载 Assembly Class Selection（装配类选择）对话框（见下文），当您在 Import CAD Files 对话框中单击 Import 时会出现。
   > 此选项还会在 CAD 文件显示于 Import CAD Files 对话框中时，在三个类列中显示星号（*）。如果您同时激活了 Import as Equipment Prototype 和 Set detailed eMServer classes at a later stage of the import 复选框，则 CAD 文件在 Import CAD Files 对话框中显示为 Base Class 列选中 Resource，Compound Class 和 Prototype Class 列显示星号。
   > **注意：** 在此阶段，您可以查看所选项选项并在必要时进行修改。您可以单击 **Add** 向 Import CAD Files 对话框添加更多 CAD 文件，或单击 **Edit（编辑）** 再次打开 File Import Settings 对话框以更改 Class Type 选择、Target Folder（目标文件夹）路径或复选框选项。单击 **Remove** 从 CAD 文件列表中删除所选文件。
7. 单击 **Import** 开始使用所选选项批量转换所有文件，然后将生成的 XML 文件导入 eMServer。（单击 **Close（关闭）** 关闭对话框，并为下次打开 Import CAD Files 对话框保存您的 Target Folder 和 Options 选择。）Import CAD Files Progress（导入 CAD 文件进度）对话框打开。
   该对话框显示将 CAD 文件转换为 `.cojt` 以及为每个零件和装配创建 XML 文件的进度。对话框包含一个转换完成后创建的日志文件链接。单击该链接可在记事本等文本编辑器中查看日志文件。如果在转换或导入过程中出现问题，将显示 Import CAD Files Results（导入 CAD 文件结果）对话框。
8. 如果您为一个或多个文件选择了 Set detailed eMServer classes at a later stage of the import 复选框，系统会在导入 XML 文件之前显示 Assembly Class Selection 对话框。使用此对话框通过下拉框在节点级别指定 eMServer 类。选择 Base Class 时，eMServer Class 会相应过滤。您也可以将根节点设置为 RobcadStudy。通过为根节点 Base Class 选择 RobcadStudy，您可以将原型的子节点类设置为 Part 和 Resource 类的派生类。您还可以更改每个原型的 Target Folder。您可以轻松展开或折叠 Assembly Class Selection 对话框中显示的 CAD 装配的整个层级，或展开到指定级别。右键单击节点也可让您从这些展开和折叠选项中选择。

<a id="v2-s16"></a>
### Export objects to JT（将对象导出为 JT）

Export JT（导出 JT）命令允许您将对象导出为 JT 格式，兼容外部 CAD 应用。

用户可以将研究（study）或产线（line）的几何 3D 快照导出为 JT 格式，用于以下目的：

- **Visualization（可视化）** — 提供制造规划项目状态的 3D 图像。
- **Layout（布局）** — 为工厂布局提供静态工作单元或产线的精确尺寸及资源。
- **Equipment design（设备设计）** — 将仿真期间带穿透数据的碰撞点快照发回 CAD 设计人员进行修改。
- **Layer information（图层信息）** — 提供 CAD 图层 ID、名称和描述，以及标签。

对于 Graphic Viewer 中显示的研究，用户可以指明要导出为 JT 的对象：Studies（研究）、Compound objects（复合对象）或 Instances（实例）。

> **注意：**
> - 导出为 JT 格式时，系统默认创建 9.5 版的 JT 文件。
> - 仅导出已加载且可见（未隐藏）的对象。
> - 不能对多项选定对象运行 Export JT 命令。

**操作步骤：**

1. 从树或 Graphic Viewer 中选择一个研究、复合对象或组件实例（被隐藏的对象不能导出为 JT）。
2. 选择 **File 选项卡 → Import/Export → Export JT**。将显示 Export JT 对话框。
   如果运行命令时未选择任何对象，则导出整个研究，文件名设置为 `GraphicViewer.jt`。如果选择了对象，其名称会自动出现在 Selected Node（所选节点）字段中。Target JT File（目标 JT 文件）字段默认显示当前会话中最近使用的路径，并提供与所选节点同名的装配文件名。
   > **注意：** 对于缺少显示名称的对象，系统使用内部名称。对于既无显示名称也无内部名称的对象，系统为创建的 JT 文件分配默认值，例如 `TxJtExportComponent.jt`、`TxJtExportComponent1.jt`、`TxJtExportComponent2.jt` 等。
3. 在 **Target file format（目标文件格式）** 字段中，选择以下之一：
   - **JT per part（每个零件一个 JT，默认）** — 系统将所选节点下层级 descending（向下）的所有数据作为层级 jt 文件写入，并为每个实例写入单独的 jt 文件。实例的 jt 文件位于与层级文件同名的子目录中。
   - **JT monolithic（单一 JT）** — 系统将整个节点及其子层级存储在单个 JT 文件中。
   > **注意：** 如果选择表示单个组件的节点，两种 Target file format 选项的结果相同。两种情况下，系统都会创建一个包含所选组件所有信息的 JT 文件。
4. 在 **JT Version（JT 版本）** 下拉列表中，为新 JT 文件选择 JT 版本。为方便您，Process Designer 会标识与各 JT 版本兼容的 Teamcenter Vis 和 Tecnomatix 版本。
   > **注意：** General Tab 中的 JT Version 设置不影响此命令的输出。
5. 在 **Include（包含）** 区域，勾选要包含在目标 JT 文件中的项，清除要排除的项：
   - **Frames（坐标系）** — 以坐标系 PMI 形式写入导出的 JT 文件。
   - **Locations（位置）** — 以坐标系 PMI 形式写入导出的 JT 文件。
   - **Dimensions（尺寸）** — 以尺寸 PMI 形式写入导出的 JT 文件。
   - **Notes（注释）** — 以注释 PMI 形式写入导出的 JT 文件。
   - **Labels（标签）** — 以标签 PMI 形式写入导出的 JT 文件。
   - **Layer Information（图层信息）** — 要导出 Label layer（标签图层）、Section Layer（剖面图层）和 Logistic Manipulators（逻辑操纵器，LogArea 和 Tracks），必须选择 study 节点作为 JT 导出的根，并勾选 Layer Information。如果您已使用 Set Configuration and Information for CAD Layers（设置 CAD 图层的配置和信息）命令配置了图层元数据，此信息也会写入导出的 JT 文件。但是，如果您设置了 Layer Information 选项但未配置任何图层信息，系统会返回缺失图层警告。
   - **PMIs** — 以 PMI 形式写入导出的 JT 文件。
   > **注意：**
   > - 选择 **Exact geometry（精确几何）** 选项允许您导出 XtBrep、WfRep 和 JtBrep 几何格式。
   > - 也可以导出 2D 对象。
   > 如果导出的源是组件，则导出时不带方向。导出研究或复合对象时，默认组件相对于原点导出方向。如果选择 **Working Frame（工作坐标系）** 选项，组件将相对于工作坐标系导出。

<a id="v2-s17"></a>
## Open recently viewed studies（打开最近查看的研究）

要打开最近查看的文件，选择 **File → Recent Studies（最近的研究）** 并选择一项研究。

<a id="v2-s18"></a>
## Load（加载）

Load 命令用于将 Robcad 和 Line Simulation（产线仿真）研究加载到树和 Graphic Viewer 中。您也可以加载新对象，替代先前的对象。

要将对象加载到 Graphic Viewer：

**操作步骤：**

1. 在 Navigation Tree 中选择对象，并选择 **File 选项卡 → Study 组 → Load**。该对象出现在 Graphic Viewer 和相应的树中。
2. 当您加载新项目时，将显示以下对话框。
3. 单击 **Yes** 保存更改，**No** 不保存更改，或 **Cancel（取消）** 退出而不执行所请求的加载。

> **注意：** 加载备用对象时，它会替换 Graphic Viewer 和树中的先前对象。

要加载最近的研究：

**操作步骤：**

1. 如有必要，从 Customize（自定义）对话框中拖出 Recent Studies 命令，并将其嵌套到 Process Designer 功能区（ribbon）上。
2. 单击相应按钮，从下拉列表中选择要打开的最近研究。

<a id="v2-s19"></a>
## Save Scenario（保存场景）

Save Scenario 命令保存当前会话中的工程数据和 eMServer 数据。只要存在已加载的项目，它就可用。工程数据的示例包括：

- Notes（注释）
- Sections（剖面）
- Frames（坐标系）
- Snapshots（快照）
- Mount/unmount（安装/卸载）
- Poses（位姿）
- TCPF

eMServer 数据的示例包括：

- 零件、资源和流程装配的快捷方式位置
- 资源位置
- Labels（标签）
- Mount information（安装信息）

数据保存到当前加载的研究中。数据保存后，在 Process Designer 中调用该研究会自动打开已保存的工程数据。应用 Variant Filter（变体过滤器）时，系统会询问是否在继续之前保存数据。

要保存当前会话中的数据：

**操作步骤：**

1. 选择 **File 选项卡 → Save Scenario**。Save Scenario 对话框出现。
2. 勾选以下之一并单击 **OK**：
   - **Store eMServer and study data（存储 eMServer 和研究数据）** — 同时存储 eMServer 数据和研究信息（工程数据）。
   - **Store study data only（仅存储研究数据）** — 存储研究信息（工程数据）和快捷方式位置（研究文件夹直接子项的位置）。如果当前研究中的部分或全部对象被其他用户签出，此选项很有用。被其他用户签出的对象在 Resource Tree（资源树）或 Product Tree 中以红色 X 标记。
   工程数据保存到当前研究中。任何现有数据都会被覆盖，并显示成功消息。
   > **注意：**
   > - 如果系统需要签出对象以更新它们，它会在后台静默完成。
   > - 如果资源已被其他用户签出，将出现以下窗口，显示已签出的资源列表。
3. 单击 **Close（关闭）**。
   > **注意：**
   > - 如果未加载任何研究，系统会保存 eMServer 数据，例如标签、安装信息和位置。
   > - 退出 Process Designer 时，系统会提示您是否保存 eMServer 和/或工程数据。

<a id="v2-s20"></a>
## Save studies（保存研究）

要保存多个研究的副本：

**操作步骤：**

1. 在流程数据库导航视图中选择一个或多个研究。
2. 选择 Search（搜索）栏并输入“save studies”。将显示 Save Studies…（保存研究…）命令。
3. 选择 **Save Studies…**。将显示 Select folder（选择文件夹）对话框。
4. 为本地文件选择目标文件夹。
5. 在 **Save type（保存类型）** 下拉列表中，执行以下任一操作：
   - **Study (*.psz)** — 创建单个压缩研究文件。
   - **Study (*.psz) and Library Components (*.zip)（研究 (*.psz) 和库组件 (*.zip)）** — 创建 PSZ 和一个包含所有库组件的 ZIP 文件。
     > **注意：** 运行 Process Simulate Standalone（Process Simulate 独立版）时，需要将 ZIP 文件解压到本地系统根。
   - **Study (*.psz) and Library Components (unzipped)（研究 (*.psz) 和库组件（未压缩））** — 创建 PSZ 文件并将库组件复制到指定文件夹。
     > **注意：** 运行 Process Simulate Standalone 时，本地系统根应为指定文件夹。
   - **Study and All Components (*.pszx)（研究和所有组件 (*.pszx)）** — 创建包含所有库组件的单个 PSZX 文件。
6. 默认情况下，保存研究和库组件时，Macros（宏）和 SPSS 文件夹会添加到 .pszx 文件中。如果您希望停用此选项，请取消勾选 **Include Macros and SPSS folders（包含宏和 SPSS 文件夹）**。此选择会从上一个会话延续到下一个会话。
7. 单击 **Select Folder** 保存数据。

此操作的持续时间取决于所选研究的大小。在目标文件夹下会创建 `StudyExportedAt_<timestamp>` 子文件夹，其中包含每个所选研究的独立压缩研究文件。

<a id="v2-s21"></a>
## Change password（更改密码）

**操作步骤：**

1. 选择 **File 选项卡 → Change Password**。Change Password 对话框出现。
2. 输入 New password（新密码）。
3. 在 Password validation（密码确认）中再次输入新密码，并单击 **OK**。

<a id="v2-s22"></a>
## Options（选项）

<a id="v2-s23"></a>
### Appearance Tab（外观选项卡）

Appearance（外观）选项卡允许您为多种项目设置参数，如下图所示：

从文本框中选择一个项目，即可从下拉颜色面板更改该项目的颜色。您可以选择不同颜色来指示已分配和未分配组件。通过单击 **Preview（预览）** 按钮，无需关闭 Options 窗口即可在 Graphic Viewer 中预览所选项目的选定颜色。可更改以下项目的颜色：

| 项目（Item） | 说明（Description） |
| --- | --- |
| **Background（背景）** | Graphic Viewer 中的背景颜色。从下拉列表中选择以下背景选项之一：<br>• **Solid（纯色）** — 仅启用左上角颜色选择器，为背景选择一种颜色。<br>• **Vertical（垂直）** — 启用左侧两个颜色选择器，选择两种颜色以创建垂直颜色渐变背景。<br>• **Horizontal（水平）** — 启用顶部两个颜色选择器，选择两种颜色以创建水平颜色渐变背景。<br>• **Corners（四角）** — 启用所有颜色选择器，为每个角选择颜色。系统会创建角之间带颜色渐变的背景。<br>**注意：** 您可以通过单击 Preview 在 Graphic Viewer 中预览所选项目的颜色，而无需关闭 Options 对话框。 |
| **Pick Field（拾取字段）** | 对话框中活动数据字段的颜色。这适用于通过选择 Graphic Viewer 中的对象或点来输入值的所有字段。 |
| **Floor（地板）** | 地板的颜色。 |
| **Floor Grid（地板网格）** | 地板网格的颜色。 |
| **New Objects Colors（新对象颜色）** | 任何新对象以此颜色显示。例如，新的 Object Labels（对象标签）根据此选项卡中定义的颜色显示。 |
| **Simulation Objects Colors（仿真对象颜色）** | 仿真对象以此颜色显示。 |
| **Online Indications Colors（在线指示颜色）** | 更改用于指示已分配和未分配操作的颜色，默认红和绿。 |
| **Kinematics Colors（运动学颜色）** | 设置颜色以指示机器人关节达到其物理和工作极限。 |
| **Sections（剖面）** | 配置以下剖面平面参数：<br>• **Border（边界）** — 设置颜色和宽度（像素数）。<br>• **Currently managed border（当前管理的边界）** — 为剖面管理器当前打开的平面边界设置颜色和宽度（像素数）。<br>• **Plane（平面）** — 为未当前管理的已停用平面设置颜色和透明度百分比。<br>• **Currently managed plane（当前管理的平面）** — 为当前管理的已停用平面设置颜色和透明度百分比。<br>• **Contour（轮廓）** — 在 Clip Section（裁剪剖面）模式下设置颜色和宽度（像素数）；在 Cut Section（剖切）模式下，系统默认使用对象的颜色。 |
| **Point Cloud Shading（点云着色）** | 设置显示点云的首选着色方法。可在 RBG and Intensity（默认）、RBG color（RBG 颜色）或 Intensity（强度）之间选择。选择不同方法可增强不同点云的显示效果。 |

<a id="v2-s24"></a>
### Collision tab（碰撞选项卡）

Collision（碰撞）选项卡（如下图所示）包含用于指定近失（near-miss）和碰撞检测的参数。碰撞包含两种状态：Contact（接触）和 penetration（穿透）。

Collision 选项卡包含以下选项：

| 选项（Option） | 说明（Description） |
| --- | --- |
| **Collision Check Options（碰撞检查选项）** > Check for Collision（检查碰撞） | |
| **Collision Check Options** > Near-Miss（近失） | 选中后，预览中的近失对象以黄色显示。此外，Graphic Viewer 和 Collision Viewer（碰撞查看器）中近失碰撞的对象也以黄色显示。在 Near-Miss Default Value（近失默认值）中指定近失碰撞距离。 |
| **Collision Check Options** > Near-Miss Default Value（近失默认值） | 定义两个对象之间被认为处于近失状态的距离下限。该值在 0 到 10000 mm 之间。您可以在 Collision Viewer 中覆盖此值。 |
| **Collision Check Options** > Collision Contact（碰撞接触） | 根据 Allowed Penetration Value（允许穿透值）字段右侧的默认值，每对碰撞对象都会检查是否允许接触。该字段的最大值为 5 个默认测量单位。小于此值的穿透不被视为碰撞。此参数消除了虚假碰撞，例如螺钉与螺栓连接，或工具放置在表面上。<br>选中 **Show Contacts in Collision Report（在碰撞报告中显示接触）** 复选框后，会将接触对以 Contact 状态显示在碰撞报告中。如果未选择此选项且 Near-Miss 选项处于活动状态，它们将显示为距离 0。如果 Near-Miss 选项未激活，则它们根本不会显示。 |
| **Collision Check Options** > Contact objects color（接触对象颜色） | 有三种颜色选项：<br>• **Red（红色）** — 任何接触都像穿透一样被视为碰撞。<br>• **Orange（橙色）** — 系统分别识别接触和穿透。如果禁用 Near-Miss 选项，此颜色选项不可用。<br>• **No color（无颜色）** — 系统从 Collision Viewer 中移除接触。如果启用 Near-Miss 选项，此颜色选项不可用。 |
| **Collision Check Options** > Allowed Penetration Value（允许穿透值） | 根据 Allowed Penetration Value 指定的值，每对碰撞对象都会检查是否允许接触。算法实际使用的 Allowed Penetration 值相对于碰撞对象的大小。使用以下两个值中较小的一个：用户输入值或较小对象边界框大小的 2%。小于或等于此值的穿透被视为接触，大于此值的穿透被视为碰撞。此参数消除了虚假碰撞，例如螺钉与螺栓连接，或工具放置在表面上。该字段的最大值为 5 mm。您可以在 Collision Viewer 中覆盖此值。<br>**注意：** 接触计算忽略显示的线框对象。 |
| **Collision Check Options** > Ignore wireframe entities（忽略线框实体） | 选中后，计算碰撞和近失时会忽略所有曲线和其他线框对象。 |
| **Collision Report Level（碰撞报告级别）** | 选择 **Component level（组件级别）** 会在 Graphic Viewer 和 Collision Viewer 中于组件级别创建碰撞报告。选择 **Lowest available level（最低可用级别）** 会在 Graphic Viewer 和 Collision Viewer 中于最低可用级别（Entity/Block/Link/Component）创建碰撞报告。 |
| **Collision Detection Behavior（碰撞检测行为）** > Stop Simulation when a Violation is Detected（检测到违规时停止仿真） | 选中后，对象碰撞时仿真停止。注意，违规可能是近失（如果激活了 Check for Collision Near-Miss 选项），或接触（碰撞）。但穿透不被视为违规，也不会停止仿真。每次碰撞状态改变时仿真都会停止。例如，状态从 Near Miss 变为 Contact 时停止，再次从 Contact 变为 Near Miss 时也停止。 |
| **Collision Detection Behavior** > Play a Sound when a Collision is Detected（检测到碰撞时播放声音） | 选中后，发生碰撞时会听到声音。要指定播放的声音，单击选项卡右侧的 Browse 并浏览到所需的 (*.wav) 文件。<br>**注意：** 视频未包含在 PDF 中。要访问视频，请使用 HTML。 |

<a id="v2-s25"></a>
### eBOP report tab（eBOP 报告选项卡）

eBOP Report（eBOP 报告）选项卡允许您按名称和端口号指定报告机器。您还可以设置是否使用常规 http 或安全 https 协议访问 Web 机器。

您可以配置以下参数：

- **Machine（机器）** — 输入提供报告的机器名称。
- **Port number（端口号）** — 定义连接到 Web 服务器时使用的端口。
- **Use SSL connection（使用 SSL 连接）** — 勾选以在访问 Web 服务器获取报告时使用安全 https 协议。

管理员可在以下位置为 Web Server 选项提供相关条目：
`[HKEY_LOCAL_MACHINE\Software\Tecnomatix\TUNE\NewViewer\Options\EBOPREPORT]`

```
"Server name"="destrhesse06"          <- web 服务器的主机名
"Port number"=dword:00001f90          <- 端口号（DWORD）
"Use SSL connection"=dword:00000000   <- 指示是否使用 SSL 的 DWORD 标志
```

如果系统找不到条目 `HKEY_CURRENT_USER\Software\Tecnomatix\TUNE\NewViewer\Options\EBOPREPORT`，则会读取这些值。每当保存选项时，这些值会保存在当前用户分支中。如果管理员需要更改本地计算机的条目，则用户必须确保其用户条目已清除，并重新沿用本地计算机的值。

<a id="v2-s26"></a>
### eMServer Tab（eMServer 选项卡）

eMServer 选项卡（如下图所示）指定 eMServer 系统根（system root）的位置（您必须添加系统根）。

> **注意：** eMServer 选项卡中的设置始终可查看（灰色显示），但您需要 System Root Administration（系统根管理）权限才能编辑它们（常规显示）。有关分配权限的信息，请参阅 Tecnomatix Administration 文档中的 Edit Privilege（编辑权限）。

eMServer 选项卡包含以下参数：

| 参数（Parameter） | 说明（Description） |
| --- | --- |
| **System Root（系统根）** | 指向系统根目录。要更改目录，单击对应按钮，导航到所需位置，并单击 OK。库根（Library root）必须设置在系统根下一级。例如，若 System root = `c:\project\SystemRoot`，则 Library root = `c:\project\SystemRoot\LibraryRoot`。有关如何设置库根的信息，请参阅 General Tab。<br>**重要：** 如果您使用的是 Standalone（独立）配置，请勿在 eMServer 选项卡中更改系统根。而应改用 AdminConsole 设置（参见 Tecnomatix Administration 文档中的 Association Node（关联节点）部分）。 |
| **Use a local copy of the System Root files（使用系统根文件的本地副本）** | 设置后，Process Designer 将原型 3D 几何数据（.co、.cojt 等）文件从系统根复制到 Local Files Cache（LFC，本地文件缓存）并从 LFC 加载文件。保存工作时，Process Designer 将更新后的文件直接保存到系统根，刷新 LFC，并从 LFC 重新加载文件。这样即使系统根中的数据已更改，您也可以继续处理缓存副本。清除后，Process Designer 直接从系统根加载文件并保存到其中——这是默认设置。要配置 LFC，单击 **Settings（设置）**。将出现 Local Files Cache Settings 对话框。您可以配置以下参数：<br>• **Cache folder path（缓存文件夹路径）** — 配置 LFC 文件夹的路径。<br>• **Cache folder maximum size（缓存文件夹最大大小）** — LFC 的最大大小。默认值为 1 GB。当缓存达到最大大小时，系统会删除较早存储的文件以腾出空间存放较新的文件。 |
| **Limit relative path length（限制相对路径长度）** | 系统根文件夹下允许的最大路径长度（字符数）。这会在创建可能违反应用程序限制、从而在库移动到更长的系统根路径时可能阻止加载数据的过长文件路径之前提醒您。取消勾选可允许输入更长的文件路径。 |
| **File System Locations（文件系统位置）** | **End Items Creation Folder（末端项创建文件夹）** 字段指示 Process Designer 保存新数据的目录。要更改目录，单击对应按钮，导航到所需位置，并单击 OK。 |
| **Check In / Check Out（签入/签出）** | 激活 **Notify about checked-out objects when closing project（关闭项目时通知已签出对象）**，以在关闭项目前获知已签出对象。有关通知示例，请参阅 Exit。激活 **Check out working folder when opening project（打开项目时签出工作文件夹）**，以在打开项目时自动签出工作文件夹。 |
| **File Attachments（文件附件）** | 指示 Process Designer 存储图像和影片的目录。要更改目录，单击对应按钮，导航到所需位置，并单击 OK。 |

> **注意（关于 LFC 日志级别）：** 可能的值为 Error、Warning、Info 和 Detailed。

> **注意：**
> - LFC 可同时被多个应用程序使用，例如 Process Simulate 和 Process Designer。它位于 `<Tecnomatix Application Data>\TMXLocalFileCache`。
> - LFC 机制由 LFC 文件夹和控制该文件夹的缓存管理器组成。
> - 如果要在整个组织中强制使用 LFC 模式，可以创建一个 Registry File（.reg）批处理文件，在每次客户端安装后运行。

<a id="v2-s27"></a>
### General Tab（常规选项卡）

General（常规）选项卡（如下图所示）包含用于指定 Graphic Viewer 中对象显示详细程度的参数。

General 选项卡包含以下参数：

| 参数（Parameter） | 说明（Description） |
| --- | --- |
| **Apply Default Layer Filter when loading data（加载数据时应用默认图层过滤器）** | JT 文件可包含图层过滤器，激活后可在图形应用中显示和隐藏各种图层。JT 文件中的对象可属于零个、一个或多个图层，图层可包含任意数量的对象。类似地，图层可属于零个、一个或多个过滤器，过滤器可包含任意数量的图层。例如，一个过滤器可包含产品起始材料的所有图层，另一个过滤器可包含 CNC 阶段所需材料的图层，第三个过滤器可包含最终零件图层。如果 JT 文件未指定默认过滤器，Process Designer 会加载 JT 文件中的所有对象。Process Designer 不支持过滤器的配置——这由创建 JT 文件的应用完成。勾选 Apply Default Layer Filter when loading data 以在 Graphic Viewer 中仅显示 JT 文件的默认图层过滤器，或清除它以显示所有数据。<br>**注意：**<br>• 即使 Apply Default Layer Filter when loading data 处于活动状态，不属于任何图层的对象也会显示。<br>• 更改 Apply Default Layer Filter when loading data 的状态仅影响执行 Load 之后的数据。<br>• 即使从 Graphic Viewer 中过滤掉，所有对象都会出现在 Object Viewer（对象查看器）中。<br>• 不支持 PMI 的过滤。 |
| **Define the PMI types that will be loaded（定义将加载的 PMI 类型）** | 单击 **PMI Types** 配置加载哪些标注 PMI 类型。将显示 Loaded PMI Types（已加载 PMI 类型）对话框。使用箭头按钮根据需要配置 Included types（包含类型）和 Excluded types（排除类型）列表。Process Designer 始终加载 Coordinate System（坐标系）PMI。<br>**注意：** 新设置仅影响执行 Load（重新加载研究）之后的数据。 |
| **JT Version（JT 版本）** | 在 **Save as** 下拉列表中选择新 JT 文件的 JT 版本。为方便您，Process Designer 会标识与各 JT 版本兼容的 Teamcenter Vis 和 Tecnomatix 版本。<br>**注意：** 此设置不影响 Export JT 命令的 JT 格式。 |
| **Special Behavior Area（特殊行为区域）** | 配置以下选项：<br>• **Expand tree to show Graphics Viewer selection（展开树以显示 Graphic Viewer 选择）** — 选中后，树会展开以显示并高亮 Graphic Viewer 中所选对象。<br>• **When deleting a study, delete associated cell files（删除研究时删除关联的单元文件）** — 设置后，在 Navigation Tree 中删除研究会导致系统同时删除存储在系统根中的该研究本地工程数据。默认此设置被清除。更多信息请参阅 Delete（删除）。<br>• **Dim Non Checked-Out Objects（使未签出对象变暗）** — 参见下文以及 Check Out（签出）。<br>• **Block modifications to non-Checked out objects（阻止修改未签出对象）** — 选中后，不能对未签出的任何对象进行更改。详见 Check Out。<br>• **Show Source/Sink of Operations（显示操作的源/汇）** — 在 Object Viewer 中显示。<br>• **Show LocationOperations（显示位置操作）** — 在 Process Designer 树和查看器中显示 LocationOperation 对象，使用户能够验证和记录 Process Simulate 中执行的详细仿真结果。<br>• **Keep absolute position when resource parent changes（资源父项更改时保持绝对位置）** — 设置后，资源在移动并嵌套到复合资源下的不同父项时保持其绝对位置。清除后，资源保持其相对于父项的位置。如果新父项与旧父项不在同一位置，资源会移动到新位置，导致新的 3D 布局。 |
| **Update Prompt（更新提示）** | 勾选 **Every** 以启用 Auto Save（自动保存）并设置弹出提醒（提示您保存工作）的频率（分钟）。默认值为 60 分钟，可设置 1 到 9999 分钟之间的任意值。 |
| **Preview Image（预览图像）** | 设置 **Capture cell preview upon Update eMServer（执行 eMServer 更新时捕获单元预览）**，以在执行 eMS 服务器更新时保存单元图像。 |
| **Messages（消息）** | 单击 **Messages** 显示 Message Options（消息选项）对话框。在 Message Options 区域，单击 **Retrieve（检索）** 以检索使用“Do not show this message again（不再显示此消息）”选项抑制的消息。在 Special Behavior 区域，**Show errors in prompt line only（仅在提示行显示错误）** 选项允许您在 Process Designer 窗口底部状态栏的提示行中显示所有系统消息。未选中时，系统消息显示在消息框中，您需要单击 OK 确认。 |
| **Library Root（库根）** | 单击 **Library Root** 显示 Library Root Options（库根选项）对话框。Library Root 字段定义系统库根目录。要更改目录，单击 Browse，导航到所需位置，并单击 OK。库根必须设置在系统根下一级。例如，若 System root = `c:\project\SystemRoot`，则 Library root = `c:\project\SystemRoot\LibraryRoot`。有关如何设置系统根的信息，请参阅 eMServer Tab。 |
| **Point Cloud（点云）** | 单击 **Point Cloud** 显示 Point Cloud Options（点云选项）对话框。选择存储点云文件的文件夹并单击 OK。 |
| **Dim Non-Checked Out Objects（使未签出对象变暗）** | 此选项使 Graphic Viewer 中已签入或被其他用户签出的对象变暗。这样您可以轻松区分这些对象与您已签出并可操作的对象。启用此选项后，当您和其他用户在 eMServer 中签入和签出对象时，它会自动更新 Graphic Viewer 中的显示。<br>要使 Graphic Viewer 中未签出的对象变暗：从 Options 对话框的 General 选项卡激活 Dim Non-Checked Out Objects 命令。单击 Dim Non Check-Out Objects 复选框并单击 OK。Graphic Viewer 中的显示会立即更新，使已签入或被其他用户签出的对象变暗（如下图所示）。<br>**注意：** 要恢复变暗的对象，重新选择 Dim Non-Checked Out Objects 选项。 |

<a id="v2-s28"></a>
### Graphics Viewer Tab（图形浏览器选项卡）

Graphics Viewer（图形浏览器）选项卡（如下图所示）包含用于指定鼠标查看控制，以及在 Manipulator（操纵器）帧上显示 Orientation Frame of Reference（方向参考系）和 Manipulator plane handles（操纵器平面手柄）的选项。

Graphics Viewer 选项卡包含以下参数：

| 参数（Parameter） | 说明（Description） |
| --- | --- |
| **Direct Viewing（直接查看）** | 选中后，鼠标查看选项（缩放、平移和旋转）通过按下和释放鼠标按钮来控制。有关更多信息，请参阅 Zoom to Fit（缩放以适应）。 |
| **Continuous Viewing（连续查看）** | 选中后，启用所选鼠标查看选项的连续查看。例如，按住鼠标按钮放大对象然后释放按钮后，缩放操作会根据 Continuous Viewing 滑块指定的查看速度继续。 |
| **Process Simulate users（Process Simulate 用户）Rotation method（旋转方法）** | 选择此选项可使对象按先前 Tecnomatix 应用的方式旋转（与下方的 Vis users 选项相反）。 |
| **Vis users（Vis 用户）Rotation method** | 对象旋转方向与鼠标移动方向相同，如同在 Teamcenter 和 Vis 产品中（不同于早期版本的 Tecnomatix 应用）。 |
| **Primary light source intensity（主光源强度）** | 控制显示照明的滑块。将此调整与次级光源强度控制配合使用，以帮助提高 3D 数据的可见性。 |
| **Secondary light source intensity（次级光源强度）** | 平衡 Graphic Viewer 中次级光源与主光源强度的滑块。 |
| **Display Manipulator Plane Handles（显示操纵器平面手柄）** | 选中后，在 Manipulator 帧上显示白色 X、Y 和 Z（线性）平面手柄。使用 Graphic Viewer 工具时，平面手柄可用于在 Graphic Viewer 中拖动组件。 |
| **Display Orientation Frame of Reference（显示方向参考系）** | 选中后，当箭头位于 Graphic Viewer 中显示的对象上方时显示工具提示。 |
| **Enable Selection preview（启用选择预览）** | 设置后，在 Graphic Viewer 中将鼠标从一个对象移到另一个对象时，每个后续对象会以选择预览颜色高亮，而前一个对象恢复为非高亮颜色。 |
| **Display Tooltips（显示工具提示）** | 选中后，当箭头位于 Graphic Viewer 中显示的对象上方时显示工具提示。 |
| **Enable anti-aliasing（启用抗锯齿）** | 移动滑块以在图形显示质量（更平滑的线条）和性能速度之间取得平衡。更改在下次启动应用程序时生效。 |
| **Feature line angle（特征线角度）** | 在 Wireframe Mode（线框模式）下，当相邻平面之间的断裂角度在 1 到 45 度之间时，Graphic Viewer 可在平面边界绘制特征线。此字段允许您设置最小断裂角度（默认值为 35 度）。 |
| **Feature line width（特征线宽度）** | 允许您设置特征线的宽度，范围为 1–3 像素（默认值为 1 像素）。 |
| **Show silhouette in Wireframe Mode（在线框模式下显示轮廓）** | 设置后，Wireframe Mode 下 Graphic Viewer 中对象周围可见轮廓线。这有助于查看可见性较低的对象。<br>**注意：** 轮廓线不可被拾取。 |

<a id="v2-s29"></a>
### Motion Tab（运动选项卡）

Motion（运动）选项卡包含用于配置关节运动极限的参数。

您可以从 Motion 选项卡配置以下参数：

| 参数（Parameter） | 说明（Description） |
| --- | --- |
| **Limit joint motion（限制关节运动）** | 设置后，机器人关节在达到其物理极限时停止运动。清除后，机器人关节可自由运动，覆盖所有关节极限（物理和工作极限）。 |
| **Indicate joint working limits（指示关节工作极限）** | 设置后，Process Designer 计算并显示关节极限颜色指示。 |
| **Highlight joint limits in Graphics Viewer（在 Graphic Viewer 中高亮关节极限）** | 设置后，Graphic Viewer 显示关节极限颜色指示。<br>**注意：** 此选项消耗大量计算机资源。系统会在启用前提示您。 |
| **Joint working limits（关节工作极限）** | 允许您通过以下任一方式配置关节工作极限：<br>• **Percentage（百分比）** — 工作极限为物理极限的百分比。您可以将工作极限设置为物理极限的 0% 到 30%。<br>• **Absolute（绝对值）** — 为 Prismatic joints（移动关节）和 Revolute joints（旋转关节）输入绝对工作极限值。 |

<a id="v2-s30"></a>
### Performance Tab（性能选项卡）

Performance（性能）选项卡（如下图所示）包含用于指定显示级别和图形性能质量的参数。

Performance 选项卡包含以下参数：

| 参数（Parameter） | 说明（Description） |
| --- | --- |
| **Level of details（细节级别）** | 通过移动滑块到所需位置，在 Graphic Viewer 中显示的细节级别在高质量和改进性能之间切换。 |
| **Decrease level of details while changing view point（更改视点时降低细节级别）** | 选中后，更改视点时会降低细节级别。 |
| **Cull parts with less than（剔除小于…的零件）** | 选中后，3D 数据中的细小细节会被隐藏。以完整显示的百分比指定大小。显示较少的零件可提高显示性能。默认未激活此选项。 |
| **Use background loading（使用后台加载）** | 使 Graphic Viewer 能够在加载原型文件时显示对象。您可以在后台加载期间以秒为单位配置图形显示重绘速率。默认激活此选项。 |
| **Fixed frame rate（固定帧率）** | 确保动画的最小帧率（每秒帧数，FPS）。为保持帧率，Direct Model 查看器会在必要时降低显示细节级别。默认未激活此选项。 |
| **Direct Model Settings - OpenGL acceleration level（Direct Model 设置 - OpenGL 加速级别）** | 默认情况下，系统选择硬件（显卡类型）支持的最高版本。如果这导致图形失真，您可以选择较低的 OpenGL 版本。更改在下次启动应用程序时生效。<br>**注意：** 请注意，这可能会导致禁用某些高级图形功能并使性能变慢。对于虚拟机和远程桌面连接，最高支持的 OpenGL 加速级别为 V1.1（基线渲染功能）。 |
| **Memory（内存）** | 设置内存 Limit（限制）值（作为物理内存的百分比）以定义应用程序允许消耗的最大内存量。当系统达到内存限制时，您可以选择 Display a warning（显示警告），提示内存限制将被超出。 |

**建议（Recommendations）：**

- 在大多数情况下，默认性能参数设置是令人满意的。仅当遇到特定问题时才进行更改。
- Level of details 是较高图形质量与改进系统性能之间的“折中”。如果性能太慢，请将 Level of details 滑块向“Speed（速度）”方向移动，直到您对性能满意并可以接受降低的质量。如果图形质量不够好，请将 Level of details 滑块向“Quality（质量）”方向移动，直到您对图形质量满意并可以接受降低的性能。
- 要提高图形性能，您可以开启 size culling（大小剔除）。如果组件被分割成微小形状，则不建议使用此选项。您还可以自定义剔除大小值，以调整图形性能与被剔除形状数量之间的平衡。
- 要减少内存消耗：将 Level of details 滑块向“Speed”方向移动和/或开启 size culling。这会减少加载大量数据所需的初始内存消耗（因为加载的形状更少），同时提高性能。但是，在长时间工作流程中放大到 3D 数据的不同区域后，启动时未加载的形状会被逐渐加载。您可以将提高“Speed”与内存限制结合使用，以创建稳定的环境。

> **注意（32 位系统）：** 对于 32 位系统，设置内存限制值（以 MB 为单位）以定义应用程序允许消耗的最大内存量。
> - 内存值限制的默认值为 2000 MB。降低此值会减少因超出可用内存资源而导致系统崩溃的可能性。但是，这会在加载极大量数据和执行缩放/平移操作时降低性能。
> - 仅当可在不影响功能的情况下卸载的数据量足够大时，内存限制才有帮助。如果您没有足够的内存用于研究的初始加载，此选项无效。如果您成功加载了数据，执行缩放/平移会卸载（变为不可见的）形状以加载需要可视化的其他形状。
> - 对于 Windows XP 32 位：当内存不足以加载大型数据集时，Siemens 建议开启 /3GB Boot 参数。有关配置 /3GB 的详细信息，请参阅 http://msdn.microsoft.com/en-us/library/ff556232.aspx。

<a id="v2-s31"></a>
### Units tab（单位选项卡）

Units（单位）选项卡（如下图所示）包含用于指定当前线性、角度、质量和时间计量单位的参数。

Units 选项卡包含以下参数：

| 参数（Parameter） | 说明（Description） |
| --- | --- |
| **Linear（线性）** | 用于从下拉列表中选择工程数据的线性计量单位。 |
| **Angular（角度）** | 用于从下拉列表中选择工程数据的角度计量单位。 |
| **Mass（质量）** | 用于从下拉列表中选择工程数据的质量计量单位。 |
| **Time（时间）** | 用于从下拉列表中选择工程数据的时间计量单位。 |

使用每种单位类型旁边的 spin box（数字微调框）指定该计量的小数位数。

<a id="v2-s32"></a>
## Exit（退出）

Exit（退出）选项用于关闭 Process Designer 应用程序。

选择 **File 选项卡 → Exit**。您也可以使用键盘快捷键 `<Alt+F4>` 退出查看器。

如果在 Options 对话框的 eMServer Tab 中设置了 Notify about checked-out objects when closing project（关闭项目时通知已签出对象）设置，Process Designer 会在退出前显示一个对话框，列出已签出的项。
<a id="v3-s1"></a> <!-- p389 -->
# 3. 主页（Home）

<a id="v3-s2"></a> <!-- p389 -->
## 欢迎页面（Welcome page）

启动 Process Designer 后，将显示欢迎页面（Welcome page）。

欢迎页面包含以下元素：

1. **最近文件列表（Recent Files）**——保留您最近访问过的至多 15 个文件。单击某个文件即可加载它。单击文件旁的星标可使该文件始终保持在列表顶部，从而实现"收藏夹"列表的效果。

   > **注意**
   > 您可以右键单击以将某个文件从"最近文件"列表中移除。

2. **选项卡（Tabs）**

   - **Welcome**——主窗口。
   - **What's New**——列出当前版本的新增功能。

     单击任意条目可将其放大并阅读更多细节，随后单击 **Back to New Capabilities** 返回 What's New 页面。单击右上角的 **Open Release Notes** 可阅读当前版本全部新增功能的相关信息。
   - **Useful Links**——链接到 Siemens Digital Industries Software 的各类在线资源。

   您可以设计一个包含贵组织定制信息的 HTML 页面，并通过一个新选项卡访问它，操作如下：

   a. 为该 HTML 页面命名（例如 MyCompanyInfol），并将其存放于 `<installation_folder>eMPower\LandingPage\CustomerPages\Page1\`。

   b. 重新启动 Process Designer。此时欢迎页面中将出现您的新选项卡，并带有一个默认图标。

   c. 您可以将 PNG 格式的自定义图标存放于 `<installation_folder>eMPower\LandingPage\CustomerPages\Page1\icon`，并最多再添加五个页面，路径为 `<installation_folder>eMPower\LandingPage\CustomerPages\Page<n>`。安装包中随附了一个示例页面，可用作模板。若要在 Welcome 对话框中显示该示例页面，请进入 Page1 文件夹，将 `Example.html_` 去掉下划线重命名为 `Example.html`。

3. 该区域以幻灯片形式播放当前版本的新增功能。单击右下角的箭头可显示所选功能的更多信息。单击 **Back to New Capabilities** 可跳转到 What's New 页面。

4. 用于访问 **Options**（选项）对话框。

5. 显示当前配置的系统根目录（system root）。欢迎页面允许您更改系统根目录，您也可以在 **eMServer** 选项卡中进行更改。

6. 用于 **Open a Project**（打开项目）。

7. 链接到各社交网络中的 Tecnomatix 页面。

<a id="v3-s3"></a> <!-- p392 -->
## 变体过滤器（Variant Filter）

变体过滤器（Variant Filter）选项使您能够显示项目数据的某一选定变体。变体表示基于产品差异而配置的工艺差异。例如，某汽车制造商可能生产同一车型的硬顶版和敞篷版，二者在装配基础车型时所用的零件、操作和资源均存在变体差异。应用该过滤器后，各视图中仅显示与所选变体匹配的对象。有关配置变体的信息，请参阅 Variants。

**应用变体过滤器：**

**操作步骤**

1. 选择 **Home** 选项卡 → **Variants** 组 → **Apply Variant Filter**。系统将显示一条消息，询问是否在继续之前保存工程数据。Process Designer 通过重新加载所选数据来应用变体过滤器，因此此时尚未保存的任何工程数据都将丢失。

2. 单击 **Yes** 保存工程数据，或单击 **No** 不保存数据直接继续。随即显示 **Apply Variant Filter** 窗口。

3. 单击视图左上角的 **VariantFilter** 图标。**Apply Variant Filter** 对话框随即打开。默认情况下，它会列出该项目的变体过滤器库，且对话框左上角显示 **Variant Filters** 字样。

4. 若要为对话框选择不同的查看选项，请单击对话框左上角（Variant Filters 字样右侧）的箭头。

   随即打开一个包含五个选项的下拉列表。所选项决定了 **Apply Variant Filter** 对话框上部窗格中显示的内容：

   - **all Criteria**——同时列出本项目的基本条件和可选条件。所显示的条件可用于构建变体表达式。
   - **Basic Criteria**——仅列出本项目的基本条件。所显示的条件可用于构建变体表达式。
   - **Variant Sets**——列出本项目的变体集（Variant Set）库。可展开库列表以按名称和变体表达式显示指定库中的变体集。所列出的变体集可用于构建变体表达式。
   - **Variant Types**——由适用列表定义，同样可用于构成表达式。
   - **Variant Filters**——列出本项目的变体过滤器库。可展开库列表以按名称和变体表达式显示指定库中的变体过滤器。所列出的变体过滤器**不能**用于构建变体表达式。

   下图显示了展开后的变体过滤器库。

5. 执行以下操作之一：

   - 选择一个现有的变体过滤器/变体集指定给该视图。
   - 选择一个现有的变体过滤器或变体集，对其进行编辑，并将其另存为指定给该视图的新变体过滤器。
   - 构建新的变体表达式和变体过滤器。

   **Apply Variant Filter** 对话框关闭后，您所选择的待应用过滤器将显示在视图左上角 **VariantFilter** 图标的右侧。

6. 单击 **OK**。所指定过滤器的名称将显示在视图顶部标题栏中 **VariantFilter** 图标的右侧。

<a id="v3-s4"></a> <!-- p394 -->
#### 取消指定变体过滤器（Unassigning a Variant Filter）

**为视图取消指定任何变体过滤器：**

**操作步骤**

1. 单击视图左上角的 **VariantFilter** 图标。**Apply Variant Filter** 对话框随即打开。

2. 单击 **no Filter**。视图顶部标题栏中 **VariantFilter** 图标右侧将显示"No Filter Applied"。

3. 从显示的列表中选择变体过滤器并单击 **OK**。当前会话将被清空，并重新加载与所选过滤器匹配的数据。3D 数据按照按需加载（Load on Demand）功能进行显示。

<a id="v3-s5"></a> <!-- p394 -->
## 指定/取消指定变体集（Assigning/Unassigning Variant Sets）

使用变体过滤功能，您可以在特定视图中过滤掉不关注的对象（参见 Objects Supporting Variant Sets）。

要使用变体过滤，您必须为一个或多个对象指定变体集（Variant Set），并为一个或多个视图应用变体过滤器。

有关可指定变体集的对象类型列表，请参阅 Objects Supporting Variant Sets。

为对象指定变体集时请注意：如果这些对象当前显示在某个视图中，而该视图的变体过滤器与为对象指定的变体集不一致，则这些对象将被过滤掉并从该视图中消失。

<a id="v3-s6"></a> <!-- p395 -->
### 为对象指定变体集（方法 1）

**为对象指定变体集：**

**操作步骤**

1. 按 Variant Set Editor 中所述，对该对象打开变体集编辑器（Variant Set Editor）。

2. 执行以下操作之一：

   - 选择 **Variant Sets** 选项（位于变体集编辑器中），并选择一个已存在的公共变体集指定给目标对象。
   - 选择一个现有变体集，对其进行编辑，并将其另存为指定给该对象的新公共变体集或新私有变体集。
   - 构建新的变体表达式和新的变体集（公共或私有均可）。

3. 单击 **OK**。所选对象即被指定该变体集。对于公共变体集，所指定变体集的名称显示在树窗口右侧的 **Variant Set** 列中；对于私有变体集，则显示其变体表达式。

<a id="v3-s7"></a> <!-- p395 -->
### 为对象指定变体集（方法 2）

**为对象指定变体集：**

**操作步骤**

1. 在任意视图中显示目标对象。
2. 显示包含目标变体集的变体集库。
3. 将目标变体集拖放到目标对象上。

<a id="v3-s8"></a> <!-- p395 -->
### 为一个或多个对象指定变体集（方法 3）

**为对象指定变体集：**

**操作步骤**

1. 在任意视图中显示目标对象。
2. 显示包含目标变体集的变体集库。
3. 将目标对象拖放到目标变体集上。

<a id="v3-s9"></a> <!-- p396 -->
### 为一个或多个对象指定变体集（替代方法）

**为一个或多个对象指定变体集：**

**操作步骤**

1. 在任意视图中显示目标对象。
2. 右键单击目标变体集，随即打开一个菜单。
3. 在该菜单中右键单击 **Properties**，随即打开一个对话框。
4. 在该对话框中选择 **Target Objects** 选项卡，其中列出了使用该变体集的所有对象。

   > **注意**
   > 对于关联对象数量非常庞大的变体集，系统可能需要数分钟才能显示该列表。

5. 将目标对象拖放到 **Target Objects** 选项卡上。

<a id="v3-s10"></a> <!-- p396 -->
### 为链接指定变体集（Assigning a Variant Set for Link）

您可以为操作与资源之间的**链接**（而非实际的操作与资源本身）激活变体集。

对于操作的 **Properties** 对话框中的某个资源，选择 **Special Data** 选项卡 → **Variants** 组 → **Variant Editor** → **Variant Editor for Link**。

<a id="v3-s11"></a> <!-- p396 -->
### 从对象取消指定变体集（推荐方法）

**从对象取消指定变体集：**

- 选择该对象，然后选择 **Home** 选项卡 → **Variants** 组 → **Clear Variant Set for Link**。

<a id="v3-s12"></a> <!-- p397 -->
### 从对象取消指定变体集（替代方法）

**从对象取消指定变体集：**

**操作步骤**

1. 右键单击目标变体集，随即打开一个菜单。
2. 在该菜单中右键单击 **Properties**，随即打开一个对话框。
3. 在该对话框中选择 **Target Objects** 选项卡，其中列出了使用该变体集的所有对象。

   > **注意**
   > 对于关联对象数量非常庞大的变体集，系统可能需要数分钟才能显示该列表。

4. 右键单击目标对象，随即打开一个菜单。
5. 选择 **Delete**，随即打开一个对话框。
6. 单击 **Yes**。该变体集即从目标对象上取消指定。

<a id="v3-s13"></a> <!-- p397 -->
## 签入（Check In）

请参阅 Multi-User Concurrent Access（多用户并发访问）。

**Check In** 命令仅适用于已签出的节点（以相应图标标记）。

**Check In** 窗口包含以下选项：

| 项目 | 说明 |
| --- | --- |
| **Name** | 新版本的名称。除非选中 **Check In As New Version**，否则该项呈灰显状态。 |
| **Check In As New Version** | 使用此选项可将节点签入为一个新版本。 |
| **Comment** | 文本框。 |
| **Check In With Hierarchy** | 选中此选项可签入项目树中所有相应对象；清除该选项则仅签入所选节点。只有在设置了 **Check In With Hierarchy** 时，**Include Module Content** 选项才可用。 |
| **Include Module Content** | 设置后，签入操作将包含模块内容。 |
| **Keep Objects Checked Out** | 选中后，**Check In** 命令会更新数据库，但保持节点处于签出状态以便继续修改。其他用户仅具有查看权限。 |
| **Details** | 单击可打开 **Related Objects to be Checked In**（待签入的相关对象）列表。请注意，该功能会占用机器资源，在编译"待签入的相关对象"列表期间会暂时影响系统性能。 |
| **Related Objects to be Checked In** | 所选对象将与模块一并签入。 |
| **Locals** | 用于查看任何私有（本地）版本。 |

**操作步骤**

1. 若要签入某节点，请先选中该节点。

2. 通过以下任一方式打开 **Check In** 窗口：

   - 选择 **Home** 选项卡 → **CIO** 组 → **Check In**。
   - 右键单击该节点，从上下文菜单中选择 **Check In**。

   随即出现相应的 **Check In** 窗口。

3. 选择所需选项并单击 **OK**。

> **注意**
> 修改标题（caption）的公式后，必须重新执行签入和签出，其他用户才能获得该更新。

<a id="v3-s14"></a> <!-- p399 -->
## 签出（Check Out）

请参阅 Multi-User Concurrent Access（多用户并发访问）。

**Check Out** 命令会锁定指定节点，使其仅可由执行签出的用户编辑。

**Check Out** 窗口包含以下选项：

| 项目 | 说明 |
| --- | --- |
| **Check Out With Hierarchy** | 选中后一并签出所有子树节点。 |
| **Intended for extensive data change** | （在启用 **Check out with hierarchy** 时可用）签出数据时预期将对签出对象执行大量修改（例如使用表视图更改大多数对象的属性）。若不勾选此项，签出耗时较短，但首次修改对象时性能可能略受影响。若 **Check out with hierarchy** 处于禁用状态，则忽略此选项（无论复选框状态如何）。 |
| **Include Module Content** | 选中后将连同项目树中所有相应对象一并签出模块内容。 |
| **Details** | 单击可打开 **Related Objects to be Checked Out**（待签出的相关对象）列表。 |
| **Related Objects to be checked out** | 所选对象将与模块一并签出。 |

**操作步骤**

1. 若要签出节点，请选中要签出的一个或多个节点。

2. 通过以下任一方式打开 **Check Out** 窗口：

   - 选择 **Home** 选项卡 → **CIO** 组 → **Check Out**。
   - 右键单击并从上下文菜单中选择 **Check Out**。

   **Check Out** 窗口随即打开。

3. 选择所需选项并单击 **OK**。

> **注意**
> 修改标题（caption）的公式后，必须重新执行签入和签出，其他用户才能获得该更新。

<a id="v3-s15"></a> <!-- p400 -->
## 取消签出（Cancel Check Out）

**Cancel Check Out** 命令会解锁所选节点及与之相关的所有节点，同时撤销模块签出期间所做的全部更改。这些模块随即以签出前的状态返回到公共工作区。

**Cancel Check Out** 窗口包含以下选项：

| 项目 | 说明 |
| --- | --- |
| **Cancel Check Out With Hierarchy** | 选中后一并取消所有子节点的签出。 |
| **Include Module Content** | 连同项目树中所有相应对象一并取消模块内容的签出。不勾选该复选框则保持项目树中相应对象的签出状态。 |
| **Details** | 单击可打开 **Related Objects to Cancel Check Out**（待取消签出的相关对象）列表。 |
| **Related Objects to Cancel Check Out** | 在此列表中选中的对象将被取消签出。 |
| **Locals** | 用于查看任何私有（"本地"）版本。 |

**操作步骤**

1. 若要取消签出，请选中相应节点。

2. 通过以下任一方式打开 **Cancel Check Out** 窗口：

   - 选择 **Home** 选项卡 → **CIO** 组 → **Cancel Check Out**。
   - 右键单击并从上下文菜单中选择 **Cancel Check Out**。

   **Cancel Check Out** 窗口随即打开，其中列出将被取消签出的节点。

3. 选择所需选项并单击 **OK**。

<a id="v3-s16"></a> <!-- p402 -->
## 能力条（Power Bar）

<a id="v3-s17"></a> <!-- p403 -->
### 能力条搜索工具（Power Bar search tools）

能力条（Power Bar）提供了多种用于查找对象的搜索和过滤工具。您可以将搜索结果加载到对象查看器（Object Viewer）或图形查看器（Graphic Viewer）中。

Process Simulator 包含两个强大的搜索工具：

- **Results** 和 **Filters**——针对所选节点中的对象类型执行查询，例如某个操作内的 MFG，或未指定的 MFG。查询结果会自动加载到各树和图形查看器中。
- **Search**——对整个数据库执行搜索。您可以通过单击 **Open Results List** 加载搜索结果。

这些工具简化了对已指定或未指定的零件、焊枪或资源的识别过程。在结果列表中，您可以查看对象，方便地指定或取消指定任何对象，并保存搜索结果。

能力条包含以下按钮：

| 图标 | 说明 |
| --- | --- |
| **Open Search Tool** | 打开用于定义和启动搜索的对话框。请参阅 Search。 |
| **Quick Find** | 打开一个对话框，用于定义区分大小写的搜索，以在当前项目、用户文件夹或用户自定义搜索范围的层次结构中定位节点。该搜索同时会在快捷方式文件夹中查找。 |
| **Run Current Search** | 运行最近一次使用的搜索，并打开窗格显示结果。 |
| **Run Current Query** | 按照所选选项将新数据从数据库加载到当前会话中。请参阅 Power Bar queries。 |
| **Filter Existing Data** | 过滤现有数据，例如未指定的零件或特定颜色。请参阅 Filters。 |
| **Open Results List** | 显示搜索/查询结果的列表。请参阅 Results。 |
| **Open Settings Dialog** | 打开用于定义查询设置的对话框。请参阅 Query Settings。 |

要访问能力条，请选择 **Server** 选项卡 → **Study** 组 → **Power Bar**。

<a id="v3-s18"></a> <!-- p404 -->
### 结果（Results）

执行查询时会自动显示查询和搜索结果。结果显示在 **Results** 列表中，并在查看器和相关树中高亮显示。结果数量显示在能力条上，查询名称显示在能力条图标与 **Results** 列表之间。您可以通过拖动 **Results** 列表的角来调整其显示大小。

使用 **Query** 选项时，任何尚未加载的结果都会自动加载到各树中。已加载对象的名称在 **Results** 列表中以粗体显示。您可以按下文所述在查看器中操作这些结果。

您可以在 **Results** 列表打开的状态下更改查询并运行，**Results** 列表将动态更新。

> **注意**
> 原型（Prototype）可以从 **Results** 列表拖动到各树中，但不能加载到图形查看器。尝试这样做将导致错误消息。

**Results** 列表为搜索/查询结果提供以下显示选项：

| 图标 | 说明 |
| --- | --- |
| **Load Objects** | 将所选项加载到查看器。查询结果会自动加载。此选项适用于搜索场景，以及对象已从视图中擦除之后。 |
| **Modify Color** | 修改所选对象的颜色。 |
| **Display Only Selection** | 仅显示所选对象。 |
| **Display Selection** | 加载并显示 **Results** 列表中所有选定的对象。 |
| **Blank Selection** | 在选择一项或多项并单击加载后，将这些项在查看器中隐藏（消隐）。 |

<a id="v3-s19"></a> <!-- p405 -->
### 查询（Queries）

<a id="v3-s20"></a> <!-- p405 -->
#### 能力条查询（Power Bar queries）

查询通过选择一个节点并激活其中一项查询来执行。查询是否有效取决于所选节点的类型与查询的类型。

所有查询均由其查询设置（Query Settings）定义，可通过单击 **Open Settings Dialog** 图标访问。

**操作步骤**

1. 在任意树中选择一个对象。

2. 单击当前查询图标以选择默认（上次运行的）查询，或单击向下箭头并从菜单中选择一项可用查询。有关每种查询/过滤器的完整说明，请参阅 Query Settings。

   | 工具 | 说明 | 适用范围 |
   | --- | --- | --- |
   | **Find Operations Parts** | 检索与所选工艺相关的所有操作零件。 | 对象查看器、导航树中的操作等 |
   | **Find Operations MFGs** | 检索与所选工艺相关的所有 MFG。 | 对象查看器 |
   | **Find Operations Resources** | 检索与所选工艺相关的所有资源。 | 对象查看器 |
   | **Find Operations Output** | 检索所有被消耗的零件。 | — |
   | **Find All Parts MFGs** | 检索所选产品的所有已指定 MFG。 | 产品树 |
   | **Find Assigned Parts MFGs** | 检索已指定零件的所有 MFG。 | — |
   | **Find Unassigned Parts MFGs** | 检索未指定零件的所有 MFG。 | — |

3. 结果显示在 **Results** 列表中。

<a id="v3-s21"></a> <!-- p406 -->
#### 查询设置（Query settings）

<a id="v3-s22"></a> <!-- p406 -->
##### Find Operations Parts

**Find Operations Parts** 查询选项检索所选操作的全部零件。

该查询的 **Settings** 窗口包含以下选项：

| 选项 | 说明 |
| --- | --- |
| **Consumed/Incoming Parts** | 检索所有被消耗的零件。 |
| **Consumed Parts** | 检索在所选操作之前装配的所有零件以及输入零件。 |
| **With Hierarchy** | 检索在所选操作之前装配的零件及其层次结构，以及孪生对象。 |
| **Produced Parts** | 仅检索在所选操作中装配的零件。 |
| **Load Query Results** | 将搜索结果加载到树窗格和图形查看器中。 |

<a id="v3-s23"></a> <!-- p406 -->
##### Find Operations Resources

**Find Operations Resources** 查询选项检索与所选工艺相关的所有资源。

该查询的 **Settings** 窗口包含以下选项：

| 选项 | 说明 |
| --- | --- |
| **Selected Operations** | 仅检索指定给所选操作的资源（不含层次结构），以及孪生对象。 |
| **With Hierarchy** | 检索所选操作及其层次结构的资源，以及孪生对象。 |
| **Load Query Results** | 将搜索结果加载到树和图形查看器中。 |

<a id="v3-s24"></a> <!-- p407 -->
##### Find Operations MFGs

**Find Operations MFGs** 查询选项检索指定给所选工艺的全部 MFG。

该查询的 **Settings** 窗口包含以下选项：

| 选项 | 说明 |
| --- | --- |
| **Consumed/Incoming Parts → Selected Operations** | 仅检索指定给所选操作的 MFG（不含层次结构），以及孪生对象。 |
| **With Hierarchy** | 检索所选操作及其层次结构的 MFG，以及孪生对象。 |
| **Output Types → All** | 检索所有可用的输出类型。 |
| **Specify** | 检索您在随后列表中指定的输出类型。 |
| **Load Query Results** | 将搜索结果加载到树和图形查看器中。 |

<a id="v3-s25"></a> <!-- p408 -->
##### Find All Parts MFGs

**Find All Parts MFGs** 查询选项检索所选产品的所有已指定 MFG。

该查询的 **Settings** 窗口包含以下选项：

| 选项 | 说明 |
| --- | --- |
| **Exact Match** | 仅检索属于本次查询所选零件的 MFG。 |
| **Partial Match** | 检索至少属于本次查询所选一个零件的所有 MFG。 |
| **Output Types → All** | 检索所有可用的输出类型。 |
| **Specify** | 检索您在随后列表中指定的输出类型。 |
| **Load Query Results** | 将搜索结果加载到树和图形查看器中。 |

<a id="v3-s26"></a> <!-- p409 -->
##### Find Assigned Parts MFGs

**Find Assigned Parts MFGs** 查询选项检索所选产品的所有已指定零件及 MFG。

该查询的 **Settings** 窗口包含以下选项：

| 选项 | 说明 |
| --- | --- |
| **Exact Match** | 仅检索属于本次查询所选零件的 MFG。 |
| **Partial Match** | 检索至少属于本次查询所选一个零件的所有 MFG。 |
| **Output Types → All** | 检索所有可用的输出类型。 |
| **Specify** | 检索您在随后列表中指定的输出类型。 |
| **Load Query Results** | 将搜索结果加载到树和图形查看器中。 |

<a id="v3-s27"></a> <!-- p410 -->
##### Find Unassigned Parts MFGs

**Find Unassigned Parts MFGs** 查询选项检索所选产品的所有未指定零件及 MFG。

该查询的 **Settings** 窗口包含以下选项：

| 选项 | 说明 |
| --- | --- |
| **Exact Match** | 仅检索属于本次查询所选零件的 MFG。 |
| **Partial Match** | 检索至少属于本次查询所选一个零件的所有 MFG。 |
| **Output Types → All** | 检索所有可用的输出类型。 |
| **Specify** | 检索您在随后列表中指定的输出类型。 |
| **Load Query Results** | 将搜索结果加载到树和图形查看器中。 |

<a id="v3-s28"></a> <!-- p411 -->
### 快速查找（Quick Find）

<a id="v3-s29"></a> <!-- p411 -->
#### 使用快速查找进行搜索（Search by Quick Find）

**Quick Find** 命令位于能力条上。

当项目处于打开状态时，**Quick Find** 使您能够按标题（caption）、内部 ID 或外部 ID 搜索对象。您可以设置搜索范围，包括当前项目、用户文件夹和快捷方式文件夹。

在错误处理过程中，您可以使用 **Quick Find** 查找出现在错误查看器（Error Viewer）或日志文件中的节点。当您确定了某个阻碍功能执行（例如删除子树）的节点的外部或内部 ID 后，该功能也有助于浏览各树。

搜索结果显示在 **Quick Find Results** 窗口中。在该窗口中您可以选择要在图形查看器中显示的对象。您可以以灵活、交互的方式检查并处理结果，例如执行 Load（加载）、Blank（消隐）、Display（显示）、Check-in（签入）、Check-out（签出）等操作。

1. 单击能力条中的 **Quick Find**。**Quick Find** 对话框随即打开。

2. 在 **Look For** 字段中输入所需文本或 ID。您也可以从剪贴板粘贴文本。最大长度为 1,024 个字符。

   > **注意**
   > 内部 ID 仅包含数字。搜索内部 ID 时若在 **Look For** 框中输入字母，将导致错误。

3. 如果按标题搜索对象，请在 **Search In** 字段中选择搜索范围。使用下拉列表可指定当前项目的某个子项目。从下拉列表中选择 **User Defined** 会激活 **Browse** 按钮，使您能够选择特定的项目节点。

   按 ID 搜索时，搜索范围自动设置为整个项目，且 **Search In** 选项被禁用。

4. 在 **Find Object By** 区域中，选择以下选项之一：

   - **Caption**——选择此项以按标题名称搜索对象。
     - **Match whole word** 将搜索限制为全词匹配（选中该项后搜索 weld 不会返回 welding）。
     - **Match case** 指定区分大小写的搜索（选中该项后搜索 Weld 不会返回 "weld"）。
   - **Internal ID**——选择此项以按内部 ID 搜索对象。
   - **External ID**——选择此项以按外部 ID 搜索对象。

   > **注意**
   > 默认情况下，打开 **Quick Find** 对话框时会选中上次所选的选项。

5. 单击 **Find** 或按 Enter 键。能力条随即展开并显示带有搜索结果的 **Quick Find Results** 窗口。

<a id="v3-s30"></a> <!-- p413 -->
#### 快速查找结果（Quick Find results）

**来自快速查找的搜索结果**

使用快速查找工具执行搜索后，**Quick Find Results** 窗口将打开以显示结果。单击能力条上的 **Results List** 按钮可在显示与隐藏 **Quick Find Results** 窗口之间切换。

**Quick Find Results** 窗口包含搜索所找到对象的列表，以及可对列表中对象执行操作的工具栏按钮。单击列表中的项可选中它们以进行查看、导航或其他操作。右键单击可显示上下文菜单，为所选项提供更多操作，包括签入或签出对象。

您可以单击 **Quick Find Results** 窗口中的按钮以激活以下选项：

| 选项 | 说明 |
| --- | --- |
| **Load Selection** | 将所选项加载到图形查看器中。加载某项会自动加载其所有子项和关系。 |
| **Save Results** | 将快速查找结果保存到用户文件夹中，在导航树的 **Query Result Modules** 下显示。单击 **Save Results** 后，输入一个标题名称以便日后检索。 |
| **Navigation** | 打开一棵新的导航树，从所选节点的根节点开始。最多可同时显示五棵导航树。 |
| **Modify Color** | 修改所选对象在图形查看器中的显示颜色。 |
| **Display Only Selection** | 在图形查看器中仅显示所选对象，隐藏其他对象。 |
| **Display Selection** | 在图形查看器中显示所选对象，但不隐藏其他对象。 |
| **Blank Selection** | 在图形查看器中隐藏所选对象。 |

<a id="v3-s31"></a> <!-- p415 -->
##### 上下文菜单（Context menu）

在 **Quick Find Results** 窗口中右键单击可显示上下文菜单，为所选项提供以下选项：

| 名称 | 说明 |
| --- | --- |
| **Display** | 在图形查看器中显示所选对象。 |
| **Blank** | 在图形查看器中隐藏所选对象。 |
| **Display Only** | 在图形查看器中仅显示所选对象，隐藏其他对象。 |
| **Load** | 将所选项加载到图形查看器。加载某项会同时加载其所有子项和关系。 |
| **Check In** | 签入所选项。 |
| **Check Out** | 签出所选项。 |
| **Cancel Check Out** | 取消所选项的签出。 |
| **Properties** | 为该对象打开属性查看器（Properties Viewer）窗口。最多可同时打开五个属性查看器窗口。 |

<a id="v3-s32"></a> <!-- p415 -->
### 过滤器（Filters）

过滤通过激活其中一个过滤器来执行。当前会话中各树所显示的全部信息均可被过滤，导航树中显示的信息除外。

过滤器类型如下：

| 工具 | 说明 |
| --- | --- |
| **Filter by Assigned / Unassigned Parts** | 检索已指定给/未指定给任何操作的零件。 |
| **Filter by Assigned / Unassigned MFGs** | 检索已指定给/未指定给任何操作的 MFG。 |
| **Filter by Name** | 按指定名称过滤节点。 |
| **Filter by Color** | 按指定颜色过滤对象：**Use Specific Color** 使您能够从调色板中选择任意颜色；**Use Object Color** 使您能够选择任意对象并使用其颜色。 |
| **Filter by Bounding Box** | 按所选包围盒（bounding box）过滤。 |

<a id="v3-s33"></a> <!-- p416 -->
### 搜索（Search）

<a id="v3-s34"></a> <!-- p416 -->
#### 搜索整个数据库（Searching the entire database）

搜索工具（Search Tool）在整个数据库范围内执行搜索，而不仅限于某一选定的对象类型。您可以按照属性或关系搜索任何类型的对象。搜索提供了一种识别尚未指定到流程中的 MFG 的手段。

搜索分为两部分：**Query Relations**（查询关系）和 **Query Attributes**（查询属性）。

搜索可检索：

- **Mfgs：**
  - Mfg Features（制造特征）
  - Principle Locating Points（主定位点，PLP）
  - Weld Point（焊点）
  - Gun（焊枪）
- **Resources：**
  - Robot（机器人）
  - Container（容器）
- **Variant Sets**（变体集）

搜索结果显示在 **Results** 列表中。您可以按 Results 中所述操作这些结果。

**操作步骤**

1. 单击能力条中的 **Search** 图标。**Search Tool** 窗口随即打开。

   > **注意**
   > **Exact Match** 复选框默认为选中状态。这可确保仅对与列表中零件相关联的对象执行搜索。若未选中该复选框，搜索将查找与所选操作相关联的全部零件。此选项仅适用于可指定给零件的对象，例如焊点和 MFG。

2. 在 **Object Type to Search** 列表中选择一种对象类型。可用的对象类型有：

   - MFG Feature
   - PLP
   - Weld Point
   - Gun
   - Robot
   - Container
   - Variant Set

3. 单击 **Define Search Scope**，随即出现 **Define Scope** 窗口。

4. 选择一个或多个要在其上执行搜索的节点。搜索焊点和 MFG 时选择 **Mfg Libraries**；搜索焊枪、机器人和容器时选择 **Resource Libraries**。

5. 单击右箭头图标将所选库移到右侧窗格。（可通过选中对象并单击左箭头图标将其移出 **Search Targets** 窗格。）

6. 若要缩小搜索参数范围，请从 **Query Relations** 区域中选择以下一项或多项（可用参数因 **Object Type to Search** 中的选择而异）：

   - 选中 **Physical Location** 复选框并定义沿 X、Y、Z 轴的搜索坐标。
   - 选中 **Operation Assignment** 复选框并选择 **Assigned**（已指定）或 **Unassigned**（未指定）操作。
   - 选中 **Assigned to Parts** 复选框并从显示的列表中选择所需零件。
   - 从对象查看器中选择一个操作，使其显示在 **Get Operation Parts** 字段中。
   - 选中 **Weld Point Type** 复选框，然后选择以下类型之一：**Dummy**、**Geo** 或 **Respot**。
   - 选择 **Not assigned to any object**（未指定给任何对象）或 **Assigned to at least one object**（至少指定给一个对象）。

7. **[仅焊枪搜索]** 执行焊枪搜索时，**Search Relations** 区域将显示在 **Search Tool** 窗口中央。可按以下焊枪属性进行搜索：

   - Gun Type（焊枪类型）
   - Supplier（供应商）
   - Gun Force（焊枪压力）
   - Wait State Tip Opening（等待状态电极开口量）
   - Max. Tip Opening（最大电极开口量）
   - Throat Depth（喉深）
   - Throat Height（喉高）

8. 若要按对象属性搜索：

   - 打开 **Search Attributes** 窗格。
   - 在第一个下拉列表中选择一项对象属性（列表中仅出现相关属性）。
   - 打开第二个下拉列表（其中包含与所选属性相关的运算符）并选择一项。
   - 在第三个字段中输入一个值。
   - 在窗口的初始尺寸下最多可定义三项属性；单击 **More Attributes** 展开窗口后可再定义三项。

9. 若要按对象关系搜索（当向下箭头可用时该区域可用）：

   - 打开 **Query Relations** 窗格。其中包含位置字段、指定零件、子类型，以及针对焊点/MFG 的 **Assigned to Operation** 字段。

10. 单击 **Search**。查询结果显示在 **Results** 列表中。

<a id="v3-s35"></a> <!-- p421 -->
#### 检索已保存的结果（Retrieve saved results）

**操作步骤**

1. 单击 **Stored Searches**。随即显示 **Stored Searches** 对话框。若选中 **Public** 复选框，则该搜索对所有用户可用。

2. 若要更新搜索，请选择一项已存储的搜索并单击 **Update**。

   > **注意**
   > **Public** 复选框对所有管理员以及具有 Power Bar Search 权限的任何用户启用。

3. 若要加载已存储的搜索，请选择所需搜索并单击 **Load**。

4. 单击 **Close** 关闭对话框。

<a id="v3-s36"></a> <!-- p422 -->
#### PLP 搜索（PLP search）

**搜索 PLP 对象**

**操作步骤**

1. 单击能力条中的 **Search**。**Search Tool** 对话框随即打开。
2. 在 **Object Type to Search** 列表中选择 **PLP** 对象类型。
3. 使用 **Define Search Scope** 按钮选择要搜索的 PLP 库。
4. 按下文所述输入 **Search Relations** 条件。

**搜索关系（Search relations）**

您可以使用以下条件搜索 PLP：

- **By Part Number**——输入某产品（零件）编号以生成 PLP 列表。
- **By PLP Parameters**——包含以下过滤条件：
  - **Physical Location**——指定 X、Y、Z 范围，以搜索坐标位于这些跨度内的 PLP。
  - **Control Direction**——单击复选框以搜索在前后方向（Fore/Aft，X）、横向（Cross Car，Y）和上下方向（Up/Down，Z）上进行约束的 PLP。
  - **Assigned to Parts**——勾选以搜索指定给列表中任何零件的 PLP。
  - **Filter Unassigned**——勾选以仅搜索未指定给操作的 PLP。

**搜索 PLP 用法（Search PLP usages）**

您可以搜索已指定给其他操作的 PLP 用法（usage）。可按操作的用法属性定义搜索。您可将搜索范围指定为包含上一工位，或特定类型的上一工位。

使用 **Search Relations** 参数按以下过滤条件指定搜索：

- **Type**——搜索指定工具类型的 PLP 用法。
- **Control Direction**——单击复选框以搜索在前后方向（X）、横向（Y）和上下方向（Z）上进行约束的用法。
- **Assigned to Parts**——勾选以搜索指定给列表中任何零件的用法。
- **Filter Unassigned**——勾选以仅搜索未指定给操作的用法。

搜索工具还提供高级参数，包括：

- **Previous Station**——对于搜索目标，根据流入所选操作的流指定上一工位的 PLP 用法。
- **Previous Station of Specific Type**——指定上一工位的 PLP 用法，例如工位子类型属性为 Geo 的几何工位。

对某个检索到的 PLP 用法激活 **Properties** 命令（右键单击并选择 **Properties**），可打开该用法所指定到的操作的属性。

<a id="v3-s37"></a> <!-- p424 -->
#### 搜索工位（Search stations）

您可以搜索工位以查找指定给某工位的全部对象。请按工位类型以及所要查找的任何对象来定义搜索。

您可以通过单击搜索类型复选框（**by Part**、**by Tool**、**by Operation**、**by MFG**），然后从导航树中选择相应对象，来填充 **Search Relations** 字段。搜索工具会自动将该对象设置为所选字段中的参数。使用这些搜索关系可查找既符合 **Search Attributes** 又包含所选对象的工位。

<a id="v3-s38"></a> <!-- p425 -->
#### 保存搜索查询和查询结果（Save search query and query results）

您可以存储 **Search** 或 **Quick Find** 的结果以供日后参考。此外，仅对搜索工具而言，您还能够保存搜索本身。所有存储的搜索结果均保存在用户文件夹下的 **Query Result Modules** 中。

**操作步骤**

1. 单击能力条中的 **Open Results List**，随即显示快速查找结果。
2. 单击 **Save Results**，随即显示 **Save Result List** 对话框。
3. 输入用于保存结果的名称。
4. 在 **Comment** 框中输入可选的说明。
5. 单击 **Save**。首次保存结果时，系统会创建一个 **Query result modules** 文件夹，并将当前结果保存在以您所输入标题命名的文件夹中。
<a id="v3-s39"></a> <!-- p427 -->
## 新建（New）

**新建（New）** 选项用于在树中添加新节点。新节点可插入到层级中任一复合（compound）节点之下。所选节点的类型决定了您可以插入的节点类型。

> **注意**
> 除将对象拖放到没有父节点的树窗口中的情况外，Process Designer 始终将新节点作为所选节点的子树（或子节点）插入。（该对象会显示在查看器（Viewer）中，但层级关系不会变化。）

要在导航树（Navigation Tree）中插入新节点：

**操作步骤**

1. 选择所需节点并执行签出（Check Out）。
2. 通过以下任一方式打开 **New** 窗口：
   - 右键单击所选节点并选择 **New**。
   - 选择 **Home** 选项卡 → **Edit** 组 → **New**。

   随即显示 **New** 对话框，其中列出了可在所选节点下创建的节点类型。

   > **注意**
   > 要设置节点类型在 **New** 对话框中的显示顺序，请编辑 `NewCommandConfiguration.xml` 的 `ClassOrder` 标签部分。例如：
   > ```xml
   > <ClassOrder>
   > <GeneralOrder>
   > <PermittedChild name="class PmOperation" />
   > <PermittedChild name="class PmHumanOperation" />
   > <PermittedChild name="class PmWeldOperation" />
   > <PermittedChild name="class PmCompoundOperation" />
   > </GeneralOrder>
   > <Class name="CompoundOperation">
   > <PermittedChild name="class PmOperation" />
   > <PermittedChild name="class PmHumanOperation" />
   > <PermittedChild name="class PmWeldOperation" />
   > <PermittedChild name="class PmCompoundOperation" />
   > ```

3. 通过以下任一方式选择要插入的节点类型：
   - 双击该节点；或
   - 勾选第一列中的节点。

   > **注意**
   > 始终使用 `class` 说明符定义 `PermittedChild` 名称。例如：
   > - 对于 eMServer 类：`<PermittedChild name="class PmCompoundOperation"/>`
   > - 对于用户自定义类：`<PermittedChild name="class UserDefinedClassName"/>`

4. 为每种类型设置所需节点数量。
5. 若要更改 **Node Type** 列中列出的名称，请在 **Name** 列中编辑它。

   > **注意**
   > - 该类型的所有新节点均使用相同名称创建。
   > - 这些名称会被系统保留，供后续会话使用。

6. 要为特定类型的多个节点创建唯一名称，可在 **Name** 列的名称中添加计数器（置于方括号中）。您可以使用数字计数器、字母计数器或二者兼用，格式如下：`NewNode[<起始值>,<增量值>]`。适用规则如下：
   - 若 `<起始值>` 省略前导零，则最高节点值由系统支持的最大整数值决定。
   - 若 `<起始值>` 包含前导零，则最高节点值由 `<起始值>` 的位数决定。例如计数器为两位时，最高节点为 99；为三位时，最高为 999。包含前导零时，节点值也包含前导零，且位数保持恒定。
   - 若省略 `<增量值>`，默认增量为 1。
   - 字母计数器限制为单个字符。
   - 您可以在节点名称之前、之中或之后，任意组合添加数字与字母计数器。
   - 多个计数器并行递增。
   - 节点数量受您在 **Amount** 字段中输入的值限制。

   例如：

   | 计数器类型 | 示例 | 生成的节点 | 说明 |
   | --- | --- | --- | --- |
   | 数字计数器 | `NewNode[3]` | NewNode3、NewNode4… | 从计数器提供的起始数字开始创建节点。最高节点由系统决定。 |
   | 多位数数字计数器 | `NewNode[03]`、`NewNode[789]` | NewNode03、NewNode04…NewNode99；NewNode789、NewNode790 | 从计数器提供的起始数字开始创建节点。若计数器含前导零，节点也含前导零。 |
   | 字母计数器 | `NewNode-[e]` | NewNode-e、NewNode-f…NewNode-z | 从计数器提供的起始字母开始创建节点。计数器限制为单个字符。 |
   | 字母数字计数器 | `NewNode[y][07]` | NewNodey07、NewNodez08…NewNodez99 | 若计数器达到最大值，则该值用于其后所有名称。 |
   | 字母数字计数器 | `[a]New[A]Node[07]` | aNewANewNode07、bNewBNode08…zNewZNode99 | 计数器可放置在任意位置。 |
   | 带增量的计数器 | `NewNode[03,03]`、`NewNode[03,02]-[t,02]` | NewNode03、NewNode06、NewNode09…NewNode99；NewNode03-t、NewNode05-v、NewNode07-x、NewNode09-z… | 可指定计数器增量。 |

7. 要重置节点名称，请单击 **Reset names**。**New** 对话框中当前显示的节点名称将重置为默认名称。
8. 要更改新节点除名称之外的属性，请参阅该节点所放置到的树的相关说明。

   > **注意**
   > - 若 `NewCommandConfiguration.xml` 中缺少 `ClassOrder`，则 **New** 对话框中的所有项按字母顺序显示；若存在，则项的顺序取决于 `GeneralOrder` 和 `Class` 定义。
   > - 若系统找到 `name` 属性等于所选节点类名的 `Class` 子元素，则允许的项按 `PermittedChild` 子元素的顺序显示；未由 `ClassOrder` 和 `Class` 定义的可用元素会追加在后面。
   > - 若未定义该类的 `ClassOrder` 和 `Class`，则项按 `ClassOrder` 和 `GeneralOrder` 显示；未由二者定义的可用元素会追加在后面。
   > - 若 `ClassOrder` 和 `Class`（或 `ClassOrder` 和 `GeneralOrder`）包含不允许用于所选节点的元素定义，则这些项会被省略。

<a id="v3-s40"></a> <!-- p431 -->
## 删除（Delete）

您可以从树视图中删除所选节点及其子树。如果当前项目使用了库中实体的实例，则无法删除这些库中的实体。从树视图中删除节点仅删除该节点的实例，不会从数据库中移除相应的原型（prototype）对象。要从数据库中移除原型，请从库树中删除该节点。

若删除共享节点，该节点将不再由该节点共享；其在其他节点中的定义保持不变。请参阅"共享节点（Sharing Nodes）"。

诸如备注（Notes）之类的对象可通过此选项从当前视图中删除。

<a id="v3-s41"></a> <!-- p431 -->
### 删除节点（Deleting Nodes）

要删除节点：

**操作步骤**

1. 签出要删除的父节点及其整个子树和对象，包括所有子节点。
2. 选择要删除的节点。
3. 选择 **Home** 选项卡 → **Edit** 组 → **Delete**，或按 `<Delete>` 键，或右键单击并从上下文菜单中选择 **Delete**。随即打开确认窗口。
4. 单击 **Yes** 逐个确认删除每个节点；**No** 选项不起作用。删除节点会将其从所显示的树、数据库和查看器（Viewer）中移除。若节点为共享节点，则仅从您签出并从中删除它的父节点中移除，而不会从数据库中移除；若节点非共享，则同时从父节点和数据库中移除。若节点关联到库零件，而该零件在项目中有实例存在，则会导致错误。若另一个子树也包含所选节点，则该节点不会从那个子树中删除。

> **注意**
> 尝试删除对象时，若其中部分对象仍被其他用户签出，状态窗口会列出这些对象及相应用户。这样便于通知各用户（例如通过电子邮件）请求其签入对象。下次您尝试删除时，系统会自动签出所有随后被签入的对象，并在没有其他对象仍处于签出状态时执行删除。请参阅"状态报告（Status Reports）"。

> **注意**
> 如果您不希望系统为执行删除而自动签出对象，应在 systemroot 下的 General 文件夹中添加配置文件。将该文件命名为 `DeleteCommandConfiguration.xml`，并填入以下内容：
> ```xml
> <?xml version="1.0" ?>
> <DeleteOptions>
> <Option Name="DisableAutomaticCheckout" Value="1"/>
> </DeleteOptions>
> ```

<a id="v3-s42"></a> <!-- p432 -->
### 删除研究（Deleting Studies）

您可以在导航树（Navigation Tree）中选择研究（studies）并运行 **Delete**，将其从 eMServer 项目中移除。

与研究相关联、存储在本地系统根（system root）中的工程数据也可被移除，具体取决于 **Options** 对话框 **General** 选项卡中的"删除研究时删除关联单元文件（When deleting a study, delete associated cell files）"设置。勾选该设置时，Process Designer 会从 eMServer 项目中的项目删除所选研究，并从系统根中移除该研究的本地工程数据。这有助于清除系统根中的冗余内容。清除该设置时，Process Designer 会保留该研究的本地工程数据。默认情况下，此设置处于清除状态。

> **注意**
> - 若您希望同时删除 eMServer 项目中的研究以及系统根中该研究的本地工程数据，必须在调用删除命令前选择研究本身。若选择父节点，Process Designer 会从 eMServer 项目中删除该研究，但保留系统根中的本地工程数据。
> - 若您曾执行项目的导出/导入以创建克隆，或复制了整个 schema，则原始项目与复制项目现在可能都访问同一个系统根。当您希望从系统根中删除该研究的本地工程数据时，删除相关文件夹（被已删除研究访问的文件夹）也会一并移除仍在使用该文件夹的剩余研究的所有工程信息。

<a id="v3-s43"></a> <!-- p433 -->
## 重命名对象工具（Rename Objects Tool）

<a id="v3-s44"></a> <!-- p433 -->
### 重命名对象（Renaming Objects）

**重命名对象工具（Rename Objects Tool）** 是 Process Designer 中用于重命名对象的强大工具。它可进行批量名称更改，从而节省大量工作并提升效率。例如，若您复制了一个操作，可以一次性将所有重复的位置重命名为有意义的名称。

要重命名对象：

> **注意**
> 若将"应用到（Apply to）"设置为"子对象（Children）"，则无法在 **Scope** 列表中添加或移除对象。

**操作步骤**

1. 选择 **Home** 选项卡 → **Edit** 组 → **Rename Objects Tool**。随即显示 **Rename Objects Tool** 对话框。

   > **注意**
   > - 若您已在任意查看器中预选了对象，它们会显示在 **Scope** 列表中。
   > - 在下方窗格中配置规则后，**Preview** 列会显示新的对象名称；当前显示的是原始名称。

2. 将 **Apply to** 控件设置为以下之一：
   - **Selected Objects（所选对象）**——更改应用于 **Scope** 列表中选中（勾选）的对象。
   - **Children（子对象）**——更改应用于 **Scope** 列表中选中（勾选）对象下嵌套的所有对象。所选对象本身的名称保持不变。
3. 在任意查看器中选择要重命名的目标对象，并单击 **Add** 将其添加到 **Scope** 列表。
4. 使用 **Scope** 箭头按钮对 **Scope** 列表中的对象重新排序。例如，若您使用枚举器，该顺序会影响新名称。
5. 若 **Scope** 列表中已加载许多对象，可单击 **Filter** 并选择要显示/隐藏的对象类型，以聚焦于您关注的对象类型。
6. 在 **Scope** 列表中勾选要更新的对象，并清除要保持不变的对象。
7. 如有必要，单击 **Remove** 从 **Scope** 列表中移除所选对象，或单击 **Remove** 按钮旁的箭头并选择 **Remove All** 以移除所有对象。
8. 要配置重命名规则，请在 **Rules** 区域单击 **Add Rule** 并选择以下之一：
   - **Append or Trim（追加或裁剪）**
     a. 选择 **Prefix（前缀）** 或 **Suffix（后缀）**，以确定是在对象名称开头还是末尾进行更改。
     b. 在 **Append or Trim** 对话框中，可配置 **Append** 或 **Trim** 选项。默认选中 **Append** 选项——输入所需文本。

        本例中，Process Designer 在每个所选对象名称前添加字符串 `123ST_1`。

        单击 **OK** 后，新规则会显示在 **Rename Objects Tool** 对话框下半部分的 **Rules** 列表中，同时 **Scope** 列表中会显示新对象名称的预览。

     c. 若要从对象名称中裁剪字符，请在 **Append or Trim** 对话框的 **Trim** 区域执行以下任一裁剪操作：
        - a. 选择 **Characters（字符）**，输入要移除的字符数，并单击 **OK**。

          本例中，Process Designer 从每个所选对象名称中移除最后一个字符。

          单击 **OK** 后，新规则会显示在 **Rules** 列表中，同时 **Scope** 列表中会显示新对象名称的预览。

        - b. 选择 **Consecutive digits（连续数字）** 并单击 **OK**。

          本例中，Process Designer 移除对象名称开头的所有连续数字。

          单击 **OK** 后，新规则会显示在 **Rules** 列表中，同时 **Scope** 列表中会显示新对象名称的预览。
   - **Replace（替换）**
     a. 在 **Replace what（替换目标）** 区域，选择以下之一：
        - **Entire name（整个名称）**——完全移除原始文件名，并将其替换为新名称（在 **Replace with** 区域输入）。
        - **Text（文本）** 并输入要搜索的文本字符串。您可以从以下选项中选择：
          - **Match entire name（匹配整个名称）**——使文本匹配整个名称。
          - **Use wildcards（使用通配符）**——使用 `*` 和 `?` 时，您必须输入 `\*` 和 `\?`。该字符串会被新文本（在 **Replace with** 区域输入）替换。搜索文本字符串时，您还可根据需要设置 **Match case（区分大小写）**。
     b. 在 **Replace with** 区域，输入文本字符串。这是新名称，用于替换原始对象名称或其一部分（按 **Search for** 区域的配置）。
        本例中，Process Designer 将文本字符串 `ST_1` 替换为 `via`。

        单击 **OK** 后，新规则会显示在 **Rules** 列表中，同时 **Scope** 列表中会显示新对象名称的预览。
   - 在 **Append or Trim** 和 **Replace** 两种模式下，均可使用枚举器（为每个重命名对象递增追加的一个或多个数字）。例如，要追加序列号，请按以下步骤操作：
     a. （可选）输入要追加的文本。
     b. 设置 **Enumerator（枚举器）** 及其 **Step size（步长）**。
     c. 单击 **Add**。枚举器显示在追加文本之后。
9. 单击 **OK** 完成规则设置。新规则会显示在 **Rules** 列表中，同时 **Scope** 列表中会显示带枚举的新对象名称预览。

   > **注意**
   > - 您也可以通过在方括号中添加数字，以自由文本形式输入枚举器。
   > - 您可以添加多个枚举器，例如 `[010,2,0][1,5,1]`。
   > - 您可通过将最大字符串长度设置为枚举器中的位数，来限制最高枚举器值。为此，请向枚举器添加第三个数字：1 表示无限制，0 表示有限。例如 `[10,2,0]` 将枚举器限制为最大值 99。当枚举器计数器达到所定义的限值，会重置为 0 并重复枚举。对于长度受限的枚举器，您还可以在枚举器定义中添加前导零以固定长度。

10. 要在任何重命名规则中使用对象的 eMServer 属性，请单击 **Add Attribute**。随即显示 **Field Selector**。
11. 单击从列表中选择所需属性。使用 **Field Selector** 有助于避免在输入属性名称时出错。

   > **注意**
   > 若所选属性仅存在于部分待重命名对象中，系统仅考虑这些对象的属性值。若至少有一个属性不存在于任何待重命名对象中，则整个规则标记为红色，且该属性值视为空。下图中，`STATUS` 属性存在于 `Part`，但重命名对象中没有零件；`MissingAttribute` 不存在于任何待重命名对象中。

12. 配置其他规则，并使用箭头按钮根据需要重新排序。

   > **注意**
   > 请注意按有意义的顺序排列规则，因为规则顺序会影响新名称。例如，若第一条规则添加后缀，而第二条规则裁剪后缀字符，结果是裁剪掉新的后缀。

13. 如有必要，单击 **Remove** 从 **Rules** 列表中移除所选规则，或单击 **Remove** 按钮旁的箭头并选择 **Remove All** 以移除所有规则。
14. 如有必要，在 **Rules** 列表中选择一条规则并单击 **Edit** 进行更改。
15. 配置完所有规则并对预览结果满意后，单击 **Apply**。
16. 单击 **Close** 退出 **Rename Objects Tool** 对话框。

<a id="v3-s45"></a> <!-- p450 -->
## 规则集（Rule Sets）

在 **Rename Objects Tool** 对话框中配置好有用的规则后，您可以将它们保存为规则集（rule set），以便快速访问并导出给其他用户。

要保存规则集：

**操作步骤**

1. 在 **Rename Objects Tool** 对话框中，单击保存图标（Save 图标）。随即显示 **Save a set of rules** 对话框。
2. 输入有意义的 **Name**，添加 **Description**（如需要），并单击 **OK**。新规则集会显示在下方工具栏的下拉列表中。

> **注意**
> 保存后续规则集时，请单击保存图标旁的箭头并选择 **Save As**。

要加载规则集：

- 在下方工具栏的下拉列表中选择该规则集。所选规则集会加载到 **Rename Objects Tool** 对话框中。

要在规则集中添加或移除规则：

**操作步骤**

1. 加载该规则集。
2. 若要添加新规则，请按以下步骤操作：
   a. 关闭 **Rules Manager**。
   b. 在 **Rename Objects Tool** 对话框中添加新规则。
   c. 单击 **Save** 以更新当前规则集。
3. 若要移除规则，请选择该规则并单击 **Remove**。
4. 单击 **Save** 更新当前规则集，或单击 **Save** 按钮旁的箭头并选择 **Save As**，将规则另存为新规则集。

要编辑规则集：

**操作步骤**

1. 单击 **Edit Rule Set**。随即显示 **Rules Manager**。
2. 执行以下任一编辑操作：
   - 在 **Sets** 列表中选择规则集并单击 **Delete**，将其从规则集中删除。

     > **注意**
     > 在右侧 **Rules** 列表中选择某条规则并单击 **Delete**，也会删除其父规则集。

   - 双击规则 **Name** 或 **Description** 并修改文本。

要导出或导入规则集：

**操作步骤**

1. 单击 **Edit Rule Set**。随即显示 **Rules Manager**。
2. 按以下方式操作：
   - 导出规则集：
     a. 在 **Sets** 列表中选择规则集，或在 **Rules** 列表中选择规则，并单击 **Export**。随即显示 **Save As** 对话框。
     b. 输入 **File name** 并选择保存导出数据的位置（XML 文件）。
   - 导入规则集：
     a. 单击 **Import**。随即显示 **Open** 对话框。
     b. 导航到已导出的 XML 规则集文件并单击 **Open**。规则集随即加载到 **Rename Objects Tool** 对话框中。

     > **注意**
     > 若您导入了之前已导入过的规则集，系统会更新当前的规则集（而不会创建新规则集）。
<a id="v3-s46"></a> <!-- p455 -->
## 查看器（Viewers）

<a id="v3-s47"></a> <!-- p455 -->
### 碰撞检测（Collision detection）

#### 碰撞查看器（Collision Viewer）

**碰撞查看器（Collision Viewer）** 是规划和优化装配过程的重要工具。您可以使用碰撞查看器检查装配过程中所规划操作的可行性，并确保过程无碰撞。例如，在装配汽车车身时，您可以使用碰撞查看器回答以下问题：

- 在装配过程中的哪个最佳位置安装座椅？
- 在拟定的装配位置是否有足够空间安装座椅？

您可以使用碰撞查看器显示特别关注的已规划碰撞集（collision set），并隐藏其他碰撞集。例如，在 PC 机箱内安装电源时，您可以指定检查电源与 PC 机箱之间的碰撞，同时忽略硬盘与 PC 机箱之间的碰撞。

运行拟定过程的仿真时，碰撞查看器可显示碰撞对象的碰撞曲线（collision curve）。您可以将碰撞作为报告查看，或在查看器（Viewer）中图形化查看。这使您能够进行交互式修正，并优化过程以获得最佳结果。

要访问碰撞查看器，请选择 **Home** 选项卡 → **Viewers** 组 → **Viewers**，然后选择 **Collision Viewer**。

碰撞查看器使您能够定义、检测并查看当前显示在对象树（Object Tree）中的数据中的碰撞，以及查看碰撞报告。

**碰撞查看器布局**

碰撞查看器由三个窗格组成：

- 左窗格包含用于创建和管理碰撞集的编辑器。
- 中窗格显示碰撞结果并包含查看选项。主对象节点显示为红色，碰撞对象显示为蓝色。
- 右窗格显示所选碰撞的碰撞曲线列表。每条曲线以其碰撞对象命名。

左窗格包含以下选项：

| 按钮 | 工具 | 说明 |
| --- | --- | --- |
| New Collision Set | 新建碰撞集 | 定义新的碰撞集。参见"New Collision Set"。 |
| Remove Collision Set | 删除碰撞集 | 删除先前创建的碰撞集。 |
| Edit Collision Set | 编辑碰撞集 | 更改先前创建的碰撞集的定义。 |
| Fast Collision | 快速碰撞 | 根据所选对象快速创建碰撞集。该碰撞集以名称 `fast_collision_set` 显示在碰撞查看器左窗格中。使用此选项创建的碰撞集是一个自集（self set），即集内所有对象彼此之间进行碰撞检查。一个研究中只能存在一个快速碰撞集；若再创建，将替换先前的快速碰撞集。若所选对象仅由点云/点云图层组成，则 Fast Collision 被禁用；若所选对象同时包含点云/点云图层和其他对象，所有点云/点云图层会列在 Fast Collision 窗口的左窗格中。 |
| Emphasize Collision Set | 高亮碰撞集 | 在查看器（Viewer）中以黄色、蓝色和橙色高亮所选碰撞集。碰撞集编辑器（Collision Set Editor）左侧（Check:）列中的对象显示为黄色，右侧（With:）列中的对象显示为蓝色，两列中都列出的对象显示为橙色。非碰撞实体也以不同颜色高亮——左侧列青绿色、右侧列深红色、两列深绿色。再次单击图标恢复正常查看。 |
| All Displayed Objects | 所有显示对象 | 激活时，检查查看器（Viewer）中显示的所有对象之间的碰撞；此选项忽略已定义的碰撞集。启用该选项会显著影响系统性能。注意：此选项不检查点云和点云图层。 |

启用 **Emphasize Collision Set** 时，查看器（Viewer）以黄色和蓝色显示所选碰撞集中的对象（若存在碰撞，碰撞对象以红色高亮——无论是否激活 Emphasize Collision Set）：

中窗格包含以下选项：

| 按钮 | 工具 | 说明 |
| --- | --- | --- |
| Show/Hide Collision Sets | 显示/隐藏碰撞集 | 显示/隐藏碰撞查看器的碰撞集编辑窗格。 |
| Collision Mode On/Off | 碰撞模式开/关 | 激活/停用碰撞模式。 |
| Freeze Viewer | 冻结查看器 | 冻结碰撞查看器，防止在查看器（Viewer）中移动对象时动态更新碰撞报告。 |
| Collision Options | 碰撞选项 | 设置默认碰撞选项。参见 Options 对话框的 Collision 选项卡。 |
| Show Colliding Curve | 显示碰撞曲线 | 切换图形显示中碰撞对象的碰撞曲线。曲线以黄色显示，选中时以绿色显示。您也可在 Collision Curves 窗格中右键单击曲线并选择 Zoom to selection 缩放显示碰撞曲线。碰撞曲线不一定是连续线，当碰撞对象某些位置接触、某些位置未接触时，它由若干线段组成。若碰撞集包含多个碰撞对象，会生成多条碰撞轮廓；碰撞轮廓不为点云和点云图层生成。 |
| Show Colliding Pair | 显示碰撞对 | 定义碰撞对象对的碰撞状态显示方式。未选中按钮时忽略下拉选择；否则应用以下之一：Color Selected Pair（所选对在查看器（Viewer）中以主对象节点红色、碰撞对象透明蓝色显示，其他对象为白色）；Show Selected Pair Only（仅显示所选对，其他项不显示）。 |
| Export to Excel | 导出到 Excel | 将碰撞查看器中的信息保存为 .CSV 文件。 |
| Show/Hide Collision Curves | 显示/隐藏碰撞曲线 | 显示/隐藏碰撞查看器的 Collision Curves 窗格。 |
| Collision Depth | 碰撞深度 | 计算碰撞对象的穿透深度。参见 Calculate Collision Penetration。 |
| Color Colliding Objects | 碰撞对象着色 | 切换碰撞对象的颜色高亮，便于清晰查看。若 Show Colliding Pair 激活，此功能在红/透明蓝与对象原始颜色之间切换高亮。 |
| Collision Results Filter | 碰撞结果筛选 | 筛选碰撞结果。可选：List Colliding Pairs Only（仅列出碰撞对，红色高亮）；List All Pairs（显示单元中所有可见对象之间的距离）。 |

碰撞查看器在 **Parts** 列显示当前涉及碰撞的所有零件，在 **With Parts** 列显示与之碰撞的零件。单击零件旁的 + 可查看与其碰撞的所有零件列表，这些零件显示为所查看零件的子项。选择父零件时，与子零件的所有碰撞都会高亮。

**Collision Curves** 窗格（右侧）使您能够选择查看器（Viewer）中高亮的曲线；您也可选择一条曲线并单击图标将其在查看器（Viewer）中缩放至该曲线。在查看器（Viewer）中单击曲线会自动在 Collision Curves 面板中选择它。

> **注意**
> 运行仿真时，碰撞曲线不显示，且 Show Colliding Curve 图标变为非活动状态。但当仿真完成（或暂停）后，碰撞曲线会再次显示。

碰撞查看器可在链接和实体级别显示碰撞详情（当您在 Options 对话框的 Collision 选项卡中使用 Lowest available level 选项时）。单击碰撞查看器工具栏上的 Show/Hide Collision Details 图标可打开 Collision Details 窗格。

**创建碰撞集（Create Collision Set）**

**New Collision Set** 命令使您能够在对象树（Object Tree）或查看器（Viewer）中选择对象，并保存这些对象对以检查碰撞或接近碰撞（near miss）。可创建两类碰撞集：

- 碰撞列表（collision list）——使所选对象的一个列表针对另一组所选对象进行碰撞检查。
- 自集（self set）——检查集中每个对象与集中所有其他对象的碰撞。

**操作步骤**

1. 单击图标打开 **Collision Set Editor** 窗口。
2. 在对象查看器或查看器（Viewer）中选择对象。这些对象的名称出现在 **Check** 窗格中。
3. 执行以下操作之一：
   - 创建自集（每个对象与所有其他对象碰撞）时，将所有对象保留在 **Check** 窗格中。
   - 创建碰撞列表（一个所选对象列表针对另一对象列表检查）时，单击图标在 **Check** 与 **With** 窗格之间移动一个或多个对象以设置碰撞检查对象。随后在 **Check** 窗格中选择一个对象，在 **With** 窗格中选择一个对象，并单击 **OK**。该对象对作为碰撞集添加到碰撞查看器中。
4. 单击图标激活碰撞模式，并使用所选对象对检查碰撞。

创建多个碰撞集后，它们会显示在碰撞查看器的 Editing 窗格中。

> **注意**
> Near Miss 和 Contact - Allowed Penetration 的值取自 Options 对话框 Collision 选项卡的默认设置。如需更改，可单击这两个字段之一进行编辑；您输入的值会覆盖默认值。建议仅设置其中一个参数。

5. 勾选要检查碰撞的碰撞集旁边的复选框。

**删除碰撞集（Remove Collision Set）**

**Remove Collision Set** 命令用于删除先前创建的碰撞集。

**操作步骤**

1. 在碰撞查看器的 Editing 窗格中选择一个碰撞集。
2. 单击图标删除所选碰撞集。

**计算碰撞穿透深度（Calculating Collision Penetration）**

碰撞查看器可计算碰撞对象的穿透深度，并利用该信息显示用于撤出其中一个碰撞对象以消除碰撞状态的向量。

> **注意**
> 系统无法计算碰撞状态为接近碰撞（near miss）或接触（contact）的对象的穿透深度。

**操作步骤**

1. 在碰撞查看器的 Part 列表中，选择一个碰撞零件并单击图标。随即显示 **Collision Depth** 对话框。在 Collision pair 区域，Object 显示所选零件的名称，With objects 列出与该零件碰撞的所有零件。（您也可在启动 Collision Depth 前从碰撞查看器的 With Parts 列表中选择零件，此时 With objects 中只有单个条目。）在 Penetration vector 区域，Vector 显示穿透向量的 x、y、z 方向分量，Penetration depth 显示碰撞对象的穿透深度。当 **Collision Depth** 对话框处于活动状态时，查看器（Viewer）以红色显示碰撞对象、以黄色显示碰撞穿透向量（显示为箭头，指向移动所选碰撞零件的方向及消除碰撞状态所需移动的距离）。

   > **注意**
   > 您可以配置穿透向量的颜色。参见 Options 对话框的 Appearance 选项卡。

   碰撞深度不检查点云和点云图层。
2. 默认情况下，**Distance to jump** 显示碰撞对象的穿透深度，即消除碰撞状态所需移动所选碰撞零件的距离。消除碰撞状态时，如需在碰撞对象间创建额外间隙，可更改此距离。

   > **注意**
   > - 若存在多个解决碰撞状态的方案，系统选择最短向量。
   > - 若碰撞零件与多个其他对象碰撞，系统计算可消除该零件与其所有碰撞对象之间碰撞状态的最短向量。

3. 单击 **Jump**。系统按 Distance to jump 中的距离、沿穿透向量方向移动所选碰撞零件。碰撞状态被消除，查看器（Viewer）和碰撞查看器均显示无碰撞的新状态。
4. 若对方案不满意，单击 **Reset** 恢复至碰撞状态。
5. 若您更改了碰撞状态，单击 **Refresh** 重新计算穿透向量。

   > **注意**
   > 若您所做的更改已消除碰撞状态，系统显示以下消息：The penetration is no longer active。

6. 单击 **Close** 退出 **Collision Depth** 对话框。

<a id="v3-s48"></a> <!-- p466 -->
### 比较查看器（Comparison Viewer）

**比较查看器（Comparison Viewer）** 高亮备选方案（alternatives）之间的差异，并直观地说明它们之间的结构差异所在。根据您对备选方案的选择，比较查看器自动以下列两种模式之一运行：

- **将备选方案与原始数据比较（Compare alternative with original data）**——将所选备选方案的节点与其派生源始数据（scope）的节点进行比较，结果显示在比较查看器中。这也称为"与范围比较（compare with scope）"。
- **比较两个备选方案（Compare two alternatives）**——将两个源自相同原始数据的备选方案相互比较，每个备选方案的节点与另一个备选方案的节点比较，结果显示在比较查看器中。

> **注意**
> 比较查看器也会比较两个不相关的备选方案，但这种情况下所有节点都会显示为孤儿（orphan），因此不会提供任何有用信息。

比较查看器会省略比较以下字段/属性（因为这些字段始终产生无用的结果）：`externalID`、`modificationDate`、`apOriginalNode`、`apOriginalLinkedSupplyChain`、`apClonedNodes`、`apClonedLinkedSupplyChains`、`apOwnExternalId`、`apLinkedSupplyChainOwnExternalId`、`AlternativeUpdate`、`propagateSyncDate`、`ids`、`pvIds`、`simulationInfo`。以下属性也不作为普通字段比较，但会比较其结构差异：`logicalChildren`、`children`。

要比较备选方案：

**操作步骤**

1. 执行以下操作之一：
   - 在导航树（Navigation Tree）中选择一个备选方案并单击 **Compare with Original**。比较查看器打开，左窗格为所选备选方案，右窗格为其范围（scope，即其派生的原始数据），并比较该备选方案与其范围。
   - 在导航树（Navigation Tree）中选择两个备选方案并单击 **Compare**。比较查看器打开，左窗格为第一个备选方案（您首先选择的），右窗格为第二个，并比较这两个备选方案。
   - 在导航树（Navigation Tree）中选择一个备选方案并单击 **Left Side of Comparison**；在导航树（Navigation Tree）中选择另一个备选方案并单击 **Compare**。比较查看器打开，左窗格为第一个备选方案，右窗格为第二个，并比较这两个备选方案。
   - 选择 **Home** 选项卡 → **Viewers** 组 → **Viewers**，并选择 **Comparison Viewer**。比较查看器出现——为空。
     - 从导航树（Navigation Tree）拖拽一个备选方案并将其放置到比较查看器左窗格的标题上。比较查看器将该备选方案与其范围（scope）比较。
     - 在左窗格放置一个备选方案后，您可拖拽另一个备选方案并将其放置到比较查看器右窗格的标题上。放置第二个备选方案后，比较查看器刷新显示并对两个备选方案进行比较。

在所有情况下，比较查看器使用不同颜色高亮所比较备选方案之间的差异。

在比较查看器任一窗格中选择一个节点，会使所选节点的文本以白色字体显示，并在另一窗格中选择等效节点。最后选择的节点（以白色字体显示）成为活动节点。在 Process Designer 中启动其他命令时，命令对活动节点操作。滚动任一窗格也会导致另一窗格滚动。在右侧或左侧树中选择对象后，您可以使用上下箭头按钮循环浏览结构中的更改；树会在必要时展开，便于在大型结构中查找更改。

2. 您可以使用比较查看器工具栏中的以下工具：
   - **Synchronize by Selected Object（按所选对象同步）**——选择一个节点（例如……）后，单击此图标可使两个窗格同步，使它们在同一行显示同一节点。这有助于理解节点在备选方案内被移动到不同位置时的结构差异。
   - **Synchronize by Absolute Position（按绝对位置同步）**——单击此图标可将比较查看器窗格恢复到单击 Synchronize by Selected Object 之前的状态。
   - **Compare Colors（比较颜色）**——打开 Customize colors 对话框。

     > **注意**
     > 使用此工具需要 Define Compare Colors 权限。

     Customize colors 对话框显示所选备选方案窗格间存在差异的节点默认背景色。单击颜色框可更改默认背景色——打开 Modify Color 对话框，按需自定义颜色。各状态含义：
     - **Changed（已更改）**——节点在两个备选方案中都存在但已更改，并在两个窗格中高亮。例如结构已更改的节点或已重命名的节点。
     - **Deleted（已删除）**——节点在备选方案或其范围（scope）中已被删除，在它出现的窗格中高亮，另一窗格留空。此选项仅在与范围（scope）比较时相关。
     - **Newly Created（新创建）**——节点已添加到备选方案或其范围（scope），在它出现的窗格中高亮，另一窗格留空。此选项仅在与范围（scope）比较时相关。
     - **Moved（已移动）**——节点已移动，在两个窗格中高亮。
     - **Orphan（孤儿）**——节点为孤儿，在另一备选方案中没有等效节点，留空。此选项仅比较两个备选方案时相关。
   - **Compare configuration（比较配置）**——使您能够选择用于当前比较的预定义比较配置。有关如何创建比较配置，参见 Creating Compare Configurations。
3. （可选）将备选方案从比较查看器拖放到导航树（Navigation Tree）的放大镜上，或从导航树（Navigation Tree）拖放到比较查看器的放大镜上。两种情况下，所选节点成为查看器的焦点。
4. 查看所比较备选方案之间的结构差异后，您可以选择任意节点，并在底部窗格中查看该节点的差异。底部窗格显示备选方案的属性和关系。您可以将关系图标拖放到导航树（Navigation Tree）的放大镜上以聚焦它们。
5. 若您对当前显示在比较查看器中的备选方案进行了更改，请再次将其拖放至比较查看器以刷新显示。
6. 单击 **Export** 按钮，将当前比较结果导出为报告（原始 xml 或 Excel xml），便于记录并分发给其他方。

<a id="v3-s49"></a> <!-- p476 -->
### eMS 库浏览器（eMS Library Browser）

#### 使用 eMS 库浏览器（Using the eMS Library Browser）

**库浏览器（Library Browser）** 为规划人员、工程师和管理人员提供统一的通用界面，用于浏览和搜索库。他们可以：

- 将整个组织中的对象集中到单一、统一、集中的集合中。
- 通过使一组通用库对象在多个项目中全局可用，规范原型（prototype）的使用。
- 仅显示库，使浏览库元素变得容易得多。

全局库（global library）由拥有数据完全访问权限的库管理员（library manager）定义和维护。库管理员创建全局库，并负责将本地库与全局库同步。

除全局库数据外，所有用户均可维护和管理项目本地的库。

库浏览器有两种模式：

- **更新模式（update mode）**——库管理员可针对已修改的数据轻松地一次性更新多个项目。
- **浏览模式（browse mode）**——所有用户可浏览和搜索其有权访问的库。

#### 库浏览器主窗口（The Main Library Browser Window）

要启动库浏览器，请确保在 Process Designer 中已打开一个项目。选择 **Applications** 选项卡 → **Layout** 组 → **eMS Library Browser**。

随即显示库浏览器窗口。浏览器显示库的层级结构。

- 库的排序和分类方式在全局库项目中定义和维护，并由 Library Managers 组的用户更新到所有项目。常规项目中的本地库副本可供所有用户使用。
- 库和集合可通过单击展开或折叠。仅显示零件库、资源库和操作列表的内容。
- 单击左窗格中的库时，其包含的对象显示在中央窗格中。
- 单击对象时，右窗格中会显示该对象及其部分属性的预览。

该窗口提供以下按钮：

| 单击 | 用途 |
| --- | --- |
| （新建库） | 创建新库 |
| （编辑库） | 编辑所选库 |
| （删除库） | 删除所选库 |
| （显示/隐藏切换） | 切换开/关以显示/隐藏 Operations、Parts 和 Resources |
| （查看属性） | 查看所选库或原型的属性；查看所选项的产品属性 |
| （新建原型） | 创建新原型 |
| （编辑原型） | 编辑所选原型 |
| （删除原型） | 删除所选原型 |
| （搜索） | 打开搜索功能 |
| （插入对象） | 插入对象 |
| （插入特殊） | 插入特殊（多实例/带名称前缀的副本） |

#### 浏览和搜索库（Browsing and Searching Libraries）

在浏览模式下，库浏览器允许用户按库层级中的定义浏览可用库、查看单个库元素，以及搜索匹配所需条件的元素。

- 库和集合可通过单击展开或折叠。仅显示零件库、资源库和操作列表的内容。
- 单击左窗格中的库时，其包含的对象显示在中央窗格中。
- 单击对象时，右窗格中会显示该对象及其部分属性的预览。
- 单击搜索图标时，搜索功能显示在窗口右侧。

#### 创建、编辑和删除库（Creating, Editing and Deleting Libraries）

**创建库（Creating Libraries）**

库管理员可创建本地和全局库。普通用户只能创建本地库。

**操作步骤**

1. 单击图标。随即显示以下对话框。
2. 从 **Type** 列表中选择库类型。
3. 在 **Library Name** 字段中输入名称。
4. 单击 **OK**。

**编辑库属性（Editing Library Properties）**

库管理员可编辑本地和全局库的名称并添加注释。普通用户只能对本地库进行这些修改。

**操作步骤**

1. 单击图标。随即显示以下对话框。
2. 在 **Library Name** 字段中修改名称。
3. 单击 **OK**。

**删除库（Deleting Libraries）**

库管理员可删除本地和全局库。普通用户只能删除本地库。

**操作步骤**

1. 单击要删除的库或库元素。
2. 单击图标。随即显示以下对话框。
3. 单击 **Yes**。

#### 创建、编辑和删除原型（Creating, Editing and Deleting Prototypes）

**创建原型（Creating Prototypes）**

库管理员可在本地和全局库中创建原型。普通用户只能在本地库中创建原型。

**操作步骤**

1. 单击图标。随即显示以下对话框。
2. 在 **Name** 字段中输入名称。
3. 从 **Type** 列表中选择库类型。
4. 在 **Geometric Information** 区域，选择相应的 3D 文件（.cojt）和用于预览的图像文件。
5. 如需附加文件或文件夹，单击 **Attach File** 或 **Attach Folder**。
6. 单击 **OK**。

   > **注意**
   > Attribute 列表中出现的名称-值对定义在 `<system root>\General\LibraryBrowser` 处的 `LBSearchFields.xml` 文件中。该列表可自定义。

**编辑原型（Editing Prototypes）**

库管理员可编辑本地和全局库中的原型。普通用户只能编辑本地库中的原型。

**操作步骤**

1. 单击图标。随即显示以下对话框。
2. 如需，在 **Name** 字段中修改名称。
3. 在 **Geometric Information** 区域，选择相应的 3D 文件（.cojt）和用于预览的图像文件。
4. 如需附加文件或文件夹，单击 **Attach File** 或 **Attach Folder**。
5. 单击 **OK**。

**删除原型（Deleting Prototypes）**

库管理员可删除本地和全局库中的原型。普通用户只能删除本地库中的原型。

**操作步骤**

1. 单击要删除的库或库元素。
2. 单击图标。随即显示以下对话框。
3. 单击 **Yes**。
<a id="v3-s50"></a> <!-- p484 -->
#### 插入库对象（Inserting Library Objects）

拖放对象即可插入零件或资源原型（prototype）的实例，或操作的副本，到任意所需位置；或者，单击工具栏上的 **Insert** 或 **Insert Special** 图标，在目标处创建一个或多个对象。

使用 **Insert Special** 命令可显示 Insert Special 对话框，让您选择新对象的数量，并设置应用于每个对象的 **Name prefix（名称前缀）**。您可以在字段中键入名称前缀，或打开下拉框并选择 **Source name** 或 **Class name** 作为自动分配的名称前缀。

<a id="v3-s51"></a> <!-- p485 -->
#### 执行搜索（Performing Searches）

搜索功能在库层级上执行搜索。您可以根据对象的属性搜索任何类型的对象。

搜索会检索在 `<system root>\General\LibraryBrowser` 文件夹的 `LBSearchFields.xml` 文件中定义的对象类型及其相应属性。该列表可自定义。

搜索结果显示在 Query Attributes 下方的窗格中。单击结果列表中的对象可查看其属性和预览信息。

**操作步骤**

1. 在工具栏中单击搜索图标。右窗格切换为显示搜索功能。
2. 在 **Object Type** 列表中选择要搜索的一种对象类型。
3. 在左窗格中，选择要在其中执行搜索的节点。
4. 指定查询属性：
   - 在第一个下拉列表中选择一个对象属性（列表中仅显示相关属性）。
   - 打开第二个下拉列表（包含与所选属性相关的参数）并选择一项。
   - 在第三个字段中输入值。
   - 在初始大小的窗口中，您最多可按此方式定义三个属性；单击 **More Attributes** 展开窗口后可再定义三个。
5. 单击 **Go**。结果显示在 Query Attributes 下方的窗格中。
   - 单击对象可查看其属性。
6. 再次单击搜索图标可隐藏搜索功能并返回浏览。

<a id="v3-s52"></a> <!-- p486 -->
### 树与查看器（Trees and the Graphic Viewer）

将 3D 数据加载到查看器（Viewer）后，它与树中显示的数据完全集成。可在查看器（Viewer）或树中执行修改，所有视图会同时更新。选中后，无论通过树上下文菜单还是图形上下文菜单访问，所有命令均可用。

树中的所有层级变更都会反映在图形中（例如选择复合节点）。

有几种拖放（drag and drop）类型：

- 从查看器（Viewer）拖放对象到树，例如：
  - 将零件（Parts）拖到产品树（Product tree），这也会更改层级。
  - 将制造特征（Mfgs）拖到产品/操作树（Product/Operation tree），从而将 Mfg 指定给零件/操作。
- 从树拖放到属性（Properties），以指定对象。
- 从树拖放到树（将零件拖到操作，从而将零件指定给操作等）。此操作还允许您将对象从一个树拖到另一个树中的折叠节点；此时只需在折叠节点上悬停一秒，它会自动展开，然后可在目标树中按需指定或重新定位对象。
- 从导航树（Navigation tree）拖放到其他树（这会加载对象，同时加载到查看器（Viewer）），以及将对象拖回导航树（以卸载它）。
- 从导航树拖放原型（prototypes）和复合资源到查看器（Viewer），以创建资源的新实例。新实例添加在当前活动资源之下。查看器（Viewer）中会显示一个白色方框，作为正确放置新实例的视觉辅助。

鼠标按键为拖放提供变化：

- **移动/指定（Move/Assign）**：拖放结果根据上下文可能是移动或指定。
- **复制（Copy）**：拖动时按住 Control 键进行复制。
- **快捷方式（Shortcut）**：拖动时按住 Ctrl 和 Alt 键创建快捷方式。
- 按 Alt 键创建共享对象。

您可以通过以下任一方法将 Mfgs 直接加载到查看器（Viewer）（即使它们未指定给操作或零件）：

- **Mfg 库**：将 Mfg 库拖到研究（study）下并用查看器（Viewer）打开它。
- **单个 Mfg**：将单个 Mfg 直接拖到研究（study）下并用查看器（Viewer）打开它。
- **单个 Mfg**：使用搜索工具搜索并加载它们。
- 加载带有已指定 Mfgs 的零件时，Mfgs 会自动加载。

<a id="v3-s53"></a> <!-- p487 -->
### IPA 查看器（IPA Viewer）

**IPA 查看器（IPA Viewer）** 包含以下根节点：

- **Assemblies（装配）**——显示所选操作的 IPA（在制品装配，In-Process Assembly）。可以使用 Process Designer 放置工具将 IPA 相对于工位定位。
- **Groups（组）**——组节点。参见 Group。

您可以在 IPA 查看器中编辑组，如下所示：

- 完全支持复制、剪切、粘贴和拖放。
- `<Alt>` + 拖放会创建组的共享实例。一个组实例中的任何更改都会反映到另一个实例。
- 从 IPA 查看器删除组元素会将其与组解除关联，但不会删除它们——它们在对象树（Object Tree）中保持不变。此规则的例外是嵌套在复合节点下的对象，它会被删除。
- **Export Groups to Excel（导出组到 Excel）**——Process Designer 提示您配置报告，如下所示：
  1. 选择所需组并单击 **Export Groups to Excel**。随即显示 Export Groups to Excel 对话框。
  2. 对于每个对象类型，从可用属性中选择，以编译"按以下顺序显示属性（Show properties in following order）"列表，并单击 **OK**。Process Designer 生成 Excel 报告。

  > **注意**
  > 若您未选择任何组，Process Designer 会为 IPA 查看器中的所有组生成报告。

装配数据在加载到 Process Designer/Process Simulate 时，根据所加载的过程信息即时收集。打开后，装配数据与各种现有功能交互，参与查看、编写和验证装配过程的各种工作流。

用户可以选择为继承自 PmProcess 的类的对象创建 IPA，其中 IPA 创建的遍历在 PrStationProcess 级别停止；或者可以自定义应为哪些类创建 IPA 节点，以及 IPA 创建遍历应在哪些类上停止。

用户应定义两个类：

- **用于 IPA 生成的基类（Base class for IPA generation）**——此基类及其任何继承类的对象都适合 IPA 生成。
- **用于停止 IPA 生成遍历的基类（Base class for halting the traverse of IPA generation）**——收集操作用于 IPA 生成的遍历会在此类或其任何继承类的对象上停止。

（可选）您可以为操作之间的流定义条件，以从查看器（Viewer）中过滤掉零件。若某条流满足条件，连接到该流的零件不会显示在 IPA 查看器中。

加载已生成相关 IPA 查看器的过程后，该过程的零件也会加载，您可以在 IPA 查看器或关系查看器（Relations Viewer）中参考在制品装配（In-Process Assemblies）。

> **注意**
> - 加载 IPA 数据随过程需要存在已更新的 IPA 查看器。
> - IPA 查看器不会反映在加载后于研究中完成的分配。
> - 作为快捷方式加载到研究中的 IPA 层级中的零件（或作为快捷方式加载的子树中的零件）会出现在产品/对象树（Product/Object Tree）中。因此，这些零件不会进入 IPA 层级，从而 IPA 层级会显示不正确。指定给已加载操作/焊接点的零件会同时出现在 IPA 查看器和产品/对象树中（IPA 查看器中禁用了层级更改）。
> - 当过程包含范围流（scope flows）时，IPA 查看器中显示的树会遵循 IPA 与范围流中描述的某些规则。

<a id="v3-s54"></a> <!-- p491 -->
### Mfg 树（Mfg Tree）

**Mfg 树** 列出所加载数据的 Mfgs（例如焊接点）。它没有层级结构。无法添加新节点，但您可以从库中拖放 Mfgs。

要显示 Mfg 树，请选择 **Home** 选项卡 → **Viewers** 组 → **Viewers**，并选择 **Mfg Tree**。Mfg 树打开并显示当前项目的数据。

参见 Remove Root 从树中移除 Mfgs。

<a id="v3-s55"></a> <!-- p491 -->
### 对象树（Object Tree）

**对象树（Object tree）** 包含显示与特定项目相关的备注（notes）、标签（labels）、剖面（sections）和框架（frames）的节点。

要显示对象树：

**操作步骤**

1. 在导航树（Navigation tree）中选择所需的对象树节点。
2. 双击该节点，或右键单击并从出现的上下文菜单中选择 **Open** 或 **Open With**。随即为所选节点显示对象树。

<a id="v3-s56"></a> <!-- p492 -->
### 操作树（Operation Tree）

操作计划层级（operations plan hierarchy）表示构建产品所需的全部操作。该层级的最高级别（即根节点）以最通用的术语定义计划，例如"构建产品（Build Product）"。层级随后向下分支为一系列一级制造操作；下一级别包含每个操作中的子操作，依此类推，直到计划完全展开并包含每个操作。

要显示操作树：

- 选择 **Home** 选项卡 → **Viewers** 组 → **Viewers**，并选择 **Operation Tree**。操作树打开并显示当前项目的数据。

<a id="v3-s57"></a> <!-- p493 -->
### 过程模块（Process Modules）

#### 使用过程模块执行装配（Performing Assemblies with Process Modules）

**过程模块（Process Module）** 是表示执行某项功能性装配所需操作元素集合的对象。尽管过程模块包含操作元素，但它不表示"操作"。它用作操作元素的编组，例如组装车门的一组操作。过程模块表示产品驱动的过程结构（product-driven process structure），其本身由构建产品的功能性组件决定。该结构独立于工厂特定的过程结构（plant-specific process），后者表示车间中实际发生的操作及操作顺序。过程结构通常由 eMServer 中的过程（Twin）对象表示。

**过程元素（Process Element）** 是在过程模块（产品驱动结构）内管理、然后分配到过程结构（工厂特定过程）的操作，即过程元素在这两个结构之间共享。

过程元素是表示分配给车间操作人员的实际操作的对象。过程元素在过程模块内管理，即零件、工具、变体条件（以及任何其他相关信息）被指定给过程元素。过程元素对象派生自复合操作（Compound operation）对象，并进行了一些修改。

过程模块和过程元素显示在**过程模块树（Process Module Tree）**中。这是这些对象的专用视图，与其他任何树视图一样，允许管理此信息，例如新建、删除、编辑属性、操作树 - 时间选项卡编辑操作节点属性（包括增值和非增值时间）等、操作树 - 常规选项卡编辑操作节点属性。

与所有树一样，过程模块树允许用户按标题对树对象排序，并可选择将此顺序保存到数据库。此外，过程模块树使用户能够将已分配和未分配的过程元素指定到"实际过程结构（actual process structure）"。

过程模块树结合能力条（Power Bar）和关系查看器（Relations Viewer），使您能够排序和搜索数据、显示指定的工具和零件，并按颜色显示操作到工位的分配状态。

要打开过程模块树，请选择 **Home** 选项卡 → **Viewers** 组 → **Viewers**，并选择 **Process Module Tree**。过程模块树视图显示当前已加载的所有过程模块。

**排序过程元素（Sorting Process Elements）**

过程模块树包含过程元素，您可以通过拖放重新排序，或单击图标执行自动字母排序。

> **注意**
> 当排序按钮被按下（激活）时，您无法通过拖放重新排序节点。

**将元素分配到工位（Allocating an Element to a Station）**

您可以简单地将过程元素拖放到工位上，将其分配到工位。

要移除此分配，请右键单击工位范围（scope）中的过程元素（即从操作树视图，而不是从过程模块视图），并从上下文菜单中选择 **Delete** 命令。此操作将元素与工位断开，但不断开与过程模块范围（scope）的连接。

> **注意**
> 从过程模块视图中选择过程元素并选择 Delete 会尝试从数据库中移除该元素。若该过程元素当前已分配到工位，此操作会失败。要能够从过程模块树（和数据库）中删除元素，请先按上述方法将其与工位断开。

**显示分配状态（Displaying Allocation Status）**

过程模块树允许您自动指示哪些过程元素已分配到工位。您可以使用此功能快速确定哪些操作仍需分配。单击图标将已分配的过程元素节点显示为绿色，未分配节点显示为红色。使用 Options 对话框可更改分配指示颜色。

**按标题搜索（Searching by Caption）**

单击图标可对过程模块和过程元素标题执行区分大小写的搜索。搜索会在过程模块树折叠时将其展开。

**显示过程模块特征（Displaying Process Module Features）**

您可以打开 Process Designer 关系查看器（Relations Viewer）以显示与所选过程模块或过程元素关联的所有零件、工具等。当选择过程模块且 **Show Sub-Hierarchy** 选项激活时，关系查看器显示与子过程元素关联的零件和工具。

单击图标可显示所选操作（特别是过程模块或过程元素）的子操作关联的所有特征的递归总计。

<a id="v3-s58"></a> <!-- p497 -->
### 产品树（Product Tree）

**产品树（Product tree）** 描述产品的包含层级。例如，给定产品 A 和 B，产品树显示它们如何组合生成产品 C。将此树用于呈现外部系统生成的现有产品结构，以及编辑产品组。

- 要显示产品树，请选择 **Home** 选项卡 → **Viewers** 组 → **Viewers**，并选择 **Product Tree**。产品树打开并显示当前项目的数据。

<a id="v3-s59"></a> <!-- p497 -->
### 资源树（Resource Tree）

**资源树（Resource tree）** 包含自顶向下的层级视图，显示用于制造产品的所有资源，从整个工厂到单个工具 and 夹具。当前活动资源以浅蓝色框环绕显示。通过从导航树拖放而添加到工程数据中的任何新资源，都会自动作为此资源的子项添加。

要显示资源树：

**操作步骤**

1. 在导航树（Navigation tree）中选择所需的资源树节点。
2. 双击该节点，或右键单击并从打开的上下文菜单中选择 **Open** 或 **Open With**。随即为所选节点显示资源树。

<a id="v3-s60"></a> <!-- p498 -->
### 示意图查看器（Schematic Viewer）

#### 制造过程的示意图查看（Schematic Viewing of Manufacturing Processes）

要打开示意图查看器，请选择 **Home** 选项卡 → **Viewers** 组 → **Schematic Viewer**。

示意图查看器（Schematic Viewer）模块使用户能够在物理工厂环境中规划、管理和验证制造过程。该模块提供制造过程的完整可视化概览，以及与所有其他查看器（例如查看器（Viewer））的完全选择同步。

使用示意图查看器，您可以在高级视图中显示任何工位范围（station scope）的过程信息，一目了然地提供所有重要方面。

每个工位都有一个物理位置和图形表示，示意图查看器使您能够激活不同的逻辑信息层：

- **零件层（Parts layer）**：查看工位消耗的产品，并通过将产品加载到查看器（Viewer）中验证装配过程。
- **操作层（Operations layer）**：查看每个工位的下一级操作。
- **资源层（Resources layer）**：查看每个工位消耗的资源。
- **时间路径计划层（Time Way Plan layer）**：在基于甘特图（Gantt）的图表中显示已分配的操作，并通过提供操作分配和生产线平衡（line balancing）信息，评估过程所需的工位数。
- **Mfg 层（Mfg layer）**：显示分配给每个工位的 Mfg 列表。

> **注意**
> 您可以使用 Customize 自定义示意图查看器中可用的右键菜单。

> **重要提示**：为使示意图查看器正常工作，必须安装 .NET Programmability Support。使用以下步骤安装 .NET Programmability Support 组件：
> **操作步骤**
> 1. 在控制面板中运行"添加/删除程序（Add/Remove Programs）"。
> 2. 在 Microsoft Office Visio 2010 Setup 对话框的 Maintenance Mode Options 屏幕中，选择 Add or Remove Features，并单击 Next。
> 3. 展开 Microsoft Office Visio 节点并选择 .NET Programmability Support。单击 .NET Programmability Support 旁边的下拉箭头选择更新选项，并选择 Run from My Computer。

**示意图查看器工具栏（Schematic Viewer Toolbar）**

工具栏包含以下选项（已精简）：加载/添加工作站（Load/Add Workstations）、打开设置（Open Settings Options）、将图片保存到文件（Save Picture to File）、按流排列工作站（Arrange Stations According to Flow）、保存位置（Save Locations）、背景位图（Background Bitmap）、最小化所有层（Minimize All Layers），以及各数据层切换：零件层、资源层、操作层、时间层（Timing Layer）；缩放命令：放大、缩小、缩放到选择、适应页面（Fit to Page）；页面设置（Page Setup）与打印（Print）。

**示意图查看器信息（Schematic Viewer Information）**

示意图视图表示生产线上的工位。示意图查看器以分层方式提供信息，您可以一次查看以下信息层之一：工位使用的资源、工位消耗的产品、工位操作的时间、工位上的 Mfgs。

示意图查看器使您能够选择每个工位显示的信息，并更改工位在示意图查看器中的位置。您还可以将操作和资源从示意图查看器中的工作站拖放到任何其他查看器。

每个工作站以示意图查看器中的形式表示，包含以下部分：

- **Station Information（工位信息）**——包含工作站的标题和显示工作站是否已签出或签入的图标。
- **Layers Summary（层摘要）**——显示数据层中可用信息的摘要，包括工位正在使用的操作、资源和零件的数量。
- **Data Layer（数据层）**——提供有关工作站的信息，可显示以下内容之一。

**工位符号（Station Symbol）**

每个工位都带有一个符号创建。当所有层都最小化时，工位符号出现在该工位的数据层中。您可以使用属性（Properties）对话框中的选项卡配置用于工位、区域（zone）或生产线（line）的符号。

用于工位的符号由定义了符号的最详细组件（从工位起，一直到区域或生产线）决定。若未定义符号，系统使用默认工作站符号。

符号的中心表示工位在布局图（floor plan）中的位置。符号可以指向左、右、上或下。您可以使用工作站属性对话框更改符号的方向。

**时间路径计划（Time Way Plan）**

工作站的时间路径计划层提供工作站操作的时间和位置信息。它显示工作站符号作为背景。以紫色类甘特图时间条表示，条的长度表示完成操作所需的时间量。这基于输送带速度、节拍时间（cycle time）和节距（pitch）转换为操作过程中输送带移动的距离。

时间路径计划层左边缘的三角形表示工位输送带的高度。高度以相对于工作站物理高度的相对项表示。例如，若度量单位为毫米，则可能在 0mm 到 2000mm 之间。（注意：示意图查看器采用您在 Process Designer 中定义的通用度量单位。）高度值显示在三角形右侧。您可以使用工作站属性对话框更改输送带高度。

时间路径计划层还表示操作发生的位置。图表上线的位置表示执行操作的工位中的物理位置。工作站上有 14 个物理位置，映射到时间路径计划层中的六个位置。下表列出 14 个物理工位位置及其代码和说明，以及默认映射到的时间路径计划位置代码（及说明）：

| 位置代码 | 说明 | 时间路径计划代码 | 时间路径计划说明 |
| --- | --- | --- | --- |
| L1 | Left 1（左 1） | LF | Left front（左前） |
| L2 | Left 2（左 2） | LM | Left middle（左中） |
| L3 | Left 3（左 3） | LR | Left rear（左后） |
| L4 | Left 4（左 4） | LR | Left rear（左后） |
| CF | Center front（中前） | LF | Left front（左前） |
| E | Engine（发动机） | LF | Left front（左前） |
| IF | Interior front（内前） | LM | Left middle（左中） |
| IR | Interior rear（内后） | RM | Right middle（右中） |
| T | Trunk（后备箱） | RR | Right rear（右后） |
| CR | Center rear（中后） | RR | Right rear（右后） |
| R1 | Right 1（右 1） | RF | Right front（右前） |
| R2 | Right 2（右 2） | RM | Right middle（右中） |
| R3 | Right 3（右 3） | RR | Right rear（右后） |
| R4 | Right 4（右 4） | RR | Right rear（右后） |

您可以使用工作站属性对话框的 **Part Direction** 选项卡自定义 14 个工作站位置到时间路径计划层中六个位置的映射。您还可以定义工作站中的阻挡区域（blocking areas），表示可能阻止在特定部分执行操作的物理元素，例如管道、梁等。阻挡区域可使用工作站属性对话框定义（参见 Station Tree - TWP 选项卡）。

**显示操作标题（Displaying the Operation Caption）**

操作标题显示在操作条上。系统使用以下算法自动尝试防止相邻操作条标题重叠：

**操作步骤**

1. 系统首先尝试将标题的每个字段显示在不同的行中：第一个字段在顶行，然后第二个字段，依此类推。若操作标题与另一操作的标题重叠，系统转到步骤 2。
2. 系统尝试将多个字段放在一行中，从而减少行数。标题中字段的顺序不变。若标题仍与另一操作的标题重叠，系统转到步骤 3。
3. 系统尝试按标题中的空格字符将标题拆分为多行。若仍重叠，转到步骤 4。
4. 系统尝试像上一步那样将标题拆分为多行，并尝试将多个字段放在一行中以减少行数（标题中字段顺序不变）。若仍重叠，转到步骤 5。
5. 系统根据可用行数和相邻操作的位置，计算每行可显示的最大字符数，并尝试按此长度拆分标题。若仍重叠，转到步骤 6。
6. 系统无法折叠标题以避免重叠，因此在条上仅显示操作名称，并在工位底部显示完整标题。页面底部为标题保留了一些空间，但不检查重叠。

> **注意**
> 若在条上显示名称会导致与另一操作标题重叠，则不显示该名称。

系统允许标题延伸，直到同工位同位置（车中）的下一操作的标题，或直到操作条的末尾，以较长者为准。系统仅在分配到同一工位的操作之间执行重叠检查。若操作条超出工位的节拍时间，系统不检查与后续工位的操作重叠。

**打印工位（Printing the Station）**

打印对话框使您能够仅打印应用程序屏幕中可见的内容，而不是打印所有工位。每页 A4 纸打印一个如 Visio 屏幕所示的工位。打印前，您可以在 Visio 网格上移动工位。

打印对话框包含以下字段：

- **Name**——打印机的路径（位置和名称）。只能选择系统识别的打印机。
- **Where**——由系统提供。有关打印机的其他详细信息，例如其物理位置。
- **Current View**——指定打印输出是否应与屏幕上显示的完全一致（当前视图）。

**Mfg 层（Mfg Layer）**

Mfg 层显示分配给工位的全部 Mfgs（PLPs 和焊接点）列表。默认显示所有工位的全部 Mfgs。您可以单击所需工位的 Mfg 按钮，选择仅显示一个工位的 Mfgs。

**配置示意图查看器设置（Configuring Schematic Viewer Settings）**

您可以使用设置对话框配置示意图查看器设置。要打开设置对话框，请单击图标。

使用设置对话框配置以下示意图查看器设置（建议使用米或英尺为单位）：

- **Work Area（工作区）**选项卡——配置示意图查看器中工作区的大小。可更改的维度包括：Start X（最左侧工作站的横坐标）、Start Y（最低工作站的纵坐标）、Width（工作区宽度）、Height（工作区高度）。
- **Defaults（默认值）**选项卡——配置示意图查看器中工作站的默认节拍时间（Cycle Time）、节距（Pitch）和输送带速度（Conveyor Speed）、工位高度（Station Height）。节拍时间与节距可通过公式 `Conveyor Speed = Station Pitch / Cycle time` 定义输送带速度（连续输送带情况）。修改节拍时间和节距可更改输送带速度。
- **TWP Layer** 选项卡——Caption's Font Size（标题字体大小）设置网格上显示的标题字体大小。

**配置页面设置（Configuring Page Settings）**

页面设置（Page Setup）对话框使您能够配置打印示意图查看器显示的页面大小和设置。打开后包含字段：Standard paper size（标准纸张大小，从下拉列表选择）、Custom paper size（自定义纸张大小，选择长度单位并输入长度和宽度）、Orientation（方向：Portrait 纵向 / Landscape 横向）、Page fit（页面适应：One page 单页 / Split to several pages 拆分为多页，使用 Vertical 和 Horizontal 字段选择各方向页数）。

**配置背景（Configuring the Background）**

背景图像（Background Image）对话框使您能够为示意图查看器选择背景，帮助可视化工位在工厂车间上的布置并轻松定位特定工位。

要选择背景图像：

**操作步骤**

1. 选择 **Use Image**。
2. 单击"..."。打开标准文件浏览器。
3. 导航到要用作背景的图像文件并选择该文件。

   > **注意**
   > - 可用作背景的文件类型：BMP、JPG、GIF、TIF、DWG 和 DXF。
   > - 所选背景图像无法在示意图查看器中调整大小或修改。请确保图像大小适合工作区大小。

4. 单击 **Open**。
5. 单击 **OK**。示意图查看器随即显示所选背景图像。

要清除背景图像：

**操作步骤**

1. 选择 **No Image**。
2. 单击 **OK**。示意图查看器随即显示无背景图像。

<a id="v3-s61"></a> <!-- p515 -->
### 快照编辑器（Snapshot Editor）

**快照编辑器（Snapshot Editor）** 显示从查看器（Viewer）中显示的工程数据创建的快照。快照用于存储工作中的特定视图和视角，以备日后参考。快照保留工作单元（workcell）的当前视图。记录的视图包括设备姿态（device poses）、注释 PMI，以及以下对象属性：

| 属性 | 说明 |
| --- | --- |
| Point of View | 将视图旋转到拍摄快照时的角度和缩放级别。 |
| Object Visibility | 恢复对象的可见性。若未勾选，所有对象保持当前设置（显示或隐藏）。 |
| Object Locations | 恢复快照拍摄时对象的位置。若此后位置已更新，将与快照中存储的位置不同。您可以使用此选项撤销更改，或在一个研究中使用不同布局。每个布局都可使用并保存到 eMServer。 |
| Device Poses | 若拍摄快照时设备被设置为某姿态，则恢复这些位置；否则保持当前姿态。 |
| Object Attachments | 对象间的附着（attachments）重置为拍摄快照时的状态。可能导致恢复附着或断开当前已附着的对象，如同拍摄快照时那样。 |
| Object Colors | 将对象颜色恢复为拍摄快照时的颜色。若未勾选，颜色保持当前不变。 |
| Object Viewing Mode | 当前为线框（Wireframe）模式、但拍摄快照时为着色（Shaded）模式的对象，选择此选项后恢复为较早的显示模式；否则保持当前模式。 |
| PMI Text Size | 若产品制造信息（PMI，来自 JT 文件）的文本大小自拍摄快照后已更改，勾选此选项可恢复。 |

> **注意**
> 有关 PMI 的更多信息，参见 Options 对话框的 General 选项卡。

**操作步骤**

1. 选择 **Home** 选项卡 → **Viewers** 组 → **Viewers**，并选择 **Snapshot Editor**。
2. 单击 **New Snapshot**。当前在查看器（Viewer）中显示的图像的新快照出现在快照编辑器上半部分，默认名称为 `Snapshot_#`。

   > **注意**
   > 您可以在快照编辑器上半部分以三种视图显示快照：列表、小图标或大图标（如上）。使用快照编辑器中的按钮选择视图。快照编辑器下半部分显示当前所选快照。

3. 创建快照后，您可以使用快照编辑器中的按钮执行以下功能：

   | 图标 | 名称 | 说明 |
   | --- | --- | --- |
   | Apply Snapshot | 应用快照 | 用所选快照替换查看器（Viewer）中的图像。单击下拉箭头并选择可应用以下任意项：Point of View、Objects Visibility、Objects Locations、Device Poses、Objects Attachments、Objects Colors、Objects Viewing Mode。默认仅应用视点。 |
   | Edit Snapshot Properties | 编辑快照属性 | 使您能够为所选快照输入名称、类型和说明。快照必须具有唯一名称。为快照输入说明后，若快照显示在快照编辑器下半部分，说明会显示在快照旁边。 |
   | Update Snapshot | 更新快照 | 将所选快照更改为当前在查看器（Viewer）中显示的图像。 |
   | Remove Snapshot | 移除快照 | 删除所选快照。 |
   | Add Markup | 添加标记 | 打开标记编辑器（Mark Up Editor），并显示所选快照。使用标记编辑器，您可以向快照添加标注和文本，并将图像保存为 .bmp 或 .jpg 文件。参见 Markup Editor。 |
   | Remove Markup | 移除标记 | 删除使用标记编辑器添加到快照的任何标记。仅当快照包含标记时此选项才启用。 |
   | List | 列表 | 在快照编辑器上半部分以列表视图显示快照。 |
   | Small Icons | 小图标 | 在快照编辑器上半部分以小图标显示快照。 |
   | Large Icons | 大图标 | 在快照编辑器上半部分以大图标显示快照。 |

<a id="v3-s62"></a> <!-- p517 -->
### 操作树中的焊接平衡（Weld Balancing in the Operation Tree）

- 单击操作树中的 **Weld Balancing Indications** 切换图标可使用焊接平衡。对数据库的任何更改都会在线更新。
- **Weld Balancing** 选项包含用于平衡和分析焊接操作的子选项。
- 单击操作树中的 **Weld Balancing Indications** 切换图标可使用 **Weld Balance Indications** 命令。
- **Balance View Mode** 选项比较可用焊接时间与已用焊接时间，以优化焊接点分布。

要平衡焊接点：

**操作步骤**

1. 将操作加载到操作树，并单击 **Weld Balancing Indications** 切换图标。
2. 选择 **Tools** → **Weld Balancing**，并单击操作树中的 **Weld Balancing Indications** 切换图标。

   每个焊接操作以颜色代码显示：
   - **红色**：已用焊接时间大于可用时间。
   - **绿色**：已用焊接时间小于可用时间。
   - **无阴影**：已用焊接时间等于可用时间。

3. 修改焊接点的已分配时间或位置。

您还可以激活 **Weld Analysis**，参见 Weld Analysis。

<a id="v3-s63"></a> <!-- p519 -->
### 变体编辑器（Variant Editor）

参见 Variant Editor。

<a id="v3-s64"></a> <!-- p519 -->
## 查看器（Graphic Viewer）

**Direct Model 查看器（Graphic Viewer）** 使用 Vis 图形引擎，是一个显示当前 3D 工程数据内容的图像窗口。查看器（Graphic Viewer）始终可用。

Direct Model 查看器提供以下优势：

- **高级可视化能力**——Vis 图形引擎提供逼真的图形控制和各种功能，例如特征线（Feature Lines）、选择时零件的透明度（Transparency）、组件纹理（Component Textures）查看、围绕所有轴的高级旋转等。
- **可优化的硬件资源利用**——例如渐进式加载（gradual loading），使用户能够并行开始工作。用户还可以预定义应用程序分配的内存，以防止加载大型研究时出现问题。您还可以设置图形的刷新率，以更好地控制大数据集的旋转行为。此外，Direct Model 设计用于利用新的双核 CPU 技术优化性能。
- **显卡支持**——Direct Model 支持 NVIDIA 和 ATI 高端显卡。
- **最先进的图形技术**——JT 是 Siemens Digital Industries Software 的标准，并被 3D 应用社区广泛接受为稳定可靠的格式。
- **4K 显示器支持**。

> **注意**
> - 若在 Motion 选项卡中设置了 Limit joint motion，则关节运动受关节物理限制限制。
> - 若在 Motion 选项卡中设置了 Highlight joint limits，查看器（Graphic Viewer）会以颜色显示超出工作或物理限制的关节。若清除，则没有关于物理限制的颜色指示。有关配置这些颜色，参见 Appearance 选项卡。
> - 在某些情况下，理论上需要以多种颜色显示对象。例如，若关节超出工作限制且处于碰撞状态。但由于只能显示一种颜色，系统使用以下优先级顺序确定实际显示颜色：
>   - 选择颜色（Selection color）
>   - 碰撞颜色（Collision color）
>   - 高亮颜色（Emphasize color，例如高亮仿真事件）
>   - 超出物理限制颜色（Exceeded physical limits color）
>   - 超出工作限制颜色（Exceeded working limits color）
>   - 实例颜色（Instance color）

<a id="v3-s65"></a> <!-- p520 -->
### 多个查看器（Multiple Graphic Viewers）

您可以打开查看器（Graphic Viewer）的多个实例。如果您希望从多个不同视点观察图形数据或仿真，这会很有用。例如，在分析数据时，您可以看到图形的顶视图和侧视图，或两个等距视图，一个缩放至特定区域，一个显示整个工作站。

> **注意**
> - 查看器（Graphic Viewer）会编号并显示相同的数据。任意时刻只有一个查看器处于活动状态——其标题栏以颜色强调。
> - 关闭打开的查看器（Graphic Viewer）时，最后一个保持打开——它无法关闭。
> - 与对象相关的操作（例如选择或删除对象）影响所有查看器（Graphic Viewer），而与窗口相关的操作（例如缩放或旋转显示）仅影响活动查看器（Graphic Viewer）。
> - 有关查看器（Graphic Viewer）数量的信息不存储在布局管理器（Layout Manager）中。因此，重新加载数据后，仅出现查看器（Graphic Viewer）的单个实例。

要显示其他图形查看器：

- 选择 **View** 选项卡 → **Screen Layout** 组 → **New Window**。

随即显示一个额外的查看器（Graphic Viewer），它最大化并成为活动查看器（Graphic Viewer）。

要排列图形查看器：

- 选择 **View** 选项卡 → **Screen Layout** 组 → **Arrange Windows**，并选择以下之一：Vertical、Horizontal、Tiled、Cascade、Tabbed。

查看器（Graphic Viewer）按您的选择排列。

要激活图形查看器：

执行以下任一操作：

- 选择 **View** 选项卡 → **Screen Layout** 组 → **Switch Windows**，并从列表中选择所需窗口。
- 在所需窗口中的任意位置单击。

> **注意**
> 关闭活动窗口会导致另一个窗口变为活动状态。

<a id="v3-s66"></a> <!-- p522 -->
### 按需加载（Load on Demand）

**按需加载（Load on Demand）** 技术提供更好的加载时间和性能，它在首次加载 3D 数据时根据用户需求将 3D 数据加载到查看器（Graphic Viewer）中。加载对象时，默认情况下不会显示 3D 数据，直到用户在相关树中切换对象旁边的图标。参见"在查看器中隐藏/显示对象（Blanking/Displaying Objects in the Viewer）"。若研究在查看器（Graphic Viewer）中显示 3D 数据的情况下保存，下次打开该研究时，所有 3D 数据会自动显示。若研究在没有任何 3D 数据显示的情况下保存，下次打开该研究时，3D 数据将仅按需显示。

<a id="v3-s67"></a> <!-- p523 -->
### 在查看器中隐藏/显示对象（Blanking/Displaying Objects in the Viewer）

无论元素或节点在查看器（Graphic Viewer）中是隐藏还是显示，它们都会显示在树中。您可以通过除导航树（Navigation Tree）外所有树中节点名称旁边的切换图标，在查看器（Graphic Viewer）中隐藏和显示对象。单击切换图标时，它在以下图标之间切换：

| 图标 | 说明 |
| --- | --- |
| 实心蓝色图标 | 表示当前显示在查看器（Graphic Viewer）中的对象。 |
| 空心蓝色图标 | 表示当前未显示在查看器（Graphic Viewer）中的对象。 |
| 半蓝图标 | 表示具有部分子项显示在查看器（Graphic Viewer）中的复合节点。 |
| 蓝色 X 图标 | 表示对象没有任何 3D 表示，无法在查看器（Graphic Viewer）中显示。 |

<a id="v3-s68"></a> <!-- p524 -->
### 在查看器中使对象变暗（Dimming Objects in the Graphic Viewer）

查看器（Graphic Viewer）中已签入、或由其他用户签出的对象可以变暗（dimmed），以区别于您已签出并可操作的对象。

更多信息参见 General 选项卡中的 Dim Non-Checked Out Objects 部分。

<a id="v3-s69"></a> <!-- p524 -->
### 在查看器中操纵视图（Manipulating the View in the Graphic Viewer）

您可以使用鼠标按键在查看器（Graphic Viewer）中操纵视图并控制对象的显示方式，如下所示：

- **中键和右键**：同时使用这两个按钮可旋转对象的视点。向任意方向移动鼠标都会更改视点。鼠标在屏幕上移动的距离影响旋转量。
- **中键**：使用中间按钮在查看器（Graphic Viewer）中放大或缩小。向左或向右移动鼠标影响缩放方向。缩小请向左移动鼠标，放大请向右移动鼠标。鼠标在屏幕上移动的距离影响缩放幅度。
- **右键**：使用右键在查看器（Graphic Viewer）中平移（panning）。向任意方向移动鼠标都会影响平移方向。

> **注意**
> 要缩放某个区域：从空闲区域开始，按住 Alt 键同时拖动选择框，以缩放选择框内的内容。

或者，您可以从选择查看器（Graphic Viewer）时显示的弹出窗口中选择一种视图模式（Rotate、Pan 或 Zoom）。当前选择的模式保持活动，直到您选择 Select 选项。

<a id="v3-s70"></a> <!-- p525 -->
### 透明度（Transparency）

Direct Model 图形引擎使用户能够选择一个复杂装配并以透明方式查看其内部组件。

<a id="v3-s71"></a> <!-- p525 -->
### 组件纹理（Component textures）

显示引擎可以显示带有纹理的 JT 文件（在 CAD/渲染系统中定义）。这有助于提供更逼真的模型视图。

<a id="v3-s72"></a> <!-- p526 -->
### 在查看器中操纵图像（Manipulating images in the Graphic Viewer）

您可以使用鼠标按键在查看器（Graphic Viewer）中操纵图像并控制对象的显示方式，如下所示：

- **中键**——使用中间按钮放大或缩小查看器（Graphic Viewer）。向左或向右移动鼠标影响缩放方向。缩小向左移动鼠标，放大向右移动鼠标。鼠标在屏幕上移动的距离影响缩放幅度。
- **右键**——使用右键在查看器（Graphic Viewer）中平移。向任意方向移动鼠标都会影响平移方向。
- **中键和右键**——同时使用这两个按钮可旋转对象的视点。向任意方向移动鼠标都会更改视点。鼠标在屏幕上移动的距离影响旋转量。

> **注意**
> 您可以使用键盘上的箭头键在查看器（Graphic Viewer）中将视图向任意方向旋转 30 度。

或者，您可以选择以下视图模式之一：Rotate、Pan 或 Zoom。所选视图模式保持活动，直到您选择 Select 选项。

Process Designer 提供以下两种鼠标移动模式：

- **直接查看（Direct viewing）**——仅在移动鼠标时对象才移动。
- **连续查看（Continuous viewing）**——对象以鼠标移动的初始速度继续移动。

您可以在查看器（Graphic Viewer）选项卡中设置鼠标移动模式。

<a id="v3-s73"></a> <!-- p526 -->
### 使用 3D 导航设备（Using 3D navigation devices）

Process Designer 支持 3D 导航设备，以便于在 3D 环境中操纵对象并提高效率。3D 设备是在标准鼠标之外安装的。

3D 导航设备使您能够通过操控类鼠标设备来操纵 3D 环境中的对象。您可以在不点击和拖动用户界面的情况下执行以下操作：

- 平移（Pan）
- 缩放（Zoom）
- 倾斜（Tilt）
- 旋转（Spin）
- 翻滚（Roll）

每个设备还提供可配置的按钮以进行额外控制。

Siemens 已检查并批准 3DConnexion® 制造的以下设备：SpacePilot、SpaceExplorer、SpaceNavigator。有关这些设备的安装说明和完整详细信息，请参考 http://www.3Dconnexion.com。

> **注意**
> - Process Designer 支持 3DConnexion 设备驱动程序版本 6.6.4 或更高，以及 3DConnexion 软件版本 3.6.11 或更高。
> - 若您在安装 3D 导航设备之前安装了 Process Designer，必须使用 DVD Add-ons 文件夹中位于的 SpaceBallMouse.msi Add on。
> - 若您在安装 3D 导航设备时 Process Designer 正在运行，必须重启 Process Designer 才能识别该设备。

<a id="v3-s74"></a> <!-- p527 -->
### 在查看器中选择对象（Selecting objects in the Graphic Viewer）

通过单击可选择单个对象。按住键盘上的 `<Ctrl>` 并单击所需对象，或者通过围绕所需对象拖动选择框，可以选择多个对象。

在查看器（Graphic Viewer）中选择对象时，您选择的点由符号指示。

**操作步骤**

1. 单击 **Pick Intent** 图标，以确定单击对象时的精确点。参见 Pick Intent。
2. 选择 **Pick Level**（Component 或 Entity），如下所示。参见 Pick Level。
3. 您可以独立选择组件框架（Component Frames，即组件内的框架）。

<a id="v3-s75"></a> <!-- p529 -->
### 剖面（Sections）

用户可以在查看器（Graphic Viewer）中创建剖面（sections）并操纵它们。通过沿指定轴移动和旋转剖切面（section planes），您可以裁剪或切割场景中的对象，以便聚焦于感兴趣的区域。

剖面过程中大部分功能仍然可用（例如尺寸、碰撞）。

创建 **New Section Plane** 后，您会看到 Sections 命令以激活、裁剪或切割其中一个或多个剖切面。

您可以显示与剖切面相交的对象的轮廓（Contours）。

**受管剖切面（Managed Section Planes）**

创建 New Section Plane 时，它们默认显示为带有可见平面和边框线的形式。同时打开 Section Manager，使您能够通过平移和旋转其位置来操纵剖面。受管剖面（Section Manager 打开的）和非受管剖面各自具有默认平面颜色和透明度百分比，以及边框颜色。您可以在 Appearance 选项卡中修改这些剖面属性。更改这些设置时，它们会立即应用于您创建的新剖切面，以及重新加载研究后的现有剖面。

**激活/停用剖切面（Activated/Deactivated Section Planes）**

创建剖面后，您必须（在 Sections 命令中）激活它，使其能够在您通过场景操纵剖切面时执行裁剪或切割。您可以停用某个平面以停止裁剪或切割，即使您可能继续移动它。存在多个剖面时，您可以一次激活多个平面。可同时打开的激活平面数量受机器显卡能力的限制。

激活剖面会自动隐藏其内部平面，仅保留边框可见。

创建剖面时，它会列在对象树（Object Tree）的 Sections 文件夹下。其旁边显示一个图标，表明剖面是激活还是停用。在查看器（Graphic Viewer）中更改剖切面的激活状态会自动更新树中相应的叠加层。

> **注意**
> 激活剖面会禁用 Dynamic Clipping 功能。

**剖切面的位置和大小（Position and Size of Section Plane）**

创建剖面并选择对象时，剖面位于对象的几何中心，剖面大小根据该对象的大小确定。对于所选框架或位置，系统根据框架或位置的方向和位置创建剖面。

通过添加和删除对象或更改其显示/隐藏状态来修改场景，不会自动更改平面大小。您可以使用剖面命令中的调整剖面平面大小（adjust section plane size）命令，将所选剖切面的宽度和高度更新为当前显示对象边界框（bounding box）的大小。
<a id="v3-s76"></a> <!-- p530 -->
## 浏览过程数据库（Navigating the Process Database）

**导航树（Navigation tree）** 是浏览过程数据库的起始点。使用导航树可视化项目及任何共享库的内容，访问任何常规和私有数据项，并管理数据。

为便于在树中移动，导航树提供后退箭头、前进箭头和 Home 图标。

将项目拖放到放大镜上，使您能够在当前树中定位任何对象。从另一个树或查看器（Viewer）拖放对象，若该对象位于所显示树的前两级，它会在导航树中打开。

若将变体集（variant set）拖放到放大镜上，Process Designer 会在当前树中搜索该变体集及其所有已指定对象。有关变体集的更多信息，参见 Variant Sets and Variant Filters。

您可以通过从导航树拖放对象到相关树来加载数据。您还可以拖放复合资源或原型并放置到查看器（Viewer）中的任意位置，以在工程数据中创建资源的实例。新资源出现在资源树（Resource Tree）中的活动资源之下。

> **注意**
> 若当前导航树的根节点是项目节点本身，则搜索已定义对象会导致 Process Designer 为该对象的每次出现打开导航树的单独实例。

要显示导航树：

- 选择 **Home** 选项卡 → **Viewers** 组 → **Navigation Tree**。

导航树打开并显示当前项目的数据。

您可以单击导航树图标打开导航树的多个实例。每单击一次图标，就会打开导航树的额外实例，每个实例以 1 到 5 的数字标识。在任何一种布局（layout）中最多可打开五个导航树，系统会将所有打开的导航树保存到您的布局中。

> **注意**
> 若您从打开的导航树中选择一个对象并选择打开额外的树，所选对象就是额外树的主对象。

导航树工具栏上的历史图标显示该导航树中最近加载的至多 11 个对象。

双击树中具有可展开子项（以标记）的对象，可展开该树节点以显示子对象。若对象有子项但无法展开（例如位于距根超过一级的项目节点），双击该对象会将其移动到树的根。在所有其他情况下（例如没有子项的对象），双击不会对该对象执行任何操作。

> **注意**
> 当对象通过双击移动到树的根后，单击后退箭头会在新树中显示包含该对象的展开节点。若为共享节点，树会展开以显示该对象的所有实例。

您可以通过将资源、零件和操作拖放到导航树来分配它们。

**展开（Expand）**

每个展开选项使您能够快速展开树中的节点：

- **Expand 1 Level**：显示所选节点下一级子节点。
- **Expand 2 Levels**：显示所选节点下两级子节点。
- **Expand 3 Levels**：显示所选节点下三级子节点。
- **Expand All**：展开树中的所有分支（节点）。

键盘快捷键：`-` 折叠所选节点；`*` 展开全部。

**为 Twin 对象打开导航树（Open Navigation Tree for Twin Objects）**

要显示过程对象的 twin（即在导航树中打开 twin 过程资源），或反之：

- 选择对象并单击工具栏中的 **Open Twin with Navigation Tree** 图标；或
- 右键单击对象并从上下文菜单中选择 **Open Twin with Navigation Tree** 命令。

**在 Process Simulate 中打开项目（Opening Projects in Process Simulate）**

要在导航树中打开显示的项目并在 Process Simulate 中运行它，请右键单击项目并选择以下选项之一：

- Open with Process Simulate > Open with Process Simulate in Standard Mode
- Open with Process Simulate > Open with Process Simulate in Line Simulation Mode
- Open with eM-Review > Open with eM-Review in Standard Mode
- Open with eM-Review > Open with eM-Review in Line Simulation Mode

> **注意**
> eM-Review 是 Process Simulate 的受限只读版本。

<a id="v3-s77"></a> <!-- p533 -->
## 属性查看器（Properties Viewer）

<a id="v3-s78"></a> <!-- p533 -->
### 运行属性查看器（Running the Properties Viewer）

要打开属性查看器（Properties Viewer），请选择 **Home** 选项卡 → **Viewers** 组 → **Properties**。

属性查看器显示多个选项卡，其中包含有关当前所选对象的详细信息。属性查看器中显示的选项卡类型取决于所选节点/对象的类型。更多信息参见 Properties。

您可以通过单击属性查看器图标，或在导航树中右键单击对象并从上下文菜单中选择 **Properties**，来打开属性查看器的多个实例。任何一种布局中最多可打开五个实例。所有属性查看器具有默认全局行为。这意味着当从导航树中选择的对象更改时，布局中打开的所有查看器的内容会动态更改。当从查看器（Viewer）和树中选择各项时，查看器会自动更新其包含的信息以匹配选择。此外，在属性窗口中进行的任何选择也会更新属性查看器。

> **注意**
> 若您在 Name 字段中编辑对象名称，请勿使用"管道"字符（|）。

属性查看器的默认全局行为可以更改。每个查看器窗口右上角出现一组四个图标。下表说明每个图标的功能：

| 图标 | 说明 |
| --- | --- |
| 同步图标 | 将属性查看器与所选树或查看器同步。 |
| 锁定/解锁切换图标 | 锁定或解锁属性查看器的内容。在再次单击该图标解锁之前，查看器显示的内容不会更改。 |
| 保持打开/自动隐藏 | 保持查看器打开 / 当鼠标移开时自动隐藏查看器。 |
| 关闭 | 关闭属性查看器。 |

属性查看器可以与屏幕上可见的任何查看器或树同步。一旦查看器与另一个（导航树）同步，只有在该树中所做的更改会影响属性查看器的内容。属性查看器的标题栏指示它与哪个导航树同步。要结束同步，请从上下文菜单中单击 **Disconnect**。

> **注意**
> 同步属性保存到布局（Layout）。锁定属性不保存到布局。若查看器锁定到某个对象而该对象被删除，查看器会恢复到锁定之前的状态。

<a id="v3-s79"></a> <!-- p535 -->
### 属性（Properties）

属性指定有关当前项目的基本信息。属性窗口中显示的信息也会显示在查看器中。

要打开属性窗口：

- 选择 **Home** 选项卡 → **Viewers** 组 → **Properties**；或
- 在任意树中右键单击对象并选择 **Properties**。

<a id="v3-s80"></a> <!-- p535 -->
### Mfg 树属性（Mfg Tree Properties）

Mfg 树列出所加载数据的 Mfgs。它没有层级结构。无法添加新节点，但您可以将 Mfgs 从库拖放到已加载的树（而非空树）。

制造树（Manufacturing tree）节点的属性窗口包含以下选项卡：General、Attachments 和 Attributes。单个制造特征（manufacturing feature）的属性窗口包含以下选项卡：General、Physical、Times、Products、Process、Reports、Attachments 和 Attributes。这些选项卡中有许多也出现在其他树的属性窗口中。这些选项卡中的值可按"编辑产品节点属性（Editing Product Node Properties）"中的说明进行编辑。

**制造特征 - General 选项卡（Manufacturing Features - General Tab）**

General 选项卡中的大多数参数字段也出现在产品属性（ProductProperties）窗口的 General 选项卡中。仅以下描述的 Type 和 Subtype 字段是制造特征窗口 General 选项卡所特有的。未灰显的字段可编辑。

- **Type**：对于制造库节点，始终为 MfgLibrary。它在制造库节点创建时设置，无法更改。
- **Subtype**：对于几何点，Subtype 为 Dummy 或 Geo；对于点焊点，Subtype 为 Respot。这些子类型由用户设置。

**制造特征 - 其余选项卡（Manufacturing Features - Remaining Tabs）**

其余选项卡与产品属性（Product Properties）窗口中同名的选项卡相同。例外是 Products 选项卡，它与操作属性（Operation Properties）窗口中同名的选项卡相同。参见 Operation Tree - Products Tab、Product Tree - Operations Tab 和 Product Tree - Resources Tab。Products 和 Process 选项卡仅与制造特征相关。

<a id="v3-s81"></a> <!-- p538 -->
### 操作树节点类型（Operation Tree Node Types）

| 图标 | 名称 | 功能 |
| --- | --- | --- |
| Operation | 操作 | 任何通用操作。 |
| Source | 源 | 将零件引入计划的操作。 |
| Sink | 汇 | 将零件交付出计划的操作。 |
| Compound Operation | 复合操作 | 包含其他操作的操作。 |
| Compound Weld Operation | 复合焊接操作 | 包含其他操作的焊接操作。 |
| Operation Placeholder | 操作占位符 | 操作占位符。 |
| Process | 过程 | 表示通用、高级别操作/资源的对象（以前称为 Twin Object）。 |
| PrPlantProcess | 工厂过程 | 表示 Plant 类型过程的对象。 |
| PrZoneProcess | 区域过程 | 表示 Zone 类型过程的对象。 |
| PrLineProcess | 生产线过程 | 表示 Line 类型过程的对象。 |
| PrStationProcess | 工位过程 | 表示 Station 类型过程的对象。 |
| Weld Operation | 焊接操作 | 表示焊接操作的对象；只能将 WeldPoint 指定给此类操作。 |
| Task | 任务 | 自定义的低级操作。 |

<a id="v3-s82"></a> <!-- p539 -->
### 编辑操作节点属性（Editing Operation Node Properties）

双击任何操作节点会打开操作属性（Operation Properties）窗口，显示节点属性。其中部分节点属性可编辑。后续章节按属性排列的各个选项卡进行说明。

**Operation Tree - General Tab**

操作树属性 General 选项卡与其他树中的 General 选项卡几乎相同。此选项卡中显示附加字段：

- **Number**——继承自零件原型。
- **Variant Set**——变体集。
- **Station Sub Type**——仅当为 PrStation 类类型或从其派生的类型打开属性时才可见的下拉列表。可选值列表存储在系统根下的 XML 文件中。

**Operation Tree - Times Tab**

Times 选项卡显示执行操作所需的分配时间（allocated time）和实际时间（actual time）。此窗口中还显示 MTM 信息。操作分配时间可使用 Search 按钮或数据卡（Data Card，如下）选择，并可使用 Reset 按钮重置。

- **Allocated Time**——为操作指定的时间，通常是操作所需的最长时间。
- **VA（增值）**——为组装产品增加价值的操作持续时间部分（例如将后视镜固定到门上）。
- **NVA（非增值）**——不为产品增加价值的操作持续时间部分（例如走到料箱取零件）。
- **Verified Time**——执行操作的测量时间。选择您要验证时间的以下之一：Simulation、Measurement、MTM 或 MOST。
- **Calculated Time**——单击图标后，Process Designer 通过仿真所选节点及其所有子节点，在 Calculated Time 字段中计算并显示所需时间。计算对所选节点及其子树递归进行，考虑每个节点的计算方法。若在包含其他子项的复合上执行计算，则自叶节点自底向上计算。系统随后将 Calculated Time 复制到所选节点及其所有子节点的 Allocated Time。
- **Calculation Method**——可用于查找所选节点 Calculated Time 的以下方法之一：
  - **Longest path（最长路径）**——操作流中连接子项的最长单条路径的时间设为其 Calculated Time。若操作没有子项，则改用 Allocated Time。
  - **Sum（求和）**——所选节点所有子项的分配时间之和设为其 Calculated Time。若操作没有子项，则改用所选节点的 Allocated Time。
- **Copy from**——单击图标可方便地将 Verified Time 值复制到 Allocated Time。此复制操作在所有子操作上递归执行。

数据卡（Data Card）使用后更新以下 MTM 字段：**Basic Time**（MTM 时间 tg 属性，描述时间元素的基本时间，由数据卡确定且不能直接修改）、**Time Including Allowance**（实际时间指 MTM 时间 te 属性，描述基本时间加上任何额外津贴）、**Code**（数据卡提供的特定 mtm 序列 ID 号）、**Description**（从数据卡提取的时间元素的文本描述，不能直接修改）、**Frequency**（操作执行次数）。Allocated Time 和 Calculated Time 属性不受影响。

**DataCard**

数据卡（DataCard）提供快速参考工具以查看和检索 MTM 数据，同时不生成任何 MTM 代码。单击打开数据卡。数据卡包含按各种标准排列的信息表。每个表包含一组从数据卡导入到操作树 - Times 选项卡的标准化时间值，用于定义相应操作的预期持续时间。使用顶部的选项卡浏览各个表。数据卡按三个排序级别排列。找到所需时间值后单击它，该值会导出回操作的 Times 选项卡，数据卡自动关闭。数据卡填充以下字段：Verified Time、Basic Time、Time Including Allowance、Code、Description。Basic Time 字段旁边的组合框在时间单位 TMU 或 Seconds 之间切换。

**Operation Tree - Cost and Responsibility Tab**

Cost and Responsibility 选项卡定义负责所选对象及其预估成本的组的参数。Consumable Cost/Oper、Labour Hrs/Oper、Comment Cost 和 Comment Hrs 字段显示针对操作成本和执行操作所需工时计算的值。

**Operation Tree - Products Tab**

Products 选项卡包含单个窗格，列出操作中涉及的零件。该窗格显示每个零件的名称和目录号，并指示零件是操作的输入（消耗）还是由其生产。当零件作为消耗品添加到窗格时，会自动为其生成源操作（source operation），并出现在操作 PERT 图（PERT chart）中。

您通过从显示位置拖放零件来添加零件。例如，从产品树移动零件：

**操作步骤**

1. 打开产品树并显示代表所需产品的节点。节点无需签出。
2. 将节点拖到 Products 选项卡窗格。

要从 Products 选项卡删除零件，请选择它并单击 **Delete**。

**Operation Tree - Manufacturing Features Tab**

Manufacturing Features 选项卡列出已指定的 MfgFeature 对象（焊接点、ContinuousMfgs 等——PLP 除外）。这些焊接点的顺序可通过选择焊接点，然后使用上下箭头将其移动到列表中的不同位置来更改。

除以上描述的特征外，Manufacturing Features 选项卡与产品属性窗口中同名的选项卡相同；参见 Product Tree - Manufacturing Features Tab。制造特征可以从制造库复制到 Manufacturing Features 选项卡。库无需签出。可以通过选择制造特征并按 Delete 从选项卡中删除它。

**Operation Tree - PLP Tab**

Locate 操作的属性对话框包含 PLP 选项卡，显示所选操作已指定的 PLP（用法，usages）列表。您可以通过从 Datum 库拖放或选择并右键单击来添加 PLP。同样，您可以通过将 PLP 用法及其值从此选项卡复制到其他操作来指定它们。您也可以通过选择它们并按 Delete 或从右键菜单选择 Delete 命令来删除已指定的 PLP。

PLP 选项卡包含 Assignment Details 部分，包括字段：**PLP Type**（带预定义值列表的下拉菜单，也可使用 PLP Manager 定义）、**Control Direction**（若已为一个或多个 X、Y、Z 方向赋值，可激活或停用该方向的使用，否则方向参数灰显）、**Position-Primary Axis**（灰显）、**Attribute List**（显示可编辑的数据）。

**Operation Tree - Remaining Tabs**

其余选项卡与产品属性窗口中同名的选项卡相同。您通过从显示位置拖放资源来将资源添加到 Resources 选项卡。例如，从资源树移动资源：

**操作步骤**

1. 打开资源树并显示包含要移动资源的节点。节点无需签出。
2. 将节点拖到 Resource 选项卡。

要从 Resources 选项卡删除资源，请选择资源并单击 **Delete**。

**将库零件复制到产品树（Copying a Library Part to a Product Tree）**

作为使用 New 命令的替代方法，您可以通过将原型（prototypes）从零件库（Parts Library）拖放到产品树中的任何复合零件节点，来将节点添加到产品树。

**操作步骤**

1. 打开零件库树（Parts Library Tree）。
2. 选择一个或多个库零件。
3. 将库零件拖到产品树中的复合零件节点（在签出它之后）。松开鼠标按钮。零件实例作为零件节点出现在复合零件节点下。这些零件是库中原型零件的新实例。

产品树 - General 选项卡指示新节点类型为 PartPrototype(i)，表示新实例继承了库原型的属性。

> **注意**
> 零件实例在从零件库拖放原型零件，或从其他零件实例复制时创建。

**将对象从一个产品树移动到另一个产品树（Moving Objects from one Product Tree to another Product Tree）**

您可以将单个实例或操作从一个产品树拖放到另一个产品树。

<a id="v3-s83"></a> <!-- p547 -->
### 编辑产品节点属性（Editing Product Node Properties）

单击任何产品节点会打开属性窗口，显示节点属性。其中部分节点属性可编辑。以下是可用于修改属性的各个选项卡说明：

**Product Tree - General Tab**

产品树属性 General 选项卡与其他树的 General 选项卡几乎相同。但是，产品树的 General 选项卡包含两个附加字段：**Number**（继承自零件原型）和 **Variant Set**（所选产品的变体集）。

**Product Tree - Physical Tab**

Physical 选项卡显示以下列出的参数并简要说明。若零件关联了图像文件以说明它，则显示图像并显示文件名。用户只能编辑位置和方向参数。此选项卡中显示的其余数据继承自零件原型。

- **Location**——相对于根产品参考系的零件 X-Y-Z 坐标值。要修改这些值，请单击并在打开的窗口中输入新值。
- **Orientation**——表示绕与零件坐标系原点相交、但平行于产品参考系 X、Y、Z 轴的轴旋转的数值（弧度）。这些旋转表示零件相对于产品参考系的方向。方向使用 roll-pitch-yaw（RPY）方法，先绕 X 轴旋转，然后绕原 Y 轴，最后绕原 Z 轴。
- **Relative to**——测量零件坐标所相对于的零件。
- **CAD System**——零件最初设计的 CAD 系统名称。可通过零件原型选择六种不同的系统。
- **Material**——零件构造所用物质的名称。
- **Weight**——零件的重量。
- **Thickness**——零件构造材料的规格（gauge）。
- **Size**——表示长、宽、高的数值。
- **Image File**——构成零件图像的文件（若有）的完整路径名。图像显示在 Image File 字段上方。
- **3D File**——此指向 *.cojt 3D 文件的路径可通过零件原型设置。原型将此字段的路径分发给其所有实例。
- **Supplier**——零件供应商名称。
- **Bounding Box**——边界框的 X、Y、Z 参数。

**Product Tree - Cost Tab**

Cost 选项卡仅显示两项：零件的 Cost Group（可编辑）和 Estimated Cost。

- **Cost Group**——指定负责零件预算的制造组的任何字符串。
- **Estimated Cost**——表示零件预估成本的数字值。此值只能从相应原型的 Cost 选项卡更改。

**Product Tree - Manufacturing Features Tab**

Manufacturing Features 选项卡包含一个显示零件制造特征的窗格。制造特征可按如下方式方便地指定给零件：打开所需的制造树窗口，选择单个制造特征，将其签出，并将该特征拖入 Manufacturing Features 选项卡窗格。由于同一制造特征不能包含多次，拖放已在此窗格中列出的特征无效。

此窗口中的字段：**Mfg Feature**（制造特征名称）、**Type**（制造特征类型）、**Comment**（可通过 Mfg Feature Properties 窗口填写）、**no.**（从 1 递增的索引，表示零件中制造特征的数量）、**Editable**（仅当相应 Mfg 指定给所选零件时才包含星号 *；若 Mfg 指定给父节点，此字段为空）、**Show Sub-tree Levels** 组合框（要显示制造特征的零件子树级别；通过下拉列表选择 1 到 5 或 all 级别，或键入所需级别数）。

**Product Tree - Operations Tab**

Operations 选项卡显示连接到与所选零件节点连接的零件的链接操作。该连接派生自操作与零件之间的链接。从操作库（或任何其他树）拖放并放置到产品节点上的任何操作都列在此选项卡下。此选项卡仅显示信息——您无法使用此选项卡关联或取消关联操作。

- **Operation**——操作名称，如在操作树中所示。
- **Type**——操作类型。
- **Allocated time**——执行操作所需的时间，如操作树 - Times 选项卡（编辑操作节点属性）中 Allocated time 字段所指定。

**Product Tree - Resources Tab**

Resources 选项卡包含一个窗格，显示连接到与所选装配节点连接的零件的所有资源。该连接派生自资源与零件之间的链接。从资源库（或任何其他树）拖放并放置到产品节点上的任何资源都列在此选项卡下。此选项卡仅显示信息——您无法使用此选项卡关联或取消关联资源。

- **Resource**——资源名称，如在资源树中所示。
- **Number**——资源的库目录号，如资源树 - General 选项卡（编辑资源节点属性）中所示。此值只能从资源库树更改。
- **Type**——资源类型，如 Type 字段所指定。除非项目中的资源是库资源原型的实例，否则它被赋予与资源相同的名称；这种情况下，类型名称后附加 (i)。

<a id="v3-s84"></a> <!-- p552 -->
### 资源树属性（Resource Tree Properties）

资源树（Resource Trees）的有效节点类型如下：

| 图标 | 名称 | 功能 |
| --- | --- | --- |
| Cell | 工作单元 | 一个工作单元（workcell）。 |
| Compound resource | 复合资源 | 包含一个或多个资源的区域（zone）。 |
| Line | 生产线 | 特定的生产线，例如冲压、涂装、白车身、总装。 |
| Plant | 工厂 | 整个工厂。 |
| PrLine | 生产线过程 | 表示 Line 的资源类型对象。创建时也会创建 Line 类型的操作。 |
| PrPlant | 工厂过程 | 表示 Plant 的资源类型对象。创建时也会创建 Plant 类型的操作。 |
| PrStation | 工位过程 | 表示 Station 的资源类型对象。创建时也会创建 Station 类型的操作。 |
| PrZone | 区域过程 | 表示 Zone 的资源类型对象。创建时也会创建 Zone 类型的操作。 |
| Process Resource | 过程资源 | 资源类型的对象，创建时会导致并行创建资源类型的操作。 |
| Resource Placeholder | 资源占位符 | 空资源。 |
| Station | 工位 | 生产线中的单个工位。 |
| Zone | 区域 | 生产线中的单个区域。 |

<a id="v3-s85"></a> <!-- p553 -->
### 编辑资源节点属性（Editing Resource Node Properties）

单击任何资源节点会打开资源属性（Resource Properties）窗口，显示节点属性。其中部分属性可编辑。以下是属性排列的各个选项卡说明：

**Resource Tree - General Tab**

General 选项卡显示资源的 12 个参数，以及复合资源和区域的 10 个参数。非复合资源的资源还包含 Version 和 Amount 参数。用户可以修改其中三个参数，并可添加注释。其余字段与其他树中显示的相应字段相同。

- **Version**——资源原型的版本号。对于资源树中出现的资源实例，您不能更改此值。
- **Amount**——父复合资源或区域使用的资源原型实例数量；用户可以更改此值。大于 1 的数字等同于在复合资源或区域节点下具有那么多相同实例类型的独立节点。
- **Number of Active Resources**——勾选此框以将资源包含在 Manual Line Balancing 计算中。

**Resource Tree - Physical Tab**

Physical 选项卡显示 15 个参数，其中 8 个可编辑。若零件关联了图像文件，则显示图像作为插图并显示文件名。不能编辑的七个参数字段灰显。除资源特有的字段外，其余字段与其他树中显示的相应字段相同。参见 Product Tree - Physical Tab。复合资源和区域节点省略 Length、Width、Height 和 Image File 参数，但允许用户指定供应商。

两个参数涉及资源的位置和相对位置：**Relative to**（测量资源坐标所相对于的资源）和 **Located at**（资源实例所在的复合资源或区域节点名称；从节点下拉列表中选择）。

**Resource Tree - Cost Tab**

eM-Planner 中的成本规划在此选项卡中执行。它提供复合资源或资源的预估和实际成本信息。顶部 Estimate 部分专门用于预估时间（按小时计）；每小时费率在各类型工作的 Global Parameters 选项卡中定义。Estimate 部分下方是 Actual 部分，专门用于各类工作的实际小时支出。每个部分底部显示总人力工时、成本和 H/W 成本。选项卡底部三个橙色字段显示上述部分中预估与实际数字之间的差值（预估减去实际）。左下角的单选按钮（purchased、designed 等）仅用于信息，不参与计算。Status 区域提供两种模式：**In Progress**（选中时选择相邻下拉列表中的百分比，以实时指示成本方面的资源消耗）和 **Completed**（资源使用完成时，状态字段颜色反映项目结束时的成本状态）。

**复合资源中的 Cost 选项卡（Cost Tab in Compound Resources）**

复合资源的 Properties Cost 选项卡与单个资源的相应选项卡几乎相同。区别在于该选项卡的作用是汇总位于复合资源下的所有单个资源 Cost 选项卡。实际和预估成本数据为复合资源下的所有资源求和。每个字段的总和显示在相应启用字段上方的灰显字段中。启用字段允许输入仅与复合资源相关的预估和实际数据。此选项卡右下角的两个按钮负责将成本明细信息导出到外部源：使用顶部按钮创建 CSV 文件，使用底部按钮执行 MS Excel（若工作站上已安装）并在电子表格上显示明细信息。

**Resource Tree - Performance Tab**

- **Throughput**——生产率（每单位时间）。此字段及其旁边的字段（availability）涉及性能问题。
- **Availability**——反映资源的可用程度。此变量取决于（除其他因素外）它执行的不同操作的数量。

**Resource Tree - Operations Tab**

将操作拖到此选项卡以将它们指定给所选资源。**Calculate** 按钮计算已指定操作的总和。**Cycle time**——特定的资源节拍时间值（在资源节拍时间视图中以绿线表示）。**Synchronize** 按钮：使用过程对象时，Synchronize 将已指定的操作作为过程操作的子项添加。

**Resource Tree - Products Tab**

此选项卡包含指定给所选资源的零件。这些零件仅供查看，不能被删除。此外，无法通过此选项卡指定任何零件。与所选资源的归属通过已指定的操作建立。

**Station Tree - TWP Tab**

TWP 选项卡显示以下列出的参数并简要说明。TWP 选项卡中的参数影响示意图查看器中的时间路径计划（Time Way Plan）层。用户可以编辑部分参数。此选项卡中显示的其余数据继承自区域、生产线和资源。

> **注意**
> 选择区域或资源打开 TWP 选项卡时，并非所有字段都会出现。

- **Production Height**——工位中输送带的高度。
- **Product Orientation**——示意图查看器中工位示意图符号的方向。
- **Pitch**——工作站起点和终点之间的物理距离。
- **Cycle Time**——一个周期开始到下一个周期开始之间的时间间隔。
- **Conveyor Speed**——输送带通过工作站的速度。节距和节拍时间可通过公式 `Conveyor Speed = Station Pitch / Cycle time` 定义输送带速度（连续输送带情况）。您可以通过修改节拍时间和节距来更改输送带速度。
- **Blocking Area - Left Side**——工作站左侧被物理平面元素（如管道、导管、梁等）阻挡的部分。用户可以定义工作站中阻挡区域的起点和终点位置。
- **Location**——工作站参考系的精确位置，通过指定 X、Y、Z 轴以及旋转 X、Y、Z 轴和相对于输送带的工作站角度来定义。Modify 按钮允许更改 X 和 Y 坐标。RZ 值可使用 Angle to Conveyor 组合框更改。

  对工位示意图位置的更改会在线应用，或在示意图查看器中立即可见。若无值则显示零。
- **Station Schematic Symbol**——显示为工位示意图符号的图形文件。

**Station Tree - Part Direction Tab**

Part Direction 选项卡显示以下列出的参数并简要说明：

- **Location on Vehicle**——可在示意图查看器中表示的 14 个物理位置之一。
- **TWP Code**——Location on Vehicle 映射到的 TWP 位置。

有关车辆上物理位置与 TWP 代码之间映射的更多信息，请参见 Schematic Viewer Information 中的 Time Way Plan。

<a id="v3-s86"></a> <!-- p561 -->
### 研究属性（Study properties）

Robcad 和 Locational Studies - Contents 选项卡列出研究中的对象和位置信息。要向 Contents 选项卡添加对象，请将对象从项目树（Project Tree）拖放到 Contents 框中。

**Location Information**：此区域提供所选对象相对于研究参考系的位置信息。参考系可以是同一研究文件夹下的任何对象。

要定义参考系：

**操作步骤**

1. 在 Contents 列表中，选择一个对象。
2. 将您希望作为目标参考系的对象从项目树拖放到 **Relative to** 字段。

   > **注意**
   > 您必须拖放真实对象，而不是对象的快捷方式图标。为此，请将对象快捷方式拖放到图标上。随即出现树窗口，显示源对象。

3. 将对象拖放到 **Relative to** 字段。该对象被定义为研究的参考系。

Robcad 研究的研究树 Contents 选项卡包含两个 Update Object Location 选项：

- **Recalculate the location in the study**——将对象在研究中的位置信息设置为与其绝对位置相同。
- **Update physical location from study**——将对象在研究中的物理位置更新为与其绝对位置相同。

<a id="v3-s87"></a> <!-- p563 -->
## 关系查看器（Relations Viewer）

**关系查看器（Relations Viewer）** 使您能够查看与以下类型对象相关的对象：

- 程序或操作（Programs or operations）
- 资源（resource、tool prototype assignment）
- 产品（part、part prototype assignment、part appearance）
- MFGs
- 装配（Assemblies）
- 机器人程序（Robotic Programs）
- 触发器（Triggers）
- 外观（Appearances，当关系查看器聚焦于操作时）

在相应查看器中单击这些对象中的任何一个，会使它成为关系查看器的根对象。关系查看器与其他查看器同步，并在发生时显示所有更改。

关系查看器显示当前加载的程序或操作中产品、资源、工作装配（working assemblies）和 MFGs 之间的物理关系，并根据 eMServer 中保存的更改更新。

> **注意**
> - 若您加载了程序，必须激活图标才能在关系查看器中查看结果。这是因为关系查看器不支持直接关系（与父程序关联的），而仅支持间接关系（与子操作关联的）。
> - 您可以通过在 Robotic Program Inventory 中双击程序来访问程序的关系查看器。

关系查看器使您能够执行以下操作：

- 查看过程层级
- 查看过程关系，例如：
  - 查看连接到此过程的所有零件
  - 查看连接到此过程的所有 MFGs
  - 查看连接到此过程的所有工具
- 识别特定操作内对象之间的关系
- 添加 / 删除 / 修改连接
- 查看当前操作消耗的所有工作装配
- 识别哪些外观是主要外观（Primary Appearances），如在关系查看器中以下划线指示

关系查看器工具栏包含以下选项：

| 按钮 | 工具 | 说明 |
| --- | --- | --- |
| Highlight | 高亮 | 在查看器（Viewer）中以蓝色高亮与所选操作关联的所有对象。 |
| Display All | 显示全部 | 在查看器（Viewer）中显示关系查看器中的所有对象。 |
| Blank All | 隐藏全部 | 隐藏关系查看器中所有对象的显示。 |
| Show Sub-tree Relations | 显示子树关系 | 展开显示并展示树中所示产品装配下嵌套的所有组件。 |

要打开 Products、Resources 或 MFG 下节点的属性对话框，请右键单击该节点并从上下文菜单中选择 **Properties**。

关系查看器中可用的文件夹取决于其根对象的类型。下表列出每种类型对象可用的文件夹：

| 对象 | 可用文件夹 |
| --- | --- |
| Operations | Products、Part Appearances、IPAs、Resources、MFGs、Swept Volumes、Referenced Operations、Motion Segments |
| Resources | Assigned to Operations、Simulating Operations |
| MFGs | Assigned to Operations、Assigned Parts |
| Parts and Assemblies（IPA） | Assigned to Operations、Simulating Operations、Assigned to MFGs、Static Appearances |
| Robotic Programs | Owning Robot、Volumes |
| Part Appearances（dynamic - 仅线路仿真模式） | Assigned to Operations |
| Part appearances（static - 仅 Robcad 研究） | Origin Part、Scope Operation |
| Tool/Part Prototype Assignments | Assigned to Operations |
| Robots | Assigned to Operations、Simulating Operations、Mounted Tools、Programs、Motion Volumes |
| IPAs | Assigned to Operations、Simulating Operations、Assigned to MFGs、Static Appearances、Origin Operation |
<a id="v3-s88"></a> <!-- p566 -->
## 表视图（Table View）

<a id="v3-s89"></a> <!-- p566 -->
### 使用表视图查看和编辑数据（Viewing and Editing Data with Table View）

**表视图（Table View）** 是编辑和查看数据的便捷工具。它提供大量数据的可配置视图，并支持一次并排查看多个节点的属性。它可以显示所显示节点的属性，以及链接到所显示节点的节点的属性。通过使用此视图，可以轻松地对数据进行排序、筛选、查看和编辑，同时使用常见的电子表格操作。

表视图中包含的数据存储在系统的缓存内存中，从而减少性能问题并提高数据浏览和筛选的速度。

要打开表视图，请选择 **Home** 选项卡 → **Viewers** 组 → **Table View**。

表视图中查看的数据与 Process Designer 树和图形查看器（Graphic Viewer）同步。

> **注意**
> - 您可以自定义右键上下文菜单以提高工作效率。参见 Customize。
> - 您可以打开多个表视图，参见 Opening Multiple Table Views。

<a id="v3-s90"></a> <!-- p567 -->
### 表视图工具栏（Table View Toolbar）

表视图工具栏包含以下主要选项（已精简为关键功能）：

- **Reload（重新加载）**——重新加载当前表视图；也可在导航树中选择不同项并加载以替换当前项。
- **Node Level（节点级别）**——选择要在表视图中显示的节点级别数。
- **Zoom In / Zoom Out / Reset Zoom（放大 / 缩小 / 重置缩放）**——图形化聚焦视图；Reset Zoom 仅在使用了放大或缩小后激活。
- **Replace（替换）**——显示 Replace 对话框以执行查找和替换。
- **Substring Filter(*)（子串筛选）**——打开筛选对话框；输入字符串后仅显示所选列中包含该字符串的行（最多三个字符串），再次按下取消筛选。参见 Filtering Data in Table View。
- **Sort（排序）**——打开排序对话框，最多可选择三种排序方法，按输入顺序应用。参见 Sorting Data in Table View。
- **Queries（查询）**——打开查询列表；选择查询后，查询定义应用到当前表视图配置。在 Query Wizard 中创建的 Semantic 和 SQL 查询出现在表视图查询菜单中。
- **Refresh（刷新）**——用新添加到树中的对象更新表视图。
- **Print（打印）/ Print Preview / Print Setup**——按打印设置定义打印电子表格。
- **Export to Excel（导出到 Excel）**——将表导出为 MS Excel 格式（*.xls）。
- **Configuration list（配置列表）**——打开先前定义的配置列表，并可定义新配置、编辑或删除现有配置。
- **Hide/Show Rows（隐藏/显示行）**与 **Hide/Show Columns（隐藏/显示列）**——为查看方便隐藏或重新显示所选行/列（列的新设置保存在当前表视图配置中）。
- **Freeze Rows / Freeze Columns（冻结行/列）**——滚动时保持所选行/列可见（列的新顺序保存在当前表视图配置中）。
- **Conditional Format Columns（条件格式列）**——根据条件设置列中单元格的背景色，参见 Table View Display。
- **Select Configuration（选择配置）**——为表视图选择配置，参见 Viewing a Different Configuration。

> **注意**
> Substring Filter 按钮仅在选择了单列时启用。

<a id="v3-s91"></a> <!-- p569 -->
### 打开多个表视图（Opening Multiple Table Views）

您可以同时打开不同项的多个视图。这有助于比较大型结构。

要打开表视图：

- 在导航树中选择一项并执行以下任一操作：
  - 选择 **Home** 选项卡 → **Viewers** 组 → **Table View**。

  > **注意**
  > 这不是表视图工具栏中的 Reload 图标。

  - 右键单击所选对象并选择 **Table View**。

要打开更多表视图：

- 打开第一个表视图后，在导航树中进行新的选择并重复上述过程。

> **注意**
> 若您在打开多个表视图的情况下退出 Process Designer，下次启动表视图时，显示会包含相同数量的（空）表视图。这种情况下，只有在导航树中进行选择后，Reload 按钮才会激活。

<a id="v3-s92"></a> <!-- p571 -->
### 创建新的表视图配置（Creating a New Table View Configuration）

首次在特定节点上打开表视图时，主窗口出现。要定义新配置，请单击表视图工具栏上的 **Configuration List** 按钮，随即显示 Configuration List 对话框。

您可以通过从列表中选择当前配置并单击图标来编辑它，随即打开 **Field Selector** 对话框，使您能够修改表视图字段。完成必要字段的调整后，在 Field Selector 对话框中单击 **OK**，然后在 Configuration List 对话框中单击 **Close**。您所做的适用于当前显示的所有更改都会针对所选配置实施。

您可以打开表视图以分析先前打开的节点，并在组合框中查看该节点的所有配置。为避免不必要地加载大量信息，数据仅在您从列表中选择所需配置后才加载。这使您可以选择关闭表视图或加载特定配置。

**导入和导出配置（Importing and Exporting Configurations）**

导入的配置和用户通过其名称标识。因此配置必须具有唯一名称。

- 单击 **Export** 将所选配置导出到 XML 文件。若未做选择，则导出当前 schema 中的所有配置。
- 单击 **Import** 从指定的 XML 文件导入新配置（不考虑 Configuration List 对话框中的当前选择）。当前 schema 中已存在的配置会被覆盖，不存在的配置作为新配置导入。若配置名称（对于给定用户）在 schema 或文件中多次出现，则无法导入并显示失败消息。

下表根据用户的权限描述导入和导出能力：

| 权限 | 导出 | 导入 |
| --- | --- | --- |
| Create Configuration | 当前用户的私有配置 | 当前用户的私有配置 |
| Create Configuration + Application Administration | 当前用户的私有配置 | 所有用户的私有配置 |
| Create Configuration + Publish Configuration + Application Administration | 所选私有和（或）公有配置，或 schema 中的所有配置 | XML 中的所有配置 |

导入注意事项：

- 当前 schema 中不存在的用户的配置不会被导入（不发送通知）。
- 当前 schema 中不存在的类在导入期间被忽略（不发送通知）。
- 私有用户视图（private user views）是用户在表视图中选择公有配置时创建的配置。它们仅包含表视图列自定义。类和字段定义包含在相应的公有配置中。
- 公有配置的私有用户视图仅在公有配置也被导入/导出时才导入/导出。

**使用字段选择器（Using the Field Selector）**

字段选择器（Field Selector）提供直观的树视图以选择将包含在配置中的字段。您可以选择对象、属性值，以及链接对象及其属性值。

要选择任何对象或属性值：在 Available Fields 窗格（左侧）中导航到树的相应分支，选择所需项，并单击图标。所选项被添加到 Selected Fields 窗格（右侧）。

> **注意**
> 您可以在字段选择器中查看并选择用法（usage）属性。

接受更改后，表视图显示用法及其属性。您可以像其他任何属性一样在表视图中更改用法属性。

要点：

- 由于表视图可以显示对象和链接对象的属性，多个字段可能以同一属性命名。例如，属性 `allocatedTime` 在 Selected Fields 窗格中出现两次：一次用于父操作，一次用于子操作。
- 为防止意外的配置及随之对数据库造成的损坏，`FieldSelectorDialogConfiguration.xml` 文件位于 `C:\Program Files\Tecnomatix\eMPower\eM-Planner\`。该文件阻止编辑所有已知字段——您可以移除希望显示和/或编辑的字段。

<a id="v3-s93"></a> <!-- p576 -->
### 表视图显示（Table View Display）

定义新表视图配置或选择现有配置后，表视图打开，支持数据查看和修改。显示的列以颜色编码：白色列可通过表视图编辑；深灰色列仅供参考，无法通过此视图修改。

类似 Excel 的功能允许您将值从一个单元格复制到多个其他单元格：

**操作步骤**

1. 单击包含要复制的值的单元格右下角的小方块。
2. 向下拖动到要复制值的其他单元格上。

或者，您可以通过复制所选单元格，然后选择一个或多个单元格并从键盘按 `Ctrl + v`，复制到同一列中的多个单元格。

与 Excel 一样，您可以执行以下操作：

- 通过向上或向下拖动行的下边框来收缩或展开行。
- 将列拖动到新位置。新的列顺序保存在当前表视图配置中。
- 条件格式列。

从表视图工具栏单击图标（当前配置为公有时不可用）。随即显示 Column Conditional Formatting 对话框。对话框的内容根据所选列中的单元格是格式化为字符串、日期还是数字而略有不同。单击图标并从出现的调色板中选择背景单元格颜色。从 Cell value 下拉列表中选择运算符并在文本框中输入值。您所做的更改保存在当前配置中。

**表视图中链接对象的属性（Attributes of Linked Objects in Table View）**

当链接对象的属性显示在表视图中时，每行可能有不止一行相关字段。当通过向上拖动下边框收缩行时，滚动条会指示当前隐藏的额外相关字段行。当行以此方式收缩并滚动相关字段时，显示同一链接属性属性的所有其他单元格会同步滚动。链接属性的列标题格式为："Link Attribute: Attribute of Linked Object"。

<a id="v3-s94"></a> <!-- p579 -->
### 在表视图中对数据排序（Sorting Data in Table View）

Sort 对话框使您可以选择最多三种对表视图中数据排序的条件。排序条件按您输入的顺序应用。例如，若先按名称排序，再按 allocatedTime 排序，表视图将按名称的字母数字顺序排序，当表中两行名称相同时，按它们的 allocatedTime 排序。对于每个搜索条件，您可以选择升序（"a, b, c,..."）或降序（"z, y, x,..."）。

要对数据排序：

**操作步骤**

1. 通过单击工具栏上的图标打开 Sort 对话框。
2. 从"Sort by"组合框中选择排序方法。
3. 通过选择 Ascending（默认）或 Descending 单选按钮定义显示顺序。
4. 使用"Then by"组合框和单选按钮定义最多两种更多排序方法。
5. 单击 **OK**。

<a id="v3-s95"></a> <!-- p581 -->
### 在表视图中筛选数据（Filtering Data in Table View）

您可以使用 Filter 对话框筛选表视图中显示的数据。对于每个筛选器，您选择一个属性及其值。例如：要仅查看名称包含子串"CompoundOperation"的节点，请从 Attributes 组合框中选择 name，并在 Value 字段中键入"CompoundOperation"。所有名称中某处包含子串"CompoundOperation"的节点（例如"CompoundOperation"、"123CompoundOperation"和"CompoundOperation123"）都会显示。

Filter 对话框允许您选择最多三种筛选器。所有筛选器同等应用；输入顺序无关紧要。

要筛选数据：

**操作步骤**

1. 通过单击工具栏上的图标打开 Filter 对话框。
2. 从"Filter by"Attribute 组合框中选择属性（例如 name）。
3. 在"Filter by"Value 字段中键入属性的子串值。
4. 使用"Then by"组合框和字段定义最多两种更多筛选器。
5. 单击 **OK**。

<a id="v3-s96"></a> <!-- p582 -->
### 查看不同的配置（Viewing a Different Configuration）

当定义了多个配置时，您可以使用 Configuration 组合框在配置之间切换。

<a id="v3-s97"></a> <!-- p583 -->
### 表视图菜单（Table View Menu）

单击表视图工具栏按钮以显示包含表视图命令的菜单：

- **Calculate Sum**——对列中单元格包含的数值求和，并在列底部的单元格中显示总和。
- **Edit Column Properties**——打开 Column Properties 对话框（当前配置为公有时不可用）。该对话框使您能够编辑所选列的 Header 并将其配置为只读（Read only）。只读列在表视图中以灰色阴影显示。若选择了多列，Header 字段变为非活动，但您仍可以配置只读状态。您可以使用 Column Properties 对话框通过选择一个或多个只读列并取消勾选只读复选框来移除只读状态。
- **Print**——将当前表视图发送打印。
- **Print Preview**——显示电子表格打印布局以便打印前参考。
- **Print Setup**——打开 Print Setup 窗口，选择已安装打印机并定义纸张大小和方向。
- **Page Setup**——参见 Printing in Table View。
- **Add Vector Row***——在向量变量单元格中添加一行。
- **Delete Vector Row***——从向量变量单元格中移除所选行。

> **注意**
> 带 * 的按钮仅在选择值向量字段（例如 'Collections'、'Attachment' 或 'Children'）时启用。

您所做的更改保存在当前配置中。

**修改表显示（Modifying the Table Display）**

您可以通过拖动标题边框来修改行高和列宽：拖动行的下边框，拖动列的右边框。若选择了多行或多列然后拖动以修改单元格尺寸，更改会反映在所有所选项目中。要重新排列表中的列和行，请选择并将它们拖动到新位置。拖动所选列或行时，垂直或水平线会高亮显示新位置。

<a id="v3-s98"></a> <!-- p584 -->
### 在表视图中打印（Printing in Table View）

表视图使您能够广泛控制表格的打印外观和排列。打印前，使用通过表视图菜单访问的命令执行以下步骤：使用 Page Setup 选项设计打印输出（设置边距、标题和网格线、页面顺序和表格居中）。

**操作步骤**

1. 选择 **Print Setup** 以选择已安装打印机并定义纸张大小和方向。
2. 使用 **Print Preview** 在打印前查看电子表格打印布局。

若查看布局后希望进行修改，请重复上述操作。

**Page Setup**

使用此窗口定义在打印前影响表视图外观的各种参数。这些参数不会改变表视图的电子显示——它们仅修改打印的硬拷贝布局。

- **Preview 框架**模拟打印页面上的页面布局。更改定义时使用此显示作为参考。
- **Margins 设置**——定义表格周围空白边距的大小——上、下、左、右。尺寸以英寸显示，但您也可以通过键入 'cm' 输入厘米值。您在此处输入的修改不会反映在 Preview 框架中。
- **Titles and Gridlines 框架**包含切换表格功能显示的复选框。除 'Print Frame' 外，所有功能都影响 Preview 框架显示。
- **Page Order**——当表格跨多页时确定打印优先级。
- **Center on Page**——垂直、水平或两者居中对齐表格。
- **Save settings to profile**——若希望将页面设置定义与当前表视图配置一起保存，请勾选此框。

<a id="v3-s99"></a> <!-- p586 -->
### 在表视图中导航和编辑（Navigating and Editing in Table View）

在此视图中，您可以使用常见的电子表格操作轻松地对数据进行排序、筛选、查看和编辑。

**在表视图中编辑数据（Editing Data in Table View）**

要编辑单元格中的数据，请单击该单元格并输入新值。所有更改会自动反映在系统中（在树中）。

在某些情况下（例如"status"），提供组合框以从可用选项列表中选择。

关系数据（例如 children、parents）以链接形式呈现，每个链接指向其相应的属性（Properties）窗口。您可以使用拖放修改对象层级。

**在表视图中刷新数据（Refreshing Data in Table View）**

当表视图打开时向树中添加新对象，您可以通过单击工具栏上的 **Refresh** 按钮自动更新表视图。
<a id="v3-s100"></a> <!-- p587 -->
## PERT 查看器（PERT Viewer）

<a id="v3-s101"></a> <!-- p587 -->
### 查看操作逻辑图（Viewing Operation Logic Diagrams）

**PERT 查看器（PERT viewer）** 提供强大的图形工具，用于检查操作的逻辑图及其依赖关系。PERT 框显示所有相关的操作信息，包括与资源、零件和 Mfgs 的链接。

要访问 PERT 查看器，请选择 **Home** 选项卡 → **Viewers** 组 → **Pert Viewer**。

<a id="v3-s102"></a> <!-- p588 -->
### 术语/定义（Glossary/Definitions）

- **Buffer（缓冲）**——类型为 PmSource（source）或 PmSink（sink）的操作。
- **Buffer flow（缓冲流）**——连接操作与缓冲的标准流。
- **Chart（图表）**——PERT 的图形窗口。
- **Chart root（图表根）**——其子项显示在 PERT 图中的对象。
- **Connected flow（连接流）**——连接两个操作、且两个操作都链接到它的流。
- **Disconnected flow（断开流）**——仅连接一个操作、且只有一个操作链接到它的流。
- **Interface（接口）**——现有数据库对象的 UI 表示。此对象是复合操作图表根。
- **Interface flow（接口流）**——连接操作与其复合操作父项的 scope 流，其中 scope 是父项。
- **Offsite flow（站外流）**——scope 为 NULL（即无 scope）的 scope 流。它连接具有不同父操作的两个操作，或仅链接到其中一个。
- **Offsite operation（站外操作）**——来自另一个层级、非缓冲的操作，其父操作不是图表根操作，但仍显示在图中。站外操作通过 scope 为 NULL 的 scope 流（站外流）连接到图表根子项（站点操作）。
- **Operation route（操作路线）**——派生自 PmRoute（其派生自 PmGroup）的对象。
- **PERT chart**——图表的同义词。
- **PERT chart root**——图表根的同义词。
- **PERT root**——文档根。打开 PERT 的操作或路线。
- **Root**——PERT root 的同义词。
- **Route**——操作路线对象的同义词。
- **Scope flow（范围流）**——类型为 PmScopeFlow、相对于给定 scope 连接操作的流。在 PERT 中，scope 是图表根——路线（通用 scope 流）或父操作（接口流）——或 NULL（站外流）。
- **Site operation（站点操作）**——图表根的子项操作，因此自动显示在图中。
- **Standard flow（标准流）**——类型为 PmFlow 的流。标准流没有 scope——它们连接复合操作（或路线）的子项。

<a id="v3-s103"></a> <!-- p589 -->
### 打开 PERT 查看器图表（Opening a PERT Viewer Chart）

为复合操作、过程（process）或过程资源（process resource）打开 PERT 查看器图表。

要打开 PERT 查看器图表：

- 选择 **Home** 选项卡 → **Viewers** 组 → **Pert Viewer**。

操作的选择在 PERT 查看器与 Process Designer 树和图形查看器之间同步。

> **注意**
> 您可以自定义右键菜单以提高工作效率，如 Customize 中所述。

**打开 PERT 查看器的多个实例（Opening More than One Instance of the PERT Viewer）**

您可以通过单击 PERT 查看器图标，或右键单击复合操作或过程并从上下文菜单中选择 **Open With PERT viewer**，来打开 PERT 查看器的多个实例。若未做选择，PERT 查看器打开时不加载任何信息。要加载信息，请在导航树中选择一个对象并单击工具栏上的 Load 图标。

> **注意**
> Load 图标仅在您从导航树中选择合法对象时才激活。

每次单击 PERT 查看器图标，Process Designer 都会加载查看器的新实例。一种布局中最多可打开五个查看器。

<a id="v3-s104"></a> <!-- p591 -->
### PERT 图基础（PERT Chart Basics）

PERT 图包含 PERT 查看器和工具栏命令图标。PERT 查看器工具栏包含以下主要选项（已精简）：

- **Reload PERT**——重新加载所选节点的 PERT 图。
- **Drill To**——显示所选节点的 PERT 图（选择 PERT 图形显示中的节点以激活，参见 Adding Interfaces）。
- **Drill Up / Drill Down**——在层级中将 PERT 图上移/下移一级（在顶级/底层时停用；选择操作框以激活）。
- **New Node**——插入新的操作对象（参见 Adding Operations in the PERT Chart）。
- **New Flow**——在两个操作之间插入流符号（参见 Adding Flows between Operations）。
- **Group / Ungroup**——将两个或多个操作组合为单个复合操作框，或将分组的复合操作框拆分为单独操作（参见 Grouping and Un-grouping Operations）。
- **Add/Remove Operations From Other Hierarchies**——从其他层级添加/移除操作到 PERT 图（参见相应章节）。
- **Select / Pan / Zoom / Zoom To Fit / Zoom To Selection**——选择并拖动图表元素 / 平移 / 缩放 / 适应 / 缩放到选择。
- **Display/Hide Buffer Operations**——控制 PERT 图形视图中缓冲操作的显示（默认显示）。
- **Show/Hide Resources / Show/Hide Parts**——控制资源/零件在 PERT 图形视图中的显示。
- **Capture View**——拍摄 PERT 图视图的快照并保存为操作的附件。
- **Reset Size**——将操作框恢复为默认大小（当框被误拖大时）。
- **Layout Display**——将 PERT 视图中操作的布局定义为以下显示类型之一：Portrait、Landscape、Snake。
- **Save Layout Information**——即使在签入之前，也将节点的当前 PERT 布局信息更新到数据库。
- **Align Top/Middle/Bottom/Left/Center/Right**——将两个或多个操作对齐到最后一个所选操作的上/中/下/左/中/右。
- **Toggle Grid**——通过连续单击显示或隐藏背景网格。
- **More**——打开带有附加 PERT Background and Menu 命令的菜单。

**操纵 PERT 图（Manipulating the PERT Chart）**

您可以使用四种操作模式操纵图表，每种模式由不同的光标形状表示：

- **Select（选择）**：主要操作模式。用于选择图表元素并将其移动到不同位置。
- **Pan（平移）**：更改焦点图形视点（滚动条的替代方式）。
- **Zoom（缩放）**：将图形视图放大或缩小。
- **Link（链接）**：在操作之间创建流（参见 Operation PERT Flows 中的 Adding Flows between Operations）。

这四种操作模式之一始终在 PERT 查看器中处于活动状态。选择不同的操作模式会停用当前模式。

使用 PERT 查看器工具栏上的 **Drill Down** 和 **Drill Up** 按钮分别图形化显示子节点和父节点。在左框或右框中选择一个操作，并单击 **Drill To** 将 PERT 图视图设置为所选操作。

**使用缩放选项（Using the Zoom Option）**

单击 PERT 查看器工具栏上的图标时，光标在右 PERT 框上移动时变为放大镜。您可以通过以下几种方式操纵图形视图：

- 在图表区域左键单击：视图放大并以光标位置重新居中。
- 在图表区域右键单击：视图缩小并以光标位置重新居中。
- 按住鼠标左键拖动矩形以定义特定区域：图形视图放大到该定义区域，使其现在填满视图框。

Select 模式下还可使用以下缩放选项：

- 选择一个或多个 PERT 图元素并单击图标。图形视图缩放到所选元素。
- 单击图标自动调整图形视图以包含所有当前图表元素。

单击 PERT 工具栏上的 PERT 菜单按钮可获得其他缩放预设，参见 PERT Background and Menu。

<a id="v3-s105"></a> <!-- p596 -->
### PERT 图元素（PERT Chart Elements）

本节描述操作 PERT 构建块，它们说明复合操作中操作的顺序。这些图形元素排列在图表网格上，以直观表示顺序的顺序、层级和依赖性。

| 图形 | 标题 | 说明 |
| --- | --- | --- |
| Operation Box | 操作框 | 操作节点的图形表示。包含框中三行排列的操作信息：顶行——操作的名称和类型（单个或复合）；中行——操作的持续时间或时间；底行——分配给操作的资源和 Mfgs。Mfg 数量显示在 Mfg 图标旁边。 |
| Source | 源 | 该零件由非操作（例如货架、桌子、零件仓库）的源交付。 |
| Sink | 汇 | 该零件交付到非操作（例如货架、桌子、零件仓库）的源。 |
| Interface | 接口 | 接口是零件进出复合操作的网关。在 PERT 图形显示中，复合操作表示为单个框，但构成复合操作的子操作中只有一个消费或生产该零件。因此，规划人员必须将其中一个子操作标识为零件的"接口"——实际消费或生产该操作的操作。 |
| Flows | 流 | 定义 PERT 图中操作、源、汇和接口之间的依赖关系和顺序的向量。流显示被操作消费或生产的零件。 |
| Grid | 网格 | PERT 工作区。网格便于对齐 PERT 图中的对象，可显示或隐藏。 |
| Folder | 文件夹 | 若连接到流的零件超过三个，PERT 在流上显示文件夹而不是显示所有零件。悬停此图标会显示包含零件数量的工具提示。另见 Merge Sources。 |

<a id="v3-s106"></a> <!-- p597 -->
### 从 PERT 图访问属性（Accessing Properties from the PERT Chart）

通过在 PERT 查看器中右键单击操作来打开属性（Properties）窗口。

<a id="v3-s107"></a> <!-- p598 -->
### 在 PERT 图中添加操作（Adding Operations in the PERT Chart）

PERT 图中的操作框是操作树中相应操作节点的图形表示。当您在 PERT 中添加新操作框时，会在操作树中自动创建新操作节点。

要添加新操作：

**操作步骤**

1. 单击 PERT 查看器工具栏上的图标。随即打开 **New Node** 上下文菜单，其中包含可添加到 PERT 图的各种操作。注意 Source、Sink 和 Interface 也被视为操作类型。当 PERT 图中不显示 Source 和 Sink 时，它们也不会显示在菜单中。
2. 从菜单中选择操作类型，并通过单击 PERT 图形框中的空白区域来指定其位置。

> **注意**
> 当 PERT 图显示缓冲操作时，New Node 上下文菜单使您能够添加缓冲操作。当不显示缓冲操作时，此选项不可用。

或者，您可以使用以下步骤添加操作：

**操作步骤**

1. 在 PERT 图形框的空白区域右键单击，并从打开的上下文菜单中选择 **New**。随即显示标准 New 对话框。
2. 使用对话框选择节点类型和数量。

若您误将操作框拉伸为不同大小，可以单击 PERT 工具栏上的图标自动将其恢复为标准大小。

<a id="v3-s108"></a> <!-- p599 -->
### PERT 图表根中的站外操作和站外流（Offsite Operations and Offsite Flows in PERT Chart Roots）

PERT 复合操作图表根可以显示在具有不同父操作的操作之间的站外流（scope 为 NULL）。

若同时满足以下两个条件，则显示来自另一个层级的操作：

- 该操作是站外操作。
- 图表根是复合操作。

连接到图表根子项的站外操作始终与相应的站外流一起显示。断开连接的站外操作仅在 PERT 中可见但保存 PERT 布局时断开时才显示。

连接到站外操作的流在满足以下任一条件时显示：

- 该流是站外流并连接到图表根子项。
- 该流是站外流且没有其他连接（除站外操作外）。

所有其他与站外操作相关的流都将被忽略。

连接到两个操作（图表根子项和站外操作）的站外流始终显示。连接到图表根子项但与站外操作断开的站外流，在选中 **Show disconnected flows** 选项时显示，清除该选项时不显示。同样，连接到站外操作但与图表根子项断开的站外流，在选中该选项时显示，清除时不显示。因此，这些站外流可以再次重新连接到图表根子项。

站外操作以绿色高亮，附加了至少一个 scope 流的操作以蓝色高亮。

> **注意**
> PERT 操作路线图表根不支持站外流或站外操作。路线的子操作通常源自操作树，并且可以同时是多个路线的子项，因此这些操作相对于操作树来自不同层级。

<a id="v3-s109"></a> <!-- p600 -->
### 向图表添加/从图表移除来自其他层级的操作（Adding/Removing Operations from Other Hierarchies to/from the Chart）

要向 PERT 图添加来自其他层级的操作：

> **注意**
> 通过从其他层级向 PERT 图添加操作，您并不是创建新操作，而只是将现有 eMServer 对象（操作）的表示添加到图中。

**操作步骤**

1. 在导航树中选择一个或多个操作，这些操作位于 PERT 根下方的某处。
2. 单击图标。来自步骤 1 的、可添加所选操作将被添加到 PERT 图。

> **注意**
> - 可添加操作是来自其他层级、既不是 PERT 图元素也不等于图表根的操作。此外，可添加操作必须是复合操作的子项。
> - 若选择包含任何无效对象，它们将被忽略，仅添加有效操作。
> - 若选择中没有有效节点，将显示错误消息。

要从 PERT 图移除来自其他层级的操作：

> **注意**
> 通过从 PERT 图移除来自其他层级的操作，您并不是删除该操作，而只是从图中移除现有 eMServer 对象的表示。

**操作步骤**

1. 在图表中选择一个或多个操作。
2. 单击图标。那些可移除的所选操作将从 PERT 图中移除。

> **注意**
> - 可移除操作是没有连接到图表根子项的连接（站外流）的站外操作。
> - 若选择包含不可移除操作的节点，它们将被忽略，仅处理可移除操作。
> - 若选择没有任何可移除操作，将显示错误消息。
> - 移除站外流时，相应的站外操作不会随流一起从图中移除。

<a id="v3-s110"></a> <!-- p602 -->
### 使用站外操作和站外流（Working with Offsite Operations and Offsite Flows）

**连接、断开、删除（Connecting, Disconnecting, Deleting）**

一旦站外操作被添加到 PERT 图，它可以像图中显示的任何其他操作一样被连接、断开和重新连接。

允许将站外操作连接到非缓冲的图表根子项。但是，不允许将站外操作连接到另一个站外操作或图表根本身；在这两种情况下，连接命令都不会执行。

与流一样，站外流可以从 PERT 图内部（或从外部使用甘特图）删除。它们不需要特殊处理。

**移动到站外操作层级（Moving to Offsite Operation Level）**

一旦站外操作显示在图中，您可以轻松地从当前层级级别移动到站外操作层级，如下所示：

**操作步骤**

1. 选择站外操作。
2. 单击 **Drill To** 按钮。站外操作现在是图表根。
3. 单击 **Drill Up** 按钮。站外操作的 UI 父项是图表根。

> **注意**
> 若站外操作不是（PERT 文档）根子树的一部分，则无法 drill 到站外操作并向上到其父操作，或向下 drill 到其子项。不过，您可以选择站外操作的父项打开另一个 PERT 应用。

**钻取（Drilling）**

向上钻取（使用 Drill Up 按钮）与选择无关，只要当前图表根有 UI 父项就启用。向下钻取（使用 Drill Down 按钮）对站外操作禁用。要查看子项，请选择站外操作并单击 **Drill To**；站外操作现在是图表根。

<a id="v3-s111"></a> <!-- p603 -->
### 分组和取消分组操作（Grouping and Un-grouping Operations）

**分组（Group）**——将元素组合为集体元素：

**操作步骤**

1. 在单击框时按住键盘上的 Control 或 Shift 按钮，选择两个或多个操作框。或者，您可以在要选择的操框周围拖动选择矩形。您也可以通过按住 Control 或 Shift 单击操作，从树中选择多个操作。
2. 单击 PERT 查看器工具栏上的图标。从 New 对话框中出现的列表中选择组类型。显示的列表类型根据您选择用于分组的对象而定。
3. 选择所需的对象类型。尽管窗口类似于标准 New 对话框，但您不能使用"Amount"列创建多个节点。单击 **OK** 确认选择。新的集体对象替换操作树中的分组项，分组项移到其下方。

**取消分组（Ungroup）**——将分组的复合操作拆分为单个元素：

**操作步骤**

1. 从右框中选择表示操作组的框，或从左框中选择其相应的树节点。
2. 单击 PERT 查看器工具栏上的图标，或右键单击并从上下文菜单中选择"Ungroup Operations"。

所选集体对象被擦除，最初构成组的各个操作在操作树中占据层级位置。各个操作框部分彼此重叠。通过将其各自拖到网格上的位置来分离它们。

<a id="v3-s112"></a> <!-- p604 -->
### 操作 PERT 流（Operation PERT Flows）

流在 PERT 图中由箭头表示，反映操作之间的依赖关系。在以下示例中，"Get Pillar"操作必须位于"Weld Robot"操作之前，而后者必须位于"Remove Button"操作之前。

PERT 图中的流概念与甘特图（Gantt Viewer）中甘特资源查看器（GanttResourceViewer）概念的 Direct Links 相同。

**PERT 布局设置（PERT Layout Settings）**

PERT 允许您将操作的布局定义为以下显示类型之一：Portrait、Landscape、Snake。

要修改 Pert 显示的布局：

**操作步骤**

1. 通过单击图标打开下拉列表。
2. 选择视图。显示相应更改。您也可以选择单个图标并指定其布局。

要修改 Pert 显示的一部分：

**操作步骤**

1. 选择图标。
2. 选择布局视图。
3. 单击要插入图标的位置。图标移动到该区域，呈指定形状。

**PERT 布局选项（PERT Layout Options）**

要自定义 Pert 布局：

**操作步骤**

1. 单击 PERT 菜单按钮打开以下菜单。
2. 选择 **Layout Settings** 选项。随即打开 Pert View Option 对话框。
3. 根据需要修改选项（如下所述）。
4. 单击 **Apply**，然后单击 **OK**。

| 名称 | 功能 |
| --- | --- |
| Use system settings | 使用 Pert 布局的默认设置。 |
| Define settings manually | 允许您定义设置。 |
| Distance between operations | 指定操作之间的垂直和水平距离（以网格单位计）。 |
| Avoid Overlapping | 防止操作重叠，无论操作之间的距离如何。 |
| Operations' Source and Sink locating method | 源和汇的定位方法：Above、Under、Around。 |
| Distance to operation (grid unit) | 源和汇对象之间的实际距离。 |
| Snake Layout operations per line | Best fit to Pert window——Pert 确定每行的操作数；Number per line——固定每行的操作数，不自动优化视图。 |

<a id="v3-s113"></a> <!-- p608 -->
### 在操作之间添加流（Adding Flows between Operations）

**操作步骤**

1. 单击操作 PERT 工具栏中的 **Flow** 按钮。当您将鼠标移到右框上时，光标变为十字指针。
2. 单击作为流源的操作框。出现一条虚线，一端固定在源操作框上，另一端连接到光标。当您将光标移到目标操作时，此线使您能够轻松验证流是否正确。
3. 将光标移到流的目标操作；虚线指示新流。
4. 单击目标操作框以确认新流位置。连接由从源操作发出并指向目标操作的黑色箭头指示。

您可以继续在操作之间创建其他流，直到选择不同模式（Select、Zoom 或 Pan）。

若您重新定位链接的操作框，流箭头会相应调整。若流的任一端远离操作框，箭头将变为红色以指示流未连接。将未连接的一端移到操作框会恢复线条的黑色。

> **注意**
> 连接操作时，PERT 验证新流不会创建循环。若添加流导致循环，系统返回错误消息。

<a id="v3-s114"></a> <!-- p608 -->
### 编辑流属性（Editing Flow Properties）

要打开流的属性（Properties）窗口，请右键单击流并从打开的上下文菜单中选择 **Properties**。

此窗口在属性对话框中以四个选项卡显示流属性：General、Flow、Attachments 和 Attributes。Flow 选项卡用于定义两个操作之间的连接。它显示四个参数——其中两个可由用户修改。

- **Predecessor、Successor**：分别是流的源和目标操作的名称。虽然不能从此选项卡更改，但若从 PERT 图修改流布局，它们会动态更新。您可以将 Predecessor 和/或 Successor 对象拖放到任何查看器中的沙漏符号上以找到该对象。
- **Delay**：前驱操作完成与后继操作开始之间的时间（秒）。默认值 0 表示后继操作在前驱操作完成后立即开始。若为此变量分配有效值（大于零的整数），该值显示在 PERT 图中相应流箭头下方。
- **Part**：流中操作之间使用的零件和零件号列表。要向流添加零件和复合零件，请将它们的节点从树拖放并放置到操作 PERT 图中的流箭头上。您也可以直接将零件拖放到操作上。

<a id="v3-s115"></a> <!-- p610 -->
### 删除操作框和流（Deleting Operation Boxes and Flows）

删除 PERT 图形显示中的操作和流也会将它们从操作树中移除。

要删除操作或流：

**操作步骤**

1. 确保所显示的操作框属于已签出的节点，并选择要删除的操作框或流。执行以下任一操作：
   - 右键单击对象并从打开的上下文菜单中选择 **Delete**。
   - 按键盘 **Delete** 键。
   系统提示您确认删除——单击 **Ok**。
2. 删除操作框后，流箭头悬空（并变为红色）。删除悬空的流箭头。

<a id="v3-s116"></a> <!-- p611 -->
### 添加接口（Adding Interfaces）

接口是 PERT 图形显示中的图形对象，充当零件进入（被消费）和离开（产生）复合操作的网关。接口对象标识复合操作中消费或生产该零件的单个操作。接口表示图中显示的操作的父操作。

接口有两种类型：

- **Interface in**——用于消费传入零件的操作。
- **Interface out**——用于生产传出零件的操作。

要向复合操作添加接口：

**操作步骤**

1. 向下钻取（Drill down）复合操作以显示其子操作。
2. 单击 PERT 查看器工具栏上的图标，并从打开的下拉菜单中选择"Interface"。
3. 单击图表区域中的空白点以指定新接口的位置。
4. 将流从接口图标连接到操作框以添加"interface in"；或将流从操作框连接到接口图标以添加"interface out"。

<a id="v3-s117"></a> <!-- p612 -->
### 分配资源、零件和 Mfgs（Assigning Resources, Parts and Mfgs）

**在 PERT 中分配资源（Assigning Resources in PERT）**

您可以在 PERT 图中向操作分配资源（无法向 twin 对象分配资源）。拖放所需资源并将其放置到将使用此资源的操作框上。资源以图标形式出现在操作框的底行，您可以将它们从一个操作拖到另一个操作。添加的资源也出现在操作的 Resource 选项卡中。一旦进入操作框，您可以通过将光标置于图标上方读取工具提示来识别每个资源。

若向同一操作框添加超过四个资源，系统显示单个文件夹图标来表示它们。资源图标的显示可通过单击操作 PERT 工具栏上的图标来切换开/关。

要查看文件夹中的资源列表：

**操作步骤**

1. 选择包含资源文件夹的操作框。
2. 右键单击操作框并选择 **Properties**。随即打开属性窗口。
3. 单击 **Resources** 选项卡。
4. 右键单击资源并选择 **Properties** 以查看其属性。

右键菜单还包含允许您从文件夹中删除资源、签入/签出以及取消签出的命令。

若与操作关联的资源数量减少到四个或更少，文件夹图标将替换为单独的资源配置图标。

**在操作流之间移动零件和装配（Moving Part and Assemblies between Operation Flows）**

您可以将 PERT 图中一个流的多元件文件夹中包含的零件或装配拖到另一个流。

**操作步骤**

1. 右键单击包含多零件或装配的文件夹（Process Designer 会自动在流上为四个或更多对象创建文件夹）以打开属性窗口。
2. 从打开的对话框中，选择并拖动一个或多个对象，并将其放置在将使用这些零件或装配的操作流上。

**在 PERT 中分配零件（Assigning Parts in PERT）**

零件可能被操作消费或由其产生。因此，在操作 PERT 图中，零件表示在流上而非操作框中。

- 当您将零件实例从树拖到流上时，该零件被添加到 Flow 选项卡的 Part 列表中。这表示该零件由源操作生产并由目标操作消费。
- 当您将零件实例从树拖到操作框上时，会创建一个源并链接到该框。这表示该零件被操作消费，但并非源自该视图层级中存在的源操作。
- 零件图标的显示可通过单击工具栏上的图标来切换开/关。

**在 PERT 中分配制造特征（Assigning Manufacturing Features in PERT）**

要向操作添加 Mfgs，请将 Mfgs 从制造特征库拖放并放置到所需操作框中。Mfg 图标出现在操作框的底行。

若向同一操作添加多个 Mfg，已分配 Mfg 的数量显示在 Mfg 图标旁边。

**拖放以将变体集条件应用于操作流（Drag&Drop to Apply Variant Set Conditions to Operation Flows）**

您可以选择 PERT 图中的一个或多个流箭头，并将它们拖到树中的变体（Variant）节点（或其 Target Objects 选项卡），以将变体条件分配给所选操作流。

打开流的属性对话框以在 Variant Set 字段中查看关联变体集的名称。打开流的变体集编辑器——关联的变体集出现在表达式字段（下半部分）中。

<a id="v3-s118"></a> <!-- p614 -->
### PERT 背景和菜单（PERT Background and Menu）

操作 PERT 菜单包含各种 PERT 图显示选项。许多命令允许您设置 PERT 操作框和流后面的背景，以增强图表的视觉清晰度和易用性。选项包括切换显示指示打印页边界的（虚线）页面边界线，以及便于对齐 PERT 图内容的网格。默认情况下，边界线和网格都显示。

单击 PERT 菜单按钮以显示菜单：

- **Snap to Grid**：选中时，您在 PERT 图中拖动的对象在释放鼠标按钮时会捕捉到最近的网格点。
- **Grid Properties**：显示 Grid Properties 窗口，允许您指定网格点的各种参数。
- **Grid Color**：从调色板中选择以设置网格点的颜色。单击 Grid Color 字段旁边的向下箭头打开 Grid Color 调色板。
- **Grid Spacing**：以英寸指定网格点之间的水平和垂直间距。值范围为 0.0625 到 6。
- **Page Bounds**：切换显示指示打印页边界的虚线。这些线针对 A4 纸（21 × 29.7 厘米）调整。
- **Zoom**：控制 PERT 图的显示大小。Zoom 对话框包含四个值：50%、75%、100%（默认）、200%，并允许您通过键入值设置自己的视图放大率。

<a id="v3-s119"></a> <!-- p615 -->
### 捕获 PERT 图的快照（Capturing Snapshots of the PERT Chart）

在生成 PERT 图的任何阶段，您都可以拍摄图的"快照"以备日后参考。快照有助于回忆早期阶段操作和对象的配置。Process Designer 将快照保存为 JPEG（.jpg）图形格式附件，附加到当前父复合操作或过程。

要捕获 PERT 图的快照，请单击工具栏上的 **Capture View** 按钮。附加文件的图标出现在图中，并作为该文件的快捷方式。双击图标打开并查看文件（在本地计算机的默认查看器中）。
<a id="v3-s120"></a> <!-- p616 -->
## 甘特查看器（Gantt Viewer）

<a id="v3-s121"></a> <!-- p616 -->
### 操作和资源的甘特图视图（Gantt Chart View of Operations and Resources）

**甘特图（Gantt chart）** 视图提供操作和分配给它们的资源沿操作节拍时间线的图形表示。使用甘特图规划和平衡每个工作站或工作单元、以及整个生产线的负载。在甘特图中所做的任何更改都会立即反映在 Process Designer 中。

要访问甘特图，请选择 **Home** 选项卡 → **Viewers** 组 → **Gantt Chart**。

甘特查看器包含以彩色水平条表示操作的甘特图。条的长度反映操作的持续时间。操作之间的依赖关系（甘特资源视图中的 Direct Links）由箭头表示，从较早操作的（完成）边缘发出，指向后续操作的（开始）边缘。甘特查看器有其自己的工具栏。

有两种甘特视图：

- **Gantt Operation View**（甘特操作视图）
- **Gantt Resource View**（甘特资源视图）

两种视图的功能相同；甘特工具栏在视图之间有一些变化。

> **注意**
> 您可以自定义用于树的右键菜单，以提高工作效率。

<a id="v3-s122"></a> <!-- p618 -->
### 甘特工具栏（Gantt Toolbar）

甘特工具栏包含使您能够修改和控制甘特布局以及操纵图形显示的命令。两个甘特查看器的工具栏略有不同。以下按钮及其功能（已精简为关键项；除非特别说明，所有命令均适用于操作和两种资源视图）：

- **Load**——将所选过程（操作视图）或甘特研究（资源视图）加载到甘特视图。
- **Print / Print Preview**——打印 / 打印预览。
- **Link（仅操作视图）**——链接两个或多个所选操作以创建操作序列。单击箭头显示 Cyclic Link 图标。
- **Unlink（仅操作视图）**——取消两个或多个链接操作之间的链接。
- **Cyclic Link（仅操作视图）**——表示所连接两个操作之间的约束——第一个操作必须完成，第二个才能开始。
- **Hide Operation / Show All**——隐藏所选操作 / 显示所有先前隐藏的操作。
- **Zoom Out / Zoom In / Zoom to Fit / Zoom to Selection**——缩小 / 放大 / 适应 / 缩放到选择。
- **Refresh**——加载甘特查看器后若做了更改，单击 Refresh 重新计算所有当前模式。
- **Show Default Cycle Time**——切换默认节拍时间——显示红色垂直线。
- **Resource Cycle Time**——切换单个资源节拍时间——绿色垂直线。
- **Gantt Settings**——打开甘特设置对话框。
- **Show Wrapped Cycle Mode**——根据指定节拍时间以循环视图显示操作序列，忽略周期性操作。
- **Show Critical Operations**——显示所选序列的关键操作。
- **Sequence Mode / Cycle Mode（仅操作视图）**——在序列模式和循环模式之间切换。
- **Longest Path（仅操作视图）**——切换最长路径显示叠加——蓝色水平条。
- **Assigning Resources**——打开用于将资源与操作链接的对话框。
- **Relative to Parent Start Time（仅资源视图）**——将所选子操作设置为与其父复合操作同时开始。
- **Display Idle Time（仅资源视图）**——切换空闲时间模式，显示每个资源的空闲时间。
- **Latest Possible Start Time（仅操作视图）**——关闭时操作设置为最早可能开始时间（时间 0）；打开时设置为最晚可能开始时间以消除不必要延迟。
- **Display Distribution（仅资源视图）**——切换分布模式，查看周期性操作添加到平均周期的时间。
- **Constraint Groups**——允许您创建和编辑操作之间的约束（"No Overlap"或"Simultaneous"）。

<a id="v3-s123"></a> <!-- p620 -->
### 甘特操作视图（Gantt Operation View）

此视图在左窗格中显示所选复合操作的整个层级。树中每个项旁边是其变体集（如适用，参见 Variant Sets and Variant Filters）和工作时间（Working Time）。

右窗格中的甘特图说明操作的开始时间、持续时间及其相互关系。任何特定操作的条的颜色是其某个已分配资源的颜色（参见 Query Color Wizard）。您可以通过将鼠标置于甘特图中的操作上来查看其名称、开始时间和持续时间（出现包含此信息的工具提示）。

要打开甘特操作视图：

右键单击复合操作或过程，然后从上下文相关菜单中选择 **Open with Gantt Viewer**；或选择复合操作或过程并单击工具栏上的 **Gantt Viewer** 图标。

> **注意**
> - 操作的选择在甘特查看器与 Process Designer 树和图形查看器之间同步。
> - 甘特操作视图可以为甘特研究（Gantt Study）中的对象打开（参见 Gantt Resource View）。

要更改操作的开始时间：

**操作步骤**

1. 将鼠标指针置于操作的开始处。
2. 单击并按住鼠标按钮。
3. 将操作的开始处拖动到所需的开始点。所选条的轮廓移动到新的开始时间，并出现显示新开始时间的工具提示。
4. 释放鼠标按钮。

要更改操作的持续时间：

**操作步骤**

1. 将鼠标指针置于操作的末端。
2. 单击并按住鼠标按钮。
3. 将操作的末端拖动到所需的结束时间。所选条的轮廓移动到新的结束时间，并出现显示新持续时间的工具提示。
4. 释放鼠标按钮。

<a id="v3-s124"></a> <!-- p622 -->
### 甘特资源视图（Gantt Resource View）

创建甘特研究（Gantt Study）以显示每个资源的利用率。甘特研究充当活动资源的集合。

要创建甘特研究：

**操作步骤**

1. 选择项目根节点或 Collection 文件夹。
2. 选择 **Home** 选项卡 → **Edit** 组 → **New**，并从 New 对话框中选择 **Gantt Study** 文件夹。
3. 打开新研究的属性对话框并选择 **Contents** 选项卡。
4. 在单独的窗口中，打开包含您要在甘特图中查看的活动资源的树视图。
5. 将所需活动资源从资源树窗口拖到研究的 Contents 选项卡。或者，您可以将所需活动资源拖到研究的标题上。当您打开研究的 Contents 选项卡时，它包含已分配的资源。

要打开甘特资源视图：

- 右键单击甘特研究，然后从上下文相关菜单中选择 **Open with Gantt Viewer**；或
- 选择甘特研究并单击工具栏上的 **Gantt Viewer** 图标。

甘特资源视图打开。左窗格显示所选资源以及连接到它们的所有操作。单个操作的持续时间显示在操作名称旁边。右窗格中的甘特图显示资源随时间变化的操作。

> **注意**
> 您无法从甘特资源视图更改操作的开始时间。

<a id="v3-s125"></a> <!-- p625 -->
### 操作属性（Operation Properties）

甘特操作属性（Gantt Operation Properties）对话框显示所选操作的 Start、End 和 Allocated Time（持续时间）。它还指示必须在该操作可以开始前完成的操作名称（"Predecessors"），以及只能在该操作完成后开始的操作（Successors）。还显示操作的频率（"Every Cycle"或"x times every y cycles"）。

要打开甘特操作属性对话框：

- 在甘特查看器右窗格中，右键单击操作的条，并从右键菜单中选择 **Properties**。

您可以编辑开始时间、分配时间（持续时间）和频率。

<a id="v3-s126"></a> <!-- p625 -->
### 隐藏操作（Hide Operation）

您可以通过"隐藏"不需要查看的操作来清理甘特图视图。所选操作及其链接从甘特查看器中移除，并可随时恢复。

要隐藏操作：

- 选择操作并单击图标；或右键单击甘特图形显示中的操作条，并从上下文菜单中选择"Hide Operation"。

要显示隐藏的操作：

- 单击甘特工具栏上的图标。

<a id="v3-s127"></a> <!-- p626 -->
### 直接链接（Direct Links）

两个操作之间的直接链接（或流）指示第二个操作依赖于第一个操作，并且只能在第一个操作完成后开始。这与操作 PERT 流（Operation PERT Flows）概念相同。

从源操作端点的线上，光标被替换为交叉图标。当您将光标置于目标操作条上时，建立链接，您可以向左或向右拖动目标条以分别减少或增加延迟时间。

> **注意**
> 若您尝试拖动的连接无效，会出现"禁止进入（no entry）"符号而不是光标。

**操作步骤**

1. 选择源操作条。
2. 按住键盘上的 CTRL 键并选择将构成流的操作。您可以选择多个操作并将它们链接到单个流序列。
3. 单击工具栏上的图标以链接所选操作。单击箭头显示并选择 Cyclic Links 图标。选择操作的顺序决定了它们之间的流序列，因此以正确顺序选择操作很重要。

要查看流的属性：

- 选择流（箭头变为蓝色），右键单击，并从上下文菜单中选择"Properties"。

> **注意**
> Gantt 忽略循环 scope 流。将循环流的 Flow Type 更改为非循环类型会导致 Gantt 检查循环流并崩溃。系统对可能发生这种情况的流停用 Flow Type 参数。Gantt 图以与常规链接相同的颜色和线宽显示 scope 流链接，您可以以相同方式编辑它们。链接仅显示在甘特图中。

<a id="v3-s128"></a> <!-- p627 -->
### 循环链接（Cyclic Links）

循环链接表示所连接两个操作之间的约束，指示第一个操作必须完成，第二个操作才能开始。当资源在第 N 周期处理零件，而第 N+1 周期的一个零件正在等待资源可用时，需要此类约束。循环链接用于定义操作序列中的周期，并计算最小节拍时间。

要在两个操作之间创建循环链接：

**操作步骤**

1. 在甘特操作视图中，选择两个操作。
2. 单击 **Create Cyclic Link**。循环链接在甘特查看器中显示为粗箭头。

> **注意**
> - 计算操作开始时间时不考虑循环链接。
> - 循环链接只能使用同一树级别上的操作创建。

<a id="v3-s129"></a> <!-- p628 -->
### 约束组（Constraints Groups）

您可以强制甘特查看器中的操作连续运行，使其开始和结束时间不重叠。相反，您可以将不重叠（部分或完全）的操作设置为同时运行。使用约束组（Constraints Group）对话框在操作之间添加 No Overlap 或 Simultaneous 约束。

- **no Overlap**——使用约束组对话框防止所选操作重叠。
- **Simultaneous**——使用约束组对话框使操作同时运行（例如通过移除运行时中的拆分）。

要创建带约束的对象组：

**操作步骤**

1. 在甘特工具栏上，单击图标。随即打开 Constraint Manager（约束管理器）。
2. 要创建新组，单击 **New**。组列表中将创建一个新组（组按顺序排列编号，且不能重命名）。
3. 在组的 **Type** 字段中，从下拉列表中选择 **No Overlap** 或 **Simultaneous**。

> **注意**
> 如果您尝试对两个或多个已在不同组中具有 Simultaneous 约束的操作应用“No Overlap”约束，或反之，则该更改不被允许。

4. 要向组中添加操作，单击 **Add**，并浏览到所需操作。
5. 单击 **OK** 以保存更改。

<a id="v3-s130"></a> <!-- p631 -->
### 取消链接（Unlink）

要移除操作之间的流：

**操作步骤**

1. 选择链接的操作并单击工具栏上的图标。
2. 右键单击并从上下文菜单中选择"Delete"。
3. 按键盘上的 Delete 按钮。

<a id="v3-s131"></a> <!-- p632 -->
### 模式（Modes）

以下显示模式使您能够控制甘特查看器中显示的信息：

**Sequence Mode（序列模式）**

序列视图模式以默认的指定操作序列显示所选复合操作。每个单独的操作流沿时间轴布置，依赖关系以串行数组显示。使用此模式从头到尾查看所选过程并评估完成所需的时间。

单击图标在 Sequence 模式（按钮未按下）和下面的 Cycle 模式（按钮按下）之间切换。

> **注意**
> - 复合操作的分配时间不会从其子操作自动计算。
> - 若在加载甘特查看器后做了更改，请单击 Refresh（或右键单击甘特图的空白部分并从上下文菜单中选择 Refresh calculation）以重新计算所有当前模式。

**Cycle Mode（循环模式）**

甘特查看器在循环模式中显示从深度 0 到循环模式深度值（参见 Gantt Settings）的所有操作——这意味着它们的开始时间为 0。循环模式深度以下的子操作以序列模式显示，便于比较子操作。

> **注意**
> 循环模式深度必须始终大于 0。

单击图标在 Cycle 模式（按钮按下）和 Sequence 模式（按钮未按下）之间切换。

**Wrapped Cycle Mode（环绕循环模式）**

环绕循环模式将甘特操作视图中的操作序列显示为循环序列，每个节拍时间重复自身。这不是资源节拍时间，而是在 Activate Wrapped Cycle Mode 对话框中定义的节拍时间。在此模式下，甘特查看器仅考虑环绕循环级别（在 Gantt Settings 中设置）的操作。序列中超过节拍时间的操作（即在定义节拍时间之后结束的操作）环绕到周期的开始。

操作的环绕意味着该操作可以与操作序列中的其他操作并行执行。

要从环绕操作创建链接（流）：

**操作步骤**

1. 右键单击环绕操作并按住鼠标按钮。
2. 将鼠标图标拖动到您希望链接到的操作。
3. 释放鼠标按钮。出现连接环绕操作末端与第二个操作开始的箭头。

> **注意**
> - 若在加载甘特查看器后做了更改，请单击 Refresh 以重新计算所有当前模式。
> - 操作持续时间或开始时间的更改会根据定义的周期线更新操作条的显示，这些更改不会更新最小节拍时间。
> - 导致操作超过节拍时间的操作持续时间的更改或操作的添加，不会导致操作条被环绕。操作超过周期线。
> - 若操作拆分为两部分，将持续时间更改为等于或长于节拍时间会使操作变为单个条，从为操作定义的开始时间开始并超过周期线。
> - 激活环绕循环模式时，Show Longest Path 模式被禁用。若此模式已激活，则在激活环绕循环模式时它被停用。
> - 使用固定节拍时间激活环绕循环模式时，所有可见操作（无论其级别）都被环绕，且不超过周期线（前提是它们的持续时间不长于节拍时间）。
> - 使用最小节拍时间激活环绕循环模式时，甘特查看器考虑指定环绕循环级别上的所有操作及其甘特约束来计算最小节拍时间。

要计算最小节拍时间，用户必须定义计算所基于的树级别。节拍时间的默认级别由管理员设置，并保存在应用程序数据中。若管理员未设置默认节拍时间，系统假定值为 0，并在激活环绕循环模式命令时提示用户设置节拍时间。

要设置固定节拍时间：

**操作步骤**

1. 在甘特操作视图中，选择操作。
2. 单击 **Activate Wrapped Cycle Mode**。随即显示 Activate Wrapped Cycle Mode 对话框。
3. 选择 **Use Fixed Cycle Time**。
4. 输入要使用的节拍时间。
5. 单击 **OK**。

要自动计算最小节拍时间：

**操作步骤**

1. 在甘特操作视图中，选择操作。
2. 单击 **Activate Wrapped Cycle Mode**。随即显示 Activate Wrapped Cycle Mode 对话框。
3. 选择 **Automatically Calculate Minimal Cycle Time**。
4. 单击 **OK**。

> **注意**
> - 树级别用于确定计算最小节拍时间时操作的持续时间。若树中的分支包含的级别少于定义的级别，系统会发出错误消息。
> - 计算节拍时间和固定节拍时间的甘特结果不同。
> - 甘特约束仅在节拍时间自动计算时才考虑。对于固定节拍时间，不考虑约束。

<a id="v3-s132"></a> <!-- p636 -->
### 甘特设置（Gantt Settings）

单击图标使您能够编辑节拍时间并设置甘特窗口中显示的列。

**操作步骤**

1. 输入 **Default cycle time**（默认节拍时间）。
2. 输入沿操作树层级向下的级别数，用于以下之一：
   - **Cycle mode depth**——注意甘特图的根定义为深度 0；此设置在 Cycle Mode（参见 Modes）中使用，必须始终大于 0。
   - **Wrapped cycle level**——注意甘特图的根定义为级别 1；此设置在 Wrapped Cycle Mode（参见 Modes）中使用，必须始终大于 0。
3. 若您打算在 Product Designer 之外使用甘特计算信息，请勾选 **Store Calculation Results**。然后信息作为甘特信息存储在根上。
4. 在 **Show Columns in Tree** 区域，勾选要在甘特查看器中显示的列，并清除要隐藏的列。
5. 单击 **OK**。

> **注意**
> 更改显示配置后，单击图标重新加载甘特查看器。若在加载甘特查看器后做了更改，请单击 Refresh 以重新计算所有当前模式。

<a id="v3-s133"></a> <!-- p638 -->
### 显示默认节拍时间（Show Default Cycle Time）

还可以针对预定义的默认节拍时间评估各个子操作。要执行此比较，请右键单击图形显示中的空白点，并从上下文菜单中选择 **Set default cycle time**。随即显示甘特设置对话框。或者，单击 Cycle Time Settings 图标打开包含 Show columns in tree 部分的甘特设置对话框。

定义默认节拍时间后，单击工具栏上的图标以显示由一系列红色垂直线标记的周期性的默认节拍时间。在 Cycle Mode 图中，默认节拍时间设置为 50 秒——Station A1 超过默认节拍时间，而 Station A2 保持在时间约束内。

甘特操作视图以层级显示操作，并允许您定义循环模式深度。为此，右键单击图形显示中的空白点，并从上下文菜单中选择 **Set cycle mode depth**。随即显示甘特设置对话框。使用 Cycle mode depth 字段输入要应用 Cycle Mode 的操作树层级向下的级别数。

<a id="v3-s134"></a> <!-- p639 -->
### 显示关键操作（Show Critical Operations）

关键操作是决定总节拍时间的操作序列。其中一个关键操作的持续时间更改将直接影响节拍时间。

激活 Show Critical Operations 命令时，会计算所选序列中的关键操作并显示在甘特操作视图中。关键操作及其流在甘特视图和树视图中都以强调显示。计算根据所选树级别中操作的分配时间进行。您在 Project Settings 对话框中选择此级别。

要计算关键操作：

**操作步骤**

1. 在甘特操作视图中选择一个操作。
2. 单击 **Show Critical Operations**。关键操作以深色边框标记。

> **注意**
> - 更改序列中操作的开始时间、持续时间或连接时，必须使用 Refresh 命令重新计算关键操作。
> - 激活 Show Critical Operations 命令时，所有关键操作（若有多个）都以深色边框标记。

<a id="v3-s135"></a> <!-- p640 -->
### 最长路径（Longest Path）

（仅甘特资源视图）使用此选项在甘特视图中的复合操作条上显示蓝色水平条叠加。图标按钮切换最长路径显示。这些路径表示操作持续时间的摘要——每个蓝色路径是各复合操作下操作持续时间的摘要。最顶部的蓝色路径统计整个父复合操作的持续时间。显示路径时，将光标置于蓝色条上以激活包含路径开始时间和持续时间信息的工具提示。
<a id="v3-s136"></a> <!-- p642 -->
## 快速放置（Fast placement）

本视频演示如何使用 Fast placement（快速放置）、Placement manipulator（放置操纵器）和 Relocate（重定位）命令来定位对象。

> **注意**
> 视频未包含在 PDF 中。要访问视频，请使用 HTML 版本。

Fast Placement（快速放置）工具使您能够仅沿线性 X 轴和 Y 轴移动一个或多个对象。

**操作步骤**

1. 选择 **GV Toolbar** 选项卡 → **Pick Level** 组 → **Component**。
2. 选择 **Modeling** 选项卡 → **Layout** 组 → **Fast Placement**。图形查看器（Graphic Viewer）中的光标变为手形。
3. 在图形查看器中选择并拖动一个或多个对象到新位置，按需调整。

> **注意**
> 拖动对象时，对象的 X、Y 和 Z 坐标会显示在手形光标下方。Z 坐标始终为零。

4. 使用完 Fast Placement 工具后，单击 **Select** 使光标恢复为默认的箭头状态。

> **提示**
> 使用 Fast Placement 工具移动对象有时会扭曲其实际位置，因为它们不是沿 Z 轴移动的。为在图形查看器中保持正确的透视，建议将 View Point（视点）更改为 Top（顶视）。详见 View Point。

> **注意**
> 仅当父组件处于 Modeling（建模）模式时，Fast Placement 才作用于实体（entity）。否则，即使选择了实体，它也作用于组件。

<a id="v3-s137"></a> <!-- p642 -->
## 克隆集合（Clone Collection）

<a id="v3-s138"></a> <!-- p642 -->
### 集合克隆工具（Collection Cloning Tool）

Clone Collection（克隆集合）工具使用户能够快速、准确地在 Process Designer 中创建重复的流程数据。

Clone Collection 应用程序会在 Process Designer 导航树中创建某个对象的完全相同副本。该副本是源对象的克隆，在其所有属性上与源对象完全一致。

> **注意**
> 克隆对象还包含工程数据（engineering data）和仿真信息（simulation information）。

下图展示了执行 Clone Collection 后的结果，在 Process Designer 导航树中显示了两个相同的对象。

<a id="v3-s139"></a> <!-- p643 -->
### 运行克隆集合（Running Clone Collection）

如果您希望克隆包含 PLC 和产线仿真数据的研究，请确保所选集合包含全部相关的 PLC 数据。

Logic Behavior（逻辑行为）不能作为原型数据被克隆。如有必要，请在克隆操作之后手动更新逻辑行为，以便仿真能够正确运行。

要创建重复对象：

**操作步骤**

1. 在 Process Designer 项目窗口中，选择要复制的对象。

> **注意**
> Clone Collection 只能复制 Collection 类型的对象，或任何从 Collection 派生的类（参见 To view derived classes）。您可以在属性窗口的 General（常规）选项卡上找到对象的类型（参见 To view object properties）。

2. 选择 **Home** 选项卡 → **Clone** 组 → **Clone Collection**。随即出现以下消息：
3. Clone Collection 会自动签出您正在复制的对象及其层级中的所有对象。单击 **Yes** 可在执行克隆操作后签入这些对象；单击 **No** 则保持这些对象为签出状态。

随即出现 Clone Collection 对话框。

4. 按如下操作进行：
   a. 在 **Cloning includes** 部分，勾选 **Engineering Data** 或 **Simulation Info**，或两者都勾选，以使克隆包含这些数据。
   b. 在 **Target Collection** 部分，选择新克隆所添加到的目标集合文件夹之一：Collection 的父节点（Collection's Parent node），或用户自定义（User Defined）——设置您选择的文件夹。
   c. 克隆 Alternative（备选方案）节点时，可以指定是否应用 XSL 过滤器。

> **注意**
> - 启用 Clone Engineering Data 或 Clone Simulation Info 可能会显著增加完成克隆操作所需的时间。默认两者均启用。
> - 工程数据保存在本地系统根目录中，而非 eMServer 数据库中，因此需要写入权限才能修改工程数据。工程数据的示例包括 Snapshot（快照）、Pose（姿态）和 Note（注释）。
> - Simulation Info 还包含 PLC 数据。
> - 系统会复制源集合中的快捷方式（前提是目标对象未嵌套在源集合下），而不会创建目标对象的副本。

5. 单击 **Clone**。操作开始，进度条指示剩余时间。

> **注意**
> - 具有大量子对象和链接的源对象可能会增加操作的持续时间以及对计算机资源的占用。建议在执行此操作之前先准备好对象。
> - 如果您选择在克隆中包含工程数据，但没有系统根目录的写入权限，系统会提示您要么不进行工程数据克隆而继续，要么中止克隆操作。

6. 在源对象相同的父节点下查找新的重复对象。

> **注意**
> 源对象和重复对象具有相同的名称。这是可能的，因为两个对象具有不同的 External ID（外部 ID）编号（可在对象属性中查看，参见 To view object properties）。

7. 如有必要，右键单击对象并选择 **Check In**（签入）。

**To view object properties（查看对象属性）：**
在 Process Designer 导航树中，右键单击对象并选择 **Properties**（属性）。
或
按 **Alt + Enter**。
单击 **Yes**。随即出现 Object Properties（对象属性）窗口，顶部为 General 选项卡：

> **注意**
> 不同的对象可能具有不同的选项卡集（位于属性对话框顶部）。

**To view derived classes（查看派生类）：**
在 Process Designer 导航树中，派生类显示为子元素。

<a id="v3-s140"></a> <!-- p647 -->
### 克隆集合机制——范围内与范围外场景（Clone Collection Mechanism - Inside and Outside Scope Scenarios）

Clone Collection 同时支持 Inside Scope（范围内）和 Outside Scope（范围外）两种场景：

<a id="v3-s141"></a> <!-- p647 -->
#### 范围内（Inside Scope）

Inside Scope 是源中两个对象之间的链接。Clone Collection 在重复对象中创建等效的链接。重复对象独立于源对象，且重复链接独立于源链接。

下图展示了此场景：

Clone Collection 为每个克隆对象创建一个新的唯一 External ID。

当源对象（例如对象 B）将另一个对象（例如对象 A）的 External ID 作为特殊属性包含时，Clone Collection 会提供克隆对象 A 的 External ID 作为克隆对象 B 的特殊属性。

<a id="v3-s142"></a> <!-- p648 -->
#### 范围外（Outside Scope）

Outside Scope 是从源到源外某个对象的链接。Clone Collection 创建一个重复对象，并在等效的重复对象与该 Outside Scope 对象（既不在源中也不在重复中）之间建立链接。虽然源和重复彼此独立，但它们都链接到 Outside Scope 对象。

下图展示了此场景：

当源对象的特殊属性包含超出范围的某个对象的 External ID 时，Clone Collection 不会为克隆对象更改此特殊属性。

<a id="v3-s143"></a> <!-- p649 -->
### 已复制与未复制的信息（Information Duplicated and Information Not Duplicated）

Clone Collection 应用程序在 Process Designer 导航树中创建源对象的完全相同副本。该副本是源对象的克隆，在其所有属性（流程、资源和 Mfg）上与源对象完全一致。

<a id="v3-s144"></a> <!-- p649 -->
#### 系统根文件（System Root Files）

系统根文件是诸如文本文件、工程数据文件等的附件。虽然这些信息并非 Process Designer 对象的组成部分，但它们可能拥有指向系统根文件中信息的 External ID。Clone Collection 创建新的 External ID，这些 ID 虽唯一，仍使克隆对象能够指向系统根文件中的相关信息。

<a id="v3-s145"></a> <!-- p649 -->
#### 克隆集合的自动签出与签入（Automatic Check Out and Check In by Clone Collection）

Clone Collection 命令会自动签出它正在复制的对象。它还会签出对象层级中的所有对象，一直到各个零件。签出操作也会影响范围外的对象，这些对象由链接指向（在这种情况下，链接是双向的：源指向范围外对象，该对象也指回源）。

如果用户将操作设置为在操作结束时执行自动签入，Clone Collection 会签入它在操作开始时签出的所有对象。例外情况是：某个对象在运行 Clone Collection 之前就已处于签出状态。在这种情况下，操作会将该对象保持为签出状态，同时对其他对象执行签入。

<a id="v3-s146"></a> <!-- p649 -->
## 更新备选方案（Updating Alternatives）

<a id="v3-s147"></a> <!-- p649 -->
### 备选方案模块（Alternative Module）

> **注意**
> Alternative Module（备选方案模块）提供了强大的功能，可支持许多不同的用例和工作流。它仅供 KeyUser（关键用户）或管理员使用，因为它可能对您的数据产生重大影响，如下面各节所述。

Update Master（更新主对象）和 Clone/Update Alternative（克隆/更新备选方案）命令使您能够执行以下操作：
- 使用其克隆备选方案的属性值更新源（范围）。
- 使用来自多个派生克隆备选方案的若干属性值更新源。
- 将克隆备选方案中的属性值恢复为源（主对象）的值。

<a id="v3-s148"></a> <!-- p650 -->
### 常规更新（General Update）

执行对多个备选方案的 General Update（常规更新）时，系统按备选方案被选中的顺序执行更新。有两种执行常规更新的方式：
- **Update Master**：用您在有效克隆备选方案中所做的更改更新主对象。例如，在决定采用某个提议的备选方案后，可使用此命令用所选备选方案中的更改更新主对象。
- **Clone/Update Alternative**：用其所派生的主对象的数据更新备选方案。例如，在决定采用某个提议的备选方案后，可使用此命令将该所选备选方案声明为主对象。

> **注意**
> 如果存在多个备选方案范围（Alternative Scope），系统会发出错误消息。

这可能出于以下原因而有必要：
- 备选方案以不令人满意的方式被更改，您希望恢复其原始值——重置备选方案（reset Alternative）。
- 源的值已更改，您希望将新值赋给备选方案——更新备选方案（update Alternative）。
- 您已决定采用某个备选方案用于生产，并希望用所选备选方案的全部属性值更新源——更新主对象（update master）。

您可以执行以下任意操作：
- 更新所有对象（参见 General Update）。
- 如果不满意，回滚更改。参见 Roll Back（回滚）。
- 查看对备选方案所做更改的历史日志。参见 Show Alternatives History（显示备选方案历史）。

> **注意**
> 数据导出和导入在服务器上产生的负载大于客户端，而合并备选方案模块选项信息与修改则加载客户端计算机。

要更新备选方案中的所有对象：

**操作步骤**

1. 选择备选方案（Alternatives）。
2. 选择 **Home** 选项卡 → **Clone** 组 → **Update Master** 或 **Clone/Update Alternative**。随即出现 Update Master 或 Clone/Update Alternative 对话框。
3. 默认选择 **Predefined Configuration**（预定义配置）。从此下拉列表中选择一个配置。这些配置通常由系统管理员预设，参见 Alternative Configurations（备选方案配置）。
4. 具有相关权限的用户可以创建自定义配置，如下所示：
   a. 选择 **Custom Configuration**（自定义配置）并单击 **Edit Configuration**（编辑配置）。随即出现 Edit Configuration 对话框。
   b. 配置 Settings（设置）参数，如下所示：
      - **Update attributes**（更新属性）默认勾选。若希望阻止更新数据属性，请清除它。
      - 若希望配置要从更新中排除的属性，单击 **Attribute Configuration**。参见 Configuring Attribute Exclusions from General Update（配置常规更新中要排除的属性）。
      - **Update engineering data**（更新工程数据）——克隆研究文件夹及其下的研究信息。此选项还会克隆系统根上的已连接 TuneCell 文件夹，并将信息连接替换为克隆对象。所创建的 TuneCell 具有与备选方案中克隆研究相同的 externalID。可通过 externalID 样式识别，其样式与手动创建对象的 ID 约定不同。
        - 常规 externalID：PP-OracleSchemaName-date-time
        - 备选方案 externalID：PP-GenericGeneratedID
      - **Update simulation info**（更新仿真信息）——克隆在 Process Simulate 中创建的所有仿真信息，例如仿真对象、仿真事件、PLC 数据等。此操作字段包含仿真数据（包括 ID 等），在克隆属于仿真研究一部分的操作时需要更新。
      - **Check In Before Update**（更新前签入）

      > **注意**
      > 必须勾选此选项才能启用回滚（取消签出）。

      - **Check In After Update**（更新后签入）

      > **注意**
      > 签入和签出非常耗时，对于大型结构尤其如此。

   c. 配置 Structural Updates（结构更新）参数，方法是清除 **Update node structure**（更新节点结构，清除所有嵌套参数），或勾选 **Update node structure** 并配置以下参数：
      - **Update resource assignments**（更新资源分配）——更新资源到非同步操作的分配。

      > **注意**
      > 单击 **Exclude** 可配置 Update variant assignments 的例外。参见 Configuring Variant Assignment Exclusions from General Update（配置常规更新中要排除的变体分配）。

      - **Update variant assignments**（更新变体分配）——更新变体到操作、资源以及所有可附加变体的其他对象的分配。

      > **注意**
      > 单击 **Exclude** 可配置 Update variant assignments 的例外。参见 Configuring Variant Assignment Exclusions from General Update。

      - **Update synchronized operations**（更新同步操作）——更新同步操作到工位的分配。
      - **Update MfgFeature assignments to parts**（更新 MfgFeature 到零件的分配）——更新 MfgFeature 到零件的分配。
      - **Recreate deleted nodes**（重新创建已删除节点）

      > **注意**
      > 此选项仅在对单个备选方案执行更新时可用。未启用此选项时，系统会在更新期间创建新对象，但不会重新创建已删除的节点。

      - **Align clone with original**（使克隆与原始对齐）——勾选时，更新备选方案的结构，使其与原始结构匹配。这包括删除（移动到用户文件夹）克隆中新创建的对象，并将对象移回其原始父节点下。如果克隆中新创建的对象被删除，其链接对象（如 usages 和 flows）也会被删除。
   d. 单击 **OK**。
5. 在 **Comment**（注释）字段中，可以输入自由格式的注释。您在此字段中插入的注释会保存在备选方案的历史记录中，可随时访问。历史记录显示已应用于此对象的所有备选方案操作。
6. 单击 **Start Update**（开始更新）。对多个备选方案执行更新时，会显示进度条。

> **注意**
> - 所选的选项应用于您选择更新的所有备选方案。如果希望对其他备选方案实施不同的选项集，请多次执行 Update Alternatives。
> - 如果选择了多个备选方案范围，系统会显示以下错误消息：There are multiple AlternativeScopes present. The operation cannot proceed.（存在多个 AlternativeScope，操作无法继续。）

更新完成时会显示以下消息：

即使在一个或多个备选方案中遇到错误，系统也会将错误消息存储在日志文件中，并继续处理下一个备选方案。默认情况下，日志文件位于您的用户配置文件（user profile）中。

7. 单击 **View logfile**（查看日志文件）。下图显示了一个示例日志文件。

<a id="v3-s149"></a> <!-- p654 -->
### 配置常规更新中要排除的属性（Configuring Attribute Exclusions from General Update）

如果需要，您可以从 General Update 中排除特定属性。

要配置常规更新中要排除的属性：

**操作步骤**

1. 在 Clone/Update Alternative 对话框中，单击 **Attribute Configuration**。随即出现 Field Selector（字段选择器）对话框。
2. 在左窗格中单击一个对象，以在右窗格中显示其属性。
3. 在右窗格中双击某个属性以将其从更新中排除。或右键单击该属性并选择 **Add**。该属性显示在 Field Selector 底部的 Selected fields（已选字段）窗格中。
4. 在 Selected fields 窗格中双击某个属性以取消其从更新中的排除。或右键单击该属性并选择 **Remove**。该属性从 Selected fields 窗格中移除。

> **注意**
> 您可以单击 Field Selector 中的列标题对显示进行排序，以便轻松定位项目。

5. 单击 **OK** 以保存数据。

<a id="v3-s150"></a> <!-- p656 -->
### 配置常规更新中要排除的资源分配（Configuring Resource Assignment Exclusions from General Update）

如果需要，您可以从 General Update 中排除特定的资源分配。

要配置常规更新中要排除的内容：

**操作步骤**

1. 在 Update Master 或 Clone/Update Alternative 对话框中，**Update resource assignments** 默认勾选。执行以下操作之一：
   - 保持原样以更新所有资源分配。
   - 清除它以防止更新资源分配。
   - 单击图标。随即出现 Exclude Resources（排除资源）对话框。
2. 选择要从更新中排除的资源，并单击图标将其移动到右窗格，从而从更新中排除它们。
3. 在右窗格中选择资源并单击图标将其重新包含在更新中。
4. 单击 **OK** 以保存数据。

<a id="v3-s151"></a> <!-- p657 -->
### 配置常规更新中要排除的变体分配（Configuring Variant Assignment Exclusions from General Update）

如果需要，您可以从 General Update 中排除特定的变体分配。

此选项在结构更新的情况下也维护克隆结构中的变体分配。因此，结构被更新，但已分配的变体得以保留。为限制要维护的变体分配数量，您可以使用 Exclude variants（排除变体）窗口（如下所示）排除变体分配的部分内容。

要配置常规更新中要排除的内容：

**操作步骤**

1. 在 Update Master 或 Clone/Update Alternative 对话框中，**Update variant assignments** 默认勾选。执行以下操作之一：
   - 保持原样以更新所有资源分配。
   - 清除它以防止更新资源分配。
   - 单击图标。随即出现 Exclude Variants（排除变体）对话框。
2. 选择要从更新中排除的变体类型，并单击 **OK**。

以下列表描述了可用的变体类型：
- **Operations**——排除 Operation 结构上的变体分配更新
- **Parts**——排除 Part 结构上的变体分配更新
- **Resources**——排除 Resource 结构上的变体分配更新
- **Flows**——排除 Flow 上的变体分配更新
- **Mfg Features**——排除 Mfg Features 上的变体分配更新
- **Part Prototypes**——排除 Part Prototypes 上的变体分配更新
- **Tool Prototypes**——排除 Tool Prototypes 上的变体分配更新
- **Usages**——排除 Usage 上的变体分配更新。Usage 定义哪个资源被分配给操作，例如在产线平衡（line balancing）中，排除此更新有助于避免覆盖已规划好的分配。
- **Base Usages**——排除 Base Usage（如 ToolPrototype Usage、Mfg Usage、PartPrototypeUsage）上的变体分配更新。

<a id="v3-s152"></a> <!-- p658 -->
### 回滚（Roll Back）

如果您对 Updating Alternatives（更新备选方案）中所做的更新不满意，可以回滚更改并重新开始。

> **注意**
> 在执行原始更新时必须设置 Check In Before Update，否则无法回滚。

要回滚对备选方案或源更新所做的更改：
- 在导航树中右键单击对象并选择 **Cancel Check Out**（取消签出）。

<a id="v3-s153"></a> <!-- p658 -->
## 生成流（Generate Flows）

Generate Flows（生成流）对话框允许用户一次创建多个流。用户确定要用流连接哪些操作，以及以何种顺序连接。

- **Function Process（功能流程）**——用流连接一系列同级步骤（sibling steps）以形成时序。使用此工具代替手动创建流，以提高生产率和易用性。
- **Input（输入）**——通常是一个复合操作（compound operation），或特别是通用时序（generic chronology）；或：属于同一时序的一组步骤操作（step operations）。
- **User Interface（用户界面）**——用户可以右键单击复合操作节点，或一组同级操作。随即打开 Generate Flows 对话框。

要访问 Generate Flows 对话框，选择 **Home** 选项卡 → **Flows** 组 → **Generate Flows**。

对话框包含要连接的所有步骤操作的列表。如果选择复合操作，其所有直接子操作将按它们在树中出现的相同顺序显示。如果选择一组步骤，则显示所有所选步骤。

使用 Up（上）和 Down（下）箭头按钮对列表重新排序——您可以勾选 **Delete Existing Flows**（删除现有流）选项，然后单击 **Apply**（应用）以创建流。单击 **Close**（关闭）将关闭对话框而不激活命令。

- **Output and Results（输出与结果）**——命令按 Generate Flows 对话框中显示的顺序，在所列出步骤之间生成流。如果勾选了 Delete Existing Flows 选项，则先前建立的流将被移除。您可以打开 PERT 视图以在其范围上下文中显示新创建的步骤。
- **Conditions and Errors（条件与错误）**——要启用该命令，所选操作列表必须位于相同的范围和层级级别（同级）。所列节点必须已签出，否则会显示错误消息。激活 Delete Existing Nodes（删除现有节点）选项时，所有相关流都必须已签出，否则会显示错误消息。

<a id="v3-s154"></a> <!-- p660 -->
## 删除未连接的流（Delete Disconnected Flows）

未连接的流（Disconnected flows）仅具有前驱或后继，或仅连接到一个操作（断开连接）。Delete Disconnected Flows（删除未连接的流）命令使您能够删除这些冗余对象。

要删除未连接的流：

**操作步骤**

1. 在操作树中选择一个对象，并选择 **Home** 选项卡 → **Flow** 组 → **Delete Disconnected Flows**。随即出现 Delete Disconnected Flows 对话框。
2. 勾选或清除以下选项：
   - **Flows**——删除所有未连接的流/范围流（scopeflows）。
   - **Precedence Constraints**——删除所有未连接的流约束。
3. 单击 **Delete**（删除）以确认删除并关闭 Delete Disconnected Flows 对话框。

<a id="v3-s155"></a> <!-- p660 -->
## 输出（Outputs）

<a id="v3-s156"></a> <!-- p660 -->
### 附加文件（Attach File）

Attach File（附加文件）选项使您能够将三种类型的文件附加到对象：
- 外部（现有）文件
- 查看器显示的修改版本（使用 Markup Editor）
- 查看器显示中您操作的录像文件

您也可以将现有文件作为附件保存在属性窗口中。

所有附件会立即在属性窗口的 Attachment（附件）选项卡中更新。

要附加外部文件：

**操作步骤**

1. 在查看器或树中选择一个模块并签出（Check Out）。
2. 选择 **Home** 选项卡 → **Outputs** 组 → **Attach File**。随即打开 Attach File <零件名称> 窗口。
3. 单击 **Attach File** 图标。随即打开 Add Attachment（添加附件）窗口。
4. 浏览到该文件并选择它，然后单击 **Open**。该文件被附加到模块，其名称出现在 Attach File 窗口和 Planner Product Tree（规划器产品树）的 Attachments 选项卡中。

要删除附加的文件：
- 选择它并单击删除图标。该文件将在无用户确认的情况下被删除。

<a id="v3-s157"></a> <!-- p663 -->
### 将文件附加到研究节点（Attach File to Study Node）

要将快照附加到导航树中的研究节点：

**操作步骤**

1. 选择 **Home** 选项卡 → **Outputs** 组 → **Attach File**。
2. 在图形查看器或 Section Viewer（剖面查看器，若已打开）中创建视图的快照（Snapshot）。
3. 指定文件名和图形格式（.jpg 或 .bmp）。

系统将该快照附加到研究节点，您可以从该研究属性窗口的 Attachment 选项卡访问它。

<a id="v3-s158"></a> <!-- p664 -->
### 导出图像（Export Images）

此选项打开 Export Image（导出图像）窗口，用于将当前图像另存为以下格式之一的图形文件：
- Bitmap 文件（*.bmp）
- Jpeg 文件（*.jpg）
- Gif 文件（*.gif）
- Tif 文件（*.tif）

<a id="v3-s159"></a> <!-- p664 -->
### 标记编辑器（Markup Editor）

Markup Editor（标记编辑器）使您能够获取图形查看器中当前显示图像的快照。图像本身无法被修改，但可以向快照添加标签和标注（callout）。例如，您可以通过电子邮件将快照发送给组织中的其他工程师以征询意见。

选择 **Home** 选项卡 → **Outputs** 组 → **Markup Editor**。

您也可以使用 Markup Editor 从 New Section Viewer（新建剖面查看器）获取快照。

Markup Editor 工具栏中提供以下按钮：

| 按钮（Button） | 名称（Name） | 描述（Description） |
| --- | --- | --- |
| | Save（保存） | 将图像以 .bmp 或 .jpg 文件保存到指定位置。 |
| | Send Mail Message（发送邮件） | 将图像作为附件通过电子邮件发送。 |
| | Print（打印） | 打印图像。 |
| | Cut（剪切） | 将所选对象（标注、文本框或线条）剪切到剪贴板。 |
| | Copy（复制） | 将所选对象（标注、文本框或线条）复制到剪贴板。 |
| | Paste（粘贴） | 将剪贴板内容粘贴到 Markup Editor。 |
| | Delete（删除） | 从 Markup Editor 中删除所选对象（标注、文本框或线条）。 |
| | Group（组合） | 将 Markup Editor 中两个或多个所选对象（标注、文本框或线条）组合为单个组，使这些对象可一起操作。 |
| | Ungroup（取消组合） | 取消一个或多个所选组的组合。 |
| | Bring to Front（置于顶层） | 将所选对象（标注、文本框或线条）置于其他未选中且重叠的对象之前。 |
| | Send to Back（置于底层） | 将所选对象（标注、文本框或线条）置于其他未选中且重叠的对象之后。 |
| | Select Objects（选择对象） | 激活选择模式，使您能够在 Markup Editor 中选择对象（标注、文本框或线条）。 |
| | New Line（新建线条） | 绘制线条。 |
| | New Arrow（新建箭头） | 绘制带箭头的线条。 |
| | New Rectangle（新建矩形） | 绘制矩形。 |
| | New Ellipse（新建椭圆） | 绘制椭圆。 |
| | New Scribble Polygon（新建自由多边形） | 绘制自由形状。 |
| | Add Note（添加注释） | 向快照中的对象添加注释。 |
| | New Text（新建文本） | 向快照添加文本。 |
| | New Text Box（新建文本框） | 在快照中绘制文本框。 |
| | Fill Color（填充颜色） | 修改所选对象（标注、文本框或线条）的颜色。 |
| | Set Font（设置字体） | 更改文本的字体。 |
| | Line Style（线条样式） | 修改线条和箭头的宽度、颜色和样式。 |
| | Arrow Style（箭头样式） | 修改箭头样式。 |

<a id="v3-s160"></a> <!-- p666 -->
## 设置为工作文件夹（Set as Working Folder）

**描述（Description）**
Set as Working Folder（设置为工作文件夹）选项使您能够定义哪个文件夹应保存导入到项目中的所有对象。如果在导入对象之前尚未创建工作文件夹，系统会自动创建一个，并根据登录的用户名命名。例如，如果您导入带有实例（instances）的复合零件（compound part），所有相应的原型（prototypes）都存储在工作文件夹中，而复合零件存储在系统根目录下。

**操作步骤**

1. 在导航树中单击一个项目节点以选中它。
2. 签出（Check Out）该项目节点。
3. 选择 **Home** 选项卡 → **User Settings** 组 → **Set as Working Folder**。
<a id="v4-s1"></a>
# Volume 4: View（第 4 卷：视图）

本卷涵盖 Process Designer 中“视图（View）”相关的命令与功能，包括：布局管理器（Layout Manager）、显示楼层/网格（Display Floor/Grid）、多图形查看器命令（Multiple Graphic Viewer commands）、可见性（Visibility）、剖面（Sections）、轮廓（Contours）、动态裁剪（Dynamic Clipping）以及相机（Camera）。

<a id="v4-s2"></a>
## 4. View（视图）

<a id="v4-s3"></a>
### Layout Manager（布局管理器）

布局管理器（Layout Manager）可用于在 Process Designer 中保存工具栏与查看器的布局。这些布局可根据需要应用到 Process Designer 应用程序窗口。布局列表中包含“标准（Standard）”布局，便于随时恢复到默认定义。

说明（Note）
布局还可以包含对上下文菜单所做的任何自定义，例如背景颜色，详见“自定义（Customize）”。此外，多个导航树（Navigation Trees）以及查看器的多个实例（属性查看器、甘特图查看器、PERT 查看器）都会保存到布局中。
对于封闭域网络，可使用 Tecnomatix Doctor 工具的“启用漫游（Enable Roaming）”选项，将自定义布局存储起来，以便在您登录的网络中其他计算机上使用。激活该选项之前创建的自定义内容不可用。在漫游模式下配置一次的布局，在切换到本地模式时会变为不可用，但再次启用漫游模式时会恢复。

<a id="v4-s4"></a>
#### Creating a New Layout（创建新布局）

您可以根据当前打开布局中各窗口的大小和位置创建新布局。

创建布局的过程（Procedure）：
1. 按照所需布局排布各个查看器与工具栏。
2. 选择 View 选项卡 → Layout 组 → Layout Manager，显示 Layout List（布局列表）窗口。
3. 单击 New（新建）以创建新布局。将打开一个对话框，提示新布局将依据当前打开布局中各窗口的大小和位置来创建。
4. 使用相应选项，根据系统默认或当前用户设置来设定工具栏（Toolbars）、顶级菜单（Top-Level menus）和上下文菜单（context menus）。
5. 在 Layout（布局）列中输入布局名称。
6. 若希望所有用户均可访问该布局，请选中 Public（公共）复选框。

说明（Note）
公共布局只能由具有“公共配置（Public Configuration）”权限的用户创建、更新和删除——请参阅 Tecnomatix Administration 文档。

7. 要在 Process Designer 应用程序窗口中应用新布局，请选择 View 选项卡 → Layout 组 → Layout Manager。

说明（Note）
- 要重命名布局，请在 Layout List（布局列表）窗口中选中它，按键盘 F2 键，然后在 Layout（布局）列中输入新名称。
- 要更新现有布局，请对工具栏和查看器进行所需修改，从 Layout List（布局列表）窗口中选择该布局，然后单击 Update（更新）。
- 要删除布局，请在 Layout List（布局列表）窗口中选中它并单击 Delete（删除）。

<a id="v4-s5"></a>
### Select Layout（选择布局）

Select Layout（选择布局）命令可用于选择一个布局，以应用到 Process Designer 应用程序窗口。更多详情请参阅 Layout Manager（布局管理器）。

<a id="v4-s6"></a>
### Display Floor（显示楼层）

Display Floor（显示楼层）选项用于显示（默认设置）或隐藏装配体在图形查看器（Graphic Viewer）中的楼层（floor）。

选择 View 选项卡 → Screen Layout 组 → Display Floor。

<a id="v4-s7"></a>
### Adjust Floor（调整楼层）

Adjust Floor（调整楼层）命令允许您自动或手动更改楼层及楼层网格（floor grid）的大小，并可更改楼层和网格的颜色。

说明（Note）
计算新楼层大小时仅使用整数值。系统会将非整数值向上舍入到下一个整数。

调整楼层的过程（Procedure）：
1. 选择 View 选项卡 → Layout 组 → Adjust Floor。将显示 Adjust Floor（调整楼层）对话框。
2. 输入 X 和 Y 值，以确定各自网格线之间的间隔，最大不超过各自楼层尺寸的一半。
3. 输入 X 和 Y 值，以确定楼层的大小。系统会计算研究中所有对象（包括隐藏对象）的包围盒。
4. 若要在图形查看器中隐藏楼层，请选中 Show Grid Only（仅显示网格）复选框。
5. 单击 Apply（应用），或单击 Reset（重置）恢复原始设置。
6. 单击 Close（关闭）以应用更改并关闭对话框。

<a id="v4-s8"></a>
#### Changing Floor and Grid Color（更改楼层和网格颜色）

如有必要，您可以更改楼层和网格的颜色，以增强研究的图形显示效果。

过程（Procedure）：
1. 选择 File 选项卡 → Options（选项），单击 Appearance（外观）选项卡并选择 Floor（楼层）。
2. 在 Graphic Viewer 树中选择 Floor（楼层），打开下方的调色板，选择新的楼层颜色。关闭 Options（选项）对话框后颜色更改生效。
3. 在 Graphic Viewer 树中选择 Grid（网格），打开下方的调色板，选择新的网格颜色。关闭 Options（选项）对话框后颜色更改生效。

说明（Note）
默认楼层颜色为灰色。默认网格颜色为黑色。

<a id="v4-s9"></a>
### Multiple Graphic Viewer commands（多图形查看器命令）

您可以打开图形查看器（Graphic Viewer）的多个实例。以下命令可用于管理图形查看器的实例：
- New Window（新建窗口）—— 用于打开图形查看器的额外实例。
- Arrange Windows（排列窗口）—— 用于按需排列图形查看器的各个实例。
- Switch Windows（切换窗口）—— 用于选择哪一个图形查看器为活动实例。

更多信息请参阅 Graphic Viewer。

<a id="v4-s10"></a>
#### Working in the Graphics Viewer（在图形查看器中操作）

<a id="v4-s11"></a>
##### Select（选择）

Select（选择）选项用于恢复鼠标左键的功能，以便在图形查看器中选择对象。

说明（Note）
您也可以使用键盘快捷键 <Alt+S>。

选择对象的方法：
选择 Select（选择）或单击 ，然后通过在图形查看器中按住并拖动鼠标左键来选择对象。将出现标准选择框，选择框中的对象会高亮显示。

<a id="v4-s12"></a>
##### Pan（平移）

Pan（平移）选项用于在图形查看器中沿水平和垂直方向移动对象。

平移对象的方法：
- 选择 Pan（平移）或单击 ，然后使用鼠标左键按住并移动对象到所需位置。
- 或者 -
- 在图形查看器中使用鼠标右键选中对象，然后按住并移动对象到所需位置。

<a id="v4-s13"></a>
##### Zoom（缩放）

Zoom（缩放）选项用于增大或减小图形查看器中视图的大小。

缩放对象的方法：
- 选择 Zoom（缩放）或单击 ，然后使用鼠标左键按住并上下或左右拖动对象。
- 或者 -
- 在图形查看器中使用鼠标中键选中对象，然后按住并上下或左右拖动对象。

当您上下或左右移动对象时，对象会交替进行放大和缩小。

<a id="v4-s14"></a>
##### Rotate（旋转）

Rotate（旋转）选项用于在图形查看器中旋转对象。

旋转对象的方法：
- 选择 Rotate（旋转）或单击 ，然后使用鼠标左键按住并拖动对象至所需位置。
- 或者 -
- 在图形查看器中使用鼠标右键和鼠标中键选中对象，然后按住并拖动对象至所需位置。

<a id="v4-s15"></a>
##### Zoom to Fit（缩放至适应）

说明（Note）
您也可以使用键盘快捷键 <Alt+Z>。

Zoom to Fit（缩放至适应）选项会调整图形查看器中的视图，以显示所有可见对象。该选项可便捷地撤销由缩放和平移造成的较大变动，并可用于判断图形查看器中是否存在远离主对象的额外对象。当您选择 Zoom to Fit（缩放至适应）或单击 时，隐藏对象将被忽略。

<a id="v4-s16"></a>
##### Zoom to Selection（缩放至所选）

Zoom to Selection（缩放至所选）选项会调整图形查看器中的视图，使所选对象以特写方式显示。该选项可对小型选择对象提供便捷的特写视图。
您可以使用 Select（选择）工具选择视图中要缩放的对象，然后选择 Zoom to Selection（缩放至所选）或单击 。所选对象会被放大并居中显示在图形查看器中。

<a id="v4-s17"></a>
##### View Center（视图中心）

View Center（视图中心）选项可用于选择图形查看器中的任意点作为视图中心。请参阅 View Center（视图中心）。

<a id="v4-s18"></a>
##### Parallel/Perspective（平行/透视）

Parallel/Perspective（平行/透视）选项可在图形查看器中将对象的相机视图在 Parallel（平行，默认）与 Perspective（透视）模式之间切换。
- Parallel 模式（平行模式）—— 以无限远处的视角显示数据。这意味着，空间中相互平行的线条在显示中也以平行方式呈现。该模式仅由方向定义，为工程师从不同视角（例如前视图、右视图或顶视图）描述数据提供了便捷的视点。
- Perspective 模式（透视模式）—— 视点由位置、方向和视场定义，反映用户身处数据内部移动时所看到的景象。在该模式下，深度通过有意的畸变进行可视化，使对象根据其在该数据中位置的不同而显得或近或远，从而以更真实的方式呈现工程数据的虚拟世界。

<a id="v4-s19"></a>
### Visibility（可见性）

可见性（Visibility）选项可用于选择对象并更改其在图形查看器中的显示方式。对象查看（Object Viewing）选项包含以下子选项：
- Shaded（着色）
- Wireframe（线框）
- Transparent（透明）

说明（Note）
- 这些选项在研究（study）级别持久保存（它们更改实例，但不更改原型）。
- 若将对象显示设置为 Transparent（透明），随后设置结束建模（end modeling），则会覆盖其原型颜色，只能通过 Modify Color（修改颜色）命令恢复。

每个子选项在 Object Viewing（对象查看）库中都有对应的按钮，并按下表所示更改对象外观：

| 按钮（Button） | 子选项（Suboption） | 说明（Description） |
| --- | --- | --- |
| Shade | 对对象进行着色，使其显示为实体。 |
| Wireframe | 用线条勾勒对象轮廓，使其显示为线框。 |
| Transparent | 对对象进行轻微着色，使通常隐藏在其后的对象可见。 |

<a id="v4-s20"></a>
#### Restore Color（恢复颜色）

修改部件和资源的颜色后，您可以选择部件和资源并单击 以恢复其原始组件颜色。

<a id="v4-s21"></a>
#### Stereo 3D viewing（立体 3D 查看）

警告（Warning）
要启用立体 3D 查看，您的系统必须使用支持 Active Stereo（主动立体）的显卡和屏幕显示器，并且应配备立体查看眼镜。

立体可视化提供了令人印象深刻的立体化三维呈现，增强了用户和决策者的查看体验。

过程（Procedure）：
1. 关闭 Tecnomatix 应用程序——在 Tecnomatix Doctor 中，从 Tools（工具）菜单选择 Enable the use of 3D stereo viewing（启用 3D 立体查看）。
2. 在应用程序中选择 View 选项卡 → Visibility 组 → Stereo 3D On/Off（立体 3D 开/关）以切换立体查看的开与关。

说明（Note）
激活立体查看模式会将查看模式设置为 Perspective（透视），并停用 Parallel/Perspective（平行/透视）的切换功能。

3. 立体查看处于活动状态时，您可以使用 Stereo 3D 选项卡根据需要调整 3D 查看参数。

立体 3D 选项卡中的设置如下：

| 设置（Setting） | 说明（Description） |
| --- | --- |
| Field of View（视场） | 默认值为 28 度。增大度数会沿您设置的视场角方向拉伸图像，可在背景中对象可见性更高的同时造成更大的查看畸变。启用 Asymmetric Projection（非对称投影）会使系统调整每只眼睛的视线方向，使其在会聚距离处会聚视线。 |
| Asymmetric Projection（非对称投影） | 启用该选项会使系统调整每只眼睛的视线方向，使其在下方 Convergence（会聚）选项中设置的会聚距离处会聚视线。停用该选项同时也会停用 Convergence（会聚）选项。 |
| Convergence（会聚） | Relative（相对）——（无单位）会聚距离相对于所显示内容。Absolute（绝对）—— 按您设置的距离进行会聚。 |
| Eye Separation（眼间距） | 两个偏移图像之间的距离——增大距离会产生更明显的立体效果；减小距离会使显示更接近扁平的 2D 查看。Relative（相对）——（无单位）眼间距相对于所显示内容。Absolute（绝对）—— 眼间距由您设置的距离固定。 |

说明（Note）
- Stereo 3D 选项卡仅在您在 Tecnomatix Doctor 中启用 3D 立体查看时，才显示在 Options（选项）对话框中。
- 当立体 3D 模式启用且图形查看器以立体方式显示时，以下工具的输出为标准 2D/3D 图形查看：Attach Image（附加图像）、AVI Recorder（AVI 录制器）、Movie Manager（影片管理器）、3DPDF、Markup Editor（标记编辑器）。
- 以下内容的性能可能会受到一定影响：Multi Sections（多剖面）和 Sections Viewer（剖面查看器）在立体 3D（启用时）中显示。
- 切换到立体 3D 查看模式时，图形查看器工具栏将被停用。要在该模式下访问工具栏，用户需要打开功能区自定义对话框（在功能区上单击右键），并单击以激活 Main Tabs（主选项卡）下的 GV Toolbar（图形查看器工具栏）条目。

<a id="v4-s22"></a>
### Show locations/frames always on top（始终将位置/坐标系显示在顶层）

单击 。启用后，所有位置和坐标系（frames）都会显示在所有其他对象的顶层，便于清晰查看。

<a id="v4-s23"></a>
### Sections（剖面）

<a id="v4-s24"></a>
#### Sections commands（剖面命令）

Section（剖面）命令包含下表所列的多个选项。您可以使用剖切功能，从装配体中通常难以实现的视角和角度查看对象，例如发动机装配体底盘内部的复杂连接点。

| 按钮（Button） | 工具（Tool） | 说明（Description） |
| --- | --- | --- |
| New Section Plane（新建剖面平面） | 用于创建新的剖面平面。请参阅 New Section Plane（新建剖面平面）。 |
| New Section Volume（新建剖面体） | 用于创建新的剖面体。请参阅 New Section Volume（新建剖面体）。 |
| Activate Section（激活剖面） | 使所选剖面能够在图形查看器中裁剪或切割视图。可激活剖面的最大数量由您计算机的显卡决定。叠加层（overlay）显示在对象查看器（Object Viewer）中。对于剖面平面，裁剪（Clip）模式显示剖面负侧（negative side）的所有内容，并裁剪正侧（positive side）的所有内容。对于剖面体，裁剪模式根据所选裁剪模式显示剖面内部或外部的所有内容。切割（Cut）模式显示被剖面切开的装配体轮廓。激活的剖面仅以边框显示，表面不可见（以区别于未激活剖面）。您可以在 Appearance（外观）选项卡中更改默认边框颜色。说明：您可以同时激活多个剖面平面或最多两个剖面体，但不能同时激活剖面平面与剖面体。 |
| Deactivate Section（停用剖面） | 停用所选剖面的裁剪或切割功能，并恢复先前被该剖面裁剪的所有对象。该命令还会更新对象查看器（Object Viewer）中相应的叠加层（overlay）。停用剖面后，图形查看器会自动显示剖面的内侧。 |
| Section Manager（剖面管理器） | 用于在图形查看器中围绕平移和旋转轴操控所选剖面的位置。请参阅 Section Manager（剖面管理器）。创建任意 New Section Plane（新建剖面平面）或 New Section Volume（新建剖面体）后，Section Manager（剖面管理器）会自动打开。 |
| Clip Section（裁剪剖面） | 对于已激活的剖面，有两种剖切模式：裁剪（Clip）或切割（Cut）。默认是裁剪（Clip）——启用一种模式会自动停用另一种模式。裁剪在图形查看器中提供对象视图，显示剖面负侧的所有内容，并裁剪正侧的所有内容。默认情况下，裁剪时不显示对象轮廓——可使用 Show Section Contours（显示剖面轮廓）来显示它们。您可以在 Appearance（外观）选项卡中设置轮廓颜色。• Clip Inside（内部裁剪）—— 裁剪剖面体内部的所有内容。• Clip Outside（外部裁剪）—— 裁剪剖面体外部的所有内容。说明：若剖面体或图形查看器中的对象发生移动，系统会动态更新显示。 |
| Cut Section（切割剖面） | 提供包含所有活动剖面处对象轮廓的剖面图，并在其与剖面平面的相交位置显示对象轮廓。轮廓以其对象相同的颜色显示。启用切割模式会自动停用裁剪模式。 |
| Flip Section（翻转剖面） | 将剖面翻转到相反方向。 |
| Adjust Section Plane Size（调整剖面平面大小） | 将所管理剖面平面的尺寸调整为所有当前显示对象在剖面平面朝向处的包围盒大小。注释（Notes）不计入调整后的尺寸。说明：该命令不适用于剖面体。 |
| Preview Section Contours（预览剖面轮廓） | 在剖面未激活时显示剖面轮廓。选择活动剖面时该命令会被停用。 |
| Orient View to Section Plane（将视图定向到剖面平面） | 将视图更改为朝向剖面的正 Z 轴，同时保持眼睛到视图中心的距离不变。 |
| Section Alignment（剖面对齐） | 提供以下剖面对齐选项：Align to X（对齐到 X）—— 将剖面对齐到工作帧（working frame）的 YZ 平面。说明：不适用于剖面体。Align to Y（对齐到 Y）—— 将剖面对齐到工作帧的 XZ 平面。说明：不适用于剖面体。Align to Z（对齐到 Z）—— 将剖面对齐到工作帧的 XY 平面。说明：不适用于剖面体。Align to Point（对齐到点）—— 将剖面原点置于拾取的位置。Align to Frame（对齐到坐标系）—— 将剖面原点置于所拾取的坐标系处。Align to Line between Two Points（对齐到两点之间的直线）—— 将剖面对齐到您在图形查看器中拾取的两点中心。Align to Edge（对齐到边）—— 将剖面垂直于您在 2D 对象上拾取的点对齐，方向任意设置。Align to Surface（对齐到面）—— 将剖面法线对齐到所选曲面的法线，且剖面原点位于拾取位置。Align to View Plane（对齐到视图平面）—— 根据所有活动剖面的位置以及当前显示对象的位置更新其剖面轮廓。当 Dynamically update section contours（动态更新剖面轮廓）选项停用时有用。Align to View Plane（对齐到视图平面）—— 将剖面的 Z 轴对齐到从视图到向量（from→view to vector）的视图，并将剖面的正 Y 轴对齐到相机的上方向向量（up vector）。剖面位置不变。 |
| Show Section Contours（显示剖面轮廓） | 在裁剪（Clip）模式下显示白色轮廓。移动剖面时轮廓会自动更新。使用 更改轮廓的默认颜色和宽度。 |
| Capping（封盖） | 当显示轮廓且您启用 Capping（封盖）时，系统会对被剖面裁剪的所有对象表面进行着色。说明：• 系统仅对构成封闭体（closed volumes）的表面应用封盖。• 有关如何设置封盖颜色的信息，请参阅 Appearance（外观）选项卡。• 导出数据到 JT 格式时，封盖多边形不包括在内。 |
| Hatching（剖面线） | 当显示轮廓且您启用 Hatching（剖面线）时，系统会在被剖面裁剪的所有对象表面显示对角线（剖面线）。说明：• 系统仅对构成封闭体（closed volumes）的表面应用剖面线。• 有关如何设置剖面线颜色的信息，请参阅 Appearance（外观）选项卡。• 导出数据到 JT 格式时，剖面线多边形不包括在内。• 裁剪轮廓会覆盖剖面线。 |

<a id="v4-s25"></a>
#### New Section Plane（新建剖面平面）

New Section Plane（新建剖面平面）命令沿工作帧（working frame）的 YZ 平面创建一个剖面平面。新剖面平面根据所有显示对象调整大小，并位于这些对象的几何中心。如果已存在剖面平面，New Section Plane（新建剖面平面）会根据当前显示对象调整其大小和位置。

选择 New Section Plane（新建剖面平面）命令会自动打开 Section Manager（剖面管理器）对话框，允许您操控和移动剖面，并将其激活或停用以在图形查看器中切割或裁剪视图。Section Manager（剖面管理器）同时管理剖面平面和剖面体。有关如何创建剖面体的信息，请参阅 New Section Volume（新建剖面体）。

您可以通过将剖面平面添加到组（group）下，并将该组用作模拟对象（simulated object），从而在仿真中使用剖面平面。

<a id="v4-s26"></a>
#### New Section Volume（新建剖面体）

New Section Volume（新建剖面体）命令在图形查看器和对象树（Object Tree）中创建一个立方体剖面体。

选择 New Section Volume（新建剖面体）命令会自动打开 Section Manager（剖面管理器）对话框，允许您操控和移动剖面，并将其激活或停用以在图形查看器中切割或裁剪视图。Section Manager（剖面管理器）同时管理剖面平面和剖面体。有关如何创建剖面平面的信息，请参阅 New Section Plane（新建剖面平面）。

系统根据您创建剖面时选中的对象来定位新剖面体：
- 未选中对象——系统将新剖面置于当前显示对象包围盒的中心，剖面体与工作帧对齐。
- 选中单个对象——系统将新剖面置于当前所选对象的中心，剖面体与工作帧对齐。
- 选中多个对象——系统将新剖面置于当前所选对象的几何中心，剖面体与工作帧对齐。

有关如何配置剖面体大小以及剖面体顶面和底面颜色的信息，请参阅 Appearance（外观）选项卡。系统会自动将剖面体的其他面设置为同一颜色的较深阴影。

您可以通过将剖面体添加到组（group）下，并将该组用作模拟对象（simulated object），从而在仿真中使用剖面体。

<a id="v4-s27"></a>
#### Section Manager（剖面管理器）

选择 View 选项卡 → Section 组 → Section Manager，打开 Section Manager（剖面管理器）对话框。这使您能够在图形查看器中修改所选剖面平面或剖面的位置、大小和方向。

说明（Note）
尽管您可以创建多个剖面平面，但在图形查看器中一次只能管理一个剖面。

<a id="v4-s28"></a>
##### Using Section Manager（使用剖面管理器）

打开 Section Manager（剖面管理器）对话框会显示一个附加到被管理剖面上的位置操控器（Placement Manipulator）。使用它可自由移动和旋转剖面。该对话框还允许您隐藏操控器，并沿某一轴以增量步长平移或旋转剖面：
1. 选择平移轴或旋转轴。
2. 单击右箭头 或左箭头 步进按钮，以使剖面移动一步——即预定的距离或角度。每单击一次，剖面即沿该轴或旋转角度的正方向或负方向移动，具体取决于您单击的箭头。字段中的数值是剖面自其原点移动的距离。您可以以六个自由度平移和旋转剖面平面。
3. 您可以通过将光标置于 Step Size（步长）超链接上修改平移或旋转的步长。当其形状变为 时，单击以打开 Step Size（步长）对话框。您可以根据需要修改平移步长（以毫米为单位）或旋转步长（以度为单位）。

Section location（剖面位置）区域显示被管理剖面的位置：
4. 在 Graphic Manipulation（图形操控）区域中，您可以进行以下任意操作：
- Placement（放置）—— 系统在剖面体原点处显示位置操控器。您可以平移和旋转剖面。
- Scaling（缩放）—— 系统显示从剖面体每个面和角点延伸出的黄色操控线。拖动面操控器可更改所选面的大小；拖动角点操控器可重新缩放整个剖面体。说明：缩放不适用于剖面平面。
- Shape（形状）—— 当您将指针置于剖面体的某条边上方时，系统会将其以蓝色高亮显示。您可以拖动所选边以重塑剖面体。您只能沿垂直于边本身的方向拖动顶面和底面（为清晰起见以较深颜色显示）的边——这可确保顶面和底面保持平行。您可以沿任意方向拖动其他边，但它们必须保持在顶面和底面的边界之内。说明：形状不适用于剖面平面。说明：您可以同时选中 Placement（放置）、Scaling（缩放）和 Shape（形状）的任意组合。
5. 如果您希望将被管理剖面平面恢复到打开 Section Manager（剖面管理器）对话框时的位置和方向，请单击 Reset（重置）。

Section Manager（剖面管理器）对话框包含一个图标工具栏 ，其命令在 Sections（剖面）中描述如下：
- Activate Section（激活剖面）
- Deactivate Section（停用剖面）
- Flip Section（翻转剖面）
- Section Alignment（剖面对齐）
- Adjust Section Plane Size（调整剖面平面大小）说明：选择剖面体时不活动。
- Preview Section Contours（预览剖面轮廓）

<a id="v4-s29"></a>
#### New Section Viewer（新建剖面查看器）

选择 View 选项卡 → Section 组 → New Section Viewer，为单个剖面平面打开 Section Viewer（剖面查看器）。您可以为同一剖面打开最多五个 Section Viewer（剖面查看器），以便从不同角度查看视图内容。

使用 Section Manager（剖面管理器）对话框在 Section Viewer（剖面查看器）中操控剖面平面（剖面平面本身不显示）。当同时使用 Section Viewer（剖面查看器）和 Graphic Viewer（图形查看器）时，您可以灵活地停用图形查看器中所用的平面，同时仍将其用于在 Section Viewer（剖面查看器）中裁剪或切割视图。此时，您在 Section Viewer（剖面查看器）中所做的修改不会影响图形查看器中的视图。您也可以激活该平面，例如在图形查看器中用它裁剪视图，同时在 Section Viewer（剖面查看器）中切割它。

所有 Measurements（测量）选项均可在 Section Viewer（剖面查看器）中使用。

说明（Note）
您可以在 Section Viewer（剖面查看器）中单击右键以访问上下文菜单，从而快速隐藏剖面、打开新的 Section Viewer（剖面查看器）以及修改剖面（如需要）。

<a id="v4-s30"></a>
### Contours（轮廓）

<a id="v4-s31"></a>
#### Saving Section Contours as a Component（将剖面轮廓另存为组件）

在图形视图中显示对象轮廓时（请参阅 Sections（剖面）中的 Clip Section（裁剪剖面）和 Cut Section（切割剖面）），您可以将剖面轮廓另存为组件（Save section contours as a component）。当 Show Section Contours（显示剖面轮廓）处于激活状态时，该命令将轮廓作为一个所选活动剖面的组件导出。该命令会打开 Save Component As（组件另存为）对话框，允许您将轮廓保存为 .COJT 组件。

<a id="v4-s32"></a>
#### Exporting Contours as .JT File（将轮廓导出为 .JT 文件）

对于所选的切割剖面平面，Export JT（导出 JT）仅将对象轮廓作为 .jt 文件导出。如果在启动 Export JT（导出 JT）命令时未选择任何平面，则所有剖面轮廓将作为单一（monolithic）JT 导出。

<a id="v4-s33"></a>
### Dynamic Clipping（动态裁剪）

动态裁剪（Dynamic Clipping）使您能够查看被中间对象遮挡而无法直接看到的关注点。

在 Process Designer 中，图形查看器中显示的视图是由位于固定点（称为相机视点，camera view point）的外部观察者观察到的视图。位于相机与关注点之间的任何对象都可能部分或完全遮挡视图。动态裁剪（Dynamic Clipping）选项定义了一个可调裁剪平面（clipping plane），用于隐藏相机与裁剪平面之间的所有对象。该裁剪平面始终平行于相机的视平面，跨在裁剪平面上的对象会被裁剪（截断）。

启用动态裁剪的方法：
单击 以设置动态裁剪模式。下图显示了动态裁剪的结果：

再次单击 以禁用动态裁剪。

说明（Note）
当场景中包含已激活的剖面平面时，动态裁剪将被禁用。

<a id="v4-s34"></a>
#### Dynamic Clipping Plane Settings（动态裁剪平面设置）

配置动态裁剪的方法：

过程（Procedure）：
1. 使用 View Center（视图中心）将所需关注点设置为图形查看器显示的中心。
2. 选择 View 选项卡 → Section 组 → Dynamic Clipping Plane Settings。将出现 Dynamic Clipping Plane Settings（动态裁剪平面设置）对话框。
3. 通过以下任一方式定义裁剪平面（Clipping Plane）：
- 在 Camera（相机）与 View Center（视图中心）之间移动 Clipping Plane（裁剪平面）滑块至所需位置。移动滑块时图形查看器会实时更新，便于您以交互方式查看滑块设置的结果。
- 键入一个代表您希望设置裁剪平面位置的数字。该数字是相机与视图中心之间距离的百分比。
- 单击 Reset（重置）以恢复到启动 Dynamic Clipping Plane Settings（动态裁剪平面设置）对话框时的原始裁剪平面值。

<a id="v4-s35"></a>
### Camera（相机）

Camera（相机）对象可用于模拟工作站中的相机系统，有助于从特定视点查看场景。

也可以在 Process Simulate 中创建 New Object Flow Operation（新建对象流操作），并将相机设置为该操作的模拟对象（simulated object）。这样，您可以使相机在您的工作区域内移动，并从设定的角度查看感兴趣的对象和活动。此外，您可以将相机附加到任意对象上，以便相机随对象一起移动（无需创建额外操作），请参阅 Open Camera Viewer（打开相机查看器）和 Attach Camera to Moving Part（将相机附加到运动部件）。

该视频演示了相机的基本功能。

说明（Note）
视频未包含在 PDF 中。要访问视频，请使用 HTML。

<a id="v4-s36"></a>
#### Create Camera（创建相机）

过程（Procedure）：
1. 拾取任意位置或对象。
2. 选择 View 选项卡 → Camera 组 → Create Camera。将创建新相机并放置在拾取位置。您可以显示一个代表相机视场的图形。

<a id="v4-s37"></a>
#### Open Camera Viewer（打开相机查看器）

- 拾取一个相机并选择 View 选项卡 → Camera 组 → Open Camera Viewer。将打开图形查看器的新实例，显示来自该相机的视图。

说明（Note）
- 访问相机查看器时，系统会自动调用透视（perspective）视图模式，请参阅 Parallel/Perspective（平行/透视）。
- 在相机的图形查看器中，所有用于调整视图的命令均被禁用。

<a id="v4-s38"></a>
#### Viewing a simulation in the Camera viewer（在相机查看器中查看仿真）

过程（Procedure）：
1. 选择 View 选项卡 → Camera 组 → Open Camera Viewer。将打开第二个图形查看器用于该相机。您现在既可以从原始视点，也可以从相机视点查看工作区域。
2. 使用 View 选项卡 → Screen Layout 组 → Arrange Windows（排列窗口）命令，在应用程序中方便地排列图形查看器窗口。
3. 右键单击新操作，选择 Home 选项卡 → Operation 组 → Set Current Operation（设为当前操作），或将操作拖入 Sequence Editor（序列编辑器）。
4. 在 Sequence Editor（序列编辑器）中，单击 Play Forward（正向播放）以播放该操作。

<a id="v4-s39"></a>
#### Align Camera with Current View（将相机与当前视图对齐）

如果您希望调整相机视图以显示与当前图形查看器完全相同的视图，请选择 View 选项卡 → Camera 组 → Align Camera with Current View（将相机与当前视图对齐）。

<a id="v4-s40"></a>
#### Add Current View as Camera Location（将当前视图添加为相机位置）

您可以将当前视图添加为以相机为模拟对象的对象流操作中的一个位置。

过程（Procedure）：
1. 选择一个相机。
2. 在 Operation Tree（操作树）中选择要添加相机位置所属的操作。如果单击操作根节点，新位置将添加到操作末尾；如果单击操作中的某个位置，新位置将添加在所选位置之后。说明：如果选择的操作其模拟对象不是相机，系统将显示错误消息。
3. 选择 View 选项卡 → Camera 组 → Add Current View as Camera Location（将当前视图添加为相机位置）。该位置即被添加到操作中。

<a id="v4-s41"></a>
#### Display camera envelope（显示相机包络）

用于可视化相机的视场。

过程（Procedure）：
选择一个或多个相机并选择 Display Camera Envelope（显示相机包络），以显示或隐藏代表相机视场的形状。这有助于您操控相机的位置和方向，以获得所需视图。
- 您可以对多个相机运行此命令。
- 再次运行该命令时，如果当前有一个或多个包络未显示，则会显示所有指定相机的相机包络；如果全部已显示，则会隐藏所有相机包络。
- Display Camera Envelope（显示相机包络）参数来源于 RGBD Snapshot Configuration（RGBD 快照配置）对话框中的 Color (RGB) Camera Configuration（彩色(RGB)相机配置）设置。

完整信息请参阅 Cameras（相机）。

<a id="v4-s42"></a>
#### Camera settings（相机设置）

您可以在 Process Simulate 中使用相机拍摄静态 RGBD 照片，包括 RGB 照片和深度照片。

过程（Procedure）：
1. 选择一个相机并选择 View 选项卡 → Camera 组 → Camera settings（相机设置）命令。
2. 配置以下内容：
- Image Creation Configuration（图像创建配置）—— 用于选择要拍摄的照片类型：RGB、深度（depth）或两者。
- Color (RGB) Camera Configuration（彩色(RGB)相机配置）—— 用于定义 Width（宽度）和 Height（高度）以设置照片的宽高比，并输入 Diagonal Field Of View（对角线视场）的度数以设置相机视场（FOV）的宽度。
- Depth Camera Configuration（深度相机配置）—— 您可以单独为深度照片定义 Diagonal Field Of View（对角线视场）、Height（高度）和 Width（宽度）。Depth Range（深度范围）是相机前方能够确定对象深度的距离范围。您可以定义 Maximum depth range（最大深度范围）和 Minimum depth range（最小深度范围），以配置照片中对象所描绘的距离及其着色方式。处于最小距离的对象着蓝色，处于最大距离的对象着红色；处于这两点之间距离的对象按其距相机的距离以红到蓝的色阶着色。超出此范围（过近或过远）的对象深度无法被检测，将着黑色。
深度快照（depth snapshot）保存为二进制 DAT 文件。
- Save depth image as JPG picture（将深度图像另存为 JPG 图片）—— 若设置此选项，系统还会将深度图像保存为 JPG 图形图像并创建深度直方图。
- Color and Depth Image Alignment Options（彩色与深度图像对齐选项）—— 当 RGB 相机和深度相机的 FOV 不同时，这些设置用于配置两者图像的对齐方式。此时（同一对象的）图像并不重合，您可以选择 Crop the bigger FOV image（裁剪较大 FOV 图像）或 Pad the smaller FOV image with default value（用默认值填充较小 FOV 图像）。默认不执行对齐。以下图示说明了各种可能性：
- Immediate Snapshot（即时快照）—— 单击 Take Snapshot（拍摄快照）将根据您配置的设置拍摄 RGB 或深度照片或两者，并将生成的图像存储在 Select Folder（选择文件夹）中设置的文件夹内。默认情况下为系统根文件夹。如需更改，请单击 。
- 您可以单击 打开相机图像的存储文件夹。
3. 如果您希望为将来使用该相机的会话保存配置更改并将其定义为 RGBD 相机，请单击 Apply（应用）。这会使图形查看器中的相机变为橙色。

说明（Note）
- 如果您配置了 RGBD 设置并单击 Take Snapshot（拍摄快照）（而未单击 Apply（应用）），则在 RGBD Snapshot Configuration（RGBD 快照配置）对话框仍处于打开状态时拍摄的照片符合新设置。但是，该相机不会被指定为 RGBD 相机，下次打开对话框时会恢复默认设置。
- Camera settings（相机设置）对话框配置您使用该相机拍摄的所有静态照片的设置，无论是通过 Take Snapshot（拍摄快照）按钮还是通过 API。

<a id="v4-s43"></a>
#### Using cameras in object flow operations（在对象流操作中使用相机）

本过程介绍如何将相机设置为 Object Flow Operation（对象流操作）的模拟对象（simulated object）。这意味着您可以使相机在工作区域内移动，并从最佳视点观察每个制造工作站/工艺，从而微调您的仿真。使用 Add Current View as Camera Location（将当前视图添加为相机位置）命令，您可以快速选择要使相机穿过的视点，并将其转换为 Object Flow Operation（对象流操作）中的位置。

在 Object Flow Operation（对象流操作）结束时（或在其之前、甚至两个独立操作之间），您可以将相机链接到一个运动对象上（无需使用 Object Flow Operation（对象流操作））。例如，若机器人拾取一个部件，然后沿其导轨移动以将部件送达下一工作站，此时将相机附加到该部件上会使相机随部件一起移动，使您能够持续查看该部件。

过程（Procedure）：
1. 创建相机。
2. 创建新操作，步骤如下：
a. 创建新的 Object Flow Operation（对象流操作）。
b. 将操作的 Object（对象）设置为新相机，然后单击 OK。
c. 删除操作的默认位置。
3. 创建操作位置，步骤如下：
a. 操控图形查看器，使其显示您希望看到的精确视图。
b. 选择 View 选项卡 → Camera 组 → Add Current View as Camera Location（将当前视图添加为相机位置）。一个新位置被添加到新操作中，从该位置相机显示您刚设置的精确视图。
c. 创建所有所需的位置。

<a id="v4-s44"></a>
#### Attach camera to moving part（将相机附加到运动部件）

过程（Procedure）：
1. 选择相机。
2. 选择 Home 选项卡 → Tools 组 → Attachment，并单击 Attach。Attach Objects（附加对象）列表会自动填充该相机。有关此命令的更多信息，请参阅 Attach（附加）。
3. 单击 To Object（目标对象）字段，并选择要将相机附加到的部件。
4. 单击 OK。
<a id="v5-s1"></a>

# 第 5 卷：应用（Applications）

<a id="v5-s2"></a>

## 报价（Quotation）

<a id="v5-s3"></a>

### 创建报价（Creating Quotes）

<a id="v5-s4"></a>

### 报价概述与工作流（Quotation Overview and Workflow）

Quotation（报价）是 Process Designer 的附加模块，用于高效地创建报价。由于系统使用经过长期测试验证的工艺来生成报价，因此报价特别可靠。

Quotation 帮助您利用功能工艺规划（Functional Process Planning，FPP）库中的宝贵数据，快速构建详细、准确的报价。当新报价缺乏完整的产品或工艺信息时，利用过往经验积累的大量信息尤为有用。Quotation 使您能够：

- 基于以往经验和知识，快速高效地响应 RFP（Request for Proposal，提案请求）
- 评估单个报价的多种工艺方案
- 通过复用以往制造项目、成熟制造工艺和现有设施的知识，快速高效地计算总体制造成本
- 根据需求生成报价文档

Quotation 模块包含两个组件：

- FPP 数据建模器（FPP Data Modeler）——管理零件、操作和资源库的对象关系。
- 报价向导（Quote Master）——引导您完成创建详细报价的过程。

工作流包含两个阶段：使用对象属性查看器中的 FPP 选项卡以及 Quotation 模块的 FPP 数据建模器来定义功能工艺数据；使用报价向导（Quote Master），借助功能工艺数据创建报价。

设置过程是持续构建结构化定义的工程：功能产品定义、相关零件的产品模板，以及相关功能和模板操作的库。

例如，在制造计算机的场景下，功能产品结构可能包括硬盘、CD/DVD 驱动器、键盘、鼠标和液晶显示器（LCD）等零件。对于每种零件，存在若干模板：对于硬盘，有某厂商的 40GB 和 80GB 硬盘，以及另一厂商的 80GB 硬盘。此外，还应为每种驱动器类型存在与硬盘和 DVD 安装相关的通用操作模板，以及必要的特殊安装程序。要使 Quotation 有效运行，需假定上述信息已存在。

<a id="v5-s5"></a>

#### 功能工艺设置（Functional Process Setup）

要定义功能性产品工艺和信息，必须使用 FPP（功能工艺规划，Functional Process Planning）数据建模器。结果会反映在各类属性窗口的 FPP 选项卡中。

**零件功能（Part Function）FPP 选项卡**

在此选项卡中，您可以使用下拉菜单定义零件功能的产品类型（product type）和产品子类型（product sub type）。该选项卡还显示与此零件功能相关的零件原型（part prototype）列表。您可以通过将其他零件库中的对象拖放到该列表中来添加零件原型。

**零件原型（Part Prototype）FPP 选项卡**

此选项卡仅包含对功能零件的引用。也可以通过将零件功能拖放到该字段来修改此字段。

> **注意**
>   
> 此选项卡与上述零件功能 FPP 选项卡中的原型列表始终保持同步。该关系可在任一选项卡中创建。

<a id="v5-s6"></a>

### FPP 数据建模器（FPP Data Modeler）

**功能操作（Functional Operation）FPP 选项卡**

此选项卡包含与此功能操作相关的操作模板（operation template）列表。可通过将操作模板拖放到该列表中来修改此列表。

**操作模板（Operation Template）FPP 选项卡**

此选项卡包含一个字段，显示相关的操作模板功能操作（Operation template functional operation）。通过将该相关功能操作拖放到此字段来设置字段值。

> **注意**
>   
> 此选项卡与上述功能操作 FPP 选项卡中的操作模板列表始终保持同步。该关系可在任一选项卡中创建。

功能工艺规划（FPP）数据建模器是用于管理对象关系的实用工具。Process Designer 处理三类对象：零件、操作和资源。每类对象分为三个类别：

- **功能级（Functional Level）**：对象最抽象的概念，是设计新产品或工艺的有用起点。例如"车轮"。
- **模板级（Template Level，也称原型 Prototype）**：对象的特定变体。零件的特定型号属于模板级。例如"17 英寸铝合金车轮"。
- **实际级（Actual Level，也称实例 Instance）**：零件、操作或资源的物理实例。零件序列号属于实际级。例如，一辆汽车可能需要四个 17 英寸铝合金车轮的实际级实例，每个实例都有唯一的零件序列号。

虽然很容易理解零件如何归入这三个概念类别，但操作和资源也按此系统分类。

您必须定义将对象实例与相应模板级原型关联的链接。将模板级原型与匹配的功能级对象分类也是如此。例如，您必须将 17 英寸铝合金车轮的实际实例链接到 17 英寸铝合金车轮模板。类似地，您将各种车轮模板（17 英寸、18 英寸、铝合金、钢制）与相应的功能级对象"车轮"链接。

下图说明了将实际级对象与模板关联、模板与功能级对象关联的链接，同时说明了不同类型对象之间的交互——例如零件与操作。

FPP 数据建模器使您能够创建：

- 功能级和模板级对象
- 功能级与模板级对象之间的关系
- 模板级与实际级对象之间的关系

这些关系对于以下方面必不可少：

- 充分利用 Quotation
- 复用制造工艺

下图显示了 FPP 数据建模器窗口：

FPP 数据建模器窗口列出对象及其链接。左窗格中的每一行列出一个对象。该行中的三个字段显示该对象在三个级别（实例/实际、原型/模板、功能）上的链接。图中，功能级对象 `ResourceFunction*` 同时链接到 `Turn_Table` 和 `Torque_driver` 原型。

空白字段表示该对象没有到该级别的链接。例如，某个特定车轮可能尚未链接到模板级车轮原型，因此其"模板（Template）"字段为空。类似地，功能级车轮对象可能未链接任何模板或实际车轮实例，因此其"模板"和"实例（Instance）"字段为空。

右窗格列出可链接到左窗格对象的对象。例如，您可以在左窗格中列出若干车轮原型，并在右窗格中添加功能级车轮对象。这样您便可以将左窗格中的车轮原型与右窗格中的功能级车轮对象链接起来。

FPP 数据建模器的查找（Find）功能使您能够快速选择要关联的对象，例如新定义的功能级对象。查找功能显著简化了创建对象链接的任务。

<a id="v5-s7"></a>

### 左窗格选项（Left Pane Options）

要打开 FPP 数据建模器窗口：

- 单击工具栏中的相应图标。FPP 数据建模器窗口打开。在 Process Designer 树中选择的任何对象都会显示在 FPP 数据建模器的左窗格中。

要对左窗格或右窗格中对象的显示进行排序：

- 单击列标题（例如"功能（Function）"）以按该列字母顺序排序显示。再次单击可反转排序顺序。列标题中的箭头指示窗格中的对象已按该字段排序，并指示升序或降序。

**向左侧窗格添加对象：**

1. 在 Process Designer 树中选择一个对象。
2. 单击 FPP 数据建模器窗口左侧窗格上方的"添加（Add）"按钮。

**从左侧窗格移除对象：**

1. 在 FPP 数据建模器窗口的左窗格中选择一个对象。
2. 单击左窗格上方的"移除（Remove）"按钮将其移除。

**清除左侧窗格中的所有对象：**

- 单击左窗格上方的"清除（clear）"按钮。

您可以在 FPP 数据建模器中创建新的模板对象和功能级对象。在"设置（Settings）"对话框中设置新对象的存储位置。使用左窗格上方的按钮创建对象。

**设置新建对象的存储位置：**

1. 单击 FPP 数据建模器窗口中的"设置（Settings）"按钮，打开"FPP 建模器设置（FPP Modeler Settings）"对话框。
2. 在"目标位置（Target Locations）"区域，设置放置 FPP 数据建模器中创建对象的文件夹。单击某对象类型的字段以选择该字段。
3. 在 Process Designer 的任意导航树中单击一个文件夹，将该文件夹的名称复制到"FPP 建模器设置"对话框中的所选字段。
4. 重复该过程以设置每种对象类型的搜索位置。

**创建新的模板对象：**

1. 单击相应按钮。左窗格中会出现一个新的模板对象。
   > **注意**：FPP 数据建模器为新对象提供默认名称，例如 `CompoundOperation_Template`。
2. 单击默认名称并输入名称。

**创建新的功能级对象：**

1. 单击相应按钮。左窗格中会出现一个新的功能级对象。
   > **注意**：FPP 数据建模器为新对象提供以星号（*）结尾的默认名称，例如 `ResourceFunction*`。
2. 单击默认名称并输入名称。

与特定功能级对象创建关联的一种高效方式是使用复制/粘贴功能。您可以将一个模板到功能级对象的链接复制，并粘贴到其他模板。这样会将这些模板链接到同一功能级对象。这使您能够快速组织大量新模板对象。例如，如果您在零件库中添加若干新的车轮型号，可以通过复制和粘贴将它们全部与车轮功能级对象快速关联。

**复制关联到功能级对象：**

1. 选择要复制的功能级对象。
2. 单击复制图标。
3. 选择与该功能级对象关联的对象。
4. 单击粘贴图标。所选对象的"功能（Function）"列将被填充。

<a id="v5-s8"></a>

### 右窗格选项（Right Pane Options）

通常对左窗格中对象的显示进行过滤会很有用。例如，左窗格可能同时显示与车轮装配相关的零件和操作。您可能希望先处理车轮零件，然后再处理车轮操作。您可以使用过滤功能先仅显示零件，后再仅显示操作。

**过滤左窗格中对象的显示：**

- 在左窗格上方的"FPP 对象（FPP Objects）"下拉菜单中，选择"零件（Parts）"、"操作（Operations）"、"资源（Resources）"或"全部（all）"。与选择匹配的对象显示在左窗格中。选择"全部"将禁用过滤功能并显示所有添加到左窗格的对象。

**向右侧窗格添加对象：**

1. 在 Process Designer 树中选择一个对象。
2. 单击 FPP 数据建模器窗口右侧窗格上方的"添加（Add）"按钮。

**从右侧窗格移除对象：**

1. 在 FPP 数据建模器的右窗格中选择一个对象。
2. 单击右窗格上方的"移除（Remove）"按钮将其移除。

**清除右侧窗格中的所有对象：**

- 单击右窗格上方的"清除（Clear）"按钮。

您可以使用 FPP 数据建模器的查找（Find）功能搜索符合您所设条件（criteria）的功能级对象。使用 FPP 数据建模器窗口中的"范围搜索（Scope Search）"字段设置搜索范围。在"范围搜索"字段中设置位置是一个两步过程：定义搜索目标位置，然后选择一个位置放入"范围搜索"字段并用于搜索。首先，在"FPP 数据建模器设置"对话框中添加搜索条件（称为搜索方法，search methods）。单击 FPP 数据建模器窗口中的"查找（Find）"图标执行搜索，并在 FPP 数据建模器窗口的右窗格中显示匹配的功能级对象。



要定义每种搜索方法，选择两个要比较的属性：一个原型属性和一个功能级属性。可比较的属性示例包括名称和注释。搜索方法比较这两个属性，并返回符合条件的功能级对象。当功能级属性的文本等于或包含在原型属性的文本中时，属性匹配。

搜索机制在"范围搜索"字段定义的位置中定位功能级对象，并将其与 FPP 数据建模器窗口左窗格中的原型级对象进行比较。

例如，将搜索范围设置为特定项目文件夹，并定义搜索方法以定位名称字段与任何零件原型名称字段匹配的功能级对象。如果 FPP 数据建模器窗口左窗格中有一个零件原型的名称字段中包含"wheel"，则名为 wheel 和 spare wheel 的功能级对象将匹配该搜索方法。单击"查找（Find）"会将 wheel 和 spare wheel 功能级对象放入 FPP 数据建模器窗口的右窗格。

**设置搜索范围：**

1. 单击 FPP 数据建模器窗口中"范围搜索"字段旁边的"浏览（Browse）"按钮。出现"定义范围（Define Scope）"对话框。
2. 在"定义范围"对话框的左窗格中，选择要包含在"搜索目标（Search Targets）"窗格中的位置。
3. 单击相应按钮将所选位置添加到"搜索目标"窗格。在"搜索目标"窗格中选择一个对象，然后单击相应按钮移除某个位置。
4. 在"搜索目标"窗格中，选择一个要放入"范围搜索"字段的位置。
   > **注意**：搜索范围仅由"范围搜索"字段显示的单个文件夹定义，不包括其子文件夹。"搜索目标"窗格中定义的其他位置可用于快速参考以设置范围，但不包含在范围内。
5. 单击"确定（OK）"，将"搜索目标"窗格中所选位置放入 FPP 数据建模器窗口的"范围搜索"字段。这将关闭"定义范围"对话框。

**配置搜索方法：**

1. 单击 FPP 数据建模器窗口中的"设置（Settings）"按钮，打开"FPP 建模器设置"对话框。

**连接对象（Connect）：**

1. 在右窗格中选择一个对象。
2. 在左窗格中选择一个或多个对象。
3. 单击"连接（Connect）"按钮。
   > **注意**：右窗格中所选对象的名称现在会出现在左窗格中所选对象的匹配字段中。例如，如果右窗格中的对象是功能级对象，则会被添加到左窗格中所选对象的"功能（Function）"字段。

<a id="v5-s9"></a>

### 报价向导（Quote Master）

报价向导（Quote Master）是一个引导您完成创建详细报价的向导。其设置过程分为以下步骤。
  
<a id="v5-s10"></a>

#### 开始使用报价向导（Getting Started with Quote Master）

报价向导（Quote Master）是基于功能工艺规划（FPP）项目设计的 Quotation 模块的报价生成向导。它引导您完成以下步骤：

- 第 2 步：产品结构化（Product Structuring）
- 第 3 步：生产参数（Production Parameters）
- 第 4 步：工艺结构化（Process Structuring）
- 第 5 步：资源（Resources）
- 第 6 步：方案摘要（Alternative Summary）
- 第 7 步：报价分析（Quote Analysis）

系统将您使用报价向导创建的所有报价数据存储在项目根节点下的"报价（Quotes）"文件夹中。

> **注意**：报价向导需要进行定制，可从 `empower\apps` 导入。

启动报价向导：

- 选择"应用程序（Applications）"选项卡 → "报价（Quotation）"组 → "报价向导（Quote Master）"。

将显示报价向导的第一步。

<a id="v5-s11"></a>

#### 第 1 步：报价参数（Step 1: Quote Parameters）

在第 1 步"报价参数（Quote Parameters）"中，您输入基本产品信息以及报价预测所包含的年份范围。在此步骤中，您还可以将任何相关文档作为文件附件附加到报价。

下图是显示"报价参数"步骤的报价向导起始屏幕。

使用报价向导创建新报价：

- 系统在"报价名称（Quote Name）"字段中自动提供一个新的报价名称，称为 `Quote<附加递增数字>`。或者，您可以输入一个唯一名称。

使用报价向导编辑现有报价：

1. 单击"报价名称"字段中的相应控件以打开下拉菜单，显示现有报价。
2. 从当前项目中先前创建的报价列表中选择一个报价。
   > **注意**：一旦选择已保存的报价，该报价的参数将显示在报价向导的字段中。

设置报价参数字段：

1. "引用（Reference）"字段使您能够附加与报价相关的任何文档，可包括文本文件、图像等。单击"附加（Attach）"按钮将文件保存为报价的附件。在"引用"字段中选择一个附件，然后单击"移除（Remove）"按钮以移除已附加的引用文件。附加的文件列在"引用"字段中。
2. 在"注释（Comment）"字段中，输入报价的任何类型的信息，例如描述。
3. 在"客户（Customer）"字段中输入客户名称。
4. 在"报价年份（Quote Years）"列表中，选择要包含在报价中的年份。向导显示接下来的 20 年，默认仅选中显示年份中的第一年。
   > **注意**：如果选择两个不连续的年份，所有中间年份将自动包含。无法取消选择范围中间的年份。例如，如果选择 2006–2009 年范围，则无法取消选择 2007 年。
5. 从下拉菜单中选择"产品类型（Product Type）"和"子类型（Sub Type）"。这些选择充当过滤器，决定在向导的下一步中显示哪些零件原型。仅显示与产品相关的零件原型。产品类型和子类型在名为 `ListOfValuesExtension.xml` 的 XML 文件中定义。更多信息，请参阅"值列表扩展文件（List of Values Extension file）"一节。
6. 在"报价属性（Quote Attributes）"区域，为任何显示的报价属性设置值。这些属性由管理员在系统级别配置。

进入向导的下一步：

完成此步骤后，您可以：

- 单击"下一步（Next）"保存信息并前进到下一步。
- 单击"取消（Cancel）"退出报价向导。
- 单击"完成（Finish）"保存此步骤中输入的信息并退出报价向导。

<a id="v5-s12"></a>

#### 第 2 步：产品结构化（Step 2: Product Structuring）

在"产品结构化"步骤中，您创建 BOM（物料清单），选择要包含在产品中的零件原型。第 1 步中选择的"产品类型"和"子类型"字段决定哪些原型可用于包含在 BOM 中。"可用零件原型（Available Part Prototypes）"窗格将这些原型显示为导航树。

下图是报价向导的"产品结构化"步骤。

构建 BOM：

1. 从"可用零件原型"窗格的导航树中，选择一个要添加到 BOM 的零件原型。如果所选零件原型存在图像，该图像将显示在"预览（Preview）"窗格中。
   > **注意**：图像由原型"物理（Physical）"选项卡中的"图像文件（Image File）"属性定义。
2. 单击"添加（Add）"将所选零件包含在产品 BOM 中。选择一个零件并单击"移除（Remove）"或"移除零件（Remove Parts）"以从 BOM 中删除对象。
3. 单击"添加零件（Add Parts）"以包含 Process Designer 树（报价向导窗口之外）中所选的零件原型、零件实例或复合零件。
4. 重复该过程，直到您已将所需的所有零件添加到产品 BOM。
   > **注意**：要按字母顺序对产品 BOM 列表排序，请单击"列表（List）"表标题。
5. 对于 BOM 中的每个零件，您可以：
   - 单击"自制/外购（Make/Buy）"字段并从下拉菜单中选择一个选项，将参数调整为"自制（Make）"或"外购（Buy）"。此字段的选项在"值列表文件（List of Values file）"一节中设置。这表示该零件是内部生产还是从外部来源采购。
   - 单击"价格（Price）"字段设置其价格。
   - 单击"属性（Attribute）"字段调整其值。

完成此步骤后，您可以：

- 单击"下一步（Next）"保存信息并前进到下一步。
- 单击"上一步（Back）"返回第 1 步：报价参数。
- 单击"取消（Cancel）"保存当前步骤之前的所有步骤并退出报价向导。
- 单击"完成（Finish）"保存信息并退出报价向导。

<a id="v5-s13"></a>

#### 第 3 步：生产参数（Step 3: Production Parameters）

前两步中输入的信息对于每个生产年份是恒定的。第 3 步使您能够基于前两步输入的恒定信息以及在此步骤中输入的可变信息，生成多种备选报价。备选报价使您能够尝试不同的制造参数并比较结果，从而形成可靠的报价。此步骤的独特性如下：

- 输入的制造信息可按年份变化。
- 您可以创建多种备选报价，并为每个备选报价改变在此步骤中输入的所有制造参数。

在此步骤的"备选名称（Alternative Name）"字段中，为您正在创建的备选报价指定名称。如果您从下拉列表中选择先前定义的备选报价，该报价的参数将显示在页面上，使您能够编辑并调整该备选报价。

下图是报价向导的"生产参数"步骤。

配置备选报价：

1. 在"备选名称"字段中，输入名称以创建新的备选报价，或从下拉列表中选择先前定义的备选报价名称。如果在下拉列表中选择"全部（all）"，则您在此步骤中输入的参数值将应用于所有备选报价。
   > **注意**：选择现有的备选报价名称将显示该备选报价的参数值。
2. 从"年份（Year）"下拉菜单中，选择"全部"以将您在此步骤中输入的信息应用于所有生产年份。或者，从菜单中选择一个年份，以将您在此步骤中输入的信息分配给特定的生产年份。
   > **注意**：从菜单中选择年份将显示该年份先前输入的任何参数值。如果参数的值因年份而异，则在"年份"菜单中选择"全部"将在参数的字段中显示"多值（Multiple Values）"一词。如果在向导的第一步中仅选择了一个生产年份，则"年份"下拉列表将被禁用并显示"全部"。
3. 对于所选的备选报价和年份，输入参数字段的值。
4. 对于所选的备选报价，输入"报价备选属性（Quote Alternative Attributes）"表中列出的属性值。
   > **注意**：输入无效的属性值（例如，在需要数值的字段中输入字母）会导致显示错误消息。该字段的先前值将被保留。

完成此步骤后，您可以：

- 单击"下一步（Next）"保存信息并前进到第 4 步：工艺结构化。
- 单击"上一步（Back）"返回第 2 步：产品结构化。
- 单击"取消（Cancel）"保存当前步骤之前的所有步骤并退出报价向导。
- 单击"完成（Finish）"保存信息并退出报价向导。

下表描述了"生产参数"步骤中的字段。

| 字段                               | 必填/可选 | 说明                                                                                 |
| -------------------------------- | ----- | ---------------------------------------------------------------------------------- |
| 年数量（Annual Quantity）             | 必填    | 在一个生产年度内制造并达到质量目标的产品数量。此字段用于"报价分析"步骤的计算。                                           |
| 合格率 %（Yield Factor %）            | 必填    | 合格率为达到质量目标的产品百分比。用于报价分析计算，默认值为 100%。例如，如果目标产量为 1000 件，合格率为 50%，则必须制造 2000 件才能达到目标。 |
| 每年工作天数（Working Days per Year）    | 必填    | 表示每年的工作天数。                                                                         |
| 每个工作日的班次（Shifts per Working Day） | 必填    | 表示每个工作日的班次数。                                                                       |
| 每班小时数（Hours per Shift）           | 必填    | 表示每班的小时数。                                                                          |
| 能源单位成本（Energy Unit Cost）         | 可选    | 表示一个能源单位的成本。                                                                       |
| 年度空间单位成本（Annual Space Unit Cost） | 可选    | 表示制造过程所需的每单位空间的年度成本。                                                               |
| 附加成本（Additional Costs）           | 可选    | 表示生产过程中产生的附加成本。                                                                    |

<a id="v5-s14"></a>

#### 第 4 步：工艺结构化（Step 4: Process Structuring）

在此步骤中，您选择要包含在报价分析中的操作模板。选择操作模板的起点是向导第 2 步创建的产品 BOM。此步骤显示与产品 BOM 中每个对象相关的操作模板。例如，对象"硬盘"可能具有相关的操作模板"安装硬盘"。您选择将这些操作中的哪些包含在报价分析中。您还可以包含与 BOM 中对象无关的操作模板。这就是工艺结构化。

下图是报价向导的"工艺结构化"步骤。

构建工艺：

1. 在"产品 BOM"窗格中选择一个对象。与该对象相关的操作模板显示在窗口下部的"可用操作模板（Available Operation Templates）"窗格中。
2. 从操作模板列表中，选择您所需的模板，并单击"添加（Add）"将其包含在"工艺操作（Process Operations）"表中。选择一个模板并单击"移除（Remove）"或"移除操作（Remove Operations）"以从"工艺操作"窗格中删除对象。
3. 单击"添加操作（Add Operations）"以包含 Process Designer 树（报价向导窗口之外）中所选的操作。
   > **注意**：这使您能够添加与 BOM 中对象无关的操作实例和操作模板的副本。尝试添加非操作的对象将触发错误消息。
4. 重复此过程，直到工艺所需的所有操作都显示在"工艺操作"窗格中。
   > **注意**：要按字母顺序对"工艺操作"窗格排序，请单击"操作（Operation）"表标题。

完成此步骤后，您可以：

- 单击"下一步（Next）"保存信息并前进到下一步。
- 单击"上一步（Back）"返回第 3 步：生产参数。
- 单击"取消（Cancel）"保存当前步骤之前的所有步骤并退出报价向导。
- 单击"完成（Finish）"保存信息并退出报价向导。

<a id="v5-s15"></a>

#### 第 5 步：资源（Step 5: Resources）

在"资源"步骤中，您查看并调整与每个操作相关的生产资源的参数。此步骤显示一个名为"报价备选资源（Quote Alternative Resources）"的表。该表列出上一步"工艺操作"表中显示的每个操作。在每个操作下，它列出相关的生产资源。资源可包括例如输送系统（conveying systems）和人力（human labor）资源。

您可以选中"资源选择模式（Resource Selection Mode）"以选择要连接到该操作的那些资源和工具原型，或清除它以仅显示已连接到该操作的那些资源和工具原型。在"资源选择模式"下，您可以选中或清除每个资源左侧的复选框。如果"资源选择模式"关闭，则"报价备选资源"表仅显示已通过选中的灰色复选框连接到该操作的资源。"资源选择模式"设置由报价向导为每个用户存储。

> **注意**：移除资源时，如果该资源已分配给模板操作，它仍会以空复选框保留在"报价备选资源"表中。

每个资源都有一个来源（origin）参数，设置为"自制（Make）"、"外购（Buy）"或"租赁（Rent）"。此外，该表为向导第一步中选择的每个生产年份包含一列。在每一年的列中，该表列出制造过程在该年使用的每个资源容量的占比。例如，生产的第一年可能需要输送资源的全部容量。该表在该年的列中列出该资源为 100%。在下个生产年，该过程可能仅需要该输送资源容量的一半。该表在第二年的列中列出该资源为 50%。

下图是报价向导的"资源"步骤。

向操作添加资源：

1. 在 Process Designer 的任何查看器窗口中选择一个对象。
2. 在报价向导中单击"添加资源（Add Resources）"。
   > **注意**：要按字母顺序对资源列表排序，请单击"资源（Resource）"表标题。

从"报价备选资源"表中移除资源：

1. 选择表中显示要移除资源的行。
2. 单击"移除资源（Remove Resources）"。

在"资源选择模式"下连接或断开资源与操作的连接：

- 选中或清除"报价备选资源"表中相关资源的复选框。

调整资源属性值：

- 对于"报价备选资源"表中列出的每个资源，单击一个字段以调整参数的值。必须选中该资源才能调整其值。
  - 单击"自制/外购（Make/Buy）"字段并从下拉菜单中选择一个选项，将值调整为"自制（Make）"、"外购（Buy）"或"租赁（Rent）"。
  - 单击生产年份列中的"%（百分比）"值，以调整该资源在该生产年份的利用率百分比。

进入"方案摘要"步骤：

- 完成此步骤后，您可以：
  - 单击"下一步（Next）"保存信息并前进到下一步。
  - 单击"上一步（Back）"返回第 4 步：工艺结构化。
  - 单击"取消（Cancel）"保存当前步骤之前的所有步骤并退出报价向导。
  - 单击"完成（Finish）"保存信息并退出报价向导。

<a id="v5-s16"></a>

#### 第 6 步：方案摘要（Step 6: Alternative Summary）

"方案摘要"显示当前在"备选（Alternative）"下拉列表中选择的报价的零件、操作和资源的属性。这使您能够更改对象类的属性值。更新后的属性用于报价计算。

此外，管理员可以定义为每个对象类显示的零件、操作和资源的属性。

下图是报价向导的"方案摘要"步骤。

编辑报价备选的属性：

1. 从"备选"下拉列表中，选择要编辑其属性的报价备选。方案摘要表显示您所选备选的零件、操作和资源。
2. 单击要编辑其属性的零件、操作或资源。方案摘要表显示您所选择对象的属性。零件、操作和资源具有不同的属性类型。
3. 单击一个属性，并从出现的下拉列表中选择一个新值。
   > **注意**：要按字母顺序对方案摘要列表排序，请单击"标题（Caption）"表标题。

为对象类定义零件、操作和资源的属性（仅限管理员）：

1. 单击相应按钮。"属性编辑器（Attribute Editor）"对话框出现。
   > **注意**：如果方案摘要表中有未保存的更改，系统会提示您在属性编辑器打开之前保存或放弃这些更改。非管理员用户可以查看属性编辑器，但无法做任何更改。在这种情况下，属性编辑器在没有"确定（OK）"按钮的情况下打开。
2. "类（Class）"列以层次结构列出零件、操作和资源的属性。按以下步骤选择属性：
   - 在"类"列表中选择一个属性，并单击相应按钮将其添加到"属性（Attributes）"列表。
   - 单击相应按钮将所有可能的属性添加到"属性"列表。
   - 在"属性"列表中选择一个属性，并单击相应按钮将其移除。
   - 单击相应按钮从"属性"列表中移除所有属性。
3. 在"属性"列表中选择一个属性，并单击相应按钮将其在列表中上移，单击相应按钮将其下移。
4. 单击"确定（OK）"保存更改。当您在方案摘要中单击零件、操作或资源时，它将与其在属性编辑器中定义的属性一起显示。

进入"报价分析"步骤：

完成此步骤后，您可以：

- 单击"下一步（Next）"保存信息并前进到下一步。
- 单击"上一步（Back）"返回第 5 步：资源。
- 单击"取消（Cancel）"保存当前步骤之前的所有步骤并退出报价向导。
- 单击"完成（Finish）"保存信息并退出报价向导。

<a id="v5-s17"></a>

#### 第 7 步：报价分析（Step 7: Quote Analysis）

最后一步使您能够分析向导生成的报价备选。

下图是报价向导的"报价分析"步骤。

选择报价备选：

- 单击页面顶部的"报价备选分析（Quote Alternative Analysis）"下拉列表，以通过第 3 步：生产参数中给定的名称选择所需的报价备选。

对于所选的报价备选，向导显示一个表格，其中包含资源、能源、空间、人工和附加成本的摘要，按生产年份显示。

生成所选报告：

- 在"报价文档（Quote Documents）"区域，单击"Excel"或"BOP 管理器（BOP Manager）"报告选项之一，以生成特定格式的表格报告。

报告包含向导最后一步表中显示的成本摘要。此外，报告提供大量对于创建准确的生产报价很有价值的生产信息，包括：

- 制造过程所需的空间（基于操作模板数据）。
- 处理时间。
- 用户定义属性的审查。
- BOM 的显示，列出每个零件的来源。
- 资源利用数据的审查。

Excel 格式报告示例

完成向导的步骤后，您可以：

- 单击"上一步（Back）"返回第 5 步：资源。
- 单击"重新启动（Restart）"返回第 1 步：报价参数，并选中当前报价名称。
- 单击"完成（Finish）"保存信息并退出报价向导。
  > **注意**：此步骤中"取消（Cancel）"按钮被禁用。

Excel 文件模板 `AlternativeTemplate.xls` 位于为 eMServer 定义的系统根目录的"常规（General）"文件夹中。您可以根据需要替换或编辑此模板。

<a id="v5-s18"></a>

### 附录 A（Appendix A）

<a id="v5-s19"></a>

#### 值列表扩展文件（List of Values Extension file）

`ListsOfValuesExtension.xml` 是一个文件，您必须向其添加有关产品类型和子类型的信息，以供第 1 步使用。以下是添加到该 XML 文件的示例。此示例使您能够支持两种产品类型：Computer 和 Video，以及每种产品类型的两个子类型；Computer 的子类型为 Desktop 和 Laptop，Video 的子类型为 VCR 和 DVD。

```xml
<QMEnumerationField FieldName="ProductType">
  <Type data="1" DisplayedName="Computer">
    <SubType data="2" DisplayedName="Laptop"/>
  </Type>
  <Type data="2" DisplayedName="Video">
    <SubType data="1" DisplayedName="VCR"/>
    <SubType data="2" DisplayedName="DVD"/>
  </Type>
</QMEnumerationField>
```

<a id="v5-s20"></a>

### 附录 B（Appendix B）

<a id="v5-s21"></a>

#### 值列表文件（List of Values file）

`ListOfValues.xml` 是一个文件，您可以向其中添加"自制/外购（Make or Buy）"属性的更多选项，以供报价向导第 2 步使用。当您在向导中单击对象的"自制/外购"属性字段时，这些选项会显示在下拉菜单中。

以下是添加到该 XML 文件以定义三种不同对象类型（零件、资源、工具原型）的"自制/外购"属性选项的三段代码示例。在此 XML 文件中，您可以调整所提供的选项名称，或移除或添加选项。

零件值列表：

```xml
<EnumerationField FieldName="makeOrBuy" ClassName="PmPart">
  <Content data="0" DisplayedName="Buy"/>
  <Content data="1" DisplayedName="Make"/>
  <Content data="2" DisplayedName="Rent"/>
</EnumerationField>
```

资源值列表：

```xml
<EnumerationField FieldName="makeOrBuy" ClassName="PmUsage">
  <Content data="0" DisplayedName="Buy"/>
  <Content data="1" DisplayedName="Make"/>
  <Content data="2" DisplayedName="Rent"/>
</EnumerationField>
```

工具原型值列表：

```xml
<EnumerationField FieldName="makeOrBuy" ClassName="PmToolPrototypeUsage">
  <Content data="0" DisplayedName="Buy"/>
  <Content data="1" DisplayedName="Make"/>
  <Content data="2" DisplayedName="Rent"/>
</EnumerationField>
```

<a id="v5-s22"></a>

## 主定位点管理器（PLP Manager）

PLP（主定位点，Principal Locating Points）是一种制造特征（manufacturing feature）。它们指示约束装置（例如夹具 clamps）将连接到零件上的位置，以防止零件在焊接或装配操作期间移动。设计人员将 PLP 分配给定位操作（Locate operations），由这些操作将约束装置固定到零件上。为了帮助约束任何来料零件的六个自由度，您可以从"应用程序（Applications）"功能区选项卡打开 PLP Manager 查看器，或通过右键单击某个工艺（例如 `PRStationProcess` 或 `CompoundOperation`）并选择"应用程序"选项卡 → "PLP"组 → "PLP Manager"（在 Customize 中创建此上下文菜单）。

> **注意**：启用 PLP Manager 需要额外的许可证（PRD_PLP_MANAGER）。

PLP Manager 包含三个部分：

- **来料零件（Incoming Parts）列表**——针对所选工位操作。该表列出所有来料零件。系统分析每个零件，以确定在每个方向上约束它的控制方向数量。使用 X、Y、Z 列检查每个零件是否实施了 3-2-1 规则。为了正确地约束零件使其不在任何方向上移动，该规则规定以下 PLP 组合：
  - 在一个轴（例如 X）上约束的 3 个主控制方向
  - 在第二个轴（例如 Y）上约束的 2 个次控制方向
  - 在第三个轴（例如 Z）上约束的 1 个第三控制方向
      
    当零件具有正确的 3-2-1 平衡时，该表会显示绿色对勾和"OK"。
- **已分配 PLP（Assigned PLPs）列表**——针对"来料零件"列表中所选的零件，此列表显示分配给该零件的 PLP。该列表中的列为：
  - **F/A-** 根据使用（usage）上的 F/A (X) 控制方向值包含真/假值
  - **CC-**（类似于 F/A）用于使用上的 CC (Y) 控制方向字段
  - **U/D-**（类似于 F/A）用于使用上的 U/D (Z) 控制方向字段
  - **X/Y/Z-** PLP 的 X/Y/Z 坐标
      
    您可以将已分配的 PLP 拖放到的树上，例如 PLP 库、定位操作的 PLP 选项卡等。
- **PLP 一致性指示（PLP Consistency Indication）**——针对"来料零件"列表中所选的零件/装配，此网格显示 PLP：
  - 连接到该零件及其后代
  - 分配给该操作或其后代
      
    每个轴象限显示在该轴上约束该零件的 PLP。
- **取消分配（Unassign）按钮**——仅从所选工位移除"已分配 PLP"列表中所选 PLP 的分配，以调整过约束。
- **全部同步（Synchronize All）按钮**——原始属性（例如控制方向）已更改的 PLP 使用以红色显示。需要使用"全部同步"按钮用最新属性更新它们。

<a id="v5-s23"></a>

## 焊点平衡（Weld Balancing）

焊点平衡（Weld Balancing）章节介绍焊点分析、自动零件分配以及自定义列等功能。

<a id="v5-s24"></a>

### 焊点分析（Weld Analysis）

焊点分析（Weld Analysis）选项使您能够分析焊接信息，例如焊点数量、动作时间（action time）和分配时间（allocated time）。

<a id="v5-s25"></a>

### 自动零件分配（Automatic Part Assignment）

自动零件分配（Automatic Parts Assignment）命令使您能够自动将大量焊点分配到零件，无需手动分配每个焊点。该选项通过搜索并显示位于焊点可配置距离内的所有零件来简化过程。然后您可以根据需要确认或拒绝分配。

> **注意**：分配零件时，会执行强制签出（forced check-out）。

自动分配焊点零件：

1. 从图形查看器（Graphic Viewer）、制造树（Mfg Tree）或对象查看器（Object Viewer）中选择焊点。
2. 选择"研究（Study）"选项卡 → "焊接（Weld）"组 → "自动零件分配（Automatic Parts Assignment）"。
     
   显示"自动零件分配"对话框，其中包含已分配给所选焊点的所有零件列表（如果有）。

"自动零件分配"对话框工具栏中提供以下按钮：

| 按钮                           | 名称                                                                                                                                                                                                                                                                                                            | 说明 |
| ---------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -- |
| 搜索（Search）                   | 查找位于所选焊点范围（在"设置"对话框中定义）内的所有零件，并将其显示在表中。尚未分配的零件以灰色斜体显示。                                                                                                                                                                                                                                                        |    |
| 将零件移到开头（Shift Part to Start） | 将列表中的零件移到第一个位置。                                                                                                                                                                                                                                                                                               |    |
| 将零件左移（Shift Part Left）       | 将列表中的零件左移。                                                                                                                                                                                                                                                                                                    |    |
| 将零件右移（Shift Part Right）      | 将列表中的零件右移。                                                                                                                                                                                                                                                                                                    |    |
| 将零件移到末尾（Shift Part to End）   | 将列表中的零件移到末尾位置。                                                                                                                                                                                                                                                                                                |    |
| 排序（Sort）                     | 按字母升序对表中的零件排序。                                                                                                                                                                                                                                                                                                |    |
| 移除零件（Remove Part）            | 从列表中移除零件，即使它已被分配。                                                                                                                                                                                                                                                                                             |    |
| 设置（Settings）                 | 打开"设置"对话框，包含以下设置：  
• **距离（Distance）**：定义零件搜索范围的半径长度（基于"单位（Units）"选项卡中定义的激活单位）。位于焊点该距离内的任何零件都会自动分配给该焊点。  
• **可见列（Visible Columns）**：定义表中可见列数，最多可显示 8 列。显示的前四列是实际要分配的零件列。附加列以黄色显示，仅是分配的候选。要分配这些零件，必须将其列移到表中前四列的位置之内。  
• **接受更改后自动签入（Automatically check-in after accepting changes）**：选中此复选框可在接受更改后自动执行签入操作。 |    |
| 分配（Assign）                   | 将表中选定的焊点分配零件。                                                                                                                                                                                                                                                                                                 |    |
| 导出到 Excel（Export to Excel）   | 将列表导出到 CSV 文件，可用 Microsoft Excel 查看。                                                                                                                                                                                                                                                                          |    |
| 过滤已分配（Filter Out Assigned）   | 选中时，从列表中过滤掉已分配零件的焊点。                                                                                                                                                                                                                                                                                          |    |

1. 选择相应按钮，让系统查找位于所选焊点范围内的所有零件并将其显示在表中。
2. 查看零件列表，选择相应按钮以分配零件，或单击相应按钮以从列表中移除零件。要一次选择多个零件，请单击并按下键盘上的 Shift 或 Ctrl。当列表中选择了一个焊点时，范围内的每个零件都以不同颜色显示在列表和图形查看器中。
   > **提示**：单击相应按钮以从列表中过滤掉已分配零件的焊点。
3. 如果需要，选择相应按钮以修改分配给焊点的零件顺序。默认情况下，"零件 1（Part 1）"列中列出的零件被定义为附加零件（attached part，也称主导零件 leading part）。这是焊点实际附加到的零件。如果在图形查看器中移动此零件，焊点会相对于它移动。
4. 如果您在未保存的情况下关闭"自动零件分配"功能，将显示相应的对话框。
   > **提示**：要选择其他零件作为附加零件，请单击"附加到（Attach To）"列，并从显示的下拉列表中选择一个零件。

<a id="v5-s26"></a>

### 自定义列（Customizing Columns）

可以添加自定义列以显示有关焊点的信息。您可以在定制文件中添加要显示在列中的焊点属性名称，并指定如何显示信息。以下是用于执行此定制的 XML 文件的语法和示例。

```xml
<?xml version="1.0" encoding="utf-8"?>
<TecnomatixAutoPartAssignmentCnfg>
  <columnConfig Header="Attributes"> 
    <attribute fieldName="location"></attribute> 
    <attribute fieldName="name"></attribute> 
    <displayFormat format="{0},{1}"></displayFormat> 
  </columnConfig>
  <columnConfig Header="Attributes2"> 
    <attribute fieldName="comment"></attribute>
    <attribute fieldName="rotation"></attribute>
    <displayFormat format="{0},{1}"></displayFormat>
  </columnConfig>
</TecnomatixAutoPartAssignmentCnfg>
```

示例：

```xml
<?xml version="1.0" encoding="utf-8"?>
<TecnomatixAutoPartAssignmentCnfg>
  <columnConfig Header="Attributes">
    <attribute fieldName="PSAToleA"/>
    <attribute fieldName="PSAToleB"/>
    <attribute fieldName="PSAToleC"/>
    <attribute fieldName="PSAToleD"/>
    <displayFormat format="{0} , {1} , {2} , {3}"/>
  </columnConfig>
</TecnomatixAutoPartAssignmentCnfg>
```

<a id="v5-s27"></a>

## 自动焊点分布（Automatic Weld Point Distribution）

自动焊点分布（Automatic Weld Point Distribution）工具可在项目的预规划阶段自动定义初始焊点分布。

<a id="v5-s28"></a>

### 自动分布焊点的工具（Tool to Distribute Weld Points Automatically）

自动焊点分布工具使您能够在项目预规划阶段自动定义初始焊点分布的过程。

自动焊点分布算法考虑工位物料流（stations material flow），并在满足以下条件时将焊点分配给特定操作：

- 所有消耗的零件都存在于该操作工位
- 至少一个消耗的零件直接分配在该操作工位。如果不存在这样的零件，算法会将焊点分配给消耗该焊点所操作的所有零件的第一个操作
- 焊点尚未手动分配给另一个操作，除非它是焊点位置（Weld Location）操作

自动焊点分布工具还提供一致性检查（consistency check）算法。此功能突出显示过程中所有无效的焊点（可能是手动创建的）。

<a id="v5-s29"></a>

### 运行自动焊点分布（Running Automatic Weld Point Distribution）

要分布焊点：

1. 在 Process Designer 项目窗口中，选择一个 Twin 对象。您可以选择工艺或工艺资源。
2. 选择"应用程序"选项卡 → "焊接（Weld）"组 → "自动焊点分布（Automatic Weld Point Distribution）"。
     
   显示"自动焊点分布"窗口，其中包含焊点分布结果。

下表描述"自动焊点分布"窗口中的信息以及从其工具栏可用的操作：

| 字段/名称          | 说明                                          |
| -------------- | ------------------------------------------- |
| 焊点（Weld Point） | 焊点的名称。窗口中的每一行代表一个焊点。                        |
| 子类型（Subtype）   | 焊点子类型。可能的值有：Dummy、Geo、Respot，以及定制中出现的其他子类型。 |
| 状态（Status）     | 指示焊点是否附加到操作。该字段为空或具有值 Connected。            |
| 操作（Operation）  | 焊点分配到的操作。如果焊点分配给多个操作，由用户负责做出最终分配决定。         |

窗口工具栏中的操作：

- 接受焊点分配并将焊点连接到操作。连接后，连接以粗体显示。
- 从焊点行中移除建议的操作（不将操作连接到焊点）。
- 重新计算焊点分布。
- 将焊点分布结果导出到 Excel 工作表进行分析。
- 打开"设置"对话框窗口。从此窗口，您可以选择操作类型。
- 将显示的所有焊点连接到网格中的操作。

> **注意**：在单击"全部应用（Apply All）"之前，请确保删除不需要的操作，以避免将焊点连接到多个操作。

> **注意**：如果自动焊点分布无法为焊点找到有效操作，它会以红色显示该焊点；如果它指示该焊点应分配给上一个或下一个范围（即执行自动焊点分布所在范围的相邻范围），则会以橙色显示该焊点。

1. 根据需要接受和拒绝焊点分配。
   > **注意**："属性"对话框中的"工艺（Process）"选项卡以图形方式显示焊点与其分配操作之间的连接。
2. 单击"关闭（Close）"关闭"自动焊点分布"窗口。
   > **注意**：如果应用程序无法找到相关焊点，网格为空。

<a id="v5-s30"></a>

### 对自动焊点分布运行一致性检查（Running a Consistency Check on Automatic Weld Point Distribution）

要运行一致性检查：

1. 在 Process Designer 项目窗口中，选择一个 Twin 对象。您可以选择工艺或工艺资源。
2. 选择"应用程序"选项卡 → "焊接（Weld）"组 → "焊点一致性检查（Weld Points Consistency Check）"。
     
   出现"焊点一致性检查"窗口，显示所有连接无效的焊点。
3. 该窗口中的信息和可用操作与"自动焊点分布（Automatic Weld Points Distribution）"窗口相同，但有一处例外：连接（Connect）图标替换为断开连接（Disconnect）图标。选择一个焊点并单击相应按钮以移除连接。断开连接的焊点以粗体显示。
4. 单击"关闭（Close）"关闭"自动焊点分布"窗口。
   > **注意**：如果应用程序无法找到相关焊点，网格为空。
   >   
   > <a id="v5-s31"></a>

## 生产线平衡（Line Balancing）

生产线平衡（Line Balancing）由手动和自动两部分组成，两部分使用相同的数据库并集成于一个应用（Application）中。其目标是将操作分配到资源，以实现整条生产线的合理平衡。

<a id="v5-s32"></a>

### 生产线平衡概述（Overview of Line Balancing）

生产线平衡解决方案由手动和自动生产线平衡部分组成。两部分使用相同的数据库，并组合在一个应用中。

生产线平衡应用（LB）的目的是将操作分配给资源，以在整个生产线上实现适当的平衡。这种适当的平衡消除了生产线中的瓶颈，并最大限度地减少了工位的空闲时间。

LB 考虑了诸如操作优先顺序（operation precedence）和节拍时间（cycle times）等约束，以及额外的平衡方面，例如在工位放置料箱所需的物流空间（logistic space）。

LB 必须遵循以下一组规则运行：

- **资源的可用性**——某些操作无法由所有资源执行，原因包括标准不符、工作含量（work content）或数值内容（numeric content）不匹配。
- **操作的优先顺序**——必须遵循的操作序列（例如，在关闭盖子之前，必须先安装盖子）。
- **操作间约束**：
  - 必须作为一组在同一工位执行的某些操作
  - 必须同一工位并行执行的一对双操作（dual operations）
  - 与另一组操作分离、因此不能在同一工位（与该分离集合）执行的一个或多个操作
  - 必须专用于特定工位的操作

要使用 LB，生产线中的工位必须位于工艺资源（process resource）中。工艺资源和相关操作列表必须作为快捷方式放置在 `LineBalancingScope` 文件夹下，如"生产线平衡范围定义（Line Balancing Scope Definition）"中所述。

LB 在平衡过程中考虑以下节点类型：

- 操作（标记为 `nonBalancing`、`outOfTheLine` 或 `toBeDel` 的除外）
- 工位
- 工具实例（前提是它们既是工位的子节点又是激活的）

自动生产线平衡（ALB）求解器（solver）和图表仅考虑与其在"生产线平衡设置（Line Balancing Settings）"对话框中指定的工位相匹配的 `PmProcessResources`。

<a id="v5-s33"></a>

### 缩写/术语表/定义（Abbreviations/Glossary/Definitions）

- **激活资源（Active Resource）**——位于工位下一个或多个层级中的资源。此类资源被 LB 应用视为增加工作含量的资源。资源是否激活由实例"常规（General）"选项卡中的"激活（Active）"复选框指示。
- **ALB**——自动生产线平衡（Automatic Line Balancing）；由 LB 中的工具栏按钮激活。
- **ALB 会话（ALB session）**——用户针对特定 `LineBalancingScope` 对象、使用特定 ALB 参数设置打开应用后所进行的一组活动。
- **可分配操作列表（Assignable Operations list）**——显示所有可在不违反任何约束的情况下分配给所选工位的操作。
- **已分配操作视图（Assigned Operations View）**——工位操作（Station Operations）选项卡；显示分配给所选资源的所有操作。
- **原子变体（Atomic Variant）**——仅由单一标准组成的变体集。原子变体用于 LB 计算和变体数据显示。
- **图表视图（Chart View）**——通过条形图显示所选工位（或所有工位）的工作含量数据；由 LB 中的工具栏按钮激活。
- **复合变体（Compound Variant）**——由一个或多个原子变体或其他复合变体组成的变体集。复合变体是实际分配给范围内操作的变体。
- **约束视图（Constraints View）**——显示所选操作对象的约束。如果应用在 `ProcessModule` 而非 `LineBalancingScope` 对象上激活，则处理组约束的第二个选项卡被禁用。
- **硬限制（Hard Restrictions）**——必须遵守的限制。如果没有违反任何硬限制，则解有效。ALB 示例：优先约束、分离约束、组约束、数值内容。
- **LB**——生产线平衡应用（Line Balancing Application）；主应用（其旧称为 MLB，Manual Line Balancing）。
- **生产线平衡应用（Line Balancing Applications）**——由 LB + ALB + 设置对话框 + 约束视图组成。
- **LineBalancingScope 对象**——激活 LB 应用的节点类型。
- **生产线平衡求解器（Line Balancing Solver）**——ALB 底层优化引擎（optimization engine）的别名。
- **MLB**——手动生产线平衡（Manual Line Balancing），主应用的旧称。
- **非线性生产线（Nonlinear Line）**——带有分支的（工位）序列；也称为非线性生产线序列。此类序列无法由线性的工位列表明确表示。
- **数值检查（Numeric Check）**——数值内容（Numeric Content）。用户可以为工位指定一个数值字段、为操作指定一个数值字段、用于组合已分配操作以计算数值内容的运算符类型，以及用于将工位字段的值与相应数值内容进行比较的比较函数。
- **操作（Operation）**——`PmOperation` 类型的节点或从 `PmOperation` 派生的节点。
- **优化过程（Optimization Process）**——ALB 会话的优化阶段，通常在用户指定了各种设置和参数的值后激活；底层优化引擎也称为生产线平衡求解器。
- **工位的非同步操作（Nonsynchronized Operations of a station）**——通过 usage 分配给工位的操作。
- **PD**——Process Designer，前身为 eM-Designer。
- **工艺（Process）**——`PmProcess` 类型的节点或从 `PmProcess` 派生的节点。`PmProcess` 是一个 twin 对象，派生自复合操作（compound operation）。
- **工艺资源（Process Resource）**——`PmProcessResource` 类型的节点或从 `PmProcessResource` 派生的节点。`PmProcessResource` 是一个 twin 对象，派生自复合资源（compound resource）。
- **设置对话框（Settings Dialog）**——用于指定全局 LB 设置的管理员工具；由主菜单项激活。
- **软限制（Soft Restrictions）**——应当但可能违反的限制（因为它们不是良好解的必要条件）。ALB 示例：非负剩余时间。
- **可排序操作（Sortable Operations）**——已同步的操作。
- **工位（Station）**——一个工艺资源。
- **同步（Synchronization）**——工位（工艺资源）与其 twin 工艺的同步，即将通过 usage 分配给工位的操作设置为相应 twin 工艺的子节点。
- **工位的已同步操作（Synchronized Operations of a station）**——相应 twin 工艺的子节点操作，无论这些操作是否标记为 `ToBeDel`（待删除）。
- **UI**——用户界面（User Interface）。
- **变体工作含量列表视图（Variant Work Content list View）**——显示所选工位的每个原子变体的工作含量数据；由 LB 中的工具栏按钮激活。

<a id="v5-s34"></a>

### 工作流（Workflow）

Line Balance 工具支持的基本工作流如下：

<a id="v5-s35"></a>

#### 生产线平衡步骤（Line Balancing Steps）

1. 创建包含生产过程中所涉及操作的工序列表（OperationList）。
2. 创建表示待平衡生产线的工艺资源树（ProcessResource Tree）。
   > **注意**：仅当使用变体时，第 3 步到第 5 步是强制的。
3. 创建包含原子变体（atomic Variants）的变体库（Variant Library）。
4. 创建包含复合变体（compound Variants）的变体库。
5. 创建具有适当过滤器的变体过滤器库（Variant Filter Library）。
6. 创建生产线平衡范围（Line Balancing Scope）——在此步骤中，您在项目管理器（Project Tree）中创建一个名为 `LineBalancingScope` 的新对象，操作列表和生产线的相关部分放置在其下。
7. 定义生产线平衡设置（Line Balancing Settings）——在此步骤中，您可以定义各种生产线平衡标准，例如方向位置（orientation position）；可用的标准值，例如 up 或 down；以及应预定义为组约束的复合组（compound groups）类型。ALB：您必须定义工位对象类型（Station Object type）和工位收集器对象类型（Station Collector Object type）。仅当 ALB 需要添加激活资源时，用于自动生成激活资源的原型才是必需的。您还可以选择定义数值检查（Numerical Check）参数。
8. 通过将非平衡操作的 `nonBalancing` 标志设置为 1，定义平衡操作集合。
9. 定义操作的分配时间（allocated time）。
10. 定义工位的节拍时间（cycle time）（使用 LB 已分配操作视图）。
11. 使用 PERT 或 GenerateFlows 定义工位工艺之间的序列（非强制性的工位序列）。
12. 定义生产线平衡标准值（Line Balancing Criteria Values）——在此步骤中，您访问要包含在 LB 中的每个操作和资源的"生产线平衡"选项卡，并为需要特殊设置的标准选择一个值。
13. 定义生产线平衡约束（Line Balancing Constraints）——在此步骤中，您定义不同类型的约束，这些约束在对执行 LB 时的操作到工位的分配能力施加限制。可以定义多种不同类型的约束，包括优先约束、组约束和杂项约束。
    > **注意**：在生产线平衡过程中，不得更改操作列表浏览器（Operation List Explorer）和资源浏览器（Resource Explorer）的结构或其优先顺序/序列；否则无法保证结果的准确性。

<a id="v5-s36"></a>

#### 将操作分配到激活资源（Assignment of Operations to Active Resources）

1. **将操作分配到工位（Assigning Operations to Stations）**——在此步骤中，您使用生产线平衡查看器将操作分配到工位。这是一个交互式过程，会立即提醒您约束冲突，并使您能够以工作含量和每个工位的剩余可用时间的形式查看分配结果。这些信息也可以图形方式查看。另一个选项是使用自动生产线平衡来提供优化解。之后，您可以在手动生产线平衡窗口中手动调整该解。
2. **报告结果（Reporting the Results）**——生产线平衡的结果可以以各种报告和图表显示。对于 MLB，没有可用的报告，只有工作含量数据的 XML 导出。
3. **将 ALB 结果存储到数据库（Storing the ALB Results to the Database）**——完成生产线平衡的优化后，结果可以存储到原始数据结构。在此步骤中，平衡的操作将从生产线平衡范围同步到原始操作结构。

**通过拖放（D\&D）分配操作**

- 对于变暗（dimming）目的（仅），组簇（group cluster）或双对（dual pair）中的操作只有在整个簇/对已分配给给定操作时才有效。
- 在 D\&D 分配期间，仅考虑当前选定的操作。
  - 当选择了激活资源且按下了"变暗无效操作（Dim Invalid Operations）"时，假定只有可分配给激活资源及其父工位的操作才是有效操作。
  - 当选择了操作且按下了"无效工位（Invalid Stations）"或"激活资源（Active Resources）"时，假定该操作必须可分配给激活资源和父工位才能有效。

**非同时分配（Non-Simultaneous Assignment）**

- 将操作拖放到工位上会对该工位和未分配的操作调用一致性检查。
- 将操作拖放到激活资源上会对该激活资源调用一致性检查。
  - 已分配给当前 UI 父工位但未分配给激活资源的操作会被考虑。
  - 未分配给父工位的操作被省略，但如果尚未连接，则会连接到激活资源。
  - 已连接到激活资源但未分配给父工位的操作在检查中被省略。

**同时分配（Simultaneous Assignment）**

"同时分配"按钮（切换）启用操作到 UI 父工位的自动分配（这仅影响通过 D\&D 的分配操作）。选中此按钮时：

- 将操作分配到激活资源也会自动将其分配到 UI 父工位（如果该操作已分配到不同于当前目标激活资源的 UI 父工位的工位，则会出现错误消息且新分配失败）。
- 在激活资源之间移动操作也会在它们的 UI 父工位之间移动操作（仅在"已分配操作视图"中拖动操作时才能移动操作）。
  > **注意**：从激活资源中删除操作不会将其从工位中删除。将操作分配或移动到激活资源会对父工位和激活资源都调用一致性检查。

**一致性检查（Consistency Checks）**

与工位一样，当操作被拖放到 LB 资源树中的激活资源上时，会执行一致性检查。未连接到 UI 父工位（或将不会连接到父工位）的操作不被视为分配给激活资源，即使它们可能已（或将要）连接到它。这些操作在检查中被省略。

对于位置（Location），一致性检查由以下操作触发：

- 按下"检查选定工位（Check Selected Stations）"或"激活资源"工具栏按钮
- 按下"检查所有工位（Check All Stations）"工具栏按钮
- 在激活资源上通过 D\&D 分配操作时
- 按下"变暗无效操作（Dim Invalid Operations）"或"激活资源"工具栏按钮时

对于激活资源，执行以下检查：

- 激活资源与工位之间的装配位置检查（Assembly location check）
- 操作与激活资源之间的装配位置检查
- 激活资源的剩余时间检查（remaining time check）
- 双操作一致性检查（Dual operation consistency check）
  - 双操作必须分配到同一工位（但不一定是同一激活资源）
  - 如果分配到同一激活资源，会通知用户
  - 对于激活资源工作含量，该对被视作顺序执行

<a id="v5-s37"></a>

#### 混型变体（Mixed Model Variants）

原子变体（Atomic variants）是不包含任何其他变体、仅含一个标准值的变体。应用会考虑所有原子变体（用于计算和显示），这些原子变体：

- 是原子变体库的元素且位于当前过滤器中
- 包含在分配给 LB 所用操作、且位于当前过滤器中的变体中

原子变体的过滤仅在使用一个标准配合不同值时有效，因此过滤器的每个元素都是原子的且只代表一个值。此外，过滤器必须按以下方式组成：`atomicF1 || atomicF2`，且不得包含"not"。

<a id="v5-s38"></a>

#### 使用 ALB 应用的工作模式（Modes of Work with the ALB Application）

**创建/更新 LB 变体表（Create/Update LB Variant Table）命令**

该命令会生成一个 XML 文件，其中包含项目中所有变体及其原子变体（使用内部 ID；如果将项目从一个 schema 导入另一个 schema，必须重新调用该命令以创建新的 XML 文件）。

- 该命令的激活不需要任何选择。
- 该命令从原子变体和已分配的变体生成关系表，定义哪些原子集与哪些已分配集一致。
- XML 文件在首次调用命令时创建，并在后续调用时更新。该表还包括变体过滤器。
- 变体表 XML 文件位于 `SystemRoot\General`，因此所有用户都可以访问同一文件。
- 为避免不一致，如果检测到任何更改，该命令会签入（check in）必要的节点（如果用户不允许命令签入，命令将中止）。
  > **注意**：该命令可能耗时较长。如有必要，您可以取消。如果自上次更新以来变体信息未更改，会出现"无需更新（Update not necessary）"消息。如果变体被其他用户签出，会显示错误消息并中止命令。如果变体未签入，会针对这些变体显示"对象已签出（Object is checked out）"消息。

ALB 应用支持两种主要工作模式：

- **最小化工位数（Minimizing the number of stations）**——工位尚未定义，用户希望 ALB 应用提出一条生产线。此工作模式通常适用于工艺规划的初始阶段。
- **优化工位利用率（Optimizing the station utilization）**——用户激活模块时，工位的数量和类型已经确定。此工作模式适用于工艺规划的后期阶段，此时生产线的基本结构已经确定。
    
  <a id="v5-s39"></a>

### 数据模型定制（Data Model Customization）

为了正确使用生产线平衡应用，首先必须导入定制 `\Tecnomatix\eMPower\InitData\LineBalancingCust`。

<a id="v5-s40"></a>

### 自定义 ALB 报告布局（Customizing the ALB Report Layout）

ALB 能够生成报告（参见范围报告 Scope Report）。您可以按以下过程所述添加图像（例如公司徽标）来自定义报告。

> **注意**：自定义报告的过程对于典型的 Process Designer 管理员可能并不熟悉，因此强烈建议由西门子数字工业软件（Siemens Digital Industries Software）顾问执行该过程。

自定义 ALB 报告布局的示例过程（通过添加图像）：

1. 保存 `ALB_Report.xsl` 文件（位于 ALB 安装目录下）的备份副本。
2. 在 `<body background="ALB_Background.jpg">` 行之后，将以下代码添加到 `ALB_Report.xsl` 文件：
   ```xml
   <font size="7" face="arial,geneva,verdana" color="#0000ff">
     <u><strong><em>
       <p align="center" dir="ltr" style="MARGIN-RIGHT:0px">
         <table align="center" width="210" border="0" style="WIDTH:210px;HEIGHT:26px">
           <tr><td bgColor="#ffffff"></td></tr>
         </table>
       </p>
     </em></strong></u>
   </font>
   ```
   > **注意**：要将图像放置在报告的左侧或右侧，请将相关行中的 "center" 值替换为 "left" 或 "right"。
3. 将相关图像文件放置在 `...\Tecnomatix\eMPower\eBopBrowser\jsp\images` 下。
   > **注意**：如果图像文件名不是 `Symbol.jpg`，请将相关行中的 `Symbol.jpg` 字符串替换为相应文件名，并务必调整尺寸以适配指定图像。确保不要更改此文件中的任何其他行。

<a id="v5-s41"></a>

### 生产线平衡范围定义（Line Balancing Scope Definition）

将包含在 LB 过程中的操作和资源（生产线工位）必须放置在名为 `LineBalancingScope` 或 `LineBalancingScopeMA` 的新对象中，该对象可在任何现有文件夹下创建。

过程：

1. 在项目管理器中，创建一个名为 `LineBalancingScope` 的新快捷方式。对于将分配到多个工位的操作的生产线平衡，请创建一个新的 `LineBalancingScopeMA` 快捷方式。
2. 在新快捷方式下，通过拖放操作列表（Operation list）和工艺资源树（Process Resource Tree）来创建快捷方式；参见以下示例：

<a id="v5-s42"></a>

### 生产线平衡建模先决条件（Line Balancing Modeling Prerequisites）

需要考虑以下建模先决条件：

- 工位必须建模为工艺资源对象（twin 对象）。
- 生产线平衡范围可以包含一个变体集库（带有原子变体），以指定平衡中使用的原子变体总数。

<a id="v5-s43"></a>

#### 并行工位（Parallel Stations）

分配到多个工位的操作的生产线平衡。

生产线中以以下方式支持并行工位（Parallel stations）：

- 并行生产线的目的是复制一个或多个工位并增加节拍时间含量（cycle time content）。
- 并行工位是同类型的工位，相同的操作将分配给并行工位。
- 要指定并行工位，应将要复制的工位的节拍时间系数（cycle time factor）设置为 2。

<a id="v5-s44"></a>

#### 层次化操作列表（Hierarchical Operation Lists）

操作列表的层次结构受以下支持：

- 一个生产线平衡范围内的层次化操作列表
- 一个操作列表中的层次化操作

`OperationList` 对象可以在其子节点中包含其他 `OperationList` 对象，以帮助用户避免过长的扁平列表。这些 `OperationList` 中的操作将像任何其他操作一样用于平衡。只有 `OperationList` 对象本身不会用于平衡（无需为 `OperationList` 对象设置 `<nonBalancing>` 属性）。

操作列表中的操作可以结构化为层次结构。用户能够定义 `OperationList` 中的哪些操作将用于生产线平衡。此设置可以针对每个操作具体指定。该设置是 `<nonBalancing>` 属性；如果设置为 "0"，操作将用于平衡；如果设置为 "1"，操作将不用于平衡。在以下示例中，复合操作设置为 1，因此仅平衡单个操作。

<a id="v5-s45"></a>

#### ALB 和平衡求解器中的非线性生产线（Nonlinear Lines in ALB and Balancing Solver）

非线性生产线——Y 形线、反向 Y 形线和双侧传送带——以与 MLB 中相同的方式受支持。请注意，ALB 在虚拟分配（virtual assignment）期间考虑非线性生产线序列。

<a id="v5-s46"></a>

#### ProcessModule 对象（ProcessModule Objects）

`ProcessModule` 对象（在 MLB 和 ALB 中）以与 `operationList` 对象相同的方式受支持，除了：

- `ProcessModule` 被拖到 `LineBalancingScope`，而非 `operationList` 对象。
- 在 `ProcessModule` 下创建 `ProcessElements`，而非操作。

> **注意**：对于 `ProcessModule`，通常仅使用第一级进行分配。因此用户必须将所有那些不得分配的操作设置为 `<nonBalancing>`，因为应用不会以特殊方式处理它们。`<nonBalancing>` 标志可以在以下位置定义：
>
> - TableView（针对节点选择）
> - Customization 应用（针对特定操作类）

<a id="v5-s47"></a>

### 定义操作间约束（Definition of Constraints between Operations）

操作间约束在"生产线平衡约束定义（Line Balancing Constraints Definition）"窗口中定义，包括优先约束（precedence constraints）、组约束（group constraints）和杂项约束（miscellaneous constraints）。

<a id="v5-s48"></a>

#### 优先约束（Precedence Constraints）

"优先约束（Precedence Constraints）"选项卡类似于 PERT 视图，提供操作之间关系的图形表示，使您能够更轻松地在 `LineBalancingScope` 中更改优先规则。

过程：

1. 在"生产线平衡约束定义"窗口中，单击"优先约束"选项卡以显示相关内容（示例）。默认情况下，显示区域包含窗口左侧树中每个顶级操作的操作框。要显示树中显示的操作列表层次结构的另一部分，请选择一个子操作并单击右箭头。"优先约束"选项卡包含一个与 PERT 工具栏相同的工具栏，另加两个在设置优先规则时使用的按钮。
2. 使用以下方法之一定义优先约束：
   - 从树中选择多个操作，然后单击相应按钮。操作被复制到"优先链（Precedence Chain）"编辑框。使用向上和向下箭头根据需要排序操作，然后单击"创建（Create）"。链中只需包含那些需要优先约束的操作。此方法非常适合需要按特定顺序执行的若干操作。
   - 或直接在该图形表示中通过连接操作来建立优先关系。

<a id="v5-s49"></a>

#### 组约束（Group Constraints）

当一组操作必须在同一工位执行时，组定义（Group definitions）非常有用。有两种类型的组约束：

- **预定义组（Predefined groups）**：基于在"生产线平衡设置"窗口中选择的复合操作类型。详见预定义组约束（Predefined Group Constraints）。
- **用户定义组（User-defined groups）**：在"生产线平衡约束定义"窗口的"组约束"选项卡中定义，如下所述。

定义用户定义组：

1. 在"生产线平衡约束定义"窗口中，单击"组约束"选项卡以显示对话框（示例）。窗口中央的表显示已创建的组。展开组会显示组中包含的操作。
2. 要创建新组，单击"添加（Add）"以显示对话框。
3. 在"名称（Name）"字段中，输入新组的名称。
4. 在"原因（Reason）"字段中，根据需要输入组的描述。
5. 单击"确定（OK）"。新组将添加到"组约束"选项卡的"组"列表中。
6. 使用以下方法之一将操作从操作列表添加到新组：
   - 将操作从操作列表拖放至"组"列表中显示的组。
   - 从列表中选择一个或多个操作，然后单击双右箭头。操作将添加到"组"列表中选定的组。如果任何操作已属于其他组，这些组的名称将显示在"操作的组（Operation's Groups）"列表中。
   > **注意**：要编辑现有组，请从"组"列表中选择它并单击"编辑（Edit）"。要移除组，请从列表中选择它并单击"删除（Delete）"。

在"生产线平衡设置"窗口中定义的预定义组显示在选项卡右上角的"现有组（Existing Groups）"列表中。选择其中一个组会在选项卡右下角的"组成员（Group Members）"列表中显示其操作。

<a id="v5-s50"></a>

#### 杂项约束（Miscellaneous Constraints）

杂项约束包括：

- 双操作（Dual operations）
- 分离操作（Separated operations）
- 独占操作（Exclusive operations）
- 线外操作（Out-of-the-line operations）

在"生产线平衡约束定义"窗口中，单击"杂项约束"选项卡以显示相关内容。从操作列表中选择一个操作，并单击右箭头。该操作显示在选项卡顶部的字段中。

**双约束（Dual Constraint）**：要将该操作定义为双操作，请从操作列表中选择第二个操作，并单击双右箭头（过程后续还涵盖分离操作、独占操作及线外操作的定义）。

<a id="v5-s51"></a>

#### 约束检查（Constraints Check）

要检查除优先定义之外的所有已定义约束，请单击"检查约束（Check Constraints）"。系统对所有三个选项卡（优先、组、杂项）中定义的约束执行一系列一致性检查，如果发现任何不一致，则显示错误消息。

要检查优先定义，请单击"优先约束"选项卡上的"检查（Check）"按钮。

<a id="v5-s52"></a>

### 专用工位（Dedicated Stations）

当操作只能在特定工位执行时，该工位对该操作是"专用（dedicated）"的（其他操作也可以在该工位执行，但该操作只能在其专用工位执行）。必须满足以下条件：

- 双操作不能有不同专用工位
- 同一组簇中的操作不能有不同专用工位
- 分离操作不能有相同专用工位
- 任何操作都不能与独占操作具有相同的专用工位
- 后续操作的专用工位必须相同或为其后继

要将操作限制为仅一个工位：

1. 对于操作，使用参数 `dedicatedStation` 定义专用工位。
2. 对于工位，定义参数 `dedicatedOperation`。

执行此操作的工作流如下：

工作流：

1. 根据以下示例更改 `LibraryValues.xml`。（这仅是示例，`RelationField FieldName="name"` 也可以是其他属性，但必须与表视图（Table View）配置中定义的属性匹配。）
   ```xml
   <?xml version="1.0"?>
   <LogisticsDataModelExtensions>
     
     <LibraryValuesField FieldName="dedicatedStation" ClassName="PmOperation" Replace="true">
       
     </LibraryValuesField>
   </LogisticsDataModelExtensions>
   ```

<a id="v5-s53"></a>

### 生产线平衡主窗口（Line Balancing Main Window）

生产线平衡窗口使您能够将操作分配到每个工艺资源（例如工位），并检查工作负荷在工艺资源之间的分布。这为您提供了一种轻松的方式，在工艺资源之间重新分配操作，以创建一条平衡良好的生产线，其中所有工作站的工作负荷相似，且工位利用率水平最大化。

要打开生产线平衡窗口，请选择"应用程序"选项卡 → "生产线平衡"组 → "生产线平衡（Line Balancing）"以显示窗口（示例）。

> **注意**：您可以打开多个生产线平衡实例以促进工作流。

LB 应用的主窗口由以下三个固定窗格组成：

- **操作浏览器（Operation Explorer）**——位于左上角，用于处理操作树。操作列表树包含以下列：Constraints（显示适用于每个操作的约束）、Variant Set（显示连接的变体集）、Allocated Time（显示操作的分配时间）、Customized Field（显示定义的字段）。
- **资源树（Resource tree）**——用于显示和处理工艺资源。
- **已分配操作视图（Assigned Operations View）**——显示分配给所选工位的操作。

<a id="v5-s54"></a>

#### 使用生产线平衡窗口（Using the Line Balancing Window）

操作浏览器还包含以下列：

- **Assigned Active Resources**：显示分配给每个工位的资源数量（参见添加激活资源 Adding Active Resources）。
- **Required Active Resources**：显示使工位容量等于其工作含量所需的资源数量。
- **Numeric Content**：分配给给定工位的全部操作自定义字段值的最小值、最大值或总和。仅在"设置"对话框的"数值检查"选项卡中定义了字段时可用。
- **Remaining Content**：自定义工位字段的值减去工位的数值内容。仅在"数值检查"选项卡中定义了字段时可用。

已分配操作视图（Assigned Operations View）分为以下区域：

- **已分配操作列表（Assigned Operations List）**：显示分配给所选工位的所有操作。
- **节拍时间（Cycle Time）**：使您能够定义用于整条生产线的标准节拍时间，或在所选工位使用的节拍时间。
- **工位属性（Station Attributes）**：使您能够定义所选工位的属性，包括节拍时间系数（Cycle Time Factor，如果该工位被多条装配线使用则需要）和利用率水平（Utilization Level，决定分配给工位的激活资源（机器和工人）的效率）。

<a id="v5-s55"></a>

#### 定义变体概率（Defining the Variant Probability）

变体集概率（Variant Set probability）的值可以定义：

- 在变体集窗口中（参见下图示例）；或
- 通过 API 以编程方式设置。

<a id="v5-s56"></a>

#### 生产线平衡 UI 状态与工具栏按钮（Line Balancing UI State and Toolbar Buttons）

**保存 UI 状态**

在 Process Planner 中，MLB UI 状态仅在会话期间临时保存。在 Process Designer 中，MLB UI 状态永久保存在 `newViewer.xml` 文件中。用户可以保存不同的 Process Designer 布局，每种布局都有自己独立的 MLB UI 状态数据条目。

**保存按钮状态**

以下按钮的状态在 Process Designer 中保存：

- 使用全局节拍时间或工位节拍时间（Use Global Cycle Time / Station Cycle Time，已分配操作视图中的单选按钮）
- 扩展分配（Extended Assignment，工具栏按钮）
- 变暗无效操作（Dim invalid operations，工具栏按钮）
- 变暗无效工位（Dim invalid stations，工具栏按钮）
- 严格分配（Strict assignment，工具栏按钮）
- 最大工作含量模式（Maximum work content mode，工具栏按钮）
- 显示变体工作含量列表（Show variant work content list，工具栏按钮）
- 显示状态图表（Show state chart，工具栏按钮）

重新打开应用时，按钮将使用先前的值初始化，可选视图根据按钮状态显示或隐藏，节拍时间取自指定对象。

**保存窗口对齐**

在 Process Designer 中，保存以下布局信息：所有永久 LB 内部分隔视图（操作浏览器、资源树、操作树、已分配操作视图、工位操作选项卡）以及所有在应用关闭前显示的可选视图（可分配操作列表、变体工作含量列表、图表视图）的大小，以及每个内部视图的列大小。

**工具栏按钮**（摘要）：主要包括——显示自动生产线平衡数据（Display Automatic Line Balancing Data，切换图表视图和变体工作含量列表视图的数据源，在选中时显示 ALB 数据（浅蓝背景），否则显示 MLB 数据（白背景））、变体工作含量列表视图（Variant Work Content List View，打开查看所选工位各原子变体工作含量数据的窗口）、显示状态图表（Show State Chart，生成 LB 状态图表）、自动生产线平衡（Automatic Line Balancing，打开 ALB 窗口）、将工作含量数据导出到 XML 文件（Export Work Content Data to XML File，导出到默认位置，可配置为显示保存对话框）。

> **注意**：ALB 优化运行期间无法修改部分按钮状态。

<a id="v5-s57"></a>

#### 创建/更新生产线平衡变体表（Creating/Updating Line Balancing Variant Table）

生产线平衡应用会在需要时自动创建或更新变体表，无需任何用户交互。在创建/更新期间，系统显示进度条，同时通知当前状态。

<a id="v5-s58"></a>

#### 定义节拍时间和工位属性（Defining Cycle Time and Station Attributes）

在开始分配过程之前，按如下方式设置生产线的节拍时间和工位属性：

过程：

1. 在生产线平衡窗口的"节拍时间"区域，选择以下选项之一：
   - **生产线平衡范围（Line Balancing Scope）**——为当前 `LineBalancingScope` 中的所有工位输入一个节拍时间值（这是 LB 期间最常用选项）。
   - **工位（Station）**——输入仅适用于工艺资源树中当前所选工位的节拍时间。
   > **注意**：要将工位节拍时间传播到子树工位，请单击"传播（Propagate）"。
2. 根据需要从工艺资源树中选择工位，并为以下一项或两项输入新值：
   - **节拍时间系数（Cycle Time Factor）**——调整所选工位的节拍时间。默认情况下所有工位的节拍时间系数均为 1。但是，对于被两条装配线使用的工位，应输入 0.5，表示其节拍时间必须在两条线之间平均共享。
   - **利用率水平（Utilization Level）**——定义分配给工位的激活资源的效率。值 100 表示 100% 工人效率。输入较低的值（例如考虑 occasional rests）会使 MLB 计算相应向下调整。

工位的容量定义为：`(产品节拍时间 Product Cycle Time) * (节拍时间系数 Cycle Time Factor) * (利用率水平 Utilization Level) * (激活资源数量 Number of Active Resources)`。

<a id="v5-s59"></a>

#### 在手动生产线平衡中考虑激活资源（Taking Active Resources into Account for Manual Line Balancing）

工位中激活资源的数量是直接位于该工位下的所有激活资源加上其下属激活资源的递归总和。激活资源的容量参与工位工作含量的计算。

<a id="v5-s60"></a>

#### 手动生产线平衡中的共享操作（Shared Operations in Manual Line Balancing）

如果一个操作被分配到一个工位及其 N 个激活资源，其加权分配时间（weighted allocated time）会 N 次添加到工位的工作含量中。权重因子取决于平衡模式——为 1 或相关变体集的概率。已连接到工位激活资源但未连接到工位本身的操作不视为已分配，因此从计算中省略。

<a id="v5-s61"></a>

#### 用于分配操作的 MLB 过程（MLB Procedure for Assigning Operations）

将操作分配到资源（分配）的 MLB 过程如下：

过程：

1. 在工艺资源树中选择一个工位。
2. 将操作从操作列表树拖放到已分配操作视图或 LB 资源树中的某个工位。工位的工作含量和剩余时间会根据平衡模式更新，并且所有相关视图都会更新以反映更改。在平均模式（average mode）下，每个新操作的分配时间乘以相关变体集的概率并添加到当前工作含量，从而减少剩余时间。在最大工作含量模式（maximum work content mode）下，不使用概率。对于每个原子集，计算与此变体集相关的所有操作的工作含量，并使用具有最大工作含量的变体集（最大变体）。

如果系统检测到约束冲突，会显示警告消息。

<a id="v5-s62"></a>

#### 修改已分配操作的顺序（Modifying the Order of Assigned Operations）

您可以通过拖放修改分配给工位的操作顺序。

> **注意**：这适用于单分配模式和多重分配模式。但在单分配模式下，只能重新排序已同步的操作；在多重分配模式下，只能重新排序未同步的操作。在单分配模式下，您可以在执行同步后修改未同步操作的顺序。此外，您可以修改分配给激活资源的操作顺序。

<a id="v5-s63"></a>

#### 操作的重新分配（Reassignment of Operations）

无论是否已同步，已分配的操作都可以重新分配到其他工位，如下所示：

- 选择源工位。
- 转到已分配操作列表并选择要重新分配的操作。
- 将操作拖放至 LB 资源树中的目标工位。

操作将从源工位移动到目标工位。

> **注意**：只有在从已分配操作列表拖动操作时才能重新分配；从其他地方拖动会强制对所选操作进行新分配，如果操作已分配，则会失败。

<a id="v5-s64"></a>

#### 显示多个资源视图（Displaying Multiple Resource Views）

显示多个资源视图有助于检查分配给不同工位的资源和操作。要显示多个资源视图，请单击工具栏中的相应按钮。生产线平衡应用右侧会打开一个新分区，提供资源视图（Resource View）和已分配操作视图。选中"打开第二个资源（Open Second Resource）"复选框会在新分区中显示可选的第二个资源视图。

您可以将工作站或其他资源从资源浏览器或生产线平衡应用中的任何其他视图拖放至此选项显示的某个资源视图。要显示分配给资源的操作，请将单个资源从任何视图拖放至已分配操作视图。

多个视图支持：

- 并排比较多个工作站资源和操作分配。
- 通过拖放在视图之间重新分配操作。
- 通过拖放对操作重新排序。

<a id="v5-s65"></a>

#### 生产线平衡技术（Line Balancing Techniques）

已分配操作视图为每个操作显示以下信息：操作名称、变体集、链接的变体集（仅多重分配模式）、分配时间、同步状态。

> **注意**："多资源视图"选项显示的视图不影响资源浏览器中资源的选择（生产线平衡应用默认显示该资源）。从生产线平衡工具栏执行的命令作用于资源浏览器中所选的资源。

生产线平衡窗口包含多个可用于改进分配过程的选项：

- 从工具栏单击相应按钮以变暗（dim）所有无法根据已定义约束分配到所选工位的操作。系统还会在生产线平衡窗口左下角显示可分配操作列表。然后您可以从任一操作列表执行拖放。或者，在可分配操作列表中，您可以选择若干操作并按 SPACE 键进行分配。
- 单击相应按钮以强制按优先顺序（从优先链顶部开始）严格执行分配。由于这始终与变暗所有不可分配操作的选项结合使用，因此只有优先链中后续的操作可用于分配，大大简化了过程。

<a id="v5-s66"></a>

#### 未分配操作（Not Assigned Operations）

要显示所有尚未分配到工位的操作，请单击工具栏中的相应按钮。将打开结果视图（Results view），显示操作及其分配时间。选择"未分配（Not Assigned）"操作并按 F3 也会在上方列表中高亮它。您可以拖放操作。

关于激活资源共享与工作含量计算：

- 共享的激活资源添加为：`1/范围内父工位数量`（如果没有操作分配给它或其容量未定义，则资源初始在范围内父工位之间均匀分布）；或 `每工位工作含量 + (剩余时间 / 范围内父工位数量) / 激活资源容量`（资源在剩余时间方面在父工位之间均匀分布）。
- 如果分配给激活资源的操作带有变体，则激活资源数量是变体相关的。
- 当操作附加到工位及其 n 个激活资源时，假定操作顺序执行，因此分配时间 n 次添加到工位的工作含量。
- 激活资源树列显示每工位工作含量（Work Content per Station）、总体剩余时间（Overall Remaining Time）和节拍时间（容量）。如果用户自定义了数值内容，资源树中会出现两个额外行。
- 变体工作含量列表列显示每工位工作含量、总体剩余时间（"已分配激活资源"和"激活资源"列保持为空）。
    
  <a id="v5-s67"></a>

#### 按标题在范围内查找操作（Find Operations in Scope by Caption）

制造规划人员可以按标题（caption）在生产线平衡操作列表或工艺模块（Process Module）树中搜索操作。找到包含所定义搜索字符的第一个操作后，您可以继续搜索整个列表或树以查找后续操作。

单击"在范围内打开查找操作（Open Find Operation in Scope）"按钮时，系统会打开一个字段以输入用于搜索操作标题的字符。每次查找后，您可以单击"查找下一个（Find Next，放大镜）"按钮（位于向下箭头旁边）以搜索后续出现。

> **注意**：标题文本搜索不区分大小写；如果未找到结果会显示消息；搜索完成时也会显示消息。您可以通过单击"未分配操作"或"分配时间"列标题对列表中的操作排序。

<a id="v5-s68"></a>

#### 变体工作含量列表视图（Variant Work Content List View）

要显示变体工作含量列表视图，请从工具栏单击相应按钮。该视图显示 LB 范围内所有原子变体集的详细信息，但通常只有与当前所选工位相关的变体集的工作含量值大于 0。通过检查此信息，您可能能够识别可分配给工位的额外操作。可以通过单击相应列标题对列表排序。

父工位的变体工作含量计算为该变体下父工位的工作含量加上所有子工位在该变体下的工作含量之和。当选择了"显示自动生产线平衡数据"按钮时，变体工作含量列表窗口显示 ALB 数据（浅蓝背景），否则显示 MLB 数据（白背景）。为避免不存在的原子变体问题，列表中显示的可用原子变体始终取自代表当前数据库状态的 LB 主应用。

<a id="v5-s69"></a>

#### 删除生产线平衡分配（Deleting Line Balancing Assignments）

过程：

1. 选择要删除的已分配操作，并从工具栏单击相应按钮。所选操作的同步状态从 Yes 变为 ToBeDel。
   > **注意**：要取消删除标记，请选择操作并从工具栏单击相应按钮。
2. 从工具栏单击相应按钮执行同步。系统删除您标记为删除的操作。

<a id="v5-s70"></a>

#### 清理被动资源（Cleaning Up Passive Resources）

生产线平衡不考虑被动资源（passive resources），因为它们与平衡无关。"清理被动资源（Cleanup Passive Resources）"命令检测带有操作分配的被动资源，这些操作分配给了不是该资源父节点的（目标）工位。该命令将资源移动或复制到相关目标工位。

过程：

1. 在生产线平衡窗口中，单击相应按钮。出现"清理被动资源"窗口，列出"被动资源"列表中所有带有不适当操作分配的被动资源。
2. 您可以创建自动或手动解决方案：
   - **自动解决方案**——单击"提出解决方案（Propose Solutions）"。Process Designer 自动将资源移动到其中一个目标工位。如果资源在具有不同目标工位的父节点之间共享，清理被动资源会尽可能将资源移动到目标工位，否则复制它。
3. 单击"确定（OK）"接受提出的解决方案并保存更改。

<a id="v5-s71"></a>

### 通过图表显示分配结果（Displaying the Assignment Results by Means of Charts）

生产线平衡窗口使您能够生成一系列以图形方式描述已分配工作含量的图表。要打开生产线平衡图表窗口，请从生产线平衡窗口工具栏单击相应按钮。

当选择了"显示自动生产线平衡数据"按钮时，图表视图显示 ALB 数据（浅蓝背景），否则显示 MLB 数据（白背景）。此外，选中此按钮时，会显示工具栏，用户可在其中选择感兴趣的工位。

<a id="v5-s72"></a>

#### 图表概述（Overview of Charts）

在每个图表上，Y 轴表示工作含量，X 轴表示工位（例外：在"工位变体工作含量（Variant Work Contents of Station）"图表中，X 轴表示变体）。可以为任何图表选择以下选项：

- Y 轴可以根据从"Y 轴比例（Y-Axis Scale）"下拉列表选择的选项，将工作含量显示为绝对数字或百分比。
- 选中"显示最佳节拍时间（Show Optimal Cycle Time）"复选框会显示最佳节拍时间参考线。

<a id="v5-s73"></a>

#### 生产线平衡图表（Line Balancing Charts）

要显示 MLB 数据，请确保未选中"显示自动生产线平衡数据"按钮（白背景）。要显示 ALB 数据，请确保选中该按钮（浅蓝背景），并显示工具栏供选择工位。

可从"图表类型（Chart Type）"下拉列表选择以下图表类型：

- 最大变体工作含量（Maximum Variant Work Content）
- 工位变体工作含量（Variant Work Contents of Station）
- 变体的工位工作含量（Station Work Contents of Variant）

<a id="v5-s74"></a>

#### 平均变体工作含量（Average Variant Work Content）

平均变体工作含量（Average Variant Work Content）图表显示范围内每个相关工位的平均工作含量，同时考虑变体概率。系统使用不同颜色区分分配给工位的每个操作。将光标置于彩色段上方会显示包含操作名称及其分配时间的工具提示。

如果一个操作属于多个组，它会以相对时间出现在每个组中。例如，属于三个组的操作将表示为三个条，每个条位于不同组中，各表示操作持续时间的三分之一。

<a id="v5-s75"></a>

### 将操作分配到多个工位（Assigning Operations to Multiple Stations）

您可以使用生产线平衡将同一操作分配给多个工位以满足不同的变体条件，同时减少替代方案数量并降低复制需求。

如果将操作分配给附加工位导致原始分配在当前过滤器中不可见，系统会显示相应消息。您可以继续或取消分配，并在变体集编辑器（Variant Set Editor）中定义新变体集。但是，如果分配导致新创建的分配在过滤器中不可显示，系统会显示相应消息并阻止分配。

> **注意**：PERT 无法表示到多个工位的操作分配，因为操作必须作为（子节点）包含在工艺中才能表示。

**工作流——多重分配（Workflow - Multiple Assignment）**

关键用户（管理员）：

1. 设置用于接收和存储多重分配的变体集及信息的变体库。
2. 将 `LineBalancingScopeMA` 添加到"新建（New）"对话框中供用户选择的对象列表。

用户：

1. 打开生产线平衡。
2. 开始基本的生产线平衡活动并识别要分配的操作。

<a id="v5-s76"></a>

### 将操作分配到另一个工位（Assigning an Operation to another Station）

对于已分配给工位的操作：

过程：

1. 在按住 Alt 键的同时将操作拖放到另一个工位。显示以下对话框：
2. 要为附加分配定义变体信息，请单击"拆分表达式部分（Split expression part）"字段旁边的"定义（Define）"按钮。打开"编辑表达式（Editing Expression）"对话框。
3. 创建用于拆分的变体集后，必须更新变体映射表——显示以下消息：
4. 单击"确定（OK）"。操作被分配给工位 1 和工位 2，并显示 usage 的变体集。该分配标记为"X"。
5. 您可以单击"显示已连接工位（Show Connected Stations）"图标显示操作被分配到的工位。

**删除操作分配——合并变体集**：如果删除操作到其中一个工位的分配，应用会将变体集重新分配给仍连接到该操作的一个工位。确认变体应合并到哪个工位。

<a id="v5-s77"></a>

### 从生产线平衡范围中排除资源（Excluding Resources from the Line Balancing Scope）

在某些情况下，需要从资源树中排除范围（scope）。例如，发动机装配区（engine assembly zone）的其中一个工位由另一区域（发动机子装配区 engine sub-assembly zone）供料。发动机子装配区将是发动机区的子节点。但在向发动机装配区分配操作时，不应向其分配任何操作。因此，用户能够将发动机子装配区从包含发动机区的范围中排除。

排除通过使用 `LineBalancingScope` 对象中的集合快捷方式（collection shortcut）执行。该集合应包含包含被排除资源树的模块对象。

<a id="v5-s78"></a>

#### 排除范围（Exclude Scopes）

排除机制如上所述：通过 `LineBalancingScope` 对象中的集合快捷方式，将包含被排除资源树的模块对象纳入集合，从而将该资源树从平衡范围中排除。

<a id="v5-s79"></a>

#### 范围初始验证（Scope Initial Validation）

用户无需签出（check out）整个研究范围即可执行 ALB 研究。仅当将结果存储到 eMServer 时，对象才必须被签出。

<a id="v5-s80"></a>

#### 范围签入/签出状态（Scope Check-In/Out Status）

范围支持签入/签出（check-in/check-out）状态管理；在将 ALB 结果存储到 eMServer 之前，相关对象需处于已签出状态。
  
<a id="v5-s81"></a>

### 自动设置变体过滤器（Automatically Setting Variant Filters）

如果您经常对特定的生产线平衡范围使用相同的变体过滤器，您可以将范围连接到特定的变体过滤器。生产线平衡应用将过滤器保留为范围的一部分，省去您下次打开项目时搜索过滤器的麻烦。

生产线平衡应用执行以下操作之一：

- 如果在 `LineBalancingScope` 对象中能找到恰好一个有效的变体过滤器快捷方式，则自动设置过滤器。
- 当范围内没有变体过滤器时，不设置过滤器。
- 当范围内有超过一个变体过滤器时，不启动；生产线平衡通知用户移除除所需之外的所有过滤器。

要自动设置变体过滤器：将所需的变体过滤器拖放到 `LineBalancingScope` 对象下。

<a id="v5-s82"></a>

### 运行优化过程（Running the Optimization Process）

运行优化过程（Running the Optimization Process）使用 ALB 求解器（solver）寻找满足约束并最小化误差函数的最优工位分配方案。该过程由"开始（Start）"按钮启动，并在"停止（Stop）"时可呈现当前最优解。

<a id="v5-s83"></a>

#### 启动优化（Starting Optimization）

在通过 ALB 参数视图设置完所有定义后，优化过程即可开始。用户通过单击"开始（Start）"按钮启动优化过程。优化过程运行时，用户通过进度条收到其正在运行的通知。误差函数（error function）的值和中间解会在结果显示中实时更新。

> **注意**：ALB 优化运行期间无法修改"最大工作含量模式"按钮的状态。

从优化过程开始，会对范围数据执行大量检查。每条检查可能生成不同的错误/警告/通知。为避免这些消息的多次循环（用户每次修复一个问题然后收到下一个问题的通知），所有消息都集中在一个日志（文本）文件中显示给用户。用户能够保存该日志文件并修复其中列出的所有问题，从而避免漫长的修复迭代序列。用户能够通过单击"停止（Stop）"按钮中止优化过程；在这种情况下，将呈现最后找到的解。

<a id="v5-s84"></a>

#### 优化的方法（The Methodology of the Optimization）

ALB 将尝试平衡生产线并满足所有约束。ALB 唯一允许覆盖的约束——且仅在找不到其他解时——是不超过目标利用率水平。如果算法无法满足利用率水平约束，用户将在优化过程结束时收到相应的信息消息。

优化将停止运行的情况只有以下几种：

- 优化过程时间结束。
- 只有一个解。
- 找到的解的误差函数为 0%。

**操作的变体概率（Variant Probability for an Operation）**：在 ALB 应用中处理产品变体的方式是考虑每个变体的概率，并使用操作时间的加权平均值。

<a id="v5-s85"></a>

#### 生产线架构的提出（Proposition of a Line Architecture）

- **工位节拍时间（Station Cycle Time）**：ALB 应用的默认行为考虑在工位资源的操作选项卡上定义的节拍时间。
- **考虑激活资源（Taking Active Resources into Account）**：工位的可用节拍时间通过将节拍时间乘以工位中激活资源的数量来计算。
- **考虑独占约束（Taking Exclusivity Constraint into Account）**：当独占操作已分配给工位时，应用将不会把任何其他操作分配给该工位。如果独占操作多于工位，用户将收到错误消息及日志文件中的相关信息。

如果范围不包含应将操作分配到的工位，ALB 应用将计算并提出工位数量。在这种情况下，用户必须在参数定义中使用"使用计算的节拍时间（Use the calculated cycle time）"选项。如果用户未选择此选项，会出现相应的错误消息。

> **注意**：如果范围包含没有任何子节点、但其类型与 ALB 设置中定义的工位类型相关的 `ProcessResource` 对象，则新工位将在此对象下创建。如果存在多个此类对象，工位将放在其中之一下。

<a id="v5-s86"></a>

#### 添加激活资源（Adding Active Resources）

如果用户稍后接受该提议，将创建以下信息：

- 资源树中的适当结构、适当的约束属性值，以及 `LineBalancingScope` 下的快捷方式。
- 每个工位下的适当实例，标记为"激活资源"复选框。
- 将创建的激活资源是用户定义的通用资源原型（generic resource prototype）的实例（参见通用资源定义 Generic Resource Definition）。
- 操作树中的适当结构和序列。

> **注意**：如果范围包含指向任何工位收集器对象（twin 对象）的快捷方式，且该对象不包含定义的工位对象，则新创建的工位将在该对象下创建。

应用会在若干场景下向用户提出在工位中添加额外工具实例并将其标记为"激活资源"。用户接受此类结果后，这些实例可通过 ALB 在每个实例中放置的注释轻松识别："此实例由 ALB 应用自动创建（This instance was automatically created by the ALB application）。"

**解决因操作持续时间产生的矛盾（Resolving Contradictions due to Operation Duration）**：如果特定操作的持续时间超过工位允许的节拍时间，系统将通过增加该工位的激活资源数量等方式自动解决此情况。

<a id="v5-s87"></a>

### 显示优化结果（Displaying the Optimization Results）

当打开自动生产线平衡视图时，图表视图会自动打开（如果之前未打开），并激活"显示自动生产线平衡数据"按钮以显示 ALB 数据。在优化期间，图表视图和/或变体工作含量列表会用最新结果更新。可以在优化期间在数据源之间切换并显示 MLB 数据。最新的 ALB 数据会保留，直到开始新的优化或关闭 ALB 视图。

<a id="v5-s88"></a>

#### 误差函数（Error Function）

当 ALB 优化过程运行时，误差函数（Error Function）显示在 ALB 视图中，当前优化步骤的结果可选显示在图表视图和/或变体工作含量列表中。

<a id="v5-s89"></a>

### 范围报告（Scope Report）

ALB 结果图表可能包含大量信息，并非所有信息都对用户可见。但是，用户能够生成包含所显示解的 HTML 范围报告（scope report）。要生成报告，请按"报告（Reports）"按钮。

报告包括以下信息（分为若干段）：

- **场景参数（Scenario parameters）**——用户为场景定义的 ALB 参数。
- **解（Solution）**——分配给每个工位的操作及工位利用率。
- **图表（Charts）**——应用解析的操作分配图表。
- **工位（Stations）**——节拍时间、激活资源数量和 ALB 标准。
- **操作详情（Operations details）**——持续时间、变体概率和 ALB 标准。
- **操作间约束（Inter-operations constraints）**——操作之间定义的约束，例如线外、分离和优先。
- **组（Groups）**——范围中定义的组的信息。

用户接受 ALB 结果后，报告会自动附加到 `LineBalancingScope` 对象。

> **注意**：如果要从 Internet Explorer 内保存范围报告，除非在存储结果后将图表文件从系统根目录复制到存储的报告目录，否则图表文件不会加载到报告中（文件之间的链接关系会丢失）。

<a id="v5-s90"></a>

#### 范围报告模板（Scope Report Template）

（注：原文此节内容在提取中为空，可能为模板示例图像或表格，未捕获为文本。）

<a id="v5-s91"></a>

### 将结果存储/更新到 eMServer（Storing/Updating the Results to the eMServer）

用户接受 ALB 结果后，可将结果存储或更新到 eMServer。存储过程将平衡的操作从生产线平衡范围同步到原始操作结构（相关对象需处于已签出状态）；仅当存储结果时，对象才必须被签出。

具体存储内容包括：

- 在工位内创建（额外的）激活资源。
- 通过先在操作与适当的工位对象之间创建直接链接，然后同步工位，将操作分配存储到工位。这会使工位与操作之间的直接链接（被销毁并）替换为操作与工位关联工艺之间的链接。同步工位意味着将使直接链接到工位的操作成为关联 twin 工艺的子节点，并将这些操作与 twin 工艺中标记为 ToBeDel 的操作断开连接。
- 存储执行优化过程所用的一组参数（及一些附加细节）。
- 将图表图形和 HTML 报告文件作为附件存储到 `LineBalancingScope` 对象。报告文件可在任何 Web 浏览器中打开。

> **注意**：在存储结果时，如果需要签出的任何对象处于已签入状态，ALB 应用会自动将其签出。如果无法签出所需对象，用户会收到相应的错误消息。

> **注意**：当结果接受完成时（即所有信息已存储到 eMServer 且已创建适当的对象链接），用户会收到相应的消息。
>   
> <a id="v5-s92"></a>

## 任务管理器（Task Supervisor）

任务管理器（Task Supervisor）用于在 Process Designer 中创建、分配和跟踪任务，支持任务库（Task Library）、任务模板（Task Template）、任务分配、用户通知、历史记录（history notes）跟踪以及任务汇总（Task Summary）。

<a id="v5-s93"></a>

### 定制（Customization）

任务管理器可通过管理对话框（Administration dialog）进行定制，包括邮件配置（Mail configuration）、用户与分发列表（Distribution List）的定义、外部用户（External Users）配置等。

<a id="v5-s94"></a>

### 迁移任务管理器数据（Migrating Task Supervisor Data）

迁移任务管理器数据：

1. 要应用迁移，请手动将 XML 文件导入您的项目。迁移过程会从 XML 文件名中的项目名称移除任何特殊字符。您可以通过检查项目节点的内部 ID 和 XML 文件名来识别 XML 文件。
2. 应用迁移后，您可以通过删除旧的 Task Supervisor 属性和类来清理数据库 schema。

> **注意**：根据 schema 的大小和系统速度，迁移可能需要很长时间才能完成。

<a id="v5-s95"></a>

### 创建任务库（Creating Task Libraries）

任务库（Task Library）是任务模板的集合，您可以将其拖放到其他对象树中。这使您能够将任务附加到资源、零件或操作。然后您可以根据需要自定义新任务项。

创建任务库：

1. 在导航树中，右键单击 Library 文件夹，选择 New → Task Library，然后单击 OK。任务库节点出现在树视图中。
2. 您可以选择通过在主任务库中添加更多任务库节点（例如 Operations 和 Products）来定义"系列（families）"（子任务库节点），以组织任务。

<a id="v5-s96"></a>

### 定义任务模板（Defining Task Templates）

您可以创建并定义一个或多个任务模板。创建模板后，您可以从模板创建新任务项，将其分配给资源、零件或操作，并进行自定义。

定义任务模板：

1. 右键单击任务库节点并选择任务图标或单击 New → Task Template。新任务模板出现在任务库树中。
2. 右键单击新任务模板并选择 Rename，或双击并将其重命名为有意义的名称。
3. 右键单击新任务模板并选择 Properties。
4. 单击 Task Settings 选项卡。Task Settings 选项卡有两个子选项卡：Configuration 和 Users。
5. Configuration 选项卡：在 Status Options 列表中，勾选您希望在新模板中出现的状态，并清除您要隐藏的状态。例如，如果勾选 In Process 和 Closed 状态，用户可将任务状态设置为 In Process 或 Closed。
6. 可选地，输入描述任务的自有文本（Standard 和 Description）。

<a id="v5-s97"></a>

### 使用任务管理器（Working with Task Supervisor）

使用任务管理器（Working with Task Supervisor）涉及分配任务、通知分发列表上的用户、查看任务信息以及跟踪任务进度。

<a id="v5-s98"></a>

#### 分配任务（Assigning Tasks）

分配任务：

1. 从任务库中选择一个或多个任务模板，并将其拖放到任务管理器的上窗格中。新任务将添加到任务列表。
   > **注意**：以下属性由系统分配且无法更改：编号（Number）、创建日期（Creation Date）和所有者（Owner，即分配任务的用户）。
2. （在任务设置中定义任务属性后）选择要将任务分配到的用户并单击 OK。

<a id="v5-s99"></a>

#### 通知分发列表上的用户（Notifying Users on the Distribution List）

为了利用 Process Designer 自动生成任务通知消息，确保以下各项很重要：

- 用户信息（用户电子邮件地址）是最新的。
- 任务分发列表是完整的。

每当任务状态更改时，分发列表上的用户会自动收到通知。但是，当您首次将任务分配给对象（但尚未创建任何历史记录备注）时，系统不会发送任何通知。您可能需要手动执行。

手动通知分发列表上的用户：

1. 在任务管理器中，选择任务或历史记录备注并单击相应按钮。
2. Process Designer 创建一封发送给被分配用户的电子邮件消息，描述已分配的任务或历史记录备注。
3. 默认情况下，主题为任务主题（Task Subject）或历史主题（History Subject）。根据需要编辑。

<a id="v5-s100"></a>

#### 任务信息（Task Info）

1. 收件人列表是您在"定义任务模板"中配置的列表。您可以在任务项上编辑此列表（如果要更改现有任务项），或在任务模板中更改以将这些设置传输到新任务项。
2. 邮件区域包含您在"任务管理器管理 / 邮件配置"中定义的属性及其当前值。根据需要编辑。
3. 单击发送。

查看任务信息：单击相应按钮。"任务项信息（Task Item Info）"对话框显示为您所选任务输入的 Standard 和 Description 文本。

查看任务附件：如果任务有附件，请在附件列表中选择一个附件并单击"打开（Open）"查看。

<a id="v5-s101"></a>

#### 跟踪任务（Tracking Tasks）

随着任务向完成推进，用户可以添加历史记录备注（history notes）以跟踪其进度。每个任务的历史记录备注数量没有限制。任务所有者、被分配用户以及历史用户列表中列出的任何用户都可以添加历史记录备注。添加后，历史记录备注无法编辑，只能由其创建者删除。

显示历史记录备注：从任务列表中选择一个任务。所选任务的历史记录备注显示在"历史记录备注"列表中。

创建历史记录备注：

1. 在历史记录备注工具栏中单击相应按钮。历史记录备注列表中会出现一个新行，自动编号。
2. 编辑新历史记录备注：
   - **Remark**——输入一般文本注释。
   - **User**——单击字段并选择您的姓名（或适用时选择其他用户）。
   - **Start Date**——任务创建日期，只读字段。
   - **Estimated Date**——单击字段并从日历中选择预计完成日期。
   > **注意**：历史记录备注中最近的预计日期显示在任务的 DeliveryDate 字段中。如果该日期晚于原始 DueDate，DueDate 会以红色高亮。
   - **Status**——单击字段并选择状态值。
3. 在任务管理器底部，您可以选择添加：操作项（Action Items）、结果（Result）、附件（Attachment，通过单击屏幕底部的"添加"按钮并定位文件来附加任何类型的文件）。
   > **注意**：您可以查看附加文件的内容。参见任务信息（Task Info）。
4. 单击"保存（Save）"以保存新历史记录备注。

删除任务：您可以删除没有历史记录备注的已分配任务。在任务管理器的任务列表中选择要删除的任务并单击相应按钮。

删除历史记录备注：您可以删除所选的历史记录备注。在任务管理器的历史记录备注区域中选择要删除的历史记录备注并单击相应按钮。

<a id="v5-s102"></a>

### 任务汇总（Task Summary）

任务汇总（Task Summary）对话框显示任何树中任何层次级别的任何节点的所有当前任务和历史记录备注。每个任务在汇总中显示为一行，每个任务下方是其各历史记录备注的行。

打开任务汇总：选择一个节点并单击相应按钮。所选节点下所有对象的汇总出现。

筛选汇总：单击相应按钮。如果要用作筛选器的列数据是日期类型，则出现"日期筛选器设置（Date Filter Settings）"对话框。"列名称（Column Name）"字段显示您选择排序的参数名称。
  
<a id="v5-s103"></a>

## AutoCAD 集成（AutoCAD Integration）

AutoCAD 集成（AutoCAD Integration）使您能够基于制造布局规划流程，并在 Process Designer 与 AutoCAD 布局设计系统之间保持完全关联。

<a id="v5-s104"></a>

### Process Designer 与 AutoCAD 之间的关联（Association between Process Designer and AutoCAD）

基于制造布局规划流程是 Process Designer 的核心功能。使用 Process Designer，您可以定义制造区域的草图，并将其传输到提供资源（和物流）、位置、尺寸、备注和其他生产因素的精确设计的布局。布局设计是一个建模环境，Process Designer 与 AutoCAD 布局设计系统之间具有完全关联。

用户可以打开带 AutoCAD 的资源树，修改其布局并添加自由几何（free geometry，即不属于存储在 eMServer 数据库中的资源结构的 AutoCAD 实体，例如尺寸、注释等）。在 AutoCAD 中对树执行的任何布局更改都会同时反映在 Process Designer 中。

此外，AutoCAD 集成图层转换工具（AutoCAD Integration Layers Convert Tool）使您能够定义资源库中的特定资源被分配到的 AutoCAD 图层。

> **注意**：当筛选器处于活动状态时，会显示相应图标。访问相关筛选器设置对话框并单击"清除（Clear）"，将显示未筛选的汇总。要以 Microsoft Excel 格式导出汇总，请单击相应按钮。

<a id="v5-s105"></a>

### AutoCAD 版本与先决条件（AutoCAD Versions and Prerequisites）

Process Designer AutoCAD 集成支持最高 2019 版本的 AutoCAD。可以使用"选择 AutoCAD 版本（Select AutoCAD version）"命令选择不同的 AutoCAD 版本。

为确保 AutoCAD 集成正常运行，请检查每个工具/资源原型在 AutoCAD 中是否具有 2D 表示，并按如下所述设置库：

- 检查指定用于设计的资源库中每个工具/资源原型在 AutoCAD 中是否具有 2D 表示（`.dwg` 文件）。此步骤仅使用 AutoCAD 完成——您不使用 Process Designer 创建 2D `.dwg` 文件。
  - 为提升加载这些模型时的 CPU 性能，强烈建议为每个原型准备一个轻量表示（文件尽可能小）。
  - 确保原点（基准）点定义良好。
  - `.dwg` 文件应放在系统根目录下。
- 所有工具原型都需要在 eMServer 中链接到其 AutoCAD (`.dwg`) 表示。

> **注意**：2D 文件字段除了 2D 表示外，还可以包含任何 `.dwg` 文件，包括 3D 表示、FCAD 对象等。

选择"应用程序"选项卡 → "布局（Layout）"组 → "AutoCAD 集成（AutoCAD Integration）"。

<a id="v5-s106"></a>

### 配置 AutoCAD 集成环境（Configuring the AutoCAD Integration Environment）

在使用 Tecnomatix 菜单和工具栏于 AutoCAD 中开始工作之前，必须进行如"初始配置（Initial Configurations）"和"设置（Settings）"节所述的附加设置。

<a id="v5-s107"></a>

### 初始配置（Initial Configurations）

在使用 AutoCAD 集成之前，必须按以下各节所述设置支持路径（support path）并添加集成菜单和工具栏。

过程：

1. 在 AutoCAD 中，选择 Tools → Options。显示"选项（Options）"对话框。
2. 选择 Files 选项卡。
3. 选择并展开"支持文件搜索路径（Support File Search Path）"节点。如果 AutoCAD 集成路径尚未出现，请单击 Add。列表中底部会添加一个带有空字段的条目。
4. 输入 AutoCAD 集成支持路径（通过键入路径，或单击 Browse 并选择路径）。您添加的路径会显示。
5. 对于 AutoCAD 2019、2016、2015、2014，执行以下操作：
     
   a. 单击并展开"受信任位置（Trusted Locations）"节点。
   - AutoCAD 2019 的受信任位置路径：`<eMPowerDir>AutoCAD\2019\`
   - AutoCAD 2016 的受信任位置路径：`<eMPowerDir>AutoCAD\2016\`
   - （其余版本路径依此类推）

<a id="v5-s108"></a>

#### 将 AutoCAD 集成文件夹添加到支持路径（Adding AutoCAD Integration Folder to Support Path）

1. 单击 OK。准备使用 AutoCAD 集成的下一步是将 Tecnomatix 菜单和工具栏添加到 AutoCAD 工作环境（UI），如下节所述。

当用户权限受限时，用户可以复制以下文件到其本地临时目录，然后启动 AutoCAD 集成。这使系统能够将修改后的 `EmpAcadIntegration.mns`、`EmpAcadIntegration.mnr` 和 `EmpAcadIntegration.mnc` 文件保存到本地磁盘：

`EmpAcadIntegration.mnu`、`Backgrounds.bmp`、`Navigator.bmp`、`Properties.bmp`、`ResourceTree.bmp`、`SaveGeom.bmp`、`Setting.bmp`、`CompoundTemplate.dwg`、`InstanceTemplate.dwg`

<a id="v5-s109"></a>

#### 加载 Tecnomatix 菜单和工具栏（Loading Tecnomatix Menu and Toolbar）

过程：

1. 在 AutoCAD 中选择 Tools → Customize → Interface。显示"自定义用户界面（Customize User Interface）"窗口。
2. 在 Customize 选项卡的"所有 CUI 文件中的自定义（Customization in All CUI Files）"窗格中，单击"加载部分自定义文件（Load Partial Customization File）"图标，或单击下拉箭头并选择 Open。显示"打开（Open）"对话框。
3. 单击 Open 并选择提供的 `EmpAcadIntegration2010` 自定义菜单文件。

<a id="v5-s110"></a>

#### 导入 AutoCAD 集成定制（Importing AutoCAD Integration Customization）

要使用 AutoCAD 集成，Tecnomatix 管理员应从以下路径导入特殊定制：

`Program Files\Tecnomatix\eMPower\AutoCAD\AutoCADIntegCust`

> **重要**：必须为每个使用 AutoCAD 集成的 schema 导入该定制。

<a id="v5-s111"></a>

### 设置（Settings）

从 Tecnomatix 菜单中，管理员和用户可调整一些常规设置，如下所述。

> **注意**：本节所述的"管理员设置（Administrator Settings）"和"同步属性（Synchronize Attributes）"的更改在重启应用后生效。

<a id="v5-s112"></a>

#### 管理员设置（Administrator Settings）

- **插入缩放（Insert Scaling）**：从图形库插入到布局图（Layout Drawing）的块（即 Process Designer 中的工具实例）是 AutoCAD 施加插入缩放（Insert scaling）的参考块。如果布局图的绘图单位与插入块的单位不同，请调整此值。例如，如果布局图以毫米为单位，而块以英寸为单位，请将此值设置为 25.4。

<a id="v5-s113"></a>

#### 用户设置（User Settings）

- **位置缩放（Position Scaling）**：如果您希望将布局图的绘图单位设置为与 Process Designer 数据中使用的位置测量单位不同，请调整此值。例如，如果 Process Designer 中的测量单位为英寸，而您希望以毫米创建布局图，请将此值设置为 25.4。
- **背景缩放（Background Scaling）**：如果布局图的绘图单位与背景图的绘图单位不同，请调整此值。例如，如果布局图以毫米为单位，而背景图以英寸为单位，请将此值设置为 25.4。
- **保存几何（Save Geometry）**：集成可以在以下两种模式之一下处理自由几何：
  - **将几何保存为文件（数据库内）（Save Geometry as Files (in database)）**——（将来）所有自由几何数据将作为附加布局对象上每个复合操作的二进制属性存储在 eMServer 数据库中。注意保存此数据时对象必须处于已签出状态。
  - **文件系统（File System）**——对于激活的复合资源，一个 `.dwg` 文件包含……

<a id="v5-s114"></a>

### AutoCAD 集成实战（The AutoCAD Integration in Action）

集成运行后，用户在 AutoCAD 中对布局所做的修改（例如添加或删除资源、调整布局位置、执行检出/检入）会同步反映在 Process Designer 数据库中。

AutoCAD 布局中的实例块以「块参照（Block Reference）」形式表示。各实例内部保存了所引用文件的路径，这些路径继承自对应的原型（Prototype）。

集成运行后，用户可在 AutoCAD 中打开资源树、修改布局、添加自由几何，并通过资源树菜单（Resource Tree Menu）、导航树菜单（Navigation Tree Menu）和 AutoCAD 项（AutoCAD Items）等功能与 Process Designer 交互。
  
<a id="v5-s115"></a>

### 资源树菜单（Resource Tree Menu）

使用 AutoCAD 集成所做的全部修改仅影响资源树（Resource tree）上当前活动的复合资源（Active compound resource）。要将某个节点设为活动资源，请右键单击该节点，并在弹出的上下文菜单中选择「Set Active Resource（设为活动资源）」：

> **注意**：如果资源树未显示，请从 Tecnomatix 菜单中选择 Views > Resource tree。

资源树菜单（打开方式：在资源树中右键单击某个节点）包含若干仅与 AutoCAD 集成相关的命令，如下所述；同时还包含 Process Designer 中可用的命令（Check Out/In、Cancel Check Out、New、Delete、Expand）。与 AutoCAD 集成相关的命令包括：

- **Set Active Resource（设为活动资源）** — 以蓝色矩形标识，作为新实例的父节点，并存储所有自由几何（Free geometry）实体。
- **Show/Hide（显示/隐藏）** — 使用资源树中节点标题旁的复选框，在显示与隐藏 AutoCAD 显示区中的项目之间切换。要显示特定资源，请勾选其对应的复选框。或者，也可以右键单击复合节点，通过「Show（显示）」命令选择显示的层级深度。这与 Process Designer 图形查看器（Graphic Viewer）中的显示机制类似。您可以选择显示所选复合资源之下 1、2、3 层或全部层级。
- **Toggle Custom State（切换自定义状态）** — 在 Hide/Show（隐藏/显示）与 Check In/Out（检入/检出）状态图标之间切换。
- **Zoom To（缩放至）** — 用于放大所选对象（即在 AutoCAD 树中高亮显示的对象）。右键单击该对象打开上下文菜单并选择 Zoom To，即按「Zoom to Scaling（缩放至比例）」设置中定义的系数缩放到所选对象。
- **Move（移动）** — 用于移动所选资源（例如机器人），会自动激活 AutoCAD 的 Move 命令。在图形区中选择起点（From）和终点（To），新坐标会自动更新到 eMServer 数据库。

<a id="v5-s116"></a>

### 导航树菜单（Navigation Tree Menu）

选择 Home（主页）选项卡 → Viewers（查看器）组 → Viewers，然后选择 Navigation Tree（导航树）。

导航树上下文菜单包含两条仅与 AutoCAD 集成相关的命令：「Insert（插入，新建实例）」和「Define（定义，逆向工程）」；同时还包含 Process Designer 中可用的命令（在 Process Designer 文档中有说明：Check Out/In、Cancel Check Out、New、Delete、Expand）。

> **注意**：导航仅允许下钻到资源库（Resource library）级别。

<a id="v5-s180"></a>

#### 插入新实例（Inserting New Instances）

要从资源库创建新实例，必须先显示导航树。按以下步骤创建实例：

**步骤**

1. 在资源库下选择某个 Tool Prototype（工具原型）。
2. 双击该项目，或右键单击并从上下文菜单中选择 Insert。
3. 在图形区中左键单击，移动光标后再次单击以定位新实例。
4. 如果在 AutoCAD Integration Settings（AutoCAD 集成设置）对话框中选择了 Ask for New Instance Name（询问新实例名称）选项，则会显示 Rename（重命名）对话框 — 请为新实例定义名称。
5. 单击 OK。新实例即创建于活动资源（Active resource）之下。

<a id="v5-s181"></a>

#### 定义原型的图形（逆向工程，Defining a Prototype's Drawing）

使用此选项，可基于指定的块或图形中的实体来定义或更新原型的 AutoCAD 表示（.dwg 文件）。您可以基于现有图形，或使用 Factory CAD 等其他工具来构建结构。

按以下步骤定义/更新原型的 AutoCAD 表示：

**步骤**

1. 右键单击以选择某个 Tool Prototype。
2. 在显示的上下文菜单中选择 Define。将显示 Define prototype's AutoCAD representation（定义原型的 AutoCAD 表示）对话框。该对话框与 AutoCAD 的 Block definition（块定义）对话框类似。
3. 在 Source（源）区域中，选择现有块，或选择按 Objects（对象，即实体）定义。
   - **Base point（基点）**：单击 Pick point（拾取点）按钮，然后在图形区中选择一个基点；或使用字段定义基点的 x、y、z 值。
   - **Objects（对象）**：单击 Select objects（选择对象）按钮，在图形区中选择一个或多个实体，右键单击或按键盘 Esc 键以重新显示对话框。选择以下选项之一：
     - **Retain（保留）** — 定义 AutoCAD 表示后，所选实体仍保留在图形中。
     - **Convert to block（转换为块）** — 基于所定义的原型（而非图形中的对象）创建新实例。
     - **Delete from drawing（从图形中删除）** — 基于所选实体定义 AutoCAD 表示后，删除这些对象。
   - 在 Destination（目标）区域中设置以下路径：
     - **File name（文件名）** — 为新的 AutoCAD 表示指定名称。
     - **Location（位置）** — 确定新块文件的目标文件夹。

<a id="v5-s117"></a>

### AutoCAD 项（AutoCAD Items）

AutoCAD 项（AutoCAD Items）部分介绍在 AutoCAD 集成中可使用的背景图（Backgrounds）与模板文件（Template Files）等功能。如需详细了解集成功能，请参阅以下小节。

<a id="v5-s118"></a>

#### 背景图（Backgrounds）

在使用 AutoCAD 设计生产线或工位等时，添加背景图有助于为正在修改的项目提供整体环境上下文。您可以随时切换显示总体布局的图形视图，以检查门洞或立柱等是否可能与正在设计的工位项目发生干涉。

用户可同时显示多个背景文件（这些文件附加到当前资源树的根对象上，无论从哪个层级打开均可）。

显示一个或多个背景文件：

**步骤**

1. 从 Tecnomatix 菜单中选择 Backgrounds（背景图）。将显示 Backgrounds 对话框。
2. 勾选要显示的每个背景图对应的复选框。
3. 单击 OK。

要执行 Background（背景）命令，需将 Background.dwg 文件附加到 Root（根）节点。该文件包含用于根据生产线、工位等的展开范围自动计算背景位置的信息。

<a id="v5-s119"></a>

#### 模板文件（Template Files）

Tecnomatix 安装目录下的 AutoCAD 文件夹中包含以下模板图形文件：

- **CompoundTemplate.dwg** — 使用此模板定义复合资源（Compound resource）的图标表示。默认表示（可修改）采用 X-Y 方向并显示复合资源的名称。该图标将子节点锚定到复合资源上，使您能够一起移动它们。
- **InstanceTemplate.dwg** — 使用此模板定义缺少 2D 布局定义（其原型未定义 2D 文件）的表示的实例。在修改该文件之前，默认表示显示一个红色圆圈以及文本 "Unknown Resource（未知资源）"。

<a id="v5-s120"></a>

#### 向 Process Designer 资源的 AutoCAD 表示中添加属性（Adding attributes of a Process Designer resource to its AutoCAD representation）

用户可向 AutoCAD 表示添加 Process Designer 资源属性，这些属性将随资源实例一同显示在图形中。

以下步骤既适用于已存在的 Process Designer 工具资源原型（Tool Resource prototype），也适用于之后创建的原型。

添加属性：

**步骤**

1. 在 AutoCAD 中，创建表示某个原型资源的图形，例如一个容器（container）。
2. 从 Draw（绘图）菜单中选择 Block > Define attributes（定义属性）。将显示 Attribute Definition（属性定义）对话框。
3. 在 Tag（标记）字段中，输入要在图形中显示的 Process Designer 属性名称（"price"、"name"、"external ID" 等）。在 Prompt（提示）字段中输入编辑时显示的提示（显示在 Edit Attributes（编辑属性）对话框中）。也可以在 Value（值）字段中输入该属性的默认值。但请注意，如果在 AutoCAD Integration Settings 对话框中选择了 Synchronize Attributes（同步属性）选项，该默认值可能会被 Process Designer 中存储的属性值覆盖。
4. 要确定属性在图形中的位置，请单击 Pick Point 按钮，然后在图形区中单击；或者在 X、Y、Z 字段中设置位置值。此外，在 Text Options（文字选项）区域中可设置属性的高度与旋转角度。单击 OK 以接受所做定义并关闭对话框。
5. 由于 AutoCAD 的一个缺陷，Tag 在图形中以大写字母显示。若不将其改为小写字母，将无法与 Process Designer 中的属性定义正确同步。
6. 因此，必须修改 Tag：在图形中选择该属性（上例中的 "NAME"），单击 AutoCAD 工具栏上的 Properties（属性）按钮。将显示 Properties 对话框。
7. 将 Tag 的文字改为小写，使其与 Process Designer 属性完全一致。
8. [如果资源库（Resource Library）下尚不存在该容器（Tool Prototype），现在创建一个]。从 Navigation Tree 菜单中选择 Define 选项，选择容器的图形实体及相关的 Tag，以定义/更新该原型的 AutoCAD 表示（可以定义多个属性）。
9. 创建该容器的新实例时，所包含的属性即会显示，例如容器实例的名称 "My Container"。
10. 在图形窗口中双击该对象，以显示 Edit Attributes 对话框。在 Name（名称）字段中可以修改其值。

<a id="v5-s121"></a>

### 集成 AutoCAD 图层转换工具（Integrating the AutoCAD Layers Convert Tool）

在安装 AutoCAD 集成图层转换工具之前，请确认以下条件：

- 您的工作站已安装 AutoCAD 2019、2016、2015、2014 或 2013 版本。
  > **注意**：如果机器上同时安装了两个版本，集成将使用较新安装的版本。
- 在安装 Process Designer 时，您已在安装向导中勾选了 AutoCAD Integration（AutoCAD 集成）选项。
- 所有其他 AutoCAD 集成步骤均已完成。更多信息请参阅 Integrating AutoCAD（集成 AutoCAD）。

<a id="v5-s122"></a>

### 安装 AutoCAD 集成图层转换工具（Installing the AutoCAD Integration Layers Convert Tool）

安装 AutoCAD Integration Layers Convert Tool：

**步骤**

1. 导入 Tecnomatix\AutoCAD\AutoCADIntegCust\AutoCADIntegCust.ppc。
2. 在 AutoCAD 中安装 EmpAcadIntegration.arx（2013 至 2016 及 2019 任意版本）和 LayerManagement.arx（2013 至 2016 及 2019 任意版本）。AutoCAD Integration Layers Convert Tool 即安装完成。

AutoCAD Integration Layers Convert Tool 是一个 AutoCAD arx 组件，从 AutoCAD 内部运行。其在 AutoCAD 中的命令为 `EmpLmConvert`。

> **注意**：要查看 AutoCAD Integration Layers Convert Tool 的版本，请在命令行中输入 `EmpLMversion`。

您也可以在 AutoCAD 中安装一个工具栏按钮来运行 AutoCAD 集成图层转换工具。

安装运行 AutoCAD 集成图层转换工具的工具栏按钮：

**步骤**

1. 在 AutoCAD 中，右键单击工具栏区域并选择 Customize（自定义）。
2. 在 Categories（类别）列表中，选择 User Defined（用户定义）。
3. 在 Command（命令）列表中，将 User Defined Button（用户定义按钮）拖到工具栏上。
4. 右键单击新工具栏按钮并选择 Properties（属性）。Customize 对话框将打开到 Button Properties（按钮属性）选项卡。
5. 在 Name（名称）字段中输入 `Layers Convert`。
6. 在 Description（说明）字段中输入 `Convert layers by customers using Process Designer`。
7. 在 Macro associated with this button（与此按钮关联的宏）字段中输入以下之一：
   - 对于 AutoCAD 2019：`<eMPowerDir>\AutoCAD\2019\LayerManagement2019.arx`
   - 对于 AutoCAD 2016：`<eMPowerDir>\AutoCAD\2016\LayerManagement2016.arx`
   - 对于 AutoCAD 2015：`<eMPowerDir>\AutoCAD\2015\LayerManagement2015.arx`
   - 对于 AutoCAD 2013 或 AutoCAD 2014：`^C^C^PARX _L "C:/Program Files/Tecnomatix/eMPower/AutoCAD/2013/LayerManagement2013.arx";EmpLmConvert`
   > **注意**：路径分隔符必须为 `/`，不能是 `\`。
8. 为按钮图像选择  。
9. 在「Macro associated with this button」字段中输入以下内容：
     
   `^C^C^PARX_L "C:/Program Files/Tecnomatix/eMPower/AutoCAD/LayerManagement2013.arx";EmpLmConvert`
   > **注意**：路径分隔符必须为 `/`，不能是 `\`。请将您的宏输入与对话框中所示的宏进行比对，以确认其正确。
10. 单击 Close（关闭）。该工具栏按钮即可运行 AutoCAD 集成图层转换工具。

> **注意**：不要将此 arx 配置为自动启动。如果后台未加载 Process Designer 而运行此 arx，AutoCAD 会崩溃。

<a id="v5-s123"></a>

### 使用 AutoCAD 集成图层转换工具（Using the AutoCAD Integration Layers Convert Tool）

要使用 AutoCAD Integration Layers Convert Tool，必须执行以下操作：

- 在 Process Designer 项目中创建一个特殊的集合文件夹。
- 在 AutoCAD 中转换图层。

<a id="v5-s124"></a>

### 在 Process Designer 中设置 AcadLayers 集合（Setting up the AcadLayers Collection in Process Designer）

为 AutoCAD Integration Layers Convert Tool 准备 Process Designer 项目：

**步骤**

1. 右键单击项目节点，选择 New → AcadLayers，在项目内创建一个 Acad-Layers 集合文件夹。
2. 对于每个客户，右键单击 AcadLayers 节点并选择 New → Acad-Layers，创建一个嵌套的 AcadLayers 文件夹。
3. 对于每个客户规范所需的 AutoCAD 图层，右键单击 AcadLayers 节点并选择 New → AcadLayer。
4. 对于每个包含 ToolPrototype 的资源库，用户可以通过将图层从 AcadLayers 集合拖到资源库的 AcadLayer 选项卡上来设置客户指定的 AutoCAD 图层。要从资源库的 Property（属性）选项卡中删除某个图层名称，请选中该图层并按 Delete。

节点层级如下图所示。Process Designer 中的 AcadLayers 集合现在可用于在 AutoCAD 中转换图层。

<a id="v5-s125"></a>

### 在 AutoCAD 中转换图层（Converting Layers in AutoCAD）

在 Process Designer 中设置好 AcadLayers 集合后，即可使用 AutoCAD 集成图层转换工具在 AutoCAD 中转换图层。

保存带有客户定义图层的 AutoCAD 图形：

**步骤**

1. 右键单击 AutoCAD Integration 树中的顶层节点，选择 Show → All Levels（显示 → 全部层级）。
2. 选择 Preparation（准备）选项卡 → Libraries（库）组 → Application Settings（应用程序设置），打开 Load/Unload Applications（加载/卸载应用程序）对话框。
3. 浏览到 `C:\Program Files\Tecnomatix\Process Designer\AutoCAD`。
4. 双击 LayerManagement.arx 以加载该应用程序。
5. 单击 Close。
6. 单击 AutoCAD 主工具栏中的  ，或运行 `EmpLmConvert` 命令。将打开 Select layer definition（选择图层定义）对话框。
7. 从 Layer definition for（图层定义对象）列表框中选择要为其转换图层的客户。
8. 单击 Go!。该过程将遍历 AutoCAD 图形中的 Process Designer 对象，并根据在 Process Designer 中设置的客户定义将它们移动到正确的图层。如果图形中不存在某图层，该过程会创建一个新图层。过程完成后将出现以下消息。如果该过程未找到对象的定义图层，Select layer definition 对话框中的 Layer definition for 字段将显示为空白。
9. 单击 Cancel 关闭 Select layer definition 对话框。将创建一个日志文件，其中列出图形中受该过程影响的所有对象，并出现一个消息窗口。
10. 单击 OK 关闭消息窗口。

<a id="v5-s126"></a>

### 结束 AutoCAD 会话（Concluding the AutoCAD Session）

当您完成布局设计并希望结束集成会话时，可以选择从 Tecnomatix 菜单中选择「Exit（退出）」，或者直接退出 AutoCAD。

建议使用 Tecnomatix 菜单退出，因为系统会检查最新修改后自由几何是否已保存，并在存在未保存修改时提示您保存自由几何。AutoCAD 在关闭前不会检查自由几何是否已保存。
  
<a id="v5-s127"></a>

## TiCon 集成（TiCon Integration）

TiCon 集成（TiCon Integration）使 Process Designer 能够与 TiCon 外部数据库（用于方法时间测量 / MTM 数据）交互，以便在操作中查看、同步和更新 TiCon 元素的结构与字段。本章介绍 TiCon 集成的安装、字段映射配置、值分配与故障排除。

<a id="v5-s128"></a>

### 属性对话框中的 TiCon 集成选项卡（TiCon Integration Tab in Properties Dialog）

操作的属性（Properties）对话框中包含 TiCon Integration 选项卡，可用于查看与所选操作节点关联的 TiCon 元素的结构与字段。您可以配置显示哪些字段、隐藏哪些字段，还可以按需（"on demand"）与 TiCon 外部数据库同步并更新相应的值。

<a id="v5-s129"></a>

### 安装 TiCon 集成（Installing TiCon Integration）

**步骤**

1. 安装 TiCon 3.07、3.08、3.09、3.10 或 3.11 版本。
2. 安装 Process Designer — 可以是独立（Standalone）安装，也可以是仅客户端（Client only）安装。
3. 安装与客户相关的所有修补程序（Hot Fix，HF）。
4. 确保将 `ticonxp.dll` 和 `MTM.TiConXP.Wrapper.exe` 文件复制到第三方 MTM 目录：
   - 对于 TiCon 3.09，使用 `ticonxp.dll` 3.09.02.0038 或更高版本，以及 `MTM.TiConXP.Wrapper` 1.5 或更高版本。
   - 对于 TiCon 3.10 和 3.11，使用 `ticonxp.dll` 3.10.05.0049 或更高版本，以及 `MTM.TiConXP.Wrapper` 1.6 或更高版本。
   > **注意**：该文件不由 Siemens 提供，而由 MTM 提供。如有必要，请联系 MTM 获取此文件或更高版本。
5. 如有必要，将已有的自定义 `FieldMapping.xml` 文件复制到 `<systemroot>\General\TiConIntegration` 目录。如果文件缺失，TiCon 将显示错误。安装程序会创建一个默认文件。如果之后更改了系统根（system root），应将该文件复制到新的系统根。
6. 确保已将 TiCon 定制（customization）导入到工作模式（working schema）。
7. 为避免使用 TiCon 时出现信息提示，请确保 `FieldMapping.xml` 文件中映射的字段与数据库中 eMPower 定制保存的相应 eMServer 属性相匹配。验证注册表项 `HKEY_LOCAL_MACHINE\SOFTWARE\Tecnomatix\eM-Planner\TiConIntegration\TiCon XP_Version` 的值为 `3.7.x`。
8. 如果希望启用 TiCon 选项卡中可显示的层级数定义，请添加并定义以下注册表项：`[HKEY_LOCAL_MACHINE\SOFTWARE\Tecnomatix\eM-Planner\TiConIntegration] "ElementStructureDepth"="0"`。
   > **注意**：`ElementStructureDepth` 的类型为「字符串（string）」。此定义可提升将 TiCon 值应用到相关操作时的性能。
9. 支持 TiCon 3.07、3.08、3.09、3.10、3.11 与 Tecnomatix 10.1、11、11.1、12.1 的多种组合，这些组合需要不同版本的附加 TiCon 组件和注册表项。可能的组合如下表：

| TX Version | Required TiConXP.dll | Required Wrapper           | TiConXP_Version | TiConWrapper   |
| ---------- | -------------------- | -------------------------- | --------------- | -------------- |
| 10.1       | TiConXP3.07.23.01    | TiConXP.Wrapper.exe1.3     | 3.7.x           | TiConXP37.dll  |
| 10.1       | TiConXP3.05.16.01    | TiConXP.Wrapper.exe1.3     | 3.5.x           | TiConXP37.dll  |
| 10.1       | TiConXP3.08.14.00    | TiConXP.Wrapper.exe1.3     | 3.7.x           | TiConXP37.dll  |
| 10.1       | TiConXP3.08.14.00    | MTM.TiConXP.Wrapper.exe1.5 | 3.7.x           | TiConXP38.dll  |
| 11         | TiConXP3.08.14.00    | TiConXP.Wrapper.exe1.3     | 3.7.x           | TiConXP37.dll  |
| 11         | TiConXP3.09.02.00    | MTM.TiConXP.Wrapper.exe1.5 | 3.7.x           | TiConXP38.dll  |
| 11.1       | TiConXP3.09.02.00    | MTM.TiConXP.Wrapper.exe1.5 | 3.7.x           | TiConXP38.dll  |
| 11.1       | TiConXP3.10.02.00    | MTM.TiConXP.Wrapper.exe1.6 | 3.7.x           | TiConXP310.dll |
| 12.1       | TiConXP3.09.02.00    | MTM.TiConXP.Wrapper.exe1.5 | 3.7.x           | TiConXP38.dll  |
| 12.1       | TiConXP3.10.02.00    | MTM.TiConXP.Wrapper.exe1.6 | 3.7.x           | TiConXP310.dll |
| 12.1       | TiConXP3.11.00.0015  | MTM.TiConXP.Wrapper.exe1.6 | 3.7.x           | TiConXP310.dll |

<a id="v5-s130"></a>

### 注册 TiConXP 版本（Registering TiConXP Versions）

安装后，请验证以下 TiCon 注册表设置：

```
[HKEY_LOCAL_MACHINE\SOFTWARE\Tecnomatix\eM-Planner\TiConIntegration
• "TiConXP_Version"="3.7.x"
• "ElementStructureDepth"="0"
```

`ElementStructureDepth` 的默认值为 0。该值定义了由 TiCon Search 对话框中的「Apply（应用）」和「Create sub operations（创建子操作）」，以及 TiCon Integration 选项卡中的「Update（更新）」和「Update all（全部更新）」检索并分配的初始元素结构。可能的取值如下：

- **0** — 检索整个元素结构，并在初始操作中应用。
- **1** — 仅检索并存储元素结构中的第一级子元素。
- **2** — 检索并存储前两级。该设置的最大值为 32767。
- 如果您在搜索对话框中将树结构展开到超过 `ElementStructureDepth` 指定的层级，系统仅存储到该设置指定层级为止的结构。

<a id="v5-s131"></a>

### 使用字段映射（Working with Field Mapping）

安装 Process Designer 时，系统会创建 `FieldMapping.xml` 文件（采用默认设置），并存储在系统根 `<eMS SystemRoot>\General\TiConIntegration\FieldMapping.xml` 中。

> **注意**：如果更改了系统根，请将 `FieldMapping.xml` 复制到新位置。

```xml
<?xml version="1.0" encoding="utf-8"?>

<Configuration>
  <FieldMapping>
    <Field EmsField="mtmId" Type="None">
      <Name>id</Name>
      <MappingName>id</MappingName>
      <Persisted>no</Persisted>
    </Field>
    <Field EmsField="mtmCode" Type="None">
      <Name>code</Name>
      <MappingName>code</MappingName>
      <Persisted>yes</Persisted>
    </Field>
    <Field EmsField="mtmDescription" Type="None">
      <Name>description</Name>
      <MappingName>description</MappingName>
      <Persisted>yes</Persisted>
    </Field>
    <Field EmsField="mtmTime" Type="None">
      <Name>tg</Name>
      <MappingName>tg</MappingName>
      <Persisted>yes</Persisted>
    </Field>
    ...
    <Field EmsField="TiCon_Description" Type="None">
      <Name>descr</Name>
      <MappingName>descr</MappingName>
      <Persisted>yes</Persisted>
    </Field>
  </FieldMapping>
  <OperationsRelations>
    <Relation>
      <Parent>CompoundOperation</Parent>
      <Child>Operation</Child>
    </Relation>
  </OperationsRelations>
  <CriteriaIdMapping>
    <Criteria>
      <IdStartValue>10000</IdStartValue>
      <IdEndValue>10000</IdEndValue>
      <Name>NH</Name>
    </Criteria>
  </CriteriaIdMapping>
  <SearchCriteria Mapping>
    <SearchCriteria TextValue="daaa">SC</SearchCriteria>
  </SearchCriteria Mapping>
  <Options>
    <CheckHierarchy>yes</CheckHierarchy>
    <SaveElementHierarchy>no</SaveElementHierarchy>
  </Options>
  <CreateClass>MTM=STD</CreateClass>
</Configuration>
```

在许多情况下，管理员需要在文本编辑器中打开该 xml 文件以修改映射定义：

- **Name – Mapping Name 对**：
  - **Name** — 输入与 Process Designer 映射名称相对应的 TiCon 字段名称（来自外部应用程序）。
  - **MappingName** — 输入要在 TiCon 选项卡的 Process Designer 列中显示的映射名称。该名称也可用于 TiCon Search 对话框。
- **EMSField** — 如果相关的 TiCon 字段要应用到 eMServer 中的独立字段，请在 `FieldMapping.xml` 中指定 EMSField。
- **Type 属性**可取以下两个值之一：
  - **Time** — 将字段标记为时间单位值。系统将这些字段从 TiCon 单位（TMU）转换为秒，并相应存储到 eMServer 数据库。当系统在搜索对话框或 TiCon 集成选项卡中显示这些字段时，会根据 Options 中配置的时间单位设置进行转换。Time 字段只能通过 EMSField 属性映射到 eMServer 中 Double 类型的字段。
  - **None** — 字段未定义单位，按「原样」从 TiCon 数据库检索，不进行任何转换，可以存储到任何类型的 eMServer 字段。例如，字段 "state" 是整数而非时间值，不应转换为秒，可映射到整型字段；而字段 "description" 是字符串，无法映射到整型字段。
  > **注意**：系统不会对这些设置进行内部检查。正确的字段映射由配置该 xml 文件的用户负责。
- `FieldMapping.xml` 必须包含 "id" TiCon 字段（等同于 TiCon 数据库中的 TiCon 元素 ID）。
- **CheckHierarchy** — 控制当用户尝试将 TiCon 元素应用到属于同一层级分支的成员时，Process Designer 是否提示。
- **SaveElementHierarchy** — 如果设置为 "no"，元素层级不会保存到 eMServer，也不会显示在 TiCon Integration 选项卡中。设置为 "yes"（或字段映射文件中不存在该项）会将元素层级保存到 eMServer 并显示在 TiCon Integration 选项卡中。
- **Creation Class（创建类）** — 用于定义新 TiCon 元素的默认类。该类不能包含命名掩码（naming mask）。
- **Search Criteria（搜索条件）** — 用于将新类的 TiCon 搜索条件（SC）设置为所需值。例如：
  ```xml
  <SearchCriteriaMapping>
    <SearchCriteria FieldValue="allocatedTime" TextValue="TextSampleValue0">SC</SearchCriteria>
    <SearchCriteria TextValue="TextSampleValue1">AREA</SearchCriteria>
    <SearchCriteria TextValue="TextSampleValue2">DEPT</SearchCriteria>
  </SearchCriteriaMapping>
  ```
  其中 `allocatedTime` 是 eMS 字段值，`SC`、`AREA`、`DEPT` 是 TiCon 用户定义的搜索条件字段。

> **注意**：
>
> - 如果 TiCon 集成启动时 `FieldMapping.xml` 不存在（例如更改了系统根位置），系统会报错。如有必要，请将 `...\General\TiconIntegration\` 文件夹复制到系统根。
> - 如果存在条件 ID 映射（Criteria ID mapping），且其值在 `IdStartValue` 与 `IdEndValue` 之间，系统会将其替换为 `Name` 值。
> - 在 `FieldMapping.xml` 中 `Persisted` 属性为 Yes 的 TiCon 属性会上传到 eMServer，并显示在 TiCon Search 对话框和 TiCon Integration 选项卡中；`Persisted` 为 No 的属性不会上传到 eMServer，也不会显示在 TiCon Integration 选项卡中。

<a id="v5-s132"></a>

### 将 TiCon 值分配给 Process Designer 操作（Assigning TiCon Values to Process Designer Operations）

要分配 TiCon 值：

**步骤**

1. 单击  ，将显示 TiCon Login 对话框。
2. 输入您的用户名（User Name）和密码（Password）。
3. 输入数据范围 ID（Data Range ID）。可能的值为 1、10 或 20。
4. 选择所需的语言（Language）。
5. 单击 Login 以访问 TiCon 数据库。将显示 TiCon Search 对话框。
   > **注意**：在 Process Designer 会话期间，您将保持登录到 TiCon 数据库。
6. 在 Search 面板区域中，输入任意组合的 TiCon 搜索条件（更多信息请参阅 TiCon 文档）。
7. 单击 Search。TiCon Search 对话框下半部分的表格将填充查询结果。
   > **注意**：如果结果超过 5 个，系统会提示您如何继续。单击 Yes 显示所有结果，或单击 No 以细化搜索。
8. 在 Operation 字段中单击，并选择要分配 TiCon 属性的操作（从 Operation Tree 中）。
   > **注意**：
   >
   > - 有关 TiCon Search 对话框结果区域中显示的 TiCon 属性，请参阅 TiCon 文档。
   > - TiCon 属性众多。您可以通过编辑位于 `<eMS SystemRoot>\General\TiConIntegration` 的 `FieldMapping.xml` 文件，自定义 TiCon Search 对话框结果区域中 TiCon 属性的显示。更多信息请参阅「使用字段映射（Working with Field Mapping）」。
   > - 在 `FieldMapping.xml` 中 `Persisted` 属性为 Yes 的 TiCon 属性会上传到 eMServer，并显示在 TiCon Search 对话框和 TiCon Integration 选项卡中；为 No 的属性不会上传到 eMServer，也不会显示在 TiCon Integration 选项卡中。
9. 选择 TiCon 字段并单击 Create sub operation，如果希望为所选 TiCon 字段在所选操作下创建新的子操作。新子操作将显示在 Navigation Tree 中，并且 `FieldMapping.xml` 中 `Persisted` 属性为 Yes 的 TiCon 属性会分配给新子操作。
   > **注意**：创建子操作之前，必须通过编辑 `FieldMapping.xml` 为您要配置的节点类型指定子操作类型。
10. 选择 TiCon 字段并单击  ，以展开节点并查看其树结构。系统通过 TiCon API 逐个检索层级，因此用户每展开一个子级都会有一定延迟。如果折叠后再次展开该级，则不会有延迟，因为信息已存在于节点中。
11. 选择 TiCon 字段并单击 Apply，将其分配给所选操作。您可以重复此步骤将其他字段应用到其他操作。如果所选操作已分配了 TiCon 元素，将出现对话框以确认覆盖该元素。
12. 关闭 TiCon Search 对话框。您所做的更改将显示在 TiCon Integration 选项卡中。
    > **注意**：每当您使用 TiCon Integration 创建新操作，或由 TiCon Integration 更新时间时，操作的 `timeDeterminationMethod` 字段始终设置为 `MTM`。

<a id="v5-s133"></a>

### TiCon 集成选项卡（TiCon Integration Tab）

要访问 TiCon Integration：

**步骤**

1. 在 Operation Tree 中右键单击某个操作，并选择 Properties。
2. 单击 TiCon Integration 选项卡。

系统会从 TiCon 数据库加载 `Persisted` 属性为 yes 的值，将这些值上传到 eMServer 数据库，并显示该特定节点的所有字段值。

您可以执行以下任意操作：

- **Create（创建）** — 如果您选择了一个尚未关联任何 TiCon 元素的操作，此功能将创建一个新的空 TiCon 元素。通常，创建新 TiCon 元素后，贵组织中负责 TiCon 的人员会编辑该元素并通知您。
- **Update（更新）** — 从 TiCon 数据库更新显示，并将值加载到 eMServer 数据库。对所选节点操作。
- **Update All（全部更新）** — 从 TiCon 数据库更新显示，并将值加载到 eMServer 数据库。对所选节点及其子节点操作。
- **Reset（重置）** — 从所选操作及 eMServer 数据库中移除已分配的 TiCon 属性。
- **Logout（注销）** — 注销 TiCon 应用程序。
- **Open in Ticon（在 TiCon 中打开）** — 显示所选元素的 TiCon 信息。如果 TiCon 正在运行，TiCon 将显示所选元素的信息；如果 TiCon 未运行，但您已通过 Process Designer 登录 TiCon，系统将使用您的 Process Designer TiCon 登录信息登录 TiCon 并显示所选元素的信息；如果 TiCon 未运行且您也未通过 Process Designer 登录 TiCon，系统将显示 TiCon 登录对话框。
  > **注意**：
  >
  > - 如果您在未登录 TiCon 数据库的情况下单击这些按钮之一，将出现 TiCon Login 对话框（请参阅「将 TiCon 值分配给 Process Designer 操作」）。
  > - 如果您配置 `FieldMapping.xml` 以显示额外的 TiCon 属性，TiCon Integration 选项卡会为已创建 TiCon 分配但其值全为灰色的操作显示新的表头。单击 Update All 以刷新显示。
  > - 如果您尝试将同一 TiCon 元素应用到层级同一分支的成员（无论相隔多少级），Process Designer 会提示您确认该操作。

<a id="v5-s134"></a>

### TiCon SSO 登录（TiCon SSO Login）

登录 TiCon 所使用的语言是当前设置的 Tecnomatix 语言（来自 Tecnomatix Doctor）。如果 SSO 登录未成功，将出现常规登录对话框以输入用户凭据。

TiCon 集成可使用 SSO 登录，免去用户每次使用时输入凭据的麻烦。管理员可以从 TiCon Administration 应用程序为用户配置 SSO 登录，设置 Windows 用户账户并勾选 Windows Login 复选框。

<a id="v5-s135"></a>

### 字段映射（Field Mapping）

TiCon Integration 使用一个映射文件来确定 TiCon 数据库中的哪些字段映射到 eMServer 数据库中的哪些字段。该 `FieldMapping.xml` 文件必须位于 `<systemroot>\General\TiConIntegration` 下。如果文件不存在，TiCon Integration 将显示错误消息。Process Designer 安装程序会安装一个包含默认值的文件，但通常您需要修改该文件以用于生产环境。更改系统根时，请确保相应的映射文件在新系统根上可用。更多信息请参阅「使用字段映射（Working with Field Mapping）」。

<a id="v5-s136"></a>

### 故障排除（Trouble Shooting）

TiCon Integration 可映射并从 TiCon 数据库传输的字段受 TiconXP.dll 支持的限制。以下是可用字段列表：

- **标准字段（Standard fields）**：ObjectID、ObjectCode、ObjectIndex、ObjectVariant、ObjectDescription、DataRangeID、ModulClassID、TiConClassID、TiConUser、Tg、Trg、Te、Tr、ObjectChildID、Nr、Factor、FactorValue、LineIndicator1、LineIndicator2、LineIndicator3、LineDescription
- **结构字段（Structure fields）**：ObjectBegin、ObjectContent、ObjectEnd、ObjectLimit、Indicator1、Indicator2、Indicator3、DirectVwg、AnmetId、ShortCode、State、Type、Owner、Changer、Creator、ChangeDate、CreationDate
- **时间字段（Time fields）**：ttu_ins、ttb_ins、tw_ins、tb1_ins、tb2_ins、trtb_ins、trtu_ins、trw_ins、trb1_ins、trb2_ins、tr_ins、trtb_cal、trtu_cal、ttb_cal、ttu_cal、ta_cal、trw_cal、t_cal、ter_cal、tw_cal、tvp_cal、tvs_cal、trv_cal、trer_cal、tb1_cal、tb2_cal、trb1_cal、trb2_cal、t01_cal、t02_cal、t03_cal、t04_cal、t05_cal、t06_cal、t07_cal、t08_cal、t09_cal、t10_cal、t11_cal、t12_cal、va_time\_[01-40]
- **条件字段（Criteria fields）**：最多可映射 10 个客户特定的条件字段，需使用字段的代码。
- **限制（Restriction）**：同一时间最多只能映射 12 个时间字段。

如果集成未按预期工作，您可以检查以下可能的问题：

**步骤**

1. **TiCon Integration 选项卡不可见** — 可能是选项卡可执行文件的注册缺失所致。可使用以下命令纠正：`regasm <Tecnomatix Installation Directory>\eMPower\TiConIntegration.dll`
2. **显示「Connection to TiCon failed（连接 TiCon 失败）」消息** — 可能是 `ticonXP.dll` 或 TiCon wrapper 可执行文件的注册表项缺失所致。可使用以下命令纠正：
   ```
   regsvr32 <MTM Installation Directory>\<TiCon Version Directory>\ticonxp.dll
   regsvr32 <MTM Installation Directory>\<TiCon Version Directory>\TiConXP.Wrapper.exe
   <MTM Installation Directory>\<TiCon Version Directory>\TiConXP.Wrapper.exe /regserver
   ```

<a id="v5-s137"></a>

## Robcad 集成（Robcad）

Robcad 集成（Robcad）使基于 eMServer 的应用程序与 Robcad 之间能够轻松互连，从而在 eMServer 数据库与 Robcad 单元（cell）之间共享、添加和更新布局与操作数据。本章介绍 Robcad 集成的工具、使用前准备、工程库（Engineering Libraries）创建、定制导出、单元更新，以及 eMServer 与 Robcad 之间的属性交换。

<a id="v5-s138"></a>

### Robcad 集成工具（Robcad Integration Tool）

Robcad Integration 工具使基于 eMServer 的应用程序用户与 Robcad 用户能够在两个环境之间轻松连接。利用这些集成能力，您可以在 eMServer 数据库与 Robcad 单元之间共享、添加和更新布局及操作数据。

> **重要**：要在 Tecnomatix 应用程序中使用 Robcad-eMServer 集成，请在 robcad 文件中设置：`EMS_INTEGRATION_PHASE 6`

<a id="v5-s139"></a>

### 使用 Robcad 集成之前（Before Using Robcad Integration）

在使用 Robcad 集成之前，需要完成若干准备工作，包括创建 RobcadStudy 以及在研究中为相关的焊接操作、零件和资源创建快捷方式。

<a id="v5-s140"></a>

#### 准备工作（Preliminaries）

使用 Robcad 集成之前，您必须创建一个 RobcadStudy。您还应在该研究（study）中为相关的焊接操作、零件和资源创建快捷方式。

<a id="v5-s141"></a>

#### 创建工程库（Creating Engineering Libraries）

创建工程库是指将组件类型（资源或零件）分配给 Process Designer 中的 3D 数据。这是能够操作 Robcad-eMServer 集成的关键。创建工程库会在 3D 组件下存储一个名为 `TuneData.xml` 的 XML 文件。此外，此功能还会创建指向该组件图像文件（若存在）的链接。该图像显示在零件或工具原型的 Physical（物理）选项卡中。

对于数据存储在 Unix 文件系统的情况，由于 Unix 系统区分大小写，需要特别注意。例如，在使用 DiskAccess 的情况下，在将网络驱动器映射到 Unix NFS 之前，应在 DiskAccess Administrator Utility 对话框的 File Name 选项卡中激活「Preserve Case（保留大小写）」选项。

<a id="v5-s142"></a>

#### 将 eMServer 定制导出到 Robcad（Exporting eMServer Customization to Robcad）

**支持未分配 3D 组件的节点**

集成支持未分配 3D 组件的节点。

**用户责任**：确保 Robcad Library 根文件夹下存在「system」文件夹（如果尚不存在，应创建它），并确保您对该「system」文件夹具有写权限。「system」文件夹应包含名为 `emptyComponent.cojt` 的组件，该组件可从 Tecnomatix 安装目录 `Program Files\Tecnomatix\Tecnomatix\NewWorld\Data` 复制。

结构和类在 Process Designer 与 Robcad 中的表示方式相似，并在两者之间交换。使用 Robcad Hierarchy Tree 时，树中显示的图标描绘了不同的类类型。要启用此能力，请使用以下步骤将定制导出到 Robcad。

<a id="v5-s143"></a>

#### 创建 Robcad 单元（Creating Robcad Cells）

**导出过程**：

**步骤**

1. 以 Administrator 身份登录 Process Designer，且不打开任何项目。
2. 选择 Applications（应用程序）选项卡 → Robcad 组 → Export Customization to Robcad（导出定制到 Robcad）。
3. 浏览到 Robcad Library Root 文件夹，并选择其下的「system」文件夹。应用程序会在「system」文件夹下自动创建「Robcad_eMS_cust」定制文件夹。
   > **注意**：如果选择的文件夹不是 Robcad Library Root/system，您必须随后将「Robcad_eMS_cust」文件夹复制到 Robcad Library Root/system 文件夹。

此外，定制导出使 Robcad 能够在 Hierarchy Tree 中将 In Process Assemblies（终端项，end items）表示为 Locked Nodes（锁定节点），使用暗淡的斜体字体。

创建单元完成后，Process Designer 会通知您操作成功，或在创建失败时提醒您存在问题。如有必要，弹出通知允许您查看报告以获取详细信息。

<a id="v5-s144"></a>

### 更新 Robcad 单元（Updating Robcad Cells）

在 Process Designer 的 RobcadStudy 中修改布局和流程项（例如添加、移动、删除）后，使用以下步骤更新 Robcad（eM-Workplace）单元：

**步骤**

1. 从树中选择 Study 文件夹，并从 Applications 选项卡中选择 Robcad Studies 选项。将打开 Robcad Studies 对话框。
2. 选择包含要更新项的 RobcadStudy 文件夹。（或者，从树中右键单击 Study 文件夹下的某个研究，并选择 Robcad Studies 选项，将打开包含您所选研究的 Robcad Studies 对话框。）勾选要更新的研究（或多个研究）— 您可以单击 [图标] 勾选所有指定了 NFS Location 的研究，或单击 [图标] 取消勾选所有研究。
3. 单击「Update Cells（更新单元）」，并指明是否导出 Parts（零件）、Resources（资源）和/或 Operations（操作）。

> **注意**：如果您希望保存更新前单元状态的副本，请勾选 Backup（备份）选项。备份单元保存为 `<cellname>.ce.bex_ddmmyy_hhmmss`。每次带备份的连续更新都会创建一个新的备份单元。更新完成后，Process Designer 会通知您操作成功，或在更新失败时提醒您存在问题。您可以查看报告以获取详细信息。

<a id="v5-s145"></a>

### 检查 Robcad 单元组件（Checking Robcad Cell Components）

运行 Robcad Connectivity Studies 后，库管理员通常希望检查项目工程库中 Robcad 单元内的所有组件是否可用。他们可以运行 Check Robcad Cell Components 命令（所需权限与创建工程库相同）来生成列出单元中任何缺陷的文本报告。报告还列出了修复单元所需的操作。

**步骤**

1. 使用 Check Robcad Cell Components 命令之前，请打开 Settings 对话框（从 Robcad Connectivity Studies 对话框中），并设置 Library Root 以及该命令存储 Check Robcad Cell Components 报告的文件夹。
2. 单击 Check Robcad Cell Components 命令图标 [图标]，在打开的文件夹对话框中选择一个 Robcad 单元。系统会在指定文件夹中生成 `RCCCheckReport.txt` 文件。
3. 如果需要更正，文本文件报告会列出您应为修复单元采取的措施。

> **重要**：如果文本文件报告了修复单元所需的操作，请务必先执行所列的第一项，再处理后续操作。
>   
> **注意**：为便于仅查看相关组件，超级组件（supercomponents）的子组件不会列在日志中。

<a id="v5-s146"></a>

### 在 Robcad 中使用集成单元（Working with Integrated Robcad Cells in Robcad）

**焊接操作（Weld Operations）**

在导出到 eMServer 之前，您必须设置 Robcad 与 eMServer 之间的路径信息，因为 Robcad 单元可能包含与 eMServer 无关的信息。例如，一个单元可能包含两个焊接操作，而仿真中包含六个焊接路径选项。在 Robcad 中，您必须选择要导回 eMServer 的焊接路径。您可以使用 Path Selection（路径选择）表来创建此定义。默认情况下，将焊接路径导入 Robcad 会自动执行路径选择。仅当 Robcad 中的仿真期间信息发生更改时，您才需要选择路径。

<a id="v5-s147"></a>

#### 路径选择表（Path Selection Table）

Path Selection 表包含以下行：

- **Path（路径）** — Robcad 路径选择。
- **Robot（机器人）** — 将 Robot 分配给路径。
- **Gun（焊枪）** — 将 Gun 分配给路径。
- **Time（时间）** — 包含为每个焊接路径计算出的循环时间定义。

路径选择会在单元文件夹下生成映射日志文件 `<cell name>_map.log`。请参阅「使用 Robcad Legacy Cell 信息更新 eMServer（Updating eMServer with Robcad Legacy Cell Information）」。

**零件与资源（Parts and Resources）**

Hierarchy Tree 工具箱允许您设置树中复合零件与复合资源的层级排列，并在随后更新到 eMServer 后在 Process Designer 中查看这些修改。有关为 legacy 单元创建层级排列的信息，请参阅「使用 Robcad Legacy Cell 信息更新 eMServer」。

您可以通过单击鼠标左键选择节点，然后单击中键拖动，在树中拖放项目。当尝试非法拖放（例如将某个实例拖放到另一个实例上）时，会显示错误消息。

<a id="v5-s148"></a>

#### 层级树（Hierarchy Tree）

Hierarchy Tree 工具箱允许您设置树中复合零件与复合资源的层级排列，并在随后更新到 eMServer 后在 Process Designer 中查看修改。

**Tree 菜单（Tree Menu）**

- **Update（更新）** — 选择此选项时更新树显示。
- **Auto Update（自动更新）** — 自动更新树显示。

**Edit 菜单（Edit Menu）**

- **Delete（删除）** — 从单元中删除所选的项。
- **Blank（隐藏）** — 隐藏工作单元中所选的实体。
- **Display（显示）** — 显示已隐藏的实体。
- **Display all（全部显示）** — 显示整个单元。
- **Select all（全选）** — 选择树中的所有节点。
- **Create compound（创建复合）** — 从所选零件/资源创建新的复合。
- **Move（移动）** — 将所选节点移动到所需位置（与拖放效果相同）。
- **Extract from compound（从复合中提取）** — 从复合中移除零件/资源，并将其在树中上移一级。
- **Copy（复制）** — 将所选节点复制到新的层级位置。
  > **注意**：不适用于所选节点的编辑功能会被停用（变暗）。

**View 菜单（View Menu）**

- **Collapse all（全部折叠）** — 关闭树层级，仅显示最高级节点。
- **Expand all（全部展开）** — 显示整个树层级（所有级别）。
- **Sort（排序）** — 激活时按字母数字顺序排序树；停用时恢复先前显示的层级。
- **Show parts（显示零件）** — 仅显示零件。
- **Show resources（显示资源）** — 仅显示资源。

**锁定复合节点（Locked Compound Nodes）**

新的 Locked Compound Nodes 功能允许您检测在 Process Designer 中定义为 In Process Assemblies（终端项）的复合零件。锁定的复合零件禁止编辑，并在 Hierarchy Tree 中以不同的字体显示。请参阅「将 eMServer 定制导出到 Robcad（Exporting eMServer Customization to Robcad）」。

<a id="v5-s149"></a>

#### 更新（Update）

Update 命令对已经连接到 eMServer 的单元进行操作。请确保您的机器配置了正确的 eMServer 设置（模式 schema、Oracle 服务器等）。

从 eMServer-Integ 菜单中单击 Update。将显示 eMServer 登录窗口。登录后，将显示以下确认对话框：

单击 OK 以保存并卸载 Robcad 单元。将打开 Robcad Studies 对话框，其中仅包含与 Robcad 单元对应的研究。如果要中止 Update 过程，请选择 Cancel。

Update 完成后，关闭 Robcad Studies 对话框。将打开一个对话框，提供重新加载 Robcad 单元的选项。

<a id="v5-s150"></a>

#### Process Designer

单击 Process Designer 以启动 Process Designer 应用程序，并加载 Robcad 中已加载单元所对应的研究。该单元被存储，集成会提示您更新 eMServer。

<a id="v5-s151"></a>

### 使用 eM-Spot 定位焊接位置（Using eM-Spot to Orient Weld Locations）

焊接位置的默认位置在焊点（weld point）位置处。位置的默认方向为 0,0,0 — 可以使用标准的 eM-Spot 工具进行更新。

<a id="v5-s152"></a>

### 使用 Robcad 单元信息更新 eMServer（Updating eMServer with Robcad Cell Information）

使用以下步骤更新焊接位置：

**步骤**

1. 加载 eM-Spot 应用程序。
2. 打开 Weld_locs 菜单。
3. 在 'Locations' 菜单中，选择 'Display unupdated' 命令以高亮显示单元中所有新的焊点和位置。
   > **注意**：位置由 Process Designer 中未投影（unprojected）的焊点自动创建。Robcad 将未投影的焊点用黄色标识，以将其区分为仍需为机器人仿真进行投影的位置。
4. 要更新位置，请从 Locations 菜单中选择 'Update locations' 命令。
5. 在显示的窗口中，选择 'Automatic' 工作模式，并选择要投影位置的位置和零件。该位置被更新并正确投影到所选零件上。对所有位置重复步骤 1-5。完成上述步骤后，Robcad 路径已准备好进行仿真和优化。Spreadsheet 包含来自 eMServer 的所有焊点信息。但是，此信息在 Robcad 中已更改，因为所有位置都基于零件几何进行了更新。可以使用 Update 命令在 Robcad 单元内更新电子表格。所有位置更新后，建议按如下方式更新电子表格内容：
6. 在 Robcad 电子表格中，选择 eM 菜单并选择 'Update from eM' 选项 — 所有更新的 Robcad 值都会更新电子表格。现在 Robcad 单元包含来自 eMServer 的所有相关信息，并可以使用现有的 Robcad 工具进行仿真和优化。

在 Robcad 单元中修改布局项（例如添加、移动、删除；使用 Hierarchy Tree 和 Path Selection Table）并保存单元后，您可以将更新的布局数据导入 Process Designer。

**步骤**

1. 选择 Robcad Studies 文件夹后，选择 Applications 选项卡 → Robcad 组 → Robcad Connectivity Studies（或者，右键单击 Robcad Study 节点并从上下文菜单中选择 Robcad Connectivity Studies 命令）。
2. 在 Robcad Studies 对话框中，勾选一个或多个希望更新 Process Designer 中内容的 Robcad 研究。
3. 单击「Update eMServer」以打开一个对话框，在其中指定要从 Robcad 单元导入的元素类型。
   > **重要**：使用 Robcad 单元数据更新 eMServer 会根据您在 eMServer Update 对话框中勾选的选项（Parts、Resources、Operations）更新 eMServer 上的数据。例如，您仅创建了某个 RobcadStudy 的资源（该研究也包含零件），然后将其对应单元从 Robcad 更新回 Process Designer。如果您在 Import Studies 对话框中选择了更新 Removed Parts，则会从 RobcadStudy 文件夹中删除零件快捷方式以及 eMServer 中所有现有的被引用零件实例。

该更新会使用以下命名格式自动备份 Robcad 单元：`<cellname>.ce.bim_ddmmyy_hhmmss`。更新完成后，Process Designer 会通知您操作成功，或在更新失败时提醒您存在问题。您可以查看报告以获取详细信息。单击「Close」后，项目树被更新 — 从 Robcad 单元中删除的项实例会从 eMServer 中移除。项目树现在包含一个名为「<登录用户名> 的文件夹」（例如「administrator 的文件夹」）的新文件夹。该文件夹包含一个名为「Imported From <NFS 路径和 Robcad 单元名>」的子文件夹，其中包含从 Robcad 导入的新实例。此外，RobcadStudy 文件夹现在包含指向导入实例的快捷方式。

<a id="v5-s153"></a>

### 使用 Robcad Legacy Cell 信息更新 eMServer（Updating eMServer with Robcad Legacy Cell Information）

为了与 Process Designer 集成，Robcad legacy 单元为 6.0.2（及更高）版本，即尚未连接到 Process Designer 环境中某个研究的单元。

由于 Robcad 操作模型不如 eMServer 模型严格，因此必须明确定义映射。Legacy 单元的行为与 Connected（已连接）单元略有不同。「Legacy Cell（遗留单元）」是指从未更新到 eMServer 的单元。「Connected Cell（连接单元）」是指至少更新过一次到 eMServer 的单元。Connected Cell 不能再恢复为 Legacy Cell（即使使用「Force Reconnect」选项也不行）。如果用户在 Robcad 的「Path Selection（路径选择）」表中选择了某操作，则该操作称为 mapped operation（已映射操作）。术语「Weld Operation（焊接操作）」与「Weld Path（焊接路径）」含义相同。

映射的一般准则（有一些例外）：

- 只有 mapped 操作（路径）及其引用的焊接位置和焊点会更新到 eMServer。所有其他路径、位置和焊点都不会存储在 eMServer 上。用户可以在 Robcad 的「Path Selection」表中选择哪些路径被映射（即与 eMServer 共享）。
- 更新单元（从 eMServer 到 Robcad）时，仅修改 mapped 操作及其焊接位置和焊点。因此建议仅在「eMServer Update」操作之前更改路径选择，否则相关数据可能无法用 eMServer 的最新更改进行更新。

以下数据与 eMServer 不兼容。因此一般情况下（有一些例外），eMServer 更新会失败，并伴随解释失败原因的消息。失败的可能原因包括：

- 焊接位置缺少对应的焊点（legacy 单元为例外 — 它们会通过）。
- 同一焊接位置在同一路径中出现多次。

在「eMServer Update」操作期间，单元中的部分数据会自动更正，以符合 eMServer 流程模型从而保证数据一致性：

- 将 legacy 单元更新到 eMServer 时，会自动为焊接位置创建不存在的焊点。
- 出现在多个 mapped 路径中或在同一路径中出现多次的 Via 位置会被复制。因此，每个 via 位置恰好属于一个路径，并且在同一路径中仅出现一次。
- 不是「via」或「weld」类型的位置会被转换为 via。
- 被 mapped 焊接位置引用但未引用该焊接位置的焊点会被更正。结果，在「eMServer Update」之后，焊点引用 mapped 焊接位置。这对于 Robcad 中「Spot/Update」工具的正常操作是必要的。

<a id="v5-s154"></a>

#### 使用 Legacy 单元更新 eMServer 之前（Before Updating eMServer with Legacy Cells）

按照以下步骤在 Robcad 中准备单元：

**步骤**

1. 验证单元中的所有组件都来自库（libraries）而非项目（projects）。如果单元包含来自项目的组件，请使用 Data 工具将它们移动到库（置于库根下）：Data → Library utilities → Move to library。
2. 检查所有组件和单元是否已升级到 Robcad 6.0.2（及更高）。如果不确定数据是否已完全升级，请在 Robcad 中使用 `upgrade_to_version` 命令。
3. 在 Process Designer 中，使用 Create Engineering Libraries 命令创建带有各自层级的原型节点。您可以设置原型的类型（零件、资源），并相应地创建库。您可以将各种类型放在同一个 Library 文件夹中。
4. 零件和资源在 Hierarchy Tree 中以无层级（"flat"）形式表示。为创建层级，建议创建一个复合零件和一个复合资源，然后将相关实体拖到各自之下。
5. 建议选择您希望集成到 Process Designer 中的所有焊接路径，因为此单元的行为将与已集成到 Process Designer 的单元不同（如下所述）。

下表总结了 Robcad 与 eMServer 之间焊接操作的映射，并包含例外情况：

| 数据类型                                              | "eMServer Update" 行为 — Legacy Cell                                                                                                  | "eMServer Update" 行为 — Connected Cell                                                                        |
| ------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------ |
| 不存在 mapped 路径                                     | eMServer Update 失败。用户必须至少映射一个焊接操作才能成功。如果禁用 "Operations" 选项，eMServer Update 成功，但 eMServer 中不会创建流程数据；此后该单元对于操作的行为与 Connected Cell 相同。 | 不会在 eMServer 中创建或更新流程数据。之前映射到 eMServer 的焊接操作（路径）会从 eMServer 中移除。位置和焊点同样如此。相应的焊点从单元中删除，并从 eMServer 中的操作中取消分配。 |
| 带有焊接位置但无焊点的 mapped 路径                             | 为每个缺少焊点的焊接位置在单元和 eMServer 中创建焊点。如果焊接位置未附加到零件，新焊点直接分配给 eMServer 中的研究。                                                                | eMServer Update 失败。只允许在 Process Designer 中删除焊点。                                                              |
| 不属于任何 mapped 路径的焊点（参见 Study Weld Points Workflow） | 创建 mapped 焊点。                                                                                                                       | 创建/更新 mapped 焊点。                                                                                             |
| 之前已映射、现未映射的路径                                     | 该未映射路径从 eMServer 中移除（含其焊接和 via 位置）。路径及其位置仍保留在单元中。相应的焊点从单元中删除，并从 eMServer 中的操作中取消分配。                                                 | 同左。                                                                                                          |
| 属于多个路径或在同路径中出现多次的 via 位置的 mapped 路径               | via 位置在单元和 eMServer 中自动复制。                                                                                                          | 同左。                                                                                                          |
| 包含多个引用同一焊点的焊接位置的 mapped 路径                        | eMServer Update 失败，并在报告文件中给出相应消息。请用此消息修复有问题的路径。                                                                                     | 同左。                                                                                                          |
| 包含不属于「weld」或「via」类型的某个位置的 mapped 路径               | 中性位置在单元和 eMServer 中自动转换为 via 位置。                                                                                                    | 同左。                                                                                                          |
| 附加到子组件的焊点                                         | 在 eMServer 中自动分配给超级组件，随后在单元中分配。因为一个超级组件在 Process Designer 中表示为单个节点，所以所有焊点都必须分配给该节点。                                                 | 同左。                                                                                                          |
| 在组件（.cojt）内部建模的焊点                                 | 焊点在 eMServer 中创建。但是，如果 eMServer 中的焊点位置发生更改（通过用户交互或从 PDM/WPM 更新），这些更改在 "Cell Update" 操作后不会反映在 Robcad 单元中。                            | 同左。                                                                                                          |

<a id="v5-s155"></a>

### 使用 Legacy 单元更新 eMServer（Updating eMServer with Legacy Cells）

当使用来自 eMServer 的操作更新 Robcad 单元时，操作时间可在「Path Selection」表中查看。对于 legacy 单元，用户有责任在映射中指定正确的操作时间。如果在「Path Selection」中未指定操作时间，则该操作不带时间更新到 eMServer。如果指定了时间，「eMServer Update」过程会验证其是否大于沿该操作的所有焊接位置和冷却时间之和。如果不是，则「eMServer Update」过程失败。

如果 Process Designer 中尚不存在 RobcadStudy 文件夹，请使用以下步骤 1-2 在项目树中创建一个或多个 RobcadStudy 文件夹。这些文件夹将包含您从 Robcad 导入到 Process Designer 的数据。

**步骤**

1. 在项目树中右键单击，选择 New > Study folder。
2. 右键单击您创建的 Study folder，并选择 New → RobcadStudy（可以指定多个文件夹）。
3. 选择 Robcad Studies 文件夹后，选择 Applications 选项卡 → Robcad 组 → Robcad Connectivity Studies（或者，右键单击 Robcad Study 节点并从上下文菜单中选择 Robcad Connectivity Studies 命令）。
4. 在 Robcad Studies 对话框中，高亮一个 RobcadStudy 并单击 Set NFS location。您可以浏览选择 Robcad legacy 单元。选择单元后，单元名称显示在 Robcad Cell 列中，NFS 路径显示在 NFS Location 列中。
5. 单击 Settings 以设置 Reports 文件夹位置。每次更新后，系统会向现有报告添加信息，而不会覆盖它。建议将 Robcad 库根置于系统根下。当 Robcad 单元包含与该单元不再相关的研究信息时（例如单元被复制重命名或使用 Robcad Save as 功能保存，或者原始 eMServer 研究被删除），请使用 Force reconnect 选项。Force reconnect 选项会从单元文件中移除当前不相关的研究信息，以使系统能够将单元重新连接到新的 eMServer 研究。
6. 单击 Update eMServer 以打开一个对话框，在其中指定要从 Robcad 单元导入的元素类型。导入会将信息写入现有的 Robcad 单元，并使用以下命名格式自动备份 Robcad 单元：`<cellname>.ce.bim_ddmmyy_hhmmss`。导入完成后，Process Designer 会通知您操作成功，或在导入失败时提醒您存在问题。您可以查看报告以获取详细信息。单击 Close 后，项目树被更新。项目树现在包含一个名为「<登录用户名> 的文件夹」的新文件夹，其中包含名为「Imported From <NFS 路径和 Unix 单元名>」的子文件夹，其中包含从 Robcad 导入的新实例。此外，RobcadStudy 文件夹现在包含指向导入实例的快捷方式。
   > **注意**：用户施加的层级在首次导入 Robcad 单元时从 legacy 单元导入到 Process Designer 树。对于在 eMServer 菜单 Hierarchy Tree 工具箱用于 legacy 数据的情况，请注意 Robcad 树工具箱中的任何组实例都不会显示在 Hierarchy Tree 中。所有 legacy 组实例均按上述方式处理。在 eMServer 使用单元数据更新后，所有实例在 Hierarchy Tree 中变为可见。

**关于超级组件的说明（Notes about super components）**

要使用超级组件，必须使用以下设置：

- 超级组件及其子组件（Children）必须是库组件，而非项目组件。
- 库根（在建模阶段，用于定义超级组件）必须位于 Process Designer 的系统根之下。库根不能与系统根相同（此限制仅在使用超级组件时适用）。

示例环境结构：

```
-C:\project\systemroot
  LibraryRoot
    Resources
      comp1.co
      comp2.co
      super1.co
```

在此示例中，您需要执行以下操作：

- 在 Process Designer 中，设置 System root = c:\project\SystemRoot
- 在 Process Simulate 中，设置 Library root = c:\project\SystemRoot\LibraryRoot

超级组件可在 Process Simulate 中显示，但无法在 Process Designer 3D 查看器中显示。

**关于缺失库组件的说明（Note about missing library components）**

在某些情况下，legacy 单元可能包含因各种原因已不存在的库组件引用。当集成遇到缺失组件时，其行为如下：

- 如果所有单元组件都缺失，可能是由于 Robcad Studies 对话框中的 Robcad Library root 设置不正确。在这种情况下，集成失败，并在集成报告文件中打印以下消息：`ERROR: Unable to load any of the components contained in the cell. Please check the library root setting.`
- 如果部分组件缺失，集成会检查这是否为 legacy 单元。对于 legacy 单元，集成继续执行并跳过缺失的组件。如果单元已连接到 eMServer（即不是 legacy 单元），集成失败。无论哪种情况，报告文件都包含集成遇到的缺失组件通知。

<a id="v5-s156"></a>

### Robcad-eMServer 集成中的附件支持（Attachments Support Within Robcad-eMServer Integration）

对于 Robcad 与 Tecnomatix 7.6（或更高）版本之间的集成，受集成支持的对象之间的所有附件都将在两个方向（共享）传输，以下情况除外。（之前，集成仅支持有限数量的附件类型，例如安装（mounts）、焊点和焊接位置的附件，以及 OLP 仿真中带有附件命令的附件。其他类型的附件信息不共享，因此在 Robcad 端完成的附件在完整集成周期后仍保留在 Robcad 端，但这些信息不会反映在 Digital Manufacturing 应用程序中，反之亦然。）

> **注意**：附件信息不会传输给集成不支持的对象（例如项目组件）。附件信息保存在 Engineering Data 中（mount 数据除外，其存储在 eMServer 中）。请注意，在从 Robcad 到 eMServer 的集成过程中，实体（entity）级别的附件会移动到 Robcad 单元及相应研究中更高级别（组件、链接）。

<a id="v5-s157"></a>

### 完整附件支持的要求（Requirements for Full Attachment Support）

> **重要**：要实现附件支持，用户必须运行从 Tecnomatix 7.6 及更高版本的 Upgrade to Version（参见 Tecnomatix Administration 文档）。支持向后兼容。未运行 Upgrade to Version 但使用 Robcad-eMServer 集成的用户将无法共享子组件链接的附件，并且会在 Robcad 单元中丢失这些附件。

<a id="v5-s158"></a>

### 在 eMServer 与 Robcad 之间交换属性（Exchanging Attributes Between eMServer and Robcad）

eMServer-Robcad 集成（7.6 或更高版本）能够传输属性信息（软字段 soft field 或硬字段 hard field），无论是从 eMServer 到 Robcad，还是反之。属性信息传输基于一个预定义的 XML 文件，用户在文件中针对每个类设置参与集成的属性列表（及其类型）。XML 文件中不存在的属性在集成期间不会被交换。

属性交换机制不区分 eMServer 中的 hard 字段和 soft 字段，也不知道定义每个属性的 Robcad 应用程序。一旦属性与对象关联，它将保持这种状态，直到被显式移除。

属性仅为集成对象传输。无法为集成未更新的对象（例如原型）传输属性。

<a id="v5-s159"></a>

#### 在 eMServer 与 Robcad 之间传输属性信息（Transferring Attribute Information Between eMServer and Robcad）

> **注意**：如果部分组件缺失，集成会检查这是否为 legacy 单元。对于 legacy 单元，集成继续执行并跳过缺失的组件。如果单元已连接到 eMServer，集成失败。无论哪种情况，报告文件都包含缺失组件的通知。

以下表格列出了集成对象：

| eMServer 对象（支持派生对象类型）   | Robcad 对象               |
| ----------------------- | ----------------------- |
| Robcad study            | Robcad cell             |
| Part instance           | Part instance           |
| Tool instance           | Tool instance           |
| Compound part           | Compound part           |
| Compound resource       | Compound resource       |
| Weld point              | Weld point              |
| Weld location operation | Weld location operation |
| Via location operation  | Via location operation  |
| Weld operation          | Weld path               |
| Part placeholder        | Part placeholder        |
| Resource placeholder    | Resource placeholder    |

> **注意**：尽管焊接位置和 via 位置是集成对象，但无法在焊接和 via 位置上传输属性（属性传输机制不支持）。使用属性传输机制，用户可以通过将原型属性写入 Robcad 单元中相应原型的所有实例（而非原型本身），将原型属性从 eMServer 传输到 Robcad。
>   
> **重要**：原型属性的传输仅支持 eMServer 到 Robcad 方向，因此不可能在 Robcad 中更改这些 Robcad 属性的值，然后将其传回 eMServer 中的原型。
>   
> <a id="v5-s160"></a>

#### 集成对象列表（List of Integrated Objects）

属性交换机制不区分 eMServer 中的 hard 字段和 soft 字段，也不知道定义每个属性的 Robcad 应用程序。一旦属性与对象关联，它将保持这种状态，直到被显式移除。

属性仅为集成对象传输。无法为集成未更新的对象（例如原型）传输属性。

> **重要**：原型属性的传输仅支持 eMServer 到 Robcad 方向。因此，如果在 Robcad 单元中的实例上更改了这些属性的值，会出现以下问题：
>
> - 在 Robcad 到 eMServer 的集成期间，该信息不会传输到 eMServer，因此在下一次从 eMServer 到 Robcad 的集成迭代中，这些更改会被原型属性覆盖。
> - Robcad 原型属性不会传输到 eMServer 原型属性。
> - eMServer 原型属性不会传输到 Robcad 原型属性。

（有关集成对象列表，请参阅上一节「在 eMServer 与 Robcad 之间传输属性信息」中的表格。）

<a id="v5-s161"></a>

#### XML 文件（The XML File）

**XML 文件说明（Description of XML File）**

XML 文件位于库根（library root）下的 system 文件夹中，以便也能从 Robcad 访问。只有文件系统管理员才具有更改此文件所需的权限。用户应确保 XML 中的所有属性类型与定制中的字段类型一致（基于「已知限制（Known Limitations）」中的转换表）。可以使用部分字段在定制中不存在的 XML（这些字段将被忽略）。

您应使用一个应用程序（7.6 及更高版本）自动生成 XML 文件，请参阅「用于生成属性交换 XML 文件的应用程序（Application for Generating XML File for Exchanging Attributes）」。该应用程序是 Process Designer 中 Robcad 类别下的一个命令。强烈建议使用此应用程序构建 XML 文件；此 XML 文件绝不能手动编辑 — 始终使用该应用程序的 UI。

XML 文件的格式如下：

```xml
<Classes>
  <classclassName="class1">
    <field>
      <fieldName>field1_1</fieldName>
      <RobcadAttrName>attrName1_1</RobcadAttrName>
      <RobcadAttrType>tID</RobcadAttrType>
    </field>
    <field>
      <fieldName>field1_2</fieldName>
      <RobcadAttrName>attrName1_2</RobcadAttrName>
      <RobcadAttrType>tString</RobcadAttrType>
    </field>
  </class>
  <classclassName="class2">
    <field>
      <fieldName>field2_1</fieldName>
      <RobcadAttrName>attrName2_1</RobcadAttrName>
      <RobcadAttrType>tName</RobcadAttrType>
    </field>
    <field>
      <fieldName>field2_2</fieldName>
      <RobcadAttrName>attrName2_2</RobcadAttrName>
      <RobcadAttrType>tFile</RobcadAttrType>
    </field>
  </class>
</Classes>
```

- **className** 是以下 eMServer 类型之一：Robcad study、Part instance、Tool instance、Compound part、Compound resource、Weld point、Weld operation、Part prototype、Tool prototype、Part placeholder 或 Resource placeholder，或继承自这些类的类。
- **fieldName** 是用户希望在 Robcad 与 eMServer 之间交换的 eMServer 字段名称（针对指定的类名）。
- **RobcadAttrName** 是 Robcad 中保存该字段信息的属性名称（与 Robcad 属性文件中的属性名称相同）。
- **RobcadAttrType** 是 Robcad 属性的 Robcad 类型。

对于每个类（className），XML 文件指定了哪些字段将在 Robcad 与 eMServer 之间交换。（在 class1 的示例中：field1_1 和 field1_2 在 Robcad 与 eMServer 之间交换。）对于每个 eMServer 字段，指定了在 Robcad 中保存该字段信息的对应属性的名称和类型。

**XML 文件的限制（Limitations of XML File）**

用户应依据以下限制构建 XML，并确保其以正确方式构建：

- **通用 XML 限制（General XML Limitations）**：确保 XML 文件中每个类部分的每个字段名称唯一（每个字段名称在特定类部分中只出现一次），并且每个类部分中每个属性名称唯一。冲突的属性和字段可能会被覆盖。在 eMServer 端，不同的类可以拥有名称相同但类型定义不同的字段，而 Robcad 不支持这一点。作为变通，可在不同类之间使用相同的 eMServer 字段名称但不同的 Robcad 类型和不同的 Robcad 属性名称；如果 Robcad 类型相同，也可以在不同类之间使用相同的 eMServer 字段名称和相同的 Robcad 属性名称。
- **与 Robcad 属性文件相关的 XML 限制（XML Limitations Related to Robcad Attributes File）**：用户有责任在 Robcad 属性文件中写入 XML 文件中写入的每个 Robcad 属性。如果有两个不同的属性文件 — 一个用于 Tecnomatix，一个用于 Robcad — 则用户应将这些属性添加到两个文件中。用户应确保 Robcad 属性文件中属性的类型与相应字段的 eMServer 类型一致，并且每个 Robcad 属性在属性文件中只出现一次（不能在同一 Robcad 属性文件中将同一个 Robcad 属性 X 一次定义为类型 A，一次定义为类型 B）。创建 XML 文件的应用程序会将应添加到 Robcad 属性文件（采用 Robcad 属性文件格式）的属性写入报告（见「错误处理与报告（Error Handling and Report）」）。应使用此报告中的列表验证所有这些属性是否以正确格式存在于 Robcad 属性文件中。不要简单地进行复制粘贴 — 某些属性可能已存在于 Robcad 属性文件中。字段类型应与定制中的类型以及 Robcad 属性文件中相应 Robcad 属性的类型一致。在集成期间会检查这些类型的一致性；如果不一致，集成失败并显示相应的错误消息。
- **仿真参数 XML 限制（Simulation Parameters XML Limitations）**：如果 XML 文件中的某个 Robcad 属性名称是保存仿真参数信息的属性名称，则该参数信息（由集成传递）会被相应的 XML 字段值覆盖。例如，参数 A 的信息写入 Robcad 的属性 A' 上，而 XML 文件声明 eMServer 中的字段 A'' 的值应写入属性 A'。到从 eMServer 到 Robcad 的集成结束时，属性 A' 的值为字段 A'' 的值；如果稍后在 Robcad 到 eMServer 方向执行集成，参数 A 会丢失其原始值而获得字段 A'' 的值。字段值优先于参数值，因为在从 Robcad 到 eMServer 的集成中，所有未知属性都作为 eMServer 中的参数写入。
- **仿真命令/Spot 属性 XML 限制（Simulation Commands/Spot Attributes XML Limitations）**：如果 XML 文件中的某个 Robcad 属性名称是保存仿真命令信息（OLP 字符串）的属性名称，则该命令信息（由集成传递）不会被相应的字段值覆盖。在这种情况下（当 XML 文件中给出在 Robcad 中保存命令信息的 Robcad 属性名称时），在 eMServer 到 Robcad 方向不会使用该字段值更新此属性；在 Robcad 到 eMServer 方向也不会使用该属性值更新字段（因为该属性的信息已作为仿真信息中的命令存储在 eMServer 中）。在这种情况下，报告中会给出相应的通知消息（见「错误处理与报告」）。尽管命令值在集成期间不会被覆盖（与参数值不同），但用户不应将命令放入 XML 文件，因为它们在该文件中没有意义（在 XML 文件中插入参数也同样如此）。

<a id="v5-s162"></a>

#### eMServer 中的字段（Fields in the eMServer）

与仿真命令的情况一样，即使 XML 文件包含的 Robcad 属性名称已由集成传输（例如 `SW_CREATION_POINT` 等 spot 属性，或保存已知仿真参数信息的 `RRS_TOOL_FRAME` 等属性名称），它们也不会在属性传输机制中传输，并且会在报告中添加相应的通知消息。有关不被属性传输机制支持的所有 Robcad 属性列表，请参阅「属性传输机制不传输的 Robcad 属性列表（List of Robcad Attributes that are not Transferred by Attributes Transfer Mechanism）」。

**集成字段限制（Integration Fields Limitations）**

用户不应将集成字段（例如仿真信息、位置、父节点）放入 XML 文件。在这种情况下，在从 Robcad 到 eMServer 的集成中，这些字段的值会被 Robcad 属性中的值覆盖，这是不正确的结果。用户有责任不在 XML 文件中定义任何由其自身机制更新的集成字段（例如仿真信息、位置、父节点）。

**零件原型与工具原型属性传输（Part Prototype and Tool Prototype Attributes Transfer）**

如果 XML 中的类名是零件原型或工具原型，则在从 eMServer 到 Robcad 的集成期间，字段信息会写入其对应原型的所有原型实例的相应属性。（如果 Robcad 端某实例上不存在该属性，则创建它；如果存在，则修改其值。）

> **注意**：在这种情况下，集成是单向的（Robcad 到 eMServer 的集成期间不会更改 eMServer 原型属性）。任一方向的集成都会向报告添加一条通用消息（见「错误处理与报告」），说明这些原型属性的集成是单向集成。
>   
> **注意**：如果用户希望同时传输字段 X 的原型（例如焊枪）和实例（例如工具实例）信息，但在 XML 中定义字段 X 的信息在两种情况下都写入同一个 Robcad 属性 Y，则只有实例字段的信息在 eMServer 到 Robcad 以及反向上传输。将原型信息和实例信息都从 eMServer 传输到 Robcad 的正确过程是使用两个 Robcad 属性：一个用于原型信息，另一个用于实例信息。

> **重要**：eMServer 字段是针对类定义的，而不是针对实例。因此，即使用户尝试仅为类 A 的一个实例添加字段，该字段也会添加到类 A 的所有实例以及继承自类 A 的所有类中。用户应注意，这可能会因为添加的字段数量巨大（每个字段是数据库中的一列）而导致服务器性能下降。

<a id="v5-s163"></a>

#### 已知限制（Known Limitations）

在 Robcad 中，属性放置在对象上，而不是类上。

- 一旦在属性文件中定义了属性类型（并且已被集成使用，即单元中已存在这样的属性），就不可能再更改该属性类型。
- Robcad 不支持宽字符（wide characters），因此如果 eMServer 端某字段的值包含此类字符，数据在 Robcad Attributes Editor 中不会以正确格式显示。但是，数据本身在集成期间不会损坏，并且会以正确格式返回 eMServer。
- 两个环境（eMServer 和 Robcad）并不支持所有属性类型。如果某个属性类型只存在于一端而不存在于另一端（例如 eMServer 中的 date 类型，或 Robcad 中的 tFrame 类型），集成不支持传输此属性。
- 受支持的属性类型将按如下表格映射：

| eMServer 类型 | Robcad 类型                                       |
| ----------- | ----------------------------------------------- |
| Double      | Double                                          |
| Float       | tReal                                           |
| Integer     | tInt                                            |
| Boolean     | tBool                                           |
| String      | tString、tID（256 字符）、tName（256 字符）、tFile（256 字符） |

集成会检查 XML 中为每个对象定义的 eMServer 字段，以验证字段类型（基于定制）与相应属性的 Robcad 类型（基于 Robcad 属性文件）的组合在映射中受支持。如果不支持，集成失败并向报告添加错误消息。

- **字符串类型**：在 eMServer 到 Robcad 方向，如果 eMServer 字段的字符串值超过相应 Robcad 属性类型的尺寸限制，集成失败并向报告添加相应错误消息。在 Robcad 到 eMServer 方向，如果 Robcad 属性的字符串值超过相应 eMServer 字段的尺寸限制，集成在导出到 eMServer 时失败并添加相应错误消息。在 Robcad 到 eMServer 方向，如果有多个 Robcad 属性值超过相应 eMServer 字段的尺寸限制，报告仅针对其中一个添加错误消息。
- 由于在 eMServer 端可以将每个属性定义为单个值或向量（无限制数组大小），而 Robcad 端并非所有属性类型都支持这一点，因此仅支持单个值。集成不支持传输向量 eMServer 字段，并向报告写入通知。
- 由于没有简单的方法可以知道哪些实例字段是用户在 eMServer 中显式设置的（因为字段在用户显式设置之前具有默认值），因此从 eMServer 到 Robcad 的实例字段信息（即使未被用户显式设置）也会被传输；但请注意，这可能会向 Robcad 单元添加不必要的属性。
- 字符串类型的默认值是空字符串（""），这是无意义的。因此，值為 "" 的字符串字段不会从 eMServer 传输到 Robcad（这也减少了单元中不必要的属性数量）。
- **重要**：用户可以更改任意类型字段在定制中的默认值。例如，如果将字符串字段 X 的默认值更改为 "mmm"，则在用户为这些实例显式设置字段 X 之前，所有实例在字段 X 中的值都为 "mmm"。在这种情况下，如果字段 X 在 XML 文件中定义，则在 eMServer 到 Robcad 的集成期间，字段 X 将作为所有实例的属性写入。
- 即使字符串字段 X 的默认值不是 ""，在 eMServer 到 Robcad 的集成期间值为 "" 的字符串字段也不会传输。此外，如果相应属性已存在于 Robcad 单元的实例上，则对于字段 X 值为 "" 的所有实例，该属性会在 eMServer 到 Robcad 的集成期间被移除。
- 整数、浮点和双精度字段的默认值为 0（如果在定制中未重新定义）。由于 0 可能具有意义，值为 0 的字段在集成期间会被传输（即使可能未被用户显式设置）。
- 无法从实例的字段中移除 eMServer 中的值。因此，在 Robcad 中从特定实例移除 Robcad 属性，并不会导致相应字段从 eMServer 中的实例移除。结果，在下一次从 eMServer 到 Robcad 的集成迭代中，被移除的属性会被重新写入 Robcad 中的特定实例。但是，如果用户在 Robcad 单元中从实例移除了字符串属性，则在 Robcad 到 eMServer 的集成期间，相应字段中会被放入空字符串（""）。在这种情况下，在随后的从 eMServer 到 Robcad 的集成中，被删除的属性不会重新写入 Robcad 单元中的特定实例。
- 以下适用于（Integer, tBool）映射的情况：非零的 eMServer 字段被写为保存 true 值的 Robcad 属性，零 eMServer 字段被写为保存 false 值的 Robcad 属性。相应地，false 的 Robcad 属性在相应 eMServer 字段上写入 0，true 的 Robcad 属性写入 1。

<a id="v5-s164"></a>

#### 属性传输机制不传输的 Robcad 属性列表（List of Robcad Attributes that are not Transferred by Attributes Transfer Mechanism）

以下是属性传输机制中不传输的 Robcad 属性列表：

- SW FILE VERSION
- SW LOC TYPE
- SW CREATION POINT
- SW WP LOC NAME
- SW TIME ON PT
- SW LOC ORIGINAL PERPENDICULAR POS X
- SW LOC ORIGINAL PERPENDICULAR POS Y
- SW LOC ORIGINAL PERPENDICULAR POS Z
- SW LOC ORIGINAL PERPENDICULAR R
- SW LOC ORIGINAL PERPENDICULAR P
- SW LOC ORIGINAL PERPENDICULAR Y
- SW LOC DEVIATION ANGLE
- PP OPERATION TIME
- PP ROBOT NAME
- PP GUN NAME
- EMS INTEG NEW WP TMP
- EMS INTEG MOVED WP TMP
- SW UPDATED WP
- SW WAIT TIME
- SW MOTION TIME
- OLP STRING NUM
- OLP STRING ##
- MountED WORKPIECE FRAME NAME
- RRS TOOL FRAME
- RRS OBJECT FRAME

<a id="v5-s165"></a>

### 用于生成属性交换 XML 文件的应用程序（Application for Generating XML File for Exchanging Attributes）

Process Designer 提供了一个用于生成属性传输机制所用 XML 文件的应用程序（用于在 eMServer 与 Robcad 之间交换属性）。该应用程序是 Process Designer 中 Robcad 类别下的一个命令。强烈建议使用此应用程序生成 XML 文件，绝不要手动编辑生成的属性映射 XML 文件。

<a id="v5-s166"></a>

#### 生成用于属性传输的 XML 文件（Generating XML File for Attributes Transfer）

Process Designer 提供了一个应用程序，用于生成属性传输机制所使用的 XML 文件（用于在 eMServer 与 Robcad 之间交换属性）。属性传输机制读取此生成的 XML，以确定在从 eMServer 到 Robcad 传输时，针对每个类应传输哪些 eMServer 字段以及传输到哪些 Robcad 属性。在从 Robcad 到 eMServer 传输时，Robcad 属性信息被传输到 eMServer 字段。

> **重要**：属性映射 XML 文件绝不应手动编辑 — 始终使用该应用程序的 UI。

<a id="v5-s167"></a>

#### 应用程序概述（General Description of the Application）

该应用程序是 Process Designer 中 Robcad 类别下的一个命令。用户应浏览选择生成的 XML 将存储于其下的库根。用户必须具有在 Robcad Library Root 文件夹下写入的权限；如果用户没有此类权限，在单击 Apply 时会给出相应的弹出错误消息。

该应用程序将 XML 文件存储在库根下的 system 文件夹中，并将其命名为 `RobcadeMSAttributeMapping.xml`（如果 system 文件夹不存在，应用程序会创建它）。

> **注意**：如果此 XML 文件已存在，在第一阶段会读取其数据，并用这些数据初始化应用程序 UI。只有在第二阶段，单击 Apply 才会用应用程序数据覆盖 XML 文件的全部内容。

例如，如果用户在 XML 文件已生成后希望更改某个字段，用户可以打开应用程序，浏览到相关的库根，并在应用程序中更改该一个字段的映射，而无需重新映射任何其他字段。（更改多个字段时应遵循类似的过程。）

填写所有适用字段后，用户应单击 Apply。

> **注意**：在用户选择库根并在应用程序 UI 中 mappings 进行任何更改后，选择不同库根的选项将变为禁用，直到用户通过单击 Apply 保存这些更改。因此，从某个库根下的映射 XML 文件加载数据、进行更改，然后将这些更改存储到不同库根下的映射 XML 文件的正确过程如下：
>
> - 将第一个映射 XML 文件显式复制到第二个库根下的 system 文件夹。
> - 在应用程序中选择第二个库根。
> - 在 UI 中执行映射更改。

<a id="v5-s168"></a>

#### eMServer - Robcad 属性映射（eMServer - Robcad Attribute Mapping）

**支持的 eMServer 类（Supported eMServer Classes）**

eMServer - Robcad Attribute Mapping 对话框的左窗格包含 eMServer Classes 面板。它列出了从当前定制读取的、受属性传输机制支持的 eMServer 类。属性传输支持以下 eMServer 类：

- WeldPoint，以及继承自 WeldPoint 的类
- WeldOperation，以及继承自 WeldOperation 的类
- CompoundPart，以及继承自 CompoundPart 的类
- PartInstance
- PartPrototype，以及继承自 PartPrototype 的类
- CompoundResource，以及继承自 CompoundResource 的类
- ToolInstance
- ToolPrototype，以及继承自 ToolPrototype 的类
- RobcadStudy
- PartPlaceholder，以及继承自 PartPlaceholder 的类
- ResourcePlaceholder，以及继承自 ResourcePlaceholder 的类

> **注意**：属性传输不支持 PartInstance、ToolInstance 和 RobcadStudy 的派生类（因为 Robcad-eMServer 集成不支持这些类）。因此，无法在对话框中浏览它们或在左窗格中选择这些派生类。此外，该应用程序不支持传输焊接和 via 位置的属性。因此，尽管这些类型受集成支持，但它们不会出现在左窗格中。

**eMServer 字段选择（eMServer Fields Selection）**

当用户选择一个类时，其所有 eMServer 字段（类型为受支持的 eMServer 类型：double、float、integer 和 string 的单值）及其 eMServer 类型（基于定制）会在右窗格（Selected Class Attributes Mapping）中可见。

如果用户选择一个派生类，根据 Show Inherited Fields 按钮（对话框右上方按钮），有两种可能模式：

- 单击 Show Inherited Fields 会使派生类本身及其所有父类的受支持 eMServer 字段在右窗格中可见。
- 如果未单击 Show Inherited Fields，则仅可见派生类本身的受支持 eMServer 字段（不包括其父类的字段）。

然后用户应为该类指定要在属性传输机制中传输的字段。要指定字段，请为每个所需字段选择左列（Map）中的复选框。

> **注意**：Tune-eMServer 集成使用的字段不会显示在 UI 的右窗口中（因此用户将无法映射它们）。有关这些字段的列表，请参阅「Tune-eMServer 集成使用的字段（Fields Used by Tune-eMServer Integration）」。

**填写 Robcad 属性名称并选择 Robcad 类型（Filling Robcad Attribute's Name and Selecting Robcad Type）**

选择字段后，用户应填写 Robcad 属性名称并选择 Robcad 类型。（Robcad Attribute name 字段和 Robcad Type 字段仅在选中字段的复选框后启用。）

有效的 Robcad 名称应以字母（大写或小写）开头，其余字符应仅由字母、数字和下划线（"\_"）组成。

用户只能从适合相应 eMServer 字段类型的 Robcad 类型中选择：

- 对于字符串 eMServer 字段，用户从以下类型中选择：tString、tID、tName 或 tFile。
- 对于整数 eMServer 字段，用户从以下类型中选择：tInt 或 tBool。
- 对于 double 或 float eMServer 字段，Robcad Type 字段只有一个选项可用 — 分别为 double 或 tReal。在这些情况下，当用户进入该字段的 Robcad Type 列时，会自动为用户选择该唯一可用选项。

在用户完成某一行填写并尝试执行另一个操作（转到另一个字段行、转到 eMServer Classes 部分的另一个类、更改继承字段模式或单击 Apply）后，会检查该行的有效性。在以下任何情况下，该行被视为无效：

- 未完全填写的行（已勾选 Map 复选框，但 Robcad Attribute 和/或 Robcad Type 列未填写）。
- Robcad 名称无效的行。
- Robcad 名称是属性传输机制中不受支持的属性的行（请参阅「属性传输机制不传输的 Robcad 属性列表」）。
- Robcad 名称已出现在同一类的另一个映射中的行（它映射到同一个类中的另一个字段）。
- Robcad 名称已出现在另一个具有不同 Robcad 类型的映射中的行（即使该映射出现在不同的类中）。

如果当前行无效，会显示信息性弹出错误消息，在修正无效行之前用户无法继续。

> **注意**：当用户单击 Close、X 或 Reset 时，不执行这些有效性检查。

**Robcad 属性文件（Robcad Attributes Files）**

Robcad 属性文件在 eMServer-Robcad 集成期间被读取。系统检查 eMServer 字段类型是否与相应 Robcad 属性（在属性文件中定义的类型）的实际 Robcad 类型一致。如果不一致，会显示错误消息。

<a id="v5-s169"></a>

#### 错误处理与报告（Error Handling and Report）

**为父类的字段设置映射（Setting a Mapping for a Field of a Parent Class）**

> **注意**：当用户为某个基类设置 eMServer 字段与 Robcad 属性之间的映射时，所有继承自该基类的类也会自动被映射，无论用户是否单击了 Show Inherited Fields。因此，如果移除基类的映射，则所有继承自该基类的类的映射也会被移除。类似地，更改基类的映射也会更改所有继承自该基类的类的映射。用户可以在派生类本身中更改继承的映射字段，但对基类中该字段的后续更改（或其移除）会删除该更改，映射再次从基类继承。

单击 Apply 会根据用户填写的数据生成 XML 文件。在此阶段会检查 XML 文件和报告文件的写入权限。XML 文件生成过程结束时，会显示弹出消息说明 XML 文件生成是否成功，用户可以查看报告。报告文件与 Robcad-eMServer 集成相同，其在文件系统中的位置由 Robcad-eMServer 集成主窗口决定。属性映射应用程序从注册表中读取此信息。应用程序完成时不自动关闭应用程序窗口，以便失败时用户不必重新定义所有数据。

**报告内容（Report Content）**

应用程序将映射的所有 Robcad 属性及其类型（采用 Robcad 属性文件格式）的列表写入报告。例如，基于 eMServer - Robcad Attribute Mapping 中 UI 示例的映射 Robcad 属性列表为：

```
YYYY1 double -1
YYYY2 double -1
YYYY3 tName -1
YYYY4 double -1
```

> **注意**：用户有责任确保此列表中的所有属性以相同格式（具有相同、正确的类型）存在于其中一个 Robcad 属性文件中，并添加所有在 Robcad 属性文件中不存在的 Robcad 属性。应使用此报告中的列表验证所有这些属性是否以正确格式存在于 Robcad 属性文件中。不要简单地进行复制粘贴 — 某些属性可能已存在于 Robcad 属性文件中。请注意，如果有两个属性文件 — 一个用于 Tecnomatix，一个用于 Robcad — 则用户应将这些属性添加到两个文件中。Robcad 属性的实际类型（基于 Robcad 属性文件）仅在集成本身读取 Robcad 属性文件时才被检查。有关应用程序生成的 XML 文件示例，请参阅「生成的 XML 文件示例（Generated XML File Example）」。

<a id="v5-s170"></a>

#### 生成的 XML 文件示例（Generated XML File Example）

以下是生成的 XML 文件示例。它包含一个声明 XML 文件版本的表头。

> **注意**：此示例的目的仅在于展示 XML 文件的格式。其中的所有数据（类名称、字段名称、Robcad 属性名称、Robcad 属性类型）均无意义，应忽略。

```xml
<AttributesMappingfileVersion="1.0">
  <Classes>
    <classclassName="WeldPoint">
      <field>
        <fieldName>sectionDepth</fieldName>
        <RobcadAttrName>YYYY1</RobcadAttrName>
        <RobcadAttrType>type1</RobcadAttrType>
      </field>
      <field>
        <fieldName>force</fieldName>
        <RobcadAttrName>YYYY2</RobcadAttrName>
        <RobcadAttrType>type2</RobcadAttrType>
      </field>
      <field>
        <fieldName>gunStateExist</fieldName>
        <RobcadAttrName>YYYY3</RobcadAttrName>
        <RobcadAttrType>type3</RobcadAttrType>
      </field>
      <field>
        <fieldName>gunTime</fieldName>
        <RobcadAttrName>YYYY4</RobcadAttrName>
        <RobcadAttrType>type4</RobcadAttrType>
      </field>
    </class>
    <classclassName="Car_partModule">
      <field>
        <fieldName>DBL_Nr</fieldName>
        <RobcadAttrName>XXXX1</RobcadAttrName>
        <RobcadAttrType>type5</RobcadAttrType>
      </field>
      <field>
        <fieldName>DocuID</fieldName>
        <RobcadAttrName>XXXX2</RobcadAttrName>
        <RobcadAttrType>type6</RobcadAttrType>
      </field>
    </class>
  </Classes>
</AttributesMapping>
```

<a id="v5-s171"></a>

#### Tune-eMServer 集成使用的字段（Fields Used by Tune-eMServer Integration）

Tune-eMS 集成使用的 eMServer 字段不受属性传输机制支持（因此不会显示在当前应用程序的 UI 中）。这些字段列于下表中。该表由两列组成：左列为类名，右列为当前类及继承自当前类的类在 UI 中不显示的字段。

| 类名（Class Name）                    | 不支持的字段（Non-supported fields）                                                                                                                                                                                                 |
| --------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| PmCompoundOperation               | children                                                                                                                                                                                                                     |
| PmCompoundPart                    | children、threeDRep、variant                                                                                                                                                                                                   |
| PmCompoundResource                | children、threeDRep                                                                                                                                                                                                           |
| PmMfgFeature                      | operatesOn、location、rotation、holdingTime、motionTime、actionTime、threeDRep、usedBy、variant                                                                                                                                      |
| PmNode                            | name、comment、caption、parent、parents、family、parentPrototype、collections                                                                                                                                                       |
| PmOperation                       | allocatedTime、verifiedTime、usages、outputFlows、inputFlows、scopeInputFlows、scopeOutputFlows、assignedTo、picture、steps、wiPartsInfo、wiAspect、partPrototypeUsages、toolPrototypeUsages、mfgUsages、plcInfo、LineSimulationInfo、variant |
| PmPart                            | layout、variant                                                                                                                                                                                                               |
| PmPartInstance                    | prototype                                                                                                                                                                                                                    |
| PmPartPlaceholder                 | threeDRep                                                                                                                                                                                                                    |
| PmPartPrototype                   | children、threeDRep、weight、boundedBoxMin、boundedBoxMax、externalAspect、variant                                                                                                                                                 |
| PmResource                        | layout、variant                                                                                                                                                                                                               |
| PmResourcePlaceholder             | threeDRep                                                                                                                                                                                                                    |
| PmStudy                           | attachments、info                                                                                                                                                                                                             |
| PmToolInstance                    | prototype、externalAspect、plcInfo                                                                                                                                                                                             |
| PmToolPrototype                   | children、threeDRep、weight、boundedBoxMin、boundedBoxMax、externalAspect、variant                                                                                                                                                 |
| PmLocationOperation               | position、relativeTo                                                                                                                                                                                                          |
| PmWeldLocationOperation           | mountedWorkPiece、projectedLocation                                                                                                                                                                                           |
| PmWeldOperation                   | mountedWorkPiece                                                                                                                                                                                                             |
| PmGenericRoboticLocationOperation | operationType                                                                                                                                                                                                                |
| PmProcessResource                 | process                                                                                                                                                                                                                      |

<a id="v5-s172"></a>

### 更新 eMServer 时的自动原型创建（Automatic Prototype Creation when Updating eMServer）

您可以使用包含尚未在 eMServer 中具有相应原型的 .cojt 文件实例的单元来更新 eMServer。在 Robcad 单元中首先为这些实例设置类型后，集成会自动在 eMServer 中创建原型，而无需事先运行 Create Engineering Libraries。

> **注意**：即使 eMServer 定制已在先前版本中导出到 Robcad（每当定制发生变化时都是如此），在使用 Define eMServer Type 命令之前，您必须再次执行 Exporting eMServer Customization to Robcad。

**步骤**

1. 向单元添加无类型的组件后，打开 Hierarchy tree 工具箱。在显示的 Hierarchy Tree 中，新原型以特殊图标显示，表明尚未为其分配类型。
2. 在已连接单元的 Hierarchy Tree 中，选择一个或多个在 eMServer 中仍无原型的未连接实例。从 Edit 菜单或右键上下文菜单中，选择 Define eMServer Type 命令。将显示 Select Type 对话框。
   > **注意**：使用多项选择时，如果命令无法对所选的任一项目操作，则整个选择都会禁用该命令。
3. 从列表中选择一个类型。列表中的类型即类名，它们并非唯一，可能会多次出现并关联不同的图标。系统为属于同一原型的所有实例分配相同的图标。之后，如果您添加了一个其单元中已有实例且已分配类型的组件，则下次加载 Hierarchy Tree 时，新组件会被分配相同的类型和图标。类似地，如果在加载 Hierarchy Tree 之前更新 eMServer，集成会完成单元的类型和图标分配。

在更新 eMServer 时，集成会在您的工作文件夹下自动创建新的原型库，并创建新原型与其实例之间的连接。

> **注意**：如果您在分配类型后、尚未使用新原型更新 eMServer 之前，可以更改类型。创建 eMServer 原型要求用户具有创建或更新 TuneData.xml 文件（位于 .cojt 文件下）的写权限。

<a id="v5-s173"></a>

### 启用原型更改（Enabling Prototype Change）

连接 Robcad 单元后，可以通过以下两种方式之一在 eMServer 中更改现有的 Robcad 单元实例：

- 导入带有新原型的 xml 文件。
- 指向 3D 文件的不同路径。

在集成期间，该实例在单元中被替换为新原型的实例，同时将的数据丢失降至最低。在原型发生更改的情况下，系统会在继续之前自动创建单元备份。

> **注意**：对于连接到 eMServer 的单元，Robcad 的「Replace connection（替换连接）」命令被阻止。请使用 Process Designer 为此类单元替换原型。

> **重要**：
>
> - 当用户替换原型的 3D 组件（.cojt）文件时，用户有责任再次同步原型（"Update Engineering Data"）。
> - 用户每次替换原型时，都有责任将旧的 .cojt 文件保留在库中，直到更新了包含旧原型实例的所有单元。提前移除它会导致集成中的初始 "load cell" 过程无法找到它，从而导致集成失败。
> - 即使用户为某个特定实例替换原型，它也会为属于该原型的所有其余实例替换。如果用户将一个实例的原型替换为特定原型，而将另一个实例的原型替换为不同的原型 — 两个实例都会与同一个原型关联，在用户指定的两个原型之间随机选择。

<a id="v5-s174"></a>

### 支持 Study 焊点工作流（Support for Study Weld Points Workflow）

用户可以在 Robcad 与 eMServer 之间传输未分配给任何焊接操作的焊点，而无需在 Robcad 单元中创建任何冗余的焊接位置。以下各节描述了针对未分配给任何焊接操作的焊点的新 Robcad-eMServer 集成行为。

现在可以使用未分配给任何焊接操作的焊点更新 Robcad 单元。用户在 RobcadStudy 节点下创建指向这些焊点的快捷方式，并且在使用 Robcad Studies 对话框中的 Update Cells 命令时，它们会被更新到 Robcad 单元。

> **重要**：焊点被更新到 Robcad 单元时，不会生成未投影（un-projected）的焊接位置。

<a id="v5-s175"></a>

#### 从 eMServer 更新到 Robcad（Updating from eMServer to Robcad）

从 eMServer 更新到 Robcad 时，单元中会更新未分配给任何焊接操作的焊点（在 RobcadStudy 下生成指向这些焊点的快捷方式）。有关不同场景的行为，请参阅下一节「从 Robcad 更新到 eMServer（Updating from Robcad to eMServer）」中的表格。

<a id="v5-s176"></a>

#### 从 Robcad 更新到 eMServer（Updating from Robcad to eMServer）

从 Robcad 更新到 eMServer 会更新以下 Operation 对象：

- 「Mapped（已映射）」路径（即在 Path Selection Table 中从 eMServer-Integ 菜单选择的路径）及其内容。
- 已连接的焊点，即使它们不属于「mapped」路径。

以下场景描述了可能的行为：

- 连接到 eMServer 但已从 Path Selection 对话框表中移除的焊接操作（路径）作为私有数据保留在 Robcad 单元中。从 Robcad 更新到 eMServer 会断开这些路径并将其从 eMServer 删除，但其相应的焊点保留在 Robcad 单元中并会更新到 eMServer。这些焊点现在不再分配给相应的焊接操作（如果之前已分配），并在 Robcad study 下直接创建指向它们的快捷方式。
- 不属于任何路径的未投影（un-projected）焊接位置被视为冗余。从 Robcad 更新到 eMServer 时，它们会从 Robcad 单元中删除，并且不会更新到 eMServer。但是，它们相应的焊点保留在 Robcad 单元中并会更新到 eMServer。在 Robcad study 下直接创建指向这些焊点的快捷方式。
- 不属于任何路径的已投影（projected）位置不会更新到 eMServer，并作为私有数据保留在 Robcad 单元中。当用户从 Robcad 更新到 eMServer 时，分配给这些焊接位置的焊点保留在单元中，并作为 Robcad study 下的快捷方式更新到 eMServer。
- 不属于任何路径的已连接焊点保留在单元中。当用户从 Robcad 更新到 eMServer 时，这些焊点在 eMServer 中作为 Robcad study 下的快捷方式更新。

下表描述了从 eMServer 更新到 Robcad 对不同场景的行为：

| 编号 | Study 项                | Robcad 单元中的结果                                                                                        |
| -- | ---------------------- | ---------------------------------------------------------------------------------------------------- |
| 1  | 指向焊点的快捷方式              | 焊点（Weld point）                                                                                       |
| 2  | 指向焊接位置操作的快捷方式          | 若为已投影焊接位置操作：焊接位置 + 焊点。若为伪焊接位置操作：焊点。若未分配焊点：无（在下一次从 Robcad 到 eMServer 的更新中，焊接位置操作及其快捷方式从 eMServer 删除）。 |
| 3  | 指向焊点及其相应已投影焊接位置操作的快捷方式 | 焊点 + 焊接位置（在下一次更新到 eMServer 时，焊接位置操作及其快捷方式从 eMServer 删除）                                              |
| 4  | 指向焊点及其相应伪焊接位置操作的快捷方式   | 焊点（在下一次更新到 eMServer 时，焊接位置操作及其快捷方式从 eMServer 删除）                                                     |
| 5  | 指向 via 位置的快捷方式         | 无（via 位置及其快捷方式在下一次从 Robcad 到 eMServer 的更新中从 eMServer 删除）                                             |
| 6  | 指向焊接操作的快捷方式            | 焊接路径 + 其焊接位置（若在 eMServer 中已投影则为已投影，否则为未投影；若无位置则为未投影）+ 其 Via 位置 + 其焊点                                 |

下表描述了从 Robcad 更新到 eMServer 对不同场景的行为：

| 编号 | Robcad 项                            | Robcad 中的结果             | eMServer 中的结果                |
| -- | ----------------------------------- | ----------------------- | ---------------------------- |
| 1  | 已连接或 legacy 的 mapped 路径             | 保留在单元中                  | 更新到 eMServer，study 中有快捷方式    |
| 2  | 属于 mapped 路径的已连接或 legacy 焊接与 via 位置 | 保留在单元中                  | 更新到 eMServer，study 中无快捷方式    |
| 3  | 属于 mapped 路径的已连接或 legacy 焊点         | 保留在单元中                  | 更新到 eMServer，study 中无快捷方式    |
| 4  | 已连接的非 mapped 路径                     | 作为私有数据保留（与 eMServer 断开） | 从 eMServer 删除，study 快捷方式删除   |
| 5  | legacy 非 mapped 路径                  | 作为私有数据保留（未连接到 eMServer） | 不传输到 eMServer，study 中无快捷方式   |
| 6  | 属于非 mapped 路径的已连接焊接或 via 位置         | 作为私有数据保留（与 eMServer 断开） | 从 eMServer 删除，study 中无快捷方式   |
| 7  | 属于非 mapped 路径的 legacy 焊接或 via 位置    | 作为私有数据保留（未连接到 eMServer） | 不传输到 eMServer，study 中无快捷方式   |
| 8  | 属于非 mapped 路径的已连接焊点                 | 保留在单元中（连接到 eMServer）    | 在 eMServer 中未分配，study 中有快捷方式 |
| 9  | 属于非 mapped 路径的 legacy 焊点            | 作为私有数据保留（未连接到 eMServer） | 不传输到 eMServer，study 中无快捷方式   |
| 10 | 不属于路径的已连接 via 位置                    | 作为私有数据保留（与 eMServer 断开） | 从 eMServer 删除，study 中无快捷方式   |
| 11 | 不属于路径的 legacy via 位置                | 作为私有数据保留（未连接到 eMServer） | 不传输到 eMServer，study 中无快捷方式   |
| 12 | 不属于路径的已连接未投影焊接位置                    | 从单元中删除                  | 从 eMServer 删除，study 中无快捷方式   |
| 13 | 不属于路径的已连接已投影焊接位置                    | 作为私有数据保留（与 eMServer 断开） | 从 eMServer 删除，study 快捷方式删除   |
| 14 | 不属于路径的 legacy 已投影焊接位置               | 作为私有数据保留（未连接到 eMServer） | 不传输到 eMServer，study 中无快捷方式   |
| 15 | 不属于路径的已连接焊点                         | 保留在单元中（连接到 eMServer）    | 更新到 eMServer，study 中有快捷方式    |
| 16 | 不属于路径的 legacy 焊点                    | 作为私有数据保留（未连接到 eMServer） | 不传输到 eMServer，study 中无快捷方式   |

<a id="v5-s177"></a>

### Robcad 命名规则（Robcad Naming Rules）

要转换为 Robcad 用法的 eMServer 对象名称（英文）只能包含下表中的字符：

| eMServer 名称字符 | 转换为 Robcad 单元后 |
| ------------- | -------------- |
| a-z           | 不变             |
| A-Z           | 转换为小写          |
| 0-9           | 不变             |
| 空格            | 转换为下划线（"\_"）   |
| \_            | 转换为下划线（"\_"）   |
| -             | 转换为下划线（"\_"）   |
| *             | 转换为下划线（"\_"）   |
| +             | 转换为下划线（"\_"）   |
| ?             | 转换为下划线（"\_"）   |
| <             | 转换为下划线（"\_"）   |
| >             | 转换为下划线（"\_"）   |
| $             | 转换为下划线（"\_"）   |
| &             | 转换为下划线（"\_"）   |
| @             | 转换为下划线（"\_"）   |
| #             | 转换为下划线（"\_"）   |
| %             | 转换为下划线（"\_"）   |
| ^             | 转换为下划线（"\_"）   |
| =             | 转换为下划线（"\_"）   |
| !             | 转换为下划线（"\_"）   |
| ~             | 转换为下划线（"\_"）   |
| ,             | 转换为下划线（"\_"）   |
| .             | 转换为下划线（"\_"）   |
| (             | 转换为下划线（"\_"）   |
| )             | 转换为下划线（"\_"）   |

> **注意**：不得使用变音符号（umlaut，「¨」）。

<a id="v5-s178"></a>

### 创建 Legacy 单元（Creating Legacy Cell）

为了将已连接的单元连接到 eMServer 中的不同项目，用户可以将单元（的副本）恢复为 legacy Robcad 单元。Create Legacy Cell 命令会移除存储在单元中的所有路径映射数据，但在新的 legacy 单元中保持层级结构完整。

要将已连接的单元恢复为 legacy 状态：

**步骤**

1. 选择 Applications 选项卡 → Robcad 组 → Create Legacy Cell。将显示 Create Legacy Cell 对话框。
   > **注意**：无论当前是否打开了项目，此命令都可用。
2. 选择 Source Cell（要恢复为 legacy 状态的已连接单元）。
3. 为 Target Cell 指定名称并指定位置。系统会复制原始（Source）单元并将其放置在您选择的文件夹中。
4. 选择 Library Root。
   > **重要**：对特定单元使用 Create Legacy Cell 命令，然后将其连接到同一项目中的不同研究，会导致对象在 eMServer 中重复。当将已连接的 Robcad 单元连接到同一项目中的不同研究时，请改用 "Force Reconnect" 命令以避免此问题。
   >   
   > **注意**：指定已存在的 Target Cell（无论是 Source Cell 还是任何现有单元）会导致命令失败。同样，选择 legacy 单元作为 Source Cell 也会导致命令失败。您指定的 Source Cell、Target Cell 和 Library Root 会写入集成报告（如果您具有写权限）。

<a id="v5-s179"></a>

## Process Designer 的 Teamcenter 命令（Teamcenter Commands for Process Designer）

有关以下命令的信息，请参阅 Collaboration Context 文档：

- Publish to Teamcenter（发布到 Teamcenter）
- Teamcenter Data Exchange（Teamcenter 数据交换）
<a id="v6-s1"></a>
<!-- p933 -->
# 6. Study（研究）

本卷介绍 Process Designer 中 **研究（Study）** 选项卡下的各项命令，涵盖范围（Scope）、选择（Selection）、坐标系（Frame）、标注（Annotation，含尺寸（Dimension）、注释（Note）、标签（Label））、布局（Layout，含附加/分离、对齐（Alignment）、镜像、复制、多零件外观等）以及成组（Group）相关的对象命令。文件顶部的页码标记 `<!-- pXXX -->` 对应源 PDF 页码。

<a id="v6-s2"></a>
<!-- p933 -->
## Scope Group（范围组）

Scope 组提供将对象加入或移出当前研究会话的命令。

<a id="v6-s3"></a>
<!-- p933 -->
### Add Root（添加根）

Add Root（添加根）命令可用于将 Navigation Tree（导航树）中的某项添加到当前 Process Designer 会话中，作为拖放操作的替代方式。只要所选项目类型相同（全部为资源、全部为零件等），即可通过一次操作追加多个项目。

**步骤：**

1. 在 Navigation Tree（导航树）中选择一个或多个相同类型的项目。
2. 选择 **Study tab → Scope group → Add Root**。所选项目被添加到当前会话，并显示在相应的树中。例如，如果追加的是资源，则显示在 Resource Tree（资源树）中。追加到当前会话的单个项目会立即显示在 Graphic Viewer（图形查看器）中；如果追加了多个项目，则这些项目不会显示在 Graphic Viewer 中，直到在相应树中专门选中它们的切换图标为止。详见 Load on Demand。

<a id="v6-s4"></a>
<!-- p933 -->
### Remove Root（移除根）

Remove Root（移除根）选项会将所选节点从当前会话中移除（在树和 Graphic Viewer 中均如此），但**不会**将其从数据库中删除。

> **注意：** 无法通过操作闭合（operation closure）加载的根，或被其他对象引用的根，不能被移除。如果尝试移除此类根，系统会显示相应消息。

**步骤：**

1. 选择该节点。
2. 选择 **Study tab → Scope group → Remove Root**。

<a id="v6-s5"></a>
<!-- p935 -->
### Collision Mode（碰撞模式）

Collision Mode（碰撞模式）选项用于启用/禁用碰撞模式。更多信息请参阅 Collision Viewer（碰撞查看器）。

<a id="v6-s6"></a>
<!-- p935 -->
### Fast Collision Set（快速碰撞集）

该选项使用户能够从所选对象快速创建碰撞集。此碰撞集显示在 Collision Viewer 左窗格中，名称为 `fast_collision_set`。通过该选项创建的碰撞集是一个自碰撞集（self set），即集合内的所有对象彼此之间都会进行碰撞检测。一个研究中只能存在一个快速碰撞集；如果创建另一个，则会替换之前的快速碰撞集。

<a id="v6-s7"></a>
<!-- p937 -->
## Selection Group（选择组）

Selection 组提供按对象类型过滤 Graphic Viewer 中显示内容的选项。

<a id="v6-s8"></a>
<!-- p935 -->
### Save Selection（保存选择）

Save Selection（保存选择）是一个编辑工具，用于创建对象列表并将其作为选择列表临时存储。您可以在当前工程数据（engineering data）的后续工作中随时调出该选择列表。可按需创建任意数量的选择列表，并可将同一对象包含在不同的列表中。退出应用程序会删除工程数据中创建的所有选择列表。

**步骤：**

1. 在 Graphic Viewer 或 Object Viewer 的 Object Tree 中选择要包含在选择列表中的对象，然后选择 **Save Selection**。将显示 Save Selection 对话框。
2. 在 **Name** 字段中输入选择的名称。默认所有选择均命名为 `selection`。
3. 在 **Description** 字段中输入选择的描述（如需要）。
4. 单击 **OK**。选择即被创建并临时存储在工程数据中，可随时按 Retrieve Selection 中所述进行检索。

<a id="v6-s9"></a>
<!-- p936 -->
### Retrieve Selection（检索选择）

Retrieve Selection（检索选择）选项用于检索之前创建并临时存储在工程数据中的选择列表。创建选择列表的方法见 Save Selection。

**步骤：**

1. 选择 **Retrieve Selection**。将显示带有所有选择列表的 Selection 对话框。
2. 选择一个选择列表。所选选择列表中包含的对象在 Object Tree 中以粗体显示，并在 Graphic Viewer 中被选中。
3. 要编辑之前保存的选择列表，先选中它，按住 `<Ctrl>` 键，然后在 Graphic Viewer 或 Object Viewer 中选择要添加（或删除）的对象，单击 以用更改更新选择。
4. 可在对话框中选择某选择后按 `F2` 重命名（名称必须唯一）。
5. 单击 **Close** 关闭 Selection 对话框。

> **注意：** 要删除选择列表，选中后单击 。

<a id="v6-s10"></a>
<!-- p937 -->
### Selection Group Options（选择组选项）

Study tab → Selection 组提供用于过滤 Graphic Viewer 中显示对象类型的选项。主要工具如下（均为根据对象类型进行显示过滤的开关/选择框）：

- **Select Filter**：过滤并查看零件（parts）和 Mfgs。
- **Select All**：启用所有过滤器（Select Part、Select Resource、Select Mfg）。
- **Select Type All**：选中所有过滤器，即选中 Graphic Viewer 中的所有实体。
- **Select Type None**：取消选中所有过滤器，即不选中任何实体。
- **Select Part / Select Resource / Select Mfg**：分别仅以绿色过滤并显示零件、资源、Mfgs。
- **Select Type Path**：仅选中路径（paths）。
- **Select Type Global Locations**：仅选中全局位置（global locations）。
- **Selection Type Frames**：仅显示坐标系（frames）。
- **Selection Type Dimensions**：仅显示尺寸（dimensions）。
- **Selection Type Labels**：仅显示标签（labels）。
- **Selection Type Notes**：仅显示注释（notes）。
- **Selection Type Lines/Curves**：仅选中 2D 实体。
- **Selection Type Solids/Surfaces**：仅选中 3D 实体。
- **Selection Type PMI**：仅选中 PMI 对象。
- **Save Selection / Retrieve Selection**：见前述对应小节。

<a id="v6-s11"></a>
<!-- p940 -->
## Frames Group（坐标系组）

Frames 组用于创建和管理坐标系（Frame）。

<a id="v6-s12"></a>
<!-- p940 -->
### Create Frame options（创建坐标系选项）

坐标系（Frame）用于标记工作单元（workcell）中组件与机器人之间未来交互的位置。创建坐标系有助于设计和规划工作空间布局。例如，如果您正在建模已知将用于未来行走操作（walk operation）的组件，并已知该操作计划的交互及其在工作单元中的位置，即可通过创建坐标系标记所需位置，并在适当时机插入交互。

Create Frame 选项包含一个坐标系选项的弹出工具栏，对应按钮如下：

- **Frame by 6 Values**：通过指定 X、Y、Z 轴及旋转 X、Y、Z 轴创建坐标系。
- **Frame by 3 Points**：通过指定任意三个点创建坐标系。
- **Frame by Circle Center**：通过指定圆周上任意三个点创建坐标系。
- **Frame between 2 Points**：通过指定两点之间的距离创建坐标系。

通过 **Study tab → Frames group → Create Frame** 创建坐标系。

<a id="v6-s13"></a>
<!-- p940 -->
#### Creating a frame by six values（通过六个值创建坐标系）

该方法通过同时指定 X、Y、Z 轴及旋转 X、Y、Z 轴，精确定位参考坐标系或目标坐标系。

**步骤：**

1. 选择 **Study tab → Frames group → Frame by 6 values**。显示 Frame by 6 values 对话框。
2. 若仅按位置或方向指定坐标系位置，单击 **Position** 或 **Orientation** 按钮。
3. 通过在 Graphic Viewer 中单击位置指定所需坐标系位置。X、Y、Z 坐标显示在 Relative Position 区域。
   > 也可通过在 Relative Position 区域输入坐标来指定位置。
4. 如需微调，使用 Relative Position 区域的上下箭头调整 X、Y、Z 坐标。
5. 使用 Relative Orientation 区域的上下箭头调整 Rx、Ry、Rz 坐标。
   > Frame by 6 values 对话框是动态的：所选坐标位置会即时反映在 Graphic Viewer 中。
6. 要相对于单元中另一坐标系创建坐标系，从 **Reference** 下拉列表中选择参考坐标系。
   > 可单击 Frame of Reference 按钮 旁的向下箭头，使用四种可用方法之一指定位置，创建临时替代参考坐标系。
7. 单击 **OK** 关闭对话框。新坐标系以默认名称 `fr#` 显示在 Graphic Viewer 和 Object Tree 中。

<a id="v6-s14"></a>
<!-- p942 -->
#### Creating a frame by three points（通过三个点创建坐标系）

该方法通过指定任意三个点精确定位参考坐标系或目标坐标系，适用于在平坦平面上创建坐标系。

**步骤：**

1. 选择 **Study tab → Frames group → Frame by 3 points**。显示 Frame by 3 Points 对话框。
2. 在 Graphic Viewer 中选择三个点，或在对话框中输入三个点的 X、Y、Z 坐标以定义平面。第一个点确定坐标系原点，第二个点确定 X 轴位置，第三个点确定 Z 轴位置。坐标系位置会动态反映在 Graphic Viewer 中。
   > 单击 可在 Z 轴上将坐标系翻转至相反方向（如需要）。
3. 单击 **OK**。新坐标系以默认名称 `fr#` 显示在 Graphic Viewer 和 Object Tree 中。

<a id="v6-s15"></a>
<!-- p942 -->
#### Creating a frame by circle center（通过圆心创建坐标系）

该方法通过指定圆周上任意三个点精确定位参考坐标系或目标坐标系，圆心会自动计算，适用于在圆柱形状顶部创建坐标系。

**步骤：**

1. 选择 **Study tab → Frames group → Frame by circle center**。显示 Frame by 3 Point Circle Center 对话框。
2. 在圆周上指定三个点（在 Graphic Viewer 中选择，或在对话框中输入各点的 X、Y、Z 坐标）。圆心自动定义，位置动态反映。坐标系方向为：Z 轴垂直于三点定义的平面，X 轴指向第一个点的方向。
   > 单击 可在 Z 轴上翻转（如需要）。
3. 单击 **OK**。新坐标系以默认名称 `fr#` 显示。

<a id="v6-s16"></a>
<!-- p943 -->
#### Creating a frame between two points（通过两点之间创建坐标系）

该方法通过指定两点之间的距离精确定位参考坐标系或目标坐标系，适用于在两点的中点创建坐标系。

**步骤：**

1. 选择 **Study tab → Frames group → Frame between 2 points**。显示 Frame Between Two Points 对话框。
2. 在 Graphic Viewer 中选择两点，或在对话框中输入两点坐标以定义线段。
3. 通过以下任一方式定义创建坐标系的两点间距离：
   - 拖动滑块；
   - 在文本框中手动输入值；
   - 使用上下箭头指定。
   > 默认距离为两点的中点。位置动态反映。
   > 单击 可在 Z 轴上翻转（如需要）。
4. 单击 **OK**。新坐标系以默认名称 `fr#` 显示。

<a id="v6-s17"></a>
<!-- p944 -->
### Set Working Frame（设置工作坐标系）

使用 Set Working Frame 可为研究自定义工作坐标系（working frame）作为参考。工作坐标系是 X、Y、Z 坐标被定义为 0 的位置，研究中的所有坐标值均相对于工作坐标系显示。默认情况下，每个研究的工作坐标系等同于全局坐标系（global frame）。

更改研究的工作坐标系会影响引用坐标、位置和旋转的命今与查看器。例如，为放置命令和操作器（manipulators）输入的坐标值即相对于工作坐标系。

配置工作坐标系可简化流程中坐标的显示。例如，对于车轮装配流程，可将车轮中心定义为工作坐标系，装配中所有零件和资源的坐标均相对于车轮中心显示。

自定义研究的工作坐标系**不会**改变数据库中对象的位置，它只是用于相对于自定义参考显示位置的工作工具。若工作坐标系不同于全局坐标系，沿工作坐标系 x 轴移动对象会：在研究中按期望方向（图形上）移动对象；按全局坐标系计算移动的投影；用该投影更新数据库。

使用 Merge Studies 命令合并多个研究时，会将新合并研究中的工作坐标系重置为全局坐标系位置；对单个研究执行 Merge Studies（创建研究副本）则会保留新合并研究中的工作坐标系位置。每个研究的工作坐标系定义随其他研究数据一同保存。

**配置工作坐标系步骤：**

1. 选择 **Study tab → Frames group → Set Working Frame**。显示 Set Working Frame 对话框。
2. 执行以下任一操作：
   - 选择 **Reset to Origin** 将工作坐标系重置为全局坐标系；或
   - 单击 Frame of Reference 按钮 旁的向下箭头，使用任一标准坐标系指定方法指定位置（参见 Create Frame options）；或
   - 在 Graphic Viewer 中单击工作坐标系的所需位置。
3. 单击 **OK**。工作坐标系按输入配置，对话框关闭。

<a id="v6-s18"></a>
<!-- p945 -->
## Annotation Group（标注组）

Annotation 组提供尺寸（Dimension）、注释（Note）和标签（Label）的创建与管理命令。

<a id="v6-s19"></a>
<!-- p945 -->
### Create Dimension（创建尺寸）

Create Dimension（创建尺寸）选项可在两个所选对象之间创建尺寸并将其作为工程数据的一部分存储。每个尺寸由所选对象之间连线或弧线以及测量值组成，可按需创建多个尺寸并添加到项目中。与 Graphic Viewer 上的 Measurement 工具不同，尺寸是动态的——若所测对象移至新位置，其值会自动更新。还可对 Graphic Viewer 或 Section Viewer（若打开）中的视图创建快照，并作为图形文件附件保存到研究节点。

通过 **Study tab → Annotation group → Create Dimension** 并选择以下任一工具：

- **Point to point Distance**：显示两对象之间的点对点距离。
- **Minimal Distance**：显示两对象之间的最小距离。
- **Angular Dimension**：显示两对象之间的角度。激活命令后，在 Graphic Viewer 中选择两个平面、两条线性边，或一个平面与一条线性边（位于相交平面/直线上的）。尺寸是动态的——移动参与对象时尺寸更新；可拖动尺寸调整其位置（在测量平面内移动）；隐藏/删除包含参与对象的组件会隐藏/删除该尺寸。
- **Linear Dimension**：显示两个平行对象之间的线性距离。激活后选择两个平行平面、两条平行线性边，或一个平面与一条平行于该面的线性边。
- **X-Axis / Y-Axis / Z-Axis Distance**：分别显示两对象沿 X、Y、Z 轴的距离。
- **Normal to Source Curve Dimension**：测量从曲线到曲线的距离。
- **Normal to Target Curve Dimension**：测量从点到曲线的距离。
- **Curve Length Dimension**：测量从点到点的距离。

**示例（创建点对点尺寸）：**

1. 选择 **Study tab → Annotation group → Create Dimension** 并选择 **Point to Point dimension**。
2. 单击要测量其间的两个对象。Graphic Viewer 中两对象之间显示一条带箭头和测量值的线。
   > 测量单位基于 Units 选项卡中的设置。若对象在 Graphic Viewer 中被移动，相关尺寸值会自动更新。尺寸对象会添加到 Object Viewer。
3. 如需，单击并拖动白线将测量移到更醒目的位置。
   > 若在 Graphic Viewer 中隐藏（blank）某对象，与其相关的尺寸也会隐藏；要重新显示，必须在 Object Viewer 中选中该尺寸旁的空方块。
   > 删除对象会删除与其相关的所有尺寸。

**配置尺寸颜色：**

1. 选择 **File tab → Options** 并选择 **Appearance** 选项卡。
2. 从列表中展开 New Objects Colors 下的选项，选择 Dimension、Dimension Text 或 Dimension Text Background。
3. 单击调色板按钮更改当前设置。

执行 eMServer Update 后再次打开研究时，系统会根据您最后一次的颜色选择为所有现有尺寸、尺寸文本及背景着色。

<a id="v6-s20"></a>
<!-- p952 -->
### Notes Tools（注释工具）

Notes（注释）选项提供子选项，用于创建并将说明性注释附加到 Graphic Viewer 中的对象上，有助于澄清复杂项目或向其他用户/部门传达对象信息。注释保存在工程数据（engineering data）中，与保存在 eMServer 上的标签（Labels）不同。

通过 **Study tab → Annotation group → Notes** 并选择以下子选项（摘要）：

- **Create Note**：创建新注释（见 Create Note）。
- **Objects Notes**：创建以所选对象名称为注释文本的自动注释（见 Objects Notes）。
- **Edit Note**：修改现有注释（见 Edit Note）。
  > 无法编辑嵌套在组件下的注释。
- **Toggle Note Visibility**：切换注释显示，显示完整文本或仅显示标记（flag）（见 Toggle Note Visibility）。
  > 无法切换嵌套在组件下的注释的可见性。
- **Automatic Note Flag placement**：运行算法重新排布注释，避免注释相互重叠（见 Automatic Note Flag Placement）。
- **Note Settings**：设置注释参数以启用成功的自动注释放置（见 Customizing notes）。

> 创建注释后，可将带注释的图像导出为图形文件，参见 Export Images。

<a id="v6-s21"></a>
<!-- p953 -->
#### Create Note（创建注释）

Create Note（创建注释）选项使用 Note Editor 向 Graphic Viewer 中所选对象添加电子作业指导书、Web 链接或说明性注释，也可在需要时在特定对象上标记问题。

创建注释后，可修改注释文本、位置（对象）或名称（见 Edit Note）。可选择在缩放时保持注释相对大小，并显示/隐藏其引线。还可将带注释的图像导出为图形文件（见 Export Images）。注释无法从 Graphic Viewer 中删除，但可切换其可见性（见 Toggle Note Visibility）或隐藏（blank）它们（见 Blank）。要查看无任何显示、切换或隐藏注释的图像，必须启动新会话并重新打开图像（见 Launching Process Designer）。

> 若在 Graphic Viewer 中隐藏某对象，其注释也会隐藏。

**创建注释步骤：**

1. 在 Graphic Viewer 或树中选择一个对象。
2. 选择 **Study tab → Annotation group → Notes** 并选择 **Create Note**。显示 Note Editor 窗口，Object 字段中显示所选对象名称。
   > 也可先选择 Create Note 显示 Note Editor 对话框，定义注释文本，单击 Object 字段，再在 Graphic Viewer 或 Object Tree 中选择所需对象。彩色字段表示当前活动字段（默认绿色，可在 Options 窗口的 Appearance 选项卡中自定义）。
3. 在 **Name** 字段中输入注释名称。默认命名为 `Note#`（# 为下一顺序号）。
4. 在 **Text** 区域输入要显示的文本。
5. （可选）在 **Appearance** 区域：
   - 单击 **Leader line**（默认选中）可取消选中以隐藏连接注释与其对象的引线；再次单击可显示。关闭对话框时该选项的选中/取消状态会被保留。
   - 单击 **Keep size while changing zoom**（默认选中）可取消选中以允许注释随视图缩放改变大小；再次单击可在缩放时保持大小。状态同样会被保留。
6. （可选）单击 **Preview** 在 Graphic Viewer 中预览注释文本。
7. （可选）要添加 Web 链接，在 **Link** 区域单击 **Browse** 并导航至所需 URL；若不希望包含链接，删除 Link 区域内容。
   > 可单击 **Open** 查看 Link 区域当前显示的 Web 链接页面。
8. 单击 **OK**。注释以所选对象上打开的标记（flag）形式显示在 Graphic Viewer 中。
   > 注释可通过单击并拖动在 Graphic Viewer 中移至新位置。

<a id="v6-s22"></a>
<!-- p955 -->
#### Objects Notes（对象注释）

Objects Notes（对象注释）选项用于为一个或多个对象创建自动标签，标签文本为所选对象的名称。为多个对象同时创建自动标签时，会为每个所选对象分别创建标签并置于其相应位置。

（对象注释同样支持：创建后修改文本/位置/名称见 Edit Note；导出带注释图像见 Export Images；无法从 Graphic Viewer 删除，但可切换可见性或隐藏；重新查看无注释图像需新会话；隐藏对象会隐藏其注释。）

> 可在 **File tab → Options group → Appearance Tab** 中调整 Objects Notes 的字体大小。

**创建自动注释步骤：**

1. 在 Graphic Viewer 或树中选择一个或多个对象。
2. 选择 **Study tab → Annotation group → Notes** 并选择 **Objects Notes**。在每个所选对象上您选取的位置显示自动注释（带文本的打开标记）。

或：不选中任何对象，激活 Objects Notes 命令，单击各所需对象以创建对象注释；按 `<Esc>` 或启动另一命令退出。

<a id="v6-s23"></a>
<!-- p956 -->
#### Edit Note（编辑注释）

注释创建后，Edit Note（编辑注释）选项可用于按需修改注释文本、位置（对象）或名称。注释无法从 Graphic Viewer 删除，但可切换可见性或隐藏（参见前述）。

> 若在 Graphic Viewer 中隐藏某对象，其注释也会隐藏。
> 无法编辑嵌套在组件下的注释。

**编辑注释步骤：**

1. 在 Graphic Viewer 中选择所需注释。
2. 选择 **Study tab → Annotation group → Notes** 并选择 **Edit Note**。显示 Note Editor 窗口。
3. 修改各注释元素：
   - 在 **Name** 字段中选中现有名称并输入新名称。
   - 在 **Text** 区域选中现有文本并输入新文本。
   - （可选）单击 **Preview** 预览。
   - （可选）在 **Link** 区域单击 **Browse** 添加 Web 链接（删除内容则不含链接）。
4. 单击 **OK** 保存更改。Graphic Viewer 中相应注释自动更新。

<a id="v6-s24"></a>
<!-- p958 -->
#### Toggle Note Visibility（切换注释可见性）

Toggle Note Visibility（切换注释可见性）选项用于将 Graphic Viewer 中的注释显示为：显示状态（带文本的打开标记）或切换状态（无文本的关闭标记）。每个显示或切换的注释都位于其相关对象上。

该选项仅对您所选的注释有效；选择 Toggle Note Visibility 或单击 后，所有所选注释根据其当前状态被切换或显示。

**切换注释可见性步骤：**

1. 在 Graphic Viewer 中选择要切换的一个或多个注释。
2. 选择 **Study tab → Annotation group → Notes** 并选择 **Toggle Note Visibility**。所有当前显示的注释被切换，所有当前切换的注释被显示。
3. 重复步骤 2 再次切换。
   > 也可按需选择或取消选择其他注释，然后切换其可见性。
   > 无法切换嵌套在组件下的注释的可见性。

<a id="v6-s25"></a>
<!-- p959 -->
#### Automatic Note Flag Placement（自动注释标记放置）

创建注释后会生成标记（flag）指示注释存在，但标记不表示注释内容，仅表示某对象或对象组存在注释。可使用 Hide note 选项隐藏注释，或使用 Automatic Note Flag Placement（自动注释标记放置）优化功能重新排布多个注释，使标记互不重叠且易于访问；该功能适用于所有类型对象的对象注释，对浮动注释（floating notes）同样有效。

**优化注释标记放置：**

激活命令前，必须在 Note Settings 对话框中设置控制标记替换结果的参数。定义参数后可将其应用于当前研究。研究处于所需视图（旋转和缩放后），选择要自动重排标记的对象，然后可更改注释内容、位置或创建附加注释。可从 Notes 工具栏或 Image Properties 对话框访问 Automatic Flag Placement 命令。

**设置参数步骤：**

1. 选择 **Study tab → Annotation group → Notes** 并选择 **Notes Settings**。显示 Note Settings 对话框。
2. 输入所需参数：
   - **A - Distance from Edges**：标记距边缘的距离（像素）。
   - **B - Minimal vertical distance**：标记间最小垂直距离（像素）。
   - **C - Minimal horizontal distance**：标记间最小水平距离（像素）。
   - **D - Minimal distance from origin**：标记原点到标记本身的最小距离（像素）。
   > 距边缘的最小距离是从 Graphic Viewer 边界边缘起算。
3. 单击 **Apply** 保存设置；单击 **Reset** 恢复 A、B、C、D 的先前值；单击 **Close** 保存并关闭对话框。

**自动重排注释标记步骤：**

1. 缩放至所需对象。
2. 选择要重排的注释标记。
3. 选择 **Automatic Flag placement**；所选标记被重排，互不重叠。若重排后仍有部分重叠，需手动微调。

> 命令对所有所选标记有效；隐藏的标记不受影响。若未选择任何标记，窗口中所有标记都会被重定位。自动注释标记放置不影响嵌套在组件下的注释——这些标记必须手动重定位；被嵌套注释阻挡或过于靠近的对象注释和浮动注释标记也须手动重定位。

<a id="v6-s26"></a>
<!-- p963 -->
#### Customizing notes（自定义注释）

Customizable Notes（自定义注释）功能允许向自动生成的对象注释添加额外的自由文本和属性。该附加信息显示在对象注释中。管理员和用户均可自定义注释，可应用于零件、资源、MFG 及其继承的所有对象。应用自定义前，应在 Note Settings 对话框中定义自定义规则。

> 若用户未定义自定义注释，则使用管理员的自定义；若未定义任何注释自定义，默认自定义为对象名称。管理员的自定义保存在管理员组，用户的更改保存在用户应用程序数据中。

**步骤：**

1. 选择 **Study tab → Annotation group → Notes** 并从下拉列表中选择 **Note Settings**。显示 Note Settings 对话框。
2. 在 Automatic Note Flag Placement Margins 区域配置最小距离设置。
3. 从 **Note Format** 组合框中选择要设置注释格式的对象（可用格式：Part、Resource、MFG 及其子类）。选择后，对象类型显示在对话框底部的 Object Type。
   > 若选择 PLP 对象，Object Type 中会出现 PLP 和 PLP Usage，可分别为这两类对象类型选择属性。Object Type 是拥有有效属性的基类元素列表，这些在 `TcPsMappingConfiguration.xml` 中定义（默认位于 `<installation_directory>\eMPower\TeamcenterIntegration\dat\`）。对于每个对象类型，Attribute 仅包含映射文件自身或其某一基类中所定义的属性。
   > 显示名称含 `[`、`]` 或 `-` 字符的对象类型或属性，会分别用 `{`、`}` 或 `_` 显示。
4. 在文本框中输入所需自由文本；除自由文本外，还可将所选对象的属性添加到注释。
5. 从 **Attribute** 列表框中选择所需属性并单击 **Add**。属性以带括号的属性名形式添加到文本框，且不可编辑。可按需添加任意数量属性。
   > 可根据需要启用从右到左（Right-to-left）文本方向。
6. 单击 **Apply** 保存所选对象及其所有继承对象的自定义。
7. 单击 **OK** 应用更改并关闭对话框。

**创建对象注释：** 定义自定义注释后，通过加载研究、选择对象、选择 **Modeling tab → Note group → Notes** 并从下拉列表中选择 **Object Notes** 来创建注释，注释按自定义生成。

**更改现有对象注释的自定义：** 若更改自定义注释的属性或文本，更新不会自动反映在 Graphic Viewer 中显示的注释上，必须单击更新注释按钮在屏幕上更新。更新时，应用程序检索所有所选注释并按 Note Setting 对话框中的自定义重新生成；可在 Object Tree 中选择所有单元格注释以全选。手动编辑的注释在 Object Notes 更新后丢失数据。

**更新现有注释步骤：**

1. 加载所选研究。
2. 选择要更新的所需注释，选择 **Study tab → Annotation group → Notes** 并选择 **Update Object Notes**。所选注释被更新。

<a id="v6-s27"></a>
<!-- p968 -->
### Labels Tools（标签工具）

Labels（标签）选项提供用于创建并将标签附加到 Graphic Viewer 中对象的子选项。对象可附加一个或多个标签，这些标签也作为对象出现在 Object Tree 中并保存到 eMServer（与保存在工程数据中的注释不同）。可自定义标签外观，包括文本大小和颜色、背景与边框颜色以及背景透明度。

通过 **Study tab → Annotation group → Labels** 并选择以下子选项（摘要）：

- **Objects Labels**：为所选对象创建以对象名称为文本的自动标签（见 Object Labels）。
- **Create Label**：创建新标签（见 Create Label）。
- **Modify Label**：修改现有标签（见 Modify Label）。

<a id="v6-s28"></a>
<!-- p968 -->
#### Create Label（创建标签）

Create Label（创建标签）选项用于为 Graphic Viewer 中所选对象创建文本标签。默认情况下，标签包含其所附对象的名称，可按需添加额外文本。与注释不同，可修改标签的显示属性，包括字体大小和颜色、文本属性（粗体、斜体等）、标签边框和背景颜色以及背景透明度。

创建标签时，所附对象无需签出（checked out）。但在执行 Save As 时，只有在执行签出操作后，才能将标签保存到 eMServer。

标签随图像在 Graphic Viewer 中旋转和缩放因子改变而保持相对于对象的相对大小和方向。无法从 Graphic Viewer 删除标签，但可隐藏它们（见 Blank）。隐藏或删除所附对象时，标签也会被隐藏或删除。

> 当标签因所附对象被隐藏而隐藏时，必须在 Object Viewer 中选择该标签的切换图标以重新显示。

**步骤：**

1. 在 Graphic Viewer 或树中选择一个对象。
2. 选择 **Study tab → Annotation group → Labels** 并选择 **Create Label**。显示 Create Label 对话框，Labeled Object 字段中显示所选对象名称。
3. 默认在 **Label Name** 字段中显示所有者对象名称；如需，输入新名称。
   > 更改所有者对象名称也会更改附加到它的任何标签的名称。
4. 在 **Label Text** 字段中输入标签文本。默认标签名称也会出现在文本字段中。
5. 在 **Appearance** 区域输入标签文本大小，并单击 **Bold**、**Italic** 或 **Underlined**。
6. 从 **Font Color**、**Border** 和 **Background Color** 下拉列表中选择颜色。
7. 在 **Transparency** 字段中输入代表标签背景相对透明度的值（0—100），100 表示背景完全透明。
8. （可选）单击 **Preview** 在保存前于 Graphic Viewer 中显示标签。
9. 单击 **OK**。新标签显示在 Graphic Viewer 和 Object Viewer 的 Labels 节点中。

默认情况下，Process Simulate 放置标签时使其左下角邻接所有者对象顶部中心（俯视时）。可通过单击并拖动将标签重定位到新位置。

<a id="v6-s29"></a>
<!-- p970 -->
#### Modify Label（修改标签）

Modify Label（修改标签）选项用于修改一个或多个现有标签的文本和颜色属性。修改单个标签时可修改其全部属性；同时修改多个标签时，只能更改字体大小。

**步骤：**

1. 在 Graphic Viewer 或 Object Tree 的 Labels 节点中选择一个或多个标签。
2. 选择 **Study tab → Annotation group → Labels** 并选择 **Modify Label**。显示 Modify Label 对话框。
3. 按需修改所选标签的属性（多标签时仅可修改字体大小，详见 Create Label）。
4. （可选）单击 **Preview** 在保存前显示标签。
5. 单击 **OK**。修改后的标签显示在 Graphic Viewer 中。

<a id="v6-s30"></a>
<!-- p971 -->
#### Object Labels（对象标签）

Objects Labels（对象标签）选项用于为一个或多个对象创建自动标签，标签文本为所选对象的名称。为多个对象同时创建时，会为每个对象分别创建标签并置于其相应位置，每个对象标签出现在 Object Tree 的 Labels 节点中。

创建对象标签时，所附对象无需先签出；但除非在执行 Save As 时执行签出，否则标签不会保存到 eMServer。

创建对象标签后可：修改文本、颜色及文本属性（见 Modify Label）；将带注释的图像导出为图形文件（见 Export Images）。对象标签随图像旋转和缩放保持相对大小与方向；无法从 Graphic Viewer 删除，但可隐藏（见 Blank）；所附对象被隐藏时标签隐藏，被删除时标签删除。

> 当标签因所附对象被隐藏而从显示中移除时，必须在 Object Tree 中选择该标签的切换图标以重新显示。

**步骤：**

1. 在 Graphic Viewer 或树中选择一个或多个对象。
2. 选择 **Study tab → Annotation group → Labels** 并选择 **Objects Labels**。每个所选对象旁显示自动标签，文本为对象名称；颜色属性由 Options 窗口 Appearance 选项卡中的设置决定。

<a id="v6-s31"></a>
<!-- p973 -->
## Layout Group（布局组）

Layout 组提供组件附加/分离、对齐、镜像、复制、活动资源及多零件外观等布局命令。

<a id="v6-s32"></a>
<!-- p973 -->
### Attach and detach components（附加与分离组件）

Attach（附加）命令用于将一个或多个组件附加到另一个组件。

> 可通过选择组件、打开 Object Tree 并显示 Attached To 列来检查组件是否附加到其他对象。

**步骤：**

1. 在 Graphic Viewer 或 Object Viewer 中选择一个或多个组件，选择 **Study tab → Layout group → Attachment** 并选择 **Attach**。显示 Attach 对话框，Attach Objects 字段中显示所选组件名称。
   > 也可先选择 Attach 显示对话框，再在 Graphic Viewer 或 Object Viewer 中选择要附加的组件（在 Graphic Viewer 中选择对象时光标变为 ）。
2. 指定附加类型：
   - **One Way（单向）**：附加的组件可独立于其所附组件移动；若移动所附组件，所有组件一起移动。
   - **Two Way（双向）**：无论移动附加组件还是所附组件，所有组件一起移动。
3. 在 **To Object** 字段中单击，并在 Graphic Viewer 或 Object Viewer 中选择要将所选组件附加到的组件。所选组件名称显示在 To Object 字段中。
   > 若选择实体（entity），其集合（collection）自动显示；若选择坐标系或链接（link），集合不自动显示。若实体集合为块（block），则显示最低祖先、链接或组件。
4. 默认在 Store attachment 区域选中 **Local（本地）**，即附件存储在研究的工程数据中，而非作为数据库中的关系保存。例如，将机器人本地附加到导轨后，在另一研究中使用相同机器人和导轨时，该附件对该研究无效。本地附件显示如下：
5. 若希望全局保存附件，选择 **Global（全局）**，即作为数据库中的关系保存，而非存储在研究工程数据中。例如，将机器人全局附加到导轨后，在另一研究中使用相同两者时该附件仍有效。 图标表示全局附件。
   > 全局附件只能在资源之间建立；若在选中 Global 时选择零件，系统返回错误。
6. 将资源（如 k160）全局附加到另一资源（如 lnk1）后，还可将其本地附加到另一资源（如 lnk2）；此时本地附件生效，便于在不破坏全局附件的情况下测试各种场景。
   > 因此移动 lnk2 也会使 k160 移动，而移动 lnk1 不会。若分离 k160，本地附件被移除（因其当前生效），全局附件生效；此时移动 lnk1 会使 k160 移动，而移动 lnk2 不会。
7. 单击 **OK**。所选组件被附加，并可根据指定类型在 Graphic Viewer 中移动。若 Object Viewer 中当前显示 Attachments 列，则附加组件名称显示在其所附加组件旁。
   > 若删除组件，附加到它的任何对象**不会被**删除，组件保持附加直至您分离它们。

<a id="v6-s33"></a>
<!-- p975 -->
#### Detach（分离）

Detach（分离）选项用于断开附加组件之间的连接。选择附加组件，选择 **Study tab → Layout group → Attachment** 并选择 **Detach**。该附加组件不再附加，可独立移动。

> 若 Detach 选项被禁用，说明该组件未附加到其他对象，可按需附加。
> 分离"本地覆盖全局"的附件会恢复全局附件。因此，要完全断开"本地覆盖全局"附件，需运行 Detach 命令两次。

<a id="v6-s34"></a>
<!-- p976 -->
### Alignment（对齐）

Alignment（对齐）提供沿水平或垂直方向对齐对象，或沿所选轴均匀分布对象的命令。

<a id="v6-s35"></a>
<!-- p977 -->
#### Alignment Tools（对齐工具）

可沿水平或垂直方向对齐对象，或沿所选轴均匀分布。通过 **Study tab → Layout group → Align** 并选择以下选项（摘要）：

- **AlignX / AlignY / AlignZ**：分别沿 X / Y / Z 轴的正方向对齐所选对象。
- **AlignNegX / AlignNegY / AlignNegZ**：分别沿 X / Y / Z 轴的负方向对齐所选对象。
- **Distribute X / Distribute Y / Distribute Z**：分别沿 X / Y / Z 轴等距分布所选对象。

> 选择 **File → Save Scenario** 以保存对象新位置的场景；新位置不会自动保存。

<a id="v6-s36"></a>
<!-- p978 -->
#### Aligning XYZ（沿 XYZ 对齐）

通过选择对象并单击所需的对齐选项，可沿轴对齐对象。对齐由最后所选对象的轴位置决定——最后所选对象的轴位置决定对象沿所选轴的摆放位置。

> 确保 Viewing 工具栏中 Pick Level 显示为 Component。

下表示例说明三个对象的原始线性位置与执行 AlignX 后的对齐位置：

| 对象名称 | 原始位置 | Align X 位置 |
|---------|---------|--------------|
| Box1 | XYZ=(2, 5, 10) | XYZ=(7, 5, 10) |
| Box2 | XYZ=(3, 2, 5) | XYZ=(7, 3, 25) |
| Box3 | XYZ=(7, 1, 2) | XYZ=(7, 1, 2) |

<a id="v6-s37"></a>
<!-- p979 -->
#### Distributing XYZ（沿 XYZ 分布）

通过选择对象并单击所需的分布选项，可沿轴分布对象。分布由各所选对象的位置值决定：所选轴的最高值和最低值相加后除以要分布的对象数，即决定对象沿轴的摆放位置。

> 确保 Viewing 工具栏中 Pick Level 显示为 Component。

下表示例说明三个对象的原始线性位置与执行 DistributeY 后的分布位置：

| 对象名称 | 原始位置 | Distribute Y 位置 |
|---------|---------|-------------------|
| Box1 | XYZ=(2, 5, 10) | XYZ=(2, 5, 10) |
| Box2 | XYZ=(3, 2, 5) | XYZ=(3, 3, 25) |
| Box3 | XYZ=(7, 1, 2) | XYZ=(7, 1, 2) |

<a id="v6-s38"></a>
<!-- p979 -->
### Mirror Layout（镜像布局）

Mirror Layout（镜像布局）命令可在一道工序中创建工作站布局的"镜像反射"副本。例如，通过对现有（右侧）部分制作镜像版本，为工作站创建第二个（左侧）部分。可定义平面并将镜像应用于源对象，包括流程资源、其孪生操作（twin operations）以及分配给孪生操作的所有资源，但零件和 Mfgs 除外——后两类对象不进行镜像，因为其几何表示可能被错误镜像（例如右车门在镜像左侧仍为一扇右车门）。

新的镜像对象通过其自身坐标系（self frame）在镜像平面中"反射"源对象而创建。

> 镜像自身坐标系不在几何中心的对象可能产生意外结果。对象只能被镜像一次，但镜像出的对象本身可被再次镜像。
> 必须签出工作文件夹才能执行 Mirror Layout 命令，否则显示错误消息。流程资源的孪生对象也应签出。

**镜像布局步骤：**

1. 在 Resource Tree 中选择一个或多个流程资源或由其派生的任何资源（例如要镜像的"流程工位"）。
   > 不能从同一层级选择流程资源；不能镜像包含具有镜像兄弟的原始流程资源的父级，也不能镜像其以上任意层级的父级。
2. 选择 **Study tab → Layout group → Mirror Layout**。Mirror Layout 主对话框打开，左列列出所选布局中所有对象的层级。
3. 单击 **Define Mirror Plane** 图标（位于 Mirror Layout 工具栏左侧）。镜像平面出现在 Graphic Viewer 中，并打开 Mirror Plane 对话框。
4. 如有必要，选择 **Plane Directions**（镜像平面的轴）：XY、YZ 或 XZ。
5. 如有必要，选择 **Flip Axis**（默认为 Y）。
6. 选择 **Plane Reference Frame**：可为 Displayed Objects 的中心（Center of Displayed Objects）或 Other。使用其他参考帧时，选择 Other，然后单击向下箭头选择选项（见下图），并可输入参考帧名称。
7. 使用 **Manipulate Plane** 控件在 Graphic Viewer 中微调镜像平面位置：可沿 Z 轴横向移动，或绕 X、Y 轴旋转。
   > 此时旋转显示以从上方查看对象（含镜像平面两侧）会很有帮助。
8. 在 Source List 中选择一个或多个根对象（您所选用于镜像的流程资源），然后单击 **Create New Mirrored Objects** 图标（Mirror Layout 工具栏左起第二个）。将创建的镜像对象名称添加到对话框右侧（此步骤中对象本身尚未创建）。默认镜像对象名称与源对象相同，并附加 `_mirror`，但可现在或稍后重命名。
   > 对象尚未创建，必要时可从列表中删除：在 Mirror 列表中选择项目，单击 Mirror Layout 工具栏上的 Clear。
9. 单击 **Apply** 创建新的镜像对象。镜像布局即被创建。
10. 单击 **Close**。Mirror Layout 对话框关闭，镜像平面从 Graphic Viewer 中移除。

<a id="v6-s39"></a>
<!-- p987 -->
#### Mirror Layout Toolbar（镜像布局工具栏）

Mirror Layout 对话框包含以下工具栏（选项摘要）：

- **Define Mirror Plane**：打开 Mirror Plane 对话框，查看并调整镜像布局时所用平面的位置。
- **Create New Mirrored Objects**：从所选对象创建将生成的镜像副本列表。
  > 单击该图标时列出的对象在实际单击 Apply 前并未真正创建。
- **Clear**：从待创建对象列表中移除所选根对象及其子对象（选择根对象后启用）。
- **Clear All**：从待创建镜像对象列表中移除所有对象。
- **Display**：使被隐藏的所选对象可见。
- **Emphasize**：在 Graphic Viewer 中以黄色高亮所选原始对象，以蓝色高亮已存在的所选镜像对象。

<a id="v6-s40"></a>
<!-- p988 -->
### Duplicate Objects（复制对象）

Duplicate Objects（复制对象）命令用于复制所选对象的实例。

**步骤：**

1. 在 Graphic Viewer 或 Object Viewer 的 Operation Tree 中选择一个对象。
2. 选择 **Study tab → Layout group → Duplicate**。显示 Duplicate 对话框。
3. 在 **Duplicate** 区域使用上下箭头指定实例数量及沿哪个轴复制：
   - 在 **Number along X/Y/Z** 字段中输入各轴所需的实例数。
   - 在 **X/Y/Z Spacing** 字段中输入各轴上复制实例间的距离。
   > 可通过将所选对象长度与所需间隔距离相加来计算 X/Y/Z Spacing 中的间距。
4. 设置 **Preview** 可在 Graphic Viewer 中将所选对象显示为透明边界框、将复制实例显示为实体。
5. 单击 **OK**。所选对象的复制实例显示在 Graphic Viewer 和 Object Viewer 中，每个复制实例显示为带锁的组件，名称在原选对象后加 `_#`。

<a id="v6-s41"></a>
<!-- p989 -->
### Set Active Resource（设置活动资源）

Set Active Resource（设置活动资源）选项用于选择 Resource Tree 中哪个资源作为当前活动资源。从 Navigation Tree 的 Resource Library 向 Graphic Viewer 拖放原型和复合资源时，这些资源的新实例会自动放置在活动资源下。

> 在树之间拖放不使用活动资源。

**步骤：**

1. 在 Resource Tree 中选择一个复合资源。
2. 选择 **Study tab → Layout group → Set Active Resource**。所选资源以浅蓝色方框标记，表示已设为当前活动资源。

> 若未定义活动资源，原型和复合资源的新实例会添加到 Resource Tree 中作为根列出的某个复合资源下。

<a id="v6-s42"></a>
<!-- p990 -->
### Deactivate Active Resource（停用活动资源）

Deactivate Active Resource（停用活动资源）选项用于停用当前活动资源。

**步骤：**

1. 在 Resource Tree 中选择当前活动资源（以浅蓝色方框标记）。
2. 选择 **Study tab → Layout group → Deactivate Active Resource**。资源周围的浅蓝色方框被移除，表示其不再是当前活动资源。

> 若未定义活动资源，原型和复合资源的新实例会添加到 Resource Tree 中作为根列出的某个复合资源下。

<a id="v6-s43"></a>
<!-- p990 -->
### Multiple Part Appearance（多零件外观）

为在早期规划阶段辅助静态可行性验证，可显示单个零件在不同位置的多个外观。例如：产品可同时显示在容器中及一个或多个后续位置，用于静态可行性验证或评估零件尺寸变化对各位置的影响；规划容器尺寸时可在容器中放置单个零件的多个副本。

可创建零件、复合零件、出现组（occurrence groups）或加工中装配体（in-process assemblies）的多个外观。

所有零件外观都链接到创建它们的零件实例（源零件）。原始零件列在 Object Viewer 的 Parts 下，外观列在 Appearances 下，并同时显示在 Graphic Viewer 和 Relations Viewer 中。在各查看器中，外观可位于与源零件相同或不同的工位，且每个外观可被赋予唯一名称。对主零件实例属性（名称除外）的更改会在下次加载/重新加载时影响该零件的所有外观。

每个外观由用户分配给一个作用域操作（Scope operation），并出现在该操作的 Relations Viewer 中；作用域操作不必是分配给源零件实例的操作之一。

多外观允许在产品保持原始位置时模拟不同工位的操作。在需要移动/重定位零件的复杂仿真中，该功能可省去将零件送达目标工位所需的操作。将某零件外观设为主外观（primary appearance）会使该外观在仿真、碰撞、间隙、可达性分析等用途上替代原始零件。

> 创建新事件（见 Adding Events）或 OLP 命令（见 To Define Robotic Location Attributes）并将带静态外观的零件分配给新零件时，务必选择主外观；若选择其他外观，Process Simulate 会自动将其替换为该主外观（或若未设主外观则为原始零件）。
> 默认情况下，焊点和 PLPs 出现在源零件上，而不在其外观上。但若某外观被设为主外观，则分配给其作用域操作的所有焊点、焊接位置和 PLPs 会转换到该外观。每个操作只能定义一个主外观，其余外观无焊点，也不支持焊点功能（如 Weld Distribution Center 或 Smart Place）。若主外观是复合零件实例的子项，适用于该外观作用域操作的焊点出现在主外观上，其余焊点出现在源零件上。
> 碰撞和尺寸等静态验证可像外观彼此独立一样应用于外观。
> 多外观在以下场景非常有用：将产品外观同时分发到多个工位夹具或其他战略位置；使用外观处理特定机器人操作而不影响产品原始放置；使用外观模拟特定机器人操作以避免模拟原始产品放置所需的复杂操作序列。

**配置 Multiple Part Appearances 步骤：**

1. 选择 **Multiple Part Appearance**。显示 Multiple Part Appearance 对话框，Labeled Object 字段中显示所选对象名称。
   - 若在 Graphic Viewer 或 Object Viewer 中选择了零件，对话框打开时这些零件作为新外观填入。
   - 若在 Object Viewer 中选择了操作，与所选操作相关的所有零件作为新外观填入。
   - 若在 Object Viewer 中选择了外观，这些外观在对话框打开时列出供编辑。
   > 默认新外观以源零件名加 `_ap_1` 后缀命名，可双击重命名。
2. 可在 Object Viewer 中选择更多零件，或在 Object Viewer / Multiple Part Appearance 对话框中选择外观，并单击 **Create Appearance From Selection** 将其外观添加到对话框。
3. 也可单击 Create Appearance From Selection 图标旁的箭头访问以下选项：从所选零件创建外观；从所选外观创建外观；从当前分配给所选操作的零件创建外观。若选择了许多对象但只想从零件/外观/操作添加外观，这些选项很有用。添加多项后，可单击列标题按字母排序显示。
4. 如有必要，单击 从 Multiple Part Appearance 对话框中移除所选外观。
5. 为外观选择 **Location**（如选择夹具承载该外观）。若未定义位置，新外观默认放置在零件位置。
6. 选择 **Relative To** 对象（资源或坐标系，外观相对于其移动）。
7. 为对话框中所选外观分配操作，方式之一：在对话框中选择外观并单击 **Assign Operation to Selected Appearances**，出现 Select Operation 对话框；在 Graphic Viewer 或 Object Viewer 中选择所需操作并单击 ，可重复以添加多个操作。也可单击所需外观的 Operation 单元格并输入操作名或在 Object Viewer 中选择；不满意时可双击 Operation 字段输入其他操作名或重新选择。
8. 设置外观的 **Primary** 参数。若设置，作用域操作使用该外观模拟，且该操作的所有焊点和焊接位置转移到该外观；若清除，则保留在源零件上。任意时刻只能有一个外观被设为主外观。
9. 可从 Location、Relative to、Operation 列复制单元格数据，选择目标外观并粘贴到目标外观，节省例行工作。
10. 如有必要，可选择任意单元格（Part 列除外）按 `<Delete>` 键删除所选数据。
11. 单击 **Apply**。外观被创建；为指示此，所分配操作的字体由粗斜体变为常规文本。
    > 新外观显示在 Object Viewer 的 Appearances 下及 Graphic Viewer 中。
    > 在 Graphic Viewer 中无法区分源与其外观；但外观可有不同颜色、渲染等（依用户偏好），且不影响源产品。
12. 单击 **Edit Appearance From Selection** 根据当前选择显示相关外观列表：若选择了外观则直接列出；若选择了零件则列出其所有现有外观；若选择了操作则列出其 scope 下嵌套的所有外观。
    > 当前选择包含 Create Multiple Appearance 对话框、Object Viewer 或 Graphic Viewer 中所选的所有项。
    > 也可单击 Edit Appearance From Selection 图标旁的箭头访问：编辑从所选零件创建的外观；编辑从所选外观创建的外观；编辑从当前分配给所选操作的零件创建的外观；编辑 中的所有外观。
13. 若对结果不满意，可单击 取消操作与所选外观的分配——若仅分配了一个操作，该操作被取消分配且外观被删除；也可在 Object Viewer 或 Graphic Viewer 中删除外观（关闭 Multiple Part Appearance 对话框后）。
    > 从外观移除操作或删除外观在确认后立即执行（无需在对话框中单击 Apply）。
    > 若使用新的 Multiple Part Appearance 命令创建数据，则无法用以下旧命令显示或操作这些数据：Create Appearances Under Scope、Create Appearance、Edit Appearance、Part Instance Appearances。
    > 要使用新的 Multiple Part Appearance 命令，应将 `eMPower\MPA` 文件夹内容复制到 `eMPower` 文件夹（移除旧命令并添加新命令）。

<a id="v6-s44"></a>
<!-- p998 -->
## Group Group（成组组）

Group 组提供将对象组合为单一实体的命令。

<a id="v6-s45"></a>
<!-- p999 -->
### Group Tools（成组工具）

Group（成组）命令用于将零件、资源、复合零件、复合资源、截面（sections）、设备（equipment）和组组合为单一实体。

例如，当您没有对象签出权限却又希望更改其层级并运行仿真或进行新配置的碰撞检测时，可使用组。组可包含任何受支持对象，可在 IPA Viewer 中访问，且对原始对象无影响——若在组中更改对象层级，其在 Object Tree 中的配置保持不变。对象可属于多个组。

有关 Export Groups to Excel 命令，请参阅 IPA Viewer。通过 **Study tab → Group group** 访问以下命令（摘要）：

- **Create Group**：创建新组（见 Create Group）。
- **Flatten Hierarchy**：将组中（或复合体中）嵌套复合体的所有子对象提升（见 Flatten Hierarchy）。
- **Replace Compounds with Groups**：将组中（或复合体中）所有嵌套复合体替换为新组（见 Replace Compounds with Groups）。
- **Export Groups to Excel**：将组导出并保存为 Excel 文件（见 IPA Viewer）。

<a id="v6-s46"></a>
<!-- p999 -->
#### Create Group（创建组）

**步骤：**

1. 选择 **Study tab → Group group → Create Group**。显示 Create Group 对话框。
2. 在 **Name** 字段中输入新组名称。
3. 从 **Scope** 下拉列表中选择新组的作用域。默认 Process Designer 使用 IPA Viewer 中的高层 Group 项。
   > 若在调用 Create Group 前在 Object Tree 中选择了单项，Create Group 对话框打开时 Scope 下拉列表中显示组根。
4. 单击 **Grouped objects** 列表并选择要添加到组的项目。
   > 若在调用 Group 命令前在 Object Tree 中选择了项目，Create Group 对话框打开时 Scope 下拉列表中显示 IPA Viewer 组根，所选项目列在 Grouped objects 列表中。Grouped objects 列表不显示重复项。复合零件、资源和设备在添加到组时保留其层级。
5. 单击 **OK**。系统创建新组并显示在 IPA Viewer 中。

<a id="v6-s47"></a>
<!-- p1001 -->
#### Group Limitations（组的限制）

- 在产线仿真研究（line simulation study，不支持零件）中，包含零件的组不会显示在 IPA Viewer 中。
- 组中嵌套的设备不能有子组件（subcomponent）。
- 若在流程操作（flow operation）中以组作为仿真对象创建 IPA（In-Process Assembly），组的组成零件不会添加到 IPA 中。

<a id="v6-s48"></a>
<!-- p1001 -->
#### Flatten Hierarchy（展平层级）

Flatten Hierarchy（展平层级）命令对组（或复合体）操作，将嵌套复合体的所有子对象提升，使子对象直接嵌套在组下，然后移除空的复合体。该命令也对所选组下嵌套的组操作。

要展平层级，选择组并单击 **Flatten Hierarchy**。

<a id="v6-s49"></a>
<!-- p1002 -->
#### Replace Compounds with Groups（用组替换复合体）

Replace Compounds with Groups（用组替换复合体）命令对组（或复合体）操作，将所有嵌套复合体替换为新组，新组以原始复合体命名。该命令也对所选组下嵌套的组操作。

要将复合体替换为组，选择组并选择 **Study tab → Group group → Replace Compounds with Groups**。

> 操作前，复合体显示并以复合体符号 标记；操作后复合体被组替换，并以组符号 标记。
<a id="v7-s1"></a>
<!-- p1005 -->
# 7. Kinematics（运动学）

本卷介绍 Process Designer 中 **运动学（Kinematics）** 选项卡下的概念与命令。运动学描述组件的运动方式，由关节（Joint）、连杆（Link）与自由度（Degree of Freedom，DoF）构成，组合后形成运动学组（Kinematics Group）。具备运动学的组件在最简单层面是一个设备（device），在更复杂层面是一台机器人（robot）。您可以对设备或机器人进行操作，以在工作环境中模拟任务、求解并动画化机构。

本卷涵盖关节点动、机器人点动、跳转到位置、初始位置、姿态编辑器，以及显示/限制关节工作限位、安装与卸载工具等命令。文件中的页码标记 `<!-- pXXX -->` 对应源 PDF 页码。

<a id="v7-s2"></a>
<!-- p1005 -->
## Defining Kinematics（定义运动学）

本节介绍 **运动学（Kinematics）** 选项卡，它使您能够通过定义组件的运动学来为其创建连杆（Link）与关节（Joint）。具备运动学的组件在最简单层面是一个设备（device），在更复杂层面是一台机器人（robot）。您可以操作设备或机器人，以在工作环境中模拟任务。

<a id="v7-s3"></a>
<!-- p1005 -->
## Joint Jog（关节点动）

**Joint Jog（关节点动）** 命令用于移动所选设备的关节。创建设备后，您可以通过在设备中测试所选关节的运动并视需要调整其限位，来研究该关节的运动。如果您希望将机器人的工具中心点坐标系（TCPF, Tool Center Point Frame）锁定在特定位置后再点动机器人，请使用 Robot Jog（机器人点动）。默认情况下，在选中设备之前，Joint Jog 命令处于禁用状态。

**步骤：**

1. 将 Pick Level（选择级别）设置为 **Component（组件）**。
2. 选择一个组件或设备，并选择 **Robot tab → Tool and Device group → Joint Jog**。将显示 Joint Jog 对话框，其中列出所有已定义的关节。

Joint Jog 对话框包含以下列：

| 列名称 | 说明 |
| --- | --- |
| Joints tree（关节树） | 以层级方式显示所选组件及其关节。如果所选组件是超级组件（super-component），关节树显示所有子组件及其关节。 |
| Steering/Poses（转向/姿态） | 该列的显示因对象是关节还是组件而异（详见下）。 |
| Value（值） | 显示关节的精确数值，可直接编辑。设置单位请在 Units Tab 中配置 Linear 或 Angular。 |
| Lower Limit / Upper Limit（下限/上限） | 显示关节的软限位（soft limit）。可直接编辑；输入有效值显示为绿色，按 Enter 接受；输入无效值（如大于关节硬限位）显示为红色。 |

**Steering/Poses 列的行为：**

- **关节（Joints）** — 拖动滑块以设定所需的关节值，精确值显示在 Value 列。当关节值超出关节工作限位（working limit）时，滑块与 Value 以粉色（默认）高亮；超出关节物理限位（physical limit）时以紫色（默认）高亮。配置关节限位请参阅 Motion Tab 中的 Kinematic Properties；配置颜色指示请参阅 Appearance tab 中的 Kinematic Colors。
- **组件（Components）** — 从下拉列表中选择所需姿态（pose）。

> **注意：**
> - 子组件若不含有关节将被省略。
> - 超级组件在 Object Tree（对象树）中以叠加标记显示。
> - 如果某组件的各个关节滑块值组合与下拉列表中的任一姿态都不匹配，则下拉列表不显示任何选项。
> - 如果在 Motion Tab 中定义了闭合回路（closed loop），仅显示父关节。
> - 必须在 Motion Tab 中勾选 **Limit check** 复选框，才能使用限位功能。

当您在 Steering/Poses 列中进行更改时，Value 列中的数值会相应更新，所选组件也会在 Graphic Viewer（图形查看器）中随之移动。

> **注意：** 若两个或多个关节共用同一运动空间，系统会自动设置软限位并停用相应的软限位字段，以防碰撞。

Joint Jog 对话框工具栏包含以下工具：

- **Options（选项）** — 提供列管理与关节选项。在 Column Management 区域勾选/取消要显示/隐藏的列（Joint Tree 列为必选项，始终位于最左侧，不列于 Options 对话框中）；选择某列后单击上移（▲）/下移（▼）箭头可调整其顺序；可配置 Prismatic joints 步长（伸缩关节）与 Revolute joints 步长（旋转关节），以及 Steering/Poses 列滑块的灵敏度（Slider sensitivity）。
- **Show Dependent Joints（显示从动关节）** — 默认不显示从动关节（dependent joint，即复制其他关节运动的关节）；单击可显示，其滑块被禁用，且不能重置 Value、Lower Limit、Upper Limit。
- **Reset All Soft Limits（重置所有软限位）** — 将您在 Joint Jog 对话框中配置的所有关节软限位重置为各相关关节的硬限位（hard limit）。
- **Reset to Hard Limits（重置为硬限位）** — 选中某关节的 Lower Limit 或 Upper Limit 值后单击，将该限位值重置为其硬限位。

3. 单击 **Reset** 可将所有限位重置为其默认值。
4. 单击 **Close** 退出 Joint Jog 对话框。

> **注意：**
> - 若在 Motion Tab 中设置了 Limit joint motion，关节运动将受关节物理限位约束；若清除该选项，则可移动关节至任意姿态。
> - 若在 Motion Tab 中清除了 Indicate joint working limits，Joint Jog 对话框将不提供工作限位的颜色指示。

<a id="v7-s4"></a>
<!-- p1009 -->
## Robot Jog（机器人点动）

**Robot Jog（机器人点动）** 对话框用于操作机器人及其位置。它包含若干可展开/折叠的区域（expander area），便于访问操作机器人所需的命令。

选中机器人、分配给机器人的位置，或包含一台或多台机器人的设备原型（Equipment prototype）时，Robot Jog 命令可用。Robot Jog 对话框支持以下操作：

- 通过将机器人锁定到所选配置（configuration）来限制其运动，以确保平滑的路径与焊缝（seam）。
- 将机器人的工具中心点坐标系（TCPF）锁定在特定位置；点动时其所有关节自动补偿以维持 TCPF 位置（机器人基座锁定在沿导轨移动的滑橇（skid）上时亦如此）。还可选择性地将机器人基座从滑橇释放。
- 显示并移动机器人的所有关节（既包含使用 Joint Jog 命令操作的内部关节，也包含外部关节）。
- 操作嵌套在设备下的机器人：若机器人 TCPF 已锁定，则移动机器人或其关节（含外部关节）会使设备的所有嵌套组件随机器人一起移动；未嵌套在机器人父设备下、但附加到机器人或其连杆（Link）上的组件也会随之移动。这有助于使用包含机器人本身及其所有随附部件的设备。当机器人移动或旋转进入碰撞时，整个设备以及附加在机器人或其连杆上的所有组件均被纳入碰撞考虑。
- **双臂（Dual Arm）** — 若选择包含两台或多台机器人的设备原型，运行 Robot Jog 将打开 **Robot Jog: Dual Arm Robot** 对话框，可将一台机器人定义为主控（Master）、其余一台或多台定义为从控（Slave）。启用 Master/Slave 模式后，从控机器人的运动跟踪主控机器人，且无法点动从控机器人。

**步骤：**

1. 将 Pick Level 设置为 **Component**。
2. 选择机器人或分配给机器人的位置，并选择 **Robot tab → Reach group → Robot Jog**。将显示 Robot Jog 对话框。

默认情况下，对话框以展开 Manipulations 区域的方式打开，且系统在机器人工具坐标系（toolframe）上放置一个操纵器坐标系（manipulator frame）。若在启动 Robot Jog 前已选中某位置，则 Location 区域也会展开并填入所选位置。当选择的设备原型包含两台或多台机器人时，启动 Robot Jog 将打开 Robot Jog: Dual Arm Robot 对话框（此时无 Location 区域）。

<a id="v7-s5"></a>
<!-- p1009 -->
### Robot Jog 对话框与双臂机器人（Robot Jog Dialog Box & Dual Arm Robot）

除下述通用 Robot Jog 功能外，**Robot Jog: Dual Arm Robot** 对话框在顶部包含 **Robot** 与 **Robot's Role** 字段，以及额外的 **Enable Master/Slave Mode** 按钮：

- **Robot 下拉选择器** — 仅列出嵌套在该设备原型下的机器人，用于选择要点动的机器人；也可在该字段激活时，从任意查看器或树中选择嵌套在设备原型下的机器人。
- **Robot's Role 下拉选择器** — 用于将一台机器人定义为主控（Master）、一台或多台其他机器人定义为从控（Slave）。先在下拉字段中选择机器人，再分别定义每台机器人的角色。

<a id="v7-s6"></a>
<!-- p1016 -->
### Robot Jog 工具栏（Robot Jog Toolbar）

Robot Jog 工具栏提供以下图标命令：

| 图标命令 | 说明 |
| --- | --- |
| Lock TCPF（锁定 TCPF） | 将机器人的 TCPF 锁定在当前位置。设置后，TCPF 在所有 Robot Jog 命令及任何其他影响机器人运动的命令中始终保持当前位置；机器人移动时通过调整关节来补偿，以维持 TCPF 不变。 |
| Enable Robot Placement / Enable Robot and Attachment Chain Placement（启用机器人放置 / 启用机器人与附件链放置） | 默认机器人基坐标系（baseframe）锁定在当前位置。激活后，若机器人安装在滑橇上并沿导轨移动，其关节会自动补偿以维持 TCPF。可释放基座以改变机器人位置，或连同所有附加对象（如导轨）一起移动。 |
| Set External Values of Location（设置位置的外部轴值） | 配置并存储当前位置上机器人关节外部轴（external axis）的逼近值；双击该图标可自动设置所选位置的外部轴值。仅在 Follow Mode 开启时可用。 |
| Clear External Values from Location（清除位置的外部轴值） | 清除当前位置上的外部轴值。 |
| Show Dependent Joints（显示从动关节） | 默认不显示从动关节；单击可显示，其滑块被禁用且不能重置 Value、Lower Limit、Upper Limit。 |
| Reset Selected Limit to Hard Limits（将所选限位重置为硬限位） | 将所选软限位重置为关节的硬限位。 |
| Reset All Limits to Hard Limits（将所有限位重置为硬限位） | 将所有软限位重置为其关节的硬限位。 |
| Teach Location（示教位置） | 将当前机器人配置与当前位置存储到所选位置（作为示教位置 taught location 的参数，供仿真使用）。仅在 Follow Mode 开启时可用。 |
| Clear Taught Location（清除示教位置） | 从所选位置移除配置与示教位置。 |
| Robot Jog Settings（Robot Jog 设置） | 提供列管理与关节选项（针对 External Joints 与 All Joints 区域）。 |
| Enable Master Slave Mode（启用主从模式） | 仅见于 Robot Jog: Dual Arm Robot 对话框；切换 Master/Slave 模式开/关，开启时所有从控机器人的操纵器被禁用。 |

> **注意：** 锁定机器人 TCPF 会移除放置操纵器（placement manipulator）并折叠 Robot Jog 对话框中的 Manipulations 区域；若机器人具有外部轴，则 External Joints 区域会展开。

**Robot Jog Settings（设置对话框）要点：** 在 Joint Columns Management 区域勾选/取消要显示/隐藏的列（Joint 列为必选项，始终位于最左侧，不列于 Options 对话框）；可选择列并单击上移（▲）/下移（▼）调整顺序；可配置 Prismatic/Revolute joints 步长与滑块灵敏度；可设置 Copy attachment（复制参考位置的附件到新位置）；非 Follow Mode 下可设置 Display ghost gun（幽灵焊枪，ghost gun）显示焊枪跟踪行为的占位；可勾选 Manipulate continuous locations / Manipulate weld locations 以在 Follow Mode 下移动工艺操作位置。

**Master/Slave 模式：** 切换 Master/Slave Mode 按钮可开/关该模式。开启时所有从控机器人操纵器被禁用；当您点动主控机器人（拖动、使用 Manipulations 区域的按钮或 All Joints 滑块）时，每台从控机器人尝试跟随主控机器人的 TCPF。跟踪过程中也可锁定任一机器人的配置或 TCPF。

<a id="v7-s7"></a>
<!-- p1022 -->
### Location 区域（Location Area）

Location 区域提供以下控件：

| 控件 | 说明 |
| --- | --- |
| Current Location（当前位置） | 显示所选位置；单击激活后，可从 Object Viewer/Operation Tree 选择其他位置。 |
| Jump to First/Previous/Next/Last Location（跳到首/前/后/末个位置） | 将当前位置切换为操作（operation）中的第一个、上一个、下一个或最后一个位置。 |
| Move Location to TCPF（将位置移动到 TCPF） | 若 Follow Mode 关闭，以机器人当前位置更新该位置。 |
| Follow Mode（跟随模式） | 在此模式下，位置随机器人点动而跟随；若为经由位置（via location）可自由移动，若为焊接（Weld）或焊缝（Seam）则受主 Options 对话框中 Robot Jog Options 与 Weld and Seam 选项卡选项的限制。 |
| Add Location Before / After（在之前/之后添加位置） | 在所选位置前/后添加新的经由位置并点动机器人至该位置；坐标在 Follow Mode 开启时取自当前位置，否则取自机器人 TCPF 位置。 |
| Copy Parameters（复制参数） | 添加新位置时，选择从当前位置复制到新位置的参数：None、Robotic、Robotic + OLP Commands。 |

> **注意（Current Location）：** 若 Follow Mode 开启，机器人跟踪当前位置；但若当前位置是示教位置，跟踪时会忽略示教值，且 Lock TCPF 图标被禁用。
> **注意（Follow Mode）：** 若当前位置是示教位置，跟踪/操作该位置时不会使用示教值。
> **注意（Copy Parameters）：** 当前设置会保留到后续会话，并被 Robotic 路径的 Add Location Before / Add Location After 外部命令使用。若当前位置是经由位置，参数从其复制；否则从上一经由位置（或操作中第一个经由位置）复制；操作中无经由位置时则不复制参数。

<a id="v7-s8"></a>
<!-- p1024 -->
### 操作与坐标系（Manipulations and Frames）

展开 **Manipulations** 区域后可执行以下操作：

a. 使用操纵器或 Manipulations 区域中的控件移动并操作机器人（详见 Placement Manipulator）。
b. 默认参考坐标系（Frame of Reference）为机器人 TCPF，可改为相对于其他坐标系：
   - 从 **Frame of Reference** 下拉列表中选择一个坐标系；或
   - 单击 ＋ 并 **Create a new frame（创建新坐标系）**，可选类型包括：TCPF、Working Frame（工作坐标系）、BASEFRAME（基坐标系）、Robot System Frames、Robot Tool Frames（工具坐标系）、TCPF with BASE orientation。

您可单击相应按钮并选择配置（configuration），将机器人锁定在单一配置中。

> **注意：**
> - 机器人的当前位置决定 Configuration 下拉列表中显示哪些配置。
> - 机器人未被锁定在单一配置时，当前机器人配置会持续显示并更新。

<a id="v7-s9"></a>
<!-- p1025 -->
### All Joints 区域（All Joints Area）

**All Joints** 区域使您无需访问 Joint Jog 即可调整机器人关节的值。您可以在 Robot Jog 对话框中将关节软限位设为高于其硬限位的值；此时对应单元格添加黄色背景，悬停该值会显示提示信息。

> **注意：**
> - 您也可以调整机器人外部轴的值。
> - 使用 Robot Jog 调整关节值可应用配置锁定约束，而使用 Joint Jog 时无法应用此类约束。
> - 默认隐藏 **Value Percentage（值百分比）** 列；若在 Robot Jog Settings 对话框中将其设为显示，则该列以关节运动范围的百分比指示各关节当前位置：0% 表示关节处于运动范围中点，正百分比表示更接近上限，负百分比表示更接近下限。

<a id="v7-s10"></a>
<!-- p1027 -->
### External Joints 区域（External Joints Area）

**External Joints** 区域使您无需访问 Joint Jog 即可调整机器人外部关节（external joint）的值。

<a id="v7-s11"></a>
<!-- p1028 -->
### Coordinate Reference 区域（Coordinate Reference Area）

**Coordinate Reference** 区域用于测量所选位置相对于其他坐标系的位置：

a. 从 **Location relative** 列表中选择一个坐标系（默认为 Working frame）。该区域会更新 Original Frame（原始坐标系，上行）与 Reference Frame（参考坐标系，下行）的值。
b. 单击 **Snap by step size（按步长对齐）**，使数值（线性与角度）按 Manipulations 区域中 Step size 设定的增量递增或递减。

**重置与退出：**

- 单击 **Reset** 可撤销使用 Robot Jog 所做的更改；单击 Reset 旁边的箭头可选择：
  - **Reset Current Location** — 撤销自启动 Robot Jog 以来对当前位置所做的更改。
  - **Reset All Edited Locations** — 撤销自启动 Robot Jog 以来对所有位置所做的更改（撤销前系统会提示确认）。
- 单击 **Close** 关闭对话框并结束 Robot Jog 会话。

<a id="v7-s12"></a>
<!-- p1028 -->
## Jump to Location（跳转到位置）

**Jump to Location（跳转到位置）** 命令用于使机器人跳转到某一位置，以查看机器人是否能够到达所选位置。

**步骤：**

1. 选择机器人并选择 **Kinematics tab → Jog group → Jump to Location**。Jump to Location 模式被激活，光标变为相应形状。
2. 在 Graphic Viewer 中单击一个全局位置（global location）。机器人跳转到所选位置，机器人 TCPF 的 Z 轴与所选位置的 Z 轴对齐。若所选位置无法到达，状态栏显示消息 `Robot cannot reach location`。
3. 按需使机器人跳转到更多位置。
4. 单击相应按钮离开 Jump to Location 模式并返回 Select 模式。

> **注意：** 机器人依据已安装机器人控制器的数据进行跳转；若未安装此类控制器，则使用默认机器人控制器。

<a id="v7-s13"></a>
<!-- p1029 -->
## Home（初始位置）

**Home（初始位置）** 选项使设备或机器人返回到初次定义其运动学时所处的原始位置。

<a id="v7-s14"></a>
<!-- p1029 -->
## Pose Editor（姿态编辑器）

> **注意：** 视频未包含在 PDF 中；要访问视频，请使用 HTML 版本。

**Pose Editor（姿态编辑器）** 命令用于为设备和机器人创建并保存新姿态（pose），以及编辑和删除现有姿态。您可以在 Pose Editor 中保存一个姿态，并随时使设备或机器人返回到该姿态。

姿态根据关节值定义，这些关节值可在 Joint Jog 对话框中显示。您可以使用 Pose Editor 创建新姿态、编辑和删除现有姿态，以及将设备或机器人移动到所选姿态。

选择设备或机器人，并选择 **Kinematics tab → Pose group → Pose Editor** 以打开 Pose Editor。

> **注意：**
> - **HOME 姿态** 是设备或机器人初次定义运动学时所处的原始位置。默认情况下 HOME 姿态始终显示于 Pose Editor 中。您可以更新 HOME 姿态，但无法删除它。
> - 原型姿态（prototype pose）以粗体显示，以区别于实例姿态（instance pose）。

在 Pose Editor 中可执行以下操作：

- **Add（添加）** — 添加一个新姿态。
- **Edit（编辑）** — 选中某姿态后单击 Edit 以修改所选姿态的参数。将打开 **Edit Pose \<robot_name\>** 对话框，Pose name 预填为您所选姿态的名称（可更改），修改后单击 **OK** 保存。
- **Update（更新）** — 若您已用 Joint Jog 或 Robot Jog 移动所选设备，可从 Poses 列表选择任一姿态并单击 Update，将所选姿态设为设备的当前姿态。
- **Delete（删除）** — 选择一个或多个姿态并单击 Delete 将其删除。
- **Jump（跳转）** — 选中姿态并单击 Jump，使所选设备或机器人跳转到所选姿态。
- **Move（移动）** — 选中姿态并单击 Move，使所选设备或机器人移动到所选姿态；该移动在仿真中进行，便于检测到达该姿态的路径上是否发生碰撞。
- **Reset（重置）** — 单击 Reset 使所选设备或机器人返回到打开 Edit Pose 对话框时的姿态。
- **重命名** — 在 Poses 列表中双击某名称，或先选中再按 `F2` 进行编辑。

> **注意：** 以下项目受焊枪姿态（gun pose）影响：OLP Controllers（OLP 控制器）、Weld Simulation Engine（焊接模拟引擎）、Upload Robcad Program（上传 Robcad 程序）。

这些命令要求焊枪姿态使用以下系统保留名称：**OPEN、SEMIOPEN、CLOSE**（必须大写且拼写完全一致）。

**创建新姿态步骤：**

1. 在 Pose Editor 中单击 **New**。将显示 **New Pose \<robot_name\>** 对话框，列出所选设备或机器人的关节列表；Pose name 预填一个唯一的默认名称。
2. 在每个关节字段中，通过直接输入或使用上下箭头指定关节位置的值。
3. 在 Pose name 中编辑默认名称。
4. 单击 **OK**。所选设备或机器人移动到新姿态，且该新姿态被保存并显示在 Pose Editor 对话框中。

<a id="v7-s15"></a>
<!-- p1032 -->
## Indicate Joint Working Limits（显示关节工作限位）

**Indicate Joint Working Limits（显示关节工作限位）** 命令切换限位计算的全局状态。此外，它还会自动切换 Options 对话框 Motion Tab 中的 Indicate Joint Working Limits 参数，无需打开 Options 对话框。

运行 Indicate Joint Working Limits 后，Process Designer 会在 Graphic Viewer 与 Joint Jog 对话框中计算并显示关节限位的颜色指示。

> **注意：** 设置 Indicate Joint Working Limits 后，Process Designer 会消耗大量系统资源。

<a id="v7-s16"></a>
<!-- p1033 -->
## Limit Joint Motion（限制关节运动）

**Limit joint motion（限制关节运动）** 命令切换关节运动限制的全局状态。下图描述物理限位与工作限位：

- **红色（Red）— 物理关节限位（Physical joint limit）** — 实际设备关节不能越过此限位。物理限位由机器人制造商定义。若 Limit joint motion 关闭，在 Process Simulate 中关节可越过此限位。
- **黄色（Yellow）— 工作关节限位（Working joint limit）** — 您可扩展物理限位，以确保关节不接近真实物理限位。这一扩展称为工作限位，有助于延长机器人寿命。您可随时调整工作限位以适应当前约束，详见 Motion Tab 中的 Joints working limits。
- **绿色（Green）— 工作区域（Working area）**。

切换相应按钮可将关节运动限制在其物理限位（绿色与黄色）之内；同时 Motion Tab 中的 Limit joint motion 参数被自动设置，无需打开 Options 对话框。再次切换该按钮可取消限制关节运动；同时 Motion Tab 中的 Limit joint motion 参数被自动清除。

<a id="v7-s17"></a>
<!-- p1034 -->
## Mount Tool（安装工具）

**Mount Tool（安装工具）** 命令使您能够：

- 在机器人上安装工具与组件。
- 在附加到机器人的资源上安装工具与组件。

当您安装一个自身带有已定义工具坐标系（tool frame）的工具时，机器人的 TCPF 会移动到该坐标系。机器人移动时，已安装对象随机器人的工具坐标系一起移动。

通常，您安装工具以执行特定任务。例如，可在机器人上安装焊枪（weld gun），使机器人能在工作站的不同位置执行多项焊接任务。若执行任务所需的工具过大而无法安装在机器人上，则可将该待焊接对象安装在机器人上，再由机器人将其送至工具位置以完成所需任务。

默认情况下，在选中机器人之前，Mount Tool 命令处于禁用状态。

**步骤：**

1. 将 Pick Level 设置为 **Component**。
2. 选择机器人并选择 **Kinematics tab → Tools group → Mount Tool**。系统显示 Mount Tool 对话框。
3. 在 Graphic Viewer 或 Object Tree 中选择一个工具。在 Graphic Viewer 中选择对象时光标变为相应形状；系统在 Tool 字段显示工具名称。
4. 在 Mounted Tool（已安装工具）区域的 **Frame** 下拉列表中选择工具的参考坐标系（reference frame）。该参考坐标系决定如何将工具安装到目标机器人（或已安装资源）上。

> **注意：** 可单击 Frame of Reference 按钮旁的下拉箭头，使用四种可用方法之一临时修改所选坐标系的位置，详见 Create Frame。

5. 在目标机器人或资源上选择安装坐标系（mounting frame）：
   a. 在 Mounting Frame（安装坐标系）区域的 **Mount On** 下拉列表中选择包含安装坐标系的机器人或资源。
   > **注意：** 系统仅显示至少具有一个可用坐标系的资源。
   b. 在 Mounting Frame 区域的 **Frame** 下拉列表中选择该坐标系。
6. 单击 **Apply**。工具移动以使 Mounted Tool 区域中所选 Frame 与机器人或附加资源上的所选 Frame 对齐；若有，机器人的 TCPF 会移动到工具的 toolframe。

> **注意：** 若工具安装不正确，选择 **Reset** 将工具恢复到先前位置，并更改工具的参考坐标系位置。若工具位置正确但方向错误，单击 **Flip Tool（翻转工具）** 翻转工具；通过从下拉列表选择轴，工具可按 90 度增量向各个方向（X、Y、Z）翻转。已安装工具在 Object Tree 中以相应标记标识。

7. 对安装工具的位置与方向满意后，单击 **Close**。

> **注意：** 在机器人上安装伺服焊枪（Servo Gun）会自动将 Servo Gun 关节添加到机器人的外部轴列表中。

<a id="v7-s18"></a>
<!-- p1036 -->
## UnMount Tool（卸载工具）

**UnMount Tool（卸载工具）** 命令使您能够从机器人上拆卸已安装的工具或对象。拆卸后，TCPF 返回到机器人的工具坐标系（tool frame）。

> **注意：** 默认情况下，在选中安装在机器人上的工具或对象之前，UnMount Tool 命令处于禁用状态。

**步骤：**

1. 将 Pick Level 设置为 **Component**。
2. 选择安装在机器人上的工具，并选择 **Robot tab → Tool and Device group → UnMount Tool**。所选工具与机器人断开连接，TCPF 从工具移回机器人的工具坐标系。

> **注意：** 尽管工具在 Graphic Viewer 中不发生物理移动，但它已被断开，机器人与工具现在均可独立操作。从机器人上卸载伺服焊枪（Servo Gun）会自动将 Servo Gun 关节从机器人的外部轴列表中移除。
<a id="v8-s1"></a>
<!-- p1037 -->
# 8. 物流（Logistics）

<a id="v8-s2"></a>
<!-- p1037 -->
## 装箱容器（Packing Containers）

<a id="v8-s3"></a>
<!-- p1037 -->
### 创建装箱研究（Create Packing Studies）

Process Designer 支持创建装箱研究（packing study），以确定零件在容器中的最优装箱布局，以及针对零件的最优装箱容器。您可以综合考虑容器尺寸、容器与零件的总重量，并为包装材料预留余量。每项装箱研究对应一个特定零件，您可以比较多种容器，从而确定最优装箱方案。

该命令的输出为装箱布局（packing pattern）和装箱报告（packing report）。

运行装箱研究的阶段如下：

- 前提条件（Prerequisites）
- 配置装箱研究（Configuring the Packing Study）
- 计算装箱方案（Calculating the Packing Solution）

您可以使用以下任一工作流：

- 在单个装箱研究上完成所有阶段，如有必要，再对另一项装箱研究执行相同操作。到达“容器装箱方案（Container Packing Solutions）”阶段后，您可以：
  - 选择要生成的方案。
  - 创建预览。系统将预览存储在装箱研究的“方案（Solution）”文件夹下嵌套的 PatternPreview 节点中，但不会创建方案。您可以关闭对话框，并在方便时继续工作。
  - 为所有方案创建预览，并选择要生成的方案。
- 创建多项装箱研究并运行 Container Packing Batch（容器装箱批处理）。
- 如果您仅生成了预览或运行了批处理计算选项，请选择 PatternPreview 节点，运行 Preview Pattern（预览布局）命令，并生成方案。
- 您可以编辑现有研究，或复制一项研究并重命名。例如，进行修改、添加或更换容器，或更改原型数据。要重新配置研究，须手动重新加载，否则更改不会反映到配置中。

<a id="v8-s4"></a>
<!-- p1037 -->
#### 设置装箱配置（Setup packing configuration）

**过程（Procedure）**

1. 导入相关的物流定制（logistics customization）。默认位于：`C:\Program Files\Tecnomatix\eMPower\InitData`。
2. 为要加入装箱研究的所有容器和零件创建包围盒（bounding box）。有关创建包围盒的信息，请参阅 Bounding Box Calculation（包围盒计算）。
3. 验证装箱研究中容器的以下属性已配置为有效值：BottomHeight、InnerHeight、InnerLength 和 InnerWidth。否则，容器的空腔可能无法容纳零件。有关查看属性的信息，请参阅 Properties Viewer（属性查看器）。

为避免结果不一致，管理员应在定制中按下述方式为 Container 类的某些字段定义单位：

- InnerWidth、InnerLength、InnerHeight 和 BottomHeight 字段使用线性单位
- MaxFillingWeight 字段使用质量单位

> **注意（Note）**：要设置线性单位（mm、cm 等）和质量单位（kg、lbs）的类型，请使用 Options（选项）对话框中的 Units（单位）选项卡。

4. 单击[图标]。将出现 Administrative Configuration for Container Packing（容器装箱管理配置）窗口。

Administrative Configuration for Container Packing 窗口对所有用户可用，但只有系统管理员有权进行更改。它可以配置以下各项的位置：

- 临时装箱布局文件。
- 装箱指导书模板文件（PowerPoint 格式）。
- 用于存储装箱指导书的文件夹（装箱指导书文件附属于装箱方案，但存储在该位置）。
- CollisionLayerHorizontal 的位置。

确保所有路径均位于服务器系统根目录（server system root）之下。如果服务器和客户端的系统根位于不同位置，应将 CollisionLayerHorizontal 图形文件所在的文件夹按照服务器端定义的相同目录结构复制到客户端系统根。例如，复制

`Server system root\Reports\ContainerPacking\ContainerPacking_CollisionLayer.cojt`

到

`Client system root\Reports\ContainerPacking\ContainerPacking_CollisionLayer.cojt`

在 Administrative Configuration for Container Packing 窗口中，服务器系统根的路径格式应与 AdminConsole 中使用的格式一致。例如，两者均使用 UNC 路径。

5. 通过以下任一方式创建 CollisionLayerHorizontal：

- 手动 — 创建以下结构：
  - ContainerPackingLibrary（在定制中，派生自 ResourceLibrary）
  - CollisionLayerHorizontal（在 3D 文件的 Physical（物理）选项卡中手动将图形 `ContainerPacking_CollisionLayer.cojt` 设为其图形表示；在定制中派生自 ToolPrototype）
  - CompoundResource
  - ToolInstance（由 CollisionLayerHorizontal 实例化）
- 自动 — 定义创建 CollisionLayerHorizontal 的位置。可以是 ContainerPackingLibrary 或 UserFolder。系统将在下一阶段自动创建 CollisionLayerHorizontal。

6. 单击[图标]以创建装箱研究结构。

系统将创建装箱研究结构，并根据以下选项之一创建 CollisionLayerHorizontal：

- 如果检测到已有副本，系统不会创建 CollisionLayerHorizontal。
- 如果在启动 Create Packing Study Structure（创建装箱研究结构）命令之前已创建该文件夹，则系统在 ContainerPackingLibrary 中创建 CollisionLayerHorizontal。
- 如果没有 ContainerPackingLibrary，且您在启动 Create Packing Study Structure 命令之前已在 UserFolder 中创建了该文件夹，则系统在 UserFolder 的 ContainerPackingLibrary 中创建 CollisionLayerHorizontal。系统在修改 UserFolder 前会提示您。
- 如果 ContainerPackingLibrary 和 UserFolder 均不存在，系统无法创建 CollisionLayerHorizontal。您必须手动添加 CollisionLayerHorizontal。

7. 向装箱研究中添加恰好一个零件实例。
8. 向装箱研究中添加至少一个容器实例。

> **注意（Note）**：零件和容器都应是具有 3D 数据且已计算包围盒的实例。

<a id="v8-s5"></a>
<!-- p1040 -->
### 配置装箱研究（Configuring the Packing Study）

在开始配置之前，您必须在装箱研究的 TaskStudy 文件夹中添加至少一个零件和一个容器。

配置装箱研究：

**过程（Procedure）**

1. 在导航树（Navigation Tree）中，选择一项装箱研究并单击[图标]。将打开 Container Packing Configuration（容器装箱配置）向导，进入 Used Containers（已用容器）窗口。

Used Containers 窗口为研究中的每个容器显示一行，以及每个容器的以下参数：

- Inner Volume（内部体积）— 容器的内部体积，即可以存放零件的空腔，由下述容器参数计算得出。
- Capacity（容量）— 容器的存储容量，继承自原型容器。
- Inner Length（内部长度）— 容器的内部长度。
- Inner Height（内部高度）— 容器的内部高度。
- Inner Width（内部宽度）— 容器的内部宽度。
- BottomHeight（底部高度）— 从容器底部到其底面顶部的距离。

2. 单击 Next（下一步）。将显示 Part Orientations（零件方向）窗口。

Part Orientations 窗口显示待装箱零件的名称和重量（Weight）。此外，Stable Orientations（稳定方向）列表显示您可以选择用于装箱零件的稳定方向。如果将零件置于任何其他方向，它将倾倒。Stable Orientations 列表显示以下参数：

- Stable Orientation（稳定方向）— 稳定方向的名称。系统在创建稳定方向时自动分配。
- Value（值）— 稳定方向绕 x、y、z 轴的旋转值。
- Additional mirrored orientations（附加镜像方向）— 其他有效的稳定方向。它们基于稳定方向的值，但绕任意轴旋转 90、180 或 270 度得到。

创建或编辑稳定方向：

a. 在 Stable Orientations 列表中选择一个稳定方向。
b. 使用 Placement Manipulator（放置操纵器）调整零件的方向。
c. 单击 Add Current（添加当前）以使用该放置操纵器的当前值在对话框中添加一行新的稳定方向。或者单击 Apply Current（应用当前）将所选稳定方向的值更改为放置操纵器的值。
d. 可选地，您可以添加镜像方向派生项，或按如下方式删除方向：

- 单击稳定方向旁的[图标]。将出现 Mirrored Orientations（镜像方向）列表。该列表显示所选稳定方向所有可能的镜像方向。选择您要允许的方向并单击 OK（确定）。
- 选择稳定方向并单击 Remove（移除）。

3. 单击 Next。将显示 Heuristics（启发式）窗口。
4. 在 Layer Heuristic（层启发式）区域，勾选以下之一：

- Homogeneous（同质）— 容器中的所有层完全相同。
- Heterogeneous（异质）— 容器中的层可以不同。

5. 在 Block Heuristic（块启发式）区域，拖动滑块以选择装箱方案允许的块（block）数量。每个块可包含按两个方向对齐的零件，例如朝右和朝左。
6. 单击 Next。将显示 Pattern Configuration（布局配置）窗口。
7. 在 Interspacings（间距）区域，配置零件之间保留的间隙。例如，如果您希望为零件周围的包装材料预留空间，就需要这样做。
8. 勾选以下之一以配置零件重叠：

- Flexible Intermediate Layers（柔性中间层）— 允许零件重叠。将 Layers on Z 设为较低值以允许较大重叠，或设为较高值以允许较小重叠。
- Rigid Intermediate Layers（刚性中间层）— 确保零件不重叠。

9. 在 Number of Parts（零件数量）中，您可以选择性地设置 Minimum（最小值）和 Maximum（最大值）。例如，如果您知道超过 100 个零件的容器会过重，则设置 `Maximum=100`；如果少于 10 个零件的容器效率过低，则设置 `Minimum=10`。系统仅返回满足这些条件的方案。
10. 单击 Close（关闭）。系统将提示您确认配置。
11. 单击 OK 关闭 Container Packing Configuration 向导。

<a id="v8-s6"></a>
<!-- p1047 -->
### 计算装箱方案（Calculating the Packing Solution）

**过程（Procedure）**

1. 单击[图标]。将出现 Calculating Packing Solution（计算装箱方案）对话框。
2. 如果您希望在图形查看器（Graphic Viewer）中查看系统的所有计算过程，请勾选 Refresh Graphic Viewer during calculation（计算期间刷新图形查看器）。由于该选项会减慢计算速度，请仅在需要此功能时使用。无论如何，系统在计算装箱方案时都会更新图形查看器，但这比开启该选项时快得多。
3. 单击 OK。

计算将运行，系统显示进度条。

> **注意（Note）**：如果您尝试为未配置的装箱研究计算装箱方案，系统将提示您打开 Container Packing Configuration 向导。

计算完成后，系统将提出装箱方案。

Container Packing Solutions（容器装箱方案）对话框为每个提出的方案提供以下参数：

| 参数（Parameter） | 说明（Description） |
|---|---|
| Generate（生成） | 勾选此项以生成装箱方案。 |
| Container（容器） | 装箱方案中使用的容器名称。 |
| Number of Parts（零件数量） | 装箱方案中的零件数量。 |
| Number of Layers（层数） | 装箱方案中的层数。 |
| Layer Heuristic（层启发式） | 装箱方案的层启发式；同质或异质。 |
| Block Heuristic（块启发式） | 装箱方案的块启发式；即装箱块的数量。 |
| Number of Orientations（方向数量） | 装箱方案中的零件方向数量。 |
| Stable Orientation（稳定方向） | 装箱方案的稳定方向。 |
| Platter Weight（托盘重量） | 零件、容器和托盘的总重量。 |
| Inner Volume（内部体积） | 容器的内部体积。 |
| Comment（注释） | 相关时的注释。 |

4. 可选地，单击 Create Preview Of All Solutions（为所有方案创建预览）。系统将为所有提出的方案创建预览。

> **注意（Note）**：如果您单击了 Create Preview Of All Solutions，可以在此阶段单击 Close（关闭）并退出 Container Packing Solutions 对话框，而不会丢失任何信息。当您希望继续会话时，请单击 Preview Packing Pattern（预览装箱布局）[图标] 以打开 Container Packing Solutions 对话框。在此工作流中，Create Preview Of All Solutions 按钮不可用。

5. 在导航树中，单击方案结构的 Pattern Preview 文件夹中的某个预览，以在图形查看器中查看提出的方案。
6. 可选地，单击 Compress to equivalent solutions（压缩为等效方案）。系统将所有等效方案显示为单个方案。如果两个方案在以下参数上具有相同值，则视为等效：

- 稳定方向
- 层数
- 层启发式
- 块启发式
- 不同方向的数量

在等效方案中，系统显示零件数量最多的方案。

7. 从 Layers to generate（要生成的层）下拉列表中，选择以下之一：

- all Layers（所有层）— 系统为装箱方案的每一层生成预览。用于异质装箱方案。
- Bottom Layer（底层）— 系统为装箱方案的底层生成预览。用于所有层均相同的同质装箱方案。

8. 勾选您感兴趣的提出方案，并单击 Generate（生成）。系统在方案结构的 Solutions（方案）文件夹中创建并存储方案。
9. 在方案结构的 Solutions 文件夹中，选择一项装箱方案并单击[图标]以创建（或更新）装箱指导书。系统创建一份报告（PowerPoint 格式的演示文稿）并将其附属于装箱方案。
10. 如果您希望向报告添加快照（snapshot），请右键单击装箱方案并选择 Load（加载）。您现在可以使用 Snapshot Editor（快照编辑器）创建装箱方案的快照。快照将添加到 Robcad 研究信息中，在您执行保存时，它们会自动附属于方案节点。

附属于方案节点的快照会自动插入到装箱报告的快照区域中。

<a id="v8-s7"></a>
<!-- p1053 -->
## 容器装箱批处理（Container Packing Batch）

如果您已创建若干装箱研究并进行了配置，可以使用批处理功能创建装箱预览。

**过程（Procedure）**

1. 在导航树中，选择装箱研究并单击[图标]。将出现 Container Packing Batch（容器装箱批处理）对话框。
2. 单击 Calculate Studies（计算研究）。对于批处理中的每项装箱研究，系统在其 PreviewPattern 文件夹（嵌套在 Solution 文件夹下）中为所选择的每个方案创建预览。
3. 选择 PreviewPattern 节点并单击 Preview Packing Pattern（预览装箱布局）[图标] 以打开 Container Packing Solutions 对话框，参见“计算装箱方案（Calculating the Packing Solution）”。

<a id="v8-s8"></a>
<!-- p1054 -->
## 物流模块（Logistics Module）

<a id="v8-s9"></a>
<!-- p1054 -->
### 网络规划（Network Planning）

网络规划（Network Planning）允许用户定义物流网络，并使用以下步骤计算每个零件族（part family）的生产率：

**过程（Procedure）**

1. 设置网络结构（network structure）。
2. 设置零件结构（BOM，物料清单）。
3. 为每个物流工厂（logistic plant）设置一个资源（resource）。
4. 将代表物流工厂的资源分配给网络结构的物流工厂。确保该资源属于 Supplier（供应商）类型。
5. 将生产的零件分配给物流工厂项目（在 Logistics（物流）选项卡中），并在 Attributes（属性）选项卡中定义生产率（production rate）。
6. 基于 BOM 自动创建零件族。BOM 中的每个零件创建一个零件族，并将相关零件分配给该零件族。或者，可以在库中定义零件族并将其复制到物流项目下。确保为每个零件族分配一个或多个零件。基于您定义的信息，系统将计算工厂之间所需的运输（transportation）关系。计算得到的运输关系包括计算出的产品数量和所需零件数量。

<a id="v8-s10"></a>
<!-- p1057 -->
### 过程检查（Process Check）

在 Properties（属性）对话框中，物流规划人员可以打开 Process Check（过程检查）选项卡以查看物流规划的进度。

过程检查始终在 LogProcess（物流过程）对象上运行——可以是单个对象，也可以是子树中的所有 LogProcess 对象。对于 LogProcessFolders、LogProjects、LogPlants 和 Log Networks，系统可以对其节点下的整个 LogProcess 对象层级运行预定义检查。

Process Check 选项卡提供以下预定义检查：

- Parts assigned to LogProcess（分配给 LogProcess 的零件）：是否至少有一个零件分配给 LogProcess 对象？
- Container assigned LogProcess（分配给 LogProcess 的容器）：是否恰好有一个容器分配给 LogProcess 对象？
- Container assigned to logistics operations（分配给物流操作的容器）：是否恰好有一个容器分配给 LogProcess 供应链的 Move/Store（移动/存储）操作？
- Supply Chain defined（已定义供应链）：是否为 LogProcess 定义了供应链？
- Supplier assigned（已分配供应商）：是否向 LogProcess 分配了供应商？
- Areas assigned to store operations（分配给存储操作的区域）：是否有 LogAreas 分配给供应链的存储操作？
- Times assigned to logistics operations（分配给物流操作的时间）：是否向物流操作分配了时间，即供应链的所有物流操作是否都分配了 > 0 的时间？
- 一般检查，用于验证已分配的容器、供应商、区域等是否来自正确定义的库：系统检查以下类型的所有已分配资源——LogArea、LogContainer、Supplier、Vehicle（车辆）、Transporter（运输工具）、SupplyChains（用于链接的供应链）——是否属于 LogProcess 所在工厂各自的库中。库取自 `LogPlant.LibrarySuppliers` 等字段。
- 一致性检查（Consistency check），用于将物流计划与生产计划进行核对以发现任何不一致。该检查基于生产规划和物流规划使用相同的零件和物流区域。如果零件及相应的物流区域与生产计划中分配到相同的工位（station），则物流计划是一致的。

单击每项过程检查左侧的复选框以选择它，并单击 Start Process Check（启动过程检查）以执行您选择的检查。

如果您对单个对象（例如一个 LogProcess）运行过程检查，选项卡将通过显示 OK 或 not OK 来反映您所选测试的成功或失败。

如果您对多个对象（例如 LogProcessFolder、LogProject、LogPlant 和 Log Network）运行过程检查，选项卡将显示通过测试的对象百分比。右侧的窗格列出未通过的对象以及它们未通过的检查，使您能够识别哪些对象需要进一步修改。

<a id="v8-s11"></a>
<!-- p1060 -->
### 在资源树中创建容器（Create Container in Resource Tree）

为了实现部门间的协作，可以使用正确的容器数量更新资源树（Resource Tree）的全部或部分。

要使用此功能，您必须首先确保在操作树（Operation Tree）中：

- 在每个零件族层级的下方设置了供应链（supply chain）或链接的供应链。
- 在供应链层级下方设置了供应操作（provision operation）。
- 每个物流区域（Logistics area）都分配给其供应操作（因此列在操作 Resource（资源）选项卡的列表中）。
- 每个零件族分配了一个容器原型（container prototype）。
- 为每个零件族分配了正确数量的容器（使用属性 ContainersAtLine 设置）。
- 设置了正确数量的变体（variant）（使用属性 VariantsTotal 设置）。

系统将容器数量计算为 ContainersAtLine 与 VariantsTotal 的乘积。这可以通过以下两种方式之一实现：

- 通过为每个供应操作定义 ContainersAtLine 属性，可以为每个零件族定义单独的容器数量。完成此操作后，将忽略父族的 ContainersAtLine 属性。
- 当零件族中所有供应操作的容器数量相同时，仅为零件族定义 ContainersAtLine（并删除供应操作的任何 ContainersAtLine 属性）。

> **注意（Note）**：在这两种情况下，VariantsTotal 的值必须大于 0 才能进行计算。

要在资源树中更新容器数量：

**过程（Procedure）**

1. 选择一个或多个 LogProcess 对象。
2. 选择 Logistics（物流）选项卡 → Tools（工具）组 → Connect Logistics（连接物流）[图标]。

每个位置的容器数量将在资源树中 Log area（物流区域）下更新。

- 如果已分配资源，系统会提供删除它们的选项。
- 如果您多次运行 Connect Logistics 命令，并对 ContainersAtLine 和 VariantsTotal 属性使用不同的值，系统将分配计算出的容器数量，已分配的同类容器将被计入，其他容器可能被保留或删除。

<a id="v8-s12"></a>
<!-- p1061 -->
### 物流区域与轨道（Logistics Areas and Tracks）

物流区域和轨道功能用于构建物流路径网络。用户可以通过绘制区域和轨道，并使用连接点（connection point）将它们连接到其他物流区域或轨道，来描述完整的区域和路径网络。这些命令允许您定义轨道的方向，并将其设置为单向（one-way）或双向（two-way）。定义的路径网络可用作路线（route）和运输（transport）时间计算的基础。

Draw / Hide / Show / Delete LogAreas（绘制/隐藏/显示/删除物流区域）命令以及 Draw / Hide / Show / Delete LogTracks（绘制/隐藏/显示/删除物流轨道）命令可从 Logistics（物流）选项卡 → Areas & Tracks（区域与轨道）组获得。

<a id="v8-s13"></a>
<!-- p1061 -->
#### 物流区域（Logistic Areas）

<a id="v8-s14"></a>
<!-- p1061 -->
##### 绘制物流区域（Draw Logistics Areas）

此命令用于已加载的复合资源（compound resource）。如果复合资源下已定义了物流区域，该命令将从 Logistics（物流）选项卡 → Areas & Tracks（区域与轨道）组以编辑（Edit）模式打开 Draw Area（绘制区域）对话框。对于尚未定义物流区域的情况，该命令以新建（Create New）模式打开 Draw Area 对话框。

<a id="v8-s15"></a>
<!-- p1062 -->
##### 显示物流区域（Show Logistics Areas）

此命令包含两个子命令：Show Logistics Area（显示物流区域）[图标] 和 Show All Logistics Areas（显示所有物流区域）[图标]。选择一个或多个已加载的复合资源以执行该命令。Show Logistics Area 显示所选复合资源已定义的物流区域。Show All Logistics Areas 子命令显示所选复合资源及其已加载后代复合资源的物流区域。如果存在未加载的后代复合资源，系统将提示您加载它们并显示其物流区域。

<a id="v8-s16"></a>
<!-- p1063 -->
##### 隐藏物流区域（Hide Logistics Areas）

此命令有两个选项：Hide Logistics Area（隐藏物流区域）[图标] 和 Hide All Logistics Areas（隐藏所有物流区域）[图标]。选择一个或多个已加载的复合资源以执行该命令。Hide Logistics Area 隐藏所选复合资源已显示的物流区域。Hide All Logistics Areas 隐藏所选复合资源及其已加载后代复合资源的物流区域。

<a id="v8-s17"></a>
<!-- p1063 -->
##### 删除物流区域（Delete Logistics Areas）

选择一个或多个已加载的复合资源后，运行两个子命令之一：Delete Logistics Area（删除物流区域）[图标] 和 Delete All Logistics Areas（删除所有物流区域）[图标]。Delete Logistics Area 隐藏所选复合资源已定义的物流区域并删除其数据。Delete All Logistics Areas 隐藏所选复合资源及其已加载后代复合资源的物流区域并删除其数据。如果存在未加载的后代复合资源，系统将提示您加载它们并删除其物流区域。

<a id="v8-s18"></a>
<!-- p1063 -->
#### 物流轨道（Logistic Tracks）

<a id="v8-s19"></a>
<!-- p1063 -->
##### 绘制物流轨道（Draw Logistics Tracks）

在已加载的复合资源或已加载的物流轨道对象上运行此命令。当在物流轨道上执行时，该命令以编辑模式打开 Draw Track（绘制轨道）对话框。当在复合资源上执行时，该命令检查复合资源是否具有后代物流轨道，并为第一个已加载的物流轨道对象以编辑模式打开 Draw Track 对话框。如果复合资源没有后代物流轨道，该命令将在复合资源下创建一个，并以新建模式打开 Draw Track 对话框。

<a id="v8-s20"></a>
<!-- p1064 -->
##### 显示物流轨道（Show Logistics Tracks）

当选择了一个或多个已加载的复合资源（或物流轨道对象）时，使用两个子命令之一：Show Logistics Track（显示物流轨道）[图标] 或 Show All Logistics Tracks（显示所有物流轨道）[图标]。Show Logistics Track 显示所选物流轨道，而 Show All Logistics Tracks 显示所选物流轨道以及所选项的已加载后代物流轨道。如果存在未加载的后代物流轨道，系统将提示您加载它们并显示其物流轨道。

<a id="v8-s21"></a>
<!-- p1065 -->
##### 隐藏物流轨道（Hide Logistics Tracks）

此命令的两个选项 Hide Logistics Track（隐藏物流轨道）[图标] 和 Hide All Logistics Tracks（隐藏所有物流轨道）[图标] 可在选择一个或多个已加载的复合资源（或物流轨道对象）后执行。Hide Logistics Tracks 隐藏所选物流轨道，Hide All Logistics Tracks 隐藏所选物流轨道及其已加载后代物流轨道。

<a id="v8-s22"></a>
<!-- p1065 -->
##### 删除物流轨道（Delete Logistics Tracks）

选择一个或多个已加载的复合资源后，使用两个子命令之一：Delete Logistics Track（删除物流轨道）[图标] 或 Delete All Logistics Tracks（删除所有物流轨道）[图标]。Delete Logistics Track 隐藏所选物流轨道并删除其物流轨道对象。Delete All Logistics Tracks 隐藏所选物流轨道并删除其物流轨道对象，如果所选项具有已隐藏的后代物流轨道，这些也将被删除。

<a id="v8-s23"></a>
<!-- p1066 -->
### 规划物流 - 数据模型（Planning Logistics - Data Model）

数据模型（Data Model）包含下表所列的各种附加类和属性。

**类（Class）：**

| 类名称（Class name） | 继承自（Inherited From） |
|---|---|
| Shelf | PmToolPrototype |
| Trailer | PmToolPrototype |

**属性（Attribute）：**

| 类（Class） | 属性（Attribute） | 类型（Type） |
|---|---|---|
| Supplier | Email | classPuString |
| Supplier | PhoneNumber | classPuString |
| Supplier | StreetNumber | classPuString |
| LogArea | GeoLongitude | double |
| LogArea | GeoAltitude | double |
| Container | PartCarrier | int |
| ToolInstance | ContainerLayers | int |
| ToolInstance | ContainersPerLayer | int |

为简化定制，Tecnomatix 删除了下表所列的、标准应用程序未使用的以下类和属性。

**类（Class）：**

| 类名称（Class name） | 继承自（Inherited From） |
|---|---|
| ProcessTime | PmCompoundOperation |
| TimeBuildingBlock | PmOperation |
| LogView | PmCollection |
| LogViewFolder | PmModule |

**属性（Attribute）：**

| 类（Class） | 属性（Attribute） | 类型（Type） |
|---|---|---|
| LogPlant | LibraryTimeBuildingBlocks | classPuString |
| LogProject | ProductionRate | int |
| LogViewFolder | logStructureAttributeValue | classPuString |
| LogView | LogStructureRoot | classPuString |
| LogView | logStructureAttribute | classPuString |
| PmSupplyChain | CirculationDays | double |
| PmSupplyChain | LotSize | int |
| PmSupplyChain | SafetyStock | double |
| PmSupplyChain | StockCoverageRange | double |
| LogOperation | CopiedFrom | classPuString |
| LogOperation | MaxContainerThroughput | int |
| LogOperation | HandlingTimeContainer | double |
| LogOperation | ReferenceProcessTime | classPuString |
| Move | TransportationTime | double |
| Store | StorageHeightDemand | double |
| Handling | HandlingTimePart | double |
| ProcessTime | TotalTime | double |
| ProcessTime | TimeBuildingBlockList | classPuString |
| TimeBuildingBlock | ObjectType | classPuString |
| LogContainer | TareWeight | double |
| LogContainer | Package | int |
| LogContainer | Pallet | classPuString |
| LogContainer | CoverPlate | classPuString |
| LogContainer | Trailer | classPuString |
| LogContainer | CostContainer | double |
| LogContainer | CostDevelopment | double |
| LogContainer | CostDailyRent | double |
| Vehicle | Width | int |
| Vehicle | Length | int |
| Vehicle | Height | int |

<a id="v8-s24"></a>
<!-- p1067 -->
### 从库链接到树（Linking from a Library to a Tree）

当您在树中创建链接操作（linked operation）时，只要对原始操作进行更改，该链接操作也会随之更改。这在规划期间特别有用，因为此时使用的标准供应链处于不断变化的状态。链接而非复制，简化了确保所有方案保持最新的过程。

> **注意（Note）**：创建链接节点（linked node）时，只能对库中的源节点进行更改。您无法编辑链接的副本。

通过从库拖放添加链接节点：

**过程（Procedure）**

1. 打开库树（Library tree）并选择一个节点。
2. 按住 Ctrl 键，单击鼠标左键，并将所选节点拖到相关树中已签出（checked-out）的复合节点。如果系统能够链接该操作，它会高亮显示该复合节点。
3. 松开鼠标按钮，然后松开 Ctrl 键——链接节点将出现在该复合节点下方。

<a id="v8-s25"></a>
<!-- p1068 -->
#### 断开链接（Severing Links）

如果您需要对链接的供应链进行本地更改而不影响原始供应链，必须首先断开两条链之间的链接。此过程不可逆。

要断开链接节点与其源之间的链接：选择链接节点，右键单击，并从右键菜单中选择 Sever Link（断开链接）。
<a id="v9-s1"></a>
# 9. 特殊数据（Special Data）

<!-- p1069 -->

<a id="v9-s2"></a>
## 在制品装配（In Process Assembly，IPA）

<!-- p1069 -->

在制品装配（In-Process Assembly，IPA）技术会根据所加载工艺的物料消耗，在 Process Designer / Process Simulate 中自动加载装配数据。用户随后可在 IPA Viewer（IPA 浏览器）以及图形浏览器中按工位顺序显示这些数据。这些装配数据可用于各类针对具有物理存在的对象进行操作的工程功能，例如创建引用该数据的新操作（Operation）与事件（Event）。这使用户能够清晰地了解装配顺序以及沿装配线向各工位的分配情况，同时保留装配数据的物理结构。

装配数据在加载到 Process Designer / Process Simulate 时，会根据所加载的工艺信息即时收集。一旦打开，装配数据便与各种现有功能交互，并参与查看、编制与校验装配过程的各类工作流。

IPA 技术为用户提供了一种快速、简便地解答有关装配过程（工厂 / 区域 / 产线 / 工位）问题的方法，例如：

- 装配线上每个工位装配什么？
- 进入产线上任一工位的累计产品是什么？
- 装配顺序是什么（表现为装配树中工位的顺序）？

IPA 技术的运行原理在后续段落中进一步详述。

在 Process Designer / Process Simulate 中加载工艺数据时，其加载结果用于加载所有必要的产品数据。随后，新产品可通过将产品按反映装配顺序及工位沿工艺对产品消耗情况的层次结构进行分组，在图形与树中提供有意义的数据表示。

加载后，装配数据（由所有在制品装配组成）参与所有与零件相关的工程活动。此外，装配数据的逻辑分组使用户能够清晰区分各工位的消耗情况，并可在不破坏已装配产品之间关系的前提下轻松进行空间定位。

用户可以在 Process Designer / Process Simulate 中，从工艺相关数据与工程相关数据中创建指向装配数据的引用。他们可以通过维护被引用装配在工艺中的角色（而非引用装配内的具体对象）来使这些引用持久化。这样就可以维护“智能”引用，使其随物料流的变更自动更新（例如，用户可以在一个操作与另一个操作的输出之间创建引用，而不是引用具体零件）。这些引用可以是任意事件，例如毛坯（blank）、显示（display）、安装（attach）与拆卸（detach）。例如，假设存在一个对象流将 Station1 的 IPA 传送进 Station2（这可以通过将 Station1 的 IPA 选作该操作的已处理对象来实现），并且该 IPA 由两部分组成。随后，用户修改工艺，使另一个零件被添加到 Station1 的 IPA 中。对象流操作会自动传送 Station1 的 IPA 的全部三个零件，无需任何用户交互。

<a id="v9-s3"></a>
### 使用 IPA 技术的工作流（Workflow when Using the IPA Technology）

<!-- p1070 -->

使用 IPA 技术的总体工作流包括创建装配树并加载相关工艺：

**步骤**

1. 选择一个工艺，并选择 **Special Data 选项卡 → IPA 组 → Generate Assembly Tree（生成装配树）**。此时打开 Generate Assembly Tree 对话框。
2. 点击 **Browse（浏览）** 按钮。此时打开 Target Folder（目标文件夹）对话框。
3. 选择一个文件夹并点击 **OK**。Target Folder 对话框关闭，所选文件夹显示在 Generate Assembly Tree 对话框的 Target Folder 字段中。
4. 在 Generate Assembly Tree 对话框中点击 **OK**。

<a id="v9-s4"></a>
### IPA 与作用域流（IPA and scope flows）

<!-- p1072 -->

在目标文件夹下创建一个无层次的扁平装配树（骨架）。

**注意**

- 装配树中元素的顺序通常与工艺顺序不对应；无论如何，用户绝不应直接操作装配树（例如，用于加载 / 重新排序工艺）。
- Generate Assembly Tree 命令仅为所选操作及其后代创建 IPA 节点。
- 如果装配树生成由多个用户执行，则 IPA 节点会分散到各用户选择创建的文件夹之间。例如，用户 A 创建的 IPA 节点保留在用户 A 的文件夹中，即使用户 B 后来更新了它，但用户 B 新建的 IPA 节点会放置在用户 B 的文件夹中。
- 在 Tecnomatix 7.6.1 之前版本中创建的装配树必须重新生成（或删除后重建），以确保正确的 IPA 加载。更新旧的装配树时，更新后的树会覆盖已有的树，忽略所定义的目标文件夹。

5. 加载一个相关装配树已在步骤 1 中生成的工艺。

当工艺加载时，该工艺的零件也会加载，用户可以在 IPA Viewer 或 Relations Viewer（关系浏览器）中引用在制品装配。

**注意**

- 加载 IPA 数据随工艺加载，需要存在已更新的装配树。
- IPA Viewer 树不反映加载后在研究中完成的分配（allocation）。
- 以下内容会出现在 IPA Viewer 与 Product/Object 树中：
  - 作为快捷方式加载到研究中的 IPA 层次结构中的零件，或作为快捷方式加载的子树中的零件；
  - 分配给已加载操作 / 焊点（weld points）的零件。
- 在一个浏览器中选择某个零件会导致其在显示该零件的所有浏览器中被选中。
- 当工艺包含作用域流（scope flows）时，IPA Viewer 中显示的树遵循本节描述的某些规则。

IPA 算法会考虑进入工位、产线与区域的作用域流。使用作用域流的一个常见示例是：为门线（door line）进入装饰线（trim line）上的某工位时的物料流建模。

在 Process Simulate 中加载 IPA 时，会创建以下层次结构：

- 具有至少一个常规输出流或作用域输出流的操作，其 IPA 会以该流的后继的 IPA 作为父节点。
- 如果操作有多个输出流，则 IPA 父节点会被任意选择。
- 如果操作没有任何输出流，且其父操作的 IPA 已加载，则该操作的 IPA 会添加到父操作的 IPA 中。
- 否则，该操作的 IPA 会成为 IPA 浏览器中的根节点。

**注意**

关于作用域流零件的加载，以下条件相关：

- 如果加载了 IPA 结构，作用域流的零件也会被加载。这些零件会根据常规流零件的既有逻辑，显示在 Product 树或 IPA 浏览器中。
- 当未加载任何 IPA 时，作用域流的零件不会作为该操作的闭合（closure）被加载。

以下是包含作用域流与不包含作用域流的工艺的 IPA 树示例。

<a id="v9-s5"></a>
#### 示例 1（Example 1）

<!-- p1073 -->

本示例中的工艺有三条产线，没有作用域流。Line1 有两个相连的工位（Station11 和 Station12），Line2 有三个相连的工位（Station21、Station22 和 Station23），Line3 有一个工位（Station31）。

加载工厂工艺时，Logical Collections Tree（逻辑集合树）中生成的树如下图所示。

- IPA 根节点为 Plant 工艺本身。
- Plant 工艺的直接子节点是最后一个顶层工艺 Line3。
- Line3 的子节点是其子节点 Station31。
- Line2 是链接到 Line3 的前驱工艺，因此它是 Station31 的子节点。
- Line2 的所有工位均相连，因此线中最后一个工位（Station23）是 Line2 的直接子节点，且每个工位都是其链接到的工位的子节点。
- Line1 及其工位（Station11、Station12）遵循与上述 Line2 相同的规则。

<a id="v9-s6"></a>
#### 示例 2（Example 2）

<!-- p1074 -->

本示例中的工艺与示例 1 类似，区别在于 Line2 与 Line3 之间没有流，而是存在一条作用域流：Line3 链接到 Line2 的 Station22。以下两幅图分别显示了 Plant 工艺与 Line2 工艺的 PERT 视图；两幅图均显示了 Line3 与 Line2 的 Station22 之间的作用域流。

IPA Viewer 中生成的树如下图所示。注意 Line3 的 IPA 父节点是 Station22。

<a id="v9-s7"></a>
#### 示例 3（Example 3）

<!-- p1075 -->

本示例中的工艺与示例 2 类似，区别在于还显示了每个工位的零件。

部分折叠的 Logical Collections Tree 中生成的树如下图所示。注意，分配给某个工艺的零件显示为该 IPA 工艺的兄弟节点。例如，分配给 Station23 的 Part5 显示为该工位工艺的子节点，并且是 Station22 的兄弟节点（二者均馈入 Station23 工艺）。

完全展开的 Logical Collections Tree 中生成的树如下图所示。

<a id="v9-s8"></a>
#### 示例 4（Example 4）

<!-- p1078 -->

本示例中的工艺与示例 2 类似，区别在于有两条作用域流：Line3 同时链接到 Line2 的 Station21 和 Station22。

如 IPA Viewer 的规则所述，Line3 有两个父节点：Station21 和 Station22。IPA Viewer 中生成的树同时显示 Station21 和 Station22 作为 Line3 的父节点。

<a id="v9-s9"></a>
### 支持在线通知与更新（Support for Online Notifications and Updates）

<!-- p1079 -->

系统仅根据对已加载操作的修改来更新 IPA（IPA Viewer）结构。IPA 树与以下变更保持同步：

- 对操作层次的修改（不包括孪生（twin）操作或替换孪生操作的层次变更）；
- 删除操作；
- 零件向操作的分配 / 取消分配。

添加新操作不会影响 IPA 树。

<a id="v9-s10"></a>
### 清理装配树（Cleaning the Assembly Tree）

<!-- p1079 -->

装配树的扁平结构使得难以识别某个 IPA（IPA Viewer）节点何时脱离了层次结构，因此与工艺中已不存在的操作相关联的 IPA 节点可能会被保留。虽然它们不会破坏 Process Designer / Process Simulate 中的数据加载，但这些 IPA 节点会导致无用的对象残留在数据库文件夹中。

要清理数据库中无关的 IPA 节点：

**步骤**

1. 选择 **Special Data 选项卡 → IPA 组 → Clean Assembly Tree（清理装配树）**。此时打开 Clean Assembly Tree 对话框。
2. 点击 **Browse（浏览）** 按钮。此时打开 Target Folder 对话框。
3. 选择一个文件夹并点击 **OK**。Target Folder 对话框关闭，所选文件夹显示在 Clean Assembly Tree 对话框的 Target Folder 字段中。
4. 在 Clean Assembly Tree 对话框中点击 **OK**，以清除文件夹中无关的 IPA 节点。

<a id="v9-s11"></a>
### 定义用于启动和停止 IPA 生成的基类（Defining Base Classes for Starting and Stopping IPA Generation）

<!-- p1080 -->

用户可以选择为继承自 `PmProcess`、且在 `PrStationProcess` 级别停止 IPA（IPA Viewer）创建遍历的那些类的对象创建 IPA。或者，他们也可以自定义为哪些类创建 IPA 节点、以及哪些类停止遍历（请参阅 Tecnomatix 管理文档中的 Administrative and Management Tools 部分）。

用户应定义两个类：

- **用于 IPA 生成的基类（Base class for IPA generation）** —— 该基类及其任意继承类的对象都适合进行 IPA 生成。
- **用于停止 IPA 生成遍历的基类（Base class for halting the traverse of IPA generation）** —— 收集用于 IPA 生成的操作的遍历会停止在该类或其任意继承类的对象上。

<a id="v9-s12"></a>
## 替换对象（Replace Objects）

<!-- p1081 -->

<a id="v9-s13"></a>
### 查找替换对象（Finding Replacement Objects）

<!-- p1081 -->

Replace Objects（替换对象）命令使用户能够查找已过时（例如数据采集之后）的零件、制造特征（Mfg）或资源的替换对象。它是一个复合命令，由以下命令组成：Replace Parts（替换零件）、Replace Mfgs（替换制造特征）和 Replace Resources（替换资源）。术语 Replace Objects 指代上述全部三个命令。

Replace Objects 命令使用户能够执行以下操作：

- 替换实例（instances）。
- 将过时对象（obsolete objects）的连接重新分配给新对象。
- 选择是否从过时对象中删除连接。
- 该命令支持继承自以下类型的对象：`PmPart`、`PmResource` 和 `PmMfgFeature`。提供以下替换方式：
  - 相同类型对象之间的替换。例如，CompoundPart 与 CompoundPart 之间，或 ToolInstance 与 ToolInstance 之间。
  - 不同类型但继承自同一族（Part、Resource 或 MfgFeature）的对象之间的替换。例如，用 CompoundResource 替换 ToolInstance。
- 指定要将哪些属性从源对象传递到目标对象。

下表列出了 Replace Objects 命令所替换的连接。

| 相关软类描述（Related Soft Class Description） | 移除源对象的现有连接（Remove Existing Connections of Source Objects） | 保留源对象的现有连接（Keep Existing Connections of Source Objects） |
| --- | --- | --- |
| **PmPart** — `PmFlow`：与操作的连接（Connection to Operation） | 将目标零件连接到现有 `PmFlow` 对象，并断开源零件。如果 `PmFlow` 对象位于源 / 汇（source/sink）与操作之间，则创建一个新的 `PmFlow` 对象和一个新的源 / 汇。 | 如果 `PmFlow` 对象位于两个操作之间，则将目标零件连接到现有 `PmFlow` 对象。 |
| **PmMfgFeature** — 连接到源零件的 `PmMfgFeature` 对象 | 将 `PmMfgFeature` 对象从源零件重新分配给目标零件。 | 将 `PmMfgFeature` 对象分配给目标零件。 |
| **PmBuffer** — 连接到源零件的 `PmBuffer` 对象 | 将源零件连接到 `PmBuffer` 对象，并断开目标零件。 | 将 `PmBuffer` 对象分配给目标零件。 |
| **PmResource** — `PmUsage`：与操作的连接 | 将目标资源连接到现有 `PmUsage` 对象，并断开源资源。 | 创建连接到源资源的 `PmUsage` 副本，并将该副本连接到目标资源及相关操作。 |
| **PmResource** — `PmVariantSet`：变体（Variants） | 将 VariantSet 从源资源移动到目标资源。 | 将 VariantSet 设置到目标资源。 |
| **PmResource** — `PmProgram`：机器人程序（Robotic programs） | 机器人程序不会从源机器人移动到目标机器人。 | 机器人程序不会从源机器人复制到目标机器人。 |
| **PmResource** — `PmTool BoxUsage Robot` | 将目标资源连接到现有 `PmToolBoxUsage` 对象，并断开源资源。 | 创建连接到源资源的 `PmToolBoxUsage` 对象副本，并将目标资源连接到该副本。 |
| **PmResource** — `PmTool BoxUsage Tool` | 将目标资源连接到现有 `PmToolBoxUsage` 对象，并断开源资源。 | 此关系并非总是被复制，它总是被移动。 |
| **PmResource** — `PmLayout`：安装信息（Mount Information） | 将已安装对象从源资源断开，并安装到目标资源上。 | 安装信息不被复制，它总是被移动。 |
| **PmMfgFeature** — 分配给源资源的 `PmMfgFeature` 对象 | 将 `PmMfgFeature` 对象从源资源重新分配给目标资源。 | 将 `PmMfgFeature` 对象分配给目标资源。 |
| **PmMfgFeature** — `PmOperation`：与操作的连接 | 通常将目标 MfgFeature 连接到操作，并断开源 MfgFeature。对于 WeldOperation（焊接操作），从 WeldOperation 断开源 MfgFeature，并使用现有的 `WeldLocationOperation` 将目标 MfgFeature 连接到操作。 | 此关系不被复制。 |
| **PmMfgFeature** — `PmToolInstance`：源 `PmMfgFeature` 对象所分配到的 `PmToolInstance` 对象 | 将目标 MfgFeature 对象连接到 `PmToolInstance` 对象，并断开源 MfgFeature。 | 将目标 MfgFeature 对象连接到 `PmToolInstance` 对象。 |
| **PmMfgFeature** — 连接到源 `PmMfgFeature` 对象的 `PmPart` 对象 | 将目标 MfgFeature 连接到 `PmPart` 对象，并断开源 MfgFeature。 | 将目标 MfgFeature 连接到 `PmPart` 对象。 |
| **PmMfgFeature** — 连接到源 `PmMfgFeature` 对象的 `PmPartPrototype` 对象 | 将目标 MfgFeature 连接到 `PmPartPrototype` 对象，并断开源 MfgFeature。 | 将目标 MfgFeature 连接到 `PmPartPrototype` 对象。 |
| **PmMfgFeature** — `PmMfgUsage`：PLP | 将目标 MfgFeature 连接到现有 `PmMfgUsage` 对象，并断开源 MfgFeature。 | 创建现有 `PmMfgUsage` 对象的副本，并将该副本连接到目标 MfgFeature 对象及相关操作。 |

**注意**

- 以下类型的数据不会被复制或移动，并可能在替换操作中丢失：
  - 工程数据，例如碰撞集（collision sets）；
  - 仿真信息，例如对象流操作；
  - PLC 信息。

系统在此情况下显示以下消息：

> 如果数据丢失，对象可能缺少研究（studies），并且仿真可能产生意外结果。请执行手动更新。

- 即使在目标对象被用作替换对象之后，它们仍保留其原有的子 - 父关系。
- 只读属性（例如 caption 和 externalID）不会被复制。

要替换对象：

**步骤**

1. 选择已过时的 Part、Mfg 或 Resource。
2. 点击 Replace Objects 图标中的箭头，并选择 **Replace Parts**、**Replace Mfgs** 或 **Replace Resources**。此时出现相应的 Replace Parts、Replace Mfgs 或 Replace Resources 对话框。
3. 要修改对话框中的列表，点击相应按钮并选择 **Include descendents（包含后代）** 或 **Without descendents（不含后代）**。所选对象的所有有效源对象都会添加到 Source objects（源对象）列表中。如果选择了 Include descendents，则子对象及其所有后代也会添加；如果选择了 Without descendents，则省略子对象。

**注意**：系统会移除重复的源对象。

4. 如有必要，在 Source objects 列表中选择源对象并点击相应按钮将其移除。
5. 从 **Target search scope（目标搜索范围）** 中，选择一个用于搜索匹配目标（包含您希望使用的替换对象）的范围，方式如下：
   - 从 Navigation Tree（导航树）中选择一个复合对象；或
   - 点击相应按钮。此时出现 Select Target Scope（选择目标范围）对话框，选择一个复合对象并点击 **OK**。

**注意**：对于 Replace Parts 和 Replace Resources，目标范围必须是 Collection 或 CompoundType 类型；对于 Replace Mfgs，必须是 Collection 类型。

6. 点击相应按钮，在目标范围中搜索匹配的替换对象。搜索结果填充到 Matching targets（匹配目标）列。

**注意**

- 搜索算法在目标范围中搜索，并为 Replace Parts 对话框中的每个源对象定位匹配的目标对象。
- 符合 `<system root>\General\ReplaceObjectCnf.xml` 中定义的规则的对象被视为匹配。
- 以下是 `PmToolInstance` 类类型规则的一个示例：

```xml
<RuleForClass ClassName="PmToolInstance" Formula="F1">
  <FieldPaths>
    <RuleFields FieldType="attribute" FieldName="name" FormulaId="F1"/>
  </FieldPaths>
</RuleForClass>
```

- 系统提供默认的 `ReplaceObjectCnf.xml`。您可以根据需求编辑此文件。如果定义了无效属性或文件缺失，Replace Objects 命令会返回错误。
- 对象不能同时作为源对象和目标对象，且源对象不能被重复。
- 系统用相应图标标记重复的目标对象。
- 仅当 Source objects 列表已填充且已定义目标范围时，搜索图标才可用。

搜索结果可能是以下之一：

- **无匹配（No matches）** —— 不显示任何匹配。
- **单匹配（Single match）** —— 显示匹配对象的名称。
- **多匹配（Multiple matches）** —— 显示 Multiple matches 以及 Customize（自定义）按钮。点击该按钮打开 Multiple Targets（多个目标）对话框。选择一个目标并点击 **OK**，该目标会显示在 Matching targets 列中，取代 Multiple matches。

- 对于 Source objects 中的每个对象，勾选或清除 V 列中相应的复选框，以启用用其 Matching targets 替换源对象。您也可以使用 **Select All（全选）** 和 **Clear All（清除全部）** 按钮。

**注意**：V 复选框仅对有效的源对象与匹配目标对可用。

- 可选：选择任意源对象或目标对象并点击相应按钮，以在 Navigation Tree 中定位它。
- 可选：配置 Replace Objects Options（替换对象选项）并重复步骤 6。
- 点击 **Replace（替换）**。系统检出（check out）所有必需的源对象和目标对象，并执行替换操作。如果这些对象或任何其他所需对象已被其他用户检出，则整个 Replace Objects 操作失败。如果 Replace Objects 成功地将所有选定源对象替换为相应目标对象，命令对话框关闭并显示成功消息。

点击 **Yes** 查看报告日志。报告存储在 `C:\Documents and Settings\Administrator\Local Settings\Temp`，可能的文件名如下：`ReplacePartCmd.log`、`ReplaceMfgsCmd.log` 或 `ReplaceResourcesCmd.log`。日志记录了命令名称、命令选项、待替换的对以及结果。

如果 Replace Objects 未能成功替换任何选定的源与目标对，Error Viewer（错误浏览器）会显示失败信息。

<a id="v9-s14"></a>
### 替换对象选项（Replace Objects Options）

<!-- p1089 -->

要配置 Replace Objects 命令的选项：

**步骤**

1. 在 Replace Parts、Replace Mfgs 或 Replace Resources 中，点击相应按钮。此时出现 Replace Parts Options、Replace Mfgs Options 或 Replace Resources Options 对话框。

2. 配置以下选项：

| 选项（Option） | 描述（Description） |
| --- | --- |
| **Include Compound Parts（包含复合零件）** | 设置后，系统允许 `CompoundPart` 类型的对象作为源和目标，并在 Replace Parts 对话框中显示它们。清除该选项时，仅允许作为最终项（end items）的复合零件。仅与 Replace Parts 相关。 |
| **Include Compound Resources（包含复合资源）** | 设置后，系统允许 `CompoundResource` 类型的对象作为源和目标，并在 Replace Resources 对话框中显示它们。仅与 Replace Resources 相关。 |
| **Consider locations with offset（考虑带偏移的位置）** | 设置后，如果两个对象之间的距离小于或等于指定的偏移量，系统认为二者匹配。配置文件必须包含位置规则。清除该选项时，数值控件被禁用。 |
| **Keep existing connections to target objects（保留到目标对象的现有连接）** | 设置后，系统将关系从源对象复制到目标对象（它们保留在源对象上），且目标对象保留其在替换前已有的连接。清除该选项时，关系从源对象移除并移动到目标对象。 |
| **Keep existing connections to source objects（保留到源对象的现有连接）** | 设置后，系统将关系从源对象复制到目标对象（它们保留在源对象上）。清除该选项时，关系从源对象移除并移动到目标对象。 |
| **Copy specified attributes（复制指定属性）** | 使您能够指定要从源对象传递到目标对象的属性。设置该选项时，相应按钮变为可用。请参阅 Replace Objects - Specifying Attributes（替换对象 - 指定属性）。 |
| **Automatically Check-In sources and targets（自动签入源与目标）** | 设置后，系统自动签回（check in）其曾自动检出的对象。 |

3. 点击 **OK**。

<a id="v9-s15"></a>
### 替换对象 - 指定属性（Replace Objects - Specifying Attributes）

<!-- p1092 -->

要指定 Replace Objects 命令的属性：

**步骤**

1. 在 Replace Part Settings、Replace Mfg Settings 或 Replace Resource Settings 中，点击相应按钮。此时出现 Replace Parts - Properties Customization、Replace Mfgs - Properties Customization 或 Replace Resources - Properties Customization 对话框。

**注意**：对象类型会根据相关基类自动配置。

2. 通过以下任一方式选择属性：
   - 在 Available properties（可用属性）窗格中选择属性并点击相应按钮，将其添加到 Show properties in following order（按以下顺序显示属性）窗格；
   - 点击相应按钮，将所有可用属性添加到 Show properties in following order；
   - 在 Show properties in following order 中选择属性并点击相应按钮将其移除；
   - 点击相应按钮，从 Show properties in following order 中移除所有属性。

**注意**：Show properties in following order 中属性的顺序并不重要。

3. 当您已在 Show properties in following order 列表中配置好要传递的属性后，点击 **OK**。

<a id="v9-s16"></a>
### 替换资源原型（Replace Resource Prototype）

<!-- p1094 -->

由于许多制造行业的动态特性，标准更新、现有数据升级或资源数据变更十分常见。Replace Resource Prototype（替换资源原型）命令使用户能够自动更改所选资源实例范围的原型（prototype）。这适用于标准变更、现有数据升级或现有资源数据变更的情况。用户选择若干资源实例和单一资源原型，同时可以选择为所有资源实例定义旋转和平移偏移。Replace 命令相应地移动所有选定的资源实例，并将用户选择的原型设置为所有选定实例的替换原型。

例如，有一个工作台资源原型，其若干实例分布在多个工位中。如果必须增大工作台的尺寸，则各工位中的所有工作台都必须获取修改后的原型的更新后 3D 几何，并且由于尺寸变大，它们的位置和偏移也可能需要改变。

**步骤**

1. 从 Navigation tree、Resource tree 或 Graphic Viewer 中，选择一个或多个资源实例。
2. 选择 **Special Data 选项卡 → Replace 组 → Replace Resource Prototype**。此时出现 Replace Resource Prototype 对话框。

**注意**

- 通常无法通过 Graphic Viewer 或 Resource tree 中的拾取（picking）进行选择，因为一般情况下您正在处理尚未加载的组件。
- 必须至少选择一个资源实例才能打开对话框。

3. 要选择一个替换原型，点击相应按钮。此时出现 Select Resource Prototype（选择资源原型）对话框。
4. 选择一个单一资源原型（可以包含 3D 数据）作为资源实例变更的源。
5. 点击 **OK**（对于非原型的对象，该按钮保持禁用）。

**注意**：该命令仅对直接从 Equipment Prototypes（设备原型）实例化的 Equipment Instances（设备实例）可用。如果 Equipment Instance 是另一个 Equipment Instance 的子级，则该命令被禁用。

您可以：

- 用另一个 Equipment Prototype 替换 Equipment Prototype Occurrence 的 Equipment Prototype。
- 当选择了 Equipment Instance 或 Equipment Prototype Occurrence 对象时，使用 Replace Prototype 命令。
- 用另一个 Equipment Prototype 替换 Equipment Instance 的 Equipment Prototype。
- 在替换其 Equipment Prototype 时更改 Equipment Instance 的位置或旋转。

Replace Resource Prototype 对话框将再次打开，并显示您所选的原型于 Replace with（替换为）字段中。

6. 如有必要，为 **Offset in Position（位置偏移，X、Y、Z 轴偏移）** 和 / 或 **Offset in Orientation（方向偏移，RX、RY、RZ 偏移）** 设置新的偏移量，以相应地移动实例。
7. 可选：设置自动检出与签入，如下所示：
   a. 点击 **Options（选项）**。此时出现 Replace Resource Prototype Options 对话框。
   b. 如果您希望自动检出节点，勾选 **Automatically Check-Out Nodes（自动检出节点）**。此时 Automatically Check-In Nodes（自动签入节点）选项变为可用。
   c. 如果您希望自动签入节点，勾选 **Automatically Check-In Nodes**。系统自动签入那些被自动检出的组件。Automatically Check-In Nodes 仅在激活了 Automatically Check-Out Nodes 时才可用。
   d. 点击 **OK** 关闭 Replace Resource Prototype Options 对话框，返回 Replace Resource Prototype 对话框。
   e. 如果您正在替换 Equipment Instance 的原型，Automatically Check-Out Nodes 会自动检出并签入该 Equipment Instance 及其子级。
8. 点击 **Replace（替换）**。系统尝试将您指定的所有实例设置为所选的新原型。会显示一条消息，指示替换原型是为所有选定实例成功设置、仅为其中部分设置，还是完全未设置。如果某些节点的签入 / 检出状态阻碍了变更，则出现以下消息：

> 这可能在以下情况发生：资源实例已被签入而您未选择 Automatically Check-Out Nodes 选项；或者资源实例已被其他用户检出，无论您是否选择了该选项。

9. 如果您希望对部分资源实例运行替换算法，点击 **OK**。会显示一条消息，指示系统在再次运行算法后成功替换了多少个原型。
10. 如果您希望查看报告，点击 **Yes**。

报告会告知您是否选择了 Automatic Check In 和 / 或 Automatic Check Out，并指示系统尝试设置替换原型的每个实例的成功 / 失败状态。

**注意**：Replace Resource Prototype 命令在临时目录中创建报告文件，并在每次运行时覆盖先前的报告。如果您希望保存现有报告，请在再次运行命令之前将其名称从默认的 `ReplaceResourcePrototype.log` 改掉。

<a id="v9-s17"></a>
## 变体（Variants）

<!-- p1098 -->

<a id="v9-s18"></a>
### 工艺 / 产品变更（Process/Product Variations）

<!-- p1098 -->

变体（Variants）模块使用户能够基于产品变更来设计工艺变更（process variations）。该模块使用户能够定义一组由若干标准（criteria）和选项（options）指定的配置。您可以将适当的配置分配给相关对象，然后按照所需配置筛选数据。此外，您可以指定约束，以防止出现不适用的变体组合（例如 convertible 汽车上的车顶行李架），或强制执行产品策略（例如任何豪华车型都必须配备高级音响设备）。

变体配置针对每个 eMServer 项目是特定的，它作为变体表达式（variant expressions，表现为逻辑表达式）的基础，您可以为项目中的所有对象定义这些表达式。逻辑表达式可以包含任意数量的项，每一项指定所需产品的一个条件。分配给某个对象（例如零件、资源或操作）的变体表达式称为变体集（variant set）。一个变体集可以分配给多个对象。

应用于项目视图的变体表达式称为变体过滤器（variant filter）：视图按照所分配的变体表达式进行筛选。使用变体过滤器的视图不会显示分配了与该过滤器不一致的变体集的对象。例如，指定“两门汽车”的变体过滤器的视图，不会显示分配了指定“四门汽车”的变体集的对象。但请注意，指定“四门汽车或两门汽车”的变体集被视为与该变体过滤器一致，分配了此类变体集的对象不会被筛选掉。

变体集和变体过滤器可以存储在变体集库（variant set libraries）和变体过滤器库（variant filter libraries）中，并可以修改、复用或删除。有关变体集和变体过滤器的更多信息，请参阅 Variant Sets and Variant Filters。

一组标准、标准值以及配置约束在每个项目的变体定制（Variant Customization）中定义。您可以在一个 eMServer 项目与另一个项目之间导出和导入变体定制文件，从而加速能够同时容纳同一产品线中多种变更的制造过程的开发。

<a id="v9-s19"></a>
### 概念与定义（Concepts and Definitions）

<!-- p1099 -->

本节对概念与定义进行了比上文更深入的技术性说明。

变体标准（variant criteria）是定义变体的主要参数。每个变体标准都有一个名称（例如 roof-type 或 number-of-doors）以及一组有限的可能取值。汽车变体标准的示例包括 roof-type（regular、convertible）和 number-of-doors（2、3、4）。

变体标准可以是基本的（basic）或可选的（optional）。基本标准必须在类型定义中定义（即它对配置的界定是必不可少的），而可选标准不是配置的必需组成部分。基本标准的一个示例是 engine-type，umbrella-heater 可能是一个可选标准。标准是否为基本或可选是在定制中定义的，并用于类型列表（Types List，详见下文）。

变体表达式（variant expression，或称配置）是一种通过逻辑运算符将变体标准进行逻辑组合的形式语言表达式。对于汽车，一个配置示例为 `audio (tape) && doors (2)`。在此示例中，“&&”是 AND 运算符的符号（“||”是 OR 运算符的符号；“!”是 NOT 运算符的符号）。

规则列表（Rules List）是产品的所有强制组合（mandatory combinations）的列表（例如，豪华车型必须配备高级音响设备）。不适用列表（Inapplicable List）是不允许用于产品的配置列表（例如，convertible 汽车上的车顶行李架）。类型列表（Types List）是每个基本标准取一个标准值的组合列表。类型列表中的项可以是正向要求（positive requirement）或负向要求（negative requirement）。规则列表、不适用列表和类型列表合称为背景信息（background information），将在 Variant Customization 中进一步说明。

变体类型（variant type）通过为各标准提供特定取值来描述产品的配置；它是经过命名并保存的基本配置。变体集由一个变体表达式组成，用于标注节点（对象）。公共变体集（public variant set）是保存在变体集库中、可由多个对象共享的变体集。私有变体集（private variant set）属于特定对象，只能由分配给它的对象访问。变量类型或公共变体集可用于其他变量表达式。

变体过滤器描述一个特定的变体表达式，用于筛选已分配变体集的数据（对象），使只有所需的节点 / 对象保持可见和可访问。

<a id="v9-s20"></a>
### 变体集与变体过滤器（Variant Sets and Variant Filters）

<!-- p1099 -->

变体集和变体过滤器由经过语法、一致性及其他约束检查的变体逻辑表达式组成（参见 Variants）。集和过滤器都是使用同一个变体集编辑器（Variant Set Editor）创建的。

变体集和变体过滤器的用途不同但互补，如下所述：

- **变体集**描述对象在 eMServer 中适用的变体组合；即需要、允许或禁止哪些标准、类型和其它变体集的组合。变体集分配给诸如零件、资源和操作等对象。一个变体集可以分配给多个对象。公共变体集是保存在变体集库中、可供多个对象使用的变体集。私有变体集属于特定对象，只能由分配给它的对象访问。
- **变体过滤器**描述应用于项目视图、用于筛选数据的特定变体表达式（或配置），使只有所需的节点 / 对象保持可见和可访问。整个项目只能使用一个变体过滤器。更多信息请参阅 Variant Filtering。

<a id="v9-s21"></a>
### 支持变体集的对象（Objects Supporting Variant Sets）

<!-- p1100 -->

以下 eMServer 对象支持变体集：

- Flow（流）
- Manufacturing Features（制造特征）
- Operation（操作）
- Part（零件）
- PartPrototype（零件原型）
- Resource（资源）
- ToolPrototype（工具原型）
- Usage（MfGUsage、PLPUsage、ToolPrototypeUsage、PartPrototypeUsage、PmUsage）

<a id="v9-s22"></a>
### 变体集编辑器（Variant Set Editor）

<!-- p1100 -->

<a id="v9-s23"></a>
#### 创建 / 编辑变体集表达式（Creating/Editing Variant Set Expressions）

<!-- p1100 -->

变体集编辑器可用于：

- 为变体集或变体过滤器创建新的变体表达式；
- 编辑现有的变体集表达式。

<a id="v9-s24"></a>
#### 打开变体集编辑器（Opening the Variant Set Editor）

<!-- p1100 -->

选择 **Special Data 选项卡 → Variants 组 → Variant Editor**。变体集编辑器打开。

<a id="v9-s25"></a>
#### 选择显示选项（Selecting Display Options）

<!-- p1101 -->

变体集编辑器打开时，顶部窗格中显示的信息取决于打开编辑器所用的方法：

- 通过点击变体集打开编辑器时，显示所有标准（基本和可选）。
- 通过点击对象打开编辑器时，显示所有变体集库。

显示选项指示在变体集编辑器对话框的左上角。在下图中它显示为 Variant Sets。该显示选项右侧有一个箭头。点击该箭头会打开一个带四个选项的下拉列表。

所选项决定变体集编辑器上部窗格中显示的内容：

- **all Criteria（所有标准）**：列出本项目的基本标准和可选标准。所显示的标准可用于构建变体表达式。

**注意**：显示标准时（选择 all Criteria 或 Basic Criteria），排序图标可按字母顺序排序显示，便于快速定位变体集。

- **Basic Criteria（基本标准）**：仅列出本项目的基本标准。所显示的标准可用于构建变体表达式。
- **Variant Sets（变体集）**：列出本项目的变体集库。
- **Variant Types（变体类型）**：列出变体类型及每种类型的表达式。库列表可展开，以按名称和变体表达式显示给定库中的变体集。所列变体集可用于构建变体表达式。

<a id="v9-s26"></a>
#### 变体集编辑器元素说明（Description of Elements of Variant Set Editor）

<!-- p1102 -->

在最左侧一列对标准进行编号。第二列指示每个标准是基本标准（B）还是可选标准（O）。第三列列出标准名称。第四列（Value1）及之后各列包含该标准的可能取值。列出所有取值后，其余列为空。

双击列标题可对 Criterion 列和 Value 列排序。

当单元格包含显示名称时，将鼠标移到单元格上会显示带有相应 caption 的工具提示。

点击相应按钮可将 Editing Variant Set（编辑变体集）对话框展开，提供用于编辑表达式的更多细节和选项。相关信息请参阅 Details。

变体集编辑器的一般行为如下：

- 在上部窗格中，通过点击或导航来选择某项。要选择多项（针对一个标准或多个标准），使用标准 Windows 约定（按 Ctrl、Shift 和方向键，或使用鼠标按钮）。您可以按标准 Windows 方式在窗格中导航，但 Home 和 End 键没有导航功能。

您可以点击相应按钮打开 Find（查找）对话框，按标准名称搜索列表。

在选择时按 Shift 键可包含前一个选择与当前选择之间的所有取值。请谨慎使用此功能，因为包含“所有取值”的参数并不总是显而易见，并可能导致不期望的结果。按 Ctrl 加左箭头键可选择当前行的所有取值。

双击一个术语，然后双击另一个（不同标准的）术语，会默认自动创建一个带有 AND 运算符的表达式。您可以通过选择该运算符，然后点击不同运算符（例如 OR）的按钮来更改运算符。

要通过双击添加多项，按住 Ctrl，逐个点击选择除最后一项外的所有项，然后双击最后一项。添加多个标准时，会在每对添加的项之间自动放置一个运算符。选择多个标准时，它们之间放置 AND 运算符；选择多个变体集时，它们之间放置 OR 运算符。双击变体集会在此后连续选择的每个变体集之间插入 OR 表达式。

- 在下部窗格中，按钮操作（如 Add、OR 等）通常作用于变体表达式中当前光标位置的右侧。使用鼠标或方向键将光标移动到所需位置。

唯一作用于光标位置左侧的按钮操作是：向光标左侧同一标准添加一个或多个取值。例如，如果变体表达式中光标左侧的标准是 `DOORS (2,4)`，而您从上部窗格选择并添加 DOORS 的取值 3 和 5，则新取值与旧取值合并，变体表达式中结果为 `DOORS (2,3,4,5)`。请注意，这种新旧取值的合并仅在您从上部窗格的单个行（标准）选择取值时发生。

通过点击选择某个术语或逻辑表达式。下部窗格中所选的任何术语或逻辑表达式，在您从上部窗格添加术语（或多个术语），或点击 AND 或 OR 时会被替换。在术语前插入逻辑非（通过使用 not 命令）或在逻辑表达式中展开变体集（通过使用 Expand 命令）的唯一方式是先选择它。

语法错误由一个或多个显示为红色的术语指示。

<a id="v9-s27"></a>
#### 变体集编辑器命令按钮（Variant Set Editor command buttons）

<!-- p1105 -->

变体集编辑器中出现以下按钮。

| 按钮（Button） | 描述（Description） |
| --- | --- |
| **Add（添加）** | 添加在上部窗格中所选的术语，如下：<br>• 如果选择一个标准名称，添加的项为 `ANY(CRITERION_NAME)()`。例如，选择 ENG 会高亮该标准的整行，点击 Add 会添加术语 `ANY(ENG)()`。此行为与 Any 命令相同。<br>• 如果为同一标准选择一个或多个取值，添加的项为 `ONEOF(CRITERION_NAME)(VALUE_a,VALUE_b,...)`。例如，为 ENG 选择取值 1600 和 2200，点击 Add 会添加术语 `ONEOF(ENG)(1600,2200)`。此行为与 One of 命令相同。<br>• 选择时按 Ctrl，可从多行选择取值。所添加术语若为标准是逻辑 AND 连接，若为变体集则是逻辑 OR 连接。 |
| **Remove（移除）** | 从表达式中移除光标右侧的任何项（术语或逻辑运算符）。注意：执行移除前不发出警告。也可按 Delete 移除光标右侧的项，或按 Backspace 移除光标左侧的项。 |
| **And（与）** | 插入逻辑 AND，即两个标准及 / 或现有变体集表达式的合取（交集）。在变体表达式中，AND 表示为 `&&`。 |
| **Or（或）** | 插入逻辑 OR，即两个标准及 / 或现有变体集表达式的析取（并集）。在变体表达式中，OR 表示为 `||`。 |
| **Not（非）** | 插入逻辑非，即对单个标准或现有变体集表达式的否定。要使此命令按钮可用，需在当前变体表达式中选择一个术语（您要否定的那个）。在变体表达式中，not 表示为 `!`。 |
| **Any（任意）** | 如果选择一个标准并点击此按钮，会添加形如 `ANY(CRITERION_NAME)()` 的术语。例如，选择 ENG 会高亮该标准的整行，点击 Add 会添加术语 `ANY(ENG)()`。 |
| **One of（其中之一）** | 如果为同一标准选择一个或多个取值，添加的项为 `ONEOF(CRITERION_NAME)(VALUE1,VALUE2,...)`。例如，为 ENG 选择取值 1600 和 2200，点击 Add 会添加术语 `ONEOF(ENG)(1600,2200)`。要选择多个术语，按 Ctrl、Shift、方向键和 / 或鼠标键，采用标准 Windows 方式。 |
| **Expand（展开）** | 展开变体集表达式。要使此按钮可用，需在当前的变体表达式中选择要展开的变体集表达式。现有变体集表达式只有在变体集编辑器上部窗格选择显示 Variants Sets 时才能引入当前变体表达式。但无论上部窗格当前显示什么，Expand 按钮都可使用。 |
| **Check（检查）** | 检查变体表达式的有效性（一致性及是否符合约束）。此命令也测试语法错误，尽管通常更简单快捷的方式是查看 OK 命令或 Save as Filter 命令是否可用——这些命令仅在无语法错误时可用。相关信息请参阅 Checking Variant Expressions。 |
| **Clear（清除）** | 删除整个变体表达式。执行删除前会出现警告消息。 |
| **Probability（概率）** | 打开一个窗口，可将变体集的概率设置为 0.00 到 1.00 之间的值。概率仅与变体集相关，且仅用于负载均衡（load-balancing）。 |
| **Save as Filter（另存为过滤器）** | 用于将变体表达式保存为变体过滤器。注意，如果变体表达式包含语法错误，此命令不可用。有关如何保存变体过滤器的更多信息，请参阅 Saving an Expression as a Variant Filter。 |
| **Properties（属性）** | 打开对象的属性选项卡。 |
| **Details（细节）** | 显示对创建表达式有用的附加信息：<br>• Show fully expanded expression（显示完全展开的表达式）——选择此选项会显示每个级别完全展开的表达式。<br>• Show Matching Types pane（显示匹配类型窗格）——显示一个包含匹配表达式的变体类型的表。对每种匹配变体类型，表显示名称和表达式。Match Indicated values only 和 Use Variant Rules 选项使您能够控制匹配类型的显示。<br>• Find（查找）——打开 Find 对话框，便于搜索匹配变体类型的表。 |
| **Apply（应用）** | 用于将变体表达式保存为变体集。注意，如果变体表达式包含语法错误，此命令不可用。相关信息请参阅 Saving an Expression as a Variant Set。 |
| **Close（关闭）** | 关闭变体集编辑器；任何未保存的内容都会丢失。关闭前会出现警告消息。 |

<a id="v9-s28"></a>
#### 构建变体表达式（Building Variant Expressions）

<!-- p1107 -->

变体表达式由变体集编辑器上部窗格中显示的标准和 / 或现有变体集表达式构建。对于同一变体表达式，您可以在上部窗格中切换显示 all Criteria、Basic Criteria 和 Variant Sets。

这些标准和表达式必须由逻辑运算符 AND 或 OR 分隔，通过点击 AND 和 OR 命令（位于上部与下部窗格之间的分隔区）插入。

使用变体集编辑器构建变体表达式的典型顺序如下：

**步骤**

1. 从变体集编辑器的上部窗格中选择一个术语，并点击 **Add**。该术语被添加到变体表达式，如“变体集编辑器元素说明”中所述。
2. 点击 **AND** 或 **OR**。根据选择插入 `&&`（表示 AND）或 `||`（表示 OR）。由于此逻辑运算符后面暂时没有第二个操作数，语法当前不正确：运算符显示为红色，OK 和 Save as Filter 命令不可用。
3. 重复步骤 1。

**注意**：当插入此术语时，前一个逻辑运算符变为蓝色，表示变体表达式的语法现已正确。如果两个术语之间没有逻辑运算符（AND 或 OR）分隔，第二项显示为红色，表示语法错误。

4. 根据需要重复步骤 2 和 3，直到变体表达式完成。也可以按需使用 NOT 逻辑运算符。您可以通过移动变体表达式中的光标位置来移动术语和逻辑运算符的插入点。您也可以移除术语或展开已插入的表达式，如“变体集编辑器元素说明”中所述。

<a id="v9-s29"></a>
#### 检查变体表达式（Checking Variant Expressions）

<!-- p1108 -->

在构建变体表达式的任何时候，您都可以点击 **Check**，根据规则、不适用列表和类型列表（Rules, Inapplicable, and Types Lists）检查其是否符合约束及一致性。注意，当您点击 OK 或 Save as Filter 时，系统会自动调用 Check 命令。

点击 Check 时，会显示以下消息之一：

- 如果变体表达式没有语法错误且有效：如果通过点击 OK 或 Save as Filter 调用 Check 命令，系统不显示此消息。
- 如果变体表达式有语法错误，则不检查其有效性，系统显示以下消息：

**注意**：如果变体表达式包含语法错误，您无法调用 Check 命令，因为 OK 和 Save as Filter 不可用。

- 如果变体表达式没有语法错误但仍无效（由于不一致或违反约束），系统显示以下消息：

如果通过点击 OK 或 Save as Filter 调用 Check 命令遇到无效变体表达式，系统也会显示此错误消息。您可以右键点击错误描述，打开指示所违反规则的变体约束（Variant constraints）。

<a id="v9-s30"></a>
#### 保存变体表达式（Saving Variant Expressions）

<!-- p1109 -->

一旦构建了语法正确的变体表达式，您可以将其保存为变体集、变体过滤器，或两者（这种情况下，重要的是先将表达式保存为变体过滤器）。保存时系统会自动检查变体表达式的有效性。更多信息请参阅 Saving an Expression as a Variant Set 和 Saving an Expression as a Variant Filter。

<a id="v9-s31"></a>
#### 将表达式保存为变体集（Saving an Expression as a Variant Set）

<!-- p1110 -->

要将变体表达式保存为变体集：

**步骤**

1. 在变体集编辑器中，点击 **OK**。系统自动调用 Check 命令，测试变体表达式的有效性（一致性和是否符合约束）。如果变体表达式未通过有效性测试，系统显示失败消息，如“检查变体表达式”中所示。如果通过，则打开 Saving Options（保存选项）对话框。
2. 选择将变体表达式保存为公共变体集（public Variant Set）还是私有地保存在所选对象上（Privately on the selected object）。默认保存为公共集。
   - 如果保存为私有变体集，则只能由为其创建（并分配）的对象访问。
   - 如果保存为公共变体集，该集可被其它表达式访问、分配给其它对象，并保存为变体过滤器。此外，您可以通过勾选“Search for equivalent Variant Sets in the libraries（在库中搜索等效变体集）”复选框，搜索与当前变体表达式逻辑等效的现有公共变体集。
3. 点击 **OK**。

如果选择保存为私有变体集，系统保存该变体集并关闭 Saving Options 对话框和变体集编辑器。

如果选择保存为公共变体集，后续操作取决于是否选择搜索等效变体集：

- 如果未选择搜索等效变体集，则打开 Save Variant Set to Library（将变体集保存到库）对话框（如下述步骤 4）。
- 如果选择搜索等效变体集，系统执行搜索。若未找到等效变体集，则自动打开 Save Variant Set to Library 对话框（如下述步骤 4）。若找到等效变体集，则打开 Equivalent Variant Sets（等效变体集）对话框，显示所有与当前变体表达式匹配的公共变体集。通常，最好使用现有公共变体集（并忽略当前变体表达式）。为此，在 Equivalent Variant Sets 对话框中点击 **OK**。该对话框和变体集编辑器关闭，当前变体表达式丢失。如果您希望保存当前变体表达式（即使已存在等效变体集），在 Equivalent Variant Sets 对话框中点击 **Cancel**。该对话框关闭，并打开 Save Variant Set to Library 对话框（如下一步）。

4. Save Variant Set to Library 对话框（参见下图）可能因上述若干命令序列之一而打开。有时该对话框打开时，Caption 列列出所有变体集库的名称。其它时候，它打开到上次用于保存变体集的库，Caption 列显示该库中所有变体集的名称。要显示所有变体集库的名称，点击 Up（向上）图标。
5. 选择要将变体集保存到的库。
   - 如果您希望保存到的库当前已打开，继续下面的步骤 6。
   - 要选择变体集库，在 Save Variant Set to Library 对话框的 Caption 列中点击它，并点击 **Open**（如果当前打开的是不同的库，点击 Up 图标上移一级）。Open 命令变为 Save，Caption 列显示所选变体集库中所有变体集的名称，Name 字段变为可编辑。Name 字段显示添加到此变体集库的最后一个变体集的名称。
   - 要创建新的变体集库，点击 Create New Library（创建新库）图标，并以标准 Windows 方式创建新库。在 Save Variant Set to Library 对话框中，Open 命令变为 Save，Caption 列变为空（新库不含变体集），Name 字段变为可编辑。
6. 在 Name 字段中，为新的变体集键入名称。强烈建议在新变体集库内选择唯一的名称。但是，如果您选择使用现有变体集的名称，新变体集不会覆盖同名的旧变体集；相反，两个变体集具有相同的名称。
7. 点击 **Save**。系统关闭 Save Variant Set to Library 对话框和变体集编辑器。

<a id="v9-s32"></a>
#### 将表达式保存为变体过滤器（Saving an Expression as a Variant Filter）

<!-- p1112 -->

要将变体表达式保存为变体过滤器：

**步骤**

1. 在变体集编辑器中，点击 **Save as Filter**。Save as Filter 命令首先自动检查以测试变体表达式的有效性（一致性和是否符合约束）。如果未通过，显示失败消息，如“检查变体表达式”中所示。如果通过，则打开 Save Variant Filter to Library 对话框。
2. 选择要将变体过滤器保存到的库。
   - 如果您希望保存到的库当前已打开，继续下面的步骤 3。
   - 要选择变体过滤器库，在 Save Variant Filter to Library 对话框的 Caption 列中点击该变体过滤器库，然后点击 **Open**（如果当前打开的是不同的库，点击 Up 图标上移一级）。Open 命令变为 Save，Caption 列显示所选变体过滤器库中所有变体过滤器的名称，Name 字段变为可编辑。Name 字段显示添加到此变体过滤器库的最后一个变体过滤器的名称。
   - 要创建新的变体过滤器库，点击 Create New Library 图标，并以标准 Windows 方式创建新库。在 Save Variant Filter to Library 对话框中，Open 命令变为 Save，Caption 列变为空（新库不含变体过滤器），Name 字段变为可编辑。
3. 在 Name 字段中，为新的变体过滤器键入名称。强烈建议在该变体集库内选择唯一的名称。但是，如果您确实选择使用现有变体过滤器的名称，新变体过滤器不会覆盖同名的旧变体过滤器；相反，两个变体过滤器具有相同的名称。
4. 执行以下任一操作以保存变体过滤器：
   - 点击 **Save**，使用 Editing Variant Set 对话框中指定的变体集表达式保存为过滤器。
   - 点击 **Save as Reference**，使用对变体集的引用（链接）而非变体集表达式本身保存为过滤器。对于保存为引用的变体过滤器，更改变体集中的表达式会自动更新该过滤器。

Save Variant Filter to Library 对话框关闭，您返回变体集编辑器。

<a id="v9-s33"></a>
#### 变体集库与变体过滤器库（Variant Set Library and Variant Filter Library）

<!-- p1114 -->

要创建变体集库或变体过滤器库：

**步骤**

1. 右键点击项目对象或任意文件夹对象。
2. 在打开的上下文菜单中，点击 **New**。打开 New 对话框。
3. 在 New 对话框中，向下滚动到 VariantFilterLibrary 和 / 或 VariantSetLibrary。
4. 勾选 VariantFilterLibrary 和 / 或 VariantSetLibrary 的 Node 复选框。确保 Amount 列显示“1”；否则，您应键入它。但是，如果您需要多个 VariantFilterLibrary 和 / 或 VariantSetLibrary，可以为每项键入更大的数字（两个数字可以不同）。
5. 点击 **OK**。Navigation tree 会填充您指定数量的 VariantFilterLibrary 和 / 或 VariantSetLibrary。

<a id="v9-s34"></a>
#### 编辑现有变体集与变体过滤器（Editing Existing Variant Sets and Variant Filters）

<!-- p1114 -->

<a id="v9-s35"></a>
##### 编辑现有变体集（Editing an Existing Variant Set）

<!-- p1114 -->

要编辑现有变体集，在窗口或树中双击该变体集。变体集编辑器打开，显示现有的变体集作为变体表达式。您可以执行“变体集编辑器”及其后续小节中描述的所有操作。

<a id="v9-s36"></a>
##### 编辑现有变体过滤器（Editing an Existing Variant Filter）

<!-- p1115 -->

要编辑现有变体过滤器，执行以下任一操作：

- 在窗口或树中双击该变体过滤器。
- 选择一个变体过滤器，并选择 **Special Data 选项卡 → Variants 组 → Variant Editor**。

变体过滤器编辑器（Variant Filter Editor）打开，显示所选变体过滤器作为变体表达式。

变体过滤器编辑器在外观和功能上都非常类似于变体集编辑器，区别如下：

- 点击 **OK** 将变体表达式保存为更新后的变体过滤器（无法从变体过滤器创建变体集）。
- 点击 **Save Filter** 打开一个对话框（参见 Saving an Expression as a Variant Filter）以保存新的变体过滤器。
- 变体过滤器编辑器的左下角显示 **Filter Properties** 命令（取代仅与变体集相关的变体集编辑器的 Probability 命令）。

<a id="v9-s37"></a>
##### 使用过滤器属性对话框（Using the Filter Properties dialog box）

<!-- p1115 -->

点击 Filter Properties（位于变体过滤器编辑器左下角）会打开 Filter Properties 对话框：

Filter Properties 对话框中各选项的效果如下：

- **Match indicated values only（仅匹配指定的取值，对可选标准有效）**

此选项使您能够筛选掉（分配了指定）您当前不感兴趣的可选标准的变体集的对象。默认情况下此选项被清除。

如果选择此选项，则每当应用此过滤器时，任何分配了包含未由变体过滤器指定的可选标准（无论取值如何）的变体集的对象都会被筛选掉。此选项适用于未指定所有可选标准的任何过滤器。

例如，假设 `audio (tape, CD, none)` 是一个可选标准，electrical-system 对象分配了一个包含该可选 audio 标准的变体集，而应用于视图的变体过滤器未指定 audio 标准。

- 如果未选择此选项（Match indicated values only [对可选标准有效]），则 electrical-system 对象显示在视图中：指定 audio 选项的变体集与未指定 audio 选项的变体过滤器是一致的。
- 如果选择此选项，则无论该对象的变体集为 audio 指定什么取值，electrical-system 对象都会从视图中被筛选掉，因为变体过滤器不包含可选的 audio 标准。

- **Use Variant Rules（使用变体规则）**

此选项使您能够将项目的定制应用于筛选过程。默认情况下此选项被选中。

如果选择此选项，则每当应用此过滤器时，也会应用背景信息（Rules、Inapplicable 和 Types 列表）。更多信息请参阅 Background Information: Rules, Inapplicable, and Types Lists。

例如，假设变体过滤器指定分销到日本（distribution is to Japan），背景规则指定如果分销到日本则汽车必须为右舵（right-hand drive），并且该过滤器应用于包含分配了指定左舵（left-hand drive）的变体集的 steering-wheel 对象的视图。

- 如果未选择此选项（Use Variant Rules），则 steering-wheel 对象显示在视图中：变体集（左舵）与变体过滤器（分销到日本）是一致的。
- 如果选择此选项，则 steering-wheel 对象被筛选掉：当考虑背景规则（分销到日本仅意味着右舵）时，变体集（左舵）与变体过滤器（分销到日本）不一致。

<a id="v9-s38"></a>
### 删除变体集与变体过滤器（Deleting Variant Sets and Variant Filters）

<!-- p1117 -->

只有当变体集未分配给任何对象时，您才能删除它。只有当变体过滤器未应用于任何视图时，您才能删除它。

要从库中删除变体集或变体过滤器，右键点击它，并选择 Delete 选项。会打开一个对话框，询问是否删除该对象——点击 **Yes**。

<a id="v9-s39"></a>
### 变体过滤（Variant Filtering）

<!-- p1117 -->

<a id="v9-s40"></a>
### 分配变体集与过滤器的一般效果（General Effects of Assigning Variant Sets and Filters）

<!-- p1117 -->

变体过滤仅影响那些分配了与所应用过滤器不一致的变体集的对象。例如，对象的变体集仅指定右舵，但变体过滤器仅指定左舵。

下图显示了应用变体过滤器之前的树。指定左舵和右舵的对象都会显示。

应用指定仅左舵的过滤器后，窗口外观变为下图所示：

注意，上一个窗口中的一个对象（Dashboard，分配了仅指定右舵的变体集 S_RH）已被筛选掉，不再可见。但是，变体集本身（S_RH）未被筛选掉。

无论先为对象分配变体集再为视图分配变体过滤器，还是以相反顺序分配，变体过滤的效果都是相同的。

**注意**：必须打开 Show/Hide Columns（显示 / 隐藏列），才能显示过滤后包含变更的 Variant Set 列。

<a id="v9-s41"></a>
### 过滤器属性对话框中选项的效果（Effects of Options in Filter Properties Dialog）

<!-- p1120 -->

您在 Filter Properties 对话框中对以下选项（参见 Using the Filter Properties Dialog）的选择会显著影响过滤的执行方式。

- **Consider Non-Specified Optional Criteria as Inconsistent（将未指定的可选标准视为不一致）**

此选项使您能够筛选掉分配了您当前不感兴趣的可选标准的变体集的对象。默认情况下此选项被清除。

如果选择此选项，则每当应用此过滤器时，任何分配了包含未由变体过滤器指定的可选标准（无论取值如何）的变体集的对象都会被筛选掉。此选项适用于未指定所有可选标准的任何过滤器。

例如，假设 `audio (tape, CD, none)` 是一个可选标准，electrical-system 对象分配了一个包含该可选 audio 标准的变体集，而应用于视图的变体过滤器未指定 audio 标准。

- 如果未选择此选项（Consider Non-Specified Optional Criteria as Inconsistent），则 electrical-system 对象显示在视图中：指定 audio 选项的变体集与未指定 audio 选项的变体过滤器是一致的。
- 如果选择此选项，则无论该对象的变体集为 audio 指定什么取值，electrical-system 对象都会从视图中被筛选掉，因为变体过滤器不包含可选的 audio 标准。

- **Take into Account Background Rules when Filtering（过滤时考虑背景规则）**

此选项使您能够将项目的定制应用于筛选过程。默认情况下此选项被选中。

如果选择此选项，则每当应用此过滤器时，也会应用背景信息（Rules、Inapplicable 和 Types 列表）。更多信息请参阅 Background Information: Rules, Inapplicable, and Types Lists。

例如，假设变体过滤器指定分销到日本，背景规则指定如果分销到日本则汽车必须为右舵，并且该过滤器应用于包含分配了指定左舵的变体集的 steering-wheel 对象的视图。

- 如果未选择此选项（Take into Account Background Rules when Filtering），则 steering-wheel 对象显示在视图中：变体集（左舵）与变体过滤器（分销到日本）是一致的。
- 如果选择此选项，则 steering-wheel 对象被筛选掉：当考虑背景规则（分销到日本仅意味着右舵）时，变体集（左舵）与变体过滤器（分销到日本）不一致。

<a id="v9-s42"></a>
### 分配变体集与过滤器的特定效果（Specific Effects of Assigning Variant Sets and Filters）

<!-- p1121 -->

变体过滤对 Process Designer 的功能有特定影响，如下所述。

<a id="v9-s43"></a>
#### 树视图（Tree Views）

<!-- p1121 -->

当对项目应用变体过滤器时，Navigation、Operation 等树首先折叠。随后，树自动展开到应用变体过滤器时已选中的树对象所在的级别。

<a id="v9-s44"></a>
#### 编辑器（属性选项卡）[Editor (Properties tab)]

<!-- p1121 -->

对象根据所应用的变体过滤器进行筛选。例如，如果某个操作被筛选掉，则仅显示与其过滤器一致的连接资源、零件和制造特征（包括连接对象：flow 和 usage）。

**注意**：如果（其编辑器已打开的）对象仅通过一条被筛选掉的流连接到第二个对象，则即使第二个对象与过滤器一致，它也会被筛选掉。

<a id="v9-s45"></a>
#### 计算时间（Calculated Time）

<!-- p1121 -->

在操作树视图的 Times 选项卡的 Calculated Time 字段中，时间计算根据所应用的变体过滤器执行。

<a id="v9-s46"></a>
#### PERT 视图（Pert View）

<!-- p1121 -->

PERT 视图的筛选针对所有操作、关系（与零件、资源和制造特征）以及流，依据它们所分配的变体集进行。其中一个连接操作被筛选掉的流或 usage 对象以红色显示。两个连接操作都被筛选掉的流或 usage 对象也会被筛选掉。如果某个流对象被筛选掉，两个操作仍会显示。

**注意**：同一两个操作之间可能有多条流。此类多条流以粗线表示。根据您筛选视图和为流对象分配变体集的方式，两个操作之间的“流线”可能变粗（多条流未被筛选掉）或变细（仅一条流未被筛选掉）。

<a id="v9-s47"></a>
#### Gantt 操作视图（Gantt Operation View）

<!-- p1121 -->

Gantt 操作视图的筛选针对所有操作和流，依据它们所分配的变体集进行。任一连接操作被筛选掉的流对象也会被筛选掉。计算出的 Longest Path（最长路径）根据所应用的变体过滤器更新。

**注意**：同一两个操作之间可能有多条流，此时仅显示最先创建的流对象。根据您筛选视图和为流对象分配变体集的方式，两个操作之间的“流线”长度可能改变。

<a id="v9-s48"></a>
#### Gantt 资源视图（Gantt Resource View）

<!-- p1122 -->

Gantt 资源视图的筛选针对所有资源、操作和流，依据它们所分配的变体集进行。任一连接操作被筛选掉的流对象也会被筛选掉。

**注意**：同一两个操作之间可能有多条流，此时仅显示最先创建的流对象。根据您筛选视图和为流对象分配变体集的方式，两个操作之间的“流线”长度可能改变。

<a id="v9-s49"></a>
#### 导出（Export）

<!-- p1122 -->

您可以根据所应用的变体过滤器导出筛选后的数据。所使用的变体过滤器表达式与当前应用于项目的相同。但是，如下图所示，您可以选择不对导出数据应用任何变体过滤器。

<a id="v9-s50"></a>
#### 成本计算（Cost Calculation）

<!-- p1123 -->

操作视图和资源视图中的成本计算均根据应用于项目的变体过滤器执行。分配了与所应用过滤器不一致的变体集的对象不包含在计算中。

<a id="v9-s51"></a>
### 变体定制（Variant Customization）

<!-- p1123 -->

<a id="v9-s52"></a>
#### 项目的变体定制（Variant Customization for Projects）

<!-- p1123 -->

变体定制存储在项目级别，这与存储在数据库级别的通用定制（general customization）不同。

变体定制仅适用于单个项目，尽管变体定制文件可以从一个项目导出并导入到另一个项目。只有管理员可以导入或导出定义变体定制的文件。

<a id="v9-s53"></a>
#### 版本控制（Versioning）

<!-- p1123 -->

变体标准及其可能取值是工艺模型对象，可以像其它工艺模型对象一样进行版本控制。

<a id="v9-s54"></a>
#### 约束与一致性（Constraints and Consistency）

<!-- p1123 -->

除语法正确外，变体表达式还必须针对规则、不适用列表和类型列表（Rules, Inapplicable, and Types Lists）施加的约束和一致性进行检查，如下所述。

您可以通过在变体集编辑器中点击 **Check** 来检查变体表达式。当您点击 OK 或 Save as Filter 时，系统会自动执行此检查。

<a id="v9-s55"></a>
#### 背景信息：规则、不适用列表和类型列表（Background Information: Rules, Inapplicable, and Types Lists）

<!-- p1123 -->

规则、不适用列表和类型列表合称为背景信息，由定制指定。例如，您可以指定约束，以防止物理上不兼容的变体组合（如 convertible 汽车上的车顶行李架），或违反产品策略的组合（如豪华车中的标准收音机）。

<a id="v9-s56"></a>
##### 规则列表（Rules List）

<!-- p1124 -->

规则列表是产品的所有适用基本配置（applicable basic configurations）的列表。例如，假设唯一的基本标准是 number-of-doors 和 body-type，规则列表中的一项可以是 `number-of-doors(2) && body-type(compact)`。

<a id="v9-s57"></a>
##### 不适用列表（Inapplicable List）

<!-- p1124 -->

不适用列表是不允许用于产品的严格配置（strict configurations）的列表。例如，如果 air-conditioner 是可选标准，不适用列表中的一项可能是 `body-type(compact) && air-conditioner(yes)`。

<a id="v9-s58"></a>
##### 类型列表（Types List）

<!-- p1124 -->

类型列表是关系的列表，其中每个关系描述一个严格配置与一个基本变体（basic variant）之间的依赖关系。类型列表中的项可以是正向要求：`body-type(luxury) implies air-conditioner(yes)`；或负向要求：`body-type(compact) implies not 4-wheel-drive(yes)`。类型列表文件指定产品的基本标准和可选标准。

<a id="v9-s59"></a>
#### 定制文件（Customization Files）

<!-- p1124 -->

变体定制文件是 CSV（逗号分隔值）格式、逐行格式（PPV）或 XML 格式的文本文件。这些文件可以在 eMServer 之外使用标准文本编辑器创建，然后导入。PPV 和 XML 格式也可导出。

<a id="v9-s60"></a>
##### 标准列表文件格式（Format of Criteria List File）

<!-- p1124 -->

标准列表文件（Criteria List File）包含三个部分：

- 第一部分（CSV 格式时）由文件的第一行组成，必须如下所示：
  - `Class,externalId,comment,domainId,name,criterionValue,criterion`
  
  这七个逗号分隔的值是文件七列的标题。
- 第二部分由格式类似于以下的行组成：
  - `PmBasicCriterion,BasicDOR,Doors,1,DOR,,`
  - `PmOptionCriterion,OptionAIB,Airbag,1,AIB,,`
  
  第一个术语，即 `PmBasicCriterion` 或 `PmOptionalCriterion`，指定类——该标准是否为基本或可选。第二个术语（如 BasicDOR 或 OptionAIB）指定标准的外部 ID。第三个术语（如 Doors 或 Airbag）是描述该标准的注释。第四个术语 domainId 必须始终为 1。第五个术语（如 DOR 或 AIB）是该标准的名称。第六和第七个术语必须留空。
- 第三部分由格式类似于以下的行组成：
  - `PmStringCriterionValue,CTValueDor2,2,,2,2,BasicDOR`
  - `PmStringCriterionValue,CTValueAib3,3,,3,3,OptionAIB`
  
  第一个术语 `PmStringCriterionValue` 指定正在指定一个标准取值。第二个术语 `CTValue_...` 指定标准取值的外部 ID。第三个术语（如 2 或 3）是描述该标准取值的注释。第四个术语 domainId 必须留空。第五个术语（如 2 或 3）是该标准取值的名称。第六个术语（如 2 或 3）指定标准取值。第七个术语（如 BasicDOR 或 OptionAIB）指定该标准取值适用的是哪个标准。

**注意**：此术语必须引用本文件第二部分中的有效 externalId 值。在第三部分中，每个标准取值必须单独添加一行。

<a id="v9-s61"></a>
##### 规则列表文件格式（Format of Rules List File）

<!-- p1126 -->

规则列表文件（PPV 格式）的一般格式由每个条目的一个头行（header line）加上每个条目七行数据行（data lines）组成。每个数据行必须以分号结尾。

- **头行（Header line）**：标识规则，例如：`PM_UPDATE PmExpressionRule ex_id_Rule_4`，其中 BasicRule 是 External ID，应符合 eMServer ID 规则。
- **name 数据行**：描述规则，例如：`name = "3 Airbags are available only with 4 Door configuration";`
- **comment 数据行**：可存在，例如：`Comment = "4 doors";`
- **domainId 数据行**：必须包含值 1：`domainId = 1;`
- **expression 数据行**：为此规则指定的变体表达式，例如：`expression = "ONEOF(Basic_DRV)(CT_Value_DRV_4WD)~";`
- **entailedExpression 数据行**：由表达式行隐含的变体表达式。在下面的示例中（3 个安全气囊仅在 4 门配置中可用，见 name 数据行），此数据行为：`entailedExpression = "ONEOF(OptionAIB)(CTValueAib3)~";`
- 其它常见的 eMServer 对象字段（如 status 和 attachments）也可为 `PmExpressionRule` 定义。

示例：

```
PM_UPDATE PmExpressionRule BasicRule
{
  name = "3 Airbags are available only with 4 Door configuration";
  Comment = "4 Doors";
  domainId = 1;
  expression = "ONEOF(BasicDOR)(CTValueDor4)~";
  entailedExpression = "ONEOF(OptionAIB)(CTValueAib3)~";
}
```

<a id="v9-s62"></a>
##### 不适用列表文件格式（Format of Inapplicable List File）

<!-- p1127 -->

不适用列表文件（PPV 格式）的一般格式由每个条目的一个头行加上每个条目六行数据行组成。每个数据行必须以分号结尾。

- **头行**：标识规则，例如：`PM_UPDATE PmInapplicableSet InapplicableRule`，其中 InapplicableRule 是 External ID，应符合 eMServer ID 规则。
- **name 数据行**：描述规则，例如：`name = "3 Airbags are not available with 2 Doors";`
- **comment 数据行**：即使为空也必须存在，例如：`Comment = "";`
- **domainId 数据行**：此版本中必须包含值 1，例如：`domainId = 1;`
- **expression 数据行**：指定为不适用的变体表达式。在此示例中（导航系统不可用于标准装饰级别），此数据行为：`expression = "ONEOF(OptionAIB)(CTValueAib3) && ONEOF(BasicDOR)(CTValueDor2)~";`
- 其它常见的 eMServer 对象字段（如 status 和 attachments）也可为 `PmExpressionRule` 定义。

示例：

```
PM_UPDATE PmInapplicableSet InapplicableRule
{
  name = "3 Airbags are not available with 2 Doors";
  Comment = "";
  domainId = 1;
  expression = "ONEOF(OptionAIB)(CTValueAib3) && ONEOF(BasicDOR)(CTValueDor2)~";
}
```

<a id="v9-s63"></a>
##### 类型列表文件格式（Format of Types List File）

<!-- p1129 -->

类型列表文件（PPV 格式）的一般格式由每个条目的一个头行加上每个条目六行数据行组成。每个数据行必须以分号结尾。

- **头行**：标识类型，例如：`PM_UPDATE PmVariantType VariantTypeA`，其中 VariantTypeA 是 External ID，应符合 eMServer ID 规则。
- **name 数据行**：描述规则，例如：`name = "Model_A";`
- **comment 数据行**：即使为空也必须存在，例如：`Comment = "";`
- **domainId 数据行**：此版本中必须包含值 1，例如：`domainId = 1;`
- **expression 数据行**：表征该类型的变体表达式。在此示例中，每种类型由一个基本标准表征：`expression = "ONEOF(BasicDOR)(CTValueDor2)~";`
- 其它常见的 eMServer 对象字段（如 status 和 attachments）也可为 `PmExpressionRule` 定义。

示例：

```
PM_UPDATE PmVariantType VariantTypeA
{
  name = "Model_A";
  Comment = "";
  domainId = 1;
  expression = "ONEOF(BasicDOR)(CTValueDor2)~";
}
PM_UPDATE PmVariantType VariantTypeB
{
  name = "Model_B";
  Comment = "";
  domainId = 1;
  expression = "ONEOF(BasicDOR)(CTValueDor4)~";
}
```

变体定制 PPV 文件示例：

```
PM_UPDATE PmBasicCriterion BasicDOR
{
  name = "DOR";
  comment = "Door";
  attachments = ;
  status = "Open";
  domainId = 1;
  criterionValues = "";
}
PM_UPDATE PmStringCriterionValue CTValueDor2
{
  name = "2";
  comment = "2";
  attachments = ;
  status = "Open";
  criterion = BasicDOR;
  criterionValue = "2";
}
PM_UPDATE PmStringCriterionValue CTValueDor4
{
  name = "4";
  comment = "4";
  attachments = ;
  status = "Open";
  criterion = BasicDOR;
  criterionValue = "4";
}
PM_UPDATE PmOptionCriterion OptionAIB
{
  name = "AIB";
  comment = "Airbag";
  attachments = ;
  status = "Open";
  domainId = 1;
  criterionValues = "";
}
PM_UPDATE PmStringCriterionValue CTValueAib2
{
  name = "2";
  comment = "2";
  attachments = ;
  status = "Open";
  criterion = OptionAIB;
  criterionValue = "2";
}
PM_UPDATE PmStringCriterionValue CTValueAib3
{
  name = "3";
  comment = "3";
  attachments = ;
  status = "Open";
  criterion = OptionAIB;
  criterionValue = "3";
}
PM_UPDATE PmExpressionRule BasicRule
{
  name = "3 Airbags are available only with 4 Door configuration";
  Comment = "4 Doors";
  domainId = 1;
  expression = "ONEOF(BasicDOR)(CTValueDor4)~";
  entailedExpression = "ONEOF(OptionAIB)(CTValueAib3)~";
}
PM_UPDATE PmInapplicableSet InapplicableRule
{
  name = "3 Airbags are not available with 2 Doors";
  Comment = "";
  domainId = 1;
  expression = "ONEOF(OptionAIB)(CTValueAib3) && ONEOF(BasicDOR)(CTValueDor2)~";
}
PM_UPDATE PmVariantType VariantTypeA
{
  name = "Model_A";
  Comment = "";
  domainId = 1;
  expression = "ONEOF(BasicDOR)(CTValueDor2)~";
}
PM_UPDATE PmVariantType VariantTypeB
{
  name = "Model_B";
  Comment = "";
  domainId = 1;
  expression = "ONEOF(BasicDOR)(CTValueDor4)~";
}
```

<a id="v9-s64"></a>
#### 导入定制文件（Importing Customization Files）

<!-- p1132 -->

要导入定制文件：

**步骤**

1. 选择 **Preparation 选项卡 → Import 组 → Import Variant Customization**。打开浏览窗口。
2. 在浏览窗口中，选择要导入的定制文件。
3. 点击 **Import**。

<a id="v9-s65"></a>
#### 导出定制文件（Exporting Customization Files）

<!-- p1133 -->

要导出定制文件：

**步骤**

1. 选择 **Preparation 选项卡 → Export 组 → Export Variant Customization**。打开浏览窗口。
2. 在浏览窗口中，选择要导出的定制文件。您可以指定是否将所选变体过滤器应用到文件。
3. 点击 **Export**。

<a id="v9-s66"></a>
#### 变体定制浏览器（Variant Customization Viewer）

<!-- p1133 -->

<a id="v9-s67"></a>
##### 打开变体定制浏览器（Opening the Variant Customization Viewer）

<!-- p1133 -->

选择 **Special Data 选项卡 → Variants 组 → Variant Customization Viewer**。变体定制浏览器打开。

变体定制浏览器工具显示所应用变体定制可用的标准和规则。

该表包含一个下拉列表，用于设置表显示以下内容：

- **All criteria（所有标准）**
- **Basic criteria（基本标准）**
- **Variant types（变体类型）** —— 每种类型及其表达式
- **Inapplicable sets（不适用集）** —— 显示的名称是每个表达式的 caption
- **Variant rules（变体规则）** —— 每个表达式包含其相关的隐含表达式

<a id="v9-s68"></a>
### 在筛选模式下检索已删除对象（Retrieving Deleted Objects in Filtered Mode）

<!-- p1136 -->

丢失与查找（Lost and Found）通知可防止意外删除被筛选掉的项。

当您将过滤器应用于树结构时，某些数据元素可能被筛选掉。例如，对于以下数据结构：

如果将标准与 Object C 不匹配的过滤器应用于该数据，则该对象被筛选掉：

如果随后删除 Object B，您也会（可能是无意的）删除其被筛选掉的子元素 Object C。

Lost and Found 功能通过执行以下操作确保任何被间接删除的数据不会丢失：

- 系统显示以下通知：
- 它将数据移动到一个位于 `<user>` 文件夹下的特殊 Lost and Found 文件夹：

移除过滤器后，您可以根据需要管理此数据。

<a id="v9-s69"></a>
## 分析（Analysis）

<!-- p1138 -->

<a id="v9-s70"></a>
### 获取已签入对象（Get Checked In Objects）

<!-- p1138 -->

在检出（check out）某个结构时，您可以运行 **Get Checked in Objects**（来自 Special Data → Analysis 组）以打开一个专用浏览器，显示仍被其他用户签入的所有对象的列表。您可以点击 **Check Out All（全部检出）** 按钮检出所有对象。

<a id="v9-s71"></a>
### 获取由我检出的对象（Get Objects Checked Out by Me）

<!-- p1138 -->

在某些情况下，在特定结构下仍有若干节点由您自己检出，您可以运行 **Get Objects Checked Out by Me**（来自 Special Data → Analysis 组）。此命令生成这些对象的列表，便于查看您仍需处理的内容。

您可以右键点击 Get Objects Checked Out by Me 对话框中的对象，以显示允许您签入或取消检出的菜单。

<a id="v9-s72"></a>
### 获取由他人检出的对象（Get Objects Checked Out by Others）

<!-- p1139 -->

在某些情况下，仍有若干节点由其他用户检出，您可以运行 **Get Objects Checked Out by Others**（来自 Special Data → Analysis 组）以生成这些对象及检出它们的用户的列表。您可以点击 **Log All to File（全部记录到文件）** 按钮创建列表，然后联系这些用户以请求他们签入对象。

<a id="v9-s73"></a>
### 获取具有无效表达式的对象（Get Objects with Invalid Expressions）

<!-- p1140 -->

如果在导入变体定制后存在包含无效表达式的变体对象，您需要定位并修复它们。

要定位具有无效表达式的对象：

选择 **Preparation 选项卡 → Import 组 → Get Objects with Invalid Expressions**。

- 点击 **Get Objects with Invalid Expressions**。

对于私有变体，显示的是分配了该变体的对象（而非私有变体本身）。

您现在可以通过移除或替换所有 Null 实例来修复所有无效表达式。有关操作方法，请参阅 Creating/Editing Variant Set Expressions。

<a id="v9-s74"></a>
### 获取签入 / 检出状态（Get Check In/Checked Out Status）

<!-- p1140 -->

当与其他用户一起处理大型数据集时，您可以轻松地使用 Get Objects Checked Out by Others 创建已检出对象的列表及其用户。随后，在用户签入对象后，您可以使用 Get Checked In Objects 一键检出它们全部。您也可以使用 Get Objects Checked Out by Me 生成这些对象的列表，便于查看您仍需处理的内容。

<a id="v9-s75"></a>
## 替代方案（Alternatives）

<!-- p1140 -->

<a id="v9-s76"></a>
### 面向制造工艺规划的替代方案（Alternatives for Manufacturing Process Planning）

<!-- p1140 -->

在制造工艺的规划阶段，通常会评估不同的方法。这些可能表现为：

- **场景（Scenarios）**：评估不同的工厂作为生产某产品变体的候选；或
- **替代方案（Alternatives）**：审查总体规划内容的一部分，以在不同可能的供应商或供应链之间做出决策。

以下各节描述在使用场景和 / 或替代方案时如何使用 Process Designer。为简洁起见，全文使用术语 Alternative（替代方案）来指代场景和替代方案两者。

您也可以使用快速方法 Creating Alternatives from Studies（从研究创建替代方案）。

<a id="v9-s77"></a>
### 创建替代方案（Creating Alternatives）

<!-- p1141 -->

替代方案对象可以从源节点创建，在规划阶段按需要进行更改，然后与原始方案进行比较，以查看计划发生了怎样的变化。

**注意**：在使用 Alternative Management（替代方案管理）功能之前，必须导入相关定制。该定制位于：`Tecnomatix\eMPower\InitData\AlternativeManagementCust`。

要创建替代方案：

**步骤**

1. 在 Project tree 中，选择要在其下创建 Alternative 的项目或集合节点，并按 `<Ctrl>+N`。出现 New 对话框。
2. 勾选 **Alternative** 并点击 **OK**。在树中创建一个新的空 Alternative 文件夹。该文件夹充当容器，用于从中定义 Alternative 的源（Alternative 范围）以及更新后的 Alternative 的结果。您可以根据需要重命名 Alternative 文件夹。
3. 在新的 Alternative 下创建一个新的 Alternative Scope（替代方案范围）对象（参见 Creating a Module）。该节点是 Alternative 的源或范围——它以 Alternative 图标标记。此节点下的所有项都链接到它们在项目层次结构中的原始位置。这是 Alternative 范围内的一个对象，并充当 Clone Alternative（克隆替代方案）功能的源。
4. 通过拖放将对象添加到其中，以定义 Alternative Scope 的范围（参见 Adding Objects to a Module）。范围对象现已填充。在上例中，它包含 Operation。
5. 根据需要创建过滤器（参见 Filtering Alternatives）。
6. 通过选择源范围节点并从右键菜单中选择 **Clone/Update Alternative** 来创建 Alternative。Alternative 出现在 Alternative 节点下方。该节点是实际的 Alternative。它是 Alternative 的子节点。

**注意**：Alternative 的默认名称是其 Scope Operation 的名称，但可根据需要重命名。在上例中，Scope Operation 命名为“Operation”，而 Alternative 已重命名为“Alternative Operation”。

下图显示了两个 Alternative，每个都有自己的源（或范围）节点。替代方案彼此独立，即使从同一源创建，对其中一个的更改也不会影响另一个。

**注意**

- 根据范围内容，克隆范围之间的关系会链接到新结构或原始结构。例如，克隆 CompoundPart 时，实例仍指向 PartPrototypes。但是，将 PartPrototypeLibrary 包含在克隆范围中会创建新的 PartPrototypes；被克隆 CompoundPart 的 PartInstances 指向新的 PartPrototypes。
- 每个被克隆的元素都被标记为已克隆，因此当从范围中移除元素时，在后续的 Alternative 更新中也会移除它们。
- 如果某个被克隆的元素在 Alternative 结构内被移动到原始结构（源结构）中不存在的新父对象，则该父关系在更新期间保持不变。

<a id="v9-s78"></a>
### 筛选替代方案（Filtering Alternatives）

<!-- p1143 -->

可以创建过滤器，使得在创建 Alternative 时仅克隆源中的某些对象。

要创建显示在 Alternative 节点下方的对象的过滤器：

**步骤**

1. 选择源 Alternative 节点，并点击 **Attachments** 选项卡。
2. 从系统根目录中选择所需的 `.xsl` 过滤器定义文件。仅所选对象会包含在从此节点克隆的 Alternative 中。

过滤器功能用于 Clone Collection（克隆集合）以及 Cloning/Updating Alternatives（克隆 / 更新替代方案），以从克隆过程中移除特定节点。它不用于阻止节点的更新，您可以通过节点上的 AlternativeUpdate 字段来实现。要应用过滤器，您应创建 `.xls` 文件并将其放置在系统根目录下的 `\General\AlternativeManagement\Filters` 中。每当对集合运行 clone collection，或对替代方案运行 Clone\Update Alternative 时，此过滤器即被激活。此外，您可以将 `.xls` 过滤器放置在 Filters 文件夹以外的任意位置，并附加到集合或替代方案上，此过滤器仅适用于该特定集合。您可以在 Clone Collection 对话框中取消勾选 Apply Filters（应用过滤器）复选框，以防止在存在过滤器时克隆集合时使用它们（参见 Running Clone Collection）。

例如，对于您不希望克隆的操作，在 `.xsl` 文件中使用 `<Filtered-Object>` 指定它，同时使用 `<Remove-Link>` 标签指定其与其它对象的所有可能链接。在下例中，具有 ExternalId `PP-TEST2-15-4-2015-17-35-56-9055-10576` 的节点是具有 ID `PP-TEST2-15-4-2015-17-35-29-9055-10573` 的节点的子节点：

因此您需同时指定要筛选的对象以及要移除的链接，在本例中为“Children”关系。如果您未指定指向被筛选对象的链接，会导致克隆过程失败。

由于 `.xsl` 文件使用实际的编程语言，您可以编写更复杂的过滤器以排除符合特定条件的节点，例如没有实例的 PartPrototypes。在这种情况下，`.xsl` 过滤器可以定位它们，并由脚本填充 `<Filtered-Object>` 和 `<Remove-Link>`。以下为一个示例过滤器：

```xml
<?xml version="1.0"?>
<!-- partprotoypefilter.xsl, version 1 2/12/2006 XSLT filter for the 'Clone/Update Alternative' command that strips part prototypes without instances. -->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output indent="yes" method="xml"/>
  <!-- key that maps the prototype id of a part to its corresponding XML node -->
  <xsl:key use="prototype" match="/Data/Objects/*[NodeInfo/family='Product']" name="parts-map"/>
  <!-- key that maps the external id of a node to its parent in the node hierarchy -->
  <xsl:key use="children/item" match="/Data/Objects/*" name="parent-map"/>
  <xsl:template match="/">
    <Filter>
      <xsl:apply-templates/>
    </Filter>
  </xsl:template>
  <xsl:template match="/Data/Objects/*[NodeInfo/family='PartPrototype']">
    <xsl:choose>
      <xsl:when test="key('parts-map',@ExternalId)">
        <!-- part prototype references part -->
      </xsl:when>
      <xsl:otherwise>
        <!-- part prototype is not linked to a part -->
        <xsl:variable name="prototype-id" select="@ExternalId"/>
        <Filtered-Object>
          <xsl:value-of select="$prototype-id"/>
        </Filtered-Object>
        <xsl:for-each select="key('parent-map',$prototype-id)">
          <Remove-Link target="{$prototype-id}" source="{@ExternalId}"/>
        </xsl:for-each>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <!-- override the default rules to prevent copying the text content of element nodes -->
  <xsl:template match="text()"/>
</xsl:stylesheet>
```

<a id="v9-s79"></a>
### 保存替代方案差异（Saving Alternative Differences）

<!-- p1145 -->

Process Designer 使您能够导出一个 CSV 文件，列出源与其克隆之间的差异。

要导出替代方案差异到文件：

**步骤**

1. 选择替代方案层次结构中的任意节点并点击相应按钮。
2. 当提示时，输入文件路径和名称并点击 **Save**。显示该文件。

**注意**：您可以选择替代方案层次结构中的任意节点，但那些以灰色显示的节点除外。系统收集有关所选节点及其所有后代节点的信息，并在输出文件中列出所有差异。

<a id="v9-s80"></a>
### 显示替代方案历史（Show Alternatives History）

<!-- p1147 -->

系统在 `apUpdateHistory` 属性中跟踪您对替代方案所做的所有更改。

要查看替代方案历史：

- 点击相应按钮。历史在 Internet Explorer 中打开。

```xml
<AlternativeSettings>
  <UpdateMaster></UpdateMaster>
  <UpdateAlternative>
    <UpdateEvent DateStamp="1/27/2010 4:16:33 PM"></UpdateEvent>
    <UpdateEvent DateStamp="1/27/2010 4:25:29 PM">
      <User>administrator</User>
      <Settings>
        <Setting id="UpdateAttributeData">1</Setting>
        <Setting id="UpdateNodeStructure">1</Setting>
        <Setting id="UpdateResourceAssignments">1</Setting>
        <Setting id="UpdateOperatorAssignments">0</Setting>
        <Setting id="IgnoreErrors">0</Setting>
        <Setting id="RecreateDeletedNodes">1</Setting>
        <Setting id="DeleteImportFolder">0</Setting>
        <Setting id="UpdateEngineeringData">1</Setting>
        <Setting id="UpdateSimulationInfo">1</Setting>
        <Setting id="UpdateMfgAssignments">1</Setting>
        <Setting id="UpdateOperationSequence">1</Setting>
        <Setting id="UpdateAll">1</Setting>
        <Setting id="CheckInBeforeUpdate">1</Setting>
        <Setting id="CheckInAfterUpdate">1</Setting>
      </Settings>
      <AttributeConfiguration>
        <Node class="class PmNode">
          <Field name="caption"/>
        </Node>
      </AttributeConfiguration>
      <ExcludedResources>
        <Comment></Comment>
      </ExcludedResources>
    </UpdateEvent>
    <UpdateEvent DateStamp="1/27/2010 4:42:54 PM"></UpdateEvent>
  </UpdateAlternative>
</AlternativeSettings>
```

<a id="v9-s81"></a>
### 用于属性的替代方案更新（Alternative Update for Attributes）

<!-- p1147 -->

Alternative Update for Attributes（用于属性的替代方案更新）命令使您能够从主方案（Master）向替代方案更新所选属性。系统仅更新相关属性，而不会浪费资源去更新整个替代方案。

要更新替代方案的属性：

**步骤**

1. 点击相应按钮。出现 Alternative Update for Attributes 对话框。
2. 勾选以下之一：
   - **Include all（包含全部）** —— 在更新中包含所有属性。
   - **Include specific fields（包含特定字段）** —— 点击相应按钮打开 Field Selector，并选择您希望更新的属性。

**注意**：您可以点击 Field Selector 中的列标题对显示进行排序，便于定位项。

   a. 在左窗格中点击一个对象，以在右窗格中显示其属性。
   b. 在右窗格中双击一个属性以将其包含在更新中。或者右键点击该属性并选择 **Add**。该属性显示在 Field Selector 底部的 Selected fields 窗格中。
   c. 在 Selected fields 窗格中双击一个属性以将其从更新中移除。或者右键点击该属性并选择 **Remove**。该属性从 Selected fields 窗格中移除。
3. 点击 **OK** 保存数据。

<a id="v9-s82"></a>
### 用于属性的主方案更新（Master Update for Attributes）

<!-- p1149 -->

Master Update for Attributes（用于属性的主方案更新）命令使您能够从替代方案向主方案更新所选属性。您仅更新相关属性，从而节省更新整个主方案的开销。

要更新主方案的属性：

**步骤**

1. 点击相应按钮。出现 Master Update for Attributes 对话框。
2. 勾选以下之一：
   - **Include all** —— 在更新中包含所有属性。
   - **Include specific fields** —— 点击相应按钮打开 Field Selector，并选择您希望更新的属性。使用 Field Selector 配置要更新的属性，如 Alternative Update for Attributes 中所述。
3. 点击 **OK** 保存数据。

<a id="v9-s83"></a>
### 杂项（Misc）

<!-- p1149 -->

<a id="v9-s84"></a>
#### 同步工艺对象（Synchronize Process Objects）

<!-- p1149 -->

使用 Synchronize Process Objects（同步工艺对象）功能维护包含孪生（twin）对象的 Operation 树与 Resource 树之间的同步。该命令可从任一类型的树激活。

**步骤**

1. 选择一个工艺对象节点，并选择 **Special Data 选项卡 → Misc 组 → Synchronize Process Objects**。显示 Synchronize Process Objects 对话框。
2. 在 Attributes to Synchronize（要同步的属性）窗格中，点击您希望为孪生对象同步的属性旁边的复选框。默认情况下，对话框显示时已选中以下属性：Name、Number、Comment 和 Order，这将对 Operation 树和 Resource 树中的孪生对象施加相同的顺序。

为方便和加快操作，您可以使用 Attributes to Synchronize 窗格右侧的按钮来：

- 勾选列表中的所有项；
- 勾选默认项；
- 取消勾选列表中的所有项。

3. 可选：您可以为同步的对象添加前缀（Prefix）和 / 或后缀（Suffix），并点击复选框以包含子树（sub-trees）。

<a id="v9-s85"></a>
#### 设置 CAD 图层的配置与信息（Set Configuration and Information for CAD Layers）

<!-- p1150 -->

Process Designer 支持向对象添加属性以定义关联的 CAD 图层。配置图层属性并添加信息后，用户可以使用 Export to JT（导出为 JT）为图层信息做准备，以便传输到 CAD 程序。

要设置图层配置（具有管理员权限的用户）：

**步骤**

1. 选择 **Special Data 选项卡 → Misc 组 → Set Layer Configuration**。

2. 在 Open File 对话框中，选择包含所需图层信息（id、name 和 description）的 xml 文件。该 xml 文件的结构类似于值列表（List of Values，LOV）文件的结构。系统会根据您的项目名称及内部 id 自动重命名该文件。它还将该文件复制到 System root 下的 General 文件夹，供其它命令（例如 Set Layer Information）使用。

xml 文件示例：

```xml
<LayerConfiguration version="9.1">
  <LayerInfo>
    <Layer data="0" name="Default" description="DefaultDescription" />
    <Layer data="1" name="NameofLayer1" description="DescriptionofLayer1" />
    <Layer data="2" name="NameofLayer2" description="DescriptionofLayer2" />
    <Layer data="3" name="NameofLayer3" description="DescriptionofLayer3" />
    <Layer data="4" name="NameofEmptyLayer4" description="" />
    <Layer data="5" name="NameofLayer5" description="DescriptionofLayer5" />
  </LayerInfo>
</LayerConfiguration>
```

**为对象设置图层信息：**

**步骤**

1. 要为资源、零件等设置图层名称，选择 **Special Data 选项卡 → Misc 组 → Set Layer Configuration**。显示 Set Layer Information 对话框：
2. 您可以在 Name 字段中修改图层名称，并查看只读的 description。此信息从图层的 xml 文件更新到内部的 CAD Layer 属性。您可以在 Export to JT 期间包含此图层信息，以供 CAD 应用程序、Microstation XM 等使用。

**为项目设置图层信息：**

**步骤**

1. 选择 **Special Data 选项卡 → Misc 组 → Set Layer Configuration**。显示 Set Project Layer Information 对话框，用于 Graphic viewer 中包含的所有内容：
2. 您可以在各自的 Name 字段中修改 Dimensions、Label 和 Section 图层的名称，并查看其只读描述。此信息从图层的 xml 文件更新到内部图层属性。您可以在 Export to JT 期间包含此图层信息（Dimensions 图层信息除外），以供 CAD 应用程序、Microstation XM 等使用。

**注意**：您也可以为物流区域（Logistics Areas）和轨道（Tracks）设置图层信息。

<a id="v9-s86"></a>
#### 合并研究（Merge Studies）

<!-- p1153 -->

Merge Studies（合并研究）命令使您能够将来自若干不同研究的工程数据合并到一个新的研究中。

**步骤**

1. 选择 **Special Data 选项卡 → Misc 组 → Merge Studies**。

   - 如果未预选任何研究，会显示以下消息，警告系统即将打开项目中的所有研究，且此操作可能执行时间较长：点击 **Yes** 继续，或点击 **No** 中止并在再次启动命令前选择研究。Merge Studies 对话框出现，当前项目的研究位于 Available Studies（可用研究）列表中。
   - 如果您已预选研究，Merge Studies 对话框出现，预选的研究位于 Available Studies 列表中。
2. 输入 **New Study Name（新研究名称）**。
3. 从 **New Study Type（新研究类型）** 下拉列表中选择新研究的类型。
4. 您可以点击 **Add All Studies** 将 Available Studies 列表中项目里的所有研究列出。
5. 要指示要合并的研究，在 Available Studies 列表中选择一个研究并点击相应按钮，将其添加到 Studies to Merge（要合并的研究）列表。
6. 重复上一步以包含您希望合并的所有研究。
7. 要从合并操作中移除某个研究，在 Studies to Merge 列表中选择它并点击相应按钮。

例如，合并 RobcadStudy1 和 RobcadStudy2。

8. 点击 **Merge** 完成操作。

新研究出现在 Navigation Tree 中。您可以在 Object Viewer 中查看新研究，以查看合并结果。

<a id="v9-s87"></a>
## 变更管理（Change Management）

<!-- p1156 -->

<a id="v9-s88"></a>
### 管理版本之间的变更（Managing Changes between Versions）

<!-- p1156 -->

Process Designer 使您能够验证任意两个版本（系统版本或本地版本）之间的变更。您可以查看这些变更，然后在需要时恢复从先前版本检索到的数据。

Change Management 窗口可针对任意对象访问。在打开某个对象的 Change Management 窗口之前，您必须至少对该对象签入过一次。

要访问该列表，选择节点，然后点击 Change Management 图标，或选择 **Home 选项卡 → Viewers 组 → Viewers** 并选择 **Change Management**。

变更管理包含以下功能：

- **上下文菜单（Context Menus）**：用户可以为显示在 Change Management 树中的对象定义上下文菜单。
- **选择同步（Selection Synchronization）**：用户可以将对象（操作、零件、资源等）的选择与相关的树（Operation 树、Part 树、Resource 树等）以及所有其它浏览器（包括 Graphic Viewer、Relations Viewer 等）同步。同步的选择在 Change Management 中选择对象时提供高亮指示。
- **对象历史（Object History）**：用户可以查看所选对象的版本历史。

**注意**：为节省时间，最好选择一个复合节点或制造特征库，然后对树或子树的节点使用 Change Management，而不是为单个节点打开 Change Management 窗口。

<a id="v9-s89"></a>
### 变更管理工具栏（Change Management Toolbar）

<!-- p1158 -->

| 按钮（Button） | 名称（Name） | 功能（Function） |
| --- | --- | --- |
| | Restore to other version（恢复到其它版本） | 将右窗格中所选的较旧节点版本恢复到左窗格。仅当左窗格中选定当前版本时才能执行此操作。参见 Using Change Management。 |
| | Display Object Modification list（显示对象修改列表） | 显示对该节点所执行变更的列表。 |
| | Approve status（批准状态） | 标记所选节点。此标记仅供用户使用。参见 Using Change Management。 |
| | Disapprove status（取消批准状态） | 取消标记所选节点。 |
| | Scrollers UnSynchronization（滚动条取消同步） | 左、右窗格垂直滚动条独立操作。 |
| | Scrollers Synchronization（滚动条同步） | 左、右窗格联合滚动。 |
| | Nodes Synchronization（节点同步） | 左、右窗格滚动以显示相同的所选节点。垂直滚动条可处于不同位置。 |
| | Next change（下一变更） | 选择树中向下的下一个变更。 |
| | Previous change（上一变更） | 选择树中向上的下一个变更。 |
| | Last change（最后变更） | 选择朝向树底部的最后一个变更。 |
| | First change（首个变更） | 选择朝向树顶部的第一个变更。 |
| | Next change in the sub-tree（子树中的下一变更） | 在树中向下搜索变更。 |
| | Show only prototype changes（仅显示原型变更） | 按下时，此切换仅将以绿色高亮那些从其各自原型继承的变更实例。停用后，所有已变更的实例都以绿色高亮。 |
| | Open Configuration list dialog（打开配置列表对话框） | 参见 Creating a New Table View Configuration。 |

<a id="v9-s90"></a>
### 基本变更管理功能（Basic Change Management Functionality）

<!-- p1159 -->

使用每个框架上方的组合框，选择您希望比较的版本。任何系统版本或本地保存都可以与任何其它版本或保存进行比较，并可以详细分辨率追溯变更。如果您从组合框中选择的版本中不存在父节点（最顶层节点），则会出现相应的消息。

要展开节点并显露其子树，在任一框架中选择它，右键点击并选择 **Expand**，然后选择四个展开级别之一。如果选择 Expand All Levels（展开所有级别），会出现以下窗口。

下表解释了左、右框架中出现的图标：

| 颜色（Color） | 表示（Representation） | 注释（Notes） |
| --- | --- | --- |
| 绿色（Green） | 已变更节点（Changed node） | 在左、右窗口窗格中都出现。 |
| 红色（Red） | 已添加节点（Added node） | 出现在左窗口窗格中。 |
| 蓝色（Blue） | 已删除节点（Deleted node） | 出现在右窗口窗格中。 |
| | 已批准节点（Approved node） | 出现在左窗口窗格中。 |

已删除节点在左框架的树中显示为空的填充符。当您在左窗格的树中选择项时，实体在 Graphic Viewer 中高亮。新节点在右框架的树中显示为空的填充符。

窗口左下角的图标可展开 Change Manager，并在每个框架下方显示所选节点的各自属性。

<a id="v9-s91"></a>
### 修改列表（Modification List）

<!-- p1160 -->

要访问该列表，在 Change Management 窗格中右键点击，并选择 **Show Object Modification list** 选项，或点击 Modification List 图标。

修改列表仅当某个节点从一个版本到另一个版本确实发生变更时才可用。

有关如何设置过滤器配置的详细说明，请参阅 Tecnomatix 管理文档。

<a id="v9-s92"></a>
### 使用变更管理（Using Change Management）

<!-- p1161 -->

<a id="v9-s93"></a>
#### 批准节点（Approving Nodes）

<!-- p1161 -->

要将节点标记为已批准，在任一窗口框架中选择一个节点，并执行以下步骤之一：

- 点击 Change Management 工具栏中的 **Approve status** 图标。
- 右键点击该节点，并从打开的菜单中选择 **Approve Status**。

已批准节点图标出现在左窗口框架中所选节点的左侧。

<a id="v9-s94"></a>
#### 撤销变更（Undo Changes）

<!-- p1161 -->

要将节点恢复到先前状态并撤销其变更，执行以下操作：

**步骤**

1. 在左框架组合框中选择“Latest Version（最新版本）”。
2. 在右框架中，使用组合框选择将从中执行恢复的所需版本。
3. 根据需要展开树以定位相应节点，然后按 Change Management 工具栏中的相应按钮。可以根据需要部分或整体恢复某个版本。

<a id="v9-s95"></a>
#### 签入与签出对象（Checking Objects In and Out）

<!-- p1162 -->

要签入或签出对象，在左窗口框架中选择它，并执行以下步骤之一：

- 选择 **Home 选项卡 → CIO 组 → Check In** 或 **Check Out**。
- 右键点击并从打开的菜单中选择 Check In 或 Check Out。

请注意，从 Change Management 窗口签入对象会签入该对象的原型（prototype），而从对象的树签入对象仅签入该对象的实例（instance）。
<a id="v10-s1"></a>
<!-- p1163 -->
# 10. Preparation（准备）

本卷（Volume 10）介绍 Process Designer 中的准备（Preparation）与设置类命令，涵盖导入/导出（Import/Export）、库（Library）、边界框（Bounding Box）计算以及管理（Administration）等相关内容。

<a id="v10-s2"></a>
<!-- p1163 -->
## Import（导入）

<a id="v10-s3"></a>
<!-- p1163 -->
### Import Project（导入项目）

如果 Process Designer 应用中已打开一个项目，在导入另一个项目之前必须将其关闭。项目中的所有数据都以“已检出（checked out）”状态导入。建议在开始处理该项目之前，将所有数据检入（check in）。

项目可以以 `*.xml` 或 `*.ppj` 格式导入 Process Designer。

**过程（Procedure）**

1. 选择 **Preparation** 选项卡（tab）→**Import** 组（group）→**Import Project**。将显示 Import 窗口：
2. 浏览到所需目录，选择要导入的 `*.ppj` 或 `*.xml` 文件。
3. 单击 **Import**。

将显示以下消息：

> **注意（Note）**
> 导入大量数据时，此过程可能需要很长时间。如果要中止导出过程，必须联系管理员为您执行该任务。有关终止活动会话的详细信息，请参阅 Tecnomatix Administration 文档中的 Managing Access Rights。

4. 单击 **OK**。导入的项目将保存在所选目录中。

---

<a id="v10-s4"></a>
<!-- p1164 -->
### Import Customization（导入定制）

请参阅 Tecnomatix Administration 文档中 Administrative and Management Tools 部分的 Importing Customization Data 主题。

---

<a id="v10-s5"></a>
<!-- p1165 -->
### Import Queries（导入查询）

请参阅 Tecnomatix Administration 文档中 Administrative and Management Tools 部分的 Importing Queries 主题。

---

<a id="v10-s6"></a>
<!-- p1166 -->
### Import Variant Customization（导入变体定制）

请参阅 Importing Customization Files。

---

<a id="v10-s7"></a>
<!-- p1166 -->
## Export（导出）

<a id="v10-s8"></a>
<!-- p1166 -->
### Export Project（导出项目）

通过高亮显示项目节点（project node）获得的工程文件，可以通过 Tecnomatix 专用的 `*.ppj` 项目格式或 `*.xml` 格式导出。要将项目导出到外部系统，请将项目保存为 `*.xml` 格式；要将项目导出给使用不同模式（schema）的另一个 Tecnomatix 用户，请将项目保存为 `*.ppj` 或 `*.xml` 格式。

将项目导出到相同模式时，无需同时导出定制文件。但是，如果要将项目导出到同一版本中的不同模式，必须先导出定制文件，然后再导出所选项目。有关更改版本时导出项目的更多详细信息，请参阅 Tecnomatix Administration 文档中的 Data Migration。

**过程（Procedure）**

1. 选择 **Preparation** 选项卡→**Export** 组→**Export Project**。将显示 Export 窗口：
2. 浏览到要导出数据的目标目录。

> **注意（Note）**
> 为提升性能，建议将待导入/导出的数据放置在系统根目录（system root directory）或其某个子目录中。

3. 在 **File name** 字段中输入文件名。
4. 从 **Save as type** 下拉列表中选择 `*.ppj` 或 `*.xml`，然后单击 **Save**。

过程完成时将显示以下消息：

> **注意（Note）**
> 导出大量数据时，此过程可能需要很长时间。如果要中止导出过程，必须联系管理员为您执行该任务。有关终止活动会话的详细信息，请参阅 Tecnomatix Administration 文档中的 Managing Access Rights。

5. 单击 **OK**。导出的项目将保存在所选目标目录中。

---

<a id="v10-s9"></a>
<!-- p1168 -->
### Export Customization（导出定制）

请参阅 Tecnomatix Administration 文档中 Administrative and Management Tools 部分的 Exporting Customization Data 主题。

---

<a id="v10-s10"></a>
<!-- p1169 -->
### Export Queries（导出查询）

请参阅 Tecnomatix Administration 文档中 Administrative and Management Tools 部分的 Exporting Queries 主题。

---

<a id="v10-s11"></a>
<!-- p1170 -->
### Export Variant Customization（导出变体定制）

请参阅 Exporting Customization Files。

---

<a id="v10-s12"></a>
<!-- p1170 -->
## Libraries（库）

<a id="v10-s13"></a>
<!-- p1170 -->
### Creating Engineering Libraries（创建工程库）

创建工程库（Engineering Libraries）是指将组件类型（资源或部件）分配给 Process Designer 中的 3D 数据。这样，在调用集成（integration）时，3D 组件便能被识别。此外，此功能会为组件创建到图像文件（如果存在）的链接。该图像显示在部件或工具原型（prototype）的 **Physical** 选项卡中。

> **注意（Note）**
> 该命令会在用户临时目录（user temp directory）中创建一个名为 `eM_SyncLib.log` 的日志文件。

**创建工程库：**

**过程（Procedure）**

1. 选择 **Preparation** 选项卡→**Libraries** 组→**Create Engineering Libraries**。将显示 Directory browser 对话框，其中包含系统根目录下存在的所有目录。
2. 浏览到所需 3D 数据所在的目录，然后单击 **OK**。将显示 Type Setting 对话框：
3. 选择相关节点（组件目录或单个组件），然后单击该节点的 **Type** 字段。

> **注意（Note）**
> 选择组件目录时，目录中的所有单个组件将自动被选中，并在下一步中被分配相同的类型。
> 如果某个组件已创建工程库，该组件的 Type 字段将显示为灰色。

4. 从列表中选择一个类型。
5. 如果需要，重复步骤 3 和 4，为其他组件分配类型。
6. 为所有 3D 数据分配类型后，单击 **OK**。分配过程中将显示沙漏图标，状态栏（Status Bar）中出现 Creating Notes 消息。将所选类型分配给所选组件并导入 Process Designer 项目后，将显示完成消息，告知您工程库已成功创建并导入。
新的工程库现在可作为资源库（Resource Library）或部件库（Part Library）在 Working Folder 目录中使用。
7. 要查看新的工程库层次结构，请双击相应的 Resource Library 或 Part Library。

> **注意（Note）**
> 此过程会在组件下生成一个 `TuneData.xml` 文件，其中包含相关信息，如节点类型、名称和重量。同一个组件可以映射到多个项目，并为每个项目创建一个 `TuneData.xml` 文件。

---

<a id="v10-s14"></a>
<!-- p1172 -->
### Updating Engineering Library（更新工程库）

您可以对相关节点或库进行更新，例如，在 Process Designer 中更新资源的重量值后，可以更新相关的工程库，使更新后的信息也能在 Process Simulate 中使用。

> **注意（Note）**
> 该命令会在用户临时目录中创建一个名为 `eM_SyncLib.log` 的日志文件。

**更新工程库：**

**过程（Procedure）**

1. 在 Process Designer 中，根据需要修改原型（prototype(s)）。
2. 选择相关的 Resource Library 或 Part Library，并选择 **Preparation** 选项卡→**Libraries** 组→**Update Engineering Library**。组件文件夹中的相关 `TuneData.xml` 文件将自动更新。

---

<a id="v10-s15"></a>
<!-- p1172 -->
### Updating Libraries（更新库）

定义为主库项目（master library project）的项目包含在相同数据库模式（schema）中与其他项目相关的库。定义和管理主库项目是库管理员（library administrator）的职责。

为了同步和更新其他项目的库，库管理员在主库项目的库中进行更改，并使用 Library Update 命令（参见 Preparing for Updates）将这些更改传播到同一数据库模式中的其他项目。管理员可以运行立即更新（immediate updates）或计划更新（scheduled updates）（参见 Updating User's Libraries）。

<a id="v10-s16"></a>
<!-- p1172 -->
#### Library Update（库更新）

包含相同数据库模式中各项目相关库的、被定义为主库项目的项目，其同步与更新由库管理员负责。库管理员通过 Library Update 命令将主库中的更改传播到其他项目。

<a id="v10-s17"></a>
<!-- p1172 -->
#### Preparing for Updates（准备更新）

只有拥有库管理（Library Administration）权限的用户才能使用 Library Update 工具。库管理员需要使用 Admin Console 工具授予此权限。

为了使 `masterLibrary` 字段可用，必须将 `LibraryUpdateCust` 定制（位于 `eM-Power/InitData` 文件夹）导入 eMServer。

要成功地将用户库与主库同步，请检查：

- 所有待更新的用户库均已检入（checked in）。
- 任何模式中有且仅有一个项目被预设为 Master Library。

**将项目设置为主库（Master Library）：**

**过程（Procedure）**

1. 右键单击要设置的项目。
2. 从上下文菜单中选择 **Properties**。将显示 Properties 窗口：
3. 单击 **Attributes** 选项卡。
4. 单击 `masterLibrary` 属性。确保显示相应的值非零。
5. 关闭窗口。

建议检查没有其他库被设置为主库。

<a id="v10-s18"></a>
<!-- p1174 -->
#### Updating User's Libraries（更新用户库）

要使用主库数据更新库：

**过程（Procedure）**

1. 确保已关闭 Process Designer 中的所有项目。

> **注意（Note）**
> 只有在没有打开任何项目时，此命令才处于活动状态。

2. 选择 **Preparation** 选项卡→**Libraries** 组→**Library Update**。将出现 Library Update 窗口。

> **注意（Note）**
> 此选项仅对 Library Managers 组中的用户可用。

3. 如果未将任何项目设置为主库，或设置了多个主库，将显示以下对话框：
4. 从下拉列表中选择要设置为主库的项目，然后单击 **OK**。返回步骤 1。
5. 在相应字段中键入被指定为管理员的 eMS 用户的名称和密码。
6. 选择要更新的项目。Projects 列表显示当前模式可用的项目。
   - 要更新 Projects 列表中的所有项目，请选择 **All Projects**。
   - 要更新所选项目，请勾选这些项目。
7. 您可以立即更新项目，也可以按计划间隔更新：
   - 要立即更新已勾选的项目，请选择 **Immediate Update**，然后单击 **Update**。
   - 要一次性或按特定间隔计划更新，请选择 **Scheduled Update**，然后选择所需的重复模式（recurrence pattern）和重复范围（range of recurrence）选项。为使更新按计划执行（即使无人登录计算机），请提供 Windows 用户组中定义的 Windows 系统用户名和密码。单击 **Update**。
8. 单击 **Settings** 按需配置 Library Update。请参阅 Library Update Settings。

<a id="v10-s19"></a>
<!-- p1175 -->
#### Changes in User's Libraries During Update（更新期间用户库的变化）

在更新期间，每个用户的库层次结构会发生以下变化：

- 首次运行时，Library Update (Browser) 在相应的库层次结构下创建一个集合（collection），名称与主库相同。
- Library Update (Browser) 更新所有用户的库。
- 当主库与用户库存在差异时（例如某原型曾在用户项目中使用，但随后已从主库中删除），相关对象不会被删除，而是被移动到名为 **Deleted Objects** 的特殊集合中。

<a id="v10-s20"></a>
<!-- p1176 -->
#### Immediate and Scheduled Updates（立即更新与计划更新）

更新可以针对全部或选定的项目，可以是立即执行，也可以计划在特定时间间隔执行。计划更新时，它将出现在 Scheduled Tasks 列表中（通过双击 Windows 控制面板中的 Scheduled Tasks 可访问）。

<a id="v10-s21"></a>
<!-- p1176 -->
#### Library Update Settings（库更新设置）

您可以通过选择 **Preparation** 选项卡→**Libraries** 组→**Library Update**，然后在 Library Update 窗口中单击 **Settings** 来配置 Library Update 设置。将出现 Library Settings 窗口。

您可以配置以下内容：

- **Export File Path（导出文件路径）** —— 用于定义保存主库信息、在更新过程中生成的临时 XML 文件的位置。该文件会被导入到每个正在更新的项目中。更新过程完成后，该文件将被删除。导出文件的默认位置为：`<system root>\General\LibraryUpdate\Export`。
- **Log File Path（日志文件路径）** —— 用于定义更新过程中生成的日志文件的位置。更新日志文件的默认位置为：`<system root>\General\LibraryUpdate\Logs`，其命名为 `LibraryUpdate-<创建日期和时间>`。以下为日志示例：
- **Check-out needed objects in the updated projects（检出更新项目中所需的对象）** —— 设置此选项可在更新期间自动检出所有所需对象。如果不使用此选项，则必须在运行更新之前手动检出所有对象。
- **Check-in updated objects when done（完成后检入更新对象）** —— 设置此选项可在更新完成时自动检入所有更新后的对象。

---

<a id="v10-s22"></a>
<!-- p1177 -->
### Upgrade CO Prototypes to Version（将 CO 原型升级到版本）

**Upgrade CO Prototypes to Version** 命令用于升级原型 3D 表示文件，或将原型 3D 表示文件从 `.CO` 格式转换为 `.COJT` 格式。它不对原生 JT 组件（native JT components）进行操作。您可以升级或转换单个原型，也可以指定一个部件库或资源库进行升级或转换。指定库将对库中每个原型的 3D 表示文件进行升级或转换。您还可以自定义 Upgrade To Version 管理工具中使用的升级或转换参数。更多信息请参阅 Upgrade to Version。

Upgrade CO Prototypes to Version 还执行以下操作：

- 升级组件时自动导入 Robcad 材料（materials）。您可以根据需要关闭此选项。
- 对于已升级为 `.co` 或 `.cojt` 格式的组件，提供导入 Robcad 材料的选项而无需执行完整升级。此选项通常用于最初在 Robcad 中创建的组件。

下图说明了导入 Robcad 材料时的改进效果。

**升级 3D 表示文件：**

**过程（Procedure）**

1. 选择以下任意一项：
   - 一个或多个部件（parts）或部件库（part libraries）
   - 一个或多个资源（resources）或资源库（resource libraries）
2. 选择 **Preparation** 选项卡→**Libraries** 组→**Upgrade CO Prototypes to Version**。将打开 Upgrade CO to Version 对话框，使您能够设置升级选项。
3. 选择 **Target Format（目标格式）**，如下：
   - **COJT** —— 仅当希望仅包含详细表示（detailed representations）而排除联合表示（united representations）时，才勾选 **Detailed only**。清除此选项以包含联合表示。
     您可以指定系统在确定所选原型已存在 COJT 文件时的反应：勾选 **Skip** 中止升级，或 **Overwrite** 覆盖现有文件。
   - **CO** —— 提供以下选项：
     - 勾选 **Include entity level** 以包含原型的详细表示。
     - 勾选 **Exclude frames in united** 以从联合表示中排除帧（frames）。
     - 勾选 **Exclude 2D in united** 以从联合表示中排除 2D 对象。
     - 勾选 **Skip when JT file exists under the CO** 以跳过在 co 下已存在 jt 的组件。
   - **Keep current format and update materials only（保留当前格式，仅更新材料）** —— 此选项不执行完整升级。它在保留当前 `.co` 或 `.cojt` 格式的同时，用 Robcad 材料更新所选组件。它针对已升级为 `.co` 或 `.cojt` 格式的组件。

   > **注意（Note）**
   > - Keep current format and update materials only 不对原生 JT 组件进行操作。
   > - 如果输入 `.co` 或 `.cojt` 文件夹下没有嵌套的 `.jt` 文件，则无法运行 Keep current format and update materials only 选项。请对此数据运行完整升级。
   > - 选择 Keep current format and update materials only 选项时，对话框中除 Clean interval 外的所有其他选项均被禁用。

4. 勾选并配置以下高级选项。它们均为可选项：
   - **Approximation（近似）**
     - **Force upgrade approximation** —— 重新创建联合表示近似。此选项耗时较长。
     - **LOD ratio** —— 用于设置基础级别（level 0）与下一级别（level 1）之间的细节级别比率（level of details ratio）。LOD 值必须在 0.01 到 0.99 之间，精确到两位小数。输入无效值将禁用 OK 按钮。如果清除此选项，系统使用 LOD 比率 0.5。
   - **General（常规）**
     - **Clean interval** —— Upgrade CO Prototypes to Version 在其常规流程中会运行 Upgrade To Version 管理工具。升级许多大型组件时，需要不时重新启动 Upgrade To Version 管理工具。勾选 Clean interval 可自定义在当前版本中升级的组件数量，达到该数量后自动重新启动 Upgrade To Version 外部应用程序进程。该值必须在 1–100 之间。输入无效值将禁用 OK 按钮。如果清除此选项，默认 Clean interval 值为 50。有关更多信息，请参阅 Upgrade To Version 管理工具中的 Clean Interval 参数。注意：Clean Interval 不会反映在进度条文本“Upgrading component 1 out of x...”中，也不会反映在日志文件中（同时显示当前正在处理的组件）。
     - **Exclude material definitions（排除材料定义）** —— 默认情况下，Upgrade CO Prototypes to Version 执行 Update Materials 以将所选组件的 Robcad 材料导入升级后的组件。

   > **注意（Note）**
   > 在任何阶段，将鼠标悬停在帮助图标 上可显示各设置的工具提示。

5. 单击 **OK** 执行升级并保存最后的升级设置（包括对话框的展开/折叠状态）。

   > **注意（Note）**
   > 系统会警告您这是一个耗时的过程。单击 **Yes** 继续。

转换所选部件或资源的 3D 表示文件随之开始。将出现进度条，指示当前正在执行的升级。

将 `.CO` 转换为 `.COJT` 时，会发生以下情况：

- 对于每个原型，在与原型 `.CO` 文件相同的目录中创建一个 `.COJT` 文件。
- 系统保留源 `.CO` 文件在目录中（不会被删除）。
- 应用程序将 `.COJT` 文件指定为原型的 3D 文件。系统在 Properties Viewer 的 **Physical** 选项卡中的 **3D File** 字段显示该文件。

转换完成后，将出现一个对话框，使您能够查看日志文件。

> **注意（Note）**
> 如果选择了 Keep current format and update materials only 选项，该消息与更新材料相关。

未在日志中升级的组件列出于以下原因：

- 输入文件夹不是组件（没有 `.co` 或 `.cojt` 扩展名）。
- 组件在另一个应用程序中打开。
- 组件被另一用户检出。

此外，未更新的组件列出于以下原因：

- `.co` 或 `.cojt` 文件夹中没有 JT 文件。
- JT 文件源自原生 CAD 应用程序。
- JT 文件已用 Robcad 材料更新。

将 `.CO` 转换为 `.CO` 时，现有 `.CO` 被覆盖，并在 `.CO` 组件下创建一个 JT 文件。

6. 执行以下操作之一：
   - 单击 **Yes** 查看日志文件。
     日志文件在默认文本查看器中显示，包含以下信息：
     - 用于转换原型的命令行。
     - 任何转换失败的原因。
     - 被其他用户检出的原型数量。
     - 显示 `UpgradeToVersion.exe` 日志。其中包含每个输入组件状态摘要，以及当前 Upgrade To Version 调用中正确转换的组件数量摘要。
     - 将 `.CO` 转换为 `.COJT` 时，还提供以下附加信息：
       - 已转换的原型数量。
       - 未转换的原型数量。
       - 被其他用户检出的原型数量。

     > **注意（Note）**
     > 被其他用户检出的原型的 3D 表示文件不会被转换。
   - 单击 **No** 关闭对话框而不查看日志。

<a id="v10-s23"></a>
<!-- p1184 -->
## Bounding Box Calculation（边界框计算）

**Bounding Box Calculation** 命令在 Process Designer 应用中可用。它使您能够围绕所选择的复合体（compound）、资源（resource）或部件（part）计算一个虚拟边界框（bounding box），然后在 Process Designer 的 **Physical** 选项卡上显示计算结果。边界框的 3D 表示可在 Process Designer 中查看，并在执行邻近搜索（Neighboring Search）时用于查找并显示跨越边界框体积任意部分的其他相关对象。

> **注意（Note）**
> 边界框计算只能从 Process Designer 的资源树（Resource Tree）、产品树（Product Tree）、ResourceLibrary 节点或 ResourceLibrary 内的节点执行。此外，每个复合体、资源或部件只能执行一次此过程，且应由系统管理员执行。

可以在 Process Designer 中为复合体以及资源和部件的原型/实例（prototypes/instances）计算边界框，如下：

- 复合体的边界框涵盖所有相关子项（实例）的组合体积。因此，复合体边界框的 X、Y 和 Z 参数直接受复合体子项（实例）边界框位置的影响。
- 资源或部件的原型/实例的边界框涵盖紧邻原型/实例周围的体积。原型边界框的 X、Y 和 Z 参数与分配给该原型的组件自身原点（self-origin）相关。因此，由于实例从其原型继承边界框参数，每个实例的边界框参数始终与其原型的边界框参数相同，且不受实例位置的影响。

当您选择复合体、资源或部件并计算其边界框时，系统会自动执行并显示计算，其中包括虚拟边界框的最小点（minimum point）、最大点（maximum point）和尺寸（dimensions）的 X、Y 和 Z 参数。

在 Graphic Viewer 中，根据您从中打开 Process Designer 应用的资源树或产品树节点，可以查看复合体和实例的边界框。

**在 Process Designer 中计算边界框：**

**过程（Procedure）**

1. 在 Process Designer 应用中，在资源树或产品树中选择所需节点（复合体、资源或部件）。
2. 选择 **Preparation** 选项卡→**Libraries** 组→**Bounding Box Calculation**。将显示以下确认窗口：
3. 单击 **Yes**。系统自动对所选节点（复合体、资源或部件）及任何相关的子树节点执行边界框计算。结果显示在 Process Designer 和 Process Designer 的 **Physical** 选项卡中，包括虚拟边界框的最小点、最大点和尺寸的 X、Y 和 Z 参数。

---

<a id="v10-s24"></a>
<!-- p1186 -->
## Application Settings（应用设置）

<a id="v10-s25"></a>
<!-- p1186 -->
### Creating Compare Configurations（创建比较配置）

您可以创建在 Compare Viewer 中使用的预定义配置（predefined configurations）。每个配置可包含要在所选备选方案（alternatives）之间比较的不同属性和关系（relations）。创建配置后，可以使用 Compare Viewer 工具栏选择要在当前比较中使用的配置。使用此对话框需要 Create Compare Configuration 权限。

例如，数据可能包含成本信息，而某个备选方案可能省略了该信息。在这种情况下，将该备选方案与其范围（scope）比较时，您可以使用一个比较配置，省略对所有成本属性的比较，以免用与您无关的差异充斥 Compare Viewer。

**过程（Procedure）**

1. 单击 。将出现 Compare Configurations 对话框。
2. 单击 **Create**。对话框中新增一行。
3. 为新配置输入 **Name**，可选择性地添加 **Description**，然后单击 **Edit**。将出现 Edit Configuration 对话框。

4. 在 **Attributes** 区域，要么在比较中包含全部属性，要么按照 Using the Field Selector 中的说明选择要包含在比较中或从比较中排除的属性。
5. 在 **Relations** 区域，要么在比较中包含全部关系，要么按照 Using the Field Selector 中的说明选择要包含在比较中或从比较中排除的关系。
6. 在 **Colors** 区域，您可以指定已更改和/或已移动的对象以颜色标识，便于识别。还有一个选项，可在其父项未更改的情况下不对已移动对象着色。
7. 您还可以勾选 **Two stage relations（两段关系）**，并选择在比较中要包含的两段关系。

8. 在 Edit Configuration 对话框中单击 **OK**。
9. 单击 **OK** 关闭 Compare Configurations 对话框。

---

<a id="v10-s26"></a>
<!-- p1188 -->
### Alternative Configurations（备选方案配置）

系统管理员可以创建和编辑备选方案的配置。这些配置在用户更新主对象（master objects）和克隆备选方案时可用，请参阅 General Update。

**创建备选方案配置：**

**过程（Procedure）**

1. 单击 **Alternative Configurations**。将出现 Alternative Configurations 对话框。
2. 执行以下操作之一：
   - 单击 **Create** 添加新配置，为配置指定一个有意义的 **Name**，并根据需要添加 **Description**。
   - 选择现有配置并单击 **Edit** 将其打开。
   将出现 Edit Configuration 对话框。

3. 配置 **Settings** 参数，如下：
   - **Update attributes（更新属性）** 默认勾选。如果要阻止更新数据属性，请清除它。
   - 如果要配置从更新中排除的属性，请单击 **Attribute Configuration**。请参阅 Configuring Attribute Exclusions from General Update。
   - **Update engineering data（更新工程数据）** —— 克隆（clone）研究文件夹（study folder）及其下方的研信息。此选项还会克隆系统根下的连接 TuneCell 文件夹，并替换到克隆对象的连接信息。所创建的 TuneCell 与其备选方案中克隆研究的 externalID 相同。它可通过 externalID 样式识别，该样式与手动创建对象的 ID 约定不同。
     - 常规 externalID > `PP-OracleSchemaName-date-time`
     - 备选 externalID > `PP-GenericGeneratedID`
   - **Update simulation info（更新仿真信息）** —— 克隆在 Process Simulate 中创建的所有仿真信息，如仿真对象、仿真事件、PLC 数据等。此操作字段包含仿真数据（包括 ID 等），在克隆属于仿真研究一部分的操作时需要更新。
   - **Check In Before Update（更新前检入）** —— 必须勾选此选项以启用回滚（roll-back，取消检出）。
   - **Check In After Update（更新后检入）**

   > **注意（Note）**
   > 检入和检出非常耗时，尤其对于大型结构。

4. 配置 **Structural Updates（结构更新）** 参数，通过清除 **Update node structure（更新节点结构）**（清除所有嵌套参数），或勾选 Update node structure 并配置以下参数：
   - **Update resource assignments（更新资源分配）** —— 更新资源对非同步操作（non-synchronized operations）的分配。

     > **注意（Note）**
     > 单击 **Exclude** 配置 Update resource assignments 的例外。请参阅 Configuring Resource Assignment Exclusions from General Update。

   - **Update variant assignments（更新变体分配）** —— 更新变体对操作、资源以及所有其他可附加变体的对象的分配。

     > **注意（Note）**
     > 单击 **Exclude** 配置 Update variant assignments 的例外。请参阅 Configuring Variant Assignment Exclusions from General Update。

   - **Update synchronized operations（更新同步操作）** —— 更新同步操作对工位的分配。
   - **Update MfgFeature assignments to parts（更新 MfgFeature 对部件的分配）** —— 更新 MfgFeature 对部件的分配。
   - **Recreate deleted nodes（重新创建已删除节点）**

     > **注意（Note）**
     > 此选项仅在针对单个备选方案（Alternative）执行更新时可用。未启用此选项时，系统在更新期间创建新对象，但不重新创建已删除的节点。

   - **Delete out of scope root objects（删除超出范围的根对象）** —— 勾选时，即使源对象（更新备选方案情况下的主对象 / 更新主对象情况下的克隆对象）被删除或移出范围，也阻止根对象被删除。
   - **Align clone with original（使克隆与原始对齐）** —— 勾选时，更新原始对象的结构以使其与备选方案匹配。这包括删除（移动到用户文件夹）原始对象中新创建的对象，并将对象移动到其原始父项下。如果原始对象中新创建的对象被删除，其关联对象（如 usages 和 flows）也会被删除。
   - **Apply traversal restrictions（应用遍历限制）** —— 超出范围的已分配部件不会被跟随，即不会被备选方案管理导出。这可以显著提升性能。

     > **注意（Note）**
     > 使用此选项时，用户有责任将所有所需部件直接包含在备选方案范围内，因为它们不会通过流程上的遍历被累积。

5. 单击 **OK**。
6. 如果要删除配置，请选择该配置并单击 **Delete**。
7. 单击 **OK**。

---

<a id="v10-s27"></a>
<!-- p1192 -->
## Line Balancing（产线平衡）

<a id="v10-s28"></a>
<!-- p1192 -->
### Line Balancing Settings（产线平衡设置）

Line Balancing Settings 窗口包含 LB 的默认设置，包括 MLB（手动产线平衡）和 ALB（自动产线平衡）。对于 LB，这包括可为每个操作和资源配置的每个标准（criteria）的可选值，以及在将操作分配到工位（stations）时应预定义为组约束（group constraints）的复合操作组类型（compound operation group types）。

要访问 Line Balancing Settings 对话框，请单击 。

<a id="v10-s29"></a>
<!-- p1192 -->
#### Boundary Conditions Tab（边界条件选项卡）

<a id="v10-s30"></a>
<!-- p1192 -->
##### Criteria Definition（标准定义）

默认情况下，LB 中包含的每个操作都可以分配到任意工位。但是，您可以访问特定操作的属性中的 Line Balancing 选项卡，并根据需要为 Line Balancing Settings 窗口中定义的标准选择特定值。对于每个资源，您可以为每个标准选择一个或多个值。这些值作为可分配到每个工位的操作的约束。例如，如果操作 A 属于某个 Type，则只能在支持该 Type 的工位上执行。

要定义标准，请执行以下步骤：

**过程（Procedure）**

1. 在 Boundary Conditions 选项卡的 **Criteria** 区域，在 Criterion 列表中定义一个条目。
2. 在 **Possible Values** 列表中为该标准输入一个或多个可能的值。每个值输入后按 Enter（键盘）。例如，您可以从标准列表中定义 Accessibility 为一个标准，并在 Possible Values 编辑框中输入 Left、Right、Front、Back、Above 和 Below。
3. 此窗口中列出的标准将出现在包含在产线平衡范围内的每个操作和资源的 Line Balancing 选项卡中。定义 LB 设置后，您可以为这些操作和资源中的每个选择适当的值。
4. 根据需要，对 Criterion 列表中的每个条目重复步骤 1 至 3。

**为操作和资源选择 LB 标准值：**

**过程（Procedure）**

1. 在 **Operation** 列表中，选择要包含在 LineBalancingScope 中的操作，并单击 **Line Balancing** 选项卡以显示以下对话框（示例）：
2. 从显示的列表中选择每个标准，并选择可能的值之一（在 Line Balancing Settings 窗口中定义），或保留默认 **Any** 设置。如果选择了 Any 以外的值，则该操作只能分配到支持此设置的资源。
3. 在 Resource Tree 中，选择要包含在 LineBalancingScope 中的资源。
4. 在 **Line Balancing** 选项卡中，为每个标准勾选一个或多个值旁边的复选框。
5. 对 LineBalancingScope 中包含的每个其他资源重复步骤 4。

<a id="v10-s31"></a>
<!-- p1195 -->
##### Target Utilization Level of a Station（工位的目标准利用率）

用户可以为每个工位定义不同的目标准利用率（target utilization level）。这种灵活性支持针对具有不同利用率水平的工位进行假设分析（what-if scenario analysis）。例如，用户可以选择仅利用某个特定工位的 50%，作为在该工位引入新技术时的一种安全措施。

如果用户在 ALB 参数中设置全局目标准利用率，则在结果存储后，该值会相应更新。

<a id="v10-s32"></a>
<!-- p1195 -->
##### Predefined Group Constraints（预定义组约束）

**Compound Operation Group Types（复合操作组类型）** —— 通过此设置，用户可以定义任何派生自 CompoundOperation 的数据模型类（data model classes），这些类默认将被视为组。

在 **Constraints** 区域，选择应被 LB 视为预定义组约束的复合操作子类型，如下：

**过程（Procedure）**

1. 单击 **Add** 以显示以下内容：
2. 从 **Operation sub-types** 下拉列表中选择一个复合操作类型，然后单击 **OK**。所选复合操作类型现在被定义为预定义组约束。

   > **注意（Note）**
   > 从下拉列表中选择 CompoundOperation 可将所有复合操作类型都包含为预定义 LB 组约束。

   通过将这些复合操作类型定义为组约束，LB 必须将复合操作的所有子项分配到同一工位。

3. 重复步骤 1 和 2，定义其他复合操作类型为预定义组约束。
4. 单击 **OK** 保存设置。

   > **注意（Note）**
   > 单击 **Import Settings** 可导入在外部 XML 文件中定义的 LB 设置。单击 **Export Settings** 可将 Line Balancing Settings 窗口中定义的设置导出到 XML 文件。

<a id="v10-s33"></a>
<!-- p1196 -->
#### Global Tab（全局选项卡）

ALB 的具体定义可以在设置对话框的 Global 选项卡中完成。

<a id="v10-s34"></a>
<!-- p1197 -->
##### Global Objects（全局对象）

在这些设置中，ALB 管理员用户定义应用所需的默认全局对象。

- **Active Resource（活动资源）** —— 在某些场景中，ALB 应用会建议向工位添加活动资源。用户可以显式激活 LB 中的一个工具栏命令来添加所需的活动资源。此字段定义哪些资源将作为活动资源添加到工位。此资源原型的选择通过项目导航器（project navigator）视图完成。

  **打开项目导航器视图：**

  **过程（Procedure）**

  1. 单击 **Generic Resource** 字段旁的 **Browse** 按钮（...）。
  2. 逐层展开树，直到找到所需的 Resource Library 对象。

  3. 双击所需的 Resource Library 对象；窗口中的视图将更改为反映所选 Resource Library 的内容。
  4. 逐层展开，直到找到所需资源。
  5. 选择所需资源。
  6. 单击 **Apply**。

  > **注意（Note）**
  > 由于活动资源定义需要从特定项目中选择一个原型，所有全局设置仅对特定项目有效。此外，只有在 Process Designer 中已打开项目后，浏览器窗口才会显示 Process Designer 项目树。

- **Station Object（工位对象）** —— 必须定义表示项目中工位的数据库模型类。要设置/更改此设置，ALB 管理员用户需要从弹出菜单中选择适当的类。弹出菜单将显示所有继承自 ProcessResource 的类。
- **Station Collector Object（工位收集器对象）** —— 还必须定义表示定义工位的范围（通常为区域 zone）的数据模型类。其定义方式与工位对象相同。
- **Variant Set Library（变体集库）** —— 指定包含所需变体的库。

**Variant Parameters（变体参数）**

- **Consider Optional Criteria（考虑可选标准）** —— 在计算变体表（variant table）时考虑可选标准。默认情况下，此设置关闭。
- **Consider Variant Rules（考虑变体规则）** —— 在计算变体表时考虑变体规则。默认情况下，此设置关闭。

<a id="v10-s35"></a>
<!-- p1199 -->
##### Additional Parameters（附加参数）

- **Required Active Resources as Integer（所需活动资源为整数）** —— 默认情况下，所需活动资源值以双精度浮点值计算和显示。此选项以整数计算和显示它们。
- **Show Export Dialog（显示导出对话框）** —— 配置 XML 导出功能的行为。默认情况下，执行 XML 导出会自动将产线平衡设置保存到默认位置。选择此选项后，执行 XML 导出将显示一个对话框，用于指定 XML 文件的文件名和位置。
- **Block Assignment from other Scopes（阻止来自其他范围的分配）** —— 防止将对象从其他范围拖放（drag and drop）到 Line Balancing 视图。选择此选项后，只能添加定义在 Line Balancing Scope 中的对象。

<a id="v10-s36"></a>
<!-- p1199 -->
#### Automatic Line Balancing Tab（自动产线平衡选项卡）

Automatic Line Balancing 选项卡提供自动产线平衡的配置选项。

<a id="v10-s37"></a>
<!-- p1199 -->
##### New Station Template（新工位模板）

在 ALB 的某一种工作模式中，优化过程能够向现有产线添加额外的工位。这些工位的模板可以在设置对话框中定义。

- **Station Objects（工位对象）** —— 使用此设置定义将由 ALB 创建的每个新工位所包含的资源，即将自动包含在工位中的对象。这些资源不是“活动资源”，将创建在工位下，作为“活动资源”的补充。
- **Station Name（工位名称）** —— 使用此设置定义新建工位的前缀。
- **Station Length（工位长度）** —— 使用此设置在 ALB 设置中定义工位的长度（例如 7000 mm）及其所参照的方向。每个工位的位置（Physical 选项卡属性）将在工位创建后自动定义。此功能支持流程的下游布局创建。例如，如果定义的值为 x 方向 7000 mm：
  - 第一个工位将在 0,0,0 创建。
  - 第二个工位将在 7000,0,0 创建。
  - 第三个工位将在 14000,0,0 创建，依此类推。

<a id="v10-s38"></a>
<!-- p1199 -->
##### Cycle Time Parameters（节拍时间参数）

这些设置将作为节拍时间（cycle time）的通用默认值。通用节拍时间值可以基于计算或直接定义：

- **Define mode（定义模式）** —— 用户直接定义工位的默认节拍时间（以秒为单位）。
- **Calculate mode（计算模式）** —— 用户定义若干参数，由此计算默认节拍时间：
  - 产线工作时长：TO [hrs]
  - 工作时长内产线需要制造的产量：V [units]
  - 产线性能率：RS [%]

  节拍时间（TC）计算如下：`TC = (TO / V) * RS`

  根据用户选择的选项，相应字段将启用/禁用：
  - Define —— 仅启用节拍时间字段。
  - Calculate —— 仅禁用节拍时间字段。

<a id="v10-s39"></a>
<!-- p1200 -->
##### e-MOP Integration（e-MOP 集成）

用户可以定义一个中间层次级别，作为工位的子项，操作应分配到该子项而非工位。

如果本应用的部署是加工（Machining）解决方案部署的一部分，则 ALB 将以独特模式工作：支持 e-MOP Integration。当用户选择一个设置对象（类型为 PrSetup）时，切换到此模式。

在此模式下：

- 每个工位将有一个子项 —— 一个设置对象。
- 如果 ALB 提议一条新产线，它将在每个工位下仅创建一个设置（setup）。否则，工位下支持任意数量的设置。如果范围内存在没有设置的工位，应用将自动创建一个并将操作分配给它。
- 操作将分配到设置级别对象（而非工位）。
- 因此，ALB 应用将通过定义所需操作为设置对象的子项来分配操作。
- 应用将平衡设置而非工位：
  - “活动资源”将创建在设置级别下。
  - Resource Tree 中的约束将定义在设置对象上，而非工位上。
  - 计算出的节拍时间用于设置，而非工位。
- 应用分析设置之间以及工位之间的顺序，以确保提议的解决方案满足前序约束（Precedence Constraints）。

<a id="v10-s40"></a>
<!-- p1201 -->
##### File Attachments（文件附件）

用户可以定义在系统根下、ALB 结果附件文件（报告和图表）将存储的目录。随后 ALB 附件将存储在此目录下一个名为“ALB”的目录中。如果未指定目录，报告将存储在系统根下直接以硬编码名称“ALB”命名的目录中。

此功能支持文件存储的灵活性 —— 这很重要，例如，在支持多站点（multi-site）环境工作时 —— 避免一个 Process Designer 项目的附件覆盖另一个项目的附件。

<a id="v10-s41"></a>
<!-- p1201 -->
### ALB Settings in the ALB Application（ALB 应用中的 ALB 设置）

对于每个 ALB 会话（或场景 scenario），用户能够定义一组不同的参数。单击 **Parameters** 按钮 将打开以下视图（示例）：

<a id="v10-s42"></a>
<!-- p1201 -->
#### ALB Parameters View Buttons（ALB 参数视图按钮）

- **Load Parameters（加载参数）** 按钮根据上次存储在当前 LineBalancingScope 对象上的参数配置更新参数。
- **Reset（重置）** 按钮将参数值恢复为默认值（在 ALB 设置中定义的值）。
- **OK** 按钮关闭视图，保留任何已更改的选择。
- **Cancel（取消）** 按钮关闭视图，取消自上次打开以来所做的任何更改。
- **Define Cycle Time（定义节拍时间）** 按钮打开节拍时间定义视图。此按钮仅在其旁边的“Use calculated cycle time”复选框被选中后才启用。

如果用户单击 Load Parameters 或 Reset 按钮，将出现类似以下的消息：

<a id="v10-s43"></a>
<!-- p1202 -->
#### Dominant Goal Function Setting（主导目标函数设置）

用户可以指定在优化过程中使用两个目标函数（goal functions）中的哪一个：

- **Minimum error function（最小误差函数）** —— 力求各工位利用率（百分比）之间偏差最小，并尽力不超过工位利用率。例如，使用此函数可能导致以下类型的解决方案：
  - 所有工位利用率水平都在 50% 左右，而用户指定的目标利用率水平为 95%。
  - 某些工位利用率水平高于目标利用率水平。
  此目标函数为默认函数。
- **Minimum number of stations（最小工位数）** —— 此选项仅应在特定情况下使用，即用户认为通过不使用所有工位即可良好平衡产线。通常，用户只有在以先前设置运行优化过程并分析结果后，才能作出此假设（即不使用所有工位也能良好平衡）。对于此目标函数，ALB 不一定会提议将操作分配到范围内的所有工位。

<a id="v10-s44"></a>
<!-- p1202 -->
#### Global Optimization Parameters Definition（全局优化参数定义）

<a id="v10-s45"></a>
<!-- p1203 -->
##### Utilization Level（利用率水平）

全局利用率水平在 MLB 窗口中定义。此设置可被 ALB 设置覆盖。

用户能够定义目标工位利用率水平（以百分比计）。默认值为 100%：

- 用户可以指定小于 100% 的值。
- 用户可以指定大于 100% 的值。如果指定了这样的值，它将以红色显示。

> **注意（Note）**
> 仅允许整数值。如果用户指定了非整数值，它将自动四舍五入到最接近的整数。

<a id="v10-s46"></a>
<!-- p1203 -->
#### Use Pre-Allocated Operations Option（使用预分配操作选项）

用户可能出于各种原因希望在运行 ALB 模块之前将操作分配到工位。单个操作到工位的分配可以使用标准 Process Designer UI 或使用 MLB 模块完成。因此，用户能够指定是否考虑已分配的操作。默认行为是考虑已分配的操作，但用户可以通过取消勾选“Use pre-allocated operations”复选框来更改它。

关于工位-操作约束（标准）和操作间约束的预分配操作冲突验证，将在优化过程开始运行时立即进行。

如果预分配操作与分配到的工位的标准冲突，用户将收到通知，并可以执行以下操作之一：

- 停止优化过程并修改数据，以消除冲突。
- 继续优化过程。在这种情况下，任何冲突的预分配操作将被重新分配到有效工位。

<a id="v10-s47"></a>
<!-- p1203 -->
#### Use Calculated Cycle Time Option（使用计算节拍时间选项）

用户能够定义在优化过程中考虑哪个节拍时间：

- 为每个工位定义的节拍时间。这是默认行为。
- 通用计算/定义节拍时间。仅当选择此选项时，**Define Cycle Time** 按钮才启用。

用户可以通过两种方式为每个产线平衡范围定义通用节拍时间值：

- 直接定义节拍时间。
- 基于以下对话框中所示的参数计算节拍时间。

<a id="v10-s48"></a>
<!-- p1204 -->
#### Use Active Resources Option（使用活动资源选项）

当选择 Use Active Resources 选项时，将考虑每个工位中存在的“活动资源”数量。

当未选择此选项时，仅考虑每个工位允许的节拍时间（类似于每个工位有一个“活动资源”的情况）。

<a id="v10-s49"></a>
<!-- p1204 -->
#### Generic Resource Definition（通用资源定义）

选择 Generic Resource 定义使用户能够定义一种原型，在需要向工位添加/创建“活动资源”时，应用将创建该原型的实例。

参数对话框中显示的默认资源原型是在全局（ALB 设置）中定义的资源原型。但是，用户能够从任何资源库树定义不同的资源原型。

<a id="v10-s50"></a>
<!-- p1205 -->
#### Adding Active Resources Option（添加活动资源选项）

选择“Do not add active resources”选项指定用户不允许在提议解决方案中包含额外的“活动资源”。此选项是必需的，因为有时流程规划人员由于各种原因（例如现有产线的布局限制等）无权更改工位中的活动资源数量。

<a id="v10-s51"></a>
<!-- p1205 -->
#### Define Balancing Parameters（定义平衡参数）

用户能够选择要考虑的操作/工位参数。

<a id="v10-s52"></a>
<!-- p1205 -->
#### Optimization Time Definitions（优化时间定义）

用户能够以分钟和分钟的小数定义优化过程时间。最短时间为 0.1 分钟（6 秒）。默认时间为 3 分钟。

用户能够以秒为单位定义时间间隔参数。默认时间间隔为 60 秒。

如果在此期间内优化过程未找到更好的解决方案，用户将收到以下消息：

示例：

- 用户将优化过程时间定义为 10 分钟。
- 用户将时间间隔定义为 50 秒。
- 优化过程开始运行：第一个解决方案在 20 秒后找到，第二个解决方案在 40 秒后找到，而在 90 秒后优化过程尚未找到第三个解决方案。在这种情况下，优化过程在 90 秒后以上述提示停止运行。用户可以选择继续优化过程。如果用户决定继续，那么如果在 140 秒后仍未找到另一个解决方案，用户将再次收到提示。

<a id="v10-s53"></a>
<!-- p1206 -->
#### Numeric Check Tab（数值检查选项卡）

数值检查（numeric check）是在将操作分配到工位时要考虑的附加平衡方面。例如，除了节拍时间之外，用户可以指定每个操作所需的物流空间（logistic space）以及每个工位可用的物流空间。数值检查是硬约束（hard constraint）。

需要进行以下定义：

- **Station Attribute（工位属性）**：定义在工位上存储该值的属性。
- **Operation Attribute（操作属性）**：定义在操作上存储该值的属性。
- **Combining Function（组合函数）**：sum、minimum 或 maximum。它定义如何组合已分配操作的值以计算工位的数值内容。
- **Comparison Operator（比较运算符）**：`<=`（小于或等于）、`>=`（大于或等于）或 `=`（等于）。它定义如何将工位的数值内容与工位值进行比较。
- **Name of Sum of Operation Attributes（操作属性之和的名称）**：即数值内容的名称。

例如，将所有已分配操作的值求和，并检查该和是否小于或等于工位的属性值。

数值检查的操作字段、计算出的数值内容以及剩余内容将显示在 Line Balancing 窗口的附加列中。

<a id="v10-s54"></a>
<!-- p1207 -->
#### LB Settings Import / Export（LB 设置导入/导出）

全局设置存储在应用数据表（application data tables）中。此选项通过使用 XML 文件格式的导出/导入机制，支持在一个 Process Designer 模式（scheme）与另一个模式之间轻松重用此信息。创建的文件使用以下硬编码名称：`ALB_Setting_<project name>.xml`。

设置可能包含属于特定 Process Designer 项目的某些对象（例如通用资源）。在这种情况下，导入到新项目时，用户会收到关于需要当前项目中重新定义的不相关对象的适当消息。

<a id="v10-s55"></a>
<!-- p1207 -->
## Administration Tool（管理工具）

<a id="v10-s56"></a>
<!-- p1207 -->
### Assembly Module Administration Tool（装配模块管理工具）

Assembly Module Administration Tool 包含各种 Assembly Module（装配模块）应用设置。只有管理员用户才能修改这些设置。

- **Temporary Files Path（临时文件路径）** —— 定义 Assembly Module 创建的临时文件的存储位置。典型的 NFS 位置是 Windows 临时目录。此选项仅与 BOM 复制（duplication）和从 BOM 更新命令（Creating Pre-Assembly Tree from BOM）相关。
- **Create Copied Mfgs（创建复制的 Mfgs）** —— 确定在 BOM 复制期间，附加到 BOM 部件的 Mfgs 是否也会被复制。如果未勾选此选项，附加到原始部件的 Mfgs 也会附加到新复制的部件。更多细节和示例请参阅 Create Copied Mfgs Option。此选项仅与 BOM 复制和从 BOM 更新命令（Creating Pre-Assembly Tree from BOM）相关。
- **Search for Parts in Sub Operations（在子操作中搜索部件）** —— 确定在 MBOM 复制期间，附加到子操作（sub Operations）的部件是否也会被复制。如果保留未勾选，附加到原始子操作的部件不会附加到新复制的工位。
- **Mid-level Hidden Type（中间层隐藏类型）** —— Assembly Module Administration Tool 中的此组合框（combo-box）显示所有装配类型以及字符串“none”（默认值）。当用户从组合框中选择一个装配类型（例如 AssemblyZone）时，该类型不会显示在 MBOM 中，而其后代（descendants）保持不变。
- **Lowest Level Hidden Type（最低层隐藏类型）** —— Assembly Module Administration Tool 中的此组合框显示所有装配类型以及字符串“none”（默认值）。当用户从组合框中选择一个装配类型（例如 AssemblyStation）时，该类型及其后代都不会显示在 MBOM 中。

---

<a id="v10-s57"></a>
<!-- p1211 -->
### Configuring Alternative Management Behavior（配置备选方案管理行为）

要自定义 Alternative Management（备选方案管理）行为，请打开 `<sysroot>\General\AlternativeManagement\AMMetadata.xml` 中的 `AMMetadata.xml` 文件。

- 系统首先在客户端系统根（client system root）中查找此文件，如果未找到，则从服务器系统根（server system root）获取。
- 如果未找到该文件，则应用内部默认值。
- 可以动态自定义用于编辑配置/配置更新的 UI。
  - 用于结构更改（structural changes）的选项的 UI 可以自定义。
  - 对于每个选项，可以声明：
    - 该选项的默认值是什么。
    - 该选项是否在 UI 中可见 —— 对于所有在 UI 中不可见的选项，使用默认值。

示例：

```xml
<?xml version="1.0" ?>
<AMMetaData>
<StringRelation FieldName="LibrarySupplyChains" ClassName="LogPlant"/>
<SkipRelation FieldName="operatesOn" ClassName="MfgFeature"/>
<Option Name="CloneCommands.UpdateResourceAssignments" ShowInUI="1" 
Default="0" MayBeChanged="1"/>
<Option Name="CloneCommands.UpdateOperationAssignments" ShowInUI="1" 
Default="0" MayBeChanged="1"/>
<Option Name="CloneCommands.UpdateMfgAssignments" ShowInUI="1" Default="0" 
MayBeChanged="1"/>
<Option Name="CloneCommands.UpdateOperationSequence" ShowInUI="1" 
Default="0" MayBeChanged="1"/>
<Option Name="CloneCommands.UpdateVariantAssignments" ShowInUI="1" 
Default="0" MayBeChanged="1"/>
<Option Name="CloneCommands.RecreateDeletedNodes" ShowInUI="1" Default="0" 
MayBeChanged="1"/>
<Option Name="CloneCommands.DeleteImportFolder" ShowInUI="1" Default="0" 
MayBeChanged="1"/>
<Option Name="CloneCommands.DeleteOutOfScopeRootObjects" ShowInUI="1" 
Default="0" MayBeChanged="1"/>
<Option Name="CloneCommands.DeleteNewObjects" ShowInUI="1" Default="0" 
MayBeChanged="1"/>
</AMMetaData>
```

将 `ShowInUI` 设为 0 将使复选框消失，将 `MayBeChanged` 设为 0 将使复选框变为只读。将 `DefaultValue` 设为 1 将勾选复选框。

示例：

```xml
<?xml version="1.0" ?>
<AMMetaData>
<StringRelation FieldName="LibrarySupplyChains" ClassName="LogPlant"/>
<SkipRelation FieldName="operatesOn" ClassName="MfgFeature"/>
<Option Name="CloneCommands.UpdateResourceAssignments" ShowInUI="1" 
Default="1" MayBeChanged="0"/>
<Option Name="CloneCommands.UpdateOperationAssignments" ShowInUI="1" 
Default="1" MayBeChanged="0"/>
<Option Name="CloneCommands.UpdateMfgAssignments" ShowInUI="1" Default="0" 
MayBeChanged="1"/>
<Option Name="CloneCommands.UpdateOperationSequence" ShowInUI="1" 
Default="0" MayBeChanged="1"/>
<Option Name="CloneCommands.UpdateVariantAssignments" ShowInUI="1" 
Default="0" MayBeChanged="1"/>
<Option Name="CloneCommands.RecreateDeletedNodes" ShowInUI="1" Default="1" 
MayBeChanged="1"/>
<Option Name="CloneCommands.DeleteImportFolder" ShowInUI="1" Default="1" 
MayBeChanged="1"/>
<Option Name="CloneCommands.DeleteOutOfScopeRootObjects" ShowInUI="0" 
Default="0" MayBeChanged="1"/>
<Option Name="CloneCommands.DeleteNewObjects" ShowInUI="0" Default="0" 
MayBeChanged="1"/>
</AMMetaData>
```

---

<a id="v10-s58"></a>
<!-- p1213 -->
### Task Supervisor Administration（任务管理器管理）

<a id="v10-s59"></a>
<!-- p1213 -->
#### Defining Task Status Options（定义任务状态选项）

在定义可在 Task Supervisor 中跟踪的任务之前，必须定义可在任务推进到完成时应用于任务的状态值（status values）。任务状态的一些示例有：Open、High Priority 和 Closed。为每个状态分配一种颜色以便识别。可以根据需要随时添加其他状态。

**添加新状态：**

**过程（Procedure）**

1. 单击 。将出现 Task Supervisor Administration 对话框，顶部为 **Status Options** 选项卡。将显示当前定义的状态值。
2. 单击 **Add**。在最后一行状态下方添加一个空行。
3. 双击新行的左列并输入新状态的名称。
4. 双击新行的右列。将出现标准调色板。
5. 单击新状态所需的颜色。
6. 选择一个状态并单击 **Remove** 以删除状态。

定义状态后，您可以在任务模板（task templates）中使用它们。

<a id="v10-s60"></a>
<!-- p1215 -->
#### Defining Task External Users（定义任务外部用户）

您可以添加未在 eMServer 中定义的用户。当您将任务分配给组织外部（未在 eMServer 中注册）的用户时，这很有用。

**过程（Procedure）**

1. 单击 。将出现 Task Supervisor Administration 对话框。
2. 单击 **External Users Management** 选项卡。将显示当前定义的外部用户。
3. 单击 **Add**。在最后一个外部用户下方添加一个空行。
4. 根据需要编辑新用户的参数。
5. 选择一个外部用户并单击 **Remove** 以删除用户。

定义外部用户后，您可以在任务模板中使用它们。

<a id="v10-s61"></a>
<!-- p1216 -->
#### Defining Task Mail Configuration（定义任务邮件配置）

您可以配置 Task Supervisor，使其在发送给任务分发表（task distribution lists）中用户的邮件中包含带有其值的系统属性。

**过程（Procedure）**

1. 单击 。将出现 Task Supervisor Administration 对话框。
2. 单击 **Mail Configuration** 选项卡。
3. 在左列中选择要包含在邮件中的属性，然后单击 。所选属性将列在右列中，并在左列中高亮显示。
4. 在右列中选择要从邮件中删除的属性，然后单击 。
5. 在右列中选择一个属性并单击 **Up** 或 **Down** 以排列邮件中属性的顺序。

   > **注意（Note）**
   > 您可以单击列标题中的箭头，按字母顺序对两列中的属性进行排序。

6. 在下方面板中，您可以为任务和历史通知邮件配置主题（subject）、页眉（header）和页脚（footer）。

7. 单击 **Save**。

<a id="v10-s62"></a>
<!-- p1218 -->
#### Managing the Task/History Display（管理任务/历史显示）

您可以配置如何在 Task Supervisor 的 Task 和 History 区域中显示信息。

**配置 Task Supervisor 显示：**

**过程（Procedure）**

1. 单击 。将出现 Task Supervisor Administration 对话框。
2. 单击 **Task/History columns manager** 选项卡。
3. 在 **Task Columns** 列表中，勾选要显示的列并清除要隐藏的列。通过双击 Column Title 单元格，您可以自定义列的名称。
4. 在 **History Columns** 列表中，勾选要显示的列并清除要隐藏的列。
5. 在任一列表中，选择一个条目并单击 或 以设置所需的列顺序。
6. 单击 **Update All Users** 将设置应用于该模式（schema），或单击 **Get Customization Data** 将值重置为默认设置。
7. 单击 **Save**。

<a id="v10-s63"></a>
<!-- p1219 -->
#### Managing Task Preferences（管理任务首选项）

**配置 Task Supervisor 首选项：**

**过程（Procedure）**

1. 单击 。将出现 Task Supervisor Administration 对话框。
2. 单击 **Preferences** 选项卡。
3. 配置以下参数：
   - **Add owner and assigned user to User List automatically（自动将所有者和被指派用户添加到用户列表）** —— 自动将任务所有者和被指派用户包含到任务和历史通知邮件中。
   - **Send History Notes mail automatically（自动发送历史备注邮件）** —— 自动发送有关任务中最新更新的通知邮件。
   - **Show in "Selected Users" window Active Users only（仅在“Selected Users”窗口中显示活动用户）** —— 在配置任务或将任务分配给用户时，显示活动 eMServer 用户（并隐藏非活动用户）。
   - **Show in "Selected Users" window Distribution List users only（仅在“Selected Users”窗口中显示分发表用户）** —— 显示在分发表中配置的 eMServer 用户，并隐藏 Assigned to 对话框中的其他用户。
   - **Automatic Check Out Task Items when modified（修改时自动检出任务项）** —— Task Supervisor 对任务项执行自动检出。
   - **User name format（用户名格式）** —— 使您能够为所有任务对话框选择用户名格式。
   - **Mail server name（邮件服务器名称）** —— 使您能够输入邮件服务器，Task Supervisor 通知机制将访问该服务器以独立于客户端的电子邮件系统发送邮件。
4. 单击 **Save**。
<a id="v11-s1"></a>
<!-- p1221 -->
# 11. 附加命令（Additional Commands）

<a id="v11-s2"></a>
<!-- p1221 -->
## 创建替代方案（Create Alternative from Study）

创建替代方案（Create Alternative from Study）命令利用已签出（checked-out）的研究（study）或多项研究来创建一种结构，您可基于该结构运行"创建/更新替代方案（Create/Update Alternatives）"以生成真正的替代方案（alternative）。此过程旨在节省时间和精力。

该命令会创建 Alternative（替代方案）和 AlternativeScope（替代方案范围）节点，并将源研究复制到 AlternativeScope 节点下。

**注意**
- 创建替代方案（Create Alternative from Study）命令不会创建替代方案（alternative）。
- 运行此命令需要替代方案（alternatives）许可证。

**步骤**
1. 选择一个已签出的研究（或可多选多个已签出的研究）。
2. 选择 **Create Alternative from Study**（创建替代方案）。
   系统会在工作文件夹中为每一个所选研究创建一个 Alternative，并自动填充它们。

**注意**
如果系统无法打开某个或多个多选的研究（例如因为它们尚未签出或不可更改），它会为其他研究创建替代方案，并通知您那些未能创建的研究。

3. 通过在导航树中选择一个 Collection（集合）文件夹（而非库）来更改目标文件夹。
   - 启动创建替代方案（Create Alternative from Study）命令。
     对话框打开，当前定义的工作文件夹作为默认目标文件夹。选择不同的工作文件夹会将其设为目标文件夹，并显示在字段中。
   - 单击 **OK**（确定）。

<a id="v11-s3"></a>
<!-- p1223 -->
## 节点版本历史（Node Version History）

您可以查看和检查任何给定节点的版本历史以及本地保存（local save）记录，以供参考。

当节点处于选中状态时，单击相应图标或右键单击节点并选择 **Node History**（节点历史）。

本地保存记录旁以保存图标标记。

Process Designer 中当前打开的版本以蓝色对勾图标标记。

<a id="v11-s4"></a>
<!-- p1223 -->
### 移除制造分配（Remove Mfg Assignments）

移除制造分配（Remove Mfg assignments）选项可从所选的焊点（weld point）中移除分配（assignment）。

**步骤**
1. 从 Mfg 树中选择一个带有分配的焊点。
2. 右键单击该焊点并选择 **Remove Mfg assignments**（移除制造分配）。
   分配即被移除。

**注意**
移除制造（Mfg）分配后，当按下相应图标时，各查看器中的对象将显示为红色。

<a id="v11-s5"></a>
<!-- p1224 -->
## 点云（Point cloud）

点云（point cloud）是一组表示三维系统的数据点，通常由三维扫描仪创建。点云中的点表示被扫描三维对象的外部表面。

所有用户均可加载并查看包含点云的数据。但是，要插入或编辑点云，您必须购买额外的许可证。

借助三维扫描仪，可以扫描复杂对象（例如制造工厂），创建被扫描对象的三维模型，并将结果存储为点云文件。随后您可将点云以 POD 格式导入 Process Designer（必要时先转换为 POD 格式），并且像其他任意对象一样，它可在图形查看器（Graphic Viewer）和对象查看器（Object Viewer）中显示。

此视频演示点云的基本用法。
**注意**：PDF 中不包含视频。要访问视频，请使用 HTML 版本。

此视频演示使用点云图层（layer）。
**注意**：PDF 中不包含视频。要访问视频，请使用 HTML 版本。

- 点云是单一对象。
- 您可将点云划分为多个图层（layer）以增加灵活性。例如，您可能希望将每个工作站移动到单独的图层；这样可使您显示某些工作站而隐藏其他工作站。参见"管理点云（Managing Point Clouds）"。
- 点云文件通常包含海量数据。但是，得益于智能内存管理算法，您仍可在点云文件加载期间继续在 Process Simulate 中工作。该算法监控内存消耗，并根据用户设定的视点最优地加载点云数据。
- 您可以对点云使用所有剖面（Sections）工具。
- 您必须为点云配置系统根（system root）。更多信息请参阅"组件（Components）"选项卡中的点云（Point Cloud）选项。
- 为获得理想的着色（shading）效果，您可在外观（Appearance）选项卡的点云（Point Clouds）选项中修改"点云着色（Point Cloud Shading）"设置。
- 您可增大点的大小（外观选项卡中），这有助于提高点云的可视性。
- 点云集成不支持虚拟机，也不支持在 Citrix 环境下工作。
- 您可执行点云/点云图层与其他对象之间的碰撞（collision）检查。

执行碰撞检查时存在一些限制：
- 对于"点云/点云图层 vs 点云/点云图层"的碰撞对（collision pair）无法进行检查。
- 碰撞查看器（Collision Viewer）中的"所有显示对象（All displayed objects）"选项不显示点云/点云图层的碰撞。
- 碰撞查看器（Collision Viewer）不显示点云和点云图层的碰撞轮廓（colliding contour）。
- 如果用于碰撞检测的所有对象均为点云/点云图层，则快速碰撞（Fast collision）被停用。
- 对于包含点云/点云图层的碰撞对，不计算违例（Violation）值。

使用点云对规划工程师有利，因为：
- 扫描技术使您能够基于当前现有数据规划制造工作站布局，同时考虑工厂结构、现有资源等，并避免问题。例如，如果您希望规划新车型的生产流程，可以使用代表当前制造工作站精确布局的点云作为新生产线的基础，并进行必要的修改，而无需依赖可能不准确或过时的图纸。此外，还可以定期更新扫描并在 Process Designer 中维护数据，这在使用工厂 CAD 设计时很难做到。
- 在许多情况下，制造车间不断发生变化。创建新的点云并更新研究是一个简单的过程。

<a id="v11-s6"></a>
<!-- p1227 -->
### 插入点云（Insert point cloud）

**步骤**
1. 选择 **Point Cloud → Insert Point Clouds**（点云 → 插入点云）。
   出现"浏览点云文件（Browse Point Cloud file）"对话框。
2. 选择要打开的一个或多个点云文件（.pod）并单击 **Open**（打开）。
   点云开始加载并显示在对象树（Object Tree）中。

**注意**
如果点云显示不清晰，尝试更改外观（Appearance）选项卡中针对点云的着色（Shading）设置。

<a id="v11-s7"></a>
<!-- p1228 -->
### 重定位点云（Relocate point cloud）

您可以使用位置操纵器（Placement Manipulator）或重定位（Relocate）命令来平移（translate）和旋转点云。从对象查看器（Object Viewer）（而非图形查看器）中选择一个或多个点云，并从图形查看器工具栏中选择 **Placement Manipulator**（位置操纵器）。

您可以变换（transform）多个点云，也可以将点云与零件（part）和资源（resource）一起变换。

**注意**
- 快速放置（Fast Placement）不能用于重定位点云，因为无法在图形查看器中选择点云。
- 变换点云会将其所有图层一同重定位——点云图层不能单独变换。
- 点云不支持流程（Flow）操作。
- 当点云在碰撞集（collision set）中移动时，会检测碰撞。

<a id="v11-s8"></a>
<!-- p1229 -->
### 创建点云图层（Create point cloud layer）

**步骤**
1. 选择 **Point Cloud → Edit Point Cloud**（点云 → 编辑点云）。
   点云在对象树（Object Tree）中更新，其图标上增加一个笔形标记，表示点云处于编辑模式。
   **注意**：一次只能编辑一个点云。
2. 选择 **Point Cloud → Create Point Cloud Layer**（点云 → 创建点云图层）。新图层嵌套在对象查看器（Object Viewer）中活动点云下。您可根据需要编辑图层的名称。
3. 根据需要创建更多图层（最多 126 个图层）。
4. 将点分配到图层：
   a. 选择 **Point Cloud → Select Rectangle**（点云 → 选择矩形）。光标变为相应的选择符号。
   b. 拖动光标覆盖要分配到特定图层的点。
      释放鼠标按钮时，所选云点显示为橙色。
      **注意**：
      - 任何点只能与一个点点云图层关联。
      - 如果希望进行多选，按住 <Ctrl> 并拖动鼠标覆盖点云的其他区域。
      - 如果对已做的云点选择不满意，可单击"清除选择（Clear Selection）"图标或拖动鼠标覆盖其他区域。
   c. 为辅助做出最佳点选择，您可以旋转数据。
   d. 在对象查看器中选择所需点云图层，并单击 **Point Cloud → Move Selected Points to Layer**（点云 → 将所选点移动到图层）。
      相关图层的显示/隐藏状态从"未定义（X）"变为"已显示（阴影框）"。
   e. 为所有图层选择云点。
5. 单击相应图标退出编辑模式。
6. 根据需要设置点云图层的隐藏/显示状态。
   **注意**：您也可以在编辑模式下隐藏和显示点云图层。

<a id="v11-s9"></a>
<!-- p1236 -->
### 将点云附加到对象（Attaching a point cloud to an object）

如果您的数据缺少单个项目，例如机器人随附的 CAD 文件中未提供的机器人附件，可以将该项目的扫描点云附加到机器人上。也可以将机器人附加到点云。为此，请使用附加（Attach）命令。

**注意**
如果您拥有完整制造生产线的点云，则无需将点云附加到对象。

将点云附加到对象后，云和对象很可能存储在不同的文件夹中。通常，机器人存储在资源（resources）文件夹中，而点云存储在点点云（point clouds）文件夹中。对于碰撞检测，您可以使用创建组（Create Group）命令将云和机器人组合为一个逻辑组，然后将其用于碰撞集（collision set）。

<a id="v11-s10"></a>
<!-- p1237 -->
### 使用点云检测碰撞（Detecting collisions with a point cloud）

您可以像使用其他任意对象一样，在碰撞集（collision set）中使用点云。但是，如果您的点云是完整的制造生产线，可能无法从碰撞结果中获得有用信息。

为使碰撞结果有用，您可以将某个对象的所有点分配到专用图层，然后测试与该图层的碰撞。例如，要测试以下示例中机器人与固定车门的支架之间的碰撞：

**步骤**
1. 隐藏（Blank）制造生产线的筋肋（rib）和车门。
2. 选择 **Point Cloud → Edit Point Cloud**（点云 → 编辑点云）。
3. 选择 **Point Cloud → Create Point Cloud Layer**（点云 → 创建点云图层）并命名新图层。
4. 从上方查看支架。
5. 选择 **Point Cloud → Select Rectangle**（点云 → 选择矩形）并选择构成支架的点。
   **注意**：从上方查看支架时选择点，意味着不会选到与支架无关的点。
6. 在对象查看器中选择 Stand 图层，并单击 **Move Selected Points to Layer**（将所选点移动到图层）。
7. 打开碰撞查看器（Collision Viewer）并为机器人和支架创建一个新的碰撞集。
8. 使用碰撞查看器测试碰撞。

<a id="v11-s11"></a>
<!-- p1244 -->
## 另存为（Save As）

另存为（Save As）命令将当前会话中的工程数据（engineering data）保存到所选的研究（study）。数据保存后，在 Process Designer 中调用该研究会自动同时打开已保存的工程数据。应用变体过滤器（Variant Filter）时，系统会询问是否在继续之前保存工程数据。

要将工程数据保存到不同的研究：

**步骤**
1. 选择 **File**（文件）选项卡 → **Save Scenario**（保存方案）。显示"保存方案（Save Scenario）"窗口。
2. 从树中选择一个已签出的研究，或通过在"研究名称（Study name）"字段中输入名称来创建新研究。系统显示一条消息，询问是否覆盖该研究中已有的数据。
3. 单击 **Save**（保存）。
   如果选择了现有研究，新数据将覆盖任何现有数据。
   **注意**：如果您尝试保存在其中已签出资源发生移动的研究，会出现提示窗口，要求您签出该资源。
4. 单击 **Yes**（是）签出资源并完成保存操作。如果该资源已被其他用户签出，保存操作将中止。

<a id="v11-s12"></a>
<!-- p1247 -->
### 设置保存方案配置（Set Save Scenario Configuration）

管理员可以限制用户在研究（Study）文件夹中创建零件和资源快捷方式的能力。

通过将命令从"自定义（Customize）"对话框拖出，可将其添加到功能区（ribbon）。

<a id="v11-s13"></a>
<!-- p1247 -->
## 使用独立应用程序升级组件（Upgrading Components Using the Standalone Application）

升级 CO 到版本（Upgrade CO to Version）命令是一个在 Windows 下运行的独立实用程序。

升级 CO 到版本（Upgrade CO to Version）用于在 Process Designer 或 Process Simulate 中升级原型（prototype）三维表示文件，或将原型三维表示文件从 .CO 格式转换为 .COJT 格式。您可以升级或转换：

- 目录（Directories）。
- 组件（Components）。
- 超级组件（Super components）——无需先升级链接的组件。

您也可以使用升级 CO 到版本（Upgrade CO to Version）将 Robcad 组件转换为 Tecnomatix 格式。

您还可以自定义升级 CO 到版本（Upgrade CO to Version）管理工具中使用的参数。

**升级 CO 文件步骤**
1. 在 Windows 桌面上，单击 **Start → Programs → Tecnomatix → Admin → UpgradeToVersionWin.exe**（开始 → 程序 → Tecnomatix → 管理 → UpgradeToVersionWin.exe）。打开"升级 CO 到版本（Upgrade CO to Version）"对话框，使您能够配置升级。
2. 单击 **Add**（添加）。出现"选择组件文件（Select component files）"对话框。
3. 选择要升级的零件、资源或完整文件夹。所选对象显示在"升级 CO 到版本（Upgrade CO to Version）"对话框的"要升级的组件和目录（Components and directories to upgrade）"列表中。双击此列表中的任意对象可显示其子文件夹。如果组件包含图形文件，"选择组件文件（Select component files）"对话框会显示预览。
4. 选择目标格式（Target Format），如下：
   - **COJT** - 仅当希望仅包含详细（detailed）表示并排除联合（united）表示时，才勾选 **Detailed only**。清除此选项以包含联合表示。您可以指示系统在确定所选原型已存在 COJT 文件时如何反应：勾选 **Skip**（跳过）以中止升级，或勾选 **Overwrite**（覆盖）以覆盖现有文件。
   - **CO** - 勾选以下任意选项：
     - 勾选 **Include entity level**（包含实体级别）以包含原型的详细表示。
     - 勾选 **Exclude frames in united**（排除联合中的框架）以从联合表示中排除框架。
     - 勾选 **Exclude 2D in united**（排除联合中的 2D）以从联合表示中排除 2D 对象。
     - 勾选 **Remove .gmsimperf** 以从升级后的 .co 组件中移除联合表示中间文件（.gmsimperf）。
     - 勾选 **Skip when JT file exists under the CO to**（当 CO 下已存在 JT 文件时跳过）：跳过在 co 下已有 jt 的组件。
   - **Keep current format and update materials only**（保持当前格式仅更新材质）——此选项不执行完整升级。它将所选组件的 Robcad 材质导入到升级后的组件，同时保留当前的 .co 或 .cojt 格式。它作用于已升级到 .co 或 .cojt 格式的组件。
     **注意**：
     - "保持当前格式仅更新材质（Keep current format and update materials only）"不作用于原生（native）JT 组件。
     - 如果输入的 .co 或 .cojt 文件夹下没有嵌套的 .jt 文件，则无法运行"保持当前格式仅更新材质"选项。请对此数据运行完整升级。
     - 选择"保持当前格式仅更新材质"选项时，对话框中除"清理间隔（Clean interval）"和"创建日志文件（Create log file）"外的所有选项均被禁用。
5. 根据需要配置以下选项：
   - **Approximation**（近似）
     - **Force upgrade approximation**（强制升级近似）——重新创建联合表示近似。此选项耗时较长。
     - **LOD ratio**（LOD 比率）——使您能够设置基础级别（级别 0）与下一级别（级别 1）之间的细节级别比率。LOD 值必须介于 0.01 和 0.99 之间，精确到两位小数。输入无效值会禁用"确定（OK）"按钮。如果清除此选项，系统使用 LOD 比率 0.5。
   - **General**（常规）
     - **Clean interval**（清理间隔）——升级 CO 到版本（Upgrade CO to Version）作为其例程的一部分运行"升级到版本（Upgrade To Version）"外部管理工具。升级许多大型组件时，需要不时重新启动该外部管理工具。勾选"清理间隔"可自定义在当前版本的"升级到版本"外部应用程序进程自动重启之前升级的组件数量。该值必须介于 1-100。输入无效值会禁用"确定（OK）"按钮。如果清除此选项，默认"清理间隔"值为 50。
       **注意**：在显示当前处理组件时，"清理间隔（Clean Interval）"不反映在进度条文本"正在升级组件 1 / x..."中，也不反映在日志文件中。
     - **Exclude material definitions**（排除材质定义）——默认情况下，升级 CO 原型到版本（Upgrade CO Prototypes to Version）执行"更新材质（Update Materials）"以将所选组件的 Robcad 材质导入到升级后的组件。
     - 如果您正在升级超级组件，可勾选 **Use library root**（使用库根）并指定组件存储的位置。
     - 勾选 **Create log file**（创建日志文件）并输入名称，如果您希望系统创建日志文件。
       **注意**：在任何阶段，将鼠标悬停在帮助图标上可显示设置的工具提示。
6. 单击 **Upgrade**（升级）以执行升级并保存最后的升级设置。所选对象的转换开始。

将 .CO 转换为 .COJT 时，会发生以下情况：
- 对于每个原型，在与原型 .CO 文件相同的目录中创建 .COJT 文件。
- 系统保留源 .CO 文件在目录中（不会被删除）。
- 对于超级组件，组件及其所有链接的子组件均被转换。

将 .CO 转换为 .CO 时，现有的 .CO 被覆盖，并在 .CO 组件下创建 JT 文件。

转换完成后，出现一个对话框，使您能够查看日志文件。

7. 执行以下操作之一：
   - 单击 **Yes**（是）以查看日志文件。
     日志文件在默认文本查看器中打开，包含以下信息：
     - 用于转换原型的命令行。
     - 任何转换失败的原因。
     - "UpgradeToVersion.exe"日志：包含每个输入组件的状态摘要，以及当前"升级到版本（Upgrade To Version）"调用中正确转换的组件数量摘要。
     - 将 .CO 转换为 .COJT 时，提供以下附加信息：已转换原型数量、未转换原型数量，以及生成的日志文件内容（记录组件名称、近似与联合/详细信息的更新结果，以及升级汇总，例如"组件升级成功数量：1，警告：0，带错误成功：0，未升级：0，失败：0"等）。
   - 单击 **No**（否）以关闭对话框而不查看日志。

<a id="v11-s14"></a>
<!-- p1253 -->
## 使用命令行升级组件（Upgrading Components Using the Command Line）

请参阅"使用命令行升级组件文件（Upgrading Component Files from the Command Line）"。