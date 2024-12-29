export class User {
  userName?: string;
  email?: string;
  bio?: string;
  phoneNumber?: string;
  pictureUrl?: string;
  coverUrl?: string;
  isOwner?: boolean;
}

export class UserView {
  email?: string;
  bio?: string;
  phoneNumber?: string;
  picture?: File;
  cover?: File;
}

export type PostView = {
  id: number;
  title: string;
  content?: string;
  date: string;
  attachmentUrl: string;
  poster: {
    userName: string;
    pictureUrl: string;
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
