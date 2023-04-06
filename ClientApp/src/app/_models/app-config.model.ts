export interface IAppConfig {

  ApiServer: {
    url: string;
    
  };
  AdminUsers: [
    string
  ];

  WfStatePrinted: {
    state: string
  };
  WfStateDelivered: {
    state: string
  };


  PageSizeOptions: [
    number
  ];
}
