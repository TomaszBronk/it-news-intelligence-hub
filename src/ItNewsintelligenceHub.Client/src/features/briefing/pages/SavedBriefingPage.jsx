import { useBriefing } from "../hooks/useBriefing";
import { NewsItemsList } from "../../news-items/components/NewsItemsList";
import { LoadingSpinner } from "../../../shared/components/LoadingSpinner";

export function SavedBriefingPage() {
    const { data: items, isLoading, error } = useBriefing("saved");

    if (isLoading) {
        return <LoadingSpinner />;
    }

    if (error) {
        return (
            <div className="text-red-600">
                Nie udało się pobrać zapisanych newsów.
            </div>
        );
    }

    if (!items || items.length === 0) {
        return (
            <div className="text-gray-600">
                Brak zapisanych newsów.
            </div>
        );
    }

    return (
        <div className="space-y-4">
            <h1 className="text-2xl font-bold">Saved Briefings</h1>
            <NewsItemsList items={items} />
        </div>
    );
}