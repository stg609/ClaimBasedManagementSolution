整个 solution 包含3个部分：IdServer, BackEnd 和 FrontEnd。



其中，IdServer 用来负责用户管理（包含注册、登录）、角色管理（可以为某个角色添加多个 claim)。
IdServer 会通过解析 BackEnd 与 FrontEnd 的元数据，来得到所有 claim，并将 claim 保存在 IdServer 的数据库中。相同的 claim 被认为是用一个 claim，只保存一条记录。
IdServer 还需提供对动态添加的 BackEnd 与 FrontEnd 进行解析。


BackEnd，多为 API 服务，每个暴露的方法如果需要用户登录后才能调用，都会有 [Authorize] 来修饰。如果使用[Authorize(Policy = "CLAIM.XXX")] 方式，则认为该登录的用户还需要具备特定的 claim。[Authorize] 特性可以叠加使用，当叠加多个 authorize 特性时，表示 and 关系，即需要满足所有特性的要求。
BackEnd 需要在元数据层面暴露所有 claim，供IdServer 使用。




FrontEnd, 多为 SPA, MVC 等，除了界面渲染工作外，还负责菜单的管理。前端开发者在设计具体页面功能前，应该已经得到了一份 BackEnd 的元数据，并根据 BackEnd 需要的 claim 设计页面所需的 claim。
FrontEnd 同样需要提供一个元数据暴露所有 claim，供 idServer 使用。
FrontEnd 在第一次运行的时候回自动解析所有 action 层面的 claim，并生成一份menu数据

| Id | menuName      | Url        | claims          |  Order | Parent |
| 0  | Home	         |            |                 |    0   |        |
| 1  | HomeIndex     | Home/Index |                 |    0   |    0   |
| 2  | User          |            |                 |    0   |        |
| 3  | UserIndex     | User/Index | CLAIM.User      |    0   |    2   |
| 4  | UserEdit      | User/Edit  | CLAIM.User.Edit |    0   |    2   |

由于是自动生成的 menu 数据，所以 menuName 以 controller+action 方式, Order 都为 0。这些数据会被写入数据库，用户可以手动调整（Order, menuName), 但是不允许修改 claims。

FrontEnd 运行后，会根据当前登录的用户生成最终的 menu，如果用户的 Claim 为 CLAIM.User,没有 CLAIM.User.Edit，那么生成的菜单如下：
Home            User
  HomeIndex        UserIndex

如果用户的 Claim 为空，则 User 节点整个都不会生成（因为 User 父节点下的所有子节点都没有显示，且父节点本身也没有 Url)
Home
  HomeIndex

*建议*
Claim 以CLAIM.大类.小类命名，大类主要有 ACTION (即 mvc 中的 action), API。如：CLAIM.ACTION.Home, CLAIM.ACTION.Home.Index, CLAIM.API.USERS

API 一般认为粒度比较细，所以可以用来控制具体的某一个按钮。



*示例*
//MVC Client
public class UserController
{
	[Authorize(Policy = "CLAIM.MENU.User.Index")]
    [Authorize(Policy = "CLAIM.API.Users")]
	public IActionResult Index()
	{
		//call api.Users , required CLAIM.API.Users
	}
}

public class UserAPIController
{
	[Authorize(Policy = "CLAIM.API.Users")]
	public List<User> Users()
	{
		//
	}
}