

using System;
using Soul.SqlBatis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Entities
{	

	/// <summary>
    /// 创收单主表
    /// </summary>
	[Table("create_income")]
	public partial class CreateIncome : Entity
	{
		/// <summary>
		/// 追加费
		/// </summary>
		[Column("ci_add_money")]
		public  decimal? CiAddMoney { get; set; }
		/// <summary>
		/// 追加费收费比例
		/// </summary>
		[Column("ci_add_rate")]
		public  decimal? CiAddRate { get; set; }
		/// <summary>
		/// 创建人id
		/// </summary>
		[Column("ci_au_id")]
		public  int? CiAuId { get; set; }
		/// <summary>
		/// 创收回款金额
		/// </summary>
		[Column("ci_back_money")]
		public  decimal? CiBackMoney { get; set; }
		/// <summary>
		/// 基本费
		/// </summary>
		[Column("ci_base_money")]
		public  decimal? CiBaseMoney { get; set; }
		/// <summary>
		/// 冲红原纪录id
		/// </summary>
		[Column("ci_ch_id")]
		public  int? CiChId { get; set; }
		/// <summary>
		/// 跨年创收审批流id
		/// </summary>
		[Column("ci_cross_year_lc_type")]
		public  int? CiCrossYearLcType { get; set; }
		/// <summary>
		/// 审核时间
		/// </summary>
		[Column("ci_ex_time")]
		public  DateTime? CiExTime { get; set; }
		/// <summary>
		/// 附件
		/// </summary>
		[Column("ci_file")]
		public  string CiFile { get; set; }
		/// <summary>
		/// 创收红冲回款金额
		/// </summary>
		[Column("ci_hc_back_money")]
		public  decimal? CiHcBackMoney { get; set; }
		/// <summary>
		/// 创收坏账回款金额
		/// </summary>
		[Column("ci_hz_back_money")]
		public  decimal? CiHzBackMoney { get; set; }
		/// <summary>
		/// 是否全部回款
		/// </summary>
		[Column("ci_is_back")]
		public  int? CiIsBack { get; set; }
		/// <summary>
		/// 是否作废
		/// </summary>
		[Column("ci_is_cancel")]
		public  int? CiIsCancel { get; set; }
		/// <summary>
		/// 是否跨年度创收
		/// </summary>
		[Column("ci_is_cross_year")]
		public  int? CiIsCrossYear { get; set; }
		/// <summary>
		/// 是否已经共享
		/// </summary>
		[Column("ci_is_share")]
		public  int? CiIsShare { get; set; }
		/// <summary>
		/// 开票账户
		/// </summary>
		[Column("ci_iv_account")]
		public  string CiIvAccount { get; set; }
		/// <summary>
		/// 开票单位名称
		/// </summary>
		[Column("ci_iv_kp_name")]
		public  string CiIvKpName { get; set; }
		/// <summary>
		/// 发票号
		/// </summary>
		[Column("ci_iv_no")]
		public  string CiIvNo { get; set; }
		/// <summary>
		/// 创收名称
		/// </summary>
		[Column("ci_name")]
		public  string CiName { get; set; }
		/// <summary>
		/// 无关联项目项目名
		/// </summary>
		[Column("ci_no_pi_name")]
		public  string CiNoPiName { get; set; }
		/// <summary>
		/// 没有通过的产值年度变更数量
		/// </summary>
		[Column("ci_nopass_year_update_num")]
		public  int? CiNopassYearUpdateNum { get; set; }
		/// <summary>
		/// 无项目创收审批流
		/// </summary>
		[Column("ci_nopi_lc_type")]
		public  int? CiNopiLcType { get; set; }
		/// <summary>
		/// 一级：项目服务类型id
		/// </summary>
		[Column("ci_pi_bt_id")]
		public  int? CiPiBtId { get; set; }
		/// <summary>
		/// 一级：项目服务类型name
		/// </summary>
		[Column("ci_pi_bt_name")]
		public  string CiPiBtName { get; set; }
		/// <summary>
		/// 项目id
		/// </summary>
		[Column("ci_pi_id")]
		public  int? CiPiId { get; set; }
		/// <summary>
		/// 二级：项目服务类型id
		/// </summary>
		[Column("ci_pi_sec_bt_id")]
		public  int? CiPiSecBtId { get; set; }
		/// <summary>
		/// 二级：项目服务类型name
		/// </summary>
		[Column("ci_pi_sec_bt_name")]
		public  string CiPiSecBtName { get; set; }
		/// <summary>
		/// 创收票据回款金额
		/// </summary>
		[Column("ci_pj_back_money")]
		public  decimal? CiPjBackMoney { get; set; }
		/// <summary>
		/// 权限
		/// </summary>
		[Column("ci_power")]
		public  string CiPower { get; set; }
		/// <summary>
		/// 部门审批权限
		/// </summary>
		[Column("ci_power_dep")]
		public  string CiPowerDep { get; set; }
		/// <summary>
		/// 创收部门经理审批权限
		/// </summary>
		[Column("ci_power_deplead")]
		public  string CiPowerDeplead { get; set; }
		/// <summary>
		/// 人员审批权限
		/// </summary>
		[Column("ci_power_per")]
		public  string CiPowerPer { get; set; }
		/// <summary>
		/// 关联项目名称
		/// </summary>
		[Column("ci_re_name")]
		public  string CiReName { get; set; }
		/// <summary>
		/// 在分配状态
		/// </summary>
		[Column("ci_re_state")]
		public  int? CiReState { get; set; }
		/// <summary>
		/// 施工单位id
		/// </summary>
		[Column("ci_sg_id")]
		public  int? CiSgId { get; set; }
		/// <summary>
		/// 施工单位联系方式
		/// </summary>
		[Column("ci_sg_mobile")]
		public  string CiSgMobile { get; set; }
		/// <summary>
		/// 施工单位名称
		/// </summary>
		[Column("ci_sg_name")]
		public  string CiSgName { get; set; }
		/// <summary>
		/// 施工单位联系人
		/// </summary>
		[Column("ci_sg_per")]
		public  string CiSgPer { get; set; }
		/// <summary>
		/// 施工单位联系人id
		/// </summary>
		[Column("ci_sg_per_id")]
		public  int? CiSgPerId { get; set; }
		/// <summary>
		/// 收款类型
		/// </summary>
		[Column("ci_sk_type")]
		public  int? CiSkType { get; set; }
		/// <summary>
		/// 审批类型(0初始1人员通过待部门审核2待创收部门经理审批)
		/// </summary>
		[Column("ci_sp_state")]
		public  int? CiSpState { get; set; }
		/// <summary>
		/// 状态（0待提交1已提交2已审批）
		/// </summary>
		[Column("ci_state")]
		public  int? CiState { get; set; }
		/// <summary>
		/// 类型（0造价咨询1跟踪审计2游离）
		/// </summary>
		[Column("ci_type")]
		public  int? CiType { get; set; }
		/// <summary>
		/// 发票id
		/// </summary>
		[Column("ci_vi_id")]
		public  int? CiViId { get; set; }
		/// <summary>
		/// 委托单位id
		/// </summary>
		[Column("ci_wt_id")]
		public  int? CiWtId { get; set; }
		/// <summary>
		/// 委托单位联系方式
		/// </summary>
		[Column("ci_wt_mobile")]
		public  string CiWtMobile { get; set; }
		/// <summary>
		/// 委托单位名称
		/// </summary>
		[Column("ci_wt_name")]
		public  string CiWtName { get; set; }
		/// <summary>
		/// 委托单位联系人
		/// </summary>
		[Column("ci_wt_per")]
		public  string CiWtPer { get; set; }
		/// <summary>
		/// 委托单位联系人id
		/// </summary>
		[Column("ci_wt_per_id")]
		public  int? CiWtPerId { get; set; }
		/// <summary>
		/// 跨年度创收power
		/// </summary>
		[Column("ci_year_power")]
		public  string CiYearPower { get; set; }
		/// <summary>
		/// 产值年份变更最新状态
		/// </summary>
		[Column("ci_year_update_new_state")]
		public  int? CiYearUpdateNewState { get; set; }
		/// <summary>
		/// 创收正常回款金额
		/// </summary>
		[Column("ci_zc_back_money")]
		public  decimal? CiZcBackMoney { get; set; }
		/// <summary>
		/// 创建人
		/// </summary>
		[Column("create_time")]
		public  DateTime? CreateTime { get; set; }
		/// <summary>
		/// 创建人
		/// </summary>
		[Column("create_user")]
		public  string CreateUser { get; set; }
		/// <summary>
		/// id
		/// </summary>
		[Key][Identity][Column("id")]
		public override int? Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("o_id")]
		public  string OId { get; set; }
		/// <summary>
		/// 修改时间
		/// </summary>
		[Column("update_time")]
		public  DateTime? UpdateTime { get; set; }
		/// <summary>
		/// 修改人
		/// </summary>
		[Column("update_user")]
		public  string UpdateUser { get; set; }
	}

	/// <summary>
    /// 开票主表
    /// </summary>
	[Table("invoice")]
	public partial class Invoice : Entity
	{
		/// <summary>
		/// 创建人
		/// </summary>
		[Column("create_time")]
		public  DateTime? CreateTime { get; set; }
		/// <summary>
		/// 创建人
		/// </summary>
		[Column("create_user")]
		public  string CreateUser { get; set; }
		/// <summary>
		/// id
		/// </summary>
		[Key][Identity][Column("id")]
		public override int? Id { get; set; }
		/// <summary>
		/// 开票账户
		/// </summary>
		[Column("iv_account")]
		public  string IvAccount { get; set; }
		/// <summary>
		/// 地址
		/// </summary>
		[Column("iv_address")]
		public  string IvAddress { get; set; }
		/// <summary>
		/// 申请人
		/// </summary>
		[Column("iv_apply_per")]
		public  int? IvApplyPer { get; set; }
		/// <summary>
		/// 申请人姓名
		/// </summary>
		[Column("iv_apply_per_name")]
		public  string IvApplyPerName { get; set; }
		/// <summary>
		/// 创建人
		/// </summary>
		[Column("iv_au_id")]
		public  int? IvAuId { get; set; }
		/// <summary>
		/// 回款金额
		/// </summary>
		[Column("iv_back_money")]
		public  decimal? IvBackMoney { get; set; }
		/// <summary>
		/// 回款类型（0平均回款1部门回款）
		/// </summary>
		[Column("iv_back_type")]
		public  int? IvBackType { get; set; }
		/// <summary>
		/// 作废创收JSON
		/// </summary>
		[Column("iv_cancel_ci_json")]
		public  string IvCancelCiJson { get; set; }
		/// <summary>
		/// 作废申请状态
		/// </summary>
		[Column("iv_cancel_state")]
		public  int? IvCancelState { get; set; }
		/// <summary>
		/// 冲红原纪录id
		/// </summary>
		[Column("iv_ch_id")]
		public  int? IvChId { get; set; }
		/// <summary>
		/// 创收部门
		/// </summary>
		[Column("iv_ci_dep_ids")]
		public  string IvCiDepIds { get; set; }
		/// <summary>
		/// 关联的创收单
		/// </summary>
		[Column("iv_ci_ids")]
		public  string IvCiIds { get; set; }
		/// <summary>
		/// 创收人
		/// </summary>
		[Column("iv_ci_per_ids")]
		public  string IvCiPerIds { get; set; }
		/// <summary>
		/// 开票内容
		/// </summary>
		[Column("iv_content")]
		public  string IvContent { get; set; }
		/// <summary>
		/// 创收共享人记录id
		/// </summary>
		[Column("iv_csgp_id")]
		public  int? IvCsgpId { get; set; }
		/// <summary>
		/// 部门id
		/// </summary>
		[Column("iv_dep_id")]
		public  int? IvDepId { get; set; }
		/// <summary>
		/// 部门名称
		/// </summary>
		[Column("iv_dep_name")]
		public  string IvDepName { get; set; }
		/// <summary>
		/// 打款人
		/// </summary>
		[Column("iv_dk_per_name")]
		public  string IvDkPerName { get; set; }
		/// <summary>
		/// 到款方式（0开票单位打款1其他单位打款2个人打款）
		/// </summary>
		[Column("iv_dk_type")]
		public  int? IvDkType { get; set; }
		/// <summary>
		/// 邮箱
		/// </summary>
		[Column("iv_email")]
		public  string IvEmail { get; set; }
		/// <summary>
		/// 复核人
		/// </summary>
		[Column("iv_fh_per")]
		public  string IvFhPer { get; set; }
		/// <summary>
		/// 附件
		/// </summary>
		[Column("iv_file")]
		public  string IvFile { get; set; }
		/// <summary>
		/// 冲红申请状态
		/// </summary>
		[Column("iv_flushred_state")]
		public  int? IvFlushredState { get; set; }
		/// <summary>
		/// 发票信息json
		/// </summary>
		[Column("iv_fpinfo_json")]
		public  string IvFpinfoJson { get; set; }
		/// <summary>
		/// 跟踪审计项目ids
		/// </summary>
		[Column("iv_gzpi_id")]
		public  string IvGzpiId { get; set; }
		/// <summary>
		/// 跟踪审计项目名称
		/// </summary>
		[Column("iv_gzpi_name")]
		public  string IvGzpiName { get; set; }
		/// <summary>
		/// 红冲回款
		/// </summary>
		[Column("iv_hc_back_money")]
		public  decimal? IvHcBackMoney { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("iv_hc_del_json")]
		public  string IvHcDelJson { get; set; }
		/// <summary>
		/// 生成的负向发票关联的红冲id
		/// </summary>
		[Column("iv_hc_id")]
		public  int? IvHcId { get; set; }
		/// <summary>
		/// 红冲关联json
		/// </summary>
		[Column("iv_hc_json")]
		public  string IvHcJson { get; set; }
		/// <summary>
		/// 红冲操作人id
		/// </summary>
		[Column("iv_hc_per_id")]
		public  int? IvHcPerId { get; set; }
		/// <summary>
		/// 红冲操作人名
		/// </summary>
		[Column("iv_hc_per_name")]
		public  string IvHcPerName { get; set; }
		/// <summary>
		/// 红冲时间
		/// </summary>
		[Column("iv_hc_time")]
		public  DateTime? IvHcTime { get; set; }
		/// <summary>
		/// 坏账回款
		/// </summary>
		[Column("iv_hz_back_money")]
		public  decimal? IvHzBackMoney { get; set; }
		/// <summary>
		/// 是否作废
		/// </summary>
		[Column("iv_is_cancel")]
		public  int? IvIsCancel { get; set; }
		/// <summary>
		/// 是否冲红
		/// </summary>
		[Column("iv_is_ch")]
		public  int? IvIsCh { get; set; }
		/// <summary>
		/// 是否需要填写备注
		/// </summary>
		[Column("iv_is_fill_remark")]
		public  int? IvIsFillRemark { get; set; }
		/// <summary>
		/// 是否有红冲关联
		/// </summary>
		[Column("iv_is_hc")]
		public  int? IvIsHc { get; set; }
		/// <summary>
		/// 是否红冲
		/// </summary>
		[Column("iv_is_hcfp")]
		public  int? IvIsHcfp { get; set; }
		/// <summary>
		/// 是否是红冲自动生成
		/// </summary>
		[Column("iv_is_hcsc")]
		public  int? IvIsHcsc { get; set; }
		/// <summary>
		/// 是否是全电发票
		/// </summary>
		[Column("iv_is_qd")]
		public  int? IvIsQd { get; set; }
		/// <summary>
		/// 开户行
		/// </summary>
		[Column("iv_khh")]
		public  string IvKhh { get; set; }
		/// <summary>
		/// 可开票类型
		/// </summary>
		[Column("iv_kkp_type")]
		public  string IvKkpType { get; set; }
		/// <summary>
		/// 开票账号简称
		/// </summary>
		[Column("iv_kp_acc_abb")]
		public  string IvKpAccAbb { get; set; }
		/// <summary>
		/// 开票账号id
		/// </summary>
		[Column("iv_kp_acc_id")]
		public  int? IvKpAccId { get; set; }
		/// <summary>
		/// 地址
		/// </summary>
		[Column("iv_kp_address")]
		public  string IvKpAddress { get; set; }
		/// <summary>
		/// 开票银行
		/// </summary>
		[Column("iv_kp_bank")]
		public  string IvKpBank { get; set; }
		/// <summary>
		/// 开票银行账号
		/// </summary>
		[Column("iv_kp_bank_acc")]
		public  string IvKpBankAcc { get; set; }
		/// <summary>
		/// 开票银行id
		/// </summary>
		[Column("iv_kp_bank_id")]
		public  int? IvKpBankId { get; set; }
		/// <summary>
		/// 开票文件
		/// </summary>
		[Column("iv_kp_file")]
		public  string IvKpFile { get; set; }
		/// <summary>
		/// 开票id
		/// </summary>
		[Column("iv_kp_id")]
		public  int? IvKpId { get; set; }
		/// <summary>
		/// 开票银行开户行
		/// </summary>
		[Column("iv_kp_kh_bank")]
		public  string IvKpKhBank { get; set; }
		/// <summary>
		/// 开票账户最大票额
		/// </summary>
		[Column("iv_kp_max_amount")]
		public  decimal? IvKpMaxAmount { get; set; }
		/// <summary>
		/// 电话
		/// </summary>
		[Column("iv_kp_mobile")]
		public  string IvKpMobile { get; set; }
		/// <summary>
		/// 开票单位名称
		/// </summary>
		[Column("iv_kp_name")]
		public  string IvKpName { get; set; }
		/// <summary>
		/// 开票账户普票税率 
		/// </summary>
		[Column("iv_kp_ordinary_tax_rate")]
		public  decimal? IvKpOrdinaryTaxRate { get; set; }
		/// <summary>
		/// 开票人
		/// </summary>
		[Column("iv_kp_per")]
		public  string IvKpPer { get; set; }
		/// <summary>
		/// 开票人id
		/// </summary>
		[Column("iv_kp_per_id")]
		public  int? IvKpPerId { get; set; }
		/// <summary>
		/// 开票人联系方式
		/// </summary>
		[Column("iv_kp_per_mobile")]
		public  string IvKpPerMobile { get; set; }
		/// <summary>
		/// 开票人
		/// </summary>
		[Column("iv_kp_per_name")]
		public  string IvKpPerName { get; set; }
		/// <summary>
		/// 开票账户税号
		/// </summary>
		[Column("iv_kp_tax_no")]
		public  string IvKpTaxNo { get; set; }
		/// <summary>
		/// 发票材质
		/// </summary>
		[Column("iv_material_quality")]
		public  int? IvMaterialQuality { get; set; }
		/// <summary>
		/// 电话
		/// </summary>
		[Column("iv_mobile")]
		public  string IvMobile { get; set; }
		/// <summary>
		/// 发票金额
		/// </summary>
		[Column("iv_money")]
		public  decimal? IvMoney { get; set; }
		/// <summary>
		/// 发票号
		/// </summary>
		[Column("iv_no")]
		public  string IvNo { get; set; }
		/// <summary>
		/// 无关联项目名称
		/// </summary>
		[Column("iv_nopi_name")]
		public  string IvNopiName { get; set; }
		/// <summary>
		/// 开票操作人id
		/// </summary>
		[Column("iv_open_per_id")]
		public  int? IvOpenPerId { get; set; }
		/// <summary>
		/// 开票操作人姓名
		/// </summary>
		[Column("iv_open_per_name")]
		public  string IvOpenPerName { get; set; }
		/// <summary>
		/// 开票时间
		/// </summary>
		[Column("iv_open_time")]
		public  DateTime? IvOpenTime { get; set; }
		/// <summary>
		/// 招标代理id
		/// </summary>
		[Column("iv_pba_id")]
		public  string IvPbaId { get; set; }
		/// <summary>
		/// 招标代理名称
		/// </summary>
		[Column("iv_pba_name")]
		public  string IvPbaName { get; set; }
		/// <summary>
		/// 造价咨询项目ids
		/// </summary>
		[Column("iv_pi_id")]
		public  string IvPiId { get; set; }
		/// <summary>
		/// 造价咨询项目名称
		/// </summary>
		[Column("iv_pi_name")]
		public  string IvPiName { get; set; }
		/// <summary>
		/// 票据回款
		/// </summary>
		[Column("iv_pj_back_money")]
		public  decimal? IvPjBackMoney { get; set; }
		/// <summary>
		/// 项目管理项目id
		/// </summary>
		[Column("iv_pm_id")]
		public  string IvPmId { get; set; }
		/// <summary>
		/// 项目管理项目名称
		/// </summary>
		[Column("iv_pm_name")]
		public  string IvPmName { get; set; }
		/// <summary>
		/// 审核权限
		/// </summary>
		[Column("iv_power")]
		public  string IvPower { get; set; }
		/// <summary>
		/// 税号
		/// </summary>
		[Column("iv_rate_no")]
		public  string IvRateNo { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		[Column("iv_remark")]
		public  string IvRemark { get; set; }
		/// <summary>
		/// 发票备注
		/// </summary>
		[Column("iv_remark_new")]
		public  string IvRemarkNew { get; set; }
		/// <summary>
		/// 收款人
		/// </summary>
		[Column("iv_sk_per")]
		public  string IvSkPer { get; set; }
		/// <summary>
		/// 收款类型
		/// </summary>
		[Column("iv_sk_type")]
		public  int? IvSkType { get; set; }
		/// <summary>
		/// 状态
		/// </summary>
		[Column("iv_state")]
		public  int? IvState { get; set; }
		/// <summary>
		/// 税率
		/// </summary>
		[Column("iv_tax_rate")]
		public  decimal? IvTaxRate { get; set; }
		/// <summary>
		/// 发票类型
		/// </summary>
		[Column("iv_type")]
		public  int? IvType { get; set; }
		/// <summary>
		/// 开票方式
		/// </summary>
		[Column("iv_way")]
		public  int? IvWay { get; set; }
		/// <summary>
		/// 银行账户
		/// </summary>
		[Column("iv_yh_account")]
		public  string IvYhAccount { get; set; }
		/// <summary>
		/// 正常回款
		/// </summary>
		[Column("iv_zc_back_money")]
		public  decimal? IvZcBackMoney { get; set; }
		/// <summary>
		/// 红冲关联的正向发票的ids
		/// </summary>
		[Column("iv_zx_ids")]
		public  string IvZxIds { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("o_id")]
		public  string OId { get; set; }
		/// <summary>
		/// 修改时间
		/// </summary>
		[Column("update_time")]
		public  DateTime? UpdateTime { get; set; }
		/// <summary>
		/// 修改人
		/// </summary>
		[Column("update_user")]
		public  string UpdateUser { get; set; }
	}

	/// <summary>
    /// 开票回款
    /// </summary>
	[Table("invoice_back")]
	public partial class InvoiceBack : Entity
	{
		/// <summary>
		/// 创建人
		/// </summary>
		[Column("create_time")]
		public  DateTime? CreateTime { get; set; }
		/// <summary>
		/// 创建人
		/// </summary>
		[Column("create_user")]
		public  string CreateUser { get; set; }
		/// <summary>
		/// 回款银行id
		/// </summary>
		[Column("ib_bk_id")]
		public  int? IbBkId { get; set; }
		/// <summary>
		/// 回款银行
		/// </summary>
		[Column("ib_bk_name")]
		public  string IbBkName { get; set; }
		/// <summary>
		/// 创收人员ids
		/// </summary>
		[Column("ib_ci_per_ids")]
		public  string IbCiPerIds { get; set; }
		/// <summary>
		/// 打款单位类型
		/// </summary>
		[Column("ib_dkdw_type")]
		public  int? IbDkdwType { get; set; }
		/// <summary>
		/// 对应会计科目名称
		/// </summary>
		[Column("ib_dykjkm")]
		public  string IbDykjkm { get; set; }
		/// <summary>
		/// 对应会计科目编码
		/// </summary>
		[Column("ib_dykjkm_code")]
		public  string IbDykjkmCode { get; set; }
		/// <summary>
		/// 对应会计科目id
		/// </summary>
		[Column("ib_dykjkm_id")]
		public  int? IbDykjkmId { get; set; }
		/// <summary>
		/// 计算方式
		/// </summary>
		[Column("ib_formula_method")]
		public  int? IbFormulaMethod { get; set; }
		/// <summary>
		/// 是否作废
		/// </summary>
		[Column("ib_is_cancel")]
		public  int? IbIsCancel { get; set; }
		/// <summary>
		/// 回款金额
		/// </summary>
		[Column("ib_money")]
		public  decimal? IbMoney { get; set; }
		/// <summary>
		/// 回款日志
		/// </summary>
		[Column("ib_remark")]
		public  string IbRemark { get; set; }
		/// <summary>
		/// 实付回款类型
		/// </summary>
		[Column("ib_sf_back_type")]
		public  int? IbSfBackType { get; set; }
		/// <summary>
		/// 实付回款明细json
		/// </summary>
		[Column("ib_sfhk_json")]
		public  string IbSfhkJson { get; set; }
		/// <summary>
		/// 实付回款明细删除json
		/// </summary>
		[Column("ib_sfhk_json_del")]
		public  string IbSfhkJsonDel { get; set; }
		/// <summary>
		/// 实付回款明细回款方式
		/// </summary>
		[Column("ib_sfhk_type")]
		public  string IbSfhkType { get; set; }
		/// <summary>
		/// 回款时间
		/// </summary>
		[Column("ib_time")]
		public  DateTime? IbTime { get; set; }
		/// <summary>
		/// 回款类型（0全部回款1部分回款）
		/// </summary>
		[Column("ib_type")]
		public  int? IbType { get; set; }
		/// <summary>
		/// id
		/// </summary>
		[Key][Identity][Column("id")]
		public override int? Id { get; set; }
		/// <summary>
		/// 发票id
		/// </summary>
		[Column("iv_id")]
		public  int? IvId { get; set; }
		/// <summary>
		/// 修改时间
		/// </summary>
		[Column("update_time")]
		public  DateTime? UpdateTime { get; set; }
		/// <summary>
		/// 修改人
		/// </summary>
		[Column("update_user")]
		public  string UpdateUser { get; set; }
	}

	/// <summary>
    /// 本系统组织架构-公司部门
    /// </summary>
	[Table("organization")]
	public partial class Organization : Entity
	{
		/// <summary>
		/// 区code
		/// </summary>
		[Column("a_code")]
		public  string ACode { get; set; }
		/// <summary>
		/// 区名称
		/// </summary>
		[Column("a_name")]
		public  string AName { get; set; }
		/// <summary>
		/// 城市code
		/// </summary>
		[Column("c_code")]
		public  string CCode { get; set; }
		/// <summary>
		/// 城市名称
		/// </summary>
		[Column("c_name")]
		public  string CName { get; set; }
		/// <summary>
		/// 钉钉部门id
		/// </summary>
		[Column("do_id")]
		public  int? DoId { get; set; }
		/// <summary>
		/// 部门类型ID
		/// </summary>
		[Column("dpt_id")]
		public  int? DptId { get; set; }
		/// <summary>
		/// id
		/// </summary>
		[Key][Identity][Column("id")]
		public override int? Id { get; set; }
		/// <summary>
		/// 简称
		/// </summary>
		[Column("og_abs")]
		public  string OgAbs { get; set; }
		/// <summary>
		/// 部门归属
		/// </summary>
		[Column("og_belong")]
		public  int? OgBelong { get; set; }
		/// <summary>
		/// 部门保证金回款管理员
		/// </summary>
		[Column("og_bzj_hk")]
		public  string OgBzjHk { get; set; }
		/// <summary>
		/// 部门保证金支付管理员
		/// </summary>
		[Column("og_bzj_zf")]
		public  string OgBzjZf { get; set; }
		/// <summary>
		/// 详细地址
		/// </summary>
		[Column("og_detail_address")]
		public  string OgDetailAddress { get; set; }
		/// <summary>
		/// 部门发票回款管理员
		/// </summary>
		[Column("og_fp_hk")]
		public  string OgFpHk { get; set; }
		/// <summary>
		/// 部门发票开票管理员
		/// </summary>
		[Column("og_fp_kp")]
		public  string OgFpKp { get; set; }
		/// <summary>
		/// 部门分管副总
		/// </summary>
		[Column("og_fz")]
		public  string OgFz { get; set; }
		/// <summary>
		/// 部门分管副总
		/// </summary>
		[Column("og_fz_name")]
		public  string OgFzName { get; set; }
		/// <summary>
		/// 部门归档管理员
		/// </summary>
		[Column("og_gd")]
		public  string OgGd { get; set; }
		/// <summary>
		/// 部门合同归档管理员
		/// </summary>
		[Column("og_ht_gd")]
		public  string OgHtGd { get; set; }
		/// <summary>
		/// 合同盖章管理员
		/// </summary>
		[Column("og_ht_gz")]
		public  string OgHtGz { get; set; }
		/// <summary>
		/// 是否独立核算
		/// </summary>
		[Column("og_is_alone")]
		public  int? OgIsAlone { get; set; }
		/// <summary>
		/// 是否有组
		/// </summary>
		[Column("og_is_group")]
		public  int? OgIsGroup { get; set; }
		/// <summary>
		/// 是否独立法人
		/// </summary>
		[Column("og_is_legalper")]
		public  int? OgIsLegalper { get; set; }
		/// <summary>
		/// 部门经理(多个，分隔）
		/// </summary>
		[Column("og_lead")]
		public  string OgLead { get; set; }
		/// <summary>
		/// 部门经理姓名
		/// </summary>
		[Column("og_lead_name")]
		public  string OgLeadName { get; set; }
		/// <summary>
		/// 部门经理助力(多个，分隔）
		/// </summary>
		[Column("og_lead_zl")]
		public  string OgLeadZl { get; set; }
		/// <summary>
		/// 法人
		/// </summary>
		[Column("og_legalper")]
		public  string OgLegalper { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		[Column("og_name")]
		public  string OgName { get; set; }
		/// <summary>
		/// 统一社会信用代码
		/// </summary>
		[Column("og_no")]
		public  string OgNo { get; set; }
		/// <summary>
		/// 办公地点
		/// </summary>
		[Column("og_office_place")]
		public  string OgOfficePlace { get; set; }
		/// <summary>
		/// 项目盖章管理员
		/// </summary>
		[Column("og_pro_gz")]
		public  string OgProGz { get; set; }
		/// <summary>
		/// 项目签发审批人
		/// </summary>
		[Column("og_qf")]
		public  string OgQf { get; set; }
		/// <summary>
		/// 注册资本
		/// </summary>
		[Column("og_registered_capital")]
		public  decimal? OgRegisteredCapital { get; set; }
		/// <summary>
		/// 部门人事主管
		/// </summary>
		[Column("og_rs_zg")]
		public  string OgRsZg { get; set; }
		/// <summary>
		/// 社保缴纳地
		/// </summary>
		[Column("og_sb_address")]
		public  string OgSbAddress { get; set; }
		/// <summary>
		/// 成立日期
		/// </summary>
		[Column("og_setup_date")]
		public  DateTime? OgSetupDate { get; set; }
		/// <summary>
		/// 排序
		/// </summary>
		[Column("og_sort")]
		public  int? OgSort { get; set; }
		/// <summary>
		/// 部门行政主管
		/// </summary>
		[Column("og_xz_zg")]
		public  string OgXzZg { get; set; }
		/// <summary>
		/// 省code
		/// </summary>
		[Column("p_code")]
		public  string PCode { get; set; }
		/// <summary>
		/// 省名称
		/// </summary>
		[Column("p_name")]
		public  string PName { get; set; }
	}

	/// <summary>
    /// 创收单-产值分配-部门
    /// </summary>
	[Table("output_value_dep")]
	public partial class OutputValueDep : Entity
	{
		/// <summary>
		/// 创建人
		/// </summary>
		[Column("create_time")]
		public  DateTime? CreateTime { get; set; }
		/// <summary>
		/// 创建人
		/// </summary>
		[Column("create_user")]
		public  string CreateUser { get; set; }
		/// <summary>
		/// id
		/// </summary>
		[Key][Identity][Column("id")]
		public override int? Id { get; set; }
		/// <summary>
		/// 追加费
		/// </summary>
		[Column("ovd_add_money")]
		public  decimal? OvdAddMoney { get; set; }
		/// <summary>
		/// 部门回款
		/// </summary>
		[Column("ovd_back_money")]
		public  decimal? OvdBackMoney { get; set; }
		/// <summary>
		/// 基本费
		/// </summary>
		[Column("ovd_base_money")]
		public  decimal? OvdBaseMoney { get; set; }
		/// <summary>
		/// 冲红原纪录id
		/// </summary>
		[Column("ovd_ch_id")]
		public  int? OvdChId { get; set; }
		/// <summary>
		/// 创收单id
		/// </summary>
		[Column("ovd_ci_id")]
		public  int? OvdCiId { get; set; }
		/// <summary>
		/// 部门名称
		/// </summary>
		[Column("ovd_dep")]
		public  string OvdDep { get; set; }
		/// <summary>
		/// 部门id
		/// </summary>
		[Column("ovd_dep_id")]
		public  int? OvdDepId { get; set; }
		/// <summary>
		/// 红冲回款
		/// </summary>
		[Column("ovd_hc_back_money")]
		public  decimal? OvdHcBackMoney { get; set; }
		/// <summary>
		/// 部门坏账回款
		/// </summary>
		[Column("ovd_hz_back_money")]
		public  decimal? OvdHzBackMoney { get; set; }
		/// <summary>
		/// 是否复核创建
		/// </summary>
		[Column("ovd_is_fhcreate")]
		public  int? OvdIsFhcreate { get; set; }
		/// <summary>
		/// 项目id
		/// </summary>
		[Column("ovd_pi_id")]
		public  int? OvdPiId { get; set; }
		/// <summary>
		/// 票据回款
		/// </summary>
		[Column("ovd_pj_back_money")]
		public  decimal? OvdPjBackMoney { get; set; }
		/// <summary>
		/// 收款方式
		/// </summary>
		[Column("ovd_sk_type")]
		public  int? OvdSkType { get; set; }
		/// <summary>
		/// 审批人
		/// </summary>
		[Column("ovd_sp_per")]
		public  string OvdSpPer { get; set; }
		/// <summary>
		/// 审批时间
		/// </summary>
		[Column("ovd_sp_time")]
		public  DateTime? OvdSpTime { get; set; }
		/// <summary>
		/// 状态（0初始1审核通过2审核不通过）
		/// </summary>
		[Column("ovd_state")]
		public  int? OvdState { get; set; }
		/// <summary>
		/// 部门正常回款
		/// </summary>
		[Column("ovd_zc_back_money")]
		public  decimal? OvdZcBackMoney { get; set; }
		/// <summary>
		/// 修改时间
		/// </summary>
		[Column("update_time")]
		public  DateTime? UpdateTime { get; set; }
		/// <summary>
		/// 修改人
		/// </summary>
		[Column("update_user")]
		public  string UpdateUser { get; set; }
	}

	/// <summary>
    /// 创收单-产值分配-人员
    /// </summary>
	[Table("output_value_per")]
	public partial class OutputValuePer : Entity
	{
		/// <summary>
		/// 创建人
		/// </summary>
		[Column("create_time")]
		public  DateTime? CreateTime { get; set; }
		/// <summary>
		/// 创建人
		/// </summary>
		[Column("create_user")]
		public  string CreateUser { get; set; }
		/// <summary>
		/// id
		/// </summary>
		[Key][Identity][Column("id")]
		public override int? Id { get; set; }
		/// <summary>
		/// 追加费
		/// </summary>
		[Column("ovp_add_money")]
		public  decimal? OvpAddMoney { get; set; }
		/// <summary>
		/// 用户id
		/// </summary>
		[Column("ovp_au_id")]
		public  int? OvpAuId { get; set; }
		/// <summary>
		/// 回款金额
		/// </summary>
		[Column("ovp_back_money")]
		public  decimal? OvpBackMoney { get; set; }
		/// <summary>
		/// 回款类型（0坏账1正常）
		/// </summary>
		[Column("ovp_back_type")]
		public  int? OvpBackType { get; set; }
		/// <summary>
		/// 基本费
		/// </summary>
		[Column("ovp_base_money")]
		public  decimal? OvpBaseMoney { get; set; }
		/// <summary>
		/// 冲红原纪录id
		/// </summary>
		[Column("ovp_ch_id")]
		public  int? OvpChId { get; set; }
		/// <summary>
		/// 创收单id
		/// </summary>
		[Column("ovp_ci_id")]
		public  int? OvpCiId { get; set; }
		/// <summary>
		/// 部门id
		/// </summary>
		[Column("ovp_dep_id")]
		public  int? OvpDepId { get; set; }
		/// <summary>
		/// 审核备注
		/// </summary>
		[Column("ovp_ex_remark")]
		public  string OvpExRemark { get; set; }
		/// <summary>
		/// 个人红冲回款
		/// </summary>
		[Column("ovp_hc_back_money")]
		public  decimal? OvpHcBackMoney { get; set; }
		/// <summary>
		/// 坏账回款金额
		/// </summary>
		[Column("ovp_hz_back_money")]
		public  decimal? OvpHzBackMoney { get; set; }
		/// <summary>
		/// 划转产值备注
		/// </summary>
		[Column("ovp_hzcz_remark")]
		public  string OvpHzczRemark { get; set; }
		/// <summary>
		/// 是否回款
		/// </summary>
		[Column("ovp_is_back")]
		public  int? OvpIsBack { get; set; }
		/// <summary>
		/// 是否已分配
		/// </summary>
		[Column("ovp_is_dis")]
		public  int? OvpIsDis { get; set; }
		/// <summary>
		/// 是否复核创建
		/// </summary>
		[Column("ovp_is_fhcreate")]
		public  int? OvpIsFhcreate { get; set; }
		/// <summary>
		/// 是否划转产值
		/// </summary>
		[Column("ovp_is_hzcz")]
		public  int? OvpIsHzcz { get; set; }
		/// <summary>
		/// 是否是扣款
		/// </summary>
		[Column("ovp_is_kk")]
		public  int? OvpIsKk { get; set; }
		/// <summary>
		/// 是否外聘
		/// </summary>
		[Column("ovp_is_wp")]
		public  int? OvpIsWp { get; set; }
		/// <summary>
		/// 扣款类型
		/// </summary>
		[Column("ovp_kk_type")]
		public  int? OvpKkType { get; set; }
		/// <summary>
		/// 人员姓名
		/// </summary>
		[Column("ovp_per_name")]
		public  string OvpPerName { get; set; }
		/// <summary>
		/// 项目id
		/// </summary>
		[Column("ovp_pi_id")]
		public  int? OvpPiId { get; set; }
		/// <summary>
		/// 个人票据回款
		/// </summary>
		[Column("ovp_pj_back_money")]
		public  decimal? OvpPjBackMoney { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		[Column("ovp_remark")]
		public  string OvpRemark { get; set; }
		/// <summary>
		/// 二级复核核差
		/// </summary>
		[Column("ovp_sec_fhhc")]
		public  decimal? OvpSecFhhc { get; set; }
		/// <summary>
		/// 二级复核已扣款
		/// </summary>
		[Column("ovp_sec_fhykk")]
		public  decimal? OvpSecFhykk { get; set; }
		/// <summary>
		/// 收款方式
		/// </summary>
		[Column("ovp_sk_type")]
		public  int? OvpSkType { get; set; }
		/// <summary>
		/// 审批时间
		/// </summary>
		[Column("ovp_sp_time")]
		public  DateTime? OvpSpTime { get; set; }
		/// <summary>
		/// 状态（0初始1审核通过2审核不通过）
		/// </summary>
		[Column("ovp_state")]
		public  int? OvpState { get; set; }
		/// <summary>
		/// 三级复核核差
		/// </summary>
		[Column("ovp_th_fhhc")]
		public  decimal? OvpThFhhc { get; set; }
		/// <summary>
		/// 三级复核已扣款
		/// </summary>
		[Column("ovp_th_fhykk")]
		public  decimal? OvpThFhykk { get; set; }
		/// <summary>
		/// 外聘人员姓名
		/// </summary>
		[Column("ovp_wp_name")]
		public  string OvpWpName { get; set; }
		/// <summary>
		/// 归属年份
		/// </summary>
		[Column("ovp_year")]
		public  int? OvpYear { get; set; }
		/// <summary>
		/// 正常回款金额
		/// </summary>
		[Column("ovp_zc_back_money")]
		public  decimal? OvpZcBackMoney { get; set; }
		/// <summary>
		/// 修改时间
		/// </summary>
		[Column("update_time")]
		public  DateTime? UpdateTime { get; set; }
		/// <summary>
		/// 修改人
		/// </summary>
		[Column("update_user")]
		public  string UpdateUser { get; set; }
	}

	/// <summary>
    /// 项目信息
    /// </summary>
	[Table("project_info")]
	public partial class ProjectInfo : Entity
	{
		/// <summary>
		/// 区code
		/// </summary>
		[Column("a_code")]
		public  string ACode { get; set; }
		/// <summary>
		/// 区
		/// </summary>
		[Column("a_name")]
		public  string AName { get; set; }
		/// <summary>
		/// 城市code
		/// </summary>
		[Column("c_code")]
		public  string CCode { get; set; }
		/// <summary>
		/// 城市
		/// </summary>
		[Column("c_name")]
		public  string CName { get; set; }
		/// <summary>
		/// 项目创建时间
		/// </summary>
		[Column("create_time")]
		public  DateTime? CreateTime { get; set; }
		/// <summary>
		/// 创建人
		/// </summary>
		[Column("create_user")]
		public  string CreateUser { get; set; }
		/// <summary>
		/// 项目创建审核通过时间
		/// </summary>
		[Column("examine_time")]
		public  DateTime? ExamineTime { get; set; }
		/// <summary>
		/// 审核人
		/// </summary>
		[Column("examine_user")]
		public  string ExamineUser { get; set; }
		/// <summary>
		/// 指标模板id
		/// </summary>
		[Column("gld_zit_id")]
		public  int? GldZitId { get; set; }
		/// <summary>
		/// id
		/// </summary>
		[Key][Identity][Column("id")]
		public override int? Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("o_id")]
		public  string OId { get; set; }
		/// <summary>
		/// 省code
		/// </summary>
		[Column("p_code")]
		public  string PCode { get; set; }
		/// <summary>
		/// 省
		/// </summary>
		[Column("p_name")]
		public  string PName { get; set; }
		/// <summary>
		/// 合同特征ids
		/// </summary>
		[Column("pf_id")]
		public  string PfId { get; set; }
		/// <summary>
		/// 合同特征名称
		/// </summary>
		[Column("pf_name")]
		public  string PfName { get; set; }
		/// <summary>
		/// 业绩人员，分隔
		/// </summary>
		[Column("pi_achievement_per")]
		public  string PiAchievementPer { get; set; }
		/// <summary>
		/// 详细地址
		/// </summary>
		[Column("pi_address")]
		public  string PiAddress { get; set; }
		/// <summary>
		/// 建筑面积
		/// </summary>
		[Column("pi_architect_area")]
		public  decimal? PiArchitectArea { get; set; }
		/// <summary>
		/// 建筑装配率
		/// </summary>
		[Column("pi_architect_cent")]
		public  decimal? PiArchitectCent { get; set; }
		/// <summary>
		/// 建筑类型(0 公用建筑, 1民用建筑)
		/// </summary>
		[Column("pi_architect_type")]
		public  int? PiArchitectType { get; set; }
		/// <summary>
		/// 创建人Id
		/// </summary>
		[Column("pi_au_id")]
		public  int? PiAuId { get; set; }
		/// <summary>
		/// 项目归属
		/// </summary>
		[Column("pi_belong")]
		public  int? PiBelong { get; set; }
		/// <summary>
		/// 项目部门归属
		/// </summary>
		[Column("pi_belong_dep")]
		public  int? PiBelongDep { get; set; }
		/// <summary>
		/// 项目归属类型(0全过程咨询项目1 全过程跟踪审计项目）
		/// </summary>
		[Column("pi_belong_type")]
		public  int? PiBelongType { get; set; }
		/// <summary>
		/// 报告号
		/// </summary>
		[Column("pi_bg_no")]
		public  string PiBgNo { get; set; }
		/// <summary>
		/// 获取报告号时间
		/// </summary>
		[Column("pi_bgh_time")]
		public  DateTime? PiBghTime { get; set; }
		/// <summary>
		/// 要求报告完成时间
		/// </summary>
		[Column("pi_bgwc_time")]
		public  DateTime? PiBgwcTime { get; set; }
		/// <summary>
		/// 一级业务名称
		/// </summary>
		[Column("pi_bt_name")]
		public  string PiBtName { get; set; }
		/// <summary>
		/// 二级业务名称
		/// </summary>
		[Column("pi_bt_sec_name")]
		public  string PiBtSecName { get; set; }
		/// <summary>
		/// 业务阶段
		/// </summary>
		[Column("pi_bt_stage")]
		public  int? PiBtStage { get; set; }
		/// <summary>
		/// 作业类型：0编制类，其他都是审核类
		/// </summary>
		[Column("pi_bt_type")]
		public  int? PiBtType { get; set; }
		/// <summary>
		/// 一级业务类型
		/// </summary>
		[Column("pi_btid")]
		public  int? PiBtid { get; set; }
		/// <summary>
		/// 建筑用途ids
		/// </summary>
		[Column("pi_build_purpose_ids")]
		public  string PiBuildPurposeIds { get; set; }
		/// <summary>
		/// 建筑用途名称
		/// </summary>
		[Column("pi_build_purpose_name")]
		public  string PiBuildPurposeName { get; set; }
		/// <summary>
		/// 建筑面积
		/// </summary>
		[Column("pi_building_area")]
		public  float? PiBuildingArea { get; set; }
		/// <summary>
		/// 建筑高度
		/// </summary>
		[Column("pi_building_height")]
		public  float? PiBuildingHeight { get; set; }
		/// <summary>
		/// 工程总承包模式
		/// </summary>
		[Column("pi_cb_mode")]
		public  int? PiCbMode { get; set; }
		/// <summary>
		/// 要求初稿完成时间
		/// </summary>
		[Column("pi_cgwc_time")]
		public  DateTime? PiCgwcTime { get; set; }
		/// <summary>
		/// 创收数量
		/// </summary>
		[Column("pi_ci_num")]
		public  int? PiCiNum { get; set; }
		/// <summary>
		/// 报告号规则Id
		/// </summary>
		[Column("pi_coderule_id")]
		public  int? PiCoderuleId { get; set; }
		/// <summary>
		/// 数字
		/// </summary>
		[Column("pi_coderule_num")]
		public  int? PiCoderuleNum { get; set; }
		/// <summary>
		/// 合同价形式
		/// </summary>
		[Column("pi_contract_price_type")]
		public  int? PiContractPriceType { get; set; }
		/// <summary>
		/// 合同类型
		/// </summary>
		[Column("pi_contract_type")]
		public  int? PiContractType { get; set; }
		/// <summary>
		/// 项目创建审批流
		/// </summary>
		[Column("pi_create_lc_type")]
		public  int? PiCreateLcType { get; set; }
		/// <summary>
		/// 对应合同
		/// </summary>
		[Column("pi_ct_id")]
		public  int? PiCtId { get; set; }
		/// <summary>
		/// 整体项目id
		/// </summary>
		[Column("pi_ctp_id")]
		public  int? PiCtpId { get; set; }
		/// <summary>
		/// 删除的json
		/// </summary>
		[Column("pi_customer_del_json")]
		public  string PiCustomerDelJson { get; set; }
		/// <summary>
		/// 项目单位JSON
		/// </summary>
		[Column("pi_customer_json")]
		public  string PiCustomerJson { get; set; }
		/// <summary>
		/// 定案表文件
		/// </summary>
		[Column("pi_dab_file")]
		public  string PiDabFile { get; set; }
		/// <summary>
		/// 牵头部门
		/// </summary>
		[Column("pi_dep")]
		public  int? PiDep { get; set; }
		/// <summary>
		/// 对应人员部门小组
		/// </summary>
		[Column("pi_dep_group")]
		public  int? PiDepGroup { get; set; }
		/// <summary>
		/// 项目经理
		/// </summary>
		[Column("pi_dep_lead")]
		public  int? PiDepLead { get; set; }
		/// <summary>
		/// 代建单位
		/// </summary>
		[Column("pi_dj_unit")]
		public  string PiDjUnit { get; set; }
		/// <summary>
		/// 地下楼层数
		/// </summary>
		[Column("pi_down_floor_num")]
		public  int? PiDownFloorNum { get; set; }
		/// <summary>
		/// 第三方复核建议
		/// </summary>
		[Column("pi_dsffh_proposal")]
		public  string PiDsffhProposal { get; set; }
		/// <summary>
		/// 第三方复核建议文件
		/// </summary>
		[Column("pi_dsffh_proposal_file")]
		public  string PiDsffhProposalFile { get; set; }
		/// <summary>
		/// 竣工日期
		/// </summary>
		[Column("pi_end_date")]
		public  DateTime? PiEndDate { get; set; }
		/// <summary>
		/// 是否已经第三方复核了
		/// </summary>
		[Column("pi_end_dsf")]
		public  int? PiEndDsf { get; set; }
		/// <summary>
		/// 结束二级复核人id
		/// </summary>
		[Column("pi_end_secfh_per_id")]
		public  int? PiEndSecfhPerId { get; set; }
		/// <summary>
		/// 结束二级复核人
		/// </summary>
		[Column("pi_end_secfh_per_name")]
		public  string PiEndSecfhPerName { get; set; }
		/// <summary>
		/// 复核用章人
		/// </summary>
		[Column("pi_fhyz_per")]
		public  string PiFhyzPer { get; set; }
		/// <summary>
		/// 复核用章人json
		/// </summary>
		[Column("pi_fhyz_per_json")]
		public  string PiFhyzPerJson { get; set; }
		/// <summary>
		/// 复核用章人姓名
		/// </summary>
		[Column("pi_fhyz_per_name")]
		public  string PiFhyzPerName { get; set; }
		/// <summary>
		/// 首次盖章时间
		/// </summary>
		[Column("pi_first_gz_time")]
		public  DateTime? PiFirstGzTime { get; set; }
		/// <summary>
		/// 分配人
		/// </summary>
		[Column("pi_fp_per")]
		public  string PiFpPer { get; set; }
		/// <summary>
		/// 工程资金来源
		/// </summary>
		[Column("pi_gc_source_type")]
		public  int? PiGcSourceType { get; set; }
		/// <summary>
		/// 归档截止日期
		/// </summary>
		[Column("pi_gd_endtime")]
		public  DateTime? PiGdEndtime { get; set; }
		/// <summary>
		/// 编号
		/// </summary>
		[Column("pi_gd_no")]
		public  string PiGdNo { get; set; }
		/// <summary>
		/// 归档册数
		/// </summary>
		[Column("pi_gd_num")]
		public  string PiGdNum { get; set; }
		/// <summary>
		/// 归档是否逾期
		/// </summary>
		[Column("pi_gd_overdue")]
		public  int? PiGdOverdue { get; set; }
		/// <summary>
		/// 归档管理员id
		/// </summary>
		[Column("pi_gd_per")]
		public  string PiGdPer { get; set; }
		/// <summary>
		/// 归档管理员姓名
		/// </summary>
		[Column("pi_gd_per_name")]
		public  string PiGdPerName { get; set; }
		/// <summary>
		/// 提交归档时间
		/// </summary>
		[Column("pi_gd_submit_time")]
		public  DateTime? PiGdSubmitTime { get; set; }
		/// <summary>
		/// 归档打回备注
		/// </summary>
		[Column("pi_gdback_remark")]
		public  string PiGdbackRemark { get; set; }
		/// <summary>
		/// 归档接受时间
		/// </summary>
		[Column("pi_gdjs_time")]
		public  DateTime? PiGdjsTime { get; set; }
		/// <summary>
		/// 归档完成时间
		/// </summary>
		[Column("pi_gdwc_time")]
		public  DateTime? PiGdwcTime { get; set; }
		/// <summary>
		/// 广联达模板id
		/// </summary>
		[Column("pi_gld_templeteid")]
		public  int? PiGldTempleteid { get; set; }
		/// <summary>
		/// 一级行业名称
		/// </summary>
		[Column("pi_gld_tt_name")]
		public  string PiGldTtName { get; set; }
		/// <summary>
		/// 一级行业类型
		/// </summary>
		[Column("pi_gld_ttid")]
		public  int? PiGldTtid { get; set; }
		/// <summary>
		/// 广联达类型
		/// </summary>
		[Column("pi_gld_type")]
		public  int? PiGldType { get; set; }
		/// <summary>
		/// 盖章流程id
		/// </summary>
		[Column("pi_gz_lc")]
		public  int? PiGzLc { get; set; }
		/// <summary>
		/// 盖章人
		/// </summary>
		[Column("pi_gz_per")]
		public  string PiGzPer { get; set; }
		/// <summary>
		/// 盖章提交时间
		/// </summary>
		[Column("pi_gz_time")]
		public  DateTime? PiGzTime { get; set; }
		/// <summary>
		/// 盖章归属时间
		/// </summary>
		[Column("pi_gzgs_time")]
		public  DateTime? PiGzgsTime { get; set; }
		/// <summary>
		/// 精装份数
		/// </summary>
		[Column("pi_hardcover_num")]
		public  string PiHardcoverNum { get; set; }
		/// <summary>
		/// 是否是历史项目
		/// </summary>
		[Column("pi_his")]
		public  int? PiHis { get; set; }
		/// <summary>
		/// 核减
		/// </summary>
		[Column("pi_hj")]
		public  decimal? PiHj { get; set; }
		/// <summary>
		/// 核增
		/// </summary>
		[Column("pi_hz")]
		public  decimal? PiHz { get; set; }
		/// <summary>
		/// 高新收入归属1高新收入，0一般收入
		/// </summary>
		[Column("pi_income_type")]
		public  int? PiIncomeType { get; set; }
		/// <summary>
		/// 是否归属全过程咨询
		/// </summary>
		[Column("pi_is_belong_pm")]
		public  int? PiIsBelongPm { get; set; }
		/// <summary>
		/// 是否可借阅
		/// </summary>
		[Column("pi_is_borrow")]
		public  int? PiIsBorrow { get; set; }
		/// <summary>
		/// 是否作废
		/// </summary>
		[Column("pi_is_cancel")]
		public  int? PiIsCancel { get; set; }
		/// <summary>
		/// 是否案例分析
		/// </summary>
		[Column("pi_is_casefx")]
		public  int? PiIsCasefx { get; set; }
		/// <summary>
		/// 是否是公司业绩
		/// </summary>
		[Column("pi_is_compamy_achievement")]
		public  int? PiIsCompamyAchievement { get; set; }
		/// <summary>
		/// 定案表是否齐全
		/// </summary>
		[Column("pi_is_dab")]
		public  int? PiIsDab { get; set; }
		/// <summary>
		/// 是否需要第三方复核
		/// </summary>
		[Column("pi_is_dsf")]
		public  int? PiIsDsf { get; set; }
		/// <summary>
		/// 是否是评价表
		/// </summary>
		[Column("pi_is_evaluate")]
		public  int? PiIsEvaluate { get; set; }
		/// <summary>
		/// 是否关注项目
		/// </summary>
		[Column("pi_is_follow_pro")]
		public  int? PiIsFollowPro { get; set; }
		/// <summary>
		/// 是否有指标需求
		/// </summary>
		[Column("pi_is_gld_zbxq")]
		public  int? PiIsGldZbxq { get; set; }
		/// <summary>
		/// 部门是否有小组
		/// </summary>
		[Column("pi_is_group")]
		public  int? PiIsGroup { get; set; }
		/// <summary>
		/// 是否是历史遗留项目
		/// </summary>
		[Column("pi_is_his_legacy")]
		public  int? PiIsHisLegacy { get; set; }
		/// <summary>
		/// 是否加入项目指标库
		/// </summary>
		[Column("pi_is_join_pro_zb")]
		public  int? PiIsJoinProZb { get; set; }
		/// <summary>
		/// 是否是个人业绩
		/// </summary>
		[Column("pi_is_per_achievement")]
		public  int? PiIsPerAchievement { get; set; }
		/// <summary>
		/// 是否品茗暂存
		/// </summary>
		[Column("pi_is_pm_zc")]
		public  int? PiIsPmZc { get; set; }
		/// <summary>
		/// 项目是否退回待归档
		/// </summary>
		[Column("pi_is_retreated")]
		public  int? PiIsRetreated { get; set; }
		/// <summary>
		/// 是否是简易项目
		/// </summary>
		[Column("pi_is_simple")]
		public  int? PiIsSimple { get; set; }
		/// <summary>
		/// 是否需要三级复核
		/// </summary>
		[Column("pi_is_th")]
		public  int? PiIsTh { get; set; }
		/// <summary>
		/// 是否三级分配过
		/// </summary>
		[Column("pi_is_thfp")]
		public  int? PiIsThfp { get; set; }
		/// <summary>
		/// 是否外部复核
		/// </summary>
		[Column("pi_is_wb_fh")]
		public  int? PiIsWbFh { get; set; }
		/// <summary>
		/// 是否外部复核新
		/// </summary>
		[Column("pi_is_wb_fh_new")]
		public  int? PiIsWbFhNew { get; set; }
		/// <summary>
		/// 是否线下接收
		/// </summary>
		[Column("pi_is_xxjs")]
		public  int? PiIsXxjs { get; set; }
		/// <summary>
		/// 有无印章要求
		/// </summary>
		[Column("pi_is_yzyq")]
		public  int? PiIsYzyq { get; set; }
		/// <summary>
		/// 是否指标分析
		/// </summary>
		[Column("pi_is_zbfx")]
		public  int? PiIsZbfx { get; set; }
		/// <summary>
		/// 是否指标录入
		/// </summary>
		[Column("pi_is_zblr")]
		public  int? PiIsZblr { get; set; }
		/// <summary>
		/// 是否有指标需求
		/// </summary>
		[Column("pi_is_zbxq")]
		public  int? PiIsZbxq { get; set; }
		/// <summary>
		/// 是否建筑工程解析
		/// </summary>
		[Column("pi_is_zjgc")]
		public  int? PiIsZjgc { get; set; }
		/// <summary>
		/// 是否提交到广联达
		/// </summary>
		[Column("pi_issub_gld")]
		public  int? PiIssubGld { get; set; }
		/// <summary>
		/// 是否暂存
		/// </summary>
		[Column("pi_iszc")]
		public  int? PiIszc { get; set; }
		/// <summary>
		/// 开票数量
		/// </summary>
		[Column("pi_iv_num")]
		public  int? PiIvNum { get; set; }
		/// <summary>
		/// 监理单位
		/// </summary>
		[Column("pi_jl_unit")]
		public  string PiJlUnit { get; set; }
		/// <summary>
		/// 加入指标时间
		/// </summary>
		[Column("pi_join_zb_time")]
		public  DateTime? PiJoinZbTime { get; set; }
		/// <summary>
		/// 建设单位
		/// </summary>
		[Column("pi_js_name")]
		public  string PiJsName { get; set; }
		/// <summary>
		/// 生成结算报告时间
		/// </summary>
		[Column("pi_jsbg_create_time")]
		public  DateTime? PiJsbgCreateTime { get; set; }
		/// <summary>
		/// 结算报告可选项
		/// </summary>
		[Column("pi_jsbg_options")]
		public  string PiJsbgOptions { get; set; }
		/// <summary>
		/// 结算报告模板id
		/// </summary>
		[Column("pi_jsbg_tpl_id")]
		public  int? PiJsbgTplId { get; set; }
		/// <summary>
		/// 项目经理专业
		/// </summary>
		[Column("pi_lead_major")]
		public  int? PiLeadMajor { get; set; }
		/// <summary>
		/// 项目经理名称 
		/// </summary>
		[Column("pi_lead_name")]
		public  string PiLeadName { get; set; }
		/// <summary>
		/// 项目廉政状态
		/// </summary>
		[Column("pi_lz_state")]
		public  int? PiLzState { get; set; }
		/// <summary>
		/// 项目名称
		/// </summary>
		[Column("pi_name")]
		public  string PiName { get; set; }
		/// <summary>
		/// 其他人员id
		/// </summary>
		[Column("pi_other_per_id")]
		public  string PiOtherPerId { get; set; }
		/// <summary>
		/// 其他人员名称
		/// </summary>
		[Column("pi_other_per_name")]
		public  string PiOtherPerName { get; set; }
		/// <summary>
		/// 其他印刷要求
		/// </summary>
		[Column("pi_other_ysyq")]
		public  string PiOtherYsyq { get; set; }
		/// <summary>
		/// 其他用章要求
		/// </summary>
		[Column("pi_other_yzyq")]
		public  string PiOtherYzyq { get; set; }
		/// <summary>
		/// 简装份数
		/// </summary>
		[Column("pi_paperback_num")]
		public  int? PiPaperbackNum { get; set; }
		/// <summary>
		/// 项目成员
		/// </summary>
		[Column("pi_per_ids")]
		public  string PiPerIds { get; set; }
		/// <summary>
		/// 全过程咨询项目id
		/// </summary>
		[Column("pi_pm_id")]
		public  int? PiPmId { get; set; }
		/// <summary>
		/// 全过程咨询名称
		/// </summary>
		[Column("pi_pm_name")]
		public  string PiPmName { get; set; }
		/// <summary>
		/// 权限节点
		/// </summary>
		[Column("pi_power")]
		public  string PiPower { get; set; }
		/// <summary>
		/// 打印json
		/// </summary>
		[Column("pi_print_json")]
		public  string PiPrintJson { get; set; }
		/// <summary>
		/// 签发人
		/// </summary>
		[Column("pi_qf_per")]
		public  string PiQfPer { get; set; }
		/// <summary>
		/// 签发审批人Id
		/// </summary>
		[Column("pi_qf_per_id")]
		public  string PiQfPerId { get; set; }
		/// <summary>
		/// 项目签发备注
		/// </summary>
		[Column("pi_qf_remark")]
		public  string PiQfRemark { get; set; }
		/// <summary>
		/// 项目经理签发提交时间
		/// </summary>
		[Column("pi_qf_submit_time")]
		public  DateTime? PiQfSubmitTime { get; set; }
		/// <summary>
		/// 签发审核通过时间
		/// </summary>
		[Column("pi_qf_time")]
		public  DateTime? PiQfTime { get; set; }
		/// <summary>
		/// 签发审批流
		/// </summary>
		[Column("pi_qf_type")]
		public  int? PiQfType { get; set; }
		/// <summary>
		/// 签发完成时间（提交文印室复核时间）
		/// </summary>
		[Column("pi_qfwc_time")]
		public  DateTime? PiQfwcTime { get; set; }
		/// <summary>
		/// 质量控制流程单
		/// </summary>
		[Column("pi_quality_control_flow_file")]
		public  string PiQualityControlFlowFile { get; set; }
		/// <summary>
		/// 合同关联次数
		/// </summary>
		[Column("pi_re_ct_count")]
		public  int? PiReCtCount { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		[Column("pi_remark")]
		public  string PiRemark { get; set; }
		/// <summary>
		/// 审定价
		/// </summary>
		[Column("pi_sd_money")]
		public  decimal? PiSdMoney { get; set; }
		/// <summary>
		/// 审定人id
		/// </summary>
		[Column("pi_sd_per")]
		public  string PiSdPer { get; set; }
		/// <summary>
		/// 审定人json
		/// </summary>
		[Column("pi_sd_per_json")]
		public  string PiSdPerJson { get; set; }
		/// <summary>
		/// 审定人姓名
		/// </summary>
		[Column("pi_sd_per_name")]
		public  string PiSdPerName { get; set; }
		/// <summary>
		/// 二级业务类型
		/// </summary>
		[Column("pi_sec_btid")]
		public  int? PiSecBtid { get; set; }
		/// <summary>
		/// 二级复核人
		/// </summary>
		[Column("pi_sec_per")]
		public  string PiSecPer { get; set; }
		/// <summary>
		/// 二级复核提交时间
		/// </summary>
		[Column("pi_sec_submit_time")]
		public  DateTime? PiSecSubmitTime { get; set; }
		/// <summary>
		/// 二级行业类型
		/// </summary>
		[Column("pi_sec_ttid")]
		public  int? PiSecTtid { get; set; }
		/// <summary>
		/// 二级复核完成时间
		/// </summary>
		[Column("pi_secwc_time")]
		public  DateTime? PiSecwcTime { get; set; }
		/// <summary>
		/// 施工单位
		/// </summary>
		[Column("pi_sg_name")]
		public  string PiSgName { get; set; }
		/// <summary>
		/// 实际签发人ID
		/// </summary>
		[Column("pi_sj_qfper_id")]
		public  int? PiSjQfperId { get; set; }
		/// <summary>
		/// 设计单位
		/// </summary>
		[Column("pi_sj_unit")]
		public  string PiSjUnit { get; set; }
		/// <summary>
		/// 实际盖章人
		/// </summary>
		[Column("pi_sjgz_per")]
		public  string PiSjgzPer { get; set; }
		/// <summary>
		/// 实际盖章时间
		/// </summary>
		[Column("pi_sjgz_time")]
		public  DateTime? PiSjgzTime { get; set; }
		/// <summary>
		/// 实际文印复核人id
		/// </summary>
		[Column("pi_sjwy_per")]
		public  int? PiSjwyPer { get; set; }
		/// <summary>
		/// 实际文印复核人姓名
		/// </summary>
		[Column("pi_sjwy_per_name")]
		public  string PiSjwyPerName { get; set; }
		/// <summary>
		/// 实际文字复核人id
		/// </summary>
		[Column("pi_sjwz_per")]
		public  int? PiSjwzPer { get; set; }
		/// <summary>
		/// 实际文字复核人姓名
		/// </summary>
		[Column("pi_sjwz_per_name")]
		public  string PiSjwzPerName { get; set; }
		/// <summary>
		/// 实际用章人json
		/// </summary>
		[Column("pi_sjyz_json")]
		public  string PiSjyzJson { get; set; }
		/// <summary>
		/// 实际用章人姓名
		/// </summary>
		[Column("pi_sjyz_name")]
		public  string PiSjyzName { get; set; }
		/// <summary>
		/// 实际用章人
		/// </summary>
		[Column("pi_sjyz_per")]
		public  string PiSjyzPer { get; set; }
		/// <summary>
		/// 实际作业期结束时间
		/// </summary>
		[Column("pi_sjzyq_end_time")]
		public  DateTime? PiSjzyqEndTime { get; set; }
		/// <summary>
		/// 实际作业期开始时间
		/// </summary>
		[Column("pi_sjzyq_start_time")]
		public  DateTime? PiSjzyqStartTime { get; set; }
		/// <summary>
		/// 补充报告数量
		/// </summary>
		[Column("pi_sr_num")]
		public  int? PiSrNum { get; set; }
		/// <summary>
		/// 补充报告纸质接收数量
		/// </summary>
		[Column("pi_sr_zzjs_num")]
		public  int? PiSrZzjsNum { get; set; }
		/// <summary>
		/// 送审总价
		/// </summary>
		[Column("pi_ss_money")]
		public  decimal? PiSsMoney { get; set; }
		/// <summary>
		/// 开工日期
		/// </summary>
		[Column("pi_start_date")]
		public  DateTime? PiStartDate { get; set; }
		/// <summary>
		/// 项目开始时间
		/// </summary>
		[Column("pi_start_time")]
		public  DateTime? PiStartTime { get; set; }
		/// <summary>
		/// 项目状态
		/// </summary>
		[Column("pi_state")]
		public  int? PiState { get; set; }
		/// <summary>
		/// 存放位置
		/// </summary>
		[Column("pi_storage_location")]
		public  string PiStorageLocation { get; set; }
		/// <summary>
		/// 总建安造价金额
		/// </summary>
		[Column("pi_sum_assemble_cost_money")]
		public  decimal? PiSumAssembleCostMoney { get; set; }
		/// <summary>
		/// 总项目投资金额
		/// </summary>
		[Column("pi_sum_investment_amount")]
		public  decimal? PiSumInvestmentAmount { get; set; }
		/// <summary>
		/// 三级复核人
		/// </summary>
		[Column("pi_th_per")]
		public  string PiThPer { get; set; }
		/// <summary>
		/// 项目经理三级复核提交时间
		/// </summary>
		[Column("pi_th_submit_time")]
		public  DateTime? PiThSubmitTime { get; set; }
		/// <summary>
		/// 第三方复核单位
		/// </summary>
		[Column("pi_thfh_company")]
		public  string PiThfhCompany { get; set; }
		/// <summary>
		/// 实际三级复核分配人
		/// </summary>
		[Column("pi_thfhfp_per")]
		public  int? PiThfhfpPer { get; set; }
		/// <summary>
		/// 三级分配人
		/// </summary>
		[Column("pi_thfp_per")]
		public  string PiThfpPer { get; set; }
		/// <summary>
		/// 三级复核分配时间
		/// </summary>
		[Column("pi_thfp_time")]
		public  DateTime? PiThfpTime { get; set; }
		/// <summary>
		/// 三级复核完成时间
		/// </summary>
		[Column("pi_thwc_time")]
		public  DateTime? PiThwcTime { get; set; }
		/// <summary>
		/// 项目体量
		/// </summary>
		[Column("pi_tl")]
		public  int? PiTl { get; set; }
		/// <summary>
		/// 一级行业名称
		/// </summary>
		[Column("pi_tt_name")]
		public  string PiTtName { get; set; }
		/// <summary>
		/// 二级行业名称
		/// </summary>
		[Column("pi_tt_sec_name")]
		public  string PiTtSecName { get; set; }
		/// <summary>
		/// 一级行业类型
		/// </summary>
		[Column("pi_ttid")]
		public  int? PiTtid { get; set; }
		/// <summary>
		/// 投资类型（0国有投资1非国有投资）
		/// </summary>
		[Column("pi_tz_type")]
		public  int? PiTzType { get; set; }
		/// <summary>
		/// 地上楼层数
		/// </summary>
		[Column("pi_up_floor_num")]
		public  int? PiUpFloorNum { get; set; }
		/// <summary>
		/// 外部核减
		/// </summary>
		[Column("pi_wb_hj")]
		public  decimal? PiWbHj { get; set; }
		/// <summary>
		/// 外部核增
		/// </summary>
		[Column("pi_wb_hz")]
		public  decimal? PiWbHz { get; set; }
		/// <summary>
		/// 外部审定价
		/// </summary>
		[Column("pi_wb_sd_money")]
		public  decimal? PiWbSdMoney { get; set; }
		/// <summary>
		/// 外部预算价
		/// </summary>
		[Column("pi_wb_ys_price")]
		public  decimal? PiWbYsPrice { get; set; }
		/// <summary>
		/// 项目完成情况：正常完成，项目延期
		/// </summary>
		[Column("pi_wcqk")]
		public  int? PiWcqk { get; set; }
		/// <summary>
		/// 委托单位名称
		/// </summary>
		[Column("pi_wt_name")]
		public  string PiWtName { get; set; }
		/// <summary>
		/// 文印复核人
		/// </summary>
		[Column("pi_wy_per")]
		public  string PiWyPer { get; set; }
		/// <summary>
		/// 文印室复核通过时间
		/// </summary>
		[Column("pi_wystg_time")]
		public  DateTime? PiWystgTime { get; set; }
		/// <summary>
		/// 文字复核人
		/// </summary>
		[Column("pi_wz_per")]
		public  string PiWzPer { get; set; }
		/// <summary>
		/// 文字复核通过时间
		/// </summary>
		[Column("pi_wzfhtg_time")]
		public  DateTime? PiWzfhtgTime { get; set; }
		/// <summary>
		/// 项目负责人
		/// </summary>
		[Column("pi_xmfz_per")]
		public  string PiXmfzPer { get; set; }
		/// <summary>
		/// 项目负责人json
		/// </summary>
		[Column("pi_xmfz_per_json")]
		public  string PiXmfzPerJson { get; set; }
		/// <summary>
		/// 项目负责人名称
		/// </summary>
		[Column("pi_xmfz_per_name")]
		public  string PiXmfzPerName { get; set; }
		/// <summary>
		/// 小数点保留位数
		/// </summary>
		[Column("pi_xsd")]
		public  int? PiXsd { get; set; }
		/// <summary>
		/// 纸质报告线下接收时间
		/// </summary>
		[Column("pi_xxjs_time")]
		public  DateTime? PiXxjsTime { get; set; }
		/// <summary>
		/// 预计追加费收入
		/// </summary>
		[Column("pi_yj_add_fee")]
		public  decimal? PiYjAddFee { get; set; }
		/// <summary>
		/// 预计基本费收入
		/// </summary>
		[Column("pi_yj_base_fee")]
		public  decimal? PiYjBaseFee { get; set; }
		/// <summary>
		/// 预计总收入
		/// </summary>
		[Column("pi_yj_total_fee")]
		public  decimal? PiYjTotalFee { get; set; }
		/// <summary>
		/// 录入审批备注
		/// </summary>
		[Column("pi_yjk_entry_examine_remark")]
		public  string PiYjkEntryExamineRemark { get; set; }
		/// <summary>
		/// 录入要求
		/// </summary>
		[Column("pi_yjk_entry_requirement")]
		public  string PiYjkEntryRequirement { get; set; }
		/// <summary>
		/// 业绩库审批状态
		/// </summary>
		[Column("pi_yjk_examine_state")]
		public  int? PiYjkExamineState { get; set; }
		/// <summary>
		/// 业绩库录入方式
		/// </summary>
		[Column("pi_yjk_input_type")]
		public  int? PiYjkInputType { get; set; }
		/// <summary>
		/// 业绩库审批是否需要审批
		/// </summary>
		[Column("pi_yjk_is_need_examine")]
		public  int? PiYjkIsNeedExamine { get; set; }
		/// <summary>
		/// 业绩库审批流
		/// </summary>
		[Column("pi_yjk_lc_type")]
		public  int? PiYjkLcType { get; set; }
		/// <summary>
		/// 业绩库权限
		/// </summary>
		[Column("pi_yjk_power")]
		public  string PiYjkPower { get; set; }
		/// <summary>
		/// 业绩库审批发起人部门id
		/// </summary>
		[Column("pi_yjk_start_dep_id")]
		public  string PiYjkStartDepId { get; set; }
		/// <summary>
		/// 业绩库审批发起人部门名称
		/// </summary>
		[Column("pi_yjk_start_dep_name")]
		public  string PiYjkStartDepName { get; set; }
		/// <summary>
		/// 业绩库审批发起人id
		/// </summary>
		[Column("pi_yjk_start_per_id")]
		public  int? PiYjkStartPerId { get; set; }
		/// <summary>
		/// 业绩库审批发起人名称
		/// </summary>
		[Column("pi_yjk_start_per_name")]
		public  string PiYjkStartPerName { get; set; }
		/// <summary>
		/// 业绩库发起时间
		/// </summary>
		[Column("pi_yjk_start_time")]
		public  DateTime? PiYjkStartTime { get; set; }
		/// <summary>
		/// 项目预计收费
		/// </summary>
		[Column("pi_yjsf")]
		public  decimal? PiYjsf { get; set; }
		/// <summary>
		/// 预算价
		/// </summary>
		[Column("pi_ys_price")]
		public  decimal? PiYsPrice { get; set; }
		/// <summary>
		/// 业态其他说明
		/// </summary>
		[Column("pi_yt_remark")]
		public  string PiYtRemark { get; set; }
		/// <summary>
		/// 用章人员证书
		/// </summary>
		[Column("pi_yz_per_cert")]
		public  int? PiYzPerCert { get; set; }
		/// <summary>
		/// 用章人员要求
		/// </summary>
		[Column("pi_yz_per_yq")]
		public  int? PiYzPerYq { get; set; }
		/// <summary>
		/// 项目运作情况：正常运作，项目停止，项目作废
		/// </summary>
		[Column("pi_yzqk")]
		public  int? PiYzqk { get; set; }
		/// <summary>
		/// 印章要求
		/// </summary>
		[Column("pi_yzyq")]
		public  string PiYzyq { get; set; }
		/// <summary>
		/// 指标录入审批状态
		/// </summary>
		[Column("pi_zb_examine_state")]
		public  int? PiZbExamineState { get; set; }
		/// <summary>
		/// 装订要求
		/// </summary>
		[Column("pi_zdyq")]
		public  string PiZdyq { get; set; }
		/// <summary>
		/// 建筑工程文档
		/// </summary>
		[Column("pi_zjgc_file")]
		public  string PiZjgcFile { get; set; }
		/// <summary>
		/// 修改时间
		/// </summary>
		[Column("update_time")]
		public  DateTime? UpdateTime { get; set; }
		/// <summary>
		/// 修改人
		/// </summary>
		[Column("update_user")]
		public  string UpdateUser { get; set; }
		/// <summary>
		/// 指标模板id
		/// </summary>
		[Column("zit_id")]
		public  int? ZitId { get; set; }
	}

	/// <summary>
    /// 结算咨询-基本信息
    /// </summary>
	[Table("project_js_info")]
	public partial class ProjectJsInfo : Entity
	{
		/// <summary>
		/// 
		/// </summary>
		[Column("create_time")]
		public  DateTime? CreateTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("create_user_id")]
		public  int? CreateUserId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("create_user_name")]
		public  string CreateUserName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Key][Identity][Column("id")]
		public override int? Id { get; set; }
		/// <summary>
		/// 地区code
		/// </summary>
		[Column("pj_a_code")]
		public  string PjACode { get; set; }
		/// <summary>
		/// 地区name
		/// </summary>
		[Column("pj_a_name")]
		public  string PjAName { get; set; }
		/// <summary>
		/// 详细地址
		/// </summary>
		[Column("pj_address_detail")]
		public  string PjAddressDetail { get; set; }
		/// <summary>
		/// 退档备注
		/// </summary>
		[Column("pj_back_archive_remark")]
		public  string PjBackArchiveRemark { get; set; }
		/// <summary>
		/// 部门归属
		/// </summary>
		[Column("pj_belong_dep")]
		public  int? PjBelongDep { get; set; }
		/// <summary>
		/// 报告号
		/// </summary>
		[Column("pj_bgh")]
		public  string PjBgh { get; set; }
		/// <summary>
		/// 报告号id
		/// </summary>
		[Column("pj_bgh_id")]
		public  int? PjBghId { get; set; }
		/// <summary>
		/// 报告号num
		/// </summary>
		[Column("pj_bgh_num")]
		public  int? PjBghNum { get; set; }
		/// <summary>
		/// 生成报告号时间
		/// </summary>
		[Column("pj_bgh_time")]
		public  DateTime? PjBghTime { get; set; }
		/// <summary>
		/// 业务阶段
		/// </summary>
		[Column("pj_bt_stage")]
		public  int? PjBtStage { get; set; }
		/// <summary>
		/// 编制类型
		/// </summary>
		[Column("pj_bt_type")]
		public  int? PjBtType { get; set; }
		/// <summary>
		/// 建筑面积
		/// </summary>
		[Column("pj_building_area")]
		public  decimal? PjBuildingArea { get; set; }
		/// <summary>
		/// 建筑装配率
		/// </summary>
		[Column("pj_building_assembly_rate")]
		public  int? PjBuildingAssemblyRate { get; set; }
		/// <summary>
		/// 建筑高度
		/// </summary>
		[Column("pj_building_height")]
		public  decimal? PjBuildingHeight { get; set; }
		/// <summary>
		/// 建筑类型
		/// </summary>
		[Column("pj_building_type")]
		public  int? PjBuildingType { get; set; }
		/// <summary>
		/// 城市code
		/// </summary>
		[Column("pj_c_code")]
		public  string PjCCode { get; set; }
		/// <summary>
		/// 城市name
		/// </summary>
		[Column("pj_c_name")]
		public  string PjCName { get; set; }
		/// <summary>
		/// 创收数量
		/// </summary>
		[Column("pj_ci_num")]
		public  int? PjCiNum { get; set; }
		/// <summary>
		/// 本项目建安造价金额
		/// </summary>
		[Column("pj_construction_amount")]
		public  decimal? PjConstructionAmount { get; set; }
		/// <summary>
		/// 总项目建安造价额
		/// </summary>
		[Column("pj_construction_total_amount")]
		public  decimal? PjConstructionTotalAmount { get; set; }
		/// <summary>
		/// 合同价格形式
		/// </summary>
		[Column("pj_contract_price_type")]
		public  int? PjContractPriceType { get; set; }
		/// <summary>
		/// 合同类型
		/// </summary>
		[Column("pj_contract_type")]
		public  int? PjContractType { get; set; }
		/// <summary>
		/// 项目创建审批流
		/// </summary>
		[Column("pj_create_lc_type")]
		public  int? PjCreateLcType { get; set; }
		/// <summary>
		/// 创建审批通过时间
		/// </summary>
		[Column("pj_create_pass_time")]
		public  DateTime? PjCreatePassTime { get; set; }
		/// <summary>
		/// 归属项目合同id
		/// </summary>
		[Column("pj_ct_id")]
		public  int? PjCtId { get; set; }
		/// <summary>
		/// 归属项目合同name
		/// </summary>
		[Column("pj_ct_name")]
		public  string PjCtName { get; set; }
		/// <summary>
		/// 整体项目id
		/// </summary>
		[Column("pj_ctp_id")]
		public  int? PjCtpId { get; set; }
		/// <summary>
		/// 整体项目name
		/// </summary>
		[Column("pj_ctp_name")]
		public  string PjCtpName { get; set; }
		/// <summary>
		/// 资料精装
		/// </summary>
		[Column("pj_data_fine_num")]
		public  int? PjDataFineNum { get; set; }
		/// <summary>
		/// 其他印刷要求
		/// </summary>
		[Column("pj_data_oth_pack_require")]
		public  string PjDataOthPackRequire { get; set; }
		/// <summary>
		/// 装订要求
		/// </summary>
		[Column("pj_data_pack_require")]
		public  int? PjDataPackRequire { get; set; }
		/// <summary>
		/// 资料简装
		/// </summary>
		[Column("pj_data_simple_num")]
		public  int? PjDataSimpleNum { get; set; }
		/// <summary>
		/// 部门小组
		/// </summary>
		[Column("pj_dep_group")]
		public  int? PjDepGroup { get; set; }
		/// <summary>
		/// 牵头部门id
		/// </summary>
		[Column("pj_dep_id")]
		public  int? PjDepId { get; set; }
		/// <summary>
		/// 牵头部门name
		/// </summary>
		[Column("pj_dep_name")]
		public  string PjDepName { get; set; }
		/// <summary>
		/// 地上楼层数
		/// </summary>
		[Column("pj_ds_floor_num")]
		public  int? PjDsFloorNum { get; set; }
		/// <summary>
		/// 地下楼层数
		/// </summary>
		[Column("pj_dx_floor_num")]
		public  int? PjDxFloorNum { get; set; }
		/// <summary>
		/// 项目竣工日期
		/// </summary>
		[Column("pj_end_work_date")]
		public  DateTime? PjEndWorkDate { get; set; }
		/// <summary>
		/// 进入天秤码时间
		/// </summary>
		[Column("pj_enter_tcm_input_time")]
		public  DateTime? PjEnterTcmInputTime { get; set; }
		/// <summary>
		/// 复核形式
		/// </summary>
		[Column("pj_external_check_method")]
		public  string PjExternalCheckMethod { get; set; }
		/// <summary>
		/// 外审单位id
		/// </summary>
		[Column("pj_external_uint_id")]
		public  int? PjExternalUintId { get; set; }
		/// <summary>
		/// 外审单位name
		/// </summary>
		[Column("pj_external_uint_name")]
		public  string PjExternalUintName { get; set; }
		/// <summary>
		/// 项目特征
		/// </summary>
		[Column("pj_features")]
		public  string PjFeatures { get; set; }
		/// <summary>
		/// 项目特征名称
		/// </summary>
		[Column("pj_features_name")]
		public  string PjFeaturesName { get; set; }
		/// <summary>
		/// 首次生成报告号时间
		/// </summary>
		[Column("pj_first_bgh_time")]
		public  DateTime? PjFirstBghTime { get; set; }
		/// <summary>
		/// 资金来源
		/// </summary>
		[Column("pj_funding_source")]
		public  int? PjFundingSource { get; set; }
		/// <summary>
		/// 归档：存放册数
		/// </summary>
		[Column("pj_gd_archive_num")]
		public  string PjGdArchiveNum { get; set; }
		/// <summary>
		/// 归档：文件夹编号
		/// </summary>
		[Column("pj_gd_folder_no")]
		public  string PjGdFolderNo { get; set; }
		/// <summary>
		/// 归档：是否有评价表
		/// </summary>
		[Column("pj_gd_has_comment")]
		public  int? PjGdHasComment { get; set; }
		/// <summary>
		/// 归档：存放位置
		/// </summary>
		[Column("pj_gd_storage_location")]
		public  string PjGdStorageLocation { get; set; }
		/// <summary>
		/// 归档完成时间
		/// </summary>
		[Column("pj_gdwc_time")]
		public  DateTime? PjGdwcTime { get; set; }
		/// <summary>
		/// 核减金额
		/// </summary>
		[Column("pj_hj_amount")]
		public  decimal? PjHjAmount { get; set; }
		/// <summary>
		/// 核增金额
		/// </summary>
		[Column("pj_hz_amount")]
		public  decimal? PjHzAmount { get; set; }
		/// <summary>
		/// 行业管理
		/// </summary>
		[Column("pj_industry_manage")]
		public  int? PjIndustryManage { get; set; }
		/// <summary>
		/// 本项目投资金额
		/// </summary>
		[Column("pj_investment_amount")]
		public  decimal? PjInvestmentAmount { get; set; }
		/// <summary>
		/// 本项目投资总金额
		/// </summary>
		[Column("pj_investment_total_amount")]
		public  decimal? PjInvestmentTotalAmount { get; set; }
		/// <summary>
		/// 投资类型
		/// </summary>
		[Column("pj_investment_type")]
		public  int? PjInvestmentType { get; set; }
		/// <summary>
		/// 是否作废
		/// </summary>
		[Column("pj_is_cancel")]
		public  int? PjIsCancel { get; set; }
		/// <summary>
		/// 是否有部门小组
		/// </summary>
		[Column("pj_is_dep_group")]
		public  int? PjIsDepGroup { get; set; }
		/// <summary>
		/// 是否需要外审
		/// </summary>
		[Column("pj_is_external_check")]
		public  int? PjIsExternalCheck { get; set; }
		/// <summary>
		/// 是否历史遗留
		/// </summary>
		[Column("pj_is_his_legacy")]
		public  int? PjIsHisLegacy { get; set; }
		/// <summary>
		/// 甲方格式要求：是否有报告、审定格式要求
		/// </summary>
		[Column("pj_is_jfyq")]
		public  int? PjIsJfyq { get; set; }
		/// <summary>
		/// 其它甲方格式要求：是否有无甲方要求
		/// </summary>
		[Column("pj_is_oth_jfyq")]
		public  int? PjIsOthJfyq { get; set; }
		/// <summary>
		/// 签章准备：是否需要优先盖章
		/// </summary>
		[Column("pj_is_priority_seal")]
		public  int? PjIsPrioritySeal { get; set; }
		/// <summary>
		/// 是否盖章后复核
		/// </summary>
		[Column("pj_is_seal_after_fh")]
		public  int? PjIsSealAfterFh { get; set; }
		/// <summary>
		/// 是否盖章前外部复核
		/// </summary>
		[Column("pj_is_seal_before_fh")]
		public  int? PjIsSealBeforeFh { get; set; }
		/// <summary>
		/// 是否简易项目
		/// </summary>
		[Column("pj_is_simple")]
		public  int? PjIsSimple { get; set; }
		/// <summary>
		/// 是否需要三级复核
		/// </summary>
		[Column("pj_is_th_fh")]
		public  int? PjIsThFh { get; set; }
		/// <summary>
		/// 是否需要调整稿
		/// </summary>
		[Column("pj_is_tzg")]
		public  int? PjIsTzg { get; set; }
		/// <summary>
		/// 是否设为公司业绩库
		/// </summary>
		[Column("pj_is_yjk")]
		public  int? PjIsYjk { get; set; }
		/// <summary>
		/// 是否设为个人业绩库
		/// </summary>
		[Column("pj_is_yjk_per")]
		public  int? PjIsYjkPer { get; set; }
		/// <summary>
		/// 项目签发审批流id
		/// </summary>
		[Column("pj_issue_lc_type")]
		public  int? PjIssueLcType { get; set; }
		/// <summary>
		/// 签发通过时间
		/// </summary>
		[Column("pj_issue_pass_time")]
		public  DateTime? PjIssuePassTime { get; set; }
		/// <summary>
		/// 签章复核审批流
		/// </summary>
		[Column("pj_issue_seal_check_lc_type")]
		public  int? PjIssueSealCheckLcType { get; set; }
		/// <summary>
		/// 签章准备：审定单盖章审批流
		/// </summary>
		[Column("pj_issue_seal_lc_type")]
		public  int? PjIssueSealLcType { get; set; }
		/// <summary>
		/// 签章盖章通过人id
		/// </summary>
		[Column("pj_issue_seal_pass_per_id")]
		public  int? PjIssueSealPassPerId { get; set; }
		/// <summary>
		/// 签章盖章通过人name
		/// </summary>
		[Column("pj_issue_seal_pass_per_name")]
		public  string PjIssueSealPassPerName { get; set; }
		/// <summary>
		/// 签章盖章通过人remark
		/// </summary>
		[Column("pj_issue_seal_pass_per_remark")]
		public  string PjIssueSealPassPerRemark { get; set; }
		/// <summary>
		/// 签章通过时间：审定单盖章时间
		/// </summary>
		[Column("pj_issue_seal_pass_time")]
		public  DateTime? PjIssueSealPassTime { get; set; }
		/// <summary>
		/// 签章复核审批人ids
		/// </summary>
		[Column("pj_issue_seal_sp_per_ids")]
		public  string PjIssueSealSpPerIds { get; set; }
		/// <summary>
		/// 签章复核审批人names
		/// </summary>
		[Column("pj_issue_seal_sp_per_names")]
		public  string PjIssueSealSpPerNames { get; set; }
		/// <summary>
		/// 项目签发审批人id
		/// </summary>
		[Column("pj_issue_sp_per_ids")]
		public  string PjIssueSpPerIds { get; set; }
		/// <summary>
		/// 项目签发审批人name
		/// </summary>
		[Column("pj_issue_sp_per_names")]
		public  string PjIssueSpPerNames { get; set; }
		/// <summary>
		/// 项目签发审批备注
		/// </summary>
		[Column("pj_issue_sp_remark")]
		public  string PjIssueSpRemark { get; set; }
		/// <summary>
		/// 发票数量
		/// </summary>
		[Column("pj_iv_num")]
		public  int? PjIvNum { get; set; }
		/// <summary>
		/// 甲方格式要求：具体要求
		/// </summary>
		[Column("pj_jfyq_description")]
		public  string PjJfyqDescription { get; set; }
		/// <summary>
		/// 甲方格式要求：甲方模板文件
		/// </summary>
		[Column("pj_jfyq_file")]
		public  string PjJfyqFile { get; set; }
		/// <summary>
		/// 进入一级复核时间
		/// </summary>
		[Column("pj_jr_fir_fh_time")]
		public  DateTime? PjJrFirFhTime { get; set; }
		/// <summary>
		/// 进入签发盖章时间
		/// </summary>
		[Column("pj_jr_qf_gz_time")]
		public  DateTime? PjJrQfGzTime { get; set; }
		/// <summary>
		/// 进入二级复核时间
		/// </summary>
		[Column("pj_jr_sec_fh_time")]
		public  DateTime? PjJrSecFhTime { get; set; }
		/// <summary>
		/// 进入三级复核时间
		/// </summary>
		[Column("pj_jr_th_fh_time")]
		public  DateTime? PjJrThFhTime { get; set; }
		/// <summary>
		/// 进入调整稿时间
		/// </summary>
		[Column("pj_jrtzg_time")]
		public  DateTime? PjJrtzgTime { get; set; }
		/// <summary>
		/// 建设单位ids
		/// </summary>
		[Column("pj_js_unit_ids")]
		public  string PjJsUnitIds { get; set; }
		/// <summary>
		/// 建设单位names
		/// </summary>
		[Column("pj_js_unit_names")]
		public  string PjJsUnitNames { get; set; }
		/// <summary>
		/// 项目经理id
		/// </summary>
		[Column("pj_leader_id")]
		public  int? PjLeaderId { get; set; }
		/// <summary>
		/// 项目经理作业专业
		/// </summary>
		[Column("pj_leader_major")]
		public  int? PjLeaderMajor { get; set; }
		/// <summary>
		/// 项目经理作业专业
		/// </summary>
		[Column("pj_leader_major_name")]
		public  string PjLeaderMajorName { get; set; }
		/// <summary>
		/// 项目经理name
		/// </summary>
		[Column("pj_leader_name")]
		public  string PjLeaderName { get; set; }
		/// <summary>
		/// 廉政状态
		/// </summary>
		[Column("pj_lz_state")]
		public  int? PjLzState { get; set; }
		/// <summary>
		/// 专业二级复核完成时间
		/// </summary>
		[Column("pj_major_sec_fh_wc_time")]
		public  DateTime? PjMajorSecFhWcTime { get; set; }
		/// <summary>
		/// 项目名称
		/// </summary>
		[Column("pj_name")]
		public  string PjName { get; set; }
		/// <summary>
		/// 其它甲方格式要求：具体要求
		/// </summary>
		[Column("pj_oth_jfyq_description")]
		public  string PjOthJfyqDescription { get; set; }
		/// <summary>
		/// 其它甲方格式要求：具体要求json
		/// </summary>
		[Column("pj_oth_jfyq_description_json")]
		public  string PjOthJfyqDescriptionJson { get; set; }
		/// <summary>
		/// 其它甲方格式要求：对应文件
		/// </summary>
		[Column("pj_oth_jfyq_file")]
		public  string PjOthJfyqFile { get; set; }
		/// <summary>
		/// 其它甲方格式要求：对应图片
		/// </summary>
		[Column("pj_oth_jfyq_img")]
		public  string PjOthJfyqImg { get; set; }
		/// <summary>
		/// 省code
		/// </summary>
		[Column("pj_p_code")]
		public  string PjPCode { get; set; }
		/// <summary>
		/// 省name
		/// </summary>
		[Column("pj_p_name")]
		public  string PjPName { get; set; }
		/// <summary>
		/// 发包模式
		/// </summary>
		[Column("pj_packge_mode")]
		public  int? PjPackgeMode { get; set; }
		/// <summary>
		/// 全过程项目id
		/// </summary>
		[Column("pj_pm_id")]
		public  int? PjPmId { get; set; }
		/// <summary>
		/// 全过程项目name
		/// </summary>
		[Column("pj_pm_name")]
		public  string PjPmName { get; set; }
		/// <summary>
		/// 关联合同次数
		/// </summary>
		[Column("pj_re_contrct_num")]
		public  int? PjReContrctNum { get; set; }
		/// <summary>
		/// 项目备注
		/// </summary>
		[Column("pj_remark")]
		public  string PjRemark { get; set; }
		/// <summary>
		/// 报备说明
		/// </summary>
		[Column("pj_report_remark")]
		public  string PjReportRemark { get; set; }
		/// <summary>
		/// 审定金额
		/// </summary>
		[Column("pj_sd_amount")]
		public  decimal? PjSdAmount { get; set; }
		/// <summary>
		/// 审定单
		/// </summary>
		[Column("pj_sd_order_file")]
		public  string PjSdOrderFile { get; set; }
		/// <summary>
		/// 用印：编制盖章人id
		/// </summary>
		[Column("pj_seal_bz_per_id")]
		public  string PjSealBzPerId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pj_seal_bz_per_json")]
		public  string PjSealBzPerJson { get; set; }
		/// <summary>
		/// 用印：编制盖章人name
		/// </summary>
		[Column("pj_seal_bz_per_name")]
		public  string PjSealBzPerName { get; set; }
		/// <summary>
		/// 用印：审定盖章人id
		/// </summary>
		[Column("pj_seal_sd_per_id")]
		public  string PjSealSdPerId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pj_seal_sd_per_json")]
		public  string PjSealSdPerJson { get; set; }
		/// <summary>
		/// 用印：审定盖章人id
		/// </summary>
		[Column("pj_seal_sd_per_name")]
		public  string PjSealSdPerName { get; set; }
		/// <summary>
		/// 用印：审核盖章人id
		/// </summary>
		[Column("pj_seal_sh_per_id")]
		public  string PjSealShPerId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pj_seal_sh_per_json")]
		public  string PjSealShPerJson { get; set; }
		/// <summary>
		/// 用印：审核盖章人name
		/// </summary>
		[Column("pj_seal_sh_per_name")]
		public  string PjSealShPerName { get; set; }
		/// <summary>
		/// 用章要求：一造价+二造，只能一造
		/// </summary>
		[Column("pj_seal_usage_cert")]
		public  int? PjSealUsageCert { get; set; }
		/// <summary>
		/// 用章方式：按照合同，自由选择
		/// </summary>
		[Column("pj_seal_usage_type")]
		public  int? PjSealUsageType { get; set; }
		/// <summary>
		/// 用印：项目负责人id
		/// </summary>
		[Column("pj_seal_xm_per_id")]
		public  string PjSealXmPerId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pj_seal_xm_per_json")]
		public  string PjSealXmPerJson { get; set; }
		/// <summary>
		/// 用印：项目负责人name
		/// </summary>
		[Column("pj_seal_xm_per_name")]
		public  string PjSealXmPerName { get; set; }
		/// <summary>
		/// 施工单位ids
		/// </summary>
		[Column("pj_sg_unit_ids")]
		public  string PjSgUnitIds { get; set; }
		/// <summary>
		/// 施工单位names
		/// </summary>
		[Column("pj_sg_unit_names")]
		public  string PjSgUnitNames { get; set; }
		/// <summary>
		/// 实际作业期结束时间
		/// </summary>
		[Column("pj_sjzyq_end_time")]
		public  DateTime? PjSjzyqEndTime { get; set; }
		/// <summary>
		/// 实际作业期开始时间
		/// </summary>
		[Column("pj_sjzyq_start_time")]
		public  DateTime? PjSjzyqStartTime { get; set; }
		/// <summary>
		/// 送审金额
		/// </summary>
		[Column("pj_ss_amount")]
		public  decimal? PjSsAmount { get; set; }
		/// <summary>
		/// 项目开始时间
		/// </summary>
		[Column("pj_start_time")]
		public  DateTime? PjStartTime { get; set; }
		/// <summary>
		/// 项目开工日期
		/// </summary>
		[Column("pj_start_work_date")]
		public  DateTime? PjStartWorkDate { get; set; }
		/// <summary>
		/// 项目状态
		/// </summary>
		[Column("pj_state")]
		public  int? PjState { get; set; }
		/// <summary>
		/// 签章准备：天秤码截图
		/// </summary>
		[Column("pj_tcm_image")]
		public  string PjTcmImage { get; set; }
		/// <summary>
		/// 三级复核分配人id
		/// </summary>
		[Column("pj_th_fh_fp_per_id")]
		public  int? PjThFhFpPerId { get; set; }
		/// <summary>
		/// 三级复核分配人id
		/// </summary>
		[Column("pj_th_fh_fp_per_name")]
		public  string PjThFhFpPerName { get; set; }
		/// <summary>
		/// 三级复核分配完成时间
		/// </summary>
		[Column("pj_th_fh_fp_wc_time")]
		public  DateTime? PjThFhFpWcTime { get; set; }
		/// <summary>
		/// 三级复核完成时间
		/// </summary>
		[Column("pj_th_fh_wc_time")]
		public  DateTime? PjThFhWcTime { get; set; }
		/// <summary>
		/// 服务类型id
		/// </summary>
		[Column("pj_type")]
		public  int? PjType { get; set; }
		/// <summary>
		/// 服务类型name
		/// </summary>
		[Column("pj_type_name")]
		public  string PjTypeName { get; set; }
		/// <summary>
		/// 服务类型二级id
		/// </summary>
		[Column("pj_type_sec")]
		public  int? PjTypeSec { get; set; }
		/// <summary>
		/// 服务类型二级name
		/// </summary>
		[Column("pj_type_sec_name")]
		public  string PjTypeSecName { get; set; }
		/// <summary>
		/// 用地面积
		/// </summary>
		[Column("pj_usage_area")]
		public  decimal? PjUsageArea { get; set; }
		/// <summary>
		/// 整体二级复核人id
		/// </summary>
		[Column("pj_whole_sec_fh_per_id")]
		public  int? PjWholeSecFhPerId { get; set; }
		/// <summary>
		/// 整体二级复核人name
		/// </summary>
		[Column("pj_whole_sec_fh_per_name")]
		public  string PjWholeSecFhPerName { get; set; }
		/// <summary>
		/// 整体二级复核完成时间
		/// </summary>
		[Column("pj_whole_sec_fh_wc_time")]
		public  DateTime? PjWholeSecFhWcTime { get; set; }
		/// <summary>
		/// 项目作业审批流
		/// </summary>
		[Column("pj_work_lc_type")]
		public  int? PjWorkLcType { get; set; }
		/// <summary>
		/// 委托单位ids
		/// </summary>
		[Column("pj_wt_unit_ids")]
		public  string PjWtUnitIds { get; set; }
		/// <summary>
		/// 委托单位names
		/// </summary>
		[Column("pj_wt_unit_names")]
		public  string PjWtUnitNames { get; set; }
		/// <summary>
		/// 文印复核通过人id
		/// </summary>
		[Column("pj_wy_fh_pass_per_id")]
		public  int? PjWyFhPassPerId { get; set; }
		/// <summary>
		/// 文印复核通过人name
		/// </summary>
		[Column("pj_wy_fh_pass_per_name")]
		public  string PjWyFhPassPerName { get; set; }
		/// <summary>
		/// 文印复核通过时间
		/// </summary>
		[Column("pj_wy_fh_pass_time")]
		public  DateTime? PjWyFhPassTime { get; set; }
		/// <summary>
		/// 文印复核人ids
		/// </summary>
		[Column("pj_wy_fh_per_ids")]
		public  string PjWyFhPerIds { get; set; }
		/// <summary>
		/// 文印复核人names
		/// </summary>
		[Column("pj_wy_fh_per_names")]
		public  string PjWyFhPerNames { get; set; }
		/// <summary>
		/// 文印复核提交时间
		/// </summary>
		[Column("pj_wy_fh_tj_time")]
		public  DateTime? PjWyFhTjTime { get; set; }
		/// <summary>
		/// 文字复核通过人id
		/// </summary>
		[Column("pj_wz_fh_pass_per_id")]
		public  int? PjWzFhPassPerId { get; set; }
		/// <summary>
		/// 文字复核通过人id
		/// </summary>
		[Column("pj_wz_fh_pass_per_name")]
		public  string PjWzFhPassPerName { get; set; }
		/// <summary>
		/// 文字复核通过时间
		/// </summary>
		[Column("pj_wz_fh_pass_time")]
		public  DateTime? PjWzFhPassTime { get; set; }
		/// <summary>
		/// 文字复核人ids
		/// </summary>
		[Column("pj_wz_fh_per_ids")]
		public  string PjWzFhPerIds { get; set; }
		/// <summary>
		/// 文字复核人names
		/// </summary>
		[Column("pj_wz_fh_per_names")]
		public  string PjWzFhPerNames { get; set; }
		/// <summary>
		/// 文字复核提交时间
		/// </summary>
		[Column("pj_wz_fh_tj_time")]
		public  DateTime? PjWzFhTjTime { get; set; }
		/// <summary>
		/// 协同部门id
		/// </summary>
		[Column("pj_xt_dep_id")]
		public  string PjXtDepId { get; set; }
		/// <summary>
		/// 协同部门name
		/// </summary>
		[Column("pj_xt_dep_name")]
		public  string PjXtDepName { get; set; }
		/// <summary>
		/// 业绩库录入方式：同造价
		/// </summary>
		[Column("pj_yjk_input_type")]
		public  int? PjYjkInputType { get; set; }
		/// <summary>
		/// 业绩库状态
		/// </summary>
		[Column("pj_yjk_state")]
		public  int? PjYjkState { get; set; }
		/// <summary>
		/// 预计收费金额
		/// </summary>
		[Column("pj_yjsf_amount")]
		public  string PjYjsfAmount { get; set; }
		/// <summary>
		/// 要求天秤码截止时间
		/// </summary>
		[Column("pj_yq_tcm_end_time")]
		public  DateTime? PjYqTcmEndTime { get; set; }
		/// <summary>
		/// 要求报告时间
		/// </summary>
		[Column("pj_yqbg_time")]
		public  DateTime? PjYqbgTime { get; set; }
		/// <summary>
		/// 要求初稿时间
		/// </summary>
		[Column("pj_yqcg_time")]
		public  DateTime? PjYqcgTime { get; set; }
		/// <summary>
		/// 要求归档时间
		/// </summary>
		[Column("pj_yqgd_time")]
		public  DateTime? PjYqgdTime { get; set; }
		/// <summary>
		/// 要求进入作业时间
		/// </summary>
		[Column("pj_yqjrzy_time")]
		public  DateTime? PjYqjrzyTime { get; set; }
		/// <summary>
		/// 要求调整稿时间
		/// </summary>
		[Column("pj_yqtzg_time")]
		public  DateTime? PjYqtzgTime { get; set; }
		/// <summary>
		/// 要求终稿完成时间
		/// </summary>
		[Column("pj_yqzgwc_time")]
		public  DateTime? PjYqzgwcTime { get; set; }
		/// <summary>
		/// 业态其它说明
		/// </summary>
		[Column("pj_yt_description")]
		public  string PjYtDescription { get; set; }
		/// <summary>
		/// 项目业态id
		/// </summary>
		[Column("pj_yt_id")]
		public  int? PjYtId { get; set; }
		/// <summary>
		/// 项目业态name
		/// </summary>
		[Column("pj_yt_name")]
		public  string PjYtName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("update_time")]
		public  DateTime? UpdateTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("update_user_id")]
		public  int? UpdateUserId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("update_user_name")]
		public  string UpdateUserName { get; set; }
	}

	/// <summary>
    /// 结算咨询-项目成员
    /// </summary>
	[Table("project_js_user")]
	public partial class ProjectJsUser : Entity
	{
		/// <summary>
		/// 
		/// </summary>
		[Column("create_time")]
		public  DateTime? CreateTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("create_user_id")]
		public  int? CreateUserId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("create_user_name")]
		public  string CreateUserName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Key][Identity][Column("id")]
		public override int? Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pj_id")]
		public  int? PjId { get; set; }
		/// <summary>
		/// 初稿完成时间
		/// </summary>
		[Column("pju_cg_wc_time")]
		public  DateTime? PjuCgWcTime { get; set; }
		/// <summary>
		/// 已确认的统一口径
		/// </summary>
		[Column("pju_confirm_standard_items")]
		public  string PjuConfirmStandardItems { get; set; }
		/// <summary>
		/// 部门id
		/// </summary>
		[Column("pju_dep_id")]
		public  int? PjuDepId { get; set; }
		/// <summary>
		/// 部门name
		/// </summary>
		[Column("pju_dep_name")]
		public  string PjuDepName { get; set; }
		/// <summary>
		/// 一级复核打回次数
		/// </summary>
		[Column("pju_fir_fh_back_num")]
		public  int? PjuFirFhBackNum { get; set; }
		/// <summary>
		/// 一级复核人ids
		/// </summary>
		[Column("pju_fir_fh_per_ids")]
		public  string PjuFirFhPerIds { get; set; }
		/// <summary>
		/// 一级复核人names
		/// </summary>
		[Column("pju_fir_fh_per_names")]
		public  string PjuFirFhPerNames { get; set; }
		/// <summary>
		/// 核减金额：一级
		/// </summary>
		[Column("pju_fir_hj_amount")]
		public  decimal? PjuFirHjAmount { get; set; }
		/// <summary>
		/// 核增金额：一级
		/// </summary>
		[Column("pju_fir_hz_amount")]
		public  decimal? PjuFirHzAmount { get; set; }
		/// <summary>
		/// 审定金额：一级
		/// </summary>
		[Column("pju_fir_sd_amount")]
		public  decimal? PjuFirSdAmount { get; set; }
		/// <summary>
		/// 一级复核提交时间
		/// </summary>
		[Column("pju_first_fh_submit_time")]
		public  DateTime? PjuFirstFhSubmitTime { get; set; }
		/// <summary>
		/// 一级复核完成人id
		/// </summary>
		[Column("pju_first_fh_wc_per_id")]
		public  int? PjuFirstFhWcPerId { get; set; }
		/// <summary>
		/// 一级复核完成人name
		/// </summary>
		[Column("pju_first_fh_wc_per_name")]
		public  string PjuFirstFhWcPerName { get; set; }
		/// <summary>
		/// 一级复核完成时间
		/// </summary>
		[Column("pju_first_fh_wc_time")]
		public  DateTime? PjuFirstFhWcTime { get; set; }
		/// <summary>
		/// 最新的核减金额
		/// </summary>
		[Column("pju_hj_amount")]
		public  decimal? PjuHjAmount { get; set; }
		/// <summary>
		/// 最新的核增金额
		/// </summary>
		[Column("pju_hz_amount")]
		public  decimal? PjuHzAmount { get; set; }
		/// <summary>
		/// 是否确认统一口径
		/// </summary>
		[Column("pju_is_confirm_tykj")]
		public  int? PjuIsConfirmTykj { get; set; }
		/// <summary>
		/// 是否外聘人员
		/// </summary>
		[Column("pju_is_external")]
		public  int? PjuIsExternal { get; set; }
		/// <summary>
		/// 是否项目经理
		/// </summary>
		[Column("pju_is_leader")]
		public  int? PjuIsLeader { get; set; }
		/// <summary>
		/// 是否为专业负责人
		/// </summary>
		[Column("pju_is_professional")]
		public  int? PjuIsProfessional { get; set; }
		/// <summary>
		/// 所属专业
		/// </summary>
		[Column("pju_major")]
		public  int? PjuMajor { get; set; }
		/// <summary>
		/// 所属专业
		/// </summary>
		[Column("pju_major_name")]
		public  string PjuMajorName { get; set; }
		/// <summary>
		/// 最新的审定金额
		/// </summary>
		[Column("pju_sd_amount")]
		public  decimal? PjuSdAmount { get; set; }
		/// <summary>
		/// 实际作业用章人
		/// </summary>
		[Column("pju_seal_per_json")]
		public  string PjuSealPerJson { get; set; }
		/// <summary>
		/// 二级复核打回次数
		/// </summary>
		[Column("pju_sec_fh_back_num")]
		public  int? PjuSecFhBackNum { get; set; }
		/// <summary>
		/// 二级复核扣款
		/// </summary>
		[Column("pju_sec_fh_kk_amount")]
		public  decimal? PjuSecFhKkAmount { get; set; }
		/// <summary>
		/// 二级复核人ids
		/// </summary>
		[Column("pju_sec_fh_per_ids")]
		public  string PjuSecFhPerIds { get; set; }
		/// <summary>
		/// 二级复核人names
		/// </summary>
		[Column("pju_sec_fh_per_names")]
		public  string PjuSecFhPerNames { get; set; }
		/// <summary>
		/// 二级复核提交时间
		/// </summary>
		[Column("pju_sec_fh_submit_time")]
		public  DateTime? PjuSecFhSubmitTime { get; set; }
		/// <summary>
		/// 二级复核完成人id
		/// </summary>
		[Column("pju_sec_fh_wc_per_id")]
		public  int? PjuSecFhWcPerId { get; set; }
		/// <summary>
		/// 一级复核完成人name
		/// </summary>
		[Column("pju_sec_fh_wc_per_name")]
		public  string PjuSecFhWcPerName { get; set; }
		/// <summary>
		/// 二级复核完成时间
		/// </summary>
		[Column("pju_sec_fh_wc_time")]
		public  DateTime? PjuSecFhWcTime { get; set; }
		/// <summary>
		/// 核减金额：二级
		/// </summary>
		[Column("pju_sec_hj_amount")]
		public  decimal? PjuSecHjAmount { get; set; }
		/// <summary>
		/// 核增金额：二级
		/// </summary>
		[Column("pju_sec_hz_amount")]
		public  decimal? PjuSecHzAmount { get; set; }
		/// <summary>
		/// 审定金额：二级
		/// </summary>
		[Column("pju_sec_sd_amount")]
		public  decimal? PjuSecSdAmount { get; set; }
		/// <summary>
		/// 送审金额
		/// </summary>
		[Column("pju_ss_amount")]
		public  decimal? PjuSsAmount { get; set; }
		/// <summary>
		/// 状态
		/// </summary>
		[Column("pju_state")]
		public  int? PjuState { get; set; }
		/// <summary>
		/// 三级复核打回次数
		/// </summary>
		[Column("pju_th_fh_back_num")]
		public  int? PjuThFhBackNum { get; set; }
		/// <summary>
		/// 三级复核扣款
		/// </summary>
		[Column("pju_th_fh_kk_amount")]
		public  decimal? PjuThFhKkAmount { get; set; }
		/// <summary>
		/// 三级复核人ids
		/// </summary>
		[Column("pju_th_fh_per_ids")]
		public  string PjuThFhPerIds { get; set; }
		/// <summary>
		/// 三级复核人names
		/// </summary>
		[Column("pju_th_fh_per_names")]
		public  string PjuThFhPerNames { get; set; }
		/// <summary>
		/// 三级复核提交时间
		/// </summary>
		[Column("pju_th_fh_submit_time")]
		public  DateTime? PjuThFhSubmitTime { get; set; }
		/// <summary>
		/// 三级复核完成人id
		/// </summary>
		[Column("pju_th_fh_wc_per_id")]
		public  int? PjuThFhWcPerId { get; set; }
		/// <summary>
		/// 三级复核完成人name
		/// </summary>
		[Column("pju_th_fh_wc_per_name")]
		public  string PjuThFhWcPerName { get; set; }
		/// <summary>
		/// 三级复核完成时间
		/// </summary>
		[Column("pju_th_fh_wc_time")]
		public  DateTime? PjuThFhWcTime { get; set; }
		/// <summary>
		/// 核减金额：三级
		/// </summary>
		[Column("pju_th_hj_amount")]
		public  decimal? PjuThHjAmount { get; set; }
		/// <summary>
		/// 核增金额：三级
		/// </summary>
		[Column("pju_th_hz_amount")]
		public  decimal? PjuThHzAmount { get; set; }
		/// <summary>
		/// 审定金额：三级
		/// </summary>
		[Column("pju_th_sd_amount")]
		public  decimal? PjuThSdAmount { get; set; }
		/// <summary>
		/// 核减金额：调整稿
		/// </summary>
		[Column("pju_tzg_hj_amount")]
		public  decimal? PjuTzgHjAmount { get; set; }
		/// <summary>
		/// 核增金额：调整稿
		/// </summary>
		[Column("pju_tzg_hz_amount")]
		public  decimal? PjuTzgHzAmount { get; set; }
		/// <summary>
		/// 审定金额：调整稿
		/// </summary>
		[Column("pju_tzg_sd_amount")]
		public  decimal? PjuTzgSdAmount { get; set; }
		/// <summary>
		/// 专业人员id
		/// </summary>
		[Column("pju_user_id")]
		public  int? PjuUserId { get; set; }
		/// <summary>
		/// 专业人员name
		/// </summary>
		[Column("pju_user_name")]
		public  string PjuUserName { get; set; }
		/// <summary>
		/// 核减金额：作业
		/// </summary>
		[Column("pju_work_hj_amount")]
		public  decimal? PjuWorkHjAmount { get; set; }
		/// <summary>
		/// 核增金额：作业
		/// </summary>
		[Column("pju_work_hz_amount")]
		public  decimal? PjuWorkHzAmount { get; set; }
		/// <summary>
		/// 工作范围
		/// </summary>
		[Column("pju_work_scope")]
		public  string PjuWorkScope { get; set; }
		/// <summary>
		/// 审定金额：作业
		/// </summary>
		[Column("pju_work_sd_amount")]
		public  decimal? PjuWorkSdAmount { get; set; }
		/// <summary>
		/// 文印复核打回次数
		/// </summary>
		[Column("pju_wy_fh_back_num")]
		public  int? PjuWyFhBackNum { get; set; }
		/// <summary>
		/// 文字复核打回次数
		/// </summary>
		[Column("pju_wz_fh_back_num")]
		public  int? PjuWzFhBackNum { get; set; }
		/// <summary>
		/// 核减金额：终稿
		/// </summary>
		[Column("pju_zg_hj_amount")]
		public  decimal? PjuZgHjAmount { get; set; }
		/// <summary>
		/// 核增金额：终稿
		/// </summary>
		[Column("pju_zg_hz_amount")]
		public  decimal? PjuZgHzAmount { get; set; }
		/// <summary>
		/// 审定金额：终稿
		/// </summary>
		[Column("pju_zg_sd_amount")]
		public  decimal? PjuZgSdAmount { get; set; }
		/// <summary>
		/// 终稿完成时间
		/// </summary>
		[Column("pju_zg_wc_time")]
		public  DateTime? PjuZgWcTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("update_time")]
		public  DateTime? UpdateTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("update_user_id")]
		public  int? UpdateUserId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("update_user_name")]
		public  string UpdateUserName { get; set; }
	}

	/// <summary>
    /// 项目成员
    /// </summary>
	[Table("project_user")]
	public partial class ProjectUser : Entity
	{
		/// <summary>
		/// id
		/// </summary>
		[Key][Identity][Column("id")]
		public override int? Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("o_id")]
		public  string OId { get; set; }
		/// <summary>
		/// 文印复核人
		/// </summary>
		[Column("pd_wy_per")]
		public  string PdWyPer { get; set; }
		/// <summary>
		/// 文印复核人姓名
		/// </summary>
		[Column("pd_wy_per_name")]
		public  string PdWyPerName { get; set; }
		/// <summary>
		/// 文字复核人
		/// </summary>
		[Column("pd_wz_per")]
		public  string PdWzPer { get; set; }
		/// <summary>
		/// 文字复核人姓名
		/// </summary>
		[Column("pd_wz_per_name")]
		public  string PdWzPerName { get; set; }
		/// <summary>
		/// 用户id
		/// </summary>
		[Column("pu_au_id")]
		public  int? PuAuId { get; set; }
		/// <summary>
		/// 项目成员姓名
		/// </summary>
		[Column("pu_au_name")]
		public  string PuAuName { get; set; }
		/// <summary>
		/// 初稿完成时间
		/// </summary>
		[Column("pu_cg_time")]
		public  DateTime? PuCgTime { get; set; }
		/// <summary>
		/// 部门小组id
		/// </summary>
		[Column("pu_dep_group")]
		public  int? PuDepGroup { get; set; }
		/// <summary>
		/// 复核JSON
		/// </summary>
		[Column("pu_fh_json")]
		public  string PuFhJson { get; set; }
		/// <summary>
		/// 一级核减
		/// </summary>
		[Column("pu_first_hj")]
		public  decimal? PuFirstHj { get; set; }
		/// <summary>
		/// 一级核增
		/// </summary>
		[Column("pu_first_hz")]
		public  decimal? PuFirstHz { get; set; }
		/// <summary>
		/// 一级审定金额
		/// </summary>
		[Column("pu_first_sdprice")]
		public  decimal? PuFirstSdprice { get; set; }
		/// <summary>
		/// 一级预算价
		/// </summary>
		[Column("pu_first_ys_price")]
		public  decimal? PuFirstYsPrice { get; set; }
		/// <summary>
		/// 分数
		/// </summary>
		[Column("pu_grade")]
		public  float? PuGrade { get; set; }
		/// <summary>
		/// 是否个人业绩库
		/// </summary>
		[Column("pu_is_per_achievement")]
		public  int? PuIsPerAchievement { get; set; }
		/// <summary>
		/// 是否是项目经理（0不是1是）
		/// </summary>
		[Column("pu_is_promanager")]
		public  int? PuIsPromanager { get; set; }
		/// <summary>
		/// 是否是外聘人员
		/// </summary>
		[Column("pu_is_wp")]
		public  int? PuIsWp { get; set; }
		/// <summary>
		/// 是否为专业负责人
		/// </summary>
		[Column("pu_is_zy")]
		public  int? PuIsZy { get; set; }
		/// <summary>
		/// 部门ID
		/// </summary>
		[Column("pu_og_id")]
		public  int? PuOgId { get; set; }
		/// <summary>
		/// 项目部门id
		/// </summary>
		[Column("pu_pd_id")]
		public  int? PuPdId { get; set; }
		/// <summary>
		/// 项目id
		/// </summary>
		[Column("pu_pi_id")]
		public  int? PuPiId { get; set; }
		/// <summary>
		/// 二级复核待复核人
		/// </summary>
		[Column("pu_sec_fh")]
		public  string PuSecFh { get; set; }
		/// <summary>
		/// 二级复核待复核人姓名
		/// </summary>
		[Column("pu_sec_fh_name")]
		public  string PuSecFhName { get; set; }
		/// <summary>
		/// 二级复核核减
		/// </summary>
		[Column("pu_sec_fhhj")]
		public  decimal? PuSecFhhj { get; set; }
		/// <summary>
		/// 二级复核核增
		/// </summary>
		[Column("pu_sec_fhhz")]
		public  decimal? PuSecFhhz { get; set; }
		/// <summary>
		/// 二级复核已扣款
		/// </summary>
		[Column("pu_sec_fhkk")]
		public  decimal? PuSecFhkk { get; set; }
		/// <summary>
		/// 二级复核扣分
		/// </summary>
		[Column("pu_sec_grade")]
		public  float? PuSecGrade { get; set; }
		/// <summary>
		/// 二级核减
		/// </summary>
		[Column("pu_sec_hj")]
		public  decimal? PuSecHj { get; set; }
		/// <summary>
		/// 二级核增
		/// </summary>
		[Column("pu_sec_hz")]
		public  decimal? PuSecHz { get; set; }
		/// <summary>
		/// 二级复核扣款数量
		/// </summary>
		[Column("pu_sec_kk_num")]
		public  int? PuSecKkNum { get; set; }
		/// <summary>
		/// 二级复核人id
		/// </summary>
		[Column("pu_sec_per")]
		public  int? PuSecPer { get; set; }
		/// <summary>
		/// 二级审定金额
		/// </summary>
		[Column("pu_sec_sdprice")]
		public  decimal? PuSecSdprice { get; set; }
		/// <summary>
		/// 二级复核提交时间
		/// </summary>
		[Column("pu_sec_submit_time")]
		public  DateTime? PuSecSubmitTime { get; set; }
		/// <summary>
		/// 二级复核统计数量
		/// </summary>
		[Column("pu_sec_tj_num")]
		public  int? PuSecTjNum { get; set; }
		/// <summary>
		/// 二级预算价
		/// </summary>
		[Column("pu_sec_ys_price")]
		public  decimal? PuSecYsPrice { get; set; }
		/// <summary>
		/// 二级复核打回次数
		/// </summary>
		[Column("pu_secback_num")]
		public  int? PuSecbackNum { get; set; }
		/// <summary>
		/// 实际用章人json
		/// </summary>
		[Column("pu_sjyz_json")]
		public  string PuSjyzJson { get; set; }
		/// <summary>
		/// 实际用章人id
		/// </summary>
		[Column("pu_sjyz_per")]
		public  int? PuSjyzPer { get; set; }
		/// <summary>
		/// 实际用章人姓名
		/// </summary>
		[Column("pu_sjyz_per_name")]
		public  string PuSjyzPerName { get; set; }
		/// <summary>
		/// 送审金额
		/// </summary>
		[Column("pu_ss_price")]
		public  decimal? PuSsPrice { get; set; }
		/// <summary>
		/// 状态
		/// </summary>
		[Column("pu_state")]
		public  int? PuState { get; set; }
		/// <summary>
		/// 广联达模板权限
		/// </summary>
		[Column("pu_tempscope")]
		public  string PuTempscope { get; set; }
		/// <summary>
		/// 三级复核待复核人
		/// </summary>
		[Column("pu_th_fh")]
		public  string PuThFh { get; set; }
		/// <summary>
		/// 三级复核待复核人姓名
		/// </summary>
		[Column("pu_th_fh_name")]
		public  string PuThFhName { get; set; }
		/// <summary>
		/// 三级复核核减
		/// </summary>
		[Column("pu_th_fhhj")]
		public  decimal? PuThFhhj { get; set; }
		/// <summary>
		/// 三级复核核增
		/// </summary>
		[Column("pu_th_fhhz")]
		public  decimal? PuThFhhz { get; set; }
		/// <summary>
		/// 三级复核已扣款
		/// </summary>
		[Column("pu_th_fhkk")]
		public  decimal? PuThFhkk { get; set; }
		/// <summary>
		/// 三级复核扣分
		/// </summary>
		[Column("pu_th_grade")]
		public  float? PuThGrade { get; set; }
		/// <summary>
		/// 三级核减
		/// </summary>
		[Column("pu_th_hj")]
		public  decimal? PuThHj { get; set; }
		/// <summary>
		/// 三级核增
		/// </summary>
		[Column("pu_th_hz")]
		public  decimal? PuThHz { get; set; }
		/// <summary>
		/// 三级复核扣款数量
		/// </summary>
		[Column("pu_th_kk_num")]
		public  int? PuThKkNum { get; set; }
		/// <summary>
		/// 三级复核人id
		/// </summary>
		[Column("pu_th_per")]
		public  int? PuThPer { get; set; }
		/// <summary>
		/// 三级审定金额
		/// </summary>
		[Column("pu_th_sdprice")]
		public  decimal? PuThSdprice { get; set; }
		/// <summary>
		/// 三级复核统计数量
		/// </summary>
		[Column("pu_th_tj_num")]
		public  int? PuThTjNum { get; set; }
		/// <summary>
		/// 三级预算价
		/// </summary>
		[Column("pu_th_ys_price")]
		public  decimal? PuThYsPrice { get; set; }
		/// <summary>
		/// 三级复核打回次数
		/// </summary>
		[Column("pu_thback_num")]
		public  int? PuThbackNum { get; set; }
		/// <summary>
		/// 证书编号
		/// </summary>
		[Column("pu_uc_code")]
		public  string PuUcCode { get; set; }
		/// <summary>
		/// 证书记录id
		/// </summary>
		[Column("pu_uc_id")]
		public  int? PuUcId { get; set; }
		/// <summary>
		/// 专业类型
		/// </summary>
		[Column("pu_uc_major")]
		public  string PuUcMajor { get; set; }
		/// <summary>
		/// 证书类型
		/// </summary>
		[Column("pu_uc_type")]
		public  int? PuUcType { get; set; }
		/// <summary>
		/// 工作范围
		/// </summary>
		[Column("pu_works")]
		public  string PuWorks { get; set; }
		/// <summary>
		/// 外聘人员姓名
		/// </summary>
		[Column("pu_wp_name")]
		public  string PuWpName { get; set; }
		/// <summary>
		/// 文字复核扣分
		/// </summary>
		[Column("pu_wz_grade")]
		public  float? PuWzGrade { get; set; }
		/// <summary>
		/// 文字复核统计数量
		/// </summary>
		[Column("pu_wz_tj_num")]
		public  int? PuWzTjNum { get; set; }
		/// <summary>
		/// 文字复核打回次数
		/// </summary>
		[Column("pu_wzback_num")]
		public  int? PuWzbackNum { get; set; }
		/// <summary>
		/// 文字复核人id
		/// </summary>
		[Column("pu_wzfh_per")]
		public  int? PuWzfhPer { get; set; }
		/// <summary>
		/// 预算价
		/// </summary>
		[Column("pu_ys_price")]
		public  decimal? PuYsPrice { get; set; }
		/// <summary>
		/// 终稿完成时间
		/// </summary>
		[Column("pu_zg_time")]
		public  DateTime? PuZgTime { get; set; }
		/// <summary>
		/// 自检JSON
		/// </summary>
		[Column("pu_zj_json")]
		public  string PuZjJson { get; set; }
		/// <summary>
		/// 作业专业id
		/// </summary>
		[Column("pu_zy_id")]
		public  int? PuZyId { get; set; }
	}

	/// <summary>
    /// VIEW
    /// </summary>
	[Table("v_project_info_union")]
	public partial class VProjectInfoUnion : Entity
	{
		/// <summary>
		/// 
		/// </summary>
		[Column("a_code")]
		public  string ACode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("a_name")]
		public  string AName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("c_code")]
		public  string CCode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("c_name")]
		public  string CName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("create_time")]
		public  DateTime? CreateTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("create_user_id")]
		public  int? CreateUserId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("create_user_name")]
		public  string CreateUserName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("id")]
		public override int? Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("p_code")]
		public  string PCode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("p_name")]
		public  string PName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_address")]
		public  string PiAddress { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_belong_dep")]
		public  int? PiBelongDep { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_bg_no")]
		public  string PiBgNo { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_bgh_time")]
		public  DateTime? PiBghTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_bgwc_time")]
		public  DateTime? PiBgwcTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_bt_name")]
		public  string PiBtName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_bt_sec_name")]
		public  string PiBtSecName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_bt_stage")]
		public  int? PiBtStage { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_bt_type")]
		public  int? PiBtType { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_btid")]
		public  int? PiBtid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_cgwc_time")]
		public  DateTime? PiCgwcTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_create_lc_type")]
		public  int? PiCreateLcType { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_ct_id")]
		public  int? PiCtId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_ctp_id")]
		public  int? PiCtpId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_dep")]
		public  int? PiDep { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_dep_group")]
		public  int? PiDepGroup { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_dep_lead")]
		public  int? PiDepLead { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_dep_name")]
		public  string PiDepName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_gd_endtime")]
		public  DateTime? PiGdEndtime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_gdwc_time")]
		public  DateTime? PiGdwcTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_hj")]
		public  decimal? PiHj { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_hz")]
		public  decimal? PiHz { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_is_cancel")]
		public  long? PiIsCancel { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_is_compamy_achievement")]
		public  int? PiIsCompamyAchievement { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_is_gd_yq")]
		public  long? PiIsGdYq { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_is_group")]
		public  int? PiIsGroup { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_is_his_legacy")]
		public  int? PiIsHisLegacy { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_is_per_achievement")]
		public  int? PiIsPerAchievement { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_is_simple")]
		public  long? PiIsSimple { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_is_th")]
		public  int? PiIsTh { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_is_wb_fh_new")]
		public  int? PiIsWbFhNew { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_js_name")]
		public  string PiJsName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_lead_name")]
		public  string PiLeadName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_name")]
		public  string PiName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_pm_id")]
		public  int? PiPmId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_pm_name")]
		public  string PiPmName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_qf_per_id")]
		public  string PiQfPerId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_qf_time")]
		public  DateTime? PiQfTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_sd_money")]
		public  decimal? PiSdMoney { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_sec_btid")]
		public  int? PiSecBtid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_secwc_time")]
		public  DateTime? PiSecwcTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_sg_name")]
		public  string PiSgName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_sjgz_time")]
		public  DateTime? PiSjgzTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_ss_money")]
		public  decimal? PiSsMoney { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_start_time")]
		public  DateTime? PiStartTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_state")]
		public  int? PiState { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_thfhfp_per")]
		public  int? PiThfhfpPer { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_thwc_time")]
		public  DateTime? PiThwcTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_union_state")]
		public  long? PiUnionState { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_wt_name")]
		public  string PiWtName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_wzfhtg_time")]
		public  DateTime? PiWzfhtgTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_yqtzg_time")]
		public  DateTime? PiYqtzgTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pi_ys_price")]
		public  decimal? PiYsPrice { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("re_type")]
		public  long? ReType { get; set; }
	}

	/// <summary>
    /// VIEW
    /// </summary>
	[Table("v_project_user_union")]
	public partial class VProjectUserUnion : Entity
	{
		/// <summary>
		/// 
		/// </summary>
		[Column("id")]
		public override int? Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_au_id")]
		public  int? PuAuId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_au_name")]
		public  string PuAuName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_cg_time")]
		public  DateTime? PuCgTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_dep_id")]
		public  int? PuDepId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_is_promanager")]
		public  int? PuIsPromanager { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_og_id")]
		public  int? PuOgId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_pi_id")]
		public  int? PuPiId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_sd_price")]
		public  decimal? PuSdPrice { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_sjyz_json")]
		public  string PuSjyzJson { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_ss_price")]
		public  decimal? PuSsPrice { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_state")]
		public  int? PuState { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_ys_price")]
		public  decimal? PuYsPrice { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("pu_zy_id")]
		public  int? PuZyId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("re_type")]
		public  long? ReType { get; set; }
	}

	/// <summary>
    /// View 'test.v_student' references invalid table(s) or column(s) or function(s) or definer/invoker of view lack rights to use them
    /// </summary>
	[Table("v_student")]
	public partial class VStudent : Entity
	{
	}
}

