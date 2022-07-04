export interface Vote
{
    _id: string;
    authorId: string;
    votedAt: string;
}

export type UpvoteVote = Vote;

export interface SimpleVote
{
    rating: number;
}

export interface CategoriesVote
{
    ratedCategories: Record<string, number>;
}
