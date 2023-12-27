﻿using Kitchen;
using KitchenData;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace KitchenRestoreFranchise
{
    public class RestoreFranchisePopup : GenericChoicePopupManager, IModSystem
    {
        public override PopupType ManagedType => Main.POPUP_TYPE_RESTORE_FRANCHISE;

        private EntityQuery Unlocks;

        protected override void Initialise()
        {
            base.Initialise();
            Unlocks = GetEntityQuery(typeof(CProgressionUnlock));
        }

        protected override bool HandleDecision(Entity popup, GenericChoiceDecision decision)
        {
            if (decision == GenericChoiceDecision.Accept)
            {
                if (CreateFranchise() && Require(out SSelectedLocation selectedLocation))
                {
                    Persistence.FullWorld.Clear(selectedLocation.Selected.Slot);
                    StartSceneTransition(SceneType.Franchise);
                }
            }
            return true;
        }

        public override Entity CreateNewPopup(Entity request)
        {
            return base.PopupUtilities.CreateGenericPopup(GenericChoiceType.AcceptOrCancel, ManagedType, PopupLocation.Centre);
        }

        private bool CreateFranchise()
        {
            if (!Require(out CFranchiseTier cFranchiseTier) || cFranchiseTier.Tier == 0)
                return false;
            Entity entity = EntityManager.CreateEntity(typeof(CFranchiseItem), typeof(CFranchiseTier), typeof(CPersistThroughSceneChanges));
            Set(entity, new CPersistentItem
            {
                ItemID = AssetReference.FranchiseCardSet,
                Type = PersistentStorageType.FranchiseCardSet
            });
            base.EntityManager.SetComponentData(entity, new CFranchiseTier
            {
                Tier = cFranchiseTier.Tier
            });
            DataObjectList cards = default(DataObjectList);
            NativeArray<CProgressionUnlock> unlocks = Unlocks.ToComponentDataArray<CProgressionUnlock>(Allocator.Temp);
            for (int i = 0; i < unlocks.Length; i++)
            {
                if (unlocks[i].FromFranchise)
                {
                    cards.Add(unlocks[i].ID);
                }
            }
            base.EntityManager.SetComponentData(entity, new CFranchiseItem
            {
                Name = Require(out CRenameRestaurant cRenameRestaurant) ? cRenameRestaurant.Name : "Restaurant",
                Cards = cards
            });
            return true;
        }
    }
}
