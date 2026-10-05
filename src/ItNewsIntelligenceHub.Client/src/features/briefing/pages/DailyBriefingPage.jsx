import { useBriefing } from "../hooks/useBriefing";
import { NewsItemsList } from "../../news-items/components/NewsItemsList";
import { LoadingSpinner } from "../../../shared/components/LoadingSpinner";

export function DailyBriefingPage() {
    const { data: items, isLoading, error } = useBriefing("daily");

    if (isLoading) {
        return <LoadingSpinner />;
    }

    if (error) {
        return (
            <div className="text-red-600">
                Nie udało się pobrać Daily Briefingu.
            </div>
        );
    }

    if (!items || items.length === 0) {
        return (
            <div className="text-gray-600">
                Brak newsów z ostatnich 24 godzin.
            </div>
        );
    }

    return (
        <div className="space-y-4">
            <h1 className="text-2xl font-bold">Daily Briefing</h1>
            <NewsItemsList items={items} />
        </div>
    );
}