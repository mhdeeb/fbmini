export class User {
  userName?: string;
  email?: string;
  bio?: string;
  phoneNumber?: string;
  picture?: any;
  cover?: any;
}

export class UserView {
  userName?: string;
  email?: string;
  bio?: string;
  phoneNumber?: string;
}

export type PostView = {
  id: number;
  title?: string;
  content?: string;
  date: Date;
  attachment: {
    filename: string;
    contentType: string;
    size: number;
    fileData: string;
  };
  parent_post?: number;
  poster: {
    userName: string;
    picture: { contentType: string; fileData: string };
  };
  likes: number;
  dislikes: number;
  vote: number;
  subPostsIds: number[];
};

export type VoteView = {
  likes: number;
  dislikes: number;
  vote: number;
};
