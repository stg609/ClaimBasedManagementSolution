���� solution ����3�����֣�IdServer, BackEnd �� FrontEnd��



���У�IdServer ���������û���������ע�ᡢ��¼������ɫ��������Ϊĳ����ɫ��Ӷ�� claim)��
IdServer ��ͨ������ BackEnd �� FrontEnd ��Ԫ���ݣ����õ����� claim������ claim ������ IdServer �����ݿ��С���ͬ�� claim ����Ϊ����һ�� claim��ֻ����һ����¼��
IdServer �����ṩ�Զ�̬��ӵ� BackEnd �� FrontEnd ���н�����


BackEnd����Ϊ API ����ÿ����¶�ķ��������Ҫ�û���¼����ܵ��ã������� [Authorize] �����Ρ����ʹ��[Authorize(Policy = "CLAIM.XXX")] ��ʽ������Ϊ�õ�¼���û�����Ҫ�߱��ض��� claim��[Authorize] ���Կ��Ե���ʹ�ã������Ӷ�� authorize ����ʱ����ʾ and ��ϵ������Ҫ�����������Ե�Ҫ��
BackEnd ��Ҫ��Ԫ���ݲ��汩¶���� claim����IdServer ʹ�á�




FrontEnd, ��Ϊ SPA, MVC �ȣ����˽�����Ⱦ�����⣬������˵��Ĺ���ǰ�˿���������ƾ���ҳ�湦��ǰ��Ӧ���Ѿ��õ���һ�� BackEnd ��Ԫ���ݣ������� BackEnd ��Ҫ�� claim ���ҳ������� claim��
FrontEnd ͬ����Ҫ�ṩһ��Ԫ���ݱ�¶���� claim���� idServer ʹ�á�
FrontEnd �ڵ�һ�����е�ʱ����Զ��������� action ����� claim��������һ��menu����

| Id | menuName      | Url        | claims          |  Order | Parent |
| 0  | Home	         |            |                 |    0   |        |
| 1  | HomeIndex     | Home/Index |                 |    0   |    0   |
| 2  | User          |            |                 |    0   |        |
| 3  | UserIndex     | User/Index | CLAIM.User      |    0   |    2   |
| 4  | UserEdit      | User/Edit  | CLAIM.User.Edit |    0   |    2   |

�������Զ����ɵ� menu ���ݣ����� menuName �� controller+action ��ʽ, Order ��Ϊ 0����Щ���ݻᱻд�����ݿ⣬�û������ֶ�������Order, menuName), ���ǲ������޸� claims��

FrontEnd ���к󣬻���ݵ�ǰ��¼���û��������յ� menu������û��� Claim Ϊ CLAIM.User,û�� CLAIM.User.Edit����ô���ɵĲ˵����£�
Home            User
  HomeIndex        UserIndex

����û��� Claim Ϊ�գ��� User �ڵ��������������ɣ���Ϊ User ���ڵ��µ������ӽڵ㶼û����ʾ���Ҹ��ڵ㱾��Ҳû�� Url)
Home
  HomeIndex

*����*
Claim ��CLAIM.����.С��������������Ҫ�� ACTION (�� mvc �е� action), API���磺CLAIM.ACTION.Home, CLAIM.ACTION.Home.Index, CLAIM.API.USERS

API һ����Ϊ���ȱȽ�ϸ�����Կ����������ƾ����ĳһ����ť��



*ʾ��*
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